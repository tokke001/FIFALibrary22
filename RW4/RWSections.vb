Imports FIFALibrary22.Rw.Bxd
Imports FIFALibrary22.Rw.Collision
Imports FIFALibrary22.Rw.Core.Arena
Imports FIFALibrary22.Rw.EA
Imports FIFALibrary22.Rw.EA.FxShader
Imports FIFALibrary22.Rw.Graphics
Imports FIFALibrary22.Rw.OldAnimation

Namespace Rw
    Public Class RWSections
        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.Load(r)
        End Sub

        Private RwArena As Rw.Core.Arena.Arena
        Private ReadOnly Objects As New List(Of RWObject)
        Private ReadOnly _NumObjects As Object
        Public Const INDEX_OBJECT As Integer = 0
        Public Const INDEX_NO_OBJECT As Integer = 1
        Public Const INDEX_SUB_REFERENCE As Integer = 2

        Public Sub Load(ByVal r As FileReader)
            ' First we must create the objects
            ' Don't read the objects themselves as they might reference an object that does not yet exist.
            For Each Info As ArenaDictEntry In Me.RwArena.ArenaDictEntries
                Dim m_Object As RWObject = CreateObject(Info.TypeId)

                If m_Object IsNot Nothing Then
                    m_Object.SectionInfo = Info
                    'If (m_Object.SectionInfo.Alignment <> m_Object.GetAlignment()) Or (m_Object.SectionInfo.Reloc <> 0) Then
                    '    MsgBox("test")
                    'End If
                Else
                    Console.Error.WriteLine("Unrecognised RW section type: 0x" & Info.TypeId.ToString("x"))
                End If

                Objects.Add(m_Object)

                Console.WriteLine("0x" & Info.TypeId.ToString("x") & vbTab & Info.Offset)
            Next Info

            ' Now that all objects have been created, read the sub references
            'Me.RW4SectionManifest.SubReferences.LoadReferences(r)


            ' Read the objects
            For Each m_object As RWObject In Objects

                If m_object IsNot Nothing Then
                    r.BaseStream.Position = m_object.SectionInfo.Offset
                    m_object.Load(r)
                End If
            Next m_object
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            For Each m_Object As RWObject In Me.Objects

                m_Object.SectionInfo = New ArenaDictEntry
                m_Object.SectionInfo.Alignment = m_Object.GetAlignment()
                m_Object.SectionInfo.TypeId = m_Object.GetTypeCode()
                m_Object.SectionInfo.TypeIndex = Me.RwArena.SectionManifest.Types.TypeCodes.IndexOf(m_Object.SectionInfo.TypeId) '--> 'index of type at sectionManifest.ArenaSectionTypes.TypeCodes()
                'm_Object.SectionInfo.Reloc = 0

                '4.1.1 - Add padding (so alignment is oke)   'put alignment of previous section before current starts: because current "Alignment" value is used before this section
                FifaUtil.WriteAlignment(w, m_Object.SectionInfo.Alignment)
                '4.1.2 - Save Offset Section (to Section info)
                m_Object.SectionInfo.Offset = w.BaseStream.Position

                m_Object.Save(w)

                m_Object.SectionInfo.Size = w.BaseStream.Position - m_Object.SectionInfo.Offset
            Next m_Object





            'Dim num1_VB As Integer = 0
            'Dim num2_IB As Integer = 0
            'Dim num3 As Integer = 0
            'Dim num4 As Integer = 0
            'Dim num5 As Integer = 0
            'Dim num6 As Integer = 0
            'Dim num7 As Integer = 0
            'Dim num8_RAS As Integer = 0
            'Dim num9 As Integer = 0
            'Dim num10 As Integer = 0
            'Dim num11 As Integer = 0
            'Dim num12 As Integer = 0
            'Dim num13 As Integer = 0
            'Dim num14 As Integer = 0
            'Dim num15 As Integer = 0
            'Dim num16 As Integer = 0
            'Dim num17 As Integer = 0
            'Dim num18 As Integer = 0
            'Dim num19 As Integer = 0
            'Dim num20 As Integer = 0
            'Dim num21 As Integer = 0
            'Dim num22 As Integer = 0
            'Dim num23 As Integer = 0
            'Dim num24 As Integer = 0
            'Dim num25 As Integer = 0
            'Dim num26 As Integer = 0
            'Dim OffsetBuffers As UInteger = 0

            ''4 - Save Sections
            'For j = 0 To Me.ArenaDictEntries.Length - 1

            '    '4.1 - Add padding + Save Offset Section (to Section info)
            '    If Me.ArenaDictEntries(j).TypeId = 0 Then
            '        Continue For
            '    ElseIf Me.ArenaDictEntries(j).TypeId = SectionTypeCode.RWOBJECTTYPE_BUFFER Then
            '        Me.ArenaDictEntries(j).Offset = OffsetBuffers
            '    Else
            '        '4.1.1 - Add padding (so alignment is oke)   'put alignment of previous section before current starts: because current "Alignment" value is used before this section
            '        FifaUtil.WriteAlignment(w, Me.ArenaDictEntries(j).Alignment)
            '        '4.1.2 - Save Offset Section (to Section info)
            '        Me.ArenaDictEntries(j).Offset = w.BaseStream.Position
            '    End If

            '    '4.2 - Save Section
            '    Select Case Me.ArenaDictEntries(j).TypeId

            '        Case SectionTypeCode.RWOBJECTTYPE_BUFFER    'texture / vertex / index buffer       'present at end of file / Rx3 sections
            '            'Continue For

            '        Case 0  'FIFA 07 "stadium_141_3_container_0.rx2"
            '            Continue For

            '        Case SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR
            '            Me.Sections.VertexDescriptor(num5).Save(w)
            '            num5 += 1

            '        Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
            '            Me.Sections.VertexBuffer(num1_VB).Save(w)
            '            num1_VB += 1

            '        Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
            '            Me.Sections.IndexBuffer(num2_IB).Save(w)
            '            num2_IB += 1

            '        Case SectionTypeCode.RWGOBJECTTYPE_MESH
            '            Me.Sections.RW4Meshes(num3).Save(w)
            '            num3 += 1

            '        Case SectionTypeCode.EA_FxShader_FxRenderableSimple
            '            Me.Sections.FxRenderableSimple(num4).Save(w)
            '            num4 += 1

            '        Case SectionTypeCode.OBJECTTYPE_SKELETON
            '            Me.Sections.RW4Skeletons(num7).Save(w)
            '            num7 += 1

            '        Case SectionTypeCode.OBJECTTYPE_ANIMATIONSKIN    'BONE_MATRICES
            '            Me.Sections.AnimationSkin(num6).Save(w)
            '            num6 += 1

            '        Case SectionTypeCode.EA_HOTSPOT
            '            Me.Sections.RW4HotSpot.Save(w)

            '        Case SectionTypeCode.RWGOBJECTTYPE_RASTER
            '            Me.Sections.Raster(num8_RAS).Save(w)
            '            num8_RAS += 1

            '        Case SectionTypeCode.EA_ArenaDictionary
            '            Me.Sections.ArenaDictionary.Save(w)

            '        Case SectionTypeCode.OBJECTTYPE_SKINMATRIXBUFFER
            '            Me.Sections.RW4SkinMatrixBuffers(num9).Save(w)
            '            num9 += 1

            '        Case SectionTypeCode.EA_FxShader_ParameterBlockDescriptor  '2
            '            Me.RW4Shader_ParameterBlockDescriptors(num10).Save(w)      'ALWAYS little endian (set in section) !
            '            num10 += 1

            '        Case SectionTypeCode.EA_FxShader_ParameterBlock     '2
            '            Me.RW4Shader_ParameterBlocks(num11).Save(w) ', Me.RW4Shader_ParameterBlockDescriptors(num11))
            '            num11 += 1

            '        Case SectionTypeCode.EA_FxShader_FxMaterial           '2
            '            Me.Sections.RW4Shader_FxMaterials(num12).Save(w)
            '            num12 += 1

            '        Case SectionTypeCode.RWCOBJECTTYPE_VOLUME      '2
            '            Me.RW4Volumes(num13).Save(w)
            '            num13 += 1

            '        Case SectionTypeCode.RWCOBJECTTYPE_SIMPLEMAPPEDARRAY   '2
            '            Me.RW4SimpleMappedArrays(num14).Save(w)
            '            num14 += 1

            '        Case SectionTypeCode.MODELINSTANCE_ARENAID '2
            '            Me.RW4ModelInstances(num15).Save(w)
            '            num15 += 1

            '        Case SectionTypeCode.MODELSKELETON_ARENAID '2
            '            Me.RW4ModelSkeletons(num16).Save(w)
            '            num16 += 1

            '        Case SectionTypeCode.MODELSKELETONPOSE_ARENAID '2
            '            Me.RW4ModelSkeletonPoses(num17).Save(w)
            '            num17 += 1

            '        Case SectionTypeCode.MODELRENDER_ARENAID        '2
            '            Me.RW4ModelRenders(num18).Save(w)
            '            num18 += 1

            '        Case SectionTypeCode.MODELCOLLISION_ARENAID   '2
            '            Me.RW4ModelCollisions(num19).Save(w)
            '            num19 += 1

            '        Case SectionTypeCode.SPLINE_ARENAID '2
            '            Me.RW4Splines(num20).Save(w)
            '            num20 += 1

            '        Case SectionTypeCode.SCENELAYER_ARENAID '2
            '            Me.RW4SceneLayers(num21).Save(w)
            '            num21 += 1

            '        Case SectionTypeCode.LOCATION_ARENAID '2
            '            Me.RW4Locations(num22).Save(w)
            '            num22 += 1

            '        Case SectionTypeCode.CULLINFO_ARENAID   '1
            '            Me.RW4CullInfo.Save(w)
            '            'num23 += 1

            '        Case SectionTypeCode.CAMERA_ARENAID
            '            Me.RW4Cameras(num24).Save(w)
            '            num24 += 1

            '        Case SectionTypeCode.CHANNELCURVE_ARENAID
            '            Me.RW4ChannelCurves(num25).Save(w)
            '            num25 += 1

            '        Case SectionTypeCode.ANIMSEQ_ARENAID
            '            Me.RW4AnimSeqs(num26).Save(w)
            '            num26 += 1

            '        Case Else
            '            MsgBox("Error at saving Rx3File: Unknown RW4 section found - " & Me.ArenaDictEntries(j).TypeId.ToString)
            '            Continue For

            '    End Select

            '    '4.3 - Save Size Section (to Section info)
            '    If Me.ArenaDictEntries(j).TypeId = SectionTypeCode.RWOBJECTTYPE_BUFFER Then
            '        If j + 1 <= Me.ArenaDictEntries.Length Then
            '            Dim IdType As UInteger = 0
            '            Select Case Me.ArenaDictEntries(j + 1).TypeId
            '                Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
            '                    IdType = num1_VB
            '                Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
            '                    IdType = num2_IB
            '                Case SectionTypeCode.RWGOBJECTTYPE_RASTER
            '                    IdType = num8_RAS
            '            End Select
            '            Me.ArenaDictEntries(j).Size = GetSizeRWBuffer(Me.ArenaDictEntries(j).Alignment, Me.ArenaDictEntries(j + 1).TypeId, IdType)
            '            OffsetBuffers += Me.ArenaDictEntries(j).Size
            '        End If
            '    Else
            '        Me.ArenaDictEntries(j).Size = w.BaseStream.Position - Me.ArenaDictEntries(j).Offset
            '    End If

            'Next j
        End Sub

        Friend Sub WriteSectionInfos(ByVal w As FileWriter)
            For i = 0 To Objects.Count - 1
                Me.Objects(i).SectionInfo.Save(w)
            Next
        End Sub

        Private Function GetSizeRWBuffer(ByVal Alignment As UInteger, ByVal TypeBuffer As SectionTypeCode, ByVal IdType As UInteger) As UInteger  'ByVal StartOffset As UInteger,
            Dim ReturnValue As UInteger = 0
            Select Case TypeBuffer
                Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
                    ReturnValue = Me.VertexBuffers(IdType).VertexStride * Me.VertexBuffers(IdType).NumVertices

                    Do While ReturnValue Mod Alignment <> 0   'calculate padding
                        ReturnValue += 1
                    Loop

                Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
                    ReturnValue = Me.IndexBuffers(IdType).IndexStride * Me.IndexBuffers(IdType).NumIndices

                    Do While ReturnValue Mod Alignment <> 0   'calculate padding
                        ReturnValue += 1
                    Loop

                Case SectionTypeCode.RWGOBJECTTYPE_RASTER
                    Dim width As Integer = Me.Rasters(IdType).D3d.Format.Width
                    Dim height As Integer = Me.Rasters(IdType).D3d.Format.Height
                    'Dim testvalue = totalsize
                    'Do While testvalue Mod 8192 <> 0   'calculate padding
                    'ReturnValue += 1
                    'testvalue += 1

                    'Loop
                    'Dim returnvalue_2 = 0
                    For i = 0 To Me.Rasters(IdType).NumMipLevels - 1
                        ReturnValue += GraphicUtil.GetTextureSize(width, height, GraphicUtil.GetEFromRWTextureFormat(Me.Rasters(IdType).D3d.Format.TextureFormat))

                        width \= 2
                        height \= 2
                    Next i
                    'totalsize += returnvalue_2
                    'ReturnValue += returnvalue_2

                    'If Me.Raster(IdType).NumLevels = 1 Then
                    'ReturnValue = (GraphicUtil.GetTextureSize(Me.Raster(IdType).Width, Me.Raster(IdType).Height, GraphicUtil.GetEFromRWTextureFormat(Me.Raster(IdType).TextureFormat)))
                    'Else
                    'ReturnValue = (GraphicUtil.GetTextureSize(Me.Raster(IdType).Width, Me.Raster(IdType).Height, GraphicUtil.GetEFromRWTextureFormat(Me.Raster(IdType).TextureFormat)) * (Me.Raster(IdType).NumLevels - 1))
                    'End If

                    'If ReturnValue Mod Alignment <> 0 Then
                    'ReturnValue += 4096
                    'End If
                    'ReturnValue += 1
                    'testvalue = totalsize
                    'ReturnValue -= 2
                    'ReturnValue += totalsize
                    'totalsize = 0
                    Do While ReturnValue Mod 8192 <> 0   'calculate padding
                        ReturnValue += 1

                    Loop
                    'ReturnValue += 1
                    'Do While ReturnValue Mod (4096 * Me.Raster(IdType).NumLevels) <> 0   'calculate padding
                    'ReturnValue += 1
                    'Loop
                    'If Me.Raster(IdType).NumLevels > 1 Then
                    'For i = 0 To Me.Raster(IdType).NumLevels - 1
                    'ReturnValue += 4096

                    'Next
                    'End If
                    'If StartOffset Mod Alignment <> 0 Then
                    'ReturnValue += 4096
                    'End If

            End Select

            Return ReturnValue
        End Function

        Public Function CreateObject(ByVal typeCode As Rw.SectionTypeCode) As RWObject
            Select Case typeCode
                Case Rw.Core.Arena.Buffer.TYPE_CODE     'texture / vertex / index buffer       'present at end of file / Rx3 sections
                    Return New Rw.Core.Arena.Buffer(Me.RwArena)

                'Case 0  'FIFA 07 "stadium_141_3_container_0.rx2"
                '    Return Nothing

                Case VertexDescriptor.TYPE_CODE 'Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR
                    Return New VertexDescriptor(Me.RwArena)

                Case VertexBuffer.TYPE_CODE
                    Return New VertexBuffer(Me.RwArena)

                Case IndexBuffer.TYPE_CODE
                    Return New IndexBuffer(Me.RwArena)

                Case EmbeddedMesh.TYPE_CODE
                    Return New EmbeddedMesh(Me.RwArena)

                Case FxRenderableSimple.TYPE_CODE
                    Return New FxRenderableSimple(Me.RwArena)

                Case OldAnimation.Skeleton.TYPE_CODE
                    Return New OldAnimation.Skeleton(Me.RwArena)

                Case AnimationSkin.TYPE_CODE    'BONE_MATRICES
                    Return New AnimationSkin(Me.RwArena)

                Case HotSpot.TYPE_CODE
                    Return New HotSpot(Me.RwArena)

                Case Raster.TYPE_CODE
                    Return New Raster(Me.RwArena)

                Case ArenaDictionary.TYPE_CODE
                    Return New ArenaDictionary(Me.RwArena)

                Case SkinMatrixBuffer.TYPE_CODE
                    Return New SkinMatrixBuffer(Me.RwArena)

                Case ParameterBlockDescriptor.TYPE_CODE
                    Return New ParameterBlockDescriptor(Me.RwArena)

                Case ParameterBlock.TYPE_CODE
                    Return New ParameterBlock(Me.RwArena)

                Case FxMaterial.TYPE_CODE
                    Return New FxMaterial(Me.RwArena)

                Case Volume.TYPE_CODE
                    Return New Volume(Me.RwArena)

                Case SimpleMappedArray.TYPE_CODE
                    Return New SimpleMappedArray(Me.RwArena)

                Case Instance.TYPE_CODE
                    Return New Instance(Me.RwArena)

                Case Bxd.Skeleton.TYPE_CODE
                    Return New Bxd.Skeleton(Me.RwArena)

                Case Skeletonpose.TYPE_CODE
                    Return New Skeletonpose(Me.RwArena)

                Case RenderModel.TYPE_CODE
                    Return New RenderModel(Me.RwArena)

                Case CollisionModel.TYPE_CODE
                    Return New CollisionModel(Me.RwArena)

                Case Spline.TYPE_CODE
                    Return New Spline(Me.RwArena)

                Case SceneLayer.TYPE_CODE
                    Return New SceneLayer(Me.RwArena)

                Case Location.TYPE_CODE
                    Return New Location(Me.RwArena)

                Case CullInfo.TYPE_CODE
                    Return New CullInfo(Me.RwArena)

                Case Camera.TYPE_CODE   'euro08 "stadium_167_3_container_0.rx2" "stadium_167_4_container_0.rx2" '1
                    Return New Camera(Me.RwArena)

                Case ChannelCurve.TYPE_CODE    ' 'WC2010 "stadium_213_1_container_0.rx3"  'FIFA09 "festadium_188_4_container_0.rx2" "stadium_29_1_container_0.rx2" "stadium_29_4_container_0.rx2"  'array
                    Return New ChannelCurve(Me.RwArena)

                Case AnimSeq.TYPE_CODE      'WC2010 "stadium_213_1_container_0.rx3" 'FIFA09 "festadium_188_4_container_0.rx2" (1) "stadium_29_1_container_0.rx2" (1) "stadium_29_4_container_0.rx2" (1) "stadium_175_1_container_0.rx2"  (array)  'usually exists with CHANNELCURVE_ARENAID ? -> may have links ! ?
                    Return New AnimSeq(Me.RwArena)

                Case Else
                    Return Nothing
            End Select
        End Function


        ''' Adds an object to this render ware. 
        Public Sub AddObject(ByVal m_object As RWObject)
            Objects.Add(m_object)
        End Sub
        ''' Returns the list of RenderWare objects contained in this class.
        'Public Overridable ReadOnly Property GetObjects As List(Of RWObject)
        '    Get
        '        Return objects
        '    End Get
        'End Property
        Public Function GetObjects() As List(Of RWObject)
            Return Me.Objects
        End Function
        Public Function GetObjects(ByVal m_Type As Rw.SectionTypeCode) As IList(Of RWObject) 'Of T As RWObject)(ByVal m_Type As Type(Of T)) As List(Of T)
            Return Me.Objects.FindAll(Function(x) x.GetTypeCode = m_Type)

            'Dim result As List(Of RWObject) = New List(Of RWObject)

            'For Each m_object As RWObject In Objects
            '    If m_object.GetTypeCode = m_Type Then 'm_Type.IsInstanceOfType(m_object) Then
            '        result.Add(m_object)
            '    End If
            'Next m_object

            'Return result
        End Function

        ''' Returns the object at the specified index. The first 2 bytes are used to determine
        ''' the type of index (INDEX_OBJECT, etc):
        ''' <li>If the index is of type INDEX_OBJECT, it returns an object from the objects list.
        ''' <li>If the index is of type INDEX_SUB_REFERENCE, it returns an object from the sub references list.
        ''' <li>If the index is of type INDEX_NO_OBJECT, it returns null.
        Public Overridable Function GetObject(ByVal Index As Integer) As RWObject   '--> dump: IdToObject ?
            Dim sectionType As Integer = Index >> 22

            Select Case sectionType
                Case INDEX_OBJECT
                    Return Objects(Index)
                    'Case INDEX_SUB_REFERENCE   '--> used at modelrender section
                    'Return Me.SectionManifest.SubReferences.Records(Index And &H3FFFF).ObjectId
                Case Else
                    Return Nothing
            End Select
        End Function

        Public Overridable Function GetObject(ByVal m_Type As Rw.SectionTypeCode, ByVal Index As Integer) As RWObject
            Dim sectionType As Integer = Index >> 22
            Dim m_Count As UInteger = 0

            Select Case sectionType
                Case INDEX_OBJECT
                    For Each m_object As RWObject In Objects
                        If m_object.GetTypeCode = m_Type Then
                            If m_Count = Index Then
                                Return Objects(Index)
                            End If
                            m_Count += 1
                        End If
                    Next m_object

                    'Case INDEX_SUB_REFERENCE   '--> used at modelrender section
                    'Return Me.SectionManifest.SubReferences.Records(Index And &H3FFFF).ObjectId
                Case Else
                    Return Nothing
            End Select
        End Function

        ''' Returns the index of the specified object, used for referencing. The index is assumed to
        ''' be of type INDEX_OBJECT, although it will return INDEX_NO_OBJECT if the parameter is null.
        ''' This method returns -1 if the object is not present, and it must be interpreted as an error. 
        Public Overridable Function IndexOf(ByVal m_Object As RWObject) As Integer   '--> dump: ObjectToId ?
            If m_Object Is Nothing Then
                Return -1 'INDEX_NO_OBJECT << 22
            End If

            ' We do it manually because List.indexOf calls equals(), which is not necessary
            For i As Integer = 0 To Objects.Count - 1
                If m_Object Is Objects(i) Then
                    Return i
                End If
            Next i

            Return -1
        End Function

        Public Sub SetObjects(ByVal m_Objects As IList(Of RWObject))
            If m_Objects Is Nothing Then
                Exit Sub
            End If

            Dim Index As UInteger = 0

            For i = 0 To Me.Objects.Count - 1
                If Me.Objects(i).GetTypeCode = m_Objects(Index).GetTypeCode Then
                    Me.Objects(i) = m_Objects(Index)
                End If

                Index += 1
                If Index > m_Objects.Count - 1 Then
                    Exit For
                End If
            Next i

            If Index <= m_Objects.Count - 1 Then
                For j As UInteger = Index To m_Objects.Count - 1
                    AddObject(m_Objects(Index))
                Next
            End If
        End Sub

        Public Sub SetObject(ByVal m_Object As RWObject, ByVal Index As Integer)
            Dim m_Count As UInteger = 0

            If Index <= Objects.Count - 1 Then
                For i = 0 To Me.Objects.Count - 1
                    If Me.Objects(i).GetTypeCode = m_Object.GetTypeCode Then
                        If m_Count = Index Then
                            Me.Objects(i) = m_Object
                            Exit Sub
                        End If
                        m_Count += 1
                    End If
                Next i
            Else
                AddObject(m_Object)
            End If
        End Sub

        Public ReadOnly Property NumObjects
            Get
                Return Me.Objects.Count
            End Get
        End Property


        '-- Graphics ----------------------
        'Public ReadOnly Property VertexBuffers(ByVal Index As UInteger) As VertexBuffer
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER, Index)
        '    End Get

        'End Property
        Public Property VertexBuffers As List(Of VertexBuffer)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property IndexBuffers As List(Of IndexBuffer)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property EmbeddedMeshes As List(Of EmbeddedMesh)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.RWGOBJECTTYPE_MESH)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Rasters As List(Of Raster)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.RWGOBJECTTYPE_RASTER)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property VertexDescriptors As List(Of VertexDescriptor)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property
        '-- EA ----------------------
        Public Property HotSpot As HotSpot
            Get
                Return Me.GetObject(Rw.SectionTypeCode.EA_HOTSPOT, 0)
            End Get
            Set
                Me.SetObject(Value, 0)
            End Set
        End Property

        Public Property ArenaDictionary As ArenaDictionary
            Get
                Return Me.GetObject(Rw.SectionTypeCode.EA_ArenaDictionary, 0)
            End Get
            Set
                Me.SetObject(Value, 0)
            End Set
        End Property
        '-- EA_FxShader ----------------------
        Public Property FxRenderableSimples As List(Of FxRenderableSimple)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.EA_FxShader_FxRenderableSimple)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property ParameterBlockDescriptors As List(Of ParameterBlockDescriptor)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.EA_FxShader_ParameterBlockDescriptor)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property ParameterBlocks As List(Of ParameterBlock)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.EA_FxShader_ParameterBlock)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property FxMaterials As List(Of FxMaterial)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.EA_FxShader_FxMaterial)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property
        '-- OldAnimation ----------------------
        Public Property AnimationSkins As List(Of AnimationSkin)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.OBJECTTYPE_ANIMATIONSKIN)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Skeletons As List(Of OldAnimation.Skeleton)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.OBJECTTYPE_SKELETON)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property SkinMatrixBuffers As List(Of SkinMatrixBuffer)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.OBJECTTYPE_SKINMATRIXBUFFER)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property
        '-- Collision ----------------------
        Public Property Volumes As List(Of Volume)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.RWCOBJECTTYPE_VOLUME)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property SimpleMappedArrays As List(Of SimpleMappedArray)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.RWCOBJECTTYPE_SIMPLEMAPPEDARRAY)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property
        '-- Bxd ----------------------
        Public Property ModelInstances As List(Of Instance)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.MODELINSTANCE_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property ModelSkeletons As List(Of Bxd.Skeleton)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.MODELSKELETON_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property ModelSkeletonPoses As List(Of Skeletonpose)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.MODELSKELETONPOSE_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property ModelRenders As List(Of RenderModel)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.MODELRENDER_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property ModelCollisions As List(Of CollisionModel)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.MODELCOLLISION_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Splines As List(Of Spline)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.SPLINE_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property SceneLayers As List(Of SceneLayer)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.SCENELAYER_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Locations As List(Of Location)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.LOCATION_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property CullInfos As List(Of CullInfo)   '1 or array ?
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.CULLINFO_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Cameras As List(Of Camera)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.CAMERA_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property ChannelCurves As List(Of ChannelCurve)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.CHANNELCURVE_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property AnimSeqs As List(Of AnimSeq)
            Get
                Return Me.GetObjects(Rw.SectionTypeCode.ANIMSEQ_ARENAID)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property


        Public Property VertexBuffers(ByVal Index As UInteger) As VertexBuffer
            Get
                Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property IndexBuffers(ByVal Index As UInteger) As IndexBuffer
            Get
                Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property EmbeddedMeshes(ByVal Index As UInteger) As EmbeddedMesh
            Get
                Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_MESH, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Rasters(ByVal Index As UInteger) As Raster
            Get
                Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_RASTER, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property VertexDescriptors(ByVal Index As UInteger) As VertexDescriptor
            Get
                Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        '-- EA_FxShader ----------------------
        Public Property FxRenderableSimples(ByVal Index As UInteger) As FxRenderableSimple
            Get
                Return Me.GetObject(Rw.SectionTypeCode.EA_FxShader_FxRenderableSimple, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property ParameterBlockDescriptors(ByVal Index As UInteger) As ParameterBlockDescriptor
            Get
                Return Me.GetObject(Rw.SectionTypeCode.EA_FxShader_ParameterBlockDescriptor, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property ParameterBlocks(ByVal Index As UInteger) As ParameterBlock
            Get
                Return Me.GetObject(Rw.SectionTypeCode.EA_FxShader_ParameterBlock, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property FxMaterials(ByVal Index As UInteger) As FxMaterial
            Get
                Return Me.GetObject(Rw.SectionTypeCode.EA_FxShader_FxMaterial, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property
        '-- OldAnimation ----------------------
        Public Property AnimationSkins(ByVal Index As UInteger) As AnimationSkin
            Get
                Return Me.GetObject(Rw.SectionTypeCode.OBJECTTYPE_ANIMATIONSKIN, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Skeletons(ByVal Index As UInteger) As OldAnimation.Skeleton
            Get
                Return Me.GetObject(Rw.SectionTypeCode.OBJECTTYPE_SKELETON, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property SkinMatrixBuffers(ByVal Index As UInteger) As SkinMatrixBuffer
            Get
                Return Me.GetObject(Rw.SectionTypeCode.OBJECTTYPE_SKINMATRIXBUFFER, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property
        '-- Collision ----------------------
        Public Property Volumes(ByVal Index As UInteger) As Volume
            Get
                Return Me.GetObject(Rw.SectionTypeCode.RWCOBJECTTYPE_VOLUME, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property SimpleMappedArrays(ByVal Index As UInteger) As SimpleMappedArray
            Get
                Return Me.GetObject(Rw.SectionTypeCode.RWCOBJECTTYPE_SIMPLEMAPPEDARRAY, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property
        '-- Bxd ----------------------
        Public Property ModelInstances(ByVal Index As UInteger) As Instance
            Get
                Return Me.GetObject(Rw.SectionTypeCode.MODELINSTANCE_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property ModelSkeletons(ByVal Index As UInteger) As Bxd.Skeleton
            Get
                Return Me.GetObject(Rw.SectionTypeCode.MODELSKELETON_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property ModelSkeletonPoses(ByVal Index As UInteger) As Skeletonpose
            Get
                Return Me.GetObject(Rw.SectionTypeCode.MODELSKELETONPOSE_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property ModelRenders(ByVal Index As UInteger) As RenderModel
            Get
                Return Me.GetObject(Rw.SectionTypeCode.MODELRENDER_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property ModelCollisions(ByVal Index As UInteger) As CollisionModel
            Get
                Return Me.GetObject(Rw.SectionTypeCode.MODELCOLLISION_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Splines(ByVal Index As UInteger) As Spline
            Get
                Return Me.GetObject(Rw.SectionTypeCode.SPLINE_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property SceneLayers(ByVal Index As UInteger) As SceneLayer
            Get
                Return Me.GetObject(Rw.SectionTypeCode.SCENELAYER_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Locations(ByVal Index As UInteger) As Location
            Get
                Return Me.GetObject(Rw.SectionTypeCode.LOCATION_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property CullInfos(ByVal Index As UInteger) As CullInfo   '1 or array ?
            Get
                Return Me.GetObject(Rw.SectionTypeCode.CULLINFO_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Cameras(ByVal Index As UInteger) As Camera
            Get
                Return Me.GetObject(Rw.SectionTypeCode.CAMERA_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property ChannelCurves(ByVal Index As UInteger) As ChannelCurve
            Get
                Return Me.GetObject(Rw.SectionTypeCode.CHANNELCURVE_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property AnimSeqs(ByVal Index As UInteger) As AnimSeq
            Get
                Return Me.GetObject(Rw.SectionTypeCode.ANIMSEQ_ARENAID, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property
    End Class
End Namespace