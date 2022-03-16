
Public Class FifaBigFile
    Inherits FifaFile
    ' Methods
    Public Sub New(ByVal fifaFile As FifaFile)
        MyBase.New(fifaFile)
        Me.m_Alignement = 16
        If MyBase.IsCompressed Then
            MyBase.Decompress()
        End If
        Dim r As FileReader = MyBase.GetReader
        Me.Load(r)
        MyBase.ReleaseReader(r)
    End Sub

    Public Sub New(ByVal fileName As String)
        MyBase.New(fileName, True)
        Me.m_Alignement = 16
        If MyBase.IsCompressed Then
            MyBase.Decompress()
        End If
        Dim r As FileReader = MyBase.GetReader
        Me.Load(r)
        MyBase.ReleaseReader(r)
    End Sub

    Private Function ComputeHeaderSize() As Integer
        Dim v As Integer = 16
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            v = (v + (Me.m_Headers(i).Name.Length + 1))
            v = (v + 8)
        Next i
        Me.m_HeaderSize = v
        If (Not Me.m_EndSignature Is Nothing) Then
            Me.m_HeaderSize = (Me.m_HeaderSize + Me.m_EndSignature.Length)
        End If
        Return FifaUtil.RoundUp(v, Me.m_Alignement)
    End Function

    Public Function Delete(ByVal fifaFile As FifaFile) As Boolean
        Dim name As String = fifaFile.Name
        Return Me.Delete(name)
    End Function

    Public Function Delete(ByVal index As Integer) As Boolean
        If (index >= Me.m_NFiles) Then
            Return False
        End If
        Me.m_NFiles -= 1
        Dim i As Integer
        For i = index To Me.m_NFiles - 1
            Me.m_Files(i) = Me.m_Files((i + 1))
        Next i
        Dim j As Integer
        For j = index To Me.m_NFiles - 1
            Me.m_Headers(j) = Me.m_Headers((j + 1))
        Next j
        Return True
    End Function

    Public Function Delete(ByVal fileName As String) As Boolean
        Dim archivedFileIndex As Integer = Me.GetArchivedFileIndex(fileName, True)
        Return ((archivedFileIndex >= 0) AndAlso Me.Delete(archivedFileIndex))
    End Function

    Public Function Delete(ByVal fileNames As String()) As Integer
        Dim num As Integer = 0
        Dim i As Integer
        For i = 0 To fileNames.Length - 1
            If Me.Delete(fileNames(i)) Then
                num += 1
            End If
        Next i
        Return num
    End Function

    Private Function EstimateAlignement() As Integer
        Dim num As Integer = 256
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            Dim num3 As Integer = FifaUtil.ComputeAlignementLong(CLng(Me.m_Headers(i).StartPosition))
            If (num3 < num) Then
                num = num3
            End If
        Next i
        Return num
    End Function

    Public Function Export(ByVal fileIndex As Integer, ByVal exportDir As String) As Boolean
        Dim archivedFile As FifaFile = Me.GetArchivedFile(fileIndex)
        If (archivedFile Is Nothing) Then
            Return False
        End If
        If (archivedFile.UncompressedSize = 0) Then
            Return False
        End If
        Return archivedFile.Export(exportDir)
    End Function

    Public Function Export(ByVal fileName As String, ByVal exportDir As String) As Boolean
        Dim archivedFile As FifaFile = Me.GetArchivedFile(fileName, True)
        If (archivedFile Is Nothing) Then
            Return False
        End If
        If (archivedFile.UncompressedSize = 0) Then
            Return False
        End If
        Return archivedFile.Export(exportDir)
    End Function

    Public Function Export(ByVal fileNames As String(), ByVal exportDir As String) As Boolean
        Dim flag As Boolean = False
        If (fileNames.Length = 0) Then
            Return False
        End If
        Dim i As Integer
        For i = 0 To fileNames.Length - 1
            Dim flag2 As Boolean = Me.Export(fileNames(i), exportDir)
            flag = (flag Or flag2)
        Next i
        Return flag
    End Function

    Public Function GetArchivedFile(ByVal fileIndex As Integer) As FifaFile
        If ((fileIndex < 0) OrElse (fileIndex >= Me.NFiles)) Then
            Return Nothing
        End If
        If (Me.m_Files(fileIndex) Is Nothing) Then
            Dim r As FileReader = MyBase.GetReader
            Me.m_Files(fileIndex) = New FifaFile(Me.m_Headers(fileIndex), r)
            MyBase.ReleaseReader(r)
        End If
        Return Me.m_Files(fileIndex)
    End Function

    Public Function GetArchivedFile(ByVal fileName As String, ByVal useFullPath As Boolean) As FifaFile
        Dim archivedFiles As FifaFile() = Me.GetArchivedFiles(fileName, useFullPath)
        If (archivedFiles.Length = 0) Then
            Return Nothing
        End If
        If (archivedFiles.Length > 1) Then
            Return Nothing
        End If
        Return archivedFiles(0)
    End Function

    Public Function GetArchivedFileIndex(ByVal fileName As String, ByVal useFullPath As Boolean) As Integer
        If Not useFullPath Then
            fileName = Path.GetFileName(fileName)
        End If
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            If FifaUtil.CompareWildcardString(fileName, Me.m_Headers(i).Name) Then
                Return i
            End If
        Next i
        Return -1
    End Function

    Public Function GetArchivedFileNames(ByVal searchPattern As String, ByVal useFullPath As Boolean) As String()
        Dim flagArray As Boolean() = New Boolean(Me.m_NFiles - 1) {}
        Dim num As Integer = 0
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            Dim name As String = Me.m_Headers(i).Name
            If Not useFullPath Then
                name = Path.GetFileName(name)
            End If
            If flagArray(i) = FifaUtil.CompareWildcardString(searchPattern, name) Then
                num += 1
            End If
        Next i
        Dim strArray As String() = New String(num - 1) {}
        num = 0
        Dim j As Integer
        For j = 0 To Me.m_NFiles - 1
            If flagArray(j) Then
                strArray(num) = Me.m_Headers(j).Name
                num += 1
            End If
        Next j
        Return strArray
    End Function

    Public Function GetArchivedFiles(ByVal searchPattern As String, ByVal useFullPath As Boolean) As FifaFile()
        Dim flagArray As Boolean() = New Boolean(Me.m_NFiles - 1) {}
        Dim num As Integer = 0
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            Dim name As String = Me.m_Headers(i).Name
            If Not useFullPath Then
                name = Path.GetFileName(name)
            End If
            If flagArray(i) = FifaUtil.CompareWildcardString(searchPattern, name) Then
                num += 1
            End If
        Next i
        Dim fileArray As FifaFile() = New FifaFile(num - 1) {}
        If (num <> 0) Then
            Dim r As FileReader = Nothing
            Dim j As Integer
            For j = 0 To Me.m_NFiles - 1
                If (flagArray(j) AndAlso (Me.m_Files(j) Is Nothing)) Then
                    r = MyBase.GetReader
                    Exit For
                End If
            Next j
            num = 0
            Dim k As Integer
            For k = 0 To Me.m_NFiles - 1
                If flagArray(k) Then
                    If (Me.m_Files(k) Is Nothing) Then
                        Me.m_Files(k) = New FifaFile(Me.m_Headers(k), r)
                    End If
                    fileArray(num) = Me.m_Files(k)
                    num += 1
                End If
            Next k
            If (Not r Is Nothing) Then
                MyBase.ReleaseReader(r)
            End If
        End If
        Return fileArray
    End Function

    Public Function GetFirstDds() As FifaFile
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            If (Me.m_Files(i) Is Nothing) Then
                Me.LoadArchivedFiles()
                Exit For
            End If
        Next i
        Dim j As Integer
        For j = 0 To Me.m_NFiles - 1
            If Me.m_Files(j).IsDds Then
                Return Me.m_Files(j)
            End If
        Next j
        Return Nothing
    End Function

    Public Sub ImportFile(ByVal path As String, ByVal compressionMode As ECompressionMode)
        Dim fileName As String = IO.Path.GetFileName(path)
        Dim archivedFileIndex As Integer = Me.GetArchivedFileIndex(fileName, True)
        If (archivedFileIndex <> -1) Then
            Me.ImportReplacingFile(path, archivedFileIndex)
        Else
            Me.ImportNewFile(path, compressionMode)
        End If
    End Sub

    Public Sub ImportFileAs(ByVal path As String, ByVal archivedName As String, ByVal compressionMode As ECompressionMode)
        Dim archivedFileIndex As Integer = Me.GetArchivedFileIndex(archivedName, True)
        If (archivedFileIndex <> -1) Then
            Me.ImportReplacingFile(path, archivedFileIndex)
        Else
            Me.ImportNewFileAs(path, archivedName, compressionMode)
        End If
    End Sub

    Public Function ImportNewFile(ByVal path As String, ByVal compressionMode As ECompressionMode) As Integer
        Dim fileName As String = IO.Path.GetFileName(path)
        Return Me.ImportNewFileAs(path, fileName, compressionMode)
    End Function

    Public Function ImportNewFileAs(ByVal path As String, ByVal archivedName As String, ByVal compressionMode As ECompressionMode) As Integer
        If ((Me.m_Files Is Nothing) OrElse (Me.m_Files.Length <= (Me.m_NFiles + 1))) Then
            Me.Resize((Me.m_NFiles + 32))
        End If
        Dim input As New FileStream(path, FileMode.Open, FileAccess.Read)
        Dim reader1 As New FileReader(input)
        Dim buffer As Byte() = reader1.ReadBytes(CInt(reader1.BaseStream.Length))
        input.Close()
        reader1.Close()
        Me.m_Files(Me.m_NFiles) = New FifaFile(Me, buffer, archivedName, compressionMode)
        Me.m_NFiles += 1
        Return (Me.m_NFiles - 1)
    End Function

    Public Sub ImportReplacingFile(ByVal path As String, ByVal fileIndex As Integer)
        Dim input As New FileStream(path, FileMode.Open, FileAccess.Read)
        Dim reader1 As New FileReader(input)
        Dim buffer As Byte() = reader1.ReadBytes(CInt(reader1.BaseStream.Length))
        input.Close()
        reader1.Close()
        Dim archivedFile As FifaFile = Me.GetArchivedFile(fileIndex)
        If (archivedFile.CompressionMode = ECompressionMode.Chunkzip2) Then
            Me.m_Files(fileIndex) = New FifaFile(Me, buffer, archivedFile.Name, ECompressionMode.None)
        Else
            Me.m_Files(fileIndex) = New FifaFile(Me, buffer, archivedFile.Name, archivedFile.CompressionMode)
        End If
    End Sub

    Private Function Load(ByVal r As FileReader) As Boolean
        If (r Is Nothing) Then
            Return False
        End If
        If (r.BaseStream.Length < 16) Then
            Return False
        End If
        Dim chArray As Char() = r.ReadChars(4)
        If (((chArray(0) <> "B"c) OrElse (chArray(1) <> "I"c)) OrElse ((chArray(2) <> "G"c) OrElse ((chArray(3) <> "F"c) AndAlso (chArray(3) <> "4"c)))) Then
            Return False
        End If
        Me.m_TotalFileSize = r.ReadInt32
        Me.m_NFiles = FifaUtil.SwapEndian(r.ReadInt32)
        Me.m_HeaderSize = FifaUtil.SwapEndian(r.ReadInt32)
        Me.m_Headers = New FifaFileHeader(Me.m_NFiles - 1) {}
        Me.m_Files = New FifaFile(Me.m_NFiles - 1) {}
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            Me.m_Headers(i) = New FifaFileHeader(Me)
            Me.m_Headers(i).Load(r)
        Next i
        If (Me.m_HeaderSize = (CInt(r.BaseStream.Position) + 8)) Then
            Me.m_EndSignature = r.ReadBytes(8)
        End If
        Me.m_Alignement = Me.EstimateAlignement
        Return True
    End Function

    Public Function LoadArchivedFile(ByVal fileIndex As Integer) As Boolean
        If ((fileIndex < 0) OrElse (fileIndex >= Me.m_NFiles)) Then
            Return False
        End If
        Dim r As FileReader = MyBase.GetReader
        Me.m_Files(fileIndex) = New FifaFile(Me.m_Headers(fileIndex), r)
        MyBase.ReleaseReader(r)
        Return True
    End Function

    Public Function LoadArchivedFiles() As Boolean
        Dim r As FileReader = MyBase.GetReader
        Me.m_Files = New FifaFile(Me.m_NFiles - 1) {}
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            Me.m_Files(i) = New FifaFile(Me.m_Headers(i), r)
        Next i
        MyBase.ReleaseReader(r)
        Return True
    End Function

    Public Sub Rename(ByVal originalName As String, ByVal newName As String)
        Dim archivedFile As FifaFile = Me.GetArchivedFile(originalName, True)
        If (Not archivedFile Is Nothing) Then
            archivedFile.Rename(newName)
        End If
    End Sub

    Public Sub Resize(ByVal nFiles As Integer)
        If (Me.m_NFiles = 0) Then
            Me.m_Files = New FifaFile(nFiles - 1) {}
        Else
            Dim num1 As Integer = Me.m_NFiles
            Dim sourceArray As FifaFile() = DirectCast(Me.m_Files.Clone, FifaFile())
            Me.m_Files = New FifaFile(nFiles - 1) {}
            If (nFiles < Me.m_NFiles) Then
                Array.Copy(sourceArray, 0, Me.m_Files, 0, nFiles)
            End If
            If (nFiles > Me.m_NFiles) Then
                Array.Copy(sourceArray, 0, Me.m_Files, 0, Me.m_NFiles)
            End If
        End If
    End Sub

    Public Sub Save()
        Dim w As FileWriter = Nothing
        Dim r As FileReader = Nothing
        Dim output As FileStream = Nothing
        If (Me.NFiles = 0) Then
            w = MyBase.GetWriter
            w.Write(0)
        Else
            w = MyBase.GetWriter
            r = MyBase.GetReader
            Me.ComputeHeaderSize()
            w.BaseStream.Position = Me.m_HeaderSize
            Dim i As Integer
            For i = 0 To Me.m_NFiles - 1
                If (Me.m_Files(i) Is Nothing) Then
                    Me.m_Files(i) = New FifaFile(Me.m_Headers(i), r)
                End If
                Me.m_Headers(i).StartPosition = CUInt(w.BaseStream.Position)   'DirectCast(w.BaseStream.Position, UInt32)
                Me.m_Headers(i).Name = Me.m_Files(i).Name
                Me.m_Files(i).Save(w)
                If Me.m_Files(i).IsToCompress Then
                    Me.m_Headers(i).Size = Me.m_Files(i).CompressedSize
                Else
                    Me.m_Headers(i).Size = Me.m_Files(i).UncompressedSize
                End If
                Dim position As Integer = CInt(w.BaseStream.Position)
                position = FifaUtil.RoundUp(position, Me.m_Alignement)
                w.BaseStream.Position = position
            Next i
            Me.m_TotalFileSize = CInt(w.BaseStream.Position)
            w.Seek(0, SeekOrigin.Begin)
            w.Write("B"c)
            w.Write("I"c)
            w.Write("G"c)
            w.Write("4"c)
            w.Write(Me.m_TotalFileSize)
            w.Write(FifaUtil.SwapEndian(Me.m_NFiles))
            w.Write(FifaUtil.SwapEndian(Me.m_HeaderSize))
            Dim j As Integer
            For j = 0 To Me.m_NFiles - 1
                Me.m_Headers(j).Save(w)
            Next j
            If (Not Me.m_EndSignature Is Nothing) Then
                w.Write(Me.m_EndSignature)
            End If
        End If
        MyBase.ReleaseReader(r)
        MyBase.ReleaseWriter(w)
        If (MyBase.Archive Is Nothing) Then
            If MyBase.IsInMemory Then
                If (File.Exists(MyBase.PhysicalName) AndAlso ((File.GetAttributes(MyBase.PhysicalName) And FileAttributes.ReadOnly) = FileAttributes.ReadOnly)) Then
                    File.SetAttributes(MyBase.PhysicalName, FileAttributes.Archive)
                End If
                output = New FileStream(MyBase.PhysicalName, FileMode.Create, FileAccess.Write)
                Dim Writer1 As New FileWriter(output)
                MyBase.Save(output)
                output.Close()
            End If
        Else
            MyBase.Archive.Save
        End If
    End Sub

    Public Sub Sort()
        Dim keys As String() = New String(Me.m_NFiles - 1) {}
        Dim strArray2 As String() = New String(Me.m_NFiles - 1) {}
        Dim i As Integer
        For i = 0 To Me.m_NFiles - 1
            keys(i) = Me.Files(i).Name
            strArray2(i) = Me.Files(i).Name
        Next i
        Array.Sort(keys, Me.m_Files, FifaBigFile.s_StringComparer)
    End Sub


    ' Properties
    Public ReadOnly Property NFiles As Integer
        Get
            Return Me.m_NFiles
        End Get
    End Property

    Public ReadOnly Property Headers As FifaFileHeader()
        Get
            Return Me.m_Headers
        End Get
    End Property

    Public ReadOnly Property Files As FifaFile()
        Get
            Return Me.m_Files
        End Get
    End Property


    ' Fields
    Private m_TotalFileSize As Integer
    Private m_NFiles As Integer
    Private m_HeaderSize As Integer
    Private m_Headers As FifaFileHeader()
    Private Shared s_StringComparer As StringComparer = New StringComparer
    Private m_Files As FifaFile()
    Private m_EndSignature As Byte()
    Private m_Alignement As Integer

    ' Nested Types
    Public Class StringComparer
        Implements IComparer
        ' Methods
        Private Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim strB As String = CStr(y)
            Return String.Compare(CStr(x), strB, StringComparison.Ordinal)
        End Function

    End Class
End Class
