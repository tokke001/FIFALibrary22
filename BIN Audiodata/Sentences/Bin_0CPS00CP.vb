Imports FIFALibrary22.AudioBin.EventSystem
Imports FIFALibrary22.AudioBin.RepetitionPools

Namespace AudioBin.Sentences
    Public Class SentencesFile
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As BinaryReader)
            '-- Header
            r.BaseStream.Position = 8

            r.ReadUInt16()  'unknown : version ?
            r.ReadUInt16()  'unknown : version ?
            r.ReadUInt16()  'unknown : "0"
            r.ReadUInt16()  'unknown : "0"

            Me.NumSections = r.ReadInt32
            Me.Unknown_1 = r.ReadInt32 '"0" padding ?

            Me.Sections = New Section(Me.NumSections - 1) {}
            Dim Index_Sentences As UInteger = 0

            For i = 0 To Me.NumSections - 1
                Dim NumSections As Integer = r.ReadInt32
                Dim Offset As Integer = r.ReadInt32

                If Offset <> -1 Then
                    Dim m_basestream As UInteger = r.BaseStream.Position

                    For j = 0 To NumSections - 1
                        r.BaseStream.Position = Offset + (j * 8)
                        Dim Section_Unknown As Integer = r.ReadInt32    'always 0, maybe another possible count
                        Dim Section_Offset As Integer = r.ReadInt32

                        r.BaseStream.Position = Section_Offset
                        Me.Sections(Index_Sentences) = New Section(r)
                        Index_Sentences += 1
                    Next

                    r.BaseStream.Position = m_basestream
                End If
            Next

        End Sub

        Public Function ToXml(ByVal FileName As String, Optional Bin_RepetitionPools As RepetitionPoolsFile = Nothing, Optional Bin_Eventsytem As EventSystemFile = Nothing) As String
            Dim StrXml As String = ""
            Dim IndexSorter As UInteger = 0
            Dim XmlSorter As XmlSorter() = Nothing '= Bin_XmlSorter(0){}


            StrXml &= "<AudioFramework>"
            StrXml &= vbNewLine & "  <Module type=""" & GetModuleType(FileName) & """ name=""" & GetModuleName(FileName) & """>"
            StrXml &= vbNewLine & "    <Version major=""3"" minor=""5"" patch=""0"" />"

            For i = 0 To Me.NumSections - 1

                For j = 0 To Me.Sections(i).NumSentences - 1
                    ReDim Preserve XmlSorter(IndexSorter)
                    XmlSorter(IndexSorter) = New XmlSorter
                    XmlSorter(IndexSorter).Id = Me.Sections(i).Sentences(j).Id

                    XmlSorter(IndexSorter).StrValue &= vbNewLine & "    <Sentence triggerName=""" & Me.Sections(i).TriggerName & """ id=""" & CStr(Me.Sections(i).Sentences(j).Id) & """ priority=""" & CStr(Me.Sections(i).Sentences(j).Priority) & """ numPhrases=""" & CStr(Me.Sections(i).Sentences(j).NumPhrases) & """>"

                    For k = 0 To Me.Sections(i).Sentences(j).NumPhrases - 1
                        XmlSorter(IndexSorter).StrValue &= vbNewLine & "      <Phrase numParameters=""" & CStr(Me.Sections(i).Sentences(j).Phrases(k).NumParamaters) & """ repetitionPool=""" & GetRepetitionPool(Bin_RepetitionPools, Me.Sections(i).Sentences(j).Phrases(k).RepetitionId) & """ repetitionId=""" & CStr(Me.Sections(i).Sentences(j).Phrases(k).RepetitionId) & """>"

                        For l = 0 To Me.Sections(i).Sentences(j).Phrases(k).NumParamaters - 1
                            Select Case Me.Sections(i).Sentences(j).Phrases(k).Parameters(l).Type
                                Case Parameter.EType.TagBuilderFixed
                                    XmlSorter(IndexSorter).StrValue &= vbNewLine & "        <TagBuilderFixed TagName=""" & GetBuilderFixed_TagName(Me.Sections(i).Sentences(j).Phrases(k).Parameters(l).TagId) & """ TagId=""" & CStr(Me.Sections(i).Sentences(j).Phrases(k).Parameters(l).TagId) & """ TagValueName=""" & GetBuilderFixed_TagValueName(Me.Sections(i).Sentences(j).Phrases(k).Parameters(l).TagValue) & """ TagValue=""" & CStr(Me.Sections(i).Sentences(j).Phrases(k).Parameters(l).TagValue) & """ />"
                                Case Parameter.EType.TagBuilderEventRef
                                    XmlSorter(IndexSorter).StrValue &= vbNewLine & "        <TagBuilderEventRef TagName=""" & GetBuilderEventRef_TagName(Bin_Eventsytem, Me.Sections(i).Sentences(j).Phrases(k).Parameters(l).TagId) & """ TagId=""" & CStr(Me.Sections(i).Sentences(j).Phrases(k).Parameters(l).TagId) & """ EventIndex=""" & CStr(Me.Sections(i).Sentences(j).Phrases(k).Parameters(l).EventIndex) & """ />"
                            End Select
                        Next

                        XmlSorter(IndexSorter).StrValue &= vbNewLine & "      </Phrase>"
                    Next

                    XmlSorter(IndexSorter).StrValue &= vbNewLine & "    </Sentence>"

                    IndexSorter += 1
                Next
            Next

            '--Sort
            If XmlSorter IsNot Nothing Then
                XmlSorter = XmlSorter.OrderBy(Function(c) c.Id).ToArray
                For i = 0 To XmlSorter.Length - 1
                    StrXml &= XmlSorter(i).StrValue
                Next
            End If
            StrXml &= vbNewLine & "  </Module>"
            StrXml &= vbNewLine & "</AudioFramework>"

            Return StrXml
        End Function

        Private Function GetModuleType(ByVal FileName As String) As String
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName.Contains("announcer_sentences")
                    Return "SpeechModule"
                Case FileName.Contains("commentary_sentences")
                    Return "SpeechModule"
                Case FileName.Contains("playercalls_sentences")
                    Return "SpeechModule"
                    'Case FileName = "sentences"    'dont exist
                    'Return "GraffitiPlayerModule"
            End Select

            Return ""
        End Function

        Private Function GetModuleName(ByVal FileName As String) As String
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName.Contains("announcer_sentences")
                    Return "Announcer"
                Case FileName.Contains("commentary_sentences")
                    Return "CommentarySpeech"
                Case FileName.Contains("playercalls_sentences")
                    Return "PlayerCalls"
                    'Case FileName = "sentences"    'dont exist
                    'Return "GraffitiPlayer"
            End Select

            Return ""
        End Function

        Private Function GetRepetitionPool(ByVal Bin_RepetitionPools As RepetitionPoolsFile, ByVal RepetitionId As UShort) As String
            If Bin_RepetitionPools IsNot Nothing Then
                For i = 0 To Bin_RepetitionPools.NumPools - 1
                    If Bin_RepetitionPools.Pools(i).Id = RepetitionId Then
                        Return Bin_RepetitionPools.Pools(i).Name
                    End If
                Next
            End If

            Return ""
        End Function

        Private Function GetBuilderFixed_TagName(ByVal TagId As Integer) As String
            'may be readed from graffitiruntime, but ussually dont contain the correct names....
            Return "SecretName_SampleBank"  'always "SecretName_SampleBank" ??!?
        End Function

        Dim Counter As UInteger = 0
        Private Function GetBuilderFixed_TagValueName(ByVal TagValue As Integer) As String
            Counter += 1
            'may be readed from graffitiruntime, but ussually dont contain the correct names....
            Return "name_" & Counter
        End Function

        Private Function GetBuilderEventRef_TagName(ByVal Bin_Eventsytem As EventSystemFile, ByVal TagId As Integer) As String
            If Bin_Eventsytem IsNot Nothing Then
                For i = 0 To Bin_Eventsytem.NumParameters - 1
                    If Bin_Eventsytem.Parameters(i).Id = TagId Then
                        Return Bin_Eventsytem.Parameters(i).Name
                    End If
                Next
            End If

            Return ""
        End Function

        Public Property NumSections As Integer

        Public Property Unknown_1 As Integer

        Public Property Sections As Section()

    End Class
End Namespace