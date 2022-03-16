Imports Microsoft.DirectX

Namespace Rx3
    Public Class CollisionTriMesh
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.COLLISION_TRI_MESH
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Dim Me_Vector3 As Vector3
            Dim Me_CollTrangles As List(Of Vector3)


            Me.TotalSize = r.ReadUInt32
            Me.Unknown_1 = r.ReadUInt32
            Me.Unknown_2(0) = r.ReadUInt32
            Me.Unknown_2(1) = r.ReadUInt32

            Me.CollisionName = FifaUtil.ReadNullTerminatedString(r)
            Me.Unknown_3 = r.ReadUInt32
            Me.NumCollTriangles = r.ReadUInt32

            For i = 0 To Me.NumCollTriangles - 1
                Me_CollTrangles = New List(Of Vector3)
                For j = 0 To 3 - 1
                    Me_Vector3 = New Vector3 With {
                            .X = r.ReadSingle,
                            .Y = r.ReadSingle,
                            .Z = r.ReadSingle
                        }

                    Me_CollTrangles.Add(Me_Vector3)
                Next j

                Me.CollisionTriangles.Add(Me_CollTrangles)

            Next i
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumCollTriangles = Me.CollisionTriangles.Count

            w.Write(Me.TotalSize)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2(0))
            w.Write(Me.Unknown_2(1))

            FifaUtil.WriteNullTerminatedString(w, Me.CollisionName)
            w.Write(Me.Unknown_3)
            w.Write(Me.NumCollTriangles)

            For i = 0 To Me.NumCollTriangles - 1
                For j = 0 To 3 - 1
                    w.Write(Me.CollisionTriangles(i)(j).X)
                    w.Write(Me.CollisionTriangles(i)(j).Y)
                    w.Write(Me.CollisionTriangles(i)(j).Z)
                Next j
            Next i

            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub


        Public Property TotalSize As UInteger
        Public Property Unknown_1 As UInteger   'always 1?
        Public Property Unknown_2 As UInteger() = New UInteger(2 - 1) {}    'always 0? maybe padding
        Public Property CollisionName As String
        Public Property Unknown_3 As UInteger   'always 1?
        Public Property NumCollTriangles As UInteger

        'CollisionTriangles: an array of triangles (each triangle is represented with Vector3 struct for 3 vertices).
        Public Property CollisionTriangles As New List(Of List(Of Vector3))

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace