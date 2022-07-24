Imports Microsoft.DirectX

Namespace CrowdDat

    Public Class Seat0105
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
            Me.Section1 = r.ReadByte
            Me.Tier = r.ReadByte
            Me.Attendance = r.ReadByte
            Me.NoChair = r.ReadByte
            Me.CardColors = r.ReadBytes(3)
            Me.CrowdPattern = r.ReadByte
            Me.Pad = r.ReadBytes(4)

            Return True
        End Function

        Public Function Save(ByVal w As FileWriter) As Boolean

            w.Write(Me.Position)
            w.Write(Me.Rotation)
            w.Write(Me.SeatColor)

            w.Write(Me.Section0)
            w.Write(Me.Section1)
            w.Write(Me.Tier)
            w.Write(Me.Attendance)
            w.Write(Me.NoChair)
            w.Write(Me.CardColors)
            w.Write(Me.CrowdPattern)
            w.Write(Me.Pad)

            Return True
        End Function

        Public Property Position As New Vector3
        Public Property Rotation As Single
        Public Property SeatColor As Byte() = New Byte(3 - 1) {}

        Public Property Section0 As Byte             'home/away ultra : 0 = all home, 255 = all away
        Public Property Section1 As Byte             'home/away neutral : 0 = all home, 255 = all away
        Public Property Tier As Byte                 'F14: 0, 1, 2, 3, 4, 5 | F11: 0, 2, 255
        Public Property Attendance As Byte          'Size of crowd: 0 = near no crowd, 255 = full packed

        Public Property NoChair As Byte                             '(ea py script: unused! )
        Public Property CardColors As Byte() = New Byte(3 - 1) {}    '3   (ea py script: WC )
        Public Property CrowdPattern As Byte                          '
        Public Property Pad As Byte() = New Byte(4 - 1) {}            'padding

        Public ReadOnly Property Size As Integer = 32

    End Class

End Namespace