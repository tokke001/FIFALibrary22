Public Class Bin_0XTC00CP_Id
    Public Sub New()
    End Sub
    Public Sub New(ByVal r As BinaryReader)
        Me.Id = r.ReadInt32         'ussually counting from 0 + 1
        Me.Unknown_1 = r.ReadInt32  'Usually "0" or Id might be a long value !

    End Sub

    Public Property Id As Integer
    Public Property Unknown_1 As Integer
End Class
