Imports FIFALibrary22.Rw.Bxd
Imports FIFALibrary22.Rw.Collision
Imports FIFALibrary22.Rw.Core.Arena
Imports FIFALibrary22.Rw.EA
Imports FIFALibrary22.Rw.EA.FxShader
Imports FIFALibrary22.Rw.Graphics
Imports FIFALibrary22.Rw.OldAnimation

Namespace Rw
    Public Class RwSections
        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            Me.RwArena = RwArena
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader, Optional LoadBuffers As Boolean = True)
            Me.RwArena = RwArena
            Me.Load(LoadBuffers, r)
        End Sub

        Private RwArena As Rw.Core.Arena.Arena
        Private ReadOnly Objects As New List(Of RwObject)
        Private _AnimationSkins As List(Of AnimationSkin)
        Private _AnimSeqs As List(Of AnimSeq)
        Private _ArenaDictionary As ArenaDictionary
        Private _Cameras As List(Of Camera)
        Private _ChannelCurves As List(Of ChannelCurve)
        Private _CullInfos As List(Of CullInfo)   '1 or array ?
        Private _EmbeddedMeshes As List(Of EmbeddedMesh)
        Private _FxMaterials As List(Of FxMaterial)
        Private _FxRenderableSimples As List(Of FxRenderableSimple)
        Private _HotSpot As HotSpot
        Private _IndexBuffers As List(Of IndexBuffer)
        Private _Locations As List(Of Location)
        Private _ModelCollisions As List(Of CollisionModel)
        Private _ModelInstances As List(Of Instance)
        Private _ModelRenders As List(Of RenderModel)
        Private _ModelSkeletonPoses As List(Of Skeletonpose)
        Private _ModelSkeletons As List(Of Bxd.Skeleton)
        Private _ParameterBlockDescriptors As List(Of ParameterBlockDescriptor)
        Private _ParameterBlocks As List(Of ParameterBlock)
        Private _Rasters As List(Of Raster)
        Private _SceneLayers As List(Of SceneLayer)
        Private _SimpleMappedArrays As List(Of SimpleMappedArray)
        Private _Skeletons As List(Of OldAnimation.Skeleton)
        Private _SkinMatrixBuffers As List(Of SkinMatrixBuffer)
        Private _Splines As List(Of Spline)
        Private _VertexDescriptors As List(Of VertexDescriptor)
        Private _Volumes As List(Of Volume)
        Private _VertexBuffers As List(Of VertexBuffer)

        Public Const INDEX_OBJECT As Integer = 0
        Public Const INDEX_NO_OBJECT As Integer = 1
        Public Const INDEX_SUB_REFERENCE As Integer = 2

        Public Sub Load(ByVal LoadBuffers As Boolean, ByVal r As FileReader)
            ' First we must create the objects
            ' Don't read the objects themselves as they might reference an object that does not yet exist.
            For Each Info As ArenaDictEntry In Me.RwArena.ArenaDictEntries
                Dim m_Object As RwObject = CreateObject(Info.TypeId)

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

            'Read the buffers first (if requested)
            If LoadBuffers Then
                Me.LoadBuffers(r)
            End If

            ' Read the objects 
            For Each m_object As RwObject In Objects
                If m_object IsNot Nothing AndAlso m_object.SectionInfo.TypeId <> Rw.Core.Arena.Buffer.TYPE_CODE Then
                    r.BaseStream.Position = m_object.SectionInfo.Offset
                    m_object.Load(r)
                End If
            Next m_object
        End Sub

        Public Sub LoadBuffers(ByVal r As FileReader)
            ' Read the objects
            For Each m_object As RwObject In Objects
                If m_object IsNot Nothing AndAlso m_object.SectionInfo.TypeId = Rw.Core.Arena.Buffer.TYPE_CODE Then
                    r.BaseStream.Position = Me.RwArena.ResourceDescriptor.BaseResourceDescriptors(0).Size + m_object.SectionInfo.Offset
                    m_object.Load(r)
                End If
            Next m_object
        End Sub

        Public Sub Save(ByVal RwArena As Rw.Core.Arena.Arena, ByVal w As FileWriter)
            Me.RwArena = RwArena
            Dim offsetbuffers As UInteger = 0

            Me.SetObjectsFromMemory()

            For i = 0 To Objects.Count - 1
                Dim m_Object As RwObject = Me.Objects(i)

                If m_Object IsNot Nothing AndAlso Not m_Object.GetTypeCode() = Rw.Core.Arena.Buffer.TYPE_CODE Then
                    m_Object.SectionInfo = New ArenaDictEntry
                    m_Object.SectionInfo.TypeId = m_Object.GetTypeCode()
                    m_Object.SectionInfo.TypeIndex = Me.RwArena.SectionManifest.Types.TypeCodes.IndexOf(m_Object.SectionInfo.TypeId) '--> 'index of type at sectionManifest.ArenaSectionTypes.TypeCodes()
                    'm_Object.SectionInfo.Reloc = 0

                    m_Object.SectionInfo.Alignment = m_Object.GetAlignment()

                    '4.1.1 - Add padding (so alignment is oke)   'put alignment of previous section before current starts: because current "Alignment" value is used before this section
                    FifaUtil.WriteAlignment(w, m_Object.SectionInfo.Alignment)
                    '4.1.2 - Save Offset Section (to Section info)
                    m_Object.SectionInfo.Offset = w.BaseStream.Position

                    m_Object.RwArena = Me.RwArena
                    m_Object.Save(w)

                    m_Object.SectionInfo.Size = w.BaseStream.Position - m_Object.SectionInfo.Offset
                End If


            Next i

            '-- calculate SectionInfo for buffer sections
            'For i = 0 To Objects.Count - 1
            '    Dim m_Object As RWObject = Me.Objects(i)

            '    If m_Object IsNot Nothing AndAlso m_Object.GetTypeCode() = Rw.Core.Arena.Buffer.TYPE_CODE Then
            '        m_Object.SectionInfo = New ArenaDictEntry
            '        m_Object.SectionInfo.TypeId = Rw.Core.Arena.Buffer.TYPE_CODE
            '        m_Object.SectionInfo.TypeIndex = Me.RwArena.SectionManifest.Types.TypeCodes.IndexOf(m_Object.SectionInfo.TypeId) '--> 'index of type at sectionManifest.ArenaSectionTypes.TypeCodes()
            '        'm_Object.SectionInfo.Reloc = 0

            '        Dim LinkedObject As RWObject = GetBufferType(i)
            '        m_Object.SectionInfo.Alignment = GetAlignmentBuffer(LinkedObject.GetTypeCode)

            '        m_Object.SectionInfo.Offset = offsetbuffers

            '        m_Object.SectionInfo.Size = Me.RwArena.ResourceDescriptor.BaseResourceDescriptors(0).Size + m_Object.SectionInfo.Offset
            '        Do While m_Object.SectionInfo.Size Mod 65536 <> 0   'calculate padding   65536
            '            m_Object.SectionInfo.Size += 1
            '        Loop
            '        m_Object.SectionInfo.Size += CalcSizeRWBufferNoAlignment(LinkedObject) - (Me.RwArena.ResourceDescriptor.BaseResourceDescriptors(0).Size + m_Object.SectionInfo.Offset)

            '        'If Me.RwArena.UseRwBuffers Then
            '        '    m_Object.SectionInfo.Size = GetSizeRWBuffer(CType(m_Object, Buffer).Data.Length, LinkedObject)
            '        'Else
            '        '    m_Object.SectionInfo.Size = GetSizeRWBuffer(CalcSizeRWBufferNoAlignment(LinkedObject), LinkedObject)
            '        'End If
            '        offsetbuffers += m_Object.SectionInfo.Size

            '    End If

            'Next i

            '-- calculate SectionInfo for buffer sections
            For i = 0 To Objects.Count - 1
                Dim m_Object As RwObject = Me.Objects(i)

                If m_Object IsNot Nothing AndAlso m_Object.GetTypeCode() = Rw.Core.Arena.Buffer.TYPE_CODE Then
                    Dim test_oldsize As Long = m_Object.SectionInfo.Size
                    m_Object.SectionInfo = New ArenaDictEntry
                    m_Object.SectionInfo.TypeId = Rw.Core.Arena.Buffer.TYPE_CODE
                    m_Object.SectionInfo.TypeIndex = Me.RwArena.SectionManifest.Types.TypeCodes.IndexOf(m_Object.SectionInfo.TypeId) '--> 'index of type at sectionManifest.ArenaSectionTypes.TypeCodes()
                    'm_Object.SectionInfo.Reloc = 0

                    Dim LinkedObject As RwObject = GetBufferType(i)
                    m_Object.SectionInfo.Alignment = GetAlignmentBuffer(LinkedObject.GetTypeCode)


                    'Dim datasize As UInteger = 0 '= offsetbuffers


                    '----v1
                    'Do While offsetbuffers Mod (m_Object.SectionInfo.Alignment * 2) <> 0   'calculate padding   65536
                    '    offsetbuffers += 1
                    'Loop
                    'm_Object.SectionInfo.Offset = offsetbuffers
                    'Dim SectionSize As UInteger '= 0
                    'If Me.RwArena.UseRwBuffers Then
                    '    SectionSize = GetSizeRWBuffer(CType(m_Object, Buffer).Data.Length, LinkedObject) + m_Object.SectionInfo.Size
                    'Else
                    '    SectionSize = GetSizeRWBuffer(CalcSizeRWBufferNoAlignment(LinkedObject), LinkedObject) + m_Object.SectionInfo.Size
                    'End If
                    'offsetbuffers += SectionSize
                    'm_Object.SectionInfo.Size += offsetbuffers - m_Object.SectionInfo.Offset
                    '----------

                    '-----v2
                    m_Object.SectionInfo.Offset = offsetbuffers
                    Do While offsetbuffers Mod (m_Object.SectionInfo.Alignment * 2) <> 0   'calculate padding   65536
                        offsetbuffers += 1
                        m_Object.SectionInfo.Size += 1
                    Loop
                    If Me.RwArena.UseRwBuffers Then
                        Dim SectionSize As UInteger = GetSizeRWBuffer(CType(m_Object, Buffer).Data.Length, LinkedObject) + m_Object.SectionInfo.Size
                        m_Object.SectionInfo.Size += SectionSize
                        offsetbuffers += SectionSize
                    Else
                        Dim SectionSize As UInteger = GetSizeRWBuffer(CalcSizeRWBufferNoAlignment(LinkedObject), LinkedObject) + m_Object.SectionInfo.Size
                        m_Object.SectionInfo.Size += SectionSize
                        offsetbuffers += SectionSize
                    End If
                    '---------

                End If

            Next i

        End Sub

        Public Sub SaveBuffers(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position

            For Each m_Object As RwObject In Objects
                If m_Object IsNot Nothing AndAlso m_Object.GetTypeCode() = Rw.Core.Arena.Buffer.TYPE_CODE Then

                    Do While w.BaseStream.Position <> BaseOffset + m_Object.SectionInfo.Offset
                        w.Write(CByte(0))
                    Loop

                    m_Object.Save(w)

                End If
            Next
        End Sub

        Friend Sub WriteSectionInfos(ByVal w As FileWriter)
            For i = 0 To Objects.Count - 1
                If Me.Objects(i) IsNot Nothing Then
                    Me.Objects(i).SectionInfo.Save(w)
                End If
            Next
        End Sub

        Private Function GetBufferType(ByVal IdObject As UInteger) As RwObject
            Dim ReturnValue As RwObject = Nothing

            '- Search if in next object
            If IdObject + 1 <= Me.Objects.Count - 1 Then
                ReturnValue = GetBufferType(Me.Objects(IdObject), Me.Objects(IdObject + 1))
            End If

            '- if not found yet: Search in all
            Dim SearchIndex As UInteger = 0
            Do While (ReturnValue Is Nothing) And (SearchIndex <= Me.Objects.Count - 1)
                ReturnValue = GetBufferType(Me.Objects(IdObject), Me.Objects(SearchIndex))
                SearchIndex += 1
            Loop

            Return ReturnValue
        End Function

        Private Function GetBufferType(ByVal FindBuffer As RwObject, ByVal CheckObject As RwObject) As RwObject
            Select Case CheckObject.GetTypeCode
                Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
                    If CType(CheckObject, VertexBuffer).PBuffer Is FindBuffer Then
                        Return CheckObject
                    End If
                Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
                    If CType(CheckObject, IndexBuffer).PBuffer Is FindBuffer Then
                        Return CheckObject
                    End If
                Case SectionTypeCode.RWGOBJECTTYPE_RASTER
                    If CType(CheckObject, Raster).PBuffer Is FindBuffer Then
                        Return CheckObject
                    End If
            End Select

            Return Nothing
        End Function

        Private Function GetAlignmentBuffer(ByVal BufferTypeCode As SectionTypeCode) As UInteger
            'ALIGNMENT = 4 (vertexes/indexes) or 4096 (raster)

            Select Case BufferTypeCode
                Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
                    Return 4
                Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
                    Return 4
                Case SectionTypeCode.RWGOBJECTTYPE_RASTER
                    Return 4096
            End Select

            Return 4
        End Function

        Private Function CalcSizeRWBufferNoAlignment(ByVal LinkedObject As RwObject) As UInteger
            Dim ReturnValue As UInteger = 0
            Select Case LinkedObject.GetTypeCode
                Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
                    ReturnValue = CType(LinkedObject, VertexBuffer).VertexStride * CType(LinkedObject, VertexBuffer).NumVertices

                Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
                    ReturnValue = CType(LinkedObject, IndexBuffer).IndexStride * CType(LinkedObject, IndexBuffer).NumIndices

                Case SectionTypeCode.RWGOBJECTTYPE_RASTER
                    Dim width As Integer = CType(LinkedObject, Raster).D3d.Format.Width
                    Dim height As Integer = CType(LinkedObject, Raster).D3d.Format.Height
                    'Dim testvalue = totalsize
                    'Do While testvalue Mod 8192 <> 0   'calculate padding
                    'ReturnValue += 1
                    'testvalue += 1

                    'Loop
                    'Dim returnvalue_2 = 0
                    For i = 0 To CType(LinkedObject, Raster).NumMipLevels - 1
                        ReturnValue += GraphicUtil.GetTextureSize(width, height, CType(LinkedObject, Raster).D3d.Format.TextureFormat.ToETextureFormat)

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

        Private Function GetSizeRWBuffer(ByVal DataSize As Integer, ByVal LinkedObject As RwObject) As UInteger  'ByVal StartOffset As UInteger,
            Select Case LinkedObject.GetTypeCode
                Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
                    Do While DataSize Mod 32 <> 0   'calculate padding
                        DataSize += 1
                    Loop

                Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
                    Do While DataSize Mod 16 <> 0   'calculate padding
                        DataSize += 1
                    Loop

                Case SectionTypeCode.RWGOBJECTTYPE_RASTER
                    'Do While DataSize Mod (2 ^ (CType(LinkedObject, Raster).D3d.Format.Pitch * 4)) <> 0   'calculate padding 16384 32768 65536
                    '    DataSize += 1
                    'Loop
                    '16384 32768 65536  131072  262144
                    If DataSize > 8192 Then
                        Select Case CType(LinkedObject, Raster).D3d.Format.TextureFormat
                            Case SurfaceFormat.FMT_DXN.FMT_DXT1
                                Do While DataSize Mod 32768 <> 0   'calculate padding 32768    65536
                                    DataSize += 1
                                Loop
                            Case SurfaceFormat.FMT_DXT4_5
                                Do While DataSize Mod 65536 <> 0   'calculate padding   65536
                                    DataSize += 1
                                Loop
                            Case SurfaceFormat.FMT_DXT2_3
                                Do While DataSize Mod 65536 <> 0   'calculate padding   65536	131072
                                    DataSize += 1
                                Loop
                            Case SurfaceFormat.FMT_8_8_8_8
                                Do While DataSize Mod 2048 <> 0   'calculate padding   2048     4096
                                    DataSize += 1
                                Loop
                            Case Else

                        End Select
                        'Dim Result As Long = 1
                        'Do While Result < DataSize
                        '    Result *= 2
                        'Loop
                        'DataSize = Result
                    Else
                        Do While DataSize Mod 8192 <> 0   'calculate padding   65536	131072
                            DataSize += 1
                        Loop
                    End If

            End Select

            Return DataSize
        End Function

        Public Function CreateObject(ByVal typeCode As Rw.SectionTypeCode) As RwObject
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

        Friend Function GetTotalBuffersSize() As UInteger
            Dim LastBuffer As Buffer = Me.GetObjects(Of Buffer).Last
            Return LastBuffer.SectionInfo.Offset + LastBuffer.SectionInfo.Size
        End Function

        ''' Adds an object to this render ware. 
        Public Sub AddObject(ByVal m_object As RwObject)
            Objects.Add(m_object)
        End Sub
        ''' Returns the list of RenderWare objects contained in this class.
        'Public Overridable ReadOnly Property GetObjects As List(Of RWObject)
        '    Get
        '        Return objects
        '    End Get
        'End Property
        Public Function GetObjects() As List(Of RwObject)
            Return Me.Objects
        End Function

        Public Function GetObjects(Of T As RwObject)() As List(Of T) 'Of T As RWObject)(ByVal m_Type As Type(Of T)) As List(Of T)
            'Return Me.Objects.FindAll(Function(x) x.GetTypeCode = m_Type)
            Dim result As New List(Of T)
            Dim m_Type As Type = GetType(T)

            For Each m_object As RwObject In Objects
                If m_Type.IsInstanceOfType(m_object) Then ' m_Type.IsInstanceOfType(m_object) Then 'm_object.GetTypeCode = m_Type_2.GetTypeCode Then 'm_Type.IsInstanceOfType(m_object) Then
                    result.Add(m_object)
                End If
            Next m_object

            Return result
        End Function

        'Public Function GetObjects(ByVal m_Type As Rw.SectionTypeCode) As IList(Of RWObject) 'Of T As RWObject)(ByVal m_Type As Type(Of T)) As List(Of T)
        '    Return Me.Objects.FindAll(Function(x) x.GetTypeCode = m_Type)

        '    'Dim result As List(Of RWObject) = New List(Of RWObject)

        '    'For Each m_object As RWObject In Objects
        '    '    If m_object.GetTypeCode = m_Type Then 'm_Type.IsInstanceOfType(m_object) Then
        '    '        result.Add(m_object)
        '    '    End If
        '    'Next m_object

        '    'Return result
        'End Function

        ''' Returns the object at the specified index. The first 2 bytes are used to determine
        ''' the type of index (INDEX_OBJECT, etc):
        ''' <li>If the index is of type INDEX_OBJECT, it returns an object from the objects list.
        ''' <li>If the index is of type INDEX_SUB_REFERENCE, it returns an object from the sub references list.
        ''' <li>If the index is of type INDEX_NO_OBJECT, it returns null.
        Public Overridable Function GetObject(ByVal Index As Integer) As RwObject   '--> dump: IdToObject ?
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

        'Public Overridable Function GetObject(ByVal m_Type As Rw.SectionTypeCode, ByVal Index As Integer) As RWObject
        '    Dim sectionType As Integer = Index >> 22
        '    Dim m_Count As UInteger = 0

        '    Select Case sectionType
        '        Case INDEX_OBJECT
        '            For Each m_object As RWObject In Objects
        '                If m_object.GetTypeCode = m_Type Then
        '                    If m_Count = Index Then
        '                        Return m_object '(m_Count)
        '                    End If
        '                    m_Count += 1
        '                End If
        '            Next m_object

        '            'Case INDEX_SUB_REFERENCE   '--> used at modelrender section
        '            'Return Me.SectionManifest.SubReferences.Records(Index And &H3FFFF).ObjectId
        '        Case Else
        '            Return Nothing
        '    End Select
        'End Function

        ''' Returns the index of the specified object, used for referencing. The index is assumed to
        ''' be of type INDEX_OBJECT, although it will return INDEX_NO_OBJECT if the parameter is null.
        ''' This method returns -1 if the object is not present, and it must be interpreted as an error. 
        Public Overridable Function IndexOf(ByVal m_Object As RwObject, Optional ValueNothing As Integer = -1) As Integer   '--> dump: ObjectToId ?
            If m_Object Is Nothing Then
                Return ValueNothing 'INDEX_NO_OBJECT << 22
            End If

            ' We do it manually because List.indexOf calls equals(), which is not necessary
            For i As Integer = 0 To Objects.Count - 1
                If m_Object Is Objects(i) Then
                    Return i
                End If
            Next i

            Return ValueNothing ' -1
        End Function

        Public Sub SetObjects(Of T As RwObject)(ByVal m_Objects As List(Of T))
            If m_Objects Is Nothing OrElse m_Objects.Count = 0 Then
                Exit Sub
            End If

            Dim Index As UInteger = 0
            Dim IndexSearched As UInteger = Me.Objects.Count - 1

            For i = 0 To Me.Objects.Count - 1
                If Me.Objects(i).GetTypeCode = m_Objects(Index).GetTypeCode Then
                    'IndexSearched = i      '--> location of new objects unknpwn, so added at end
                    Me.Objects(i) = m_Objects(Index)

                    If Index >= m_Objects.Count - 1 Then    'if all objects are added -> exit for
                        Exit For
                    Else
                        Index += 1
                    End If
                End If
            Next i

            If Index < m_Objects.Count - 1 Then    '- add objects
                For j As UInteger = Index To m_Objects.Count - 1
                    IndexSearched += 1
                    Me.Objects.Insert(IndexSearched, m_Objects(j))
                Next
            Else                                    '- delete objects
                Dim ListRemove As New List(Of UInteger)
                For i = IndexSearched + 1 To Me.Objects.Count - 1
                    If Me.Objects(i).GetTypeCode = m_Objects(Index).GetTypeCode Then
                        ListRemove.Add(i)
                    End If
                Next
                For i = 0 To ListRemove.Count - 1
                    Me.Objects.RemoveAt(ListRemove(i) - i)
                Next

            End If
        End Sub

        Public Sub SetObject(Of T As RwObject)(ByVal m_Object As T, ByVal Index As Integer)
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

        Private Function GetObjectFromMemory(Of T As RwObject)(ByRef m_Object As T) As T
            If m_Object Is Nothing Then
                m_Object = Me.GetObjects(Of T)(0)
            End If

            Return m_Object
        End Function

        Private Function GetObjectsFromMemory(Of T As RwObject)(ByRef m_Objects As List(Of T)) As List(Of T)
            If m_Objects Is Nothing Then
                m_Objects = Me.GetObjects(Of T)()
            End If

            Return m_Objects
        End Function

        Private Sub SetObjectsFromMemory()  'do this at saving file !
            If _HotSpot IsNot Nothing Then
                Me.SetObject(_HotSpot, 0)
            End If
            If _ArenaDictionary IsNot Nothing Then
                Me.SetObject(_ArenaDictionary, 0)
            End If

            If _AnimationSkins IsNot Nothing Then
                Me.SetObjects(_AnimationSkins)
            End If
            If _AnimSeqs IsNot Nothing Then
                Me.SetObjects(_AnimSeqs)
            End If
            If _Cameras IsNot Nothing Then
                Me.SetObjects(_Cameras)
            End If
            If _ChannelCurves IsNot Nothing Then
                Me.SetObjects(_ChannelCurves)
            End If
            If _CullInfos IsNot Nothing Then
                Me.SetObjects(_CullInfos)
            End If   '1 or array ?
            If _EmbeddedMeshes IsNot Nothing Then
                Me.SetObjects(_EmbeddedMeshes)
            End If
            If _FxMaterials IsNot Nothing Then
                Me.SetObjects(_FxMaterials)
            End If
            If _FxRenderableSimples IsNot Nothing Then
                Me.SetObjects(_FxRenderableSimples)
            End If
            If _IndexBuffers IsNot Nothing Then
                Me.SetObjects(_IndexBuffers)
            End If
            If _Locations IsNot Nothing Then
                Me.SetObjects(_Locations)
            End If
            If _ModelCollisions IsNot Nothing Then
                Me.SetObjects(_ModelCollisions)
            End If
            If _ModelInstances IsNot Nothing Then
                Me.SetObjects(_ModelInstances)
            End If
            If _ModelRenders IsNot Nothing Then
                Me.SetObjects(_ModelRenders)
            End If
            If _ModelSkeletonPoses IsNot Nothing Then
                Me.SetObjects(_ModelSkeletonPoses)
            End If
            If _ModelSkeletons IsNot Nothing Then
                Me.SetObjects(_ModelSkeletons)
            End If
            If _ParameterBlockDescriptors IsNot Nothing Then
                Me.SetObjects(_ParameterBlockDescriptors)
            End If
            If _ParameterBlocks IsNot Nothing Then
                Me.SetObjects(_ParameterBlocks)
            End If
            If _Rasters IsNot Nothing Then
                Me.SetObjects(_Rasters)
            End If
            If _SceneLayers IsNot Nothing Then
                Me.SetObjects(_SceneLayers)
            End If
            If _SimpleMappedArrays IsNot Nothing Then
                Me.SetObjects(_SimpleMappedArrays)
            End If
            If _Skeletons IsNot Nothing Then
                Me.SetObjects(_Skeletons)
            End If
            If _SkinMatrixBuffers IsNot Nothing Then
                Me.SetObjects(_SkinMatrixBuffers)
            End If
            If _Splines IsNot Nothing Then
                Me.SetObjects(_Splines)
            End If
            If _VertexDescriptors IsNot Nothing Then
                Me.SetObjects(_VertexDescriptors)
            End If
            If _Volumes IsNot Nothing Then
                Me.SetObjects(_Volumes)
            End If
            If _VertexBuffers IsNot Nothing Then
                Me.SetObjects(_VertexBuffers)
            End If

        End Sub

        '-- EA ----------------------
        Public Property HotSpot As HotSpot
            Get
                Return If(_HotSpot, Me.GetObjectFromMemory(_HotSpot))
            End Get
            Set
                _HotSpot = Value
            End Set
        End Property

        Public Property ArenaDictionary As ArenaDictionary
            Get
                Return If(_ArenaDictionary, Me.GetObjectFromMemory(_ArenaDictionary))
            End Get
            Set
                _ArenaDictionary = Value
            End Set
        End Property
        '-- Graphics ----------------------
        Public Property VertexBuffers As List(Of VertexBuffer)
            Get
                Return If(_VertexBuffers, Me.GetObjectsFromMemory(_VertexBuffers))
            End Get
            Set
                _VertexBuffers = Value
            End Set
        End Property

        Public Property IndexBuffers As List(Of IndexBuffer)
            Get
                Return If(_IndexBuffers, Me.GetObjectsFromMemory(_IndexBuffers))
            End Get
            Set
                _IndexBuffers = Value
            End Set
        End Property

        Public Property EmbeddedMeshes As List(Of EmbeddedMesh)
            Get
                Return If(_EmbeddedMeshes, Me.GetObjectsFromMemory(_EmbeddedMeshes))
            End Get
            Set
                _EmbeddedMeshes = Value
            End Set
        End Property

        Public Property Rasters As List(Of Raster)
            Get
                Return If(_Rasters, Me.GetObjectsFromMemory(_Rasters))
            End Get
            Set
                _Rasters = Value
            End Set
        End Property

        Public Property VertexDescriptors As List(Of VertexDescriptor)
            Get
                Return If(_VertexDescriptors, Me.GetObjectsFromMemory(_VertexDescriptors))
            End Get
            Set
                _VertexDescriptors = Value
            End Set
        End Property
        '-- EA_FxShader ----------------------
        Public Property FxRenderableSimples As List(Of FxRenderableSimple)
            Get
                Return If(_FxRenderableSimples, Me.GetObjectsFromMemory(_FxRenderableSimples))
            End Get
            Set
                _FxRenderableSimples = Value
            End Set
        End Property

        Public Property ParameterBlockDescriptors As List(Of ParameterBlockDescriptor)
            Get
                Return If(_ParameterBlockDescriptors, Me.GetObjectsFromMemory(_ParameterBlockDescriptors))
            End Get
            Set
                _ParameterBlockDescriptors = Value
            End Set
        End Property

        Public Property ParameterBlocks As List(Of ParameterBlock)
            Get
                Return If(_ParameterBlocks, Me.GetObjectsFromMemory(_ParameterBlocks))
            End Get
            Set
                _ParameterBlocks = Value
            End Set
        End Property

        Public Property FxMaterials As List(Of FxMaterial)
            Get
                Return If(_FxMaterials, Me.GetObjectsFromMemory(_FxMaterials))
            End Get
            Set
                _FxMaterials = Value
            End Set
        End Property
        '-- OldAnimation ----------------------
        Public Property AnimationSkins As List(Of AnimationSkin)
            Get
                Return If(_AnimationSkins, Me.GetObjectsFromMemory(_AnimationSkins))
            End Get
            Set
                _AnimationSkins = Value
            End Set
        End Property

        Public Property Skeletons As List(Of OldAnimation.Skeleton)
            Get
                Return If(_Skeletons, Me.GetObjectsFromMemory(_Skeletons))
            End Get
            Set
                _Skeletons = Value
            End Set
        End Property

        Public Property SkinMatrixBuffers As List(Of SkinMatrixBuffer)
            Get
                Return If(_SkinMatrixBuffers, Me.GetObjectsFromMemory(_SkinMatrixBuffers))
            End Get
            Set
                _SkinMatrixBuffers = Value
            End Set
        End Property
        '-- Collision ----------------------
        Public Property Volumes As List(Of Volume)
            Get
                Return If(_Volumes, Me.GetObjectsFromMemory(_Volumes))
            End Get
            Set
                _Volumes = Value
            End Set
        End Property

        Public Property SimpleMappedArrays As List(Of SimpleMappedArray)
            Get
                Return If(_SimpleMappedArrays, Me.GetObjectsFromMemory(_SimpleMappedArrays))
            End Get
            Set
                _SimpleMappedArrays = Value
            End Set
        End Property
        '-- Bxd ----------------------
        Public Property ModelInstances As List(Of Instance)
            Get
                Return If(_ModelInstances, Me.GetObjectsFromMemory(_ModelInstances))
            End Get
            Set
                _ModelInstances = Value
            End Set
        End Property

        Public Property ModelSkeletons As List(Of Bxd.Skeleton)
            Get
                Return If(_ModelSkeletons, Me.GetObjectsFromMemory(_ModelSkeletons))
            End Get
            Set
                _ModelSkeletons = Value
            End Set
        End Property

        Public Property ModelSkeletonPoses As List(Of Skeletonpose)
            Get
                Return If(_ModelSkeletonPoses, Me.GetObjectsFromMemory(_ModelSkeletonPoses))
            End Get
            Set
                _ModelSkeletonPoses = Value
            End Set
        End Property

        Public Property ModelRenders As List(Of RenderModel)
            Get
                Return If(_ModelRenders, Me.GetObjectsFromMemory(_ModelRenders))
            End Get
            Set
                _ModelRenders = Value
            End Set
        End Property

        Public Property ModelCollisions As List(Of CollisionModel)
            Get
                Return If(_ModelCollisions, Me.GetObjectsFromMemory(_ModelCollisions))
            End Get
            Set
                _ModelCollisions = Value
            End Set
        End Property

        Public Property Splines As List(Of Spline)
            Get
                Return If(_Splines, Me.GetObjectsFromMemory(_Splines))
            End Get
            Set
                _Splines = Value
            End Set
        End Property

        Public Property SceneLayers As List(Of SceneLayer)
            Get
                Return If(_SceneLayers, Me.GetObjectsFromMemory(_SceneLayers))
            End Get
            Set
                _SceneLayers = Value
            End Set
        End Property

        Public Property Locations As List(Of Location)
            Get
                Return If(_Locations, Me.GetObjectsFromMemory(_Locations))
            End Get
            Set
                _Locations = Value
            End Set
        End Property

        Public Property CullInfos As List(Of CullInfo)   '1 or array ?
            Get
                Return If(_CullInfos, Me.GetObjectsFromMemory(_CullInfos))
            End Get
            Set
                _CullInfos = Value
            End Set
        End Property

        Public Property Cameras As List(Of Camera)
            Get
                Return If(_Cameras, Me.GetObjectsFromMemory(_Cameras))
            End Get
            Set
                _Cameras = Value
            End Set
        End Property

        Public Property ChannelCurves As List(Of ChannelCurve)
            Get
                Return If(_ChannelCurves, Me.GetObjectsFromMemory(_ChannelCurves))
            End Get
            Set
                _ChannelCurves = Value
            End Set
        End Property

        Public Property AnimSeqs As List(Of AnimSeq)
            Get
                Return If(_AnimSeqs, Me.GetObjectsFromMemory(_AnimSeqs))
            End Get
            Set
                _AnimSeqs = Value
            End Set
        End Property


        'Public Property VertexBuffers(ByVal Index As UInteger) As VertexBuffer
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property IndexBuffers(ByVal Index As UInteger) As IndexBuffer
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property EmbeddedMeshes(ByVal Index As UInteger) As EmbeddedMesh
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_MESH, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Rasters(ByVal Index As UInteger) As Raster
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_RASTER, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property VertexDescriptors(ByVal Index As UInteger) As VertexDescriptor
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        ''-- EA_FxShader ----------------------
        'Public Property FxRenderableSimples(ByVal Index As UInteger) As FxRenderableSimple
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.EA_FxShader_FxRenderableSimple, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property ParameterBlockDescriptors(ByVal Index As UInteger) As ParameterBlockDescriptor
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.EA_FxShader_ParameterBlockDescriptor, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property ParameterBlocks(ByVal Index As UInteger) As ParameterBlock
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.EA_FxShader_ParameterBlock, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property FxMaterials(ByVal Index As UInteger) As FxMaterial
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.EA_FxShader_FxMaterial, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property
        ''-- OldAnimation ----------------------
        'Public Property AnimationSkins(ByVal Index As UInteger) As AnimationSkin
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.OBJECTTYPE_ANIMATIONSKIN, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Skeletons(ByVal Index As UInteger) As OldAnimation.Skeleton
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.OBJECTTYPE_SKELETON, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property SkinMatrixBuffers(ByVal Index As UInteger) As SkinMatrixBuffer
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.OBJECTTYPE_SKINMATRIXBUFFER, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property
        ''-- Collision ----------------------
        'Public Property Volumes(ByVal Index As UInteger) As Volume
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.RWCOBJECTTYPE_VOLUME, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property SimpleMappedArrays(ByVal Index As UInteger) As SimpleMappedArray
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.RWCOBJECTTYPE_SIMPLEMAPPEDARRAY, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property
        ''-- Bxd ----------------------
        'Public Property ModelInstances(ByVal Index As UInteger) As Instance
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.MODELINSTANCE_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property ModelSkeletons(ByVal Index As UInteger) As Bxd.Skeleton
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.MODELSKELETON_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property ModelSkeletonPoses(ByVal Index As UInteger) As Skeletonpose
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.MODELSKELETONPOSE_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property ModelRenders(ByVal Index As UInteger) As RenderModel
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.MODELRENDER_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property ModelCollisions(ByVal Index As UInteger) As CollisionModel
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.MODELCOLLISION_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Splines(ByVal Index As UInteger) As Spline
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.SPLINE_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property SceneLayers(ByVal Index As UInteger) As SceneLayer
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.SCENELAYER_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Locations(ByVal Index As UInteger) As Location
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.LOCATION_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property CullInfos(ByVal Index As UInteger) As CullInfo   '1 or array ?
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.CULLINFO_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Cameras(ByVal Index As UInteger) As Camera
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.CAMERA_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property ChannelCurves(ByVal Index As UInteger) As ChannelCurve
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.CHANNELCURVE_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property AnimSeqs(ByVal Index As UInteger) As AnimSeq
        '    Get
        '        Return Me.GetObject(Rw.SectionTypeCode.ANIMSEQ_ARENAID, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property
    End Class
End Namespace