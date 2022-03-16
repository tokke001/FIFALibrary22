Namespace Rx3
    Public Class Rx3Sections
        Public Sub New(ByVal Rx3File As Rx3.Rx3FileRx3Section, ByVal RW4Section As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.Rx3File = Rx3File
            Me.RW4Section = RW4Section
            Me.Load(r)
        End Sub

        Private Rx3File As Rx3.Rx3FileRx3Section
        Private RW4Section As Rw.Core.Arena.Arena
        Private ReadOnly Objects As List(Of Rx3Object) = New List(Of Rx3Object)

        Public Sub Load(ByVal r As FileReader)
            IndexVBuffer = 0

            For Each Info As SectionHeader In Me.Rx3File.Rx3SectionHeaders
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
        Private _Textures As List(Of Texture)

        Public Function CreateObject(ByVal Info As SectionHeader, ByVal r As FileReader) As Rx3Object
            Select Case Info.Signature
                 '----- SECTIONS since FIFA 11 -----
                Case SectionHash.INDEX_BUFFER_BATCH
                    Return New IndexBufferBatch(Me.Rx3File, r)

                Case SectionHash.INDEX_BUFFER
                    Return New IndexBuffer(Me.Rx3File, r)

                Case SectionHash.VERTEX_BUFFER
                    IndexVBuffer += 1
                    Return New VertexBuffer(GetVertexElements(IndexVBuffer - 1), Me.Rx3File, r)

                Case SectionHash.BONE_REMAP  'Partial Skeleton   (present at all fifas)
                    Return New BoneRemap(Me.Rx3File, r)

                Case SectionHash.TEXTURE_BATCH
                    Return New TextureBatch(Me.Rx3File, r)

                Case SectionHash.TEXTURE
                    Return New Texture(Me.Rx3File, r)

                    '----- SECTIONS since FIFA 12 -----
                Case SectionHash.MATERIAL
                    Return New Material(Me.Rx3File, r)

                Case SectionHash.SCENE_LAYER
                    Return New SceneLayer(Me.Rx3File, r)

                Case SectionHash.LOCATION
                    Return New Location(Me.Rx3File, r)

                Case SectionHash.NAME_TABLE
                    Return New NameTable(Me.Rx3File, r)

                Case SectionHash.EDGE_MESH  'found at FO3 : body\arms.rx3
                    Return New EdgeMesh(Me.Rx3File, r)

                Case SectionHash.SCENE_INSTANCE
                    Return New SceneInstance(Me.Rx3File, r)

                Case SectionHash.SCENE_ANIMATION    'found at wipe3d / found at stadium_250, index 675, offset 2250768 
                    Return New SceneAnimation(Me.Rx3File, r)

                    'Case SectionHash.MORPH_INDEXED      'probably RX3_SECTION_MORPH - used in *_morphtargets files. 

                Case SectionHash.VERTEX_FORMAT
                    Return New VertexFormat(Me.Rx3File, r)

                Case SectionHash.SIMPLE_MESH 'PRIMITIVE_TYPE 'loaded beforehand 
                    Return New SimpleMesh(Me.Rx3File, r)

                Case SectionHash.ANIMATION_SKIN 'Bone Matrices   
                    Return New AnimationSkin(Me.Rx3File, r)

                Case SectionHash.COLLISION_TRI_MESH
                    Return New CollisionTriMesh(Me.Rx3File, r)

                Case SectionHash.HOTSPOT '(ex. kit positions) 
                    Return New HotSpot(Me.Rx3File, r)

                Case SectionHash.COLLISION
                    Return New Collision(Me.Rx3File, r)

                    '----- SECTIONS since FIFA 15 -----
                Case SectionHash.CLOTH_DEF
                    Return New ClothDef(Info.Size, Me.Rx3File, r)

                Case SectionHash.SKELETON
                    Return New Skeleton(Me.Rx3File, r)


                    '----- SECTIONS since FIFA 16 -----
                    'Case SectionHash.BONE_NAME   'may not exist: is maybe not a seperate section

                Case SectionHash.QUAD_INDEX_BUFFER_BATCH
                    Return New QuadIndexBufferBatch(Me.Rx3File, r)

                Case SectionHash.QUAD_INDEX_BUFFER
                    Return New QuadIndexBuffer(Me.Rx3File, r)

                Case SectionHash.ADJACENCY
                    Return New Adjacency(Me.Rx3File, r)

                Case Else
                    'MsgBox("Error at loading Rx3File: Unknown rx3 section found - " & typeCode.ToString)
                    Return Nothing
            End Select

        End Function

        Private Function GetVertexElements(ByVal MeshIndex As UInteger) As VertexElement()
            If Me.RW4Section IsNot Nothing AndAlso Me.RW4Section.Sections.EmbeddedMeshes IsNot Nothing AndAlso Me.RW4Section.Sections.EmbeddedMeshes(MeshIndex) IsNot Nothing Then
                Return Me.RW4Section.Sections.FxRenderableSimples(MeshIndex).PVertexDescriptor.Elements 'Me.RW4Section.Sections.EmbeddedMeshes(MeshIndex).PVertexBuffers(0).PVertexDescriptor.Elements
            ElseIf Me.VertexFormats IsNot Nothing AndAlso Me.VertexFormats(MeshIndex) IsNot Nothing Then
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

                    'Case SectionHash.MORPH_INDEXED

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
        Public Function GetObjects(ByVal m_Type As Rx3.SectionHash) As IList(Of Rx3Object) 'Of T As RWObject)(ByVal m_Type As Type(Of T)) As List(Of T)
            Return Me.Objects.FindAll(Function(x) x.GetTypeCode = m_Type)

            'Dim result As List(Of RWObject) = New List(Of RWObject)

            'For Each m_object As RWObject In Objects
            '    If m_object.GetTypeCode = m_Type Then 'm_Type.IsInstanceOfType(m_object) Then
            '        result.Add(m_object)
            '    End If
            'Next m_object

            'Return result
        End Function

        ''' Returns the object at the specified index. 
        Public Overridable Function GetObject(ByVal Index As Integer) As Rx3Object
            Return Me.Objects(Index)
        End Function

        Public Overridable Function GetObject(ByVal m_Type As Rx3.SectionHash, ByVal Index As Integer) As Rx3Object
            Dim m_Count As UInteger = 0

            For Each m_object As Rx3Object In Me.Objects
                If m_object.GetTypeCode = m_Type Then
                    If m_Count = Index Then
                        Return Objects(Index)
                    End If
                    m_Count += 1
                End If
            Next m_object

            Return Nothing
        End Function

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

        Public Sub SetObjects(ByVal m_Objects As IList(Of Rx3Object))
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

        Public Property Textures As List(Of Texture)
            Get
                Return Me.GetObjects(Rx3.SectionHash.TEXTURE)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property IndexBuffers As List(Of IndexBuffer)
            Get
                Return Me.GetObjects(Rx3.SectionHash.INDEX_BUFFER)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property VertexBuffers As List(Of VertexBuffer)
            Get
                Return Me.GetObjects(Rx3.SectionHash.VERTEX_BUFFER)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property HotSpot As HotSpot
            Get
                Return Me.GetObject(Rx3.SectionHash.HOTSPOT, 0)
            End Get
            Set
                Me.SetObject(Value, 0)
            End Set
        End Property

        Public Property BoneRemaps As List(Of BoneRemap)
            Get
                Return Me.GetObjects(Rx3.SectionHash.BONE_REMAP)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property AnimationSkins As List(Of AnimationSkin)
            Get
                Return Me.GetObjects(Rx3.SectionHash.ANIMATION_SKIN)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property VertexFormats As List(Of VertexFormat)
            Get
                Return Me.GetObjects(Rx3.SectionHash.VERTEX_FORMAT)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Skeleton As List(Of Skeleton)
            Get
                Return Me.GetObjects(Rx3.SectionHash.SKELETON)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property NameTable As NameTable
            Get
                Return Me.GetObject(Rx3.SectionHash.NAME_TABLE, 0)
            End Get
            Set
                Me.SetObject(Value, 0)
            End Set
        End Property

        Public Property SimpleMeshes As List(Of SimpleMesh)
            Get
                Return Me.GetObjects(Rx3.SectionHash.SIMPLE_MESH)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property QuadIndexBuffers As List(Of QuadIndexBuffer)
            Get
                Return Me.GetObjects(Rx3.SectionHash.QUAD_INDEX_BUFFER)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Adjacencies As List(Of Adjacency)
            Get
                Return Me.GetObjects(Rx3.SectionHash.ADJACENCY)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Materials As List(Of Material)
            Get
                Return Me.GetObjects(Rx3.SectionHash.MATERIAL)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property SceneLayers As List(Of SceneLayer)
            Get
                Return Me.GetObjects(Rx3.SectionHash.SCENE_LAYER)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Locations As List(Of Location)
            Get
                Return Me.GetObjects(Rx3.SectionHash.LOCATION)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property CollisionTriMesh As List(Of CollisionTriMesh)
            Get
                Return Me.GetObjects(Rx3.SectionHash.COLLISION_TRI_MESH)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property SceneInstance As List(Of SceneInstance)
            Get
                Return Me.GetObjects(Rx3.SectionHash.SCENE_INSTANCE)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property ClothDefs As List(Of ClothDef)
            Get
                Return Me.GetObjects(Rx3.SectionHash.CLOTH_DEF)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property Collisions As List(Of Collision)
            Get
                Return Me.GetObjects(Rx3.SectionHash.COLLISION)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property EdgeMeshes As List(Of EdgeMesh)   'No Clear Section-layout info
            Get
                Return Me.GetObjects(Rx3.SectionHash.EDGE_MESH)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        Public Property SceneAnimation As List(Of SceneAnimation)   'No Clear Section-layout info
            Get
                Return Me.GetObjects(Rx3.SectionHash.SCENE_ANIMATION)
            End Get
            Set
                Me.SetObjects(Value)
            End Set
        End Property

        'Public Property BoneName As List(Of  BoneName)   'No Clear Section-layout info
        'Public Property MorphIndexed As List(Of  MorphIndexed)   'No Clear Section-layout info

        Public Property Textures(ByVal Index As UInteger) As Texture
            Get
                Return Me.GetObject(Rx3.SectionHash.TEXTURE, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property IndexBuffers(ByVal Index As UInteger) As IndexBuffer
            Get
                Return Me.GetObject(Rx3.SectionHash.INDEX_BUFFER, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property VertexBuffers(ByVal Index As UInteger) As VertexBuffer
            Get
                Return Me.GetObject(Rx3.SectionHash.VERTEX_BUFFER, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property BoneRemaps(ByVal Index As UInteger) As BoneRemap
            Get
                Return Me.GetObject(Rx3.SectionHash.BONE_REMAP, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property AnimationSkins(ByVal Index As UInteger) As AnimationSkin
            Get
                Return Me.GetObject(Rx3.SectionHash.ANIMATION_SKIN, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property VertexFormats(ByVal Index As UInteger) As VertexFormat
            Get
                Return Me.GetObject(Rx3.SectionHash.VERTEX_FORMAT, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Skeleton(ByVal Index As UInteger) As Skeleton
            Get
                Return Me.GetObject(Rx3.SectionHash.SKELETON, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property SimpleMeshes(ByVal Index As UInteger) As SimpleMesh
            Get
                Return Me.GetObject(Rx3.SectionHash.SIMPLE_MESH, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property QuadIndexBuffers(ByVal Index As UInteger) As QuadIndexBuffer
            Get
                Return Me.GetObject(Rx3.SectionHash.QUAD_INDEX_BUFFER, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Adjacencies(ByVal Index As UInteger) As Adjacency
            Get
                Return Me.GetObject(Rx3.SectionHash.ADJACENCY, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Materials(ByVal Index As UInteger) As Material
            Get
                Return Me.GetObject(Rx3.SectionHash.MATERIAL, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property SceneLayers(ByVal Index As UInteger) As SceneLayer
            Get
                Return Me.GetObject(Rx3.SectionHash.SCENE_LAYER, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Locations(ByVal Index As UInteger) As Location
            Get
                Return Me.GetObject(Rx3.SectionHash.LOCATION, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property CollisionTriMesh(ByVal Index As UInteger) As CollisionTriMesh
            Get
                Return Me.GetObject(Rx3.SectionHash.COLLISION_TRI_MESH, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property SceneInstance(ByVal Index As UInteger) As SceneInstance
            Get
                Return Me.GetObject(Rx3.SectionHash.SCENE_INSTANCE, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property ClothDefs(ByVal Index As UInteger) As ClothDef
            Get
                Return Me.GetObject(Rx3.SectionHash.CLOTH_DEF, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property Collisions(ByVal Index As UInteger) As Collision
            Get
                Return Me.GetObject(Rx3.SectionHash.COLLISION, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property EdgeMeshes(ByVal Index As UInteger) As EdgeMesh   'No Clear Section-layout info
            Get
                Return Me.GetObject(Rx3.SectionHash.EDGE_MESH, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property

        Public Property SceneAnimation(ByVal Index As UInteger) As SceneAnimation   'No Clear Section-layout info
            Get
                Return Me.GetObject(Rx3.SectionHash.SCENE_ANIMATION, Index)
            End Get
            Set
                Me.SetObject(Value, Index)
            End Set
        End Property


        'Private properties (batch areas, created by code at saving)
        'Private Property TextureBatch As TextureBatch
        'Private Property IndexBufferBatch As IndexBufferBatch
        'Private Property QuadIndexBufferBatch As QuadIndexBufferBatch
    End Class
End Namespace