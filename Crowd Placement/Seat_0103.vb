Imports Microsoft.DirectX

Namespace CrowdDat

    Public Class Seat0103
        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Function Load(ByVal r As FileReader) As Boolean

            Me.Position = r.ReadVector3
            Me.Rotation = r.ReadSingle
            Me.SeatColor = r.ReadBytes(3)

            Me.Section = r.ReadByte
            Me.Tier = r.ReadByte
            Me.Attendance = r.ReadByte
            Me.InfluenceArea = r.ReadByte

            Me.Unused = r.ReadByte
            Me.Shade = New Single(4 - 1) {}
            For i = 0 To 4 - 1
                Me.Shade(i) = r.ReadSingle
            Next
            Me.Animgroups = r.ReadByte
            Me.NumAccs = r.ReadByte

            Return True
        End Function

        Public Function Save(ByVal w As FileWriter) As Boolean

            w.Write(Me.Position)
            w.Write(Me.Rotation)
            w.Write(Me.SeatColor)

            w.Write(Me.Section)
            w.Write(Me.Tier)
            w.Write(Me.Attendance)
            w.Write(Me.InfluenceArea)

            w.Write(Me.Unused)
            For i = 0 To Me.Shade.Length - 1
                w.Write(Me.Shade(i))
            Next i
            w.Write(Me.Animgroups)
            w.Write(Me.NumAccs)

            Return True
        End Function

        Public Property Position As New Vector3
        Public Property Rotation As Single
        Public Property SeatColor As Byte() = New Byte(3 - 1) {}

        Public Property Section As Byte             'home/away : 0 = all home, 255 = all away
        Public Property Tier As Byte                'F14: 0, 1, 2, 3, 4, 5 | F11: 0, 2, 255
        Public Property Attendance As Byte          'Size of crowd: 0 = near no crowd, 255 = full packed
        Public Property InfluenceArea As Byte       'F11/14: always 127 (ea py script: unused! )

        Public Property Unused As Byte                             'always 51 (ea py script: unused! )
        Public Property Shade As Single() = New Single(4 - 1) {}    'lightning, 4 float Values: ussually between 0 - 1 (ea py script: Can be bytes)
        Public Property Animgroups As Byte                          'always 0  (ea py script: unused! )
        Public Property NumAccs As Byte                             'always 0  (ea py script: unused! )

        Public ReadOnly Property Size As Integer = 42

    End Class

End Namespace