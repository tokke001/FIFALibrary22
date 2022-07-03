Imports System.Drawing
Imports BCnEncoder.Shared
Imports FIFALibrary22.Rw.Core.Arena
Imports FIFALibrary22.Rx3
Imports Microsoft.DirectX.Direct3D

'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/

Public Class RxFile

    Public Sub New()
    End Sub

    Public Sub New(Optional Rw4Section As Arena = Nothing, Optional Rx3Section As Rx3.Rx3File = Nothing)
        Me.Rw4Section = Rw4Section
        Me.Rx3Section = Rx3Section
    End Sub

    Public Function Load(ByVal FifaFile As FifaFile) As Boolean
        If FifaFile.IsCompressed Then
            FifaFile.Decompress()
        End If
        Dim r As FileReader = FifaFile.GetReader
        Dim flag As Boolean = Me.Load(r)
        FifaFile.ReleaseReader(r)

        Return flag
    End Function

    Public Overridable Function Load(ByVal r As FileReader) As Boolean
        Me.FileSize = r.BaseStream.Length

        Dim str As New String(r.ReadChars(4))
        r.BaseStream.Position = r.BaseStream.Position - 4
        If (str.Contains("RW4")) Then   'FIFA 11
            Me.Rw4Section = New Arena(r)
            r.BaseStream.Position = Me.Rw4Section.ResourceDescriptor.BaseResourceDescriptors(0).Size

            If r.BaseStream.Position + 4 <= r.BaseStream.Length - 1 Then
                str = New String(r.ReadChars(4))
                r.BaseStream.Position = r.BaseStream.Position - 4
            End If
        End If

        If (str.StartsWith("RX3")) Then
            Me.Rx3Section = New Rx3.Rx3File(Me.Rw4Section, r)
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
        If Me.Rw4Section IsNot Nothing Then
            If Me.Rx3Section Is Nothing Then
                Me.Rw4Section.UseRwBuffers = True
            Else
                Me.Rw4Section.UseRwBuffers = False
                '1.1 - Copy Values present at RX3 section to RW4 section (vertex, index , ... datas)
                Me.SetRw4Values()
            End If
            '1.2 - Save RW4 section
            Me.Rw4Section.Save(w)
            w.BaseStream.Position = Me.Rw4Section.ResourceDescriptor.BaseResourceDescriptors(0).Size
        End If

        '2 - Rx3 Section (if present)
        If Me.Rx3Section IsNot Nothing Then
            '2.1 - Save Rx3 section
            Me.Rx3Section.Save(w)
        End If

        Return True
    End Function

    Public Function Save(ByVal FileName As String) As Boolean
        Dim output As New FileStream(FileName, FileMode.Create, FileAccess.ReadWrite)
        Dim w As New FileWriter(output, Endian.Little)
        Dim flag As Boolean = Me.Save(w)
        output.Close()
        w.Close()

        Return flag
    End Function

    Private Sub SetRw4Values()
        If Me.Rx3Section.Sections.VertexBuffers IsNot Nothing And Me.Rx3Section.Sections.IndexBuffers IsNot Nothing Then
            If Me.Rw4Section.Sections.EmbeddedMeshes.Count = Me.Rx3Section.Sections.VertexBuffers.Count Then
                For i = 0 To Me.Rw4Section.Sections.EmbeddedMeshes.Count - 1
                    'vertex buffers
                    Me.Rw4Section.Sections.EmbeddedMeshes(i).PVertexBuffers(0).SetValues(Me.Rx3Section.Sections.VertexBuffers(i).VertexData.Count, Me.Rx3Section.Sections.VertexBuffers(i).VertexStride)
                    'index buffers
                    Me.Rw4Section.Sections.EmbeddedMeshes(i).PIndexBuffer.SetValues(Me.Rx3Section.Sections.IndexBuffers(i).IndexData.Count, FifaUtil.GetIndexStride(Me.Rx3Section.Sections.IndexBuffers(i).IndexData))
                    'EmbeddedMeshes
                    Me.Rw4Section.Sections.EmbeddedMeshes(i).NumVertices = Me.Rw4Section.Sections.EmbeddedMeshes(i).PVertexBuffers(0).NumVertices
                    Me.Rw4Section.Sections.EmbeddedMeshes(i).NumIndices = Me.Rw4Section.Sections.EmbeddedMeshes(i).PIndexBuffer.NumIndices
                Next
            End If
        End If

        'raster (textures)
        If Me.Rx3Section.Sections.Textures IsNot Nothing Then
            If Me.Rw4Section.Sections.Rasters.Count = Me.Rx3Section.Sections.Textures.Count Then
                For i = 0 To Me.Rw4Section.Sections.Rasters.Count - 1
                    Me.Rw4Section.Sections.Rasters(i).SetValues(Me.Rx3Section.Sections.Textures(i).Header.NumFaces, Me.Rx3Section.Sections.Textures(i).Header.NumMipLevels, Me.Rx3Section.Sections.Textures(i).Header.Width, Me.Rx3Section.Sections.Textures(i).Header.Height, Me.Rx3Section.Sections.Textures(i).Header.TextureFormat.ToRwSurfaceFormat)
                Next
            End If
        End If
    End Sub

    Public Property Bitmaps As List(Of Bitmap)
        Get
            If Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.Bitmaps
            ElseIf Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.Bitmaps
            Else
                Return Nothing ' New List(Of Bitmap)
            End If
        End Get
        Set
            If Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.Bitmaps = Value
            ElseIf Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.Bitmaps = Value
            End If
        End Set
    End Property

    Public Property DdsTextures As List(Of DdsFile)
        Get
            If Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.DdsTextures
            ElseIf Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.DdsTextures
            Else
                Return Nothing ' New List(Of DdsFile)
            End If
        End Get
        Set
            If Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.DdsTextures = Value
            ElseIf Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.DdsTextures = Value
            End If
        End Set
    End Property

    Public Property KtxTextures As List(Of KtxFile)
        Get
            If Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.KtxTextures
            ElseIf Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.KtxTextures
            Else
                Return Nothing ' New List(Of DdsFile)
            End If
        End Get
        Set
            If Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.KtxTextures = Value
            ElseIf Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.KtxTextures = Value
            End If
        End Set
    End Property

    Public Property VertexStreams As List(Of List(Of Vertex))
        Get
            If Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.VertexStreams
            ElseIf Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.VertexStreams
            Else
                Return Nothing ' New List(Of List(Of Vertex))
            End If
        End Get
        Set
            If Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.VertexStreams = Value
            ElseIf Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.VertexStreams = Value
            End If
        End Set
    End Property

    Public Property IndexStreams As List(Of List(Of UInteger))
        Get
            If Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.IndexStreams
            ElseIf Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.IndexStreams
            Else
                Return Nothing ' New List(Of List(Of UInteger))
            End If
        End Get
        Set
            If Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.IndexStreams = Value
            ElseIf Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.IndexStreams = Value
            End If
        End Set
    End Property

    Public Property PrimitiveTypes(ByVal MeshIndex As UInteger) As PrimitiveType
        Get
            If Me.Rw4Section IsNot Nothing Then 'AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                Return Me.Rw4Section.PrimitiveTypes(MeshIndex)
            ElseIf Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.PrimitiveTypes(MeshIndex)
            Else
                Return Nothing
            End If
        End Get
        Set
            If Me.Rw4Section IsNot Nothing Then 'AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                Me.Rw4Section.PrimitiveTypes(MeshIndex) = Value
            ElseIf Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.PrimitiveTypes(MeshIndex) = Value
            End If
        End Set
    End Property

    Public Property PrimitiveTypes As List(Of PrimitiveType)
        Get
            If Me.Rw4Section IsNot Nothing Then 'AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                Return Me.Rw4Section.PrimitiveTypes
            ElseIf Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.PrimitiveTypes
            Else
                Return Nothing
            End If
        End Get
        Set
            If Me.Rw4Section IsNot Nothing Then 'AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                Me.Rw4Section.PrimitiveTypes = Value
            ElseIf Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.PrimitiveTypes = Value
            End If
        End Set
    End Property

    Public Property VertexFormats(ByVal MeshIndex As UInteger) As VertexElement()
        Get
            If Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.VertexFormats(MeshIndex)
            ElseIf Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.VertexFormats(MeshIndex)
            Else
                Return Nothing
            End If

        End Get
        Set
            If Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.VertexFormats(MeshIndex) = Value
            ElseIf Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.VertexFormats(MeshIndex) = Value
            End If
        End Set
    End Property

    Public Property VertexFormats As List(Of VertexElement())
        Get
            If Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.VertexFormats
            ElseIf Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.VertexFormats
            Else
                Return Nothing
            End If
        End Get
        Set
            If Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.VertexFormats = Value
            ElseIf Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.VertexFormats = Value
            End If
        End Set
    End Property

    Public ReadOnly Property NameTable As List(Of FIFALibrary22.NameTable)
        Get
            If Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.NameTable
            ElseIf Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.NameTable
            Else
                Return Nothing
            End If
        End Get
        'Set
        '     = Value
        'End Set
    End Property

    Public Property MeshName(ByVal MeshIndex As UInteger) As String
        Get
            If Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.MeshName(MeshIndex)
            ElseIf Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.MeshName(MeshIndex)
            Else
                Return Nothing
            End If
        End Get
        Set
            If Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.MeshName(MeshIndex) = Value
            ElseIf Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.MeshName(MeshIndex) = Value
            End If
        End Set
    End Property

    Public Property TextureName(ByVal TextureIndex As UInteger) As String
        Get
            If Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.TextureName(TextureIndex)
            ElseIf Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.TextureName(TextureIndex)
            Else
                Return Nothing
            End If
        End Get
        Set
            If Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.TextureName(TextureIndex) = Value
            ElseIf Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.TextureName(TextureIndex) = Value
            End If
        End Set
    End Property

    Public ReadOnly Property NumMeshes As UInteger
        Get
            If Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.NumMeshes
            ElseIf Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.NumMeshes
            End If

            Return 0
        End Get
    End Property

    Public ReadOnly Property NumTextures As UInteger
        Get
            If Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.NumTextures
            ElseIf Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.NumTextures
            End If

            Return 0
        End Get
    End Property

    Public Property BoneMatrices As List(Of List(Of BonePose))
        Get
            If Me.Rw4Section IsNot Nothing Then
                Return Me.Rw4Section.BoneMatrices
            ElseIf Me.Rx3Section IsNot Nothing Then
                Return Me.Rx3Section.BoneMatrices
            Else
                Return Nothing
            End If
        End Get
        Set
            If Me.Rw4Section IsNot Nothing Then
                Me.Rw4Section.BoneMatrices = Value
            ElseIf Me.Rx3Section IsNot Nothing Then
                Me.Rx3Section.BoneMatrices = Value
            End If
        End Set
    End Property

    Public Property Rw4Section As Arena = Nothing
    Public Property Rx3Section As Rx3.Rx3File = Nothing

    'Private m_IsFifa11 As Boolean = False
    Public Property FileSize As Long
End Class