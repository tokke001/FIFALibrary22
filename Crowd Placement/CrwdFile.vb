
Namespace CrowdDat
    'CrowdPlacement *.dat files
    Public Class CrwdFile
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
            Me.Header = New CrwdFileHeader(r)

            For i = 0 To Me.Header.NumSeats - 1
                Select Case Me.Header.Version
                    Case CrwdFileHeader.EVersion.TYPE_0103
                        Me.CrowdData.Add(New Seat0103(r))
                    Case CrwdFileHeader.EVersion.TYPE_0104
                        Me.CrowdData.Add(New Seat0104(r))
                    Case CrwdFileHeader.EVersion.TYPE_0105
                        Me.CrowdData.Add(New Seat0105(r))
                End Select
            Next

            Return True
        End Function
        Public Overridable Function Save(ByVal w As FileWriter) As Boolean
            Return Me.Save(Me.Header.Version, w)
        End Function

        Public Overridable Function Save(ByVal FileType As CrwdFileHeader.EVersion, ByVal w As FileWriter) As Boolean
            Me.Header.Version = FileType
            Me.Header.NumSeats = Me.CrowdData.Count

            For i = 0 To Me.Header.NumSeats - 1
                Select Case Me.Header.Version
                    Case CrwdFileHeader.EVersion.TYPE_0103
                        CType(Me.CrowdData(i), Seat0103).Save(w)
                    Case CrwdFileHeader.EVersion.TYPE_0104
                        CType(Me.CrowdData(i), Seat0104).Save(w)
                    Case CrwdFileHeader.EVersion.TYPE_0105
                        CType(Me.CrowdData(i), Seat0105).Save(w)
                End Select
            Next

            Return True
        End Function

        Public Function Save(ByVal FileName As String) As Boolean
            Return Save(FileName, Me.Header.Version)
        End Function

        Public Function Save(ByVal FileName As String, ByVal FileType As CrwdFileHeader.EVersion) As Boolean
            Dim output As New FileStream(FileName, FileMode.Create, FileAccess.ReadWrite)
            Dim w As New FileWriter(output, Endian.Little)
            Dim flag As Boolean = Me.Save(FileType, w)
            output.Close()
            w.Close()

            Return flag
        End Function

        'Public Function ToTxt() As String

        '    Return ""
        'End Function

        Public Property Header As New CrwdFileHeader
        Public Property CrowdData As List(Of Object)

    End Class
End Namespace