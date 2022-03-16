Namespace AudioBin.Sentences
    Public Class Parameter
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader)

            '-- 4 bytes : (02 FF FF FF (TagBuilderFixed) ) or ( 01 00 01 00 or 01 01 02 01 (TagBuilderEventRef) )
            Me.Type = r.ReadSByte  'maybe
            Me.EventIndex = r.ReadSByte  'FF when Me.Type = 2 --> maybe EventIndex
            Me.Unknown_2 = r.ReadSByte  'FF when Me.Type = 2 --> very maybe EventIndex
            Me.Unknown_3 = r.ReadSByte  'FF when Me.Type = 2 --> maybe EventIndex


            Me.TagId = r.ReadInt32

            Me.TagValue = r.ReadInt32  '-1 when Me.Type = 1 (value dont exist at TagBuilderEventRef)

        End Sub

        'Public Sub Save(ByVal w As BinaryWriter)

        'End Sub
        Public Property Type As EType
        Public Property EventIndex As SByte
        Public Property Unknown_2 As SByte
        Public Property Unknown_3 As SByte
        Public Property TagId As Integer
        Public Property TagValue As Integer


        Public Enum EType As SByte
            TagBuilderFixed = 2
            TagBuilderEventRef = 1
        End Enum
    End Class
End Namespace