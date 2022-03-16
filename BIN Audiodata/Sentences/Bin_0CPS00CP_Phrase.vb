Namespace AudioBin.Sentences
    Public Class Phrase
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader)
            Me.RepetitionId = r.ReadInt16  'short : repetitionId (i think)
            Me.NumParamaters = r.ReadSByte  'byte : count
            Me.Unknown_1 = r.ReadSByte      'ussually "0", but in rare unknown cases "1"

            Me.Parameters = New Parameter(Me.NumParamaters - 1) {}
            For i = 0 To Parameters.Length - 1
                Me.Parameters(i) = New Parameter(r)
            Next

        End Sub

        'Public Sub Save(ByVal w As BinaryWriter)

        'End Sub
        Public Property RepetitionId As Short
        Public Property NumParamaters As SByte
        Public Property Unknown_1 As SByte
        Public Property Parameters As Parameter()


    End Class
End Namespace