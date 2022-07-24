Namespace Rx3
    Public Class Rx3Sections

        Public Sub New(ByVal Rx3File As Rx3.Rx3File, ByVal RW4Section As Rw.Core.Arena.Arena)
            Me.Rx3File = Rx3File
            Me.RW4Section = RW4Section
        End Sub

        Public Sub New(ByVal Rx3File As Rx3.Rx3File, ByVal RW4Section As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.Rx3File = Rx3File
            Me.RW4Section = RW4Section
            Me.Load(r)
        End Sub

        Private Rx3File As Rx3.Rx3File
        Private RW4Section As Rw.Core.Arena.Arena
        Private ReadOnly Objects As List(Of Rx3Object) = New List(Of Rx3Object)
        Private _Adjacencies As List(Of Adjacency)
        Private _AnimationSkins As List(Of AnimationSkin)
        'private _BoneName As List(Of  BoneName)   'No Clear Section-layout info
        Private _BoneRemaps As List(Of BoneRemap)
        Private _ClothDefs As List(Of ClothDef)
        Private _Collisions As List(Of Collision)
        Private _CollisionTriMesh As List(Of CollisionTriMesh)
        Private _EdgeMeshes As List(Of EdgeMesh)   'No Clear Section-layout info
        Private _HotSpot As HotSpot
        Private _IndexBuffers As List(Of IndexBuffer)
        Private _Locations As List(Of Location)
        Private _Materials As List(Of Material)
        Private _MorphIndexed As List(Of MorphIndexed)
        Private _NameTable As NameTable
        Private _QuadIndexBuffers As List(Of QuadIndexBuffer)
        Private _SceneAnimation As List(Of SceneAnimation)   'No Clear Section-layout info
        Private _SceneInstance As List(Of SceneInstance)
        Private _SceneLayers As List(Of SceneLayer)
        Private _SimpleMeshes As List(Of SimpleMesh)
        Private _Skeleton As List(Of Skeleton)
        Private _VertexBuffers As List(Of VertexBuffer)
        Private _VertexFormats As List(Of VertexFormat)
        Private _Textures As List(Of Texture)


        Public Sub Load(ByVal r As FileReader)
            IndexVBuffer = 0

            For Each Info As SectionHeader In Me.Rx3File.Rx3SectionHeaders
                r.BaseStream.Position = Info.Offset
                Dim m_Object As Rx3Object = CreateObject(Info, r)

                If m_Object IsNot Nothing Then
                    m_Object.SectionInfo = Info
                Else
                    Console.Error.WriteLine("Unrecognised Rx3 section type: " & Info.Signature.ToString)
                End If

                Objects.Add(m_Object)

                Console.WriteLine(Info.Signature.ToString & vbTab & Info.Offset)
            Next Info

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            IndexVBuffer = 0
            IndexBoneRemap = 0

            For Each m_Object As Rx3Object In Me.Objects

                m_Object.SectionInfo = New SectionHeader
                m_Object.SectionInfo.Signature = m_Object.GetTypeCode()
                'm_Object.SectionInfo.Unknown = 0   'padding probably

                m_Object.SectionInfo.Offset = w.BaseStream.Position

                Me.SaveObject(m_Object, w)

                m_Object.SectionInfo.Size = w.BaseStream.Position - m_Object.SectionInfo.Offset
            Next m_Object
        End Sub

        Friend Sub WriteBatchSections(ByVal w As FileWriter)
            For i = 0 To Objects.Count - 1
                Select Case Me.Objects(i).GetTypeCode
                    Case SectionHash.INDEX_BUFFER_BATCH, SectionHash.TEXTURE_BATCH, SectionHash.QUAD_INDEX_BUFFER_BATCH
                        w.BaseStream.Position = Me.Objects(i).SectionInfo.Offset
                        Me.SaveObject(Me.Objects(i), w)
                End Select
            Next
        End Sub

        Friend Sub WriteSectionInfos(ByVal w As FileWriter)
            For i = 0 To Objects.Count - 1
                Me.Objects(i).SectionInfo.Save(w)
            Next
        End Sub

        Private IndexVBuffer As UInteger = 0
        Private IndexBoneRemap As UInteger = 0

        Public Function CreateObject(ByVal Info As SectionHeader, ByVal r As FileReader) As Rx3Object
            Select Case Info.Signature
                 '----- SECTIONS since FIFA 11 -----
                Case SectionHash.INDEX_BUFFER_BATCH
                    Return New IndexBufferBatch(r)

                Case SectionHash.INDEX_BUFFER
                    Return New IndexBuffer(r)

                Case SectionHash.VERTEX_BUFFER
                    IndexVBuffer += 1
                    Return New VertexBuffer(GetVertexElements(IndexVBuffer - 1), r)

                Case SectionHash.BONE_REMAP  'Partial Skeleton   (present at all fifas)
                    Return New BoneRemap(r)

                Case SectionHash.TEXTURE_BATCH
                    Return New TextureBatch(r)

                Case SectionHash.TEXTURE
                    Return New Texture(r)

                    '----- SECTIONS since FIFA 12 -----
                Case SectionHash.MATERIAL
                    Return New Material(r)

                Case SectionHash.SCENE_LAYER
                    Return New SceneLayer(r)

                Case SectionHash.LOCATION
                    Return New Location(r)

                Case SectionHash.NAME_TABLE
                    Return New NameTable(r)

                Case SectionHash.EDGE_MESH  'found at FO3 : body\arms.rx3
                    Return New EdgeMesh(r)

                Case SectionHash.SCENE_INSTANCE
                    Return New SceneInstance(r)

                Case SectionHash.SCENE_ANIMATION    'found at wipe3d / found at stadium_250, index 675, offset 2250768 
                    Return New SceneAnimation(r)

                Case SectionHash.MORPH_INDEXED      'probably RX3_SECTION_MORPH - used in *_morphtargets files. 
                    Return New MorphIndexed(r)

                Case SectionHash.VERTEX_FORMAT
                    Return New VertexFormat(r)

                Case SectionHash.SIMPLE_MESH 'PRIMITIVE_TYPE 'loaded beforehand 
                    Return New SimpleMesh(r)

                Case SectionHash.ANIMATION_SKIN 'Bone Matrices   
                    Return New AnimationSkin(r)

                Case SectionHash.COLLISION_TRI_MESH 'F14
                    Return New CollisionTriMesh(r)

                Case SectionHash.HOTSPOT '(ex. kit positions) 
                    Return New HotSpot(r)

                Case SectionHash.COLLISION      'F12
                    Return New Collision(r)

                    '----- SECTIONS since FIFA 15 -----
                Case SectionHash.CLOTH_DEF
                    Return New ClothDef(Info.Size, r)

                Case SectionHash.SKELETON
                    Return New Skeleton(r)


                    '----- SECTIONS since FIFA 16 -----
                    'Case SectionHash.BONE_NAME   'may not exist: is maybe not a seperate section

                Case SectionHash.QUAD_INDEX_BUFFER_BATCH
                    Return New QuadIndexBufferBatch(r)

                Case SectionHash.QUAD_INDEX_BUFFER
                    Return New QuadIndexBuffer(r)

                Case SectionHash.ADJACENCY
                    Return New Adjacency(r)

                Case Else
                    'MsgBox("Error at loading Rx3File: Unknown rx3 section found - " & typeCode.ToString)
                    Return Nothing
            End Select

        End Function

        Private Function GetVertexElements(ByVal MeshIndex As UInteger) As VertexElement()
            If Me.RW4Section IsNot Nothing AndAlso Me.RW4Section.Sections.FxRenderableSimples(MeshIndex) IsNot Nothing Then
                Return Me.RW4Section.Sections.FxRenderableSimples(MeshIndex).PVertexDescriptor.Elements 'Me.RW4Section.Sections.EmbeddedMeshes(MeshIndex).PVertexBuffers(0).PVertexDescriptor.Elements
            ElseIf Me.VertexFormats(MeshIndex) IsNot Nothing Then
                Return Me.VertexFormats(MeshIndex).Elements
            End If

            Return Nothing
        End Function

        Private Sub SaveObject(ByVal m_Object As Rx3Object, ByVal w As FileWriter)
            Select Case m_Object.SectionInfo.Signature
                    '----- SECTIONS since FIFA 11 -----
                Case SectionHash.INDEX_BUFFER_BATCH
                    CType(m_Object, IndexBufferBatch).Save(Me.IndexBuffers, w)

                Case SectionHash.INDEX_BUFFER
                    CType(m_Object, IndexBuffer).Save(w)

                Case SectionHash.VERTEX_BUFFER
                    CType(m_Object, VertexBuffer).Save(GetVertexElements(IndexVBuffer), w)
                    IndexVBuffer += 1

                Case SectionHash.BONE_REMAP  'Partial Skeleton   'between index & vertex data
                    CType(m_Object, BoneRemap).Save(Me.VertexBuffers(IndexBoneRemap), CheckBoneIndicesHasShort(), w)
                    IndexBoneRemap += 1

                Case SectionHash.TEXTURE_BATCH
                    CType(m_Object, TextureBatch).Save(Me.Textures, w)

                Case SectionHash.TEXTURE
                    CType(m_Object, Texture).Save(w)

                '----- SECTIONS since FIFA 12 -----
                Case SectionHash.MATERIAL
                    CType(m_Object, Material).Save(w)

                Case SectionHash.SCENE_LAYER
                    CType(m_Object, SceneLayer).Save(w)

                Case SectionHash.LOCATION
                    CType(m_Object, Location).Save(w)

                Case SectionHash.NAME_TABLE
                    CType(m_Object, NameTable).Save(w)

                Case SectionHash.EDGE_MESH
                    CType(m_Object, EdgeMesh).Save(w)

                Case SectionHash.SCENE_INSTANCE
                    CType(m_Object, SceneInstance).Save(w)

                Case SectionHash.SCENE_ANIMATION
                    CType(m_Object, SceneAnimation).Save(w)

                Case SectionHash.MORPH_INDEXED
                    CType(m_Object, MorphIndexed).Save(w)

                Case SectionHash.VERTEX_FORMAT
                    CType(m_Object, VertexFormat).Save(w)

                Case SectionHash.SIMPLE_MESH 'PRIMITIVE_TYPE
                    CType(m_Object, SimpleMesh).Save(w)

                Case SectionHash.ANIMATION_SKIN 'Bone Matrices   
                    CType(m_Object, AnimationSkin).Save(w)

                Case SectionHash.COLLISION_TRI_MESH
                    CType(m_Object, CollisionTriMesh).Save(w)

                Case SectionHash.HOTSPOT '(ex. kit positions) 
                    CType(m_Object, HotSpot).Save(w)

                Case SectionHash.COLLISION
                    CType(m_Object, Collision).Save(w)

                '----- SECTIONS since FIFA 15 -----
                Case SectionHash.CLOTH_DEF
                    CType(m_Object, ClothDef).Save(w)

                Case SectionHash.SKELETON '(added in FIFA 15)
                    CType(m_Object, Skeleton).Save(w)


                '----- SECTIONS since FIFA 16 -----
                    'Case SectionHash.BONE_NAME   'may not exist: is maybe not a seperate section

                Case SectionHash.QUAD_INDEX_BUFFER_BATCH
                    CType(m_Object, QuadIndexBufferBatch).Save(Me.QuadIndexBuffers, w)

                Case SectionHash.QUAD_INDEX_BUFFER
                    CType(m_Object, QuadIndexBuffer).Save(w)

                Case SectionHash.ADJACENCY
                    CType(m_Object, Adjacency).Save(w)

                Case Else
                    'MsgBox("Error at saving Rx3File: Unknown rx3 section found - " & m_Object.SectionInfo.Signature.ToString)
                    Exit Sub
            End Select
        End Sub

        Private Function CheckBoneIndicesHasShort() As Boolean
            Dim NumBones = 0

            If Me.RW4Section IsNot Nothing AndAlso Me.RW4Section.Sections.AnimationSkins IsNot Nothing Then
                NumBones = Me.RW4Section.Sections.AnimationSkins(0).NumBones
            ElseIf Me.AnimationSkins IsNot Nothing Then
                NumBones = Me.AnimationSkins(0).NumBones
            End If

            If NumBones > Byte.MaxValue Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Sub AddObject(ByVal m_object As Rx3Object)
            Objects.Add(m_object)
        End Sub
        ''' Returns the list of RenderWare objects contained in this class.
        'Public Overridable ReadOnly Property GetObjects As List(Of RWObject)
        '    Get
        '        Return objects
        '    End Get
        'End Property
        Public Function GetObjects() As List(Of Rx3Object)
            Return Me.Objects
        End Function

        '        Public Overridable Function GetObjects(Of T As RWObject)(ByVal m_Type As Rw.SectionTypeCode) As List(Of T)
        Public Function GetObjects(Of T As Rx3Object)() As List(Of T) 'Of T As RWObject)(ByVal m_Type As Type(Of T)) As List(Of T)
            'Return Me.Objects.FindAll(Function(x) x.GetTypeCode = m_Type)
            Dim result As New List(Of T)
            Dim m_Type As Type = GetType(T)

            For Each m_object As Rx3Object In Objects
                If m_Type.IsInstanceOfType(m_object) Then ' m_Type.IsInstanceOfType(m_object) Then 'm_object.GetTypeCode = m_Type_2.GetTypeCode Then 'm_Type.IsInstanceOfType(m_object) Then
                    result.Add(m_object)
                End If
            Next m_object

            Return result
        End Function

        'Public Function GetObjects(ByVal m_Type As Rx3.SectionHash) As IList(Of Rx3Object) 'Of T As RWObject)(ByVal m_Type As Type(Of T)) As List(Of T)
        '    Return Me.Objects.FindAll(Function(x) x.GetTypeCode = m_Type)

        '    'Dim result As List(Of RWObject) = New List(Of RWObject)

        '    'For Each m_object As RWObject In Objects
        '    '    If m_object.GetTypeCode = m_Type Then 'm_Type.IsInstanceOfType(m_object) Then
        '    '        result.Add(m_object)
        '    '    End If
        '    'Next m_object

        '    'Return result
        'End Function

        ''' Returns the object at the specified index. 
        Public Overridable Function GetObject(ByVal Index As Integer) As Rx3Object
            Return Me.Objects(Index)
        End Function

        'Public Overridable Function GetObject(Of T As Rx3Object)(ByVal Index As Integer) As Rx3Object
        '    Dim m_Count As UInteger = 0
        '    Dim m_Type As Type = GetType(T)

        '    For Each m_object As Rx3Object In Me.Objects
        '        If m_Type.IsInstanceOfType(m_object) Then
        '            If m_Count = Index Then
        '                Return m_object
        '            End If
        '            m_Count += 1
        '        End If
        '    Next m_object

        '    Return Nothing
        'End Function

        'Public Overridable Function GetObject(ByVal m_Type As Rx3.SectionHash, ByVal Index As Integer) As Rx3Object
        '    Dim m_Count As UInteger = 0

        '    For Each m_object As Rx3Object In Me.Objects
        '        If m_object.GetTypeCode = m_Type Then
        '            If m_Count = Index Then
        '                Return m_object
        '            End If
        '            m_Count += 1
        '        End If
        '    Next m_object

        '    Return Nothing
        'End Function

        ''' Returns the index of the specified object, used for referencing. 
        ''' This method returns -1 if the object is not present, and it must be interpreted as an error. 
        Public Overridable Function IndexOf(ByVal m_Object As Rx3Object) As Integer
            If m_Object Is Nothing Then
                Return -1
            End If

            ' We do it manually because List.indexOf calls equals(), which is not necessary
            For i As Integer = 0 To Objects.Count - 1
                If m_Object Is Objects(i) Then
                    Return i
                End If
            Next i

            Return -1
        End Function

        Public Sub SetObjects(Of T As Rx3Object)(ByVal m_Objects As List(Of T))
            If m_Objects Is Nothing OrElse m_Objects.Count = 0 Then
                Exit Sub
            End If

            Dim Index As UInteger = 0
            Dim IndexSearched As UInteger = Me.Objects.Count - 1

            For i = 0 To Me.Objects.Count - 1
                If Me.Objects(i).GetTypeCode = m_Objects(Index).GetTypeCode Then
                    IndexSearched = i
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

        Public Sub SetObject(ByVal m_Object As Rx3Object, ByVal Index As Integer)
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

        Private Function GetObjectFromMemory(Of T As Rx3Object)(ByRef m_Object As T) As T
            If m_Object Is Nothing Then
                Dim m_Objects As List(Of T) = Me.GetObjects(Of T)
                If m_Objects.Count > 0 Then
                    m_Object = Me.GetObjects(Of T)(0)
                Else
                    m_Object = Nothing
                End If
            End If

            Return m_Object
        End Function

        Private Function GetObjectsFromMemory(Of T As Rx3Object)(ByRef m_Objects As List(Of T)) As List(Of T)
            If m_Objects Is Nothing Then
                m_Objects = Me.GetObjects(Of T)()
            End If

            Return m_Objects
        End Function

        Friend Sub SetObjectsFromMemory()  'do this at saving file !
            If _HotSpot IsNot Nothing Then
                Me.SetObject(_HotSpot, 0)
            End If
            If _NameTable IsNot Nothing Then
                Me.SetObject(_NameTable, 0)
            End If

            If _Adjacencies IsNot Nothing Then
                Me.SetObjects(_Adjacencies)
            End If
            If _AnimationSkins IsNot Nothing Then
                Me.SetObjects(_AnimationSkins)
            End If
            'If _BoneName IsNot Nothing Then
            'Me.SetObjects(_BoneName)
            'End If   'No Clear Section-layout info
            If _BoneRemaps IsNot Nothing Then
                Me.SetObjects(_BoneRemaps)
            End If
            If _ClothDefs IsNot Nothing Then
                Me.SetObjects(_ClothDefs)
            End If
            If _Collisions IsNot Nothing Then
                Me.SetObjects(_Collisions)
            End If
            If _CollisionTriMesh IsNot Nothing Then
                Me.SetObjects(_CollisionTriMesh)
            End If
            If _EdgeMeshes IsNot Nothing Then
                Me.SetObjects(_EdgeMeshes)
            End If   'No Clear Section-layout info
            If _IndexBuffers IsNot Nothing Then
                Me.SetObjects(_IndexBuffers)
            End If
            If _Locations IsNot Nothing Then
                Me.SetObjects(_Locations)
            End If
            If _Materials IsNot Nothing Then
                Me.SetObjects(_Materials)
            End If
            If _MorphIndexed IsNot Nothing Then
                Me.SetObjects(_MorphIndexed)
            End If
            If _QuadIndexBuffers IsNot Nothing Then
                Me.SetObjects(_QuadIndexBuffers)
            End If
            If _SceneAnimation IsNot Nothing Then
                Me.SetObjects(_SceneAnimation)
            End If   'No Clear Section-layout info
            If _SceneInstance IsNot Nothing Then
                Me.SetObjects(_SceneInstance)
            End If
            If _SceneLayers IsNot Nothing Then
                Me.SetObjects(_SceneLayers)
            End If
            If _SimpleMeshes IsNot Nothing Then
                Me.SetObjects(_SimpleMeshes)
            End If
            If _Skeleton IsNot Nothing Then
                Me.SetObjects(_Skeleton)
            End If
            If _VertexBuffers IsNot Nothing Then
                Me.SetObjects(_VertexBuffers)
            End If
            If _VertexFormats IsNot Nothing Then
                Me.SetObjects(_VertexFormats)
            End If
            If _Textures IsNot Nothing Then
                Me.SetObjects(_Textures)
            End If
        End Sub

        'Public Property BoneName As List(Of  BoneName)   'No Clear Section-layout info

        Public Property HotSpot As HotSpot
            Get
                Return If(_HotSpot, Me.GetObjectFromMemory(_HotSpot))
            End Get
            Set
                _HotSpot = Value
            End Set
        End Property

        Public Property NameTable As NameTable
            Get
                Return If(_NameTable, Me.GetObjectFromMemory(_NameTable))
            End Get
            Set
                _NameTable = Value
            End Set
        End Property

        Public Property Textures As List(Of Texture)
            Get
                Return If(_Textures, Me.GetObjectsFromMemory(_Textures))
            End Get
            Set
                _Textures = Value
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

        Public Property VertexBuffers As List(Of VertexBuffer)
            Get
                Return If(_VertexBuffers, Me.GetObjectsFromMemory(_VertexBuffers))
            End Get
            Set
                _VertexBuffers = Value
            End Set
        End Property

        Public Property BoneRemaps As List(Of BoneRemap)
            Get
                Return If(_BoneRemaps, Me.GetObjectsFromMemory(_BoneRemaps))
            End Get
            Set
                _BoneRemaps = Value
            End Set
        End Property

        Public Property AnimationSkins As List(Of AnimationSkin)
            Get
                Return If(_AnimationSkins, Me.GetObjectsFromMemory(_AnimationSkins))
            End Get
            Set
                _AnimationSkins = Value
            End Set
        End Property

        Public Property VertexFormats As List(Of VertexFormat)
            Get
                Return If(_VertexFormats, Me.GetObjectsFromMemory(_VertexFormats))
            End Get
            Set
                _VertexFormats = Value
            End Set
        End Property

        Public Property Skeleton As List(Of Skeleton)
            Get
                Return If(_Skeleton, Me.GetObjectsFromMemory(_Skeleton))
            End Get
            Set
                _Skeleton = Value
            End Set
        End Property

        Public Property SimpleMeshes As List(Of SimpleMesh)
            Get
                Return If(_SimpleMeshes, Me.GetObjectsFromMemory(_SimpleMeshes))
            End Get
            Set
                _SimpleMeshes = Value
            End Set
        End Property

        Public Property QuadIndexBuffers As List(Of QuadIndexBuffer)
            Get
                Return If(_QuadIndexBuffers, Me.GetObjectsFromMemory(_QuadIndexBuffers))
            End Get
            Set
                _QuadIndexBuffers = Value
            End Set
        End Property

        Public Property Adjacencies As List(Of Adjacency)
            Get
                Return If(_Adjacencies, Me.GetObjectsFromMemory(_Adjacencies))
            End Get
            Set
                _Adjacencies = Value
            End Set
        End Property

        Public Property Materials As List(Of Material)
            Get
                Return If(_Materials, Me.GetObjectsFromMemory(_Materials))
            End Get
            Set
                _Materials = Value
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

        Public Property CollisionTriMesh As List(Of CollisionTriMesh)
            Get
                Return If(_CollisionTriMesh, Me.GetObjectsFromMemory(_CollisionTriMesh))
            End Get
            Set
                _CollisionTriMesh = Value
            End Set
        End Property

        Public Property SceneInstance As List(Of SceneInstance)
            Get
                Return If(_SceneInstance, Me.GetObjectsFromMemory(_SceneInstance))
            End Get
            Set
                _SceneInstance = Value
            End Set
        End Property

        Public Property ClothDefs As List(Of ClothDef)
            Get
                Return If(_ClothDefs, Me.GetObjectsFromMemory(_ClothDefs))
            End Get
            Set
                _ClothDefs = Value
            End Set
        End Property

        Public Property Collisions As List(Of Collision)
            Get
                Return If(_Collisions, Me.GetObjectsFromMemory(_Collisions))
            End Get
            Set
                _Collisions = Value
            End Set
        End Property

        Public Property EdgeMeshes As List(Of EdgeMesh)   'No Clear Section-layout info
            Get
                Return If(_EdgeMeshes, Me.GetObjectsFromMemory(_EdgeMeshes))
            End Get
            Set
                _EdgeMeshes = Value
            End Set
        End Property

        Public Property SceneAnimation As List(Of SceneAnimation)   'No Clear Section-layout info
            Get
                Return If(_SceneAnimation, Me.GetObjectsFromMemory(_SceneAnimation))
            End Get
            Set
                _SceneAnimation = Value
            End Set
        End Property

        Public Property MorphIndexed As List(Of MorphIndexed)
            Get
                Return If(_MorphIndexed, Me.GetObjectsFromMemory(_MorphIndexed))
            End Get
            Set
                _MorphIndexed = Value
            End Set
        End Property

        'Public Property Textures(ByVal Index As UInteger) As Texture
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.TEXTURE, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property IndexBuffers(ByVal Index As UInteger) As IndexBuffer
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.INDEX_BUFFER, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property VertexBuffers(ByVal Index As UInteger) As VertexBuffer
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.VERTEX_BUFFER, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property BoneRemaps(ByVal Index As UInteger) As BoneRemap
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.BONE_REMAP, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property AnimationSkins(ByVal Index As UInteger) As AnimationSkin
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.ANIMATION_SKIN, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property VertexFormats(ByVal Index As UInteger) As VertexFormat
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.VERTEX_FORMAT, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Skeleton(ByVal Index As UInteger) As Skeleton
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.SKELETON, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property SimpleMeshes(ByVal Index As UInteger) As SimpleMesh
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.SIMPLE_MESH, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property QuadIndexBuffers(ByVal Index As UInteger) As QuadIndexBuffer
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.QUAD_INDEX_BUFFER, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Adjacencies(ByVal Index As UInteger) As Adjacency
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.ADJACENCY, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Materials(ByVal Index As UInteger) As Material
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.MATERIAL, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property SceneLayers(ByVal Index As UInteger) As SceneLayer
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.SCENE_LAYER, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Locations(ByVal Index As UInteger) As Location
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.LOCATION, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property CollisionTriMesh(ByVal Index As UInteger) As CollisionTriMesh
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.COLLISION_TRI_MESH, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property SceneInstance(ByVal Index As UInteger) As SceneInstance
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.SCENE_INSTANCE, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property ClothDefs(ByVal Index As UInteger) As ClothDef
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.CLOTH_DEF, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property Collisions(ByVal Index As UInteger) As Collision
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.COLLISION, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property EdgeMeshes(ByVal Index As UInteger) As EdgeMesh   'No Clear Section-layout info
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.EDGE_MESH, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property SceneAnimation(ByVal Index As UInteger) As SceneAnimation   'No Clear Section-layout info
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.SCENE_ANIMATION, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Public Property MorphIndexed(ByVal Index As UInteger) As MorphIndexed
        '    Get
        '        Return Me.GetObject(Rx3.SectionHash.MORPH_INDEXED, Index)
        '    End Get
        '    Set
        '        Me.SetObject(Value, Index)
        '    End Set
        'End Property

        'Private properties (batch areas, created by code at saving)
        'Private Property TextureBatch As TextureBatch
        'Private Property IndexBufferBatch As IndexBufferBatch
        'Private Property QuadIndexBufferBatch As QuadIndexBufferBatch
    End Class
End Namespace