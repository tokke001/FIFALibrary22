Namespace AudioBin.RepetitionPools
    Public Class RepetitionPoolsFile
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As BinaryReader)
            '-- Header
            r.BaseStream.Position = 8

            r.ReadUInt16()  'unknown : version ?
            r.ReadUInt16()  'unknown : version ?
            r.ReadUInt16()  'unknown : "0"
            r.ReadUInt16()  'unknown : "0"

            Me.NumPools = r.ReadInt32
            Me.Unknown_1 = r.ReadInt32 '"0" padding ?

            '-- Read Pools
            Me.Pools = New Pool(Me.NumPools - 1) {}
            For i = 0 To Me.Pools.Length - 1
                Me.Pools(i) = New Pool(r)
            Next


        End Sub

        Public Function ToXml(ByVal FileName As String) As String
            Dim StrXml As String = ""

            '--Sort
            Me.Pools = Me.Pools.OrderBy(Function(c) c.Id).ToArray


            StrXml &= "<AudioFramework>"
            StrXml &= vbNewLine & "  <Module type=""" & GetModuleType(FileName) & """ name=""" & GetModuleName(FileName) & """>"
            StrXml &= vbNewLine & "    <RepetitionManager numPools=""" & CStr(Me.NumPools) & """>"
            StrXml &= vbNewLine & "      <Version major=""1"" minor=""1"" patch=""0"" />"

            For i = 0 To Me.NumPools - 1
                Select Case Me.Pools(i).PoolType

                    Case Pool.EPoolType.TimedRepetitionPool
                        StrXml &= vbNewLine & "      <TimedRepetitionPool name=""" & Me.Pools(i).Name & """ id=""" & CStr(Me.Pools(i).Id) & """ poolSize=""" & CStr(Me.Pools(i).PoolSize) & """ repeatTime=""" & CStr(Me.Pools(i).RepeatTime) & """ />"

                    Case Pool.EPoolType.UseOnceRepetitionPool
                        StrXml &= vbNewLine & "      <UseOnceRepetitionPool name=""" & Me.Pools(i).Name & """ id=""" & CStr(Me.Pools(i).Id) & """ poolSize=""" & CStr(Me.Pools(i).PoolSize) & """ />"

                    Case Pool.EPoolType.ShuffleRepetitionPool
                        StrXml &= vbNewLine & "      <ShuffleRepetitionPool name=""" & Me.Pools(i).Name & """ id=""" & CStr(Me.Pools(i).Id) & """ poolSize=""" & CStr(Me.Pools(i).PoolSize) & """ />"

                End Select
            Next

            StrXml &= vbNewLine & "    </RepetitionManager>"
            StrXml &= vbNewLine & "  </Module>"
            StrXml &= vbNewLine & "</AudioFramework>"

            Return StrXml
        End Function

        Private Function GetModuleType(ByVal FileName As String)
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName.Contains("announcer_repetitionpools")
                    Return "SpeechModule"
                Case FileName.Contains("commentary_repetitionpools")
                    Return "SpeechModule"
                Case FileName.Contains("playercalls_repetitionpools")
                    Return "SpeechModule"
                Case FileName = "repetitionpools"
                    Return "GraffitiPlayerModule"
            End Select

            Return ""
        End Function

        Private Function GetModuleName(ByVal FileName As String)
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName.Contains("announcer_repetitionpools")
                    Return "Announcer"
                Case FileName.Contains("commentary_repetitionpools")
                    Return "CommentarySpeech"
                Case FileName.Contains("playercalls_repetitionpools")
                    Return "PlayerCalls"
                Case FileName = "repetitionpools"
                    Return "GraffitiPlayer"
            End Select

            Return ""
        End Function

        Public Property NumPools As Integer

        Public Property Unknown_1 As Integer

        Public Property Pools As Pool()

    End Class
End Namespace