Imports FIFALibrary22.AudioBin.EventSystem
Imports FIFALibrary22.AudioBin.GraffitiRuntime
Imports FIFALibrary22.AudioBin.RepetitionPools
Imports FIFALibrary22.AudioBin.Sentences

Namespace AudioBin
    Public Class BinFile
        Public Function Load(ByVal FileName As String) As Boolean
            Dim f As New FileStream(FileName, FileMode.Open, FileAccess.ReadWrite)
            Dim r As New BinaryReader(f)
            Dim flag As Boolean = Me.Load(r)
            f.Close()
            r.Close()
            Return flag
        End Function
        Public Function Load(ByVal fifaFile As FifaFile) As Boolean
            If fifaFile.IsCompressed Then
                fifaFile.Decompress()
            End If
            Dim r As BinaryReader = fifaFile.GetReader
            Dim flag As Boolean = Me.Load(r)
            fifaFile.ReleaseReader(r)

            Return flag
        End Function
        Public Overridable Function Load(ByVal r As BinaryReader) As Boolean

            Me.Header = GetHeader(r)

            Select Case Header
                Case "0TVE00CP" ' *_eventsystem.bin
                    Me.BinData = New EventSystemFile(r)
                Case "0CPS00CP" ' *_sentences.bin
                    Me.BinData = New SentencesFile(r)
                Case "0FRG00CP" ' *_graffitiruntime.bin
                    Me.BinData = New GraffitiRuntimeFile(r)
                Case "0PER00CP" ' *_repetitionpools.bin
                    Me.BinData = New RepetitionPoolsFile(r)
                Case "0XTC00CP" ' *_contextdata.bin

                    'Case "0DWK00CP" 'keyworddatabase.bin (may not exist at FIFA 11)

                Case Else
                    Me.BinData = Nothing
            End Select

            Return True
        End Function

        Public Function GetHeader(ByVal r As BinaryReader) As String
            Return New String(r.ReadChars(8))
        End Function

        Public Function ToXml(ByVal FileName As String, Optional Bin_RepetitionPools As RepetitionPoolsFile = Nothing, Optional Bin_Eventsytem As EventSystemFile = Nothing) As String

            If Not (Me.Header = "" And Me.BinData Is Nothing) Then
                Select Case Header
                    Case "0TVE00CP" ' *_eventsystem.bin
                        Return CType(Me.BinData, EventSystemFile).ToXml(FileName)
                    Case "0CPS00CP" ' *_sentences.bin
                        Return CType(Me.BinData, SentencesFile).ToXml(FileName, Bin_RepetitionPools, Bin_Eventsytem)
                    Case "0FRG00CP" ' *_graffitiruntime.bin
                        Return CType(Me.BinData, GraffitiRuntimeFile).ToXml(FileName)
                    Case "0PER00CP" ' *_repetitionpools.bin
                        Return CType(Me.BinData, RepetitionPoolsFile).ToXml(FileName)

                        'Case "0XTC00CP" ' *_contextdata.bin

                        'Case "0DWK00CP" 'keyworddatabase.bin (may not exist at FIFA 11)

                End Select
            End If

            Return ""
        End Function


        Public Property Header As String
        Public Property BinData As Object

    End Class
End Namespace