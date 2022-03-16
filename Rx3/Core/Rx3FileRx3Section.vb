Imports System.Drawing
Imports Microsoft.DirectX.Direct3D
Imports FIFALibrary22.Rw.Core.Arena
'imports FIFALibrary22

'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/
Namespace Rx3
    Partial Public Class Rx3FileRx3Section

        Public Sub New(RW4Section As Arena, ByVal r As FileReader)
            Me.RW4Section = RW4Section
            Me.Load(r)
        End Sub

        Public Function Load(ByVal r As FileReader) As Boolean
            Me.Rx3Header = New Rx3FileHeader(r)

            If (Me.Rx3Header.NumSections = 0) Then
                Return False
            End If

            For i = 0 To Me.Rx3Header.NumSections - 1
                Dim sectionInfo As New SectionHeader(r)
                Me.Rx3SectionHeaders.Add(sectionInfo)
            Next i

            Me.Sections = New Rx3Sections(Me, RW4Section, r)

            Return True
        End Function

        'Public Function ConvertKit(ByVal sourceRx3File As Rx3File) As Boolean
        'Return If(Not Me.IsFifa12, Me.ConvertKitFrom12(sourceRx3File), Me.ConvertKitFrom11(sourceRx3File))
        'End Function

        'Public Function ConvertKitFrom11(ByVal sourceRx3File As Rx3File) As Boolean
        'Return If(Not Me.IsFifa11, Not sourceRx3File.IsFifa12, False)
        'End Function

        'Public Function ConvertKitFrom12(ByVal sourceRx3File As Rx3File) As Boolean
        'Return If(Not Me.IsFifa12, Not sourceRx3File.IsFifa11, False)
        'End Function



        'Public Sub GenerateRx3SectionHeaders()
        '    Dim Index As Long = 0
        '    Me.Rx3SectionHeaders = New SectionHeader() {}
        '    '-- Rx3TextureBatch
        '    If Me.Sections.Textures IsNot Nothing Then
        '        ReDim Preserve Me.Rx3SectionHeaders(Index)
        '        Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '            .Signature = SectionHash.TEXTURE_BATCH,
        '            .Offset = 0,
        '            .Size = 0,
        '            .Unknown = 0
        '            }
        '        Index += 1
        '    End If
        '    '-- Rx3IndexBufferBatch
        '    If Me.Sections.IndexBuffers IsNot Nothing Then
        '        ReDim Preserve Me.Rx3SectionHeaders(Index)
        '        Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '            .Signature = SectionHash.INDEX_BUFFER_BATCH,
        '            .Offset = 0,
        '            .Size = 0,
        '            .Unknown = 0
        '            }
        '        Index += 1
        '    End If
        '    '-- Rx3VertexFormats
        '    If Me.Rx3Section.Sections.VertexFormats IsNot Nothing Then
        '        For i = 0 To Me.Sections.VertexFormats.Count - 1
        '            ReDim Preserve Me.Rx3SectionHeaders(Index)
        '            Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '            .Signature = SectionHash.VERTEX_FORMAT,
        '            .Offset = 0,
        '            .Size = 0,
        '            .Unknown = 0
        '            }
        '            Index += 1
        '        Next i
        '    End If
        '    '-- Rx3NameTable
        '    If Me.Rx3Section.Sections.NameTable IsNot Nothing Then
        '        ReDim Preserve Me.Rx3SectionHeaders(Index)
        '        Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '            .Signature = SectionHash.NAME_TABLE,
        '            .Offset = 0,
        '            .Size = 0,
        '            .Unknown = 0
        '            }
        '        Index += 1
        '    End If
        '    '-- Rx3texture
        '    If Me.Sections.Textures IsNot Nothing Then
        '        For i = 0 To Me.Sections.Textures.Length - 1
        '            ReDim Preserve Me.Rx3SectionHeaders(Index)
        '            Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '                .Signature = SectionHash.TEXTURE,
        '                .Offset = 0,
        '                .Size = 0,
        '                .Unknown = 0
        '                }
        '            Index += 1
        '        Next i
        '    End If
        '    '-- Rx3IndexBuffer
        '    If Me.Sections.IndexBuffers IsNot Nothing Then
        '        For i = 0 To Me.Sections.IndexBuffers.Length - 1
        '            ReDim Preserve Me.Rx3SectionHeaders(Index)
        '            Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '                .Signature = SectionHash.INDEX_BUFFER,
        '                .Offset = 0,
        '                .Size = 0,
        '                .Unknown = 0
        '                }
        '            Index += 1
        '        Next i
        '    End If
        '    '-- Rx3BoneRemap
        '    If Me.Rx3Section.Sections.BoneRemaps IsNot Nothing Then
        '        For i = 0 To Me.Rx3Section.Sections.BoneRemaps.Length - 1
        '            ReDim Preserve Me.Rx3SectionHeaders(Index)
        '            Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '                .Signature = SectionHash.BONE_REMAP,
        '                .Offset = 0,
        '                .Size = 0,
        '                .Unknown = 0
        '                }
        '            Index += 1
        '        Next i
        '    End If
        '    '-- Rx3VertexBuffer
        '    If Me.Sections.VertexBuffers IsNot Nothing Then
        '        For i = 0 To Me.Sections.VertexBuffers.Length - 1
        '            ReDim Preserve Me.Rx3SectionHeaders(Index)
        '            Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '                .Signature = SectionHash.VERTEX_BUFFER,
        '                .Offset = 0,
        '                .Size = 0,
        '                .Unknown = 0
        '                }
        '            Index += 1
        '        Next i
        '    End If
        '    '-- Rx3AnimationSkins
        '    If Me.Rx3Section.Sections.AnimationSkins IsNot Nothing Then
        '        For i = 0 To Me.Rx3Section.Sections.AnimationSkins.Count - 1
        '            ReDim Preserve Me.Rx3SectionHeaders(Index)
        '            Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '                .Signature = SectionHash.ANIMATION_SKIN,
        '                .Offset = 0,
        '                .Size = 0,
        '                .Unknown = 0
        '                }
        '            Index += 1
        '        Next i
        '    End If
        '    '-- Rx3SimpleMeshes
        '    If Me.Sections.SimpleMeshes IsNot Nothing Then
        '        For i = 0 To Me.Sections.SimpleMeshes.Length - 1
        '            ReDim Preserve Me.Rx3SectionHeaders(Index)
        '            Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '                .Signature = SectionHash.SIMPLE_MESH,
        '                .Offset = 0,
        '                .Size = 0,
        '                .Unknown = 0
        '                }
        '            Index += 1
        '        Next i
        '    End If
        '    '-- Rx3EdgeMeshes        'dont exist yet
        '    If Me.Rx3Section.Sections.EdgeMeshes IsNot Nothing Then
        '        For i = 0 To Me.Rx3Section.Sections.EdgeMeshes.Length - 1
        '            ReDim Preserve Me.Rx3SectionHeaders(Index)
        '            Me.Rx3SectionHeaders(Index) = New SectionHeader With {
        '                .Signature = SectionHash.EDGE_MESH,
        '                .Offset = 0,
        '                .Size = 0,
        '                .Unknown = 0
        '                }
        '            Index += 1
        '        Next i
        '    End If

        '    '-- OTHER - Header values
        '    Me.Rx3Header.NumSections = Me.Rx3SectionHeaders.Count    ' (also done at saving)
        'End Sub

        'public Function FifaRX3Hash(ByVal str As String) As UInteger   'test
        'Dim result As UInteger = 5321
        'For Each c In str
        '       result = AscW(c) + result * 33
        'Next c
        'Return result
        'End Function

        'Private Function GetVertexElementList11() As List(Of VertexElement())
        '    Dim m_ListVertexElements As New List(Of VertexElement()) '= Nothing
        '    Dim m_VertexElements As VertexElement() '{}'= Nothing

        '    If Me.RW4Section.Sections.VertexBuffers Is Nothing Then
        '        Return Nothing
        '    End If

        '    For v = 0 To Me.RW4Section.Sections.VertexBuffers.Count - 1
        '        'm_VertexElements = New VertexElement(Me.RW4Section.VertexDescription(v).nElements - 1) {}

        '        If Me.RW4Section.Sections.VertexDescriptors.Count = 1 Then
        '            For u = 0 To Me.RW4Section.Sections.VertexDescriptors(0).NumElements - 1
        '                ReDim Preserve m_VertexElements(Me.RW4Section.Sections.VertexDescriptors(0).NumElements - 1)
        '                m_VertexElements(u) = New VertexElement With {
        '                    .DataType = Me.RW4Section.Sections.VertexDescriptors(0).Elements(u).DataType,
        '                    .Usage = Me.RW4Section.Sections.VertexDescriptors(0).Elements(u).Usage,
        '                    .Offset = Me.RW4Section.Sections.VertexDescriptors(0).Elements(u).Offset,
        '                    .UsageIndex = Me.RW4Section.Sections.VertexDescriptors(0).Elements(u).UsageIndex
        '                }
        '            Next u
        '        Else
        '            For u = 0 To Me.RW4Section.Sections.VertexDescriptors(v).NumElements - 1
        '                ReDim Preserve m_VertexElements(Me.RW4Section.Sections.VertexDescriptors(v).NumElements - 1)
        '                m_VertexElements(u) = New VertexElement With {
        '                        .DataType = Me.RW4Section.Sections.VertexDescriptors(v).Elements(u).DataType,
        '                        .Usage = Me.RW4Section.Sections.VertexDescriptors(v).Elements(u).Usage,
        '                        .Offset = Me.RW4Section.Sections.VertexDescriptors(v).Elements(u).Offset,
        '                        .UsageIndex = Me.RW4Section.Sections.VertexDescriptors(v).Elements(u).UsageIndex
        '                    }
        '            Next u
        '        End If




        '        m_ListVertexElements.Add(m_VertexElements)
        '    Next v

        '    Return m_ListVertexElements
        'End Function
        'Private Function GetVertexElementList12(m_ListVertexElements As List(Of VertexElement()), m_Rx3VertexElements As Rx3.VertexFormat.Element()) As List(Of VertexElement())
        '    'Dim m_ListVertexElements As List(Of VertexElement()) = Nothing
        '    'Dim m_VertexElements As VertexElement() = Nothing

        '    'For v = 0 To Me.m_Rx3ModelDirectory.NFiles - 1
        '    Dim m_VertexElements = New VertexElement(m_Rx3VertexElements.Length - 1) {}
        '    For u = 0 To m_VertexElements.Length - 1
        '        m_VertexElements(u) = New VertexElement With {
        '            .DataType = m_Rx3VertexElements(u).DataType,
        '            .Usage = m_Rx3VertexElements(u).Usage,
        '            .Offset = m_Rx3VertexElements(u).Offset,
        '            .UsageIndex = m_Rx3VertexElements(u).UsageIndex
        '            }
        '    Next u
        '    m_ListVertexElements.Add(m_VertexElements)
        '    'Next v

        '    Return m_ListVertexElements
        'End Function

        Public Function Save(ByVal w As FileWriter) As Boolean

            '2 - Save Header (Rx3)
            Dim OffsetRx3Headers As Long = w.BaseStream.Position
            Me.Rx3Header.NumSections = Me.Sections.NumObjects
            Me.Rx3Header.Save(w)

            '3 - Save Section headers 
            Me.Sections.WriteSectionInfos(w)

            '5 - Save Sections
            Me.Sections.Save(w)

            '6 - Save Filesize
            'FifaUtil.WriteFilePaddings(w)
            If Me.RW4Section IsNot Nothing Then '-- FIFA 11
                'totalsize_vertexbuffers + totalsizeindexbuffers + 16
                Me.Rx3Header.Size = 0
                If Me.Sections.VertexBuffers IsNot Nothing Then
                    For i = 0 To Me.Sections.VertexBuffers.Length - 1
                        Me.Rx3Header.Size += Me.Sections.VertexBuffers(i).TotalSize
                    Next
                End If
                If Me.Sections.IndexBuffers IsNot Nothing Then
                    For i = 0 To Me.Sections.IndexBuffers.Length - 1
                        Me.Rx3Header.Size += Me.Sections.IndexBuffers(i).Rx3IndexBufferHeader.TotalSize
                    Next
                End If
                If Me.Sections.Textures IsNot Nothing Then
                    For i = 0 To Me.Sections.Textures.Length - 1
                        Me.Rx3Header.Size += Me.Sections.Textures(i).Rx3TextureHeader.TotalSize
                    Next
                End If
                Me.Rx3Header.Size += 16
            Else
                Me.Rx3Header.Size = w.BaseStream.Position   ' = m_FileSize
            End If

            '7 - Save Header (rx3) & Section headers again (now with correct offsets/sizes)
            w.BaseStream.Position = OffsetRx3Headers
            Me.Rx3Header.Save(w)
            Me.Sections.WriteSectionInfos(w)

            '8 - rewrite "Batch" sections (now with correct offsets/sizes)
            Me.Sections.WriteBatchSections(w)

            Return True
        End Function

        Public Property Bitmaps As List(Of Bitmap)
            Get
                Dim BitmapArray As New List(Of Bitmap)
                For i = 0 To Me.Sections.Textures.Length - 1
                    BitmapArray.Add(Me.Sections.Textures(i).GetBitmap)
                Next i
                Return BitmapArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.Textures.Length), Value.Count, Me.Sections.Textures.Length)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.Textures(i).SetBitmap(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property DDSTextures As List(Of DDSFile)
            Get
                Dim DDSArray As New List(Of DDSFile)
                For i = 0 To Me.Sections.Textures.Length - 1
                    DDSArray.Add(Me.Sections.Textures(i).GetDds)
                Next i
                Return DDSArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.Textures.Length), Value.Count, Me.Sections.Textures.Length)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.Textures(i).SetDds(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property VertexStreams As List(Of Vertex())
            Get
                Dim VertexArray As New List(Of Vertex())
                For i = 0 To Me.Sections.VertexBuffers.Length - 1
                    VertexArray.Add(Me.Sections.VertexBuffers(i).VertexData)
                Next i
                Return VertexArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.VertexBuffers.Length), Value.Count, Me.Sections.VertexBuffers.Length)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.VertexBuffers(i).VertexData = Value(i)
                    End If
                Next i
            End Set
        End Property

        Public Property IndexStreams As List(Of UInteger())
            Get
                Dim IndexArray As New List(Of UInteger())
                For i = 0 To Me.Sections.IndexBuffers.Length - 1
                    IndexArray.Add(Me.Sections.IndexBuffers(i).IndexData)
                Next i
                Return IndexArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.IndexBuffers.Length), Value.Count, Me.Sections.IndexBuffers.Length)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.IndexBuffers(i).IndexData = Value(i)
                    End If
                Next i
            End Set
        End Property
        Public Property PrimitiveTypes(ByVal MeshIndex As UInteger) As Microsoft.DirectX.Direct3D.PrimitiveType
            Get
                If Me.Sections.SimpleMeshes IsNot Nothing AndAlso Me.Sections.SimpleMeshes(MeshIndex) IsNot Nothing Then
                    Return Me.Sections.SimpleMeshes(MeshIndex).PrimitiveType
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.SimpleMeshes IsNot Nothing AndAlso Me.Sections.SimpleMeshes(MeshIndex) IsNot Nothing Then
                    Me.Sections.SimpleMeshes(MeshIndex).PrimitiveType = Value
                End If
            End Set
        End Property
        Public Property PrimitiveTypes As List(Of Microsoft.DirectX.Direct3D.PrimitiveType)
            Get
                If Me.Sections.SimpleMeshes IsNot Nothing Then
                    Dim Result As New List(Of Microsoft.DirectX.Direct3D.PrimitiveType)
                    For i = 0 To Me.Sections.SimpleMeshes.Count - 1
                        Result.Add(Me.Sections.SimpleMeshes(i).PrimitiveType)
                    Next

                    Return Result
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.SimpleMeshes IsNot Nothing AndAlso Me.Sections.SimpleMeshes.Count = Value.Count Then
                    For i = 0 To Me.Sections.SimpleMeshes.Count - 1
                        Me.Sections.SimpleMeshes(i).PrimitiveType = Value(i)
                    Next i
                End If
            End Set
        End Property

        Public ReadOnly Property NumMeshes As UInteger
            Get
                If Me.Sections.VertexBuffers IsNot Nothing Then
                    Return Me.Sections.VertexBuffers.Length
                End If

                Return 0
            End Get
        End Property

        Public ReadOnly Property NumTextures As UInteger
            Get
                If Me.Sections.Textures IsNot Nothing Then
                    Return Me.Sections.Textures.Length
                End If

                Return 0
            End Get
        End Property

        ' Properties
        Private RW4Section As Arena
        Public Property Rx3Header As Rx3FileHeader

        Friend ReadOnly Rx3SectionHeaders As List(Of SectionHeader)
        Public Property Sections As Rx3Sections

        'Private m_IsFifa11 As Boolean
        'Private m_FileName As String
        'Private m_FileSize As Long

    End Class
End Namespace