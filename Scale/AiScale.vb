Imports Microsoft.DirectX

Namespace Scale

<Serializable> Public Class AiScale
        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Function Load(ByVal r As FileReader) As Boolean
            Me.BoneName = FifaUtil.ReadNullTerminatedString(r)
            Me.Scale = r.ReadVector3
            Me.Trans = r.ReadVector3
            Me.Unknown = r.ReadUInt32

            'Select Case Me.Unknown
            '    Case 1
            '    Case Else
            'End Select

            Return True
        End Function

        Public Function Save(ByVal w As FileWriter) As Boolean
            FifaUtil.WriteNullTerminatedString(w, Me.BoneName)
            w.Write(Me.Scale)
            w.Write(Me.Trans)
            w.Write(Me.Unknown)

            Return True
        End Function

        Public Property BoneName As String
        Public Property Scale As New Vector3
        Public Property Trans As New Vector3
        Public Property Unknown As UInteger = 1     'Always 1

    End Class

End Namespace