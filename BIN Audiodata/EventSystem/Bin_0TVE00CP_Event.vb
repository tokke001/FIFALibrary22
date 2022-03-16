Namespace AudioBin.EventSystem
    Public Class [Event]
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader)
            Me.SizeParameters = r.ReadInt32
            Me.NumParameters = r.ReadInt32
            Me.Value_2 = r.ReadInt32       'usually "0"
            Me.Value_3 = r.ReadInt32       'usually "0"
            Me.Value_4 = r.ReadInt32       'usually "0"

            Me.SystemCrc = r.ReadInt16     'reference to general "EventSystem systemCrc"
            Me.InterfaceCrc = r.ReadInt16  'unique id

            Me.Value_5 = r.ReadInt32       'usually "0"
            Me.Value_6 = r.ReadInt32       'usually "0"
            Me.Value_7 = r.ReadInt32       'usually "0"
            Me.Value_8 = r.ReadInt32       'usually "0"
            Me.Value_9 = r.ReadInt32       'usually "0"
            Me.Value_10 = r.ReadInt32       'usually "0"

            Me.ParameterIds = New Integer(Me.NumParameters - 1) {}
            For i = 0 To Me.ParameterIds.Length - 1
                Me.ParameterIds(i) = r.ReadInt32   'id of parameter
            Next

            Me.Name = FifaUtil.ReadNullTerminatedString(r)


        End Sub

        'Public Sub Save(ByVal w As BinaryWriter)

        'End Sub
        Public Property SizeParameters As Integer
        Public Property NumParameters As Integer
        Public Property Value_2 As Integer
        Public Property Value_3 As Integer
        Public Property Value_4 As Integer

        Public Property SystemCrc As Short
        Public Property InterfaceCrc As Short

        Public Property Value_5 As Integer
        Public Property Value_6 As Integer
        Public Property Value_7 As Integer
        Public Property Value_8 As Integer
        Public Property Value_9 As Integer
        Public Property Value_10 As Integer

        Public Property ParameterIds As Integer()
        Public Property Name As String


    End Class
End Namespace