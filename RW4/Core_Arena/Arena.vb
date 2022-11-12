'https://github.com/emd4600/SporeModder-FX/tree/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4
'https://emd4600.github.io/Spore-ModAPI/

Imports System.Drawing
Imports BCnEncoder.Shared

Namespace Rw.Core.Arena
    Public Class Arena
        'rw::core::arena::Arena
        Public Sub New()
        End Sub

        Public Sub New(ByVal Endianness As Endian)
            Me.FileHeader = New ArenaFileHeader(Endianness)
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        'Public Sub New(ByVal UseRwBuffers As Boolean, ByVal r As FileReader)
        '    Me.UseRwBuffers = UseRwBuffers
        '    Me.Load(r)
        'End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.FileHeader = New ArenaFileHeader(r)

            Me.LoadGeneralStructure(r)

            Me.UseRwBuffers = CheckUseRwBuffers(r)

            r.BaseStream.Position = Me.OffsetDict
            For i = 0 To Me.NumEntries - 1
                Dim sectionInfo As New ArenaDictEntry(r)
                Me.ArenaDictEntries.Add(sectionInfo)

                'If sectionInfo.TypeId = Rw.SectionTypeCode.RWOBJECTTYPE_BUFFER Then
                '    sectionInfo.Offset += Me.ResourceDescriptor.BaseResourceDescriptors(0).Size
                'End If
            Next i

            r.BaseStream.Position = Me.OffsetSectManifest
            Me.SectionManifest = New ArenaSectionManifest(Me, r)

            Me.Sections.Load(Me.UseRwBuffers, r) '= New RWSections(Me, r)

        End Sub

        Public Function CheckUseRwBuffers(ByVal r As FileReader) As Boolean
            r.BaseStream.Position = Me.ResourceDescriptor.BaseResourceDescriptors(0).Size

            If r.BaseStream.Position + 4 <= r.BaseStream.Length - 1 Then
                Dim Str As Integer = r.ReadInt32 'New String(r.ReadChars(4))
                If (Str = &H6C335852) Or (Str = &H62335852) Or (Str = &H5258336C) Or (Str = &H52583362) Then
                    Return False
                End If
            End If

            Return True
        End Function

        Private Sub LoadGeneralStructure(ByVal r As FileReader)
            Me.AssetId = r.ReadUInt32               'id
            Me.NumEntries = r.ReadUInt32            'Number of Sections
            Me.NumUsed = r.ReadUInt32               'Number of Sections Used
            Me.Alignment = r.ReadUInt32
            Me.Virt = r.ReadUInt32
            Me.OffsetDict = r.ReadUInt32            'dump name: dictStart --> pointer to rw::core::arena::ArenaDictEntry
            Me.OffsetSectManifest = r.ReadUInt32    'dump name: sections
            Me.Base = r.ReadUInt32                  '32 bit pointer to void
            Me.UnfixContext = r.ReadUInt32          '--> pointer to 'rw::core::arena::SizeAndAlignment'
            Me.FixContext = r.ReadUInt32            ' --> pointer to 'rw::core::arena::ArenaIterator'

            'me.m_resourceDescriptor --> rw::ResourceDescriptor (Array of 5, total length 40)
            Me.ResourceDescriptor = New ResourceDescriptor(r)
            'For i = 0 To 5 - 1
            '    Me.ResourceDescriptor(i) = New ResourceDescriptor(r)
            'Next i
            'me.m_resourcesUsed --> rw::ResourceDescriptor (Array of 5, total length 40)
            Me.ResourcesUsed = New ResourceDescriptor(r)
            'For j = 0 To 5 - 1
            '    Me.ResourcesUsed(j) = New ResourceDescriptor(r)
            'Next j
            'me.m_resource  --> rw::TargetResource (pointers maybe, but usually 0) (Array of 5, total length 20)
            Me.Resource = New TargetResource(r)
            'For j = 0 To 5 - 1
            '    Me.Resource(j) = r.ReadUInt32
            'Next j

            Me.ArenaGroup = r.ReadUInt32            '--> pointer to 'rw::core::arena::ArenaGroup'

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            '1 - Save Header
            Me.FileHeader.Save(w)

            '2 - Save General Structure
            Dim OffsetGeneralStructure As Long = w.BaseStream.Position
            w.Write(New Byte(144 - 1) {}) 'Me.SaveGeneralStructure(w)

            ' - Section Manifest > SubReferences : clear first
            'Me.SectionManifest.SubReferences.Records.Clear()

            ' - Section Manifest > Types : Create the list with all the type codes
            'Me.SectionManifest.Types.CreateList(Me.Sections.GetObjects)

            '3 - Save Section Manifest
            Me.OffsetSectManifest = w.BaseStream.Position
            Me.SectionManifest.Save(w)

            '4 - Save Sections
            Me.Sections.Save(Me, w)

            ' - padding after last section
            'If Me.RW4GeneralStructure.Resources_3_Used(0).DataSize <> 0 Then
            'FifaUtil.WriteAllignment(w, Me.RW4GeneralStructure.Resources_3_Used(0).DataAlignment)
            'Else
            FifaUtil.WriteAlignment(w, 4)
            'End If

            '5 - Save Section infos
            Me.OffsetDict = w.BaseStream.Position
            Me.Sections.WriteSectionInfos(w)

            '6 - Save SubReferences (at end of RW4 section)
            Me.SectionManifest.SubReferences.SaveRecords(w)

            '7 - Add padding + get Offset rx3b/rx2 section
            FifaUtil.WriteAlignment(w, Me.ResourceDescriptor.BaseResourceDescriptors(0).Alignment)
            If Me.ResourcesUsed.BaseResourceDescriptors(2).Size <> 0 Then 'And Me.RW4GeneralStructure.Resources_3_Used(2).DataAlignment = 4096 Then
                w.Write(CByte(0))
                FifaUtil.WriteAlignment(w, Me.ResourcesUsed.BaseResourceDescriptors(2).Alignment)
            End If
            Me.ResourceDescriptor.BaseResourceDescriptors(0).Size = w.BaseStream.Position   '--> size of the RW section (without buffers)
            Me.ResourceDescriptor.BaseResourceDescriptors(2).Size = Me.Sections.GetTotalBuffersSize   '--> size of buffers

            '8 - Save GeneralStructure & SectionManifest  (now with correct offsets/sizes)
            w.BaseStream.Position = OffsetGeneralStructure
            SaveGeneralStructure(w)
            w.BaseStream.Position = Me.OffsetSectManifest
            Me.SectionManifest.Save(w)

            '9 - Go to end of RW4 section (to continue to rx3b/rx2 section)
            w.BaseStream.Position = Me.ResourceDescriptor.BaseResourceDescriptors(0).Size
            If Me.UseRwBuffers Then '-- rx2 = True , Rx3Hybrid = False
                Me.Sections.SaveBuffers(w)
            End If

        End Sub

        Private Sub SaveGeneralStructure(ByVal w As FileWriter)
            Me.NumEntries = Me.ArenaDictEntries.Count
            Me.NumUsed = Me.ArenaDictEntries.Count
            'Me.OffsetDict = 'set in RWFile 
            'Me.OffsetSectManifest = 'set in RWFile 

            w.Write(Me.AssetId)
            w.Write(Me.NumEntries)
            w.Write(Me.NumUsed)
            w.Write(Me.Alignment)
            w.Write(Me.Virt)
            w.Write(Me.OffsetDict)
            w.Write(Me.OffsetSectManifest)
            w.Write(Me.Base)
            w.Write(Me.UnfixContext)
            w.Write(Me.FixContext)

            Me.ResourceDescriptor.Save(w)
            'For i = 0 To 5 - 1
            '    w.Write(Me.ResourceDescriptor(i).Size)
            '    w.Write(Me.ResourceDescriptor(i).Alignment)
            'Next i

            Me.ResourcesUsed.Save(w)
            'For j = 0 To 5 - 1
            '    w.Write(Me.ResourcesUsed(j).Size)
            '    w.Write(Me.ResourcesUsed(j).Alignment)
            'Next j

            Me.Resource.Save(w)
            'For i = 0 To 5 - 1
            '    w.Write(Me.Resource(i))
            'Next i

            w.Write(Me.ArenaGroup)

        End Sub

        'Public Overridable Sub addReference(ByVal [object] As RWObject, ByVal offset As Integer)
        '    header_Conflict.sectionManifest.subReferences.references.add(New SubReference([object], offset))
        'End Sub

        'Public Overridable Function GetName(ByVal m_Object As RWObject) As String
        '    Return m_Object.GetTypeCode.ToString 'm_Object.GetType().Name + AscW("-"c) + sectionInfos.IndexOf(m_Object.sectionInfo)
        'End Function

        'Public Function WriteDictionary()   '--> found at dump

        'End Function

        Public Property Bitmaps(ByVal Index As UInteger) As Bitmap
            Get
                If Me.Sections.Rasters IsNot Nothing AndAlso Me.Sections.Rasters(Index) IsNot Nothing Then
                    Return Me.Sections.Rasters(Index).GetBitmap
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.Rasters IsNot Nothing AndAlso Me.Sections.Rasters(Index) IsNot Nothing Then
                    Me.Sections.Rasters(Index).SetBitmap(Value)
                End If
            End Set
        End Property

        Public Property Bitmaps As List(Of Bitmap)
            Get
                Dim BitmapArray As New List(Of Bitmap)
                For i = 0 To Me.Sections.Rasters.Count - 1
                    BitmapArray.Add(Me.Sections.Rasters(i).GetBitmap)
                Next i
                Return BitmapArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.Rasters.Count), Value.Count, Me.Sections.Rasters.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.Rasters(i).SetBitmap(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property DdsTextures(ByVal Index As UInteger) As DdsFile
            Get
                If Me.Sections.Rasters IsNot Nothing AndAlso Me.Sections.Rasters(Index) IsNot Nothing Then
                    Return Me.Sections.Rasters(Index).GetDds
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.Rasters IsNot Nothing AndAlso Me.Sections.Rasters(Index) IsNot Nothing Then
                    Me.Sections.Rasters(Index).SetDds(Value)
                End If
            End Set
        End Property

        Public Property DdsTextures As List(Of DdsFile)
            Get
                Dim DdsArray As New List(Of DdsFile)
                For i = 0 To Me.Sections.Rasters.Count - 1
                    DdsArray.Add(Me.Sections.Rasters(i).GetDds)
                Next i
                Return DdsArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.Rasters.Count), Value.Count, Me.Sections.Rasters.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.Rasters(i).SetDds(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property KtxTextures(ByVal Index As UInteger) As KtxFile
            Get
                If Me.Sections.Rasters IsNot Nothing AndAlso Me.Sections.Rasters(Index) IsNot Nothing Then
                    Return Me.Sections.Rasters(Index).GetKtx
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.Rasters IsNot Nothing AndAlso Me.Sections.Rasters(Index) IsNot Nothing Then
                    Me.Sections.Rasters(Index).SetKtx(Value)
                End If
            End Set
        End Property

        Public Property KtxTextures As List(Of KtxFile)
            Get
                Dim KtxArray As New List(Of KtxFile)
                For i = 0 To Me.Sections.Rasters.Count - 1
                    KtxArray.Add(Me.Sections.Rasters(i).GetKtx)
                Next i
                Return KtxArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.Rasters.Count), Value.Count, Me.Sections.Rasters.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.Rasters(i).SetKtx(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property VertexStreams(ByVal Index As UInteger) As List(Of Vertex)
            Get
                If Me.Sections.VertexBuffers IsNot Nothing AndAlso Me.Sections.VertexBuffers(Index) IsNot Nothing Then
                    Return Me.Sections.VertexBuffers(Index).GetVertexData
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.VertexBuffers IsNot Nothing AndAlso Me.Sections.VertexBuffers(Index) IsNot Nothing Then
                    Me.Sections.VertexBuffers(Index).SetVertexData(Value)
                End If
            End Set
        End Property

        Public Property VertexStreams As List(Of List(Of Vertex))
            Get
                Dim VertexArray As New List(Of List(Of Vertex))
                For i = 0 To Me.Sections.VertexBuffers.Count - 1
                    VertexArray.Add(Me.Sections.VertexBuffers(i).GetVertexData)
                Next i
                Return VertexArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.VertexBuffers.Count), Value.Count, Me.Sections.VertexBuffers.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.VertexBuffers(i).SetVertexData(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property IndexStreams(ByVal Index As UInteger) As List(Of UInteger)
            Get
                If Me.Sections.IndexBuffers IsNot Nothing AndAlso Me.Sections.IndexBuffers(Index) IsNot Nothing Then
                    Return Me.Sections.IndexBuffers(Index).GetIndexData
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.IndexBuffers IsNot Nothing AndAlso Me.Sections.IndexBuffers(Index) IsNot Nothing Then
                    Me.Sections.IndexBuffers(Index).SetIndexData(Value)
                End If
            End Set
        End Property

        Public Property IndexStreams As List(Of List(Of UInteger))
            Get
                Dim IndexArray As New List(Of List(Of UInteger))
                For i = 0 To Me.Sections.IndexBuffers.Count - 1
                    IndexArray.Add(Me.Sections.IndexBuffers(i).GetIndexData)
                Next i
                Return IndexArray
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.IndexBuffers.Count), Value.Count, Me.Sections.IndexBuffers.Count)
                For i = 0 To num - 1
                    If (Value(i) IsNot Nothing) Then
                        Me.Sections.IndexBuffers(i).SetIndexData(Value(i))
                    End If
                Next i
            End Set
        End Property

        Public Property PrimitiveTypes(ByVal Index As UInteger) As Microsoft.DirectX.Direct3D.PrimitiveType
            Get
                If Me.Sections.FxRenderableSimples IsNot Nothing AndAlso Me.Sections.FxRenderableSimples(Index) IsNot Nothing Then
                    Return Me.Sections.FxRenderableSimples(Index).PrimitiveType
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.FxRenderableSimples IsNot Nothing AndAlso Me.Sections.FxRenderableSimples(Index) IsNot Nothing Then
                    Me.Sections.FxRenderableSimples(Index).PrimitiveType = Value
                End If
            End Set
        End Property

        Public Property PrimitiveTypes As List(Of Microsoft.DirectX.Direct3D.PrimitiveType)
            Get
                Dim Result As New List(Of Microsoft.DirectX.Direct3D.PrimitiveType)
                For i = 0 To Me.Sections.FxRenderableSimples.Count - 1
                    Result.Add(Me.Sections.FxRenderableSimples(i).PrimitiveType)
                Next

                Return Result
            End Get
            Set
                Dim num As Integer = If((Value.Count < Me.Sections.FxRenderableSimples.Count), Value.Count, Me.Sections.FxRenderableSimples.Count)
                For i = 0 To num - 1
                    If i <= Value.Count - 1 Then '(Value(i) IsNot Nothing) Then
                        Me.Sections.FxRenderableSimples(i).PrimitiveType = Value(i)
                    End If
                Next i
            End Set
        End Property

        Public Property VertexFormats(ByVal Index As UInteger) As VertexElement()
            Get
                If Me.Sections.FxRenderableSimples IsNot Nothing AndAlso Me.Sections.FxRenderableSimples(Index) IsNot Nothing Then
                    Return Me.Sections.FxRenderableSimples(Index).PVertexDescriptor.Elements 'Me.RW4Section.Sections.EmbeddedMeshes(MeshIndex).PVertexBuffers(0).PVertexDescriptor.Elements
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.FxRenderableSimples IsNot Nothing AndAlso Me.Sections.FxRenderableSimples(Index) IsNot Nothing Then
                    Me.Sections.FxRenderableSimples(Index).PVertexDescriptor.Elements = New Graphics.VertexDescriptor.Element(Value.Count - 1) {}
                    For j = 0 To Value.Count - 1
                        Me.Sections.FxRenderableSimples(Index).PVertexDescriptor.Elements(j) = New Graphics.VertexDescriptor.Element(Value(j))
                    Next j
                End If
            End Set
        End Property

        Public Property VertexFormats As List(Of VertexElement())
            Get
                Dim m_Lists As New List(Of VertexElement())
                If Me.Sections.FxRenderableSimples IsNot Nothing Then
                    For i = 0 To Me.Sections.FxRenderableSimples.Count - 1
                        m_Lists.Add(Me.Sections.FxRenderableSimples(i).PVertexDescriptor.Elements)
                    Next
                End If

                Return m_Lists
            End Get
            Set
                If Me.Sections.FxRenderableSimples IsNot Nothing AndAlso Me.Sections.FxRenderableSimples.Count = Value.Count Then
                    For i = 0 To Me.Sections.FxRenderableSimples.Count - 1
                        Me.Sections.FxRenderableSimples(i).PVertexDescriptor.Elements = New Graphics.VertexDescriptor.Element(Value(i).Count - 1) {}
                        For j = 0 To Value(i).Count - 1
                            Me.Sections.FxRenderableSimples(i).PVertexDescriptor.Elements(j) = New Graphics.VertexDescriptor.Element(Value(i)(j))
                        Next j
                    Next i
                End If
            End Set
        End Property

        Public ReadOnly Property NameTable As List(Of FIFALibrary22.NameTable)
            Get
                Return Me.Sections.ArenaDictionary.GetNameTable()
            End Get
            'Set
            '     = Value
            'End Set
        End Property

        Public Property MeshName(ByVal MeshIndex As UInteger) As String
            Get
                If Me.Sections.ArenaDictionary IsNot Nothing Then
                    Return Me.Sections.ArenaDictionary.GetNameByType(Rw.SectionTypeCode.EA_FxShader_FxRenderableSimple, MeshIndex)
                End If

                Return ""
            End Get
            Set
                Me.Sections.ArenaDictionary.SetNameByType(Value, Rw.SectionTypeCode.EA_FxShader_FxRenderableSimple, MeshIndex)
            End Set
        End Property

        Public Property TextureName(ByVal TextureIndex As UInteger) As String
            Get
                If Me.Sections.ArenaDictionary IsNot Nothing Then
                    Return Me.Sections.ArenaDictionary.GetNameByType(Rw.SectionTypeCode.RWGOBJECTTYPE_RASTER, TextureIndex)
                End If

                Return ""
            End Get
            Set
                Me.Sections.ArenaDictionary.SetNameByType(Value, Rw.SectionTypeCode.RWGOBJECTTYPE_RASTER, TextureIndex)
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
                If Me.Sections.Rasters IsNot Nothing Then
                    Return Me.Sections.Rasters.Count
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

        Public Property FileHeader As ArenaFileHeader
        Friend ReadOnly ArenaDictEntries As List(Of ArenaDictEntry) = New List(Of ArenaDictEntry)
        Public Property SectionManifest As ArenaSectionManifest
        Public Property Sections As New RwSections(Me)

        Public Property AssetId As UInteger     'Id
        Public Property NumEntries As UInteger
        Public Property NumUsed As UInteger
        Public Property Alignment As UInteger  ' (in bytes)
        Public Property Virt As UInteger  '0 - offset to file start?
        Public Property OffsetDict As UInteger
        Public Property OffsetSectManifest As UInteger
        Public Property Base As UInteger  ' (0)
        Public Property UnfixContext As UInteger  ' (0)
        Public Property FixContext As UInteger  ' (0)
        Public Property ResourceDescriptor As ResourceDescriptor '() = New ResourceDescriptor(5 - 1) {}
        Public Property ResourcesUsed As ResourceDescriptor '() = New ResourceDescriptor(5 - 1) {}
        Public Property Resource As TargetResource 'UInteger() = New UInteger(5 - 1) {}
        Public Property ArenaGroup As UInteger  ' (0)

        Friend Property UseRwBuffers As Boolean = True      '-- rx2: True , Rx3Hybrid: False

    End Class
End Namespace