Namespace AudioBin.EventSystem
    Public Class ParameterValue
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader)
            Me.Unknown_1 = r.ReadInt32  'Usually "0"
            Me.Value = r.ReadInt32
            Me.Name = FifaUtil.ReadNullTerminatedString(r)

        End Sub

        Public Property Unknown_1 As Integer
        Public Property Value As Integer
        Public Property Name As String
    End Class
End Namespace