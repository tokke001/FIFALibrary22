Namespace AudioBin.GraffitiRuntime
    Public Class GraffitiRuntimeFile
        Public Sub New()

        End Sub
        Public Sub New(ByVal r As BinaryReader)
            '-- Header
            r.BaseStream.Position = 8

            r.ReadUInt16()  'unknown : version ?
            r.ReadUInt16()  'unknown : version ?
            r.ReadUInt16()  'unknown : "0"
            r.ReadUInt16()  'unknown : "0"

            Me.Flag_HasTagName = r.ReadInt32 'ussually 0 or 1 --> 1-value (i think) when there is a tag-name avaible
            Me.NumTables = r.ReadInt32
            Me.Unknown_1 = r.ReadInt32 'always 0

            For i = 0 To (Me.NumTables * 2) - 1
                r.ReadInt32()   'unknown/unneeded -> sequence or offsets?
            Next

            Me.TagTables = New TagTable(Me.NumTables - 1) {}
            For i = 0 To Me.TagTables.Length - 1
                Me.TagTables(i) = New TagTable(r, Me.Flag_HasTagName)
            Next

        End Sub

        Public Function ToXml(ByVal FileName As String) As String
            Dim StrXml As String = ""
            'Dim IdTagName As UInteger = 0

            '--Sort
            Me.TagTables = Me.TagTables.OrderBy(Function(c) c.TagValue).ToArray
            Me.TagTables = Me.TagTables.OrderBy(Function(c) c.TagId).ToArray

            StrXml &= "<AudioFramework>"
            StrXml &= vbNewLine & "  <Module type=""" & GetModuleType(FileName) & """ name=""" & GetModuleName(FileName) & """>"
            StrXml &= vbNewLine & "    <GraffitiDatabase numTables=""" & CStr(Me.NumTables) & """ numSamples=""" & CStr(GetNumSamples()) & """>"
            StrXml &= vbNewLine & "      <Version major=""2"" minor=""1"" patch=""0"" />"

            For i = 0 To Me.NumTables - 1
                StrXml &= vbNewLine & "      <TagTable TagName=""" & GetTagName(i) & """ numRefs=""" & CStr(Me.TagTables(i).NumRefs) & """ TagId=""" & CStr(Me.TagTables(i).TagId) & """ TagValue=""" & CStr(Me.TagTables(i).TagValue) & """ NumTags=""" & CStr(Me.TagTables(i).NumTags) & """>"
                For j = 0 To Me.TagTables(i).NumRefs - 1
                    StrXml &= vbNewLine & "        <DataRef value=""" & CStr(Me.TagTables(i).DataRefs(j)) & """ />"
                Next
                StrXml &= vbNewLine & "      </TagTable>"
            Next

            StrXml &= vbNewLine & "    </GraffitiDatabase>"
            StrXml &= vbNewLine & "  </Module>"
            StrXml &= vbNewLine & "</AudioFramework>"

            Return StrXml
        End Function

        Private Function GetModuleType(ByVal FileName As String)
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName.Contains("announcer_graffitiruntime")
                    Return "SpeechModule"
                Case FileName.Contains("commentary_graffitiruntime")
                    Return "SpeechModule"
                Case FileName.Contains("playercalls_graffitiruntime")
                    Return "SpeechModule"
                Case FileName = "graffitiruntime"
                    Return "GraffitiPlayerModule"
            End Select

            Return ""
        End Function

        Private Function GetModuleName(ByVal FileName As String)
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName.Contains("announcer_graffitiruntime")
                    Return "Announcer"
                Case FileName.Contains("commentary_graffitiruntime")
                    Return "CommentarySpeech"
                Case FileName.Contains("playercalls_graffitiruntime")
                    Return "PlayerCalls"
                Case FileName = "graffitiruntime"
                    Return "GraffitiPlayer"
            End Select

            Return ""
        End Function

        Private Function GetNumSamples() As UInteger
            Dim m_List As New List(Of UInteger)

            For i = 0 To Me.NumTables - 1
                For j = 0 To Me.TagTables(i).NumRefs - 1
                    If m_List.Contains(Me.TagTables(i).DataRefs(j)) = False Then
                        m_List.Add(Me.TagTables(i).DataRefs(j))
                    End If
                Next
            Next

            Return m_List.Count
        End Function

        Private Function GetTagName(ByVal Index As UInteger) As String

            If Me.Flag_HasTagName = EHasTagName.f_True Then
                Return Me.TagTables(Index).TagName
            Else
                Return "name_" & CStr(Index + 1)
            End If

        End Function

        Public Property Flag_HasTagName As EHasTagName
        Public Property NumTables As Integer
        Public Property Unknown_1 As Integer

        Public Property TagTables As TagTable() '= New UInteger(numEvents - 1) {}

        Public Enum EHasTagName As Integer
            f_False = 0
            f_True = 1
        End Enum

    End Class
End Namespace