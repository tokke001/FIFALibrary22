Namespace Rx3
    Public Class SceneInstance
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.SCENE_INSTANCE
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.TotalSize = r.ReadUInt32
            Me.Status = r.ReadUInt32
            Me.Unknown_1 = r.ReadInt32
            Me.Unknown_2 = r.ReadInt32

            Me.TransformMatrix = New Matrix4x4(r)

            Me.BBox = New BBox(r)   '2x Vector4 (min, max)

            If Me.Status = EStatus.Used Then
                Dim m_NumMeshes As UInteger = r.ReadUInt32
                Me.Unknown_3 = r.ReadInt32

                Me.MeshDescriptor = New SceneInstanceMeshDescriptor(m_NumMeshes - 1) {}
                For i = 0 To m_NumMeshes - 1
                    Me.MeshDescriptor(i) = New SceneInstanceMeshDescriptor(r)
                Next i
            End If
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.TotalSize)
            w.Write(CUInt(Me.Status))
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)

            Me.TransformMatrix.Save(w)

            Me.BBox.Save(w)

            If Me.Status = EStatus.Used Then
                w.Write(Me.NumMeshes)
                w.Write(Me.Unknown_3)

                For i = 0 To Me.NumMeshes - 1
                    Me.MeshDescriptor(i).Save(w)
                Next i
            End If
            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub


        Private m_TotalSize As UInteger

        ''' <summary>
        ''' Total section size (ReadOnly). </summary>
        Public Property TotalSize As UInteger
            Get
                Return m_TotalSize
            End Get
            Private Set
                m_TotalSize = Value
            End Set
        End Property
        ''' <summary>
        ''' Status of the Scene instance. </summary>
        Public Property Status As EStatus   '1 if used, 0 if unused (according to FIFA 15 3D importer/exporter; I didn't see files with '0' yet)
        Public Property Unknown_1 As Integer    'always -1
        Public Property Unknown_2 As Integer    'always 0
        ''' <summary>
        ''' Identity matrix. </summary>
        Public Property TransformMatrix As Matrix4x4 'usually an identity matrix
        ''' <summary>
        ''' Bounding Box with 2 Vector4 (min, max). </summary>
        Public Property BBox As New BBox   'Vector4 (min, max)
        ''' <summary>
        ''' Returns the number of Meshes (ReadOnly). </summary>
        Public ReadOnly Property NumMeshes As UInteger
            Get
                Return If(MeshDescriptor?.Count, 0)
            End Get
        End Property
        Public Property Unknown_3 As Integer    'always -1
        ''' <summary>
        ''' Gets/Sets the Mesh Descriptors. </summary>
        Public Property MeshDescriptor As SceneInstanceMeshDescriptor()

        Public Enum EStatus As UInteger
            Used = 1
            Unused = 0
        End Enum

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class SceneInstanceMeshDescriptor
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.BBox = New BBox(r)
            Me.MeshIndex = r.ReadUInt32
            Me.MaterialIndex = r.ReadUInt32
        End Sub
        Public Sub Save(ByVal w As FileWriter)
            Me.BBox.Save(w)
            w.Write(Me.MeshIndex)
            w.Write(Me.MaterialIndex)
        End Sub
        ''' <summary>
        ''' Bounding Box of the mesh, 2 Vector4 (min, max). </summary>
        Public Property BBox As BBox   'bounding box of the mesh
        ''' <summary>
        ''' Index for the vertex buffer and index buffer (starts with 0). </summary>
        Public Property MeshIndex As UInteger   'starts with 0, index for vertex buffer and index buffer
        ''' <summary>
        ''' Index for material (starts with 0). </summary>
        Public Property MaterialIndex As UInteger   'starts with 0, index for material
    End Class

End Namespace