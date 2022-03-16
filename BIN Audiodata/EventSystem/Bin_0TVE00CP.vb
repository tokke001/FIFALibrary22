Namespace AudioBin.EventSystem
    Public Class EventSystemFile
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As BinaryReader)
            '-- Header
            r.BaseStream.Position = 8

            r.ReadUInt16()  'unknown : "4"
            r.ReadUInt16()  'unknown : "5"
            r.ReadUInt16()  'unknown : "0"
            r.ReadUInt16()  'unknown : "0"

            Me.SystemCrc = r.ReadInt32
            Me.NumEvents = r.ReadInt32
            Me.NumParameters = r.ReadInt32

            Me.OffsetOffsetsEvents = r.ReadInt32
            Me.OffsetOffsetsParameters = r.ReadInt32

            '-- Read Offsets to Events
            r.BaseStream.Position = Me.OffsetOffsetsEvents
            Me.OffsetsEvents = New Integer(Me.NumEvents - 1) {}
            For i = 0 To Me.OffsetsEvents.Length - 1
                Me.OffsetsEvents(i) = r.ReadInt32
            Next
            '-- Read Offsets to Parameters
            r.BaseStream.Position = Me.OffsetOffsetsParameters
            Me.OffsetsParameters = New Integer(Me.NumParameters - 1) {}
            For i = 0 To Me.OffsetsParameters.Length - 1
                Me.OffsetsParameters(i) = r.ReadInt32
            Next

            '-- Read Events
            Me.Events = New [Event](Me.NumEvents - 1) {}
            For i = 0 To Me.Events.Length - 1
                r.BaseStream.Position = Me.OffsetsEvents(i)
                Me.Events(i) = New [Event](r)
            Next

            '-- Read Parameters
            Me.Parameters = New Parameter(Me.NumParameters - 1) {}
            For i = 0 To Me.Parameters.Length - 1
                r.BaseStream.Position = Me.OffsetsParameters(i)
                Me.Parameters(i) = New Parameter(r)
            Next

        End Sub

        Public Function ToXml(ByVal FileName As String) As String
            Dim StrXml As String = ""

            '--Sort
            Me.Parameters = Me.Parameters.OrderBy(Function(c) c.Id).ToArray
            Me.Events = Me.Events.OrderBy(Function(c) c.Name).ToArray


            StrXml &= "<AudioFramework>"
            StrXml &= vbNewLine & "  <Module type=""" & GetModuleType(FileName) & """ name=""" & GetModuleName(FileName) & """" & GetModuleExtra(FileName) & ">"
            StrXml &= vbNewLine & "    <EventSystem systemCrc=""" & CStr(Me.SystemCrc) & """ numParameters=""" & CStr(Me.NumParameters) & """ numEvents=""" & CStr(Me.NumEvents) & """>"
            StrXml &= vbNewLine & "      <Version major=""1"" minor=""2"" patch=""4"" />"

            For i = 0 To Me.NumParameters - 1
                Select Case Me.Parameters(i).Type
                    Case Parameter.BinParameterType.EnumBit
                        StrXml &= vbNewLine & "      <enum name=""" & Me.Parameters(i).Name & """ id=""" & CStr(Me.Parameters(i).Id) & """ type=""EnumBit"" numValues=""" & CStr(Me.Parameters(i).NumValues) & """>"
                        For j = 0 To Me.Parameters(i).NumValues - 1
                            StrXml &= vbNewLine & "        <enumerator name=""" & Me.Parameters(i).ParameterValues(j).Name & """ value=""" & CStr(Me.Parameters(i).ParameterValues(j).Value) & """ />"
                        Next
                        StrXml &= vbNewLine & "      </enum>"

                    Case Parameter.BinParameterType.Int
                        StrXml &= vbNewLine & "      <parameter name=""" & Me.Parameters(i).Name & """ id=""" & CStr(Me.Parameters(i).Id) & """ type=""Int"" />"

                End Select
            Next

            For i = 0 To Me.NumEvents - 1

                If Me.Events(i).NumParameters = 0 Then
                    StrXml &= vbNewLine & "      <function name=""" & Me.Events(i).Name & """ interfaceCrc=""" & CStr(Me.Events(i).InterfaceCrc) & """ numParameters=""" & CStr(Me.Events(i).NumParameters) & """ />"
                Else
                    StrXml &= vbNewLine & "      <function name=""" & Me.Events(i).Name & """ interfaceCrc=""" & CStr(Me.Events(i).InterfaceCrc) & """ numParameters=""" & CStr(Me.Events(i).NumParameters) & """>"
                    For j = 0 To Me.Events(i).NumParameters - 1
                        Dim ParameterIndex As UInteger = GetParameterIndex(Me.Events(i).ParameterIds(j))
                        StrXml &= vbNewLine & "        <parameter type=""" & Me.Parameters(ParameterIndex).Name & """ name=""" & Me.Parameters(ParameterIndex).Name & """ parameterId=""" & Me.Parameters(ParameterIndex).Id & """ />"
                    Next
                    StrXml &= vbNewLine & "      </function>"

                End If

            Next


            StrXml &= vbNewLine & "    </EventSystem>"
            StrXml &= vbNewLine & "  </Module>"
            StrXml &= vbNewLine & "</AudioFramework>"

            Return StrXml
        End Function

        Private Function GetParameterIndex(ByVal ParameterId As UInteger) As UInteger
            For i = 0 To Me.NumParameters - 1
                If Me.Parameters(i).Id = ParameterId Then
                    Return i
                End If
            Next

            Return 0
        End Function
        Private Function GetModuleType(ByVal FileName As String)
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName.Contains("announcer_eventsystem")
                    Return "SpeechModule"
                Case FileName.Contains("commentary_eventsystem")
                    Return "SpeechModule"
                Case FileName.Contains("playercalls_eventsystem")
                    Return "SpeechModule"
                Case FileName = "chant_eventsystem"
                    Return "GraffitiPlayerModule"
                Case FileName = "crowd_eventsystem"
                    Return "ContextModule"
                Case FileName = "commentary_triggers_eventsystem"
                    Return "ContextModule"
            End Select

            Return ""
        End Function

        Private Function GetModuleName(ByVal FileName As String)
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName.Contains("announcer_eventsystem")
                    Return "Announcer"
                Case FileName.Contains("commentary_eventsystem")
                    Return "CommentarySpeech"
                Case FileName.Contains("playercalls_eventsystem")
                    Return "PlayerCalls"
                Case FileName = "chant_eventsystem"
                    Return "GraffitiPlayer"
                Case FileName = "crowd_eventsystem"
                    Return "CrowdContextModule"
                Case FileName = "commentary_triggers_eventsystem"
                    Return "ContextModule"
            End Select

            Return ""
        End Function

        Private Function GetModuleExtra(ByVal FileName As String)
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            Select Case True
                Case FileName = "crowd_eventsystem"
                    Return " maxNumContexts=""750"" maxNumGroups=""100"" maxNumValidContexts=""400"" maxNumTriggeredContexts=""100"" numPassives=""150"" priorityDecayRate=""1000"""
                Case FileName = "commentary_triggers_eventsystem"
                    Return " maxNumContexts=""1200"" maxNumGroups=""100"" maxNumValidContexts=""500"" maxNumTriggeredContexts=""150"" numPassives=""150"" priorityDecayRate=""1000"""
            End Select

            Return ""
        End Function
        Public Property SystemCrc As Integer
        Public Property NumEvents As Integer
        Public Property NumParameters As Integer

        Public Property OffsetOffsetsEvents As Integer
        Public Property OffsetOffsetsParameters As Integer

        Public Property OffsetsEvents As Integer() '= New UInteger(numEvents - 1) {}
        Public Property OffsetsParameters As Integer() '= New UInteger(numParameters - 1) {}

        Public Property Events As [Event]()
        Public Property Parameters As Parameter()

    End Class
End Namespace