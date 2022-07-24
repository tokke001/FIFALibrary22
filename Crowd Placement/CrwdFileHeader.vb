
Namespace CrowdDat
    Public Class CrwdFileHeader

        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Overridable Function Load(ByVal r As FileReader) As Boolean
            Me.Magic = New String(r.ReadChars(4))
            If Me.Magic <> "CRWD" Then
                Return False
            End If

            Me.Version = r.ReadUInt16
            Me.NumSeats = r.ReadUInt32

            Return True
        End Function

        Public Overridable Function Save(ByVal w As FileWriter) As Boolean
            w.Write(Me.Magic.ToCharArray)
            w.Write(Me.Version)
            w.Write(Me.NumSeats)

            Return True
        End Function

        Public Property Magic As String = "CRWD"    'magic
        Public Property Version As EVersion = 0
        Public Property NumSeats As UInteger = 0
        Public ReadOnly Property Size As Integer = 10

        Public Enum EVersion As UShort
            TYPE_0103 = &H103     'FIFA07 - 14
            TYPE_0104 = &H104     'WC'14
            TYPE_0105 = &H105     'FIFA 15 (or newer)
        End Enum

    End Class

End Namespace