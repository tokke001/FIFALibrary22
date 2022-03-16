Namespace AudioBin.Sentences
    Public Class Section
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader)
            Me.NumSentences = r.ReadInt32
            Me.Unknown_1 = r.ReadInt32     'always "0" ?

            Me.Sentences = New Sentence(Me.NumSentences - 1) {}
            For i = 0 To Me.Sentences.Length - 1
                Dim Offset As UInteger = r.ReadInt32
                Dim m_basestream As UInteger = r.BaseStream.Position

                r.BaseStream.Position = Offset
                Me.Sentences(i) = New Sentence(r)

                r.BaseStream.Position = m_basestream
            Next

            Me.TriggerName = FifaUtil.ReadNullTerminatedString(r)

        End Sub

        'Public Sub Save(ByVal w As BinaryWriter)

        'End Sub

        Public Property NumSentences As Integer
        Public Property Unknown_1 As Integer

        Public Property Sentences As Sentence()
        Public Property TriggerName As String = ""


    End Class
End Namespace