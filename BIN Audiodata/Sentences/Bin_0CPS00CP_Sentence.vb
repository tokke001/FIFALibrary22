Namespace AudioBin.Sentences
    Public Class Sentence
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader)
            Me.Id = r.ReadInt32    'id  ? (not sure)
            Me.Unknown_1 = r.ReadInt32 'always 100 ? 

            Me.Priority = r.ReadInt32
            Me.NumPhrases = r.ReadInt32    'numParameters ?

            Me.Phrases = New Phrase(Me.NumPhrases - 1) {}
            For i = 0 To Phrases.Length - 1
                Dim Offset As UInteger = r.ReadInt32
                Dim m_basestream As UInteger = r.BaseStream.Position

                r.BaseStream.Position = Offset
                Me.Phrases(i) = New Phrase(r)

                r.BaseStream.Position = m_basestream
            Next

            Select Case Me.Unknown_1
                Case 100
                Case Else

            End Select

        End Sub

        'Public Sub Save(ByVal w As BinaryWriter)

        'End Sub
        Public Property Id As Integer
        Public Property Unknown_1 As Integer
        Public Property Priority As Integer
        Public Property NumPhrases As Integer

        Public Property Phrases As Phrase()


    End Class
End Namespace