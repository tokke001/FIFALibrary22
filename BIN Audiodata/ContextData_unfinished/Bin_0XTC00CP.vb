Public Class Bin_0XTC00CP
    Public Sub New()

    End Sub

    Public Sub New(ByVal r As BinaryReader)
        '-- Header
        r.BaseStream.Position = 8

        r.ReadUInt16()  'unknown : "4"  -> version?
        r.ReadUInt16()  'unknown : "3"  -> version?
        Me.OffsetScriptNames = r.ReadInt32

        Me.Unknown_1 = r.ReadInt32          '"0" value, or maybe OffsetScriptNames is a long value ?
        Me.Num_1 = r.ReadInt32  'same as Num_3
        Me.Num_2 = r.ReadInt32
        Me.Num_3 = r.ReadInt32  'same as Num_1

        Me.Unknown_2 = r.ReadInt32          '"0" value
        Me.Unknown_3 = r.ReadInt32          '"0" value

        Me.Ids = New Bin_0XTC00CP_Id(Me.Num_2 - 1) {}
        For i = 0 To Me.Ids.Length - 1
            Me.Ids(i) = New Bin_0XTC00CP_Id(r)
        Next





        '-- Read Offsets to Events
        r.BaseStream.Position = Me.Num_2
        Me.OffsetsEvents = New Integer(Me.Unknown_1 - 1) {}
        For i = 0 To Me.OffsetsEvents.Length - 1
            Me.OffsetsEvents(i) = r.ReadInt32
        Next
        '-- Read Offsets to Parameters
        r.BaseStream.Position = Me.Num_3
        Me.OffsetsParameters = New Integer(Me.Num_1 - 1) {}
        For i = 0 To Me.OffsetsParameters.Length - 1
            Me.OffsetsParameters(i) = r.ReadInt32
        Next



        '-- Read Parameters
        Me.Parameters = New Bin_0TVE00CP_Parameter(Me.Num_1 - 1) {}
        For i = 0 To Me.Parameters.Length - 1
            r.BaseStream.Position = Me.OffsetsParameters(i)
            Me.Parameters(i) = New Bin_0TVE00CP_Parameter(r)
        Next

    End Sub

    Public Function ToXml(ByVal FileName As String) As String
        Dim StrXml As String = ""

        '--Sort
        Me.Parameters = Me.Parameters.OrderBy(Function(c) c.Id).ToArray
        Me.Ids = Me.Ids.OrderBy(Function(c) c.Name).ToArray


        StrXml &= "<AudioFramework>"
        StrXml &= vbNewLine & "  <Module type=""" & GetModuleType(FileName) & """ name=""" & GetModuleName(FileName) & """" & GetModuleExtra(FileName) & ">"
        StrXml &= vbNewLine & "    <EventSystem systemCrc=""" & CStr(Me.OffsetScriptNames) & """ numParameters=""" & CStr(Me.Num_1) & """ numEvents=""" & CStr(Me.Unknown_1) & """>"
        StrXml &= vbNewLine & "      <Version major=""1"" minor=""2"" patch=""4"" />"

        For i = 0 To Me.Num_1 - 1
            Select Case Me.Parameters(i).Type
                Case Bin_0TVE00CP_Parameter.BinParameterType.EnumBit
                    StrXml &= vbNewLine & "      <enum name=""" & Me.Parameters(i).Name & """ id=""" & CStr(Me.Parameters(i).Id) & """ type=""EnumBit"" numValues=""" & CStr(Me.Parameters(i).NumValues) & """>"
                    For j = 0 To Me.Parameters(i).NumValues - 1
                        StrXml &= vbNewLine & "        <enumerator name=""" & Me.Parameters(i).ParameterValues(j).Name & """ value=""" & CStr(Me.Parameters(i).ParameterValues(j).Value) & """ />"
                    Next
                    StrXml &= vbNewLine & "      </enum>"

                Case Bin_0TVE00CP_Parameter.BinParameterType.Int
                    StrXml &= vbNewLine & "      <parameter name=""" & Me.Parameters(i).Name & """ id=""" & CStr(Me.Parameters(i).Id) & """ type=""Int"" />"

            End Select
        Next

        For i = 0 To Me.Unknown_1 - 1

            If Me.Ids(i).NumParameters = 0 Then
                StrXml &= vbNewLine & "      <function name=""" & Me.Ids(i).Name & """ interfaceCrc=""" & CStr(Me.Ids(i).InterfaceCrc) & """ numParameters=""" & CStr(Me.Ids(i).NumParameters) & """ />"
            Else
                StrXml &= vbNewLine & "      <function name=""" & Me.Ids(i).Name & """ interfaceCrc=""" & CStr(Me.Ids(i).InterfaceCrc) & """ numParameters=""" & CStr(Me.Ids(i).NumParameters) & """>"
                For j = 0 To Me.Ids(i).NumParameters - 1
                    Dim ParameterIndex As UInteger = GetParameterIndex(Me.Ids(i).ParameterIds(j))
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
        For i = 0 To Me.Num_1 - 1
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
    Public Property OffsetScriptNames As Integer
    Public Property Unknown_1 As Integer
    Public Property Num_1 As Integer

    Public Property Num_2 As Integer
    Public Property Num_3 As Integer
    Public Property Unknown_2 As Integer
    Public Property Unknown_3 As Integer

    Public Property OffsetsEvents As Integer() '= New UInteger(numEvents - 1) {}
    Public Property OffsetsParameters As Integer() '= New UInteger(numParameters - 1) {}

    Public Property Ids As Bin_0XTC00CP_Id()
    Public Property Parameters As Bin_0TVE00CP_Parameter()

End Class
