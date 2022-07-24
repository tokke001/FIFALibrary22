Imports Microsoft.DirectX

Namespace CrowdDat

    Public Class Seat0104
        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Function Load(ByVal r As FileReader) As Boolean

            Me.Position = r.ReadVector3
            Me.Rotation = r.ReadSingle
            Me.SeatColor = r.ReadBytes(3)

            Me.Section0 = r.ReadByte
            Me.Unknown_1 = r.ReadByte
            Me.Unknown_2 = r.ReadByte
            Me.Unknown_3 = r.ReadByte
            Me.Unknown_4 = r.ReadByte
            Me.Attendance = r.ReadByte

            Me.Unknown_5 = New Single(4 - 1) {}
            For i = 0 To 4 - 1
                Me.Unknown_5(i) = r.ReadSingle
            Next

            Return True
        End Function

        Public Function Save(ByVal w As FileWriter) As Boolean

            w.Write(Me.Position)
            w.Write(Me.Rotation)
            w.Write(Me.SeatColor)

            w.Write(Me.Section0)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            w.Write(Me.Unknown_3)
            w.Write(Me.Unknown_4)
            w.Write(Me.Attendance)

            For i = 0 To Me.Unknown_5.Length - 1
                w.Write(Me.Unknown_5(i))
            Next i

            Return True
        End Function

        Public Property Position As New Vector3
        Public Property Rotation As Single
        Public Property SeatColor As Byte() = New Byte(3 - 1) {}

        Public Property Section0 As Byte             'home/away : 0 = all home, 255 = all away
        Public Property Unknown_1 As Byte             'Section1 maybe ?
        Public Property Unknown_2 As Byte             'Section2 maybe ?
        Public Property Unknown_3 As Byte             'Section4 maybe ?
        Public Property Unknown_4 As Byte             'Tier maybe ??
        Public Property Attendance As Byte          'Size of crowd: 0 = near no crowd, 255 = full packed

        Public Property Unknown_5 As Single() = New Single(4 - 1) {}    'maybe shading (lightning) ?? --> always 0,0,0,1

        Public ReadOnly Property Size As Integer = 41

    End Class

End Namespace