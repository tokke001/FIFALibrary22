
Namespace Scale
    'bodytype *.scale files
<Serializable> Public Class ScaleFile
        Public Function Load(ByVal FileName As String) As Boolean
            Dim f As New FileStream(FileName, FileMode.Open, FileAccess.ReadWrite)
            Dim r As New FileReader(f, Endian.Little)
            Dim flag As Boolean = Me.Load(r)
            f.Close()
            r.Close()
            Return flag
        End Function
        Public Function Load(ByVal fifaFile As FifaFile) As Boolean
            If fifaFile.IsCompressed Then
                fifaFile.Decompress()
            End If
            Dim r As FileReader = fifaFile.GetReader
            r.Endianness = Endian.Little
            Dim flag As Boolean = Me.Load(r)
            fifaFile.ReleaseReader(r)

            Return flag
        End Function
        Public Overridable Function Load(ByVal r As FileReader) As Boolean
            Me.Magic = New String(r.ReadChars(4))
            If Me.Magic <> "SCLl" Then
                Return False
            End If
            Me.Unknown_1 = r.ReadUInt32
            Me.Unknown_2 = r.ReadUInt32
            Me.Height = r.ReadSingle

            Me.Reach = r.ReadSingle
            Me.HeadStall = r.ReadSingle
            Me.NumAiScales = r.ReadUInt32

            For i = 0 To Me.NumAiScales - 1
                Me.AiScales.Add(New AiScale(r))
            Next

            'Select Case Me.NumAiScales
            '    Case 23
            '    Case Else
            'End Select
            'Select Case Me.Unknown_1
            '    Case 1
            '    Case Else
            'End Select
            'Select Case Me.Unknown_2
            '    Case 0
            '    Case Else
            'End Select
            Return True
        End Function

        Public Overridable Function Save(ByVal w As FileWriter) As Boolean
            Me.NumAiScales = If(Me.AiScales?.Count, 0)

            w.Write(Me.Magic.ToCharArray)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            w.Write(Me.Height)

            w.Write(Me.Reach)
            w.Write(Me.HeadStall)
            w.Write(Me.NumAiScales)

            For i = 0 To Me.NumAiScales - 1
                Me.AiScales(i).Save(w)
            Next

            Return True
        End Function

        Public Function Save(ByVal FileName As String) As Boolean
            Dim output As New FileStream(FileName, FileMode.Create, FileAccess.ReadWrite)
            Dim w As New FileWriter(output, Endian.Little)
            Dim flag As Boolean = Me.Save(w)
            output.Close()
            w.Close()

            Return flag
        End Function

        'Public Function ToTxt() As String

        '    Return ""
        'End Function

        Public Property Magic As String = "SCLl"    'magic
        Public Property Unknown_1 As UInteger = 1   'always 1 -> version, number of scaledata, ...?
        Public Property Unknown_2 As UInteger = 0   'always 0 -> padding, index of scaledata , ... ?
        Public Property Height As Single
        Public Property Reach As Single
        Public Property HeadStall As Single
        Public Property NumAiScales As UInteger
        Public Property AiScales As New List(Of AiScale)

    End Class
End Namespace