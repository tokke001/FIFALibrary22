Imports Microsoft.DirectX

Namespace Rx3
<Serializable> Public Class CollisionTriMesh
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.COLLISION_TRI_MESH
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
            Dim m_NumSpaces As UInteger = r.ReadUInt32
            Me.Pad = r.ReadBytes(8)

            Me.CollisionName = FifaUtil.ReadNullTerminatedString(r)     'might be part of Spaces, a CollisionName for each Space ?? : no idea because NumSpaces is always 1!! 
            Me.Spaces = New CollisionTriMeshSpace(m_NumSpaces - 1) {}
            For i = 0 To Me.Spaces.Length - 1
                Me.Spaces(i) = New CollisionTriMeshSpace(r)
            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.TotalSize)
            w.Write(Me.NumSpaces)
            w.Write(Me.Pad)

            FifaUtil.WriteNullTerminatedString(w, Me.CollisionName)
            For i = 0 To Me.NumSpaces - 1
                Me.Spaces(i).Save(w)
            Next

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
        ''' Returns the number of Spaces (ReadOnly). </summary>
        Public ReadOnly Property NumSpaces As UInteger   'always 1?
            Get
                Return If(Spaces?.Count, 0)
            End Get
        End Property
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Pad As Byte() = New Byte(8 - 1) {}    'always 0, padding

        ''' <summary>
        ''' The Collision name. </summary>
        Public Property CollisionName As String

        Public Property Spaces As CollisionTriMeshSpace()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

<Serializable> Public Class CollisionTriMeshSpace

        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Dim m_NumShapes = r.ReadUInt32

            Me.Shapes = New CollisionTriMeshShape(m_NumShapes - 1) {}
            For i = 0 To Me.Shapes.Length - 1
                Me.Shapes(i) = New CollisionTriMeshShape(r)
            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.NumShapes)

            For i = 0 To Me.NumShapes - 1
                Me.Shapes(i).Save(w)
            Next

        End Sub

        ''' <summary>
        ''' Returns the number of Shapes (ReadOnly). </summary>
        Public ReadOnly Property NumShapes As UInteger   'always 1? , NumShapes maybe (similar to rw modelCollision)?? 
            Get
                Return If(Shapes?.Count, 0)
            End Get
        End Property

        Public Property Shapes As CollisionTriMeshShape()

    End Class

<Serializable> Public Class CollisionTriMeshShape

        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Dim m_NumCollTriangles = r.ReadUInt32

            For i = 0 To m_NumCollTriangles - 1
                Dim Me_CollTrangles As New List(Of Vector3)
                For j = 0 To 3 - 1
                    Me_CollTrangles.Add(r.ReadVector3)
                Next j

                Me.CollisionTriangles.Add(Me_CollTrangles)

            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.NumCollTriangles)

            For i = 0 To Me.NumCollTriangles - 1
                For j = 0 To 3 - 1
                    w.Write(Me.CollisionTriangles(i)(j))
                Next j
            Next i

        End Sub

        ''' <summary>
        ''' Returns the number of Collision-Triangles (ReadOnly). </summary>
        Public ReadOnly Property NumCollTriangles As UInteger
            Get
                Return If(CollisionTriangles?.Count, 0)
            End Get
        End Property

        ''' <summary>
        ''' Collision Triangles: an array of triangles (each triangle is represented with Vector3 struct for 3 vertices). </summary>
        Public Property CollisionTriangles As New List(Of List(Of Vector3))

    End Class
End Namespace