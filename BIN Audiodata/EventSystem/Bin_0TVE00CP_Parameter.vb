Namespace AudioBin.EventSystem
    Public Class Parameter
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader)
            Me.Type = r.ReadInt32  'not sure about this
            Me.Id = r.ReadInt32
            Me.NumValues = r.ReadInt32
            Me.Unknown_1 = r.ReadInt32     'Usually "0"
            Me.OffsetOffsetsValues = r.ReadInt32     'Offset to start of Offsets to values
            Me.Name = FifaUtil.ReadNullTerminatedString(r)

            r.BaseStream.Position = Me.OffsetOffsetsValues
            Me.OffsetsValues = New Integer(Me.NumValues - 1) {}
            For i = 0 To Me.OffsetsValues.Length - 1
                Me.OffsetsValues(i) = r.ReadInt32
            Next
            Me.ParameterValues = New ParameterValue(Me.NumValues - 1) {}
            For i = 0 To Me.ParameterValues.Length - 1
                r.BaseStream.Position = Me.OffsetsValues(i)
                Me.ParameterValues(i) = New ParameterValue(r)
            Next

        End Sub

        Public Property Type As BinParameterType
        Public Property Id As Integer
        Public Property NumValues As Integer
        Public Property Unknown_1 As Integer
        Public Property OffsetOffsetsValues As Integer
        Public Property Name As String
        Public Property OffsetsValues As Integer()
        Public Property ParameterValues As ParameterValue()

        Enum BinParameterType As Integer
            EnumBit = 1
            Int = 3
        End Enum
    End Class

End Namespace