Namespace Rx3
    Public Class Collision
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.COLLISION
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.TotalSize = r.ReadUInt32
            Me.Unknown_1 = r.ReadUInt32
            Me.Unknown_2(0) = r.ReadUInt32
            Me.Unknown_2(1) = r.ReadUInt32

            Me.CollisionName = FifaUtil.ReadNullTerminatedString(r)
            Me.Unknown_3 = r.ReadUInt32
            Me.Unknown_4 = r.ReadUInt32
            Me.Unknown_5 = r.ReadUInt32
            Me.NumCollisions = r.ReadUInt32
            Me.Unknown_6 = r.ReadBytes(4)

            Me.Collisions = New Matrix4x4(Me.NumCollisions - 1) {}
            For i = 0 To Me.NumCollisions - 1
                Me.Collisions(i) = New Matrix4x4(r)
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumCollisions = Me.Collisions.Length

            w.Write(Me.TotalSize)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2(0))
            w.Write(Me.Unknown_2(1))

            FifaUtil.WriteNullTerminatedString(w, Me.CollisionName)
            w.Write(Me.Unknown_3)
            w.Write(Me.Unknown_4)
            w.Write(Me.Unknown_5)
            w.Write(Me.NumCollisions)
            w.Write(Me.Unknown_6)

            For i = 0 To Me.NumCollisions - 1
                Me.Collisions(i).Save(w)
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
        Public Property Unknown_4 As UInteger   'always 0?
        Public Property Unknown_5 As UInteger   'always 3?  'number of arrays in an array?
        Public Property NumCollisions As UInteger
        Public Property Unknown_6 As Byte() = New Byte(4 - 1) {}    ' 6 0 0 0 or 10 0 0 0 -> some index ?

        'Collisions: an array of 4 (each is represented with Vector4 struct ).
        Public Property Collisions As Matrix4x4() 'New List(Of List(Of Vector4))

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace