Imports System.Drawing
Imports System.Reflection
Imports BCnEncoder.Shared
Imports FIFALibrary22.Rw.Core.Arena

'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/

Namespace Rx3
    Public Class Rx3File

        Public Sub New()
            Me.Rw4Section = Nothing
        End Sub

        Public Sub New(ByVal Endianness As Endian)
            Me.Rx3Header = New Rx3FileHeader(Endianness)
            Me.Rw4Section = Nothing
        End Sub

        Public Sub New(ByVal Endianness As Endian, Rw4Section As Arena)
            Me.Rx3Header = New Rx3FileHeader(Endianness)
            Me.Rw4Section = Rw4Section
        End Sub

        Public Sub New(Rw4Section As Arena, ByVal r As FileReader)
            Me.Rw4Section = Rw4Section
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

            Me.Sections = New Rx3Sections(Me, Rw4Section, r)
            'Me.Sections.Load(r) '= New Rx3Sections(Me, Rw4Section, r)

            Return True
        End Function

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

        Public Function Save(ByVal w As FileWriter) As Boolean
            '1 - Set objects from private values to the objects list
            Me.Sections.SetObjectsFromMemory()

            '2 - Save Header (Rx3)
            Dim OffsetRx3Headers As Long = w.BaseStream.Position
            Me.Rx3Header.NumSections = Me.Sections.NumObjects
            Me.Rx3Header.Save(w)

            '3 - Save Section headers 
            w.Write(New Byte((Me.Rx3Header.NumSections * 16) - 1) {}) 'Me.Sections.WriteSectionInfos(w)

            '5 - Save Sections
            Me.Sections.Save(w)

            '6 - Save Filesize
            'FifaUtil.WriteFilePaddings(w)
            If Me.Rw4Section IsNot Nothing Then '-- FIFA 11
                'totalsize_vertexbuffers + totalsizeindexbuffers + 16
                Me.Rx3Header.Size = 0
                If Me.Sections.VertexBuffers IsNot Nothing Then
                    For i = 0 To Me.Sections.VertexBuffers.Count - 1
                        Me.Rx3Header.Size += Me.Sections.VertexBuffers(i).TotalSize
                    Next
                End If
                If Me.Sections.IndexBuffers IsNot Nothing Then
                    For i = 0 To Me.Sections.IndexBuffers.Count - 1
                        Me.Rx3Header.Size += Me.Sections.IndexBuffers(i).Header.TotalSize
                    Next
                End If
                If Me.Sections.Textures IsNot Nothing Then
                    For i = 0 To Me.Sections.Textures.Count - 1
                        Me.Rx3Header.Size += Me.Sections.Textures(i).Header.TotalSize
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

        Public Property Bitmaps(ByVal Index As UInteger) As Bitmap
            Get
                If Me.Sections.Textures IsNot Nothing AndAlso Me.Sections.Textures(Index) IsNot Nothing Then
                    Return Me.Sections.Textures(Index).GetBitmap
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.Textures IsNot Nothing AndAlso Me.Sections.Textures(Index) IsNot Nothing Then
                    Me.Sections.Textures(Index).SetBitmap(Value)
                End If
            End Set
        End Property

        Public Property Bitmaps As List(Of Bitmap)
            Get
                Dim BitmapArray As New List(Of Bitmap)
                For i = 0 To Me.Sections.Textures.Count - 1
                    BitmapArray.Add(Me.Sections.Textures(i).GetBitmap)
                Next i
                Return BitmapArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.Textures.Count), Value.Count, Me.Sections.Textures.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.Textures(i).SetBitmap(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property DdsTextures(ByVal Index As UInteger) As DdsFile
            Get
                If Me.Sections.Textures IsNot Nothing AndAlso Me.Sections.Textures(Index) IsNot Nothing Then
                    Return Me.Sections.Textures(Index).GetDds
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.Textures IsNot Nothing AndAlso Me.Sections.Textures(Index) IsNot Nothing Then
                    Me.Sections.Textures(Index).SetDds(Value)
                End If
            End Set
        End Property

        Public Property DdsTextures As List(Of DdsFile)
            Get
                Dim DdsArray As New List(Of DdsFile)
                For i = 0 To Me.Sections.Textures.Count - 1
                    DdsArray.Add(Me.Sections.Textures(i).GetDds)
                Next i
                Return DdsArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.Textures.Count), Value.Count, Me.Sections.Textures.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.Textures(i).SetDds(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property KtxTextures(ByVal Index As UInteger) As KtxFile
            Get
                If Me.Sections.Textures IsNot Nothing AndAlso Me.Sections.Textures(Index) IsNot Nothing Then
                    Return Me.Sections.Textures(Index).GetKtx
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.Textures IsNot Nothing AndAlso Me.Sections.Textures(Index) IsNot Nothing Then
                    Me.Sections.Textures(Index).SetKtx(Value)
                End If
            End Set
        End Property

        Public Property KtxTextures As List(Of KtxFile)
            Get
                Dim KtxArray As New List(Of KtxFile)
                For i = 0 To Me.Sections.Textures.Count - 1
                    KtxArray.Add(Me.Sections.Textures(i).GetKtx)
                Next i
                Return KtxArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.Textures.Count), Value.Count, Me.Sections.Textures.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.Textures(i).SetKtx(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property VertexStreams(ByVal Index As UInteger) As List(Of Vertex)
            Get
                If Me.Sections.VertexBuffers IsNot Nothing AndAlso Me.Sections.VertexBuffers(Index) IsNot Nothing Then
                    Return Me.Sections.VertexBuffers(Index).VertexData
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.VertexBuffers IsNot Nothing AndAlso Me.Sections.VertexBuffers(Index) IsNot Nothing Then
                    Me.Sections.VertexBuffers(Index).VertexData = Value
                End If
            End Set
        End Property

        Public Property VertexStreams As List(Of List(Of Vertex))
            Get
                Dim VertexArray As New List(Of List(Of Vertex))
                For i = 0 To Me.Sections.VertexBuffers.Count - 1
                    VertexArray.Add(Me.Sections.VertexBuffers(i).VertexData)
                Next i
                Return VertexArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.VertexBuffers.Count), Value.Count, Me.Sections.VertexBuffers.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.VertexBuffers(i).VertexData = Value(i)
                    End If
                Next i
            End Set
        End Property

        Public Property IndexStreams(ByVal Index As UInteger) As List(Of UInteger)
            Get
                If Me.Sections.IndexBuffers IsNot Nothing AndAlso Me.Sections.IndexBuffers(Index) IsNot Nothing Then
                    Return Me.Sections.IndexBuffers(Index).IndexData
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.IndexBuffers IsNot Nothing AndAlso Me.Sections.IndexBuffers(Index) IsNot Nothing Then
                    Me.Sections.IndexBuffers(Index).IndexData = Value
                End If
            End Set
        End Property

        Public Property IndexStreams As List(Of List(Of UInteger))
            Get
                Dim IndexArray As New List(Of List(Of UInteger))
                For i = 0 To Me.Sections.IndexBuffers.Count - 1
                    IndexArray.Add(Me.Sections.IndexBuffers(i).IndexData)
                Next i
                Return IndexArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.IndexBuffers.Count), Value.Count, Me.Sections.IndexBuffers.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.IndexBuffers(i).IndexData = Value(i)
                    End If
                Next i
            End Set
        End Property

        Public Property PrimitiveTypes(ByVal Index As UInteger) As Microsoft.DirectX.Direct3D.PrimitiveType
            Get
                If Me.Sections.SimpleMeshes IsNot Nothing AndAlso Me.Sections.SimpleMeshes(Index) IsNot Nothing Then
                    Return Me.Sections.SimpleMeshes(Index).PrimitiveType
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.SimpleMeshes IsNot Nothing AndAlso Me.Sections.SimpleMeshes(Index) IsNot Nothing Then
                    Me.Sections.SimpleMeshes(Index).PrimitiveType = Value
                End If
            End Set
        End Property

        Public Property PrimitiveTypes As List(Of Microsoft.DirectX.Direct3D.PrimitiveType)
            Get
                Dim Result As New List(Of Microsoft.DirectX.Direct3D.PrimitiveType)
                For i = 0 To Me.Sections.SimpleMeshes.Count - 1
                    Result.Add(Me.Sections.SimpleMeshes(i).PrimitiveType)
                Next

                Return Result
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.SimpleMeshes.Count), Value.Count, Me.Sections.SimpleMeshes.Count)
                For i = 0 To num - 1
                    If i <= Value.Count - 1 Then '(Value(i) IsNot Nothing) Then
                        Me.Sections.SimpleMeshes(i).PrimitiveType = Value(i)
                    End If
                Next i
            End Set
        End Property

        Public Property VertexFormats(ByVal Index As UInteger) As VertexElement()
            Get
                If Me.Sections.VertexFormats IsNot Nothing AndAlso Me.Sections.VertexFormats(Index) IsNot Nothing Then
                    Return Me.Sections.VertexFormats(Index).Elements
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.VertexFormats IsNot Nothing AndAlso Me.Sections.VertexFormats(Index) IsNot Nothing Then
                    Me.Sections.VertexFormats(Index).Elements = New VertexFormat.Element(Value.Count - 1) {}
                    For j = 0 To Value.Count - 1
                        Me.Sections.VertexFormats(Index).Elements(j) = New VertexFormat.Element(Value(j))
                    Next j
                End If
            End Set
        End Property

        Public Property VertexFormats As List(Of VertexElement())
            Get
                Dim m_Lists As New List(Of VertexElement())
                If Me.Sections.VertexFormats IsNot Nothing Then
                    For i = 0 To Me.Sections.VertexFormats.Count - 1
                        m_Lists.Add(Me.Sections.VertexFormats(i).Elements)
                    Next
                End If

                Return m_Lists
            End Get
            Set
                If Me.Sections.VertexFormats IsNot Nothing AndAlso Me.Sections.VertexFormats.Count = Value.Count Then
                    For i = 0 To Me.Sections.VertexFormats.Count - 1
                        Me.Sections.VertexFormats(i).Elements = New VertexFormat.Element(Value(i).Count - 1) {}
                        For j = 0 To Value(i).Count - 1
                            Me.Sections.VertexFormats(i).Elements(j) = New VertexFormat.Element(Value(i)(j))
                        Next j
                    Next i
                End If
            End Set
        End Property

        Public ReadOnly Property NameTable As List(Of FIFALibrary22.NameTable)
            Get
                Return Me.Sections.NameTable.GetNameTable()
            End Get
            'Set
            '     = Value
            'End Set
        End Property

        Public Property MeshName(ByVal MeshIndex As UInteger) As String
            Get
                Return Me.Sections.NameTable.GetNameByType(Rx3.SectionHash.SIMPLE_MESH, MeshIndex)
            End Get
            Set
                Me.Sections.NameTable.SetNameByType(Value, Rx3.SectionHash.SIMPLE_MESH, MeshIndex)
            End Set
        End Property

        Public Property TextureName(ByVal TextureIndex As UInteger) As String
            Get
                Return Me.Sections.NameTable.GetNameByType(Rx3.SectionHash.TEXTURE, TextureIndex)
            End Get
            Set
                Me.Sections.NameTable.SetNameByType(Value, Rx3.SectionHash.TEXTURE, TextureIndex)
            End Set
        End Property

        Public ReadOnly Property NumMeshes As UInteger
            Get
                If Me.Sections.VertexBuffers IsNot Nothing Then
                    Return Me.Sections.VertexBuffers.Count
                End If

                Return 0
            End Get
        End Property

        Public ReadOnly Property NumTextures As UInteger
            Get
                If Me.Sections.Textures IsNot Nothing Then
                    Return Me.Sections.Textures.Count
                End If

                Return 0
            End Get
        End Property

        Public Property BoneMatrices(ByVal Index As UInteger) As List(Of BonePose)
            Get
                If Me.Sections.AnimationSkins IsNot Nothing AndAlso Me.Sections.AnimationSkins(Index) IsNot Nothing Then
                    Return Me.Sections.AnimationSkins(Index).BoneMatrices
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.AnimationSkins IsNot Nothing AndAlso Me.Sections.AnimationSkins(Index) IsNot Nothing Then
                    Me.Sections.AnimationSkins(Index).BoneMatrices = Value
                End If
            End Set
        End Property

        Public Property BoneMatrices As List(Of List(Of BonePose))
            Get
                Dim m_Lists As New List(Of List(Of BonePose))
                If Me.Sections.AnimationSkins IsNot Nothing Then
                    For i = 0 To Me.Sections.AnimationSkins.Count - 1
                        m_Lists.Add(Me.Sections.AnimationSkins(i).BoneMatrices)
                    Next
                End If

                Return m_Lists
            End Get
            Set
                If Me.Sections.AnimationSkins IsNot Nothing AndAlso Me.Sections.AnimationSkins.Count = Value.Count Then
                    For i = 0 To Me.Sections.AnimationSkins.Count - 1
                        Me.Sections.AnimationSkins(i).BoneMatrices = Value(i)
                    Next i
                End If
            End Set
        End Property

        ' Properties
        Private Rw4Section As Arena = Nothing
        Public Property Rx3Header As Rx3FileHeader
        Friend ReadOnly Property Rx3SectionHeaders As New List(Of SectionHeader)
        Public Property Sections As Rx3Sections = New Rx3Sections(Me, Rw4Section)

    End Class
End Namespace