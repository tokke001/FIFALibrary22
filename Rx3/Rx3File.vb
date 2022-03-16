Imports System.Drawing
Imports Microsoft.DirectX.Direct3D
Imports FIFALibrary22.Rw.Core.Arena
'imports FIFALibrary22

'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/
Namespace Rx3
    Partial Public Class Rx3File

        ' Methods
        'Public Function ConvertKit(ByVal sourceRx3File As Rx3File) As Boolean
        'Return If(Not Me.IsFifa12, Me.ConvertKitFrom12(sourceRx3File), Me.ConvertKitFrom11(sourceRx3File))
        'End Function

        'Public Function ConvertKitFrom11(ByVal sourceRx3File As Rx3File) As Boolean
        'Return If(Not Me.IsFifa11, Not sourceRx3File.IsFifa12, False)
        'End Function

        'Public Function ConvertKitFrom12(ByVal sourceRx3File As Rx3File) As Boolean
        'Return If(Not Me.IsFifa12, Not sourceRx3File.IsFifa11, False)
        'End Function

        Public Function Load(ByVal fifaFile As FifaFile) As Boolean
            If fifaFile.IsCompressed Then
                fifaFile.Decompress()
            End If
            Dim r As FileReader = fifaFile.GetReader
            Dim flag As Boolean = Me.Load(r)
            fifaFile.ReleaseReader(r)

            Return flag
        End Function

        Public Overridable Function Load(ByVal r As FileReader) As Boolean
            Me.FileSize = r.BaseStream.Length

            Dim str As New String(r.ReadChars(4))
            r.BaseStream.Position = r.BaseStream.Position - 4
            If (str.Contains("RW4")) Then   'FIFA 11
                'Me.m_IsFifa11 = True
                Me.RW4Section = New Arena(r)
                r.BaseStream.Position = Me.RW4Section.ResourceDescriptor.BaseResourceDescriptors(0).Size

                If r.BaseStream.Position + 4 <= r.BaseStream.Length - 1 Then
                    str = New String(r.ReadChars(4))
                    r.BaseStream.Position = r.BaseStream.Position - 4
                End If
                'Else
                'Me.m_IsFifa11 = False
            End If

            If (str.StartsWith("RX3")) Then
                Me.Rx3Section = New Rx3FileRx3Section(Me.RW4Section, r)
            End If

            Return True
        End Function

        Public Function Load(ByVal FileName As String) As Boolean
            Dim f As New FileStream(FileName, FileMode.Open, FileAccess.ReadWrite)
            Dim r As New FileReader(f, Endian.Little)
            Dim flag As Boolean = Me.Load(r)
            f.Close()
            r.Close()
            Return flag
        End Function

        Public Overridable Function Save(ByVal w As FileWriter) As Boolean

            '1 - RW4 Section (if present)
            If Me.RW4Section IsNot Nothing Then
                'Me.m_IsFifa11 = True
                '1.1 - Copy Values present at RX3 section to RW4 section (vertex, index , ... datas)
                Me.SetRW4Values()
                '1.2 - Save RW4 section
                'If HasUnknownRW4Sections(Me.RW4Section.RW4SectionInfos) = False Then   '--> all sections have been added as generic byte arrays !
                Me.RW4Section.Save(w)
                'End If
                w.BaseStream.Position = Me.RW4Section.ResourceDescriptor.BaseResourceDescriptors(0).Size
                'Else
                'Me.m_IsFifa11 = False
            End If

            '2 - Rx3 Section (if present)
            If Me.Rx3Section IsNot Nothing Then
                '2.1 - Save Rx3 section
                Me.Rx3Section.Save(w)
            End If

            Return True
        End Function

        Public Function Save(ByVal fileName As String) As Boolean
            Dim output As New FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)
            Dim w As New FileWriter(output, Endian.Little)
            Dim flag As Boolean = Me.Save(w)
            output.Close()
            w.Close()

            Return flag
        End Function

        Private Sub SetRW4Values()

            'vertex buffers
            If Me.Rx3Section.Sections.VertexBuffers IsNot Nothing Then
                If Me.RW4Section.Sections.VertexBuffers.Count = Me.Rx3Section.Sections.VertexBuffers.Count Then
                    For i = 0 To Me.RW4Section.Sections.VertexBuffers.Count - 1

                        Me.RW4Section.Sections.VertexBuffers(i).VertexStride = CUShort(Me.Rx3Section.Sections.VertexBuffers(i).VertexStride)
                        Me.RW4Section.Sections.VertexBuffers(i).NumVertices = CUInt(Me.Rx3Section.Sections.VertexBuffers(i).VertexData.Length)

                        Dim Tmp_VertexSize As UInteger = (Me.RW4Section.Sections.VertexBuffers(i).VertexStride * Me.RW4Section.Sections.VertexBuffers(i).NumVertices)   'always vertex buffer size + 2
                        Do While Tmp_VertexSize Mod 32 <> 0   'calculate padding: always allignment 32 !
                            Tmp_VertexSize += 1
                        Loop
                        Me.RW4Section.Sections.VertexBuffers(i).D3dVertexBuffer.Format.SizeVertexBuffer = Tmp_VertexSize

                        'RW4 mesh
                        If Me.RW4Section.Sections.EmbeddedMeshes.Count = Me.Rx3Section.Sections.VertexBuffers.Count Then
                            Me.RW4Section.Sections.EmbeddedMeshes(i).NumVertices = Me.RW4Section.Sections.VertexBuffers(i).NumVertices
                        End If

                    Next i
                End If
            End If

            'index buffers
            If Me.Rx3Section.Sections.IndexBuffers IsNot Nothing Then
                If Me.RW4Section.Sections.IndexBuffers.Count = Me.Rx3Section.Sections.IndexBuffers.Count Then
                    For i = 0 To Me.RW4Section.Sections.IndexBuffers.Count - 1

                        Me.RW4Section.Sections.IndexBuffers(i).NumIndices = CUInt(Me.Rx3Section.Sections.IndexBuffers(i).IndexData.Length)

                        If Me.RW4Section.Sections.IndexBuffers(i).NumIndices > UShort.MaxValue Then
                            Me.RW4Section.Sections.IndexBuffers(i).IndexStride = 4 'RWIndexFormat.FMT_INDEX32
                        Else
                            Me.RW4Section.Sections.IndexBuffers(i).IndexStride = 2 'RWIndexFormat.FMT_INDEX16
                        End If

                        Me.RW4Section.Sections.IndexBuffers(i).D3dIndexBuffer.SizeIndexBuffer = Me.RW4Section.Sections.IndexBuffers(i).IndexStride * Me.RW4Section.Sections.IndexBuffers(i).NumIndices
                        Do While Me.RW4Section.Sections.IndexBuffers(i).D3dIndexBuffer.SizeIndexBuffer Mod 16 <> 0   'calculate padding
                            Me.RW4Section.Sections.IndexBuffers(i).D3dIndexBuffer.SizeIndexBuffer += 1
                        Loop

                        'RW4 mesh
                        If Me.RW4Section.Sections.EmbeddedMeshes.Count = Me.Rx3Section.Sections.IndexBuffers.Count Then
                            Me.RW4Section.Sections.EmbeddedMeshes(i).NumIndices = Me.RW4Section.Sections.IndexBuffers(i).NumIndices
                        End If
                    Next i
                End If
            End If

            'raster (textures)
            If Me.Rx3Section.Sections.Textures IsNot Nothing Then
                If Me.RW4Section.Sections.Rasters.Count = Me.Rx3Section.Sections.Textures.Count Then
                    For i = 0 To Me.RW4Section.Sections.Rasters.Count - 1

                        Me.RW4Section.Sections.Rasters(i).NumMipLevels = Me.Rx3Section.Sections.Textures(i).Rx3TextureHeader.NumMipLevels
                        Me.RW4Section.Sections.Rasters(i).D3d.Format.TextureFormat = GraphicUtil.GetRWFromRx3TextureFormat(Me.Rx3Section.Sections.Textures(i).Rx3TextureHeader.TextureFormat)
                        Me.RW4Section.Sections.Rasters(i).D3d.Format.Height = Me.Rx3Section.Sections.Textures(i).Rx3TextureHeader.Height
                        Me.RW4Section.Sections.Rasters(i).D3d.Format.Width = Me.Rx3Section.Sections.Textures(i).Rx3TextureHeader.Width
                        Me.RW4Section.Sections.Rasters(i).D3d.Format.Depth = Me.Rx3Section.Sections.Textures(i).Rx3TextureHeader.NumFaces
                        'Me.RW4Section.Sections.Raster(i).D3d.Format.Pitch = need formula ?
                        'Me.RW4Section.Sections.Raster(i).D3d.Format.Endian = 
                        'Me.RW4Section.Sections.Raster(i).D3d.Format.Tiled = 

                    Next
                End If
            End If

        End Sub

        Public Property Bitmaps As List(Of Bitmap)
            Get
                If Me.Rx3Section IsNot Nothing Then
                    Return Me.Rx3Section.Bitmaps
                Else
                    Return Nothing
                End If
            End Get
            Set
                If Me.Rx3Section IsNot Nothing Then
                    Me.Rx3Section.Bitmaps = Value
                End If
            End Set
        End Property

        Public Property DDSTextures As List(Of DDSFile)
            Get
                If Me.Rx3Section IsNot Nothing Then
                    Return Me.Rx3Section.DDSTextures
                Else
                    Return Nothing
                End If
            End Get
            Set
                If Me.Rx3Section IsNot Nothing Then
                    Me.Rx3Section.DDSTextures = Value
                End If
            End Set
        End Property

        Public Property VertexStreams As List(Of Vertex())
            Get
                If Me.Rx3Section IsNot Nothing Then
                    Return Me.Rx3Section.VertexStreams
                Else
                    Return Nothing
                End If
            End Get
            Set
                If Me.Rx3Section IsNot Nothing Then
                    Me.Rx3Section.VertexStreams = Value
                End If
            End Set
        End Property

        Public Property IndexStreams As List(Of UInteger())
            Get
                If Me.Rx3Section IsNot Nothing Then
                    Return Me.Rx3Section.IndexStreams
                Else
                    Return Nothing
                End If
            End Get
            Set
                If Me.Rx3Section IsNot Nothing Then
                    Me.Rx3Section.IndexStreams = Value
                End If
            End Set
        End Property

        Public Property PrimitiveTypes(ByVal MeshIndex As UInteger) As PrimitiveType
            Get
                If Me.RW4Section IsNot Nothing AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                    Return Me.RW4Section.PrimitiveTypes(MeshIndex)
                ElseIf Me.Rx3Section IsNot Nothing Then
                    Return Me.Rx3Section.PrimitiveTypes(MeshIndex)
                Else
                    Return Nothing
                End If
            End Get
            Set
                If Me.RW4Section IsNot Nothing AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                    Me.RW4Section.PrimitiveTypes(MeshIndex) = Value
                ElseIf Me.Rx3Section IsNot Nothing Then
                    Me.Rx3Section.PrimitiveTypes(MeshIndex) = Value
                End If
            End Set
        End Property

        Public Property PrimitiveTypes As List(Of PrimitiveType)
            Get
                If Me.RW4Section IsNot Nothing AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                    Return Me.RW4Section.PrimitiveTypes
                ElseIf Me.Rx3Section IsNot Nothing Then
                    Return Me.Rx3Section.PrimitiveTypes
                Else
                    Return Nothing
                End If
            End Get
            Set
                If Me.RW4Section IsNot Nothing AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                    Me.RW4Section.PrimitiveTypes = Value
                ElseIf Me.Rx3Section IsNot Nothing Then
                    Me.Rx3Section.PrimitiveTypes = Value
                End If
            End Set
        End Property

        Public ReadOnly Property NumMeshes As UInteger
            Get
                If Me.Rx3Section.Sections.VertexBuffers IsNot Nothing Then
                    Return Me.Rx3Section.Sections.VertexBuffers.Count
                End If

                Return 0
            End Get
        End Property

        Public ReadOnly Property NumTextures As UInteger
            Get
                If Me.Rx3Section.Sections.Textures IsNot Nothing Then
                    Return Me.Rx3Section.Sections.Textures.Count
                End If

                Return 0
            End Get
        End Property


        Public Property RW4Section As Arena = Nothing
        Public Property Rx3Section As Rx3FileRx3Section = Nothing

        'Private m_IsFifa11 As Boolean = False
        'Private m_FileName As String
        Public Property FileSize As Long

    End Class
End Namespace