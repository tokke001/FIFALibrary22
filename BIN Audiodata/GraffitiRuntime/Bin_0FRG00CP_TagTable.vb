Namespace AudioBin.GraffitiRuntime
    Public Class TagTable
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader, ByVal Flag_HasTagName As GraffitiRuntimeFile.EHasTagName)
            Me.TagId = r.ReadInt32
            Me.TagValue = r.ReadInt32
            Me.NumTags = r.ReadInt32
            Me.OffsetRefs = r.ReadInt32

            Dim m_basestream As Long = r.BaseStream.Position
            r.BaseStream.Position = 28 + Me.OffsetRefs
            Me.NumRefs = r.ReadInt32

            Me.DataRefs = New Integer(Me.NumRefs - 1) {}
            For i = 0 To Me.DataRefs.Length - 1
                Me.DataRefs(i) = r.ReadInt32
            Next

            'tag name : but not always present
            If Flag_HasTagName = GraffitiRuntimeFile.EHasTagName.f_True Then
                Me.TagName = FifaUtil.ReadNullTerminatedString(r)
            End If

            r.BaseStream.Position = m_basestream
        End Sub

        'Public Sub Save(ByVal w As BinaryWriter)

        'End Sub
        Public Property TagId As Integer
        Public Property TagValue As Integer
        Public Property NumTags As Integer
        Public Property OffsetRefs As Integer
        Public Property NumRefs As Integer

        Public Property DataRefs As Integer()
        Public Property TagName As String = ""


    End Class
End Namespace