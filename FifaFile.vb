Imports zlib
'Imports FifaLibrary

'Namespace FIFALibrary22
Public Class FifaFile
    ' Methods
    Public Sub New(ByVal fifaFile As FifaFile)
        Me.Load(fifaFile)
    End Sub

    Public Sub New(ByVal header As FifaFileHeader, ByVal r As FileReader)
        Me.Load(header.BigFile, header.StartPosition, header.Size, header.Name, False, r)
    End Sub

    Public Sub New(ByVal path As String, ByVal isAnArchive As Boolean)
        Me.Load(path, isAnArchive)
    End Sub

    Public Sub New(ByVal archive As FifaBigFile, ByVal buffer As Byte(), ByVal name As String, ByVal compressionMode As ECompressionMode)
        Me.Load(archive, buffer, name, compressionMode)
    End Sub
    Private Function CheckCompressionMode(ByVal r As FileReader) As ECompressionMode
        If (r.BaseStream.Length < 8) Then
            Return ECompressionMode.None
        End If
        Dim position As Long = r.BaseStream.Position
        Dim buffer As Byte() = r.ReadBytes(8)
        If ((buffer(0) = 16) AndAlso (buffer(1) = 251)) Then
            Me.m_CurrentCompression = ECompressionMode.Compressed_10FB
            Me.m_RequiredCompression = Me.m_CurrentCompression
            Me.m_UncompressedSize = (((CInt(buffer(2)) << 16) + (CInt(buffer(3)) << 8)) + CInt(buffer(4)))
        Else
            Dim chArray As Char() = New Char(8 - 1) {}
            For i = 0 To 8 - 1
                chArray(i) = ChrW(buffer(i)) ' DirectCast(buffer(i), Char)
            Next i
            Dim str As New String(chArray)
            If str.StartsWith("EASF") Then
                Me.m_CompressedSize = ((((CInt(buffer(4)) << 24) + (CInt(buffer(5)) << 16)) + (CInt(buffer(6)) << 8)) + CInt(buffer(7)))
                r.ReadBytes(8)
                Me.m_CurrentCompression = ECompressionMode.EASF
                Me.m_RequiredCompression = Me.m_CurrentCompression
            ElseIf (str = "chunkzip") Then
                Me.m_UncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
                Me.m_CurrentCompression = ECompressionMode.Chunkzip
                If (Me.m_UncompressedSize = 2) Then
                    Me.m_UncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
                    r.ReadInt32()
                    r.ReadInt32()
                    r.ReadInt32()
                    r.ReadInt32()
                    r.ReadInt32()
                    r.ReadInt32()
                    FifaUtil.SwapEndian(r.ReadInt32)
                    r.ReadInt32()
                    Me.m_CurrentCompression = ECompressionMode.Chunkzip2
                    Me.m_RequiredCompression = Me.m_CurrentCompression
                Else
                    Me.m_MaxBlockUncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
                    Me.m_RequiredCompression = Me.m_CurrentCompression
                End If
            ElseIf (str = "chunkref") Then
                Me.m_UncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
                If (Me.m_UncompressedSize = 2) Then
                    Me.m_UncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
                    r.ReadInt32()
                    r.ReadInt32()
                    r.ReadInt32()
                    r.ReadInt32()
                    r.ReadInt32()
                    r.ReadInt32()
                    Me.m_MaxBlockUncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
                    r.ReadInt32()
                    Me.m_CurrentCompression = ECompressionMode.Chunkref2
                Else
                    FifaUtil.SwapEndian(r.ReadInt32)
                    FifaUtil.SwapEndian(r.ReadInt32)
                    buffer = r.ReadBytes(8)
                    If ((buffer(0) = 16) AndAlso (buffer(1) = 251)) Then
                        Me.m_CurrentCompression = ECompressionMode.Chunkref
                        Me.m_RequiredCompression = Me.m_CurrentCompression
                        Me.m_UncompressedSize = (((CInt(buffer(2)) << 16) + (CInt(buffer(3)) << 8)) + CInt(buffer(4)))
                    Else
                        Me.m_CurrentCompression = ECompressionMode.Unknown
                    End If
                End If
                Me.m_RequiredCompression = Me.m_CurrentCompression
            ElseIf (str = "chunlzma") Then
                Me.m_CurrentCompression = ECompressionMode.Chunklzma
                Me.m_RequiredCompression = Me.m_CurrentCompression
            ElseIf (str = "chunklzx") Then
                Me.m_CurrentCompression = ECompressionMode.chunklzx
                Me.m_RequiredCompression = Me.m_CurrentCompression
            ElseIf (str = "chunklz4") Then
                Me.m_CurrentCompression = ECompressionMode.chunklz4
                Me.m_RequiredCompression = Me.m_CurrentCompression
            ElseIf (str = "chunkunc") Then
                Me.m_CurrentCompression = ECompressionMode.chunkunc
                Me.m_RequiredCompression = Me.m_CurrentCompression
            ElseIf (str = "chunzstd") Then
                Me.m_CurrentCompression = ECompressionMode.chunzstd
                Me.m_RequiredCompression = Me.m_CurrentCompression
            ElseIf (str = "chunoodl") Then
                Me.m_CurrentCompression = ECompressionMode.chunoodl
                Me.m_RequiredCompression = Me.m_CurrentCompression
            Else
                Me.m_CurrentCompression = ECompressionMode.None
                Me.m_RequiredCompression = Me.m_CurrentCompression
            End If
        End If
        r.BaseStream.Position = position
        Return Me.m_CurrentCompression
    End Function

    Private Sub Chunkref(ByVal outputStream As Stream)
        Me.Chunkref(outputStream, Me.m_UncompressedSize)
    End Sub

    Private Sub Chunkref(ByVal outputStream As Stream, ByVal uncompressedSize As Integer)
        Dim r As FileReader = Me.GetReader
        Dim writer As New FileWriter(outputStream)
        Me.m_UncompressedSize = uncompressedSize
        Me.m_MaxBlockUncompressedSize = 235520
        Dim position As Long = outputStream.Position
        writer.Write("c"c)
        writer.Write("h"c)
        writer.Write("u"c)
        writer.Write("n"c)
        writer.Write("k"c)
        writer.Write("r"c)
        writer.Write("e"c)
        writer.Write("f"c)
        writer.Write(FifaUtil.SwapEndian(Me.m_UncompressedSize))
        writer.Write(FifaUtil.SwapEndian(Me.m_MaxBlockUncompressedSize))
        Dim num2 As Integer = 0
        Do While True
            Dim num3 As Integer = CInt(outputStream.Position)
            writer.Write(-1)
            Dim maxBlockUncompressedSize As Integer = Me.m_MaxBlockUncompressedSize
            If ((Me.m_UncompressedSize - num2) < Me.m_MaxBlockUncompressedSize) Then
                maxBlockUncompressedSize = (Me.m_UncompressedSize - num2)
            End If
            num2 = (num2 + maxBlockUncompressedSize)
            Dim inputBuffer As Byte() = r.ReadBytes(maxBlockUncompressedSize)
            Dim buffer As Byte() = Me.Compress_10FB(inputBuffer)
            Dim length As Integer = buffer.Length
            outputStream.Write(buffer, 0, length)
            Dim num6 As Long = outputStream.Position
            outputStream.Position = num3
            writer.Write(FifaUtil.SwapEndian(length))
            outputStream.Position = num6
            If (num2 >= Me.m_UncompressedSize) Then
                Me.ReleaseReader(r)
                Me.m_CompressedSize = CInt((outputStream.Position - position))
                Me.m_CurrentCompression = ECompressionMode.Chunkref
                Return
            End If
        Loop
    End Sub

    Private Sub Chunkref2(ByVal outputStream As Stream, ByVal uncompressedSize As Integer)
    End Sub

    Private Sub Chunkzip(ByVal outputStream As Stream)
        Me.Chunkzip(outputStream, Me.m_UncompressedSize)
    End Sub

    Private Sub Chunkzip(ByVal outputStream As Stream, ByVal uncompressedSize As Integer)
        Dim r As FileReader = Me.GetReader
        Me.m_UncompressedSize = uncompressedSize
        Me.m_MaxBlockUncompressedSize = 184320
        Dim writer As New FileWriter(outputStream)
        Dim position As Long = outputStream.Position
        writer.Write("c"c)
        writer.Write("h"c)
        writer.Write("u"c)
        writer.Write("n"c)
        writer.Write("k"c)
        writer.Write("z"c)
        writer.Write("i"c)
        writer.Write("p"c)
        writer.Write(FifaUtil.SwapEndian(Me.m_UncompressedSize))
        writer.Write(FifaUtil.SwapEndian(Me.m_MaxBlockUncompressedSize))
        Dim num2 As Integer = 0
        Do While True
            Dim num3 As Integer = CInt(outputStream.Position)
            writer.Write(-1)
            Dim maxBlockUncompressedSize As Integer = Me.m_MaxBlockUncompressedSize
            If ((Me.m_UncompressedSize - num2) < Me.m_MaxBlockUncompressedSize) Then
                maxBlockUncompressedSize = (Me.m_UncompressedSize - num2)
            End If
            num2 = (num2 + maxBlockUncompressedSize)
            Dim x As Integer = Me.DeflateBlock(r.BaseStream, outputStream, maxBlockUncompressedSize)
            Dim num6 As Integer = CInt(outputStream.Position)
            outputStream.Position = num3
            writer.Write(FifaUtil.SwapEndian(x))
            outputStream.Position = num6
            If (num2 >= Me.m_UncompressedSize) Then
                Me.ReleaseReader(r)
                Me.m_CompressedSize = CInt((outputStream.Position - position))
                Return
            End If
        Loop
    End Sub

    Public Function Compress(ByVal outputStream As Stream) As Boolean
        If (Me.m_CurrentCompression <> ECompressionMode.None) Then
            outputStream.Write(Me.Read, 0, Me.m_CompressedSize)
            Return True
        End If
        If (Me.m_CurrentCompression = Me.m_RequiredCompression) Then
            outputStream.Write(Me.Read, 0, Me.m_CompressedSize)
            Return True
        End If
        Select Case Me.m_RequiredCompression
            Case ECompressionMode.Compressed_10FB
                Me.Compress_10FB(outputStream)
                Exit Select
            Case ECompressionMode.Chunkzip
                Me.Chunkzip(outputStream)
                Exit Select
            Case ECompressionMode.Chunkref
                Me.Chunkref(outputStream, Me.m_UncompressedSize)
                Exit Select
            Case Else
                Exit Select
        End Select
        Return True
    End Function

    Private Sub Compress_10FB(ByVal outputStream As Stream)
        Dim inputBuffer As Byte() = Me.Read
        Dim buffer As Byte() = Me.Compress_10FB(inputBuffer)
        Me.m_CompressedSize = buffer.Length
        Me.m_CurrentCompression = ECompressionMode.Compressed_10FB
        outputStream.Write(buffer, 0, buffer.Length)
    End Sub

    Private Function Compress_10FB(ByVal inputBuffer As Byte()) As Byte()
        Dim length As Integer
        Dim num2 As Integer
        Dim num3 As Integer
        Dim num4 As Integer
        Dim numArray2 As Integer(,)
        Dim buffer As Byte()
        Dim num6 As Integer
        Dim num12 As Integer
        Dim num13 As Integer
        Dim num14 As Integer
        Dim num15 As Integer
        Dim num5 As Integer = 0
        Dim numArray As Integer() = New Integer(131072 - 1) {}
        Dim index As Integer = 0
        Do While True
            If (index >= 131072) Then
                numArray2 = New Integer(256 - 1, 256 - 1) {}
                Dim num8 As Integer = 0
                Do While True
                    If (num8 >= 256) Then
                        length = inputBuffer.Length
                        buffer = New Byte() {16, 251, CByte((length >> 16)), CByte(((length >> 8) And 255)), CByte((length And 255))}
                        num4 = 5
                        num2 = 0
                        num3 = 0
                        Exit Do
                    End If
                    Dim num9 As Integer = 0
                    Do While True
                        If (num9 >= 256) Then
                            num8 += 1
                            Exit Do
                        End If
                        numArray2(num8, num9) = -1
                        num9 += 1
                    Loop
                Loop
                Exit Do
            End If
            numArray(index) = -1
            index += 1
        Loop
        GoTo TR_004F
TR_0017:
        num2 += 1
        num3 += 1
        GoTo TR_004F
TR_003D:
        If (num13 > (length - num2)) Then
            num13 = (num2 - length)
        End If
        If (num13 <= 2) Then
            num13 = 0
        End If
        If ((num13 = 3) AndAlso (num14 > 1024)) Then
            num13 = 0
        End If
        If ((num13 = 4) AndAlso (num14 > 16384)) Then
            num13 = 0
        End If
        If (num13 <> 0) Then
            Do While True
                If ((num2 - num5) < 4) Then
                    num6 = (num2 - num5)
                    If ((num13 <= 10) AndAlso (num14 <= 1024)) Then
                        buffer(num4) = CByte((((((num14 - 1) >> 8) << 5) + ((num13 - 3) << 2)) + num6))
                        num4 += 1
                        buffer(num4) = CByte(((num14 - 1) And 255))
                        num4 += 1
                        Do While True
                            If (num6 = 0) Then
                                num6 -= 1
                                num5 = (num5 + num13)
                                Exit Do
                            End If
                            buffer(num4) = inputBuffer(num5)
                            num4 += 1
                            num5 += 1
                        Loop
                    ElseIf ((num13 <= 67) AndAlso (num14 <= 16384)) Then
                        buffer(num4) = CByte((128 + (num13 - 4)))
                        num4 += 1
                        buffer(num4) = CByte(((num6 << 6) + ((num14 - 1) >> 8)))
                        num4 += 1
                        buffer(num4) = CByte(((num14 - 1) And 255))
                        num4 += 1
                        Do While True
                            If (num6 = 0) Then
                                num6 -= 1
                                num5 = (num5 + num13)
                                Exit Do
                            End If
                            buffer(num4) = inputBuffer(num5)
                            num4 += 1
                            num5 += 1
                        Loop
                    ElseIf ((num13 <= 1028) AndAlso (num14 < 131072)) Then
                        num14 -= 1
                        buffer(num4) = CByte((((192 + ((num14 >> 16) << 4)) + (((num13 - 5) >> 8) << 2)) + num6))
                        num4 += 1
                        buffer(num4) = CByte(((num14 >> 8) And 255))
                        num4 += 1
                        buffer(num4) = CByte((num14 And 255))
                        num4 += 1
                        buffer(num4) = CByte(((num13 - 5) And 255))
                        num4 += 1
                        Do While True
                            If (num6 = 0) Then
                                num6 -= 1
                                num5 = (num5 + num13)
                                Exit Do
                            End If
                            buffer(num4) = inputBuffer(num5)
                            num4 += 1
                            num5 += 1
                        Loop
                    End If
                    Exit Do
                End If
                num6 = (((num2 - num5) \ 4) - 1)
                If (num6 > 27) Then
                    num6 = 27
                End If
                buffer(num4) = CByte((224 + num6))
                num4 += 1
                num6 = ((4 * num6) + 4)
                Dim num18 As Integer = 0
                Do While True
                    If (num18 >= num6) Then
                        num5 = (num5 + num6)
                        num4 = (num4 + num6)
                        Exit Do
                    End If
                    buffer((num4 + num18)) = inputBuffer((num5 + num18))
                    num18 += 1
                Loop
            Loop
        End If
        GoTo TR_0017
TR_004F:
        Do While True
            If (num2 >= (length - 1)) Then
                num2 = length
                Do While ((num2 - num5) >= 4)
                    num6 = (((num2 - num5) \ 4) - 1)
                    If (num6 > 27) Then
                        num6 = 27
                    End If
                    buffer(num4) = CByte((224 + num6))
                    num4 += 1
                    num6 = ((4 * num6) + 4)
                    Dim num19 As Integer = 0
                    Do While True
                        If (num19 >= num6) Then
                            num5 = (num5 + num6)
                            num4 = (num4 + num6)
                            Exit Do
                        End If
                        buffer((num4 + num19)) = inputBuffer((num5 + num19))
                        num19 += 1
                    Loop
                Loop
                num6 = (num2 - num5)
                buffer(num4) = CByte((252 + num6))
                num4 += 1
                Do While (num6 <> 0)
                    num6 -= 1
                    buffer(num4) = inputBuffer(num5)
                    num4 += 1
                    num5 += 1
                Loop
                If (num5 <> length) Then
                    Return Nothing
                End If
                Dim buffer2 As Byte() = New Byte(num4 - 1) {}
                Dim i As Integer
                For i = 0 To num4 - 1
                    buffer2(i) = buffer(i)
                Next i
                Return buffer2
            End If
            Dim num10 As Byte = inputBuffer(num2)
            Dim num11 As Byte = inputBuffer((num2 + 1))
            num12 = numArray2(num10, num11)
            numArray((num2 And 131071)) = num12
            numArray2(num10, num11) = num2
            If (num2 >= num5) Then
                num13 = 0
                num14 = 0
                num15 = 0
                Exit Do
            End If
            GoTo TR_0017
        Loop
        Do While True
            Do While True
                If ((num12 < 0) OrElse (((num2 - num12) >= 131072) OrElse (num15 >= 100))) Then
                    num15 += 1
                    GoTo TR_003D
                Else
                    num6 = 2
                    Dim num16 As Integer = (num3 + 2)
                    If (num16 < length) Then
                        Dim num17 As Integer = (num12 + 2)
                        Dim flag As Boolean = False
                        Do While True
                            If ((inputBuffer(num16) = inputBuffer(num17)) AndAlso (num6 < 1028)) Then
                                num16 += 1
                                num17 += 1
                                num6 += 1
                                If (num16 <> length) Then
                                    Continue Do
                                End If
                                flag = True
                            End If
                            If (num6 > num13) Then
                                num13 = num6
                                num14 = (num2 - num12)
                            End If
                            If (flag OrElse (num6 = 1028)) Then
                                Exit Do
                            End If
                            num12 = numArray((num12 And 131071))
                        Loop
                    End If
                    GoTo TR_003D
                End If
            Loop
        Loop
    End Function

    Private Function Compress_10FB_Block(ByVal inputStream As Stream, ByVal outputStream As Stream, ByVal blockUncompressedSize As Integer) As Integer
        If (blockUncompressedSize = 0) Then
            Return 0
        End If
        Dim buffer As Byte() = New Byte(blockUncompressedSize - 1) {}
        inputStream.Read(buffer, 0, blockUncompressedSize)
        Dim buffer2 As Byte() = Me.Compress_10FB(buffer)
        outputStream.Write(buffer2, 0, buffer2.Length)
        Return buffer2.Length
    End Function

    Private Shared Sub CopyStream(ByVal inputStream As Stream, ByVal outputStream As ZOutputStream, ByVal size As Integer)
        Dim offset As Integer = 0
        Dim buffer As Byte() = New Byte((size + offset) - 1) {}
        inputStream.Read(buffer, offset, size)
        outputStream.Write(buffer, 0, (size + offset))
        outputStream.finish()
        outputStream.Flush()
    End Sub

    Private Shared Sub CopyStream2(ByVal inputStream As Stream, ByVal outputStream As ZOutputStream, ByVal size As Integer)
        Dim offset As Integer = 2
        Dim buffer As Byte() = New Byte((size + offset) - 1) {}
        buffer(0) = 120
        buffer(1) = 218
        inputStream.Read(buffer, offset, size)
        outputStream.Write(buffer, 0, (size + offset))
        outputStream.Flush()
    End Sub


    Public Function Decompress() As Boolean
        If (Me.m_CurrentCompression = ECompressionMode.None) Then
            Return False
        End If
        Me.m_ReadMemoryStream = New MemoryStream(Me.m_UncompressedSize)
        Dim flag1 As Boolean = Me.Decompress(Me.m_ReadMemoryStream)
        If flag1 Then
            Me.m_CurrentCompression = ECompressionMode.None
            Me.m_IsArchived = False
            Me.m_IsInMemory = True
            Me.m_StartPosition = 0
        End If
        Return flag1
    End Function

    Public Function Decompress(ByVal outputStream As Stream) As Boolean
        If ((Me.m_CurrentCompression = ECompressionMode.None) OrElse (Me.m_CurrentCompression = ECompressionMode.Unknown)) Then
            outputStream.Write(Me.Read, 0, Me.m_CompressedSize)
            Return False
        End If
        If Me.m_IsArchived Then
            Me.m_Archive.Decompress()
        End If
        Select Case Me.m_CurrentCompression
            Case ECompressionMode.Compressed_10FB
                Me.Uncompress_10FB(outputStream)
            Case ECompressionMode.Chunkzip
                Me.UnChunkzip(outputStream)
            Case ECompressionMode.Chunkzip2
                Me.UncompressBms(outputStream)
                'Me.UnChunkZip2(outputStream)       'gives errors sometimes, but may be fixed ?
            Case ECompressionMode.Chunkref
                Me.UnChunkref(outputStream)
            Case ECompressionMode.Chunkref2
                Me.UnChunkref2(outputStream)
            Case ECompressionMode.EASF
                Me.UnEASF(outputStream)
                'Case ECompressionMode.Chunklzma    'doing with BMS method
                'Me.UnChunklzma(outputStream)
            Case ECompressionMode.Chunklzma, ECompressionMode.chunklzx, ECompressionMode.chunklz4, ECompressionMode.chunkunc, ECompressionMode.chunzstd, ECompressionMode.chunoodl  'https://github.com/Crauzer/OodleSharp  '"oodle compression"
                Me.UncompressBms(outputStream)

            Case Else
                Exit Select
        End Select
        Return True
    End Function

    Private Function DeflateBlock(ByVal inputStream As Stream, ByVal outputStream As Stream, ByVal blockUncompressedSize As Integer) As Integer
        Dim stream As New ZOutputStream(outputStream, -1)
        Dim position As Long = outputStream.Position
        Try
            FifaFile.CopyStream(inputStream, stream, blockUncompressedSize)
        Finally

        End Try
        Return CInt((outputStream.Position - position))
    End Function

    Public Function Export(ByVal exportDir As String) As Boolean
        Return Me.Export(exportDir, True)
    End Function

    Public Function Export(ByVal exportDir As String, ByVal decompressionAllowed As Boolean) As Boolean
        If Me.m_Name.StartsWith("C:") Then
            Return False
        End If
        Dim path As String = (exportDir & "\" & Me.m_Name)
        Dim directoryName As String = IO.Path.GetDirectoryName(path)
        IO.Path.GetExtension(path)
        If Not Directory.Exists(directoryName) Then
            Directory.CreateDirectory(directoryName)
        End If
        Dim output As New FileStream(path, FileMode.Create, FileAccess.Write)
        If (Me.IsCompressed And decompressionAllowed) Then
            Me.Decompress()
        End If
        Dim r As FileReader = Me.GetReader
        Dim count As Integer = If(Me.IsCompressed, Me.m_CompressedSize, Me.m_UncompressedSize)
        Dim writer1 As New FileWriter(output)
        writer1.Write(r.ReadBytes(count))
        writer1.Close()
        output.Close()
        Me.ReleaseReader(r)
        Return True
    End Function


    Public Function GetReader() As FileReader
        Dim reader As FileReader = Nothing
        If Not Me.m_IsInMemory Then
            If File.Exists(Me.m_PhysicalName) Then
                reader = New FileReader(New FileStream(Me.m_PhysicalName, FileMode.Open, FileAccess.Read))
                reader.BaseStream.Position = Me.m_StartPosition
                If (Not Me.m_Archive Is Nothing) Then
                    Dim baseStream As Stream = reader.BaseStream
                    baseStream.Position = (baseStream.Position + Me.m_Archive.StartPosition)
                End If
            End If
        ElseIf (Not Me.m_ReadMemoryStream Is Nothing) Then
            reader = New FileReader(Me.m_ReadMemoryStream)
            reader.BaseStream.Position = Me.m_StartPosition
        Else
            If (Me.m_Archive Is Nothing) Then
                Return Nothing
            End If
            reader = Me.m_Archive.GetReader
            reader.BaseStream.Position = Me.m_StartPosition
        End If
        Return reader
    End Function

    Public Function GetStreamReader() As StreamReader
        If (Me.m_ReadMemoryStream Is Nothing) Then
            Dim reader As StreamReader = If((Me.m_Archive Is Nothing), New StreamReader(Me.m_PhysicalName), Me.m_Archive.GetStreamReader)
            Dim baseStream As Stream = reader.BaseStream
            baseStream.Position = (baseStream.Position + Me.m_StartPosition)
            If Not Me.IsCompressed Then
                Return reader
            End If
            Me.m_ReadMemoryStream = New MemoryStream
            Me.Decompress(Me.m_ReadMemoryStream)
            Me.m_ReadMemoryStream.Seek(0, SeekOrigin.Begin)
            Me.m_StartPosition = 0
            Me.m_IsInMemory = True
        End If
        Return New StreamReader(Me.m_ReadMemoryStream)
    End Function

    Protected Function GetStreamWriter(ByVal compressionMode As ECompressionMode) As StreamWriter
        Me.m_WriteMemoryStream = New MemoryStream
        Me.m_IsInMemory = True
        Me.m_IsArchived = False
        Me.m_CurrentCompression = compressionMode
        Return New StreamWriter(Me.m_WriteMemoryStream)
    End Function

    Public Function GetWriter() As FileWriter
        If ((Me.m_Archive Is Nothing) AndAlso (Not Me.m_PhysicalName Is Nothing)) Then
            Me.m_IsInMemory = False
            Me.m_IsArchived = False
            Return New FileWriter(New FileStream((Me.m_PhysicalName & ".temp"), FileMode.Create))
        End If
        Me.m_WriteMemoryStream = New MemoryStream
        Me.m_IsInMemory = True
        Me.m_IsArchived = False
        Return New FileWriter(Me.m_WriteMemoryStream)
    End Function

    Protected Function GetWriter(ByVal size As Integer, ByVal compressionMode As ECompressionMode) As FileWriter
        Me.m_WriteMemoryStream = New MemoryStream(size)
        Me.m_IsInMemory = True
        Me.m_IsArchived = False
        Me.m_CurrentCompression = compressionMode
        If (Me.m_CurrentCompression <> ECompressionMode.None) Then
            Me.m_CompressedSize = size
            Me.m_UncompressedSize = -1
            Return New FileWriter(Me.m_WriteMemoryStream)
        End If
        Me.m_UncompressedSize = size
        Me.m_CompressedSize = -1
        Return New FileWriter(Me.m_WriteMemoryStream)
    End Function

    Private Sub InflateBlock(ByVal inputStream As Stream, ByVal outputStream As Stream, ByVal blockCompressedSize As Integer)
        Dim stream As New ZOutputStream(outputStream)
        Try
            FifaFile.CopyStream(inputStream, stream, blockCompressedSize)
        Finally
        End Try
    End Sub

    Private Sub InflateBlock2(ByVal inputStream As Stream, ByVal outputStream As Stream, ByVal blockCompressedSize As Integer)
        Dim stream As New ZOutputStream(outputStream)
        Try
            FifaFile.CopyStream2(inputStream, stream, blockCompressedSize)
        Finally
        End Try
    End Sub

    Public Function IsDds() As Boolean
        If (Path.GetExtension(Me.m_Name) = String.Empty) Then
            Dim r As FileReader = Me.GetReader
            If (Me.CompressedSize >= 16) Then
                Dim buffer As Byte() = r.ReadBytes(16)
                If (((buffer(6) = 68) AndAlso ((buffer(7) = 68) AndAlso (buffer(8) = 83))) OrElse ((buffer(0) = 68) AndAlso ((buffer(1) = 68) AndAlso (buffer(2) = 83)))) Then
                    Return True
                End If
            End If
            Me.ReleaseReader(r)
        End If
        Return False
    End Function

    Private Sub Load(ByVal fifaFile As FifaFile)
        Me.m_Name = fifaFile.Name
        Me.m_Archive = fifaFile.Archive
        Me.m_IsArchived = fifaFile.IsArchived
        Me.m_IsInMemory = fifaFile.IsInMemory
        Me.m_PhysicalName = fifaFile.PhysicalName
        Me.m_StartPosition = fifaFile.StartPosition
        Me.m_IsAnArchive = True
        Me.m_RequiredCompression = fifaFile.m_RequiredCompression
        Me.m_CurrentCompression = fifaFile.m_CurrentCompression
        Me.m_CompressedSize = fifaFile.CompressedSize
        Me.m_UncompressedSize = fifaFile.UncompressedSize
        Me.m_ReadMemoryStream = fifaFile.m_ReadMemoryStream
        Me.m_WriteMemoryStream = fifaFile.m_WriteMemoryStream
    End Sub

    Private Sub Load(ByVal path As String, ByVal isAnArchive As Boolean)
        Me.m_Archive = Nothing
        Me.m_IsArchived = False
        Me.m_IsInMemory = False
        Me.m_PhysicalName = path
        Me.m_StartPosition = 0
        Me.m_Name = IO.Path.GetFileName(path)
        Me.m_IsAnArchive = isAnArchive
        Dim r As FileReader = Me.GetReader
        If (Not r Is Nothing) Then
            Dim length As Integer = CInt(r.BaseStream.Length)
            Me.m_CompressedSize = length
            Me.m_UncompressedSize = length
            Me.m_CurrentCompression = ECompressionMode.None
            Me.m_RequiredCompression = ECompressionMode.None
            Me.CheckCompressionMode(r)
            Me.ReleaseReader(r)
        End If
    End Sub

    Private Sub Load(ByVal archive As FifaBigFile, ByVal buffer As Byte(), ByVal name As String, ByVal compressionMode As ECompressionMode)
        Me.m_Archive = archive
        Me.m_IsArchived = False
        Me.m_IsInMemory = True
        Me.m_ReadMemoryStream = New MemoryStream(buffer)
        Me.m_PhysicalName = Nothing
        Me.m_StartPosition = 0
        Me.m_Name = name
        Dim extension As String = Path.GetExtension(name)
        extension.ToLower()

        If (extension = ".big") Then
            Me.m_IsAnArchive = True
        Else
            Me.m_IsAnArchive = False
        End If
        Me.m_RequiredCompression = compressionMode
        Me.m_CurrentCompression = ECompressionMode.None
        Me.m_UncompressedSize = buffer.Length
        If Me.IsToCompress Then
            Me.m_CompressedSize = -1
        Else
            Me.m_CompressedSize = Me.m_UncompressedSize
        End If
    End Sub


    Private Sub Load(ByVal archive As FifaBigFile, ByVal startPosition As UInt32, ByVal size As Integer, ByVal name As String, ByVal isAnArchive As Boolean, ByVal r As FileReader)
        Me.m_Name = name
        Me.m_Archive = archive
        Me.m_IsArchived = True
        Me.m_IsInMemory = archive.IsInMemory
        Me.m_PhysicalName = Me.m_Archive.PhysicalName
        Me.m_StartPosition = startPosition
        Me.m_IsAnArchive = isAnArchive
        Me.m_CompressedSize = size
        Me.m_UncompressedSize = size
        Me.m_CurrentCompression = ECompressionMode.Unknown
        Me.m_RequiredCompression = ECompressionMode.Unknown
        If (size = 0) Then
            Me.m_CurrentCompression = ECompressionMode.None
            Me.m_RequiredCompression = ECompressionMode.None
        ElseIf (r Is Nothing) Then
            r = Me.m_Archive.GetReader
            Dim baseStream As Stream = r.BaseStream
            baseStream.Position = (baseStream.Position + startPosition)
            Me.CheckCompressionMode(r)
            Me.ReleaseReader(r)
        Else
            If (Not Me.m_Archive Is Nothing) Then
                r.BaseStream.Position = (Me.m_Archive.StartPosition + startPosition)
            Else
                r.BaseStream.Position = startPosition
            End If
            Me.CheckCompressionMode(r)
        End If
    End Sub


    Private Function Read() As Byte()
        Dim r As FileReader = Me.GetReader
        Dim bytes As Byte() = r.ReadBytes(If(Me.IsCompressed, Me.m_CompressedSize, Me.m_UncompressedSize))
        Me.ReleaseReader(r)

        Return bytes
    End Function

    Public Sub ReleaseReader(ByVal r As FileReader)
        If ((Not r Is Nothing) AndAlso Not Me.m_IsInMemory) Then
            r.BaseStream.Close()
            r.Close()
        End If
    End Sub

    Public Sub ReleaseStreamReader(ByVal r As StreamReader)
        If Not Me.m_IsInMemory Then
            r.BaseStream.Close()
            r.Close()
        End If
    End Sub

    Protected Function ReleaseStreamWriter(ByVal w As StreamWriter) As Boolean
        w.Flush()
        Me.m_CompressedSize = CInt(Me.m_WriteMemoryStream.Length)
        Me.m_UncompressedSize = CInt(Me.m_WriteMemoryStream.Length)
        If Not Me.m_IsInMemory Then
            w.Close()

            If File.Exists((Me.PhysicalName & ".tmp")) Then
                File.Delete(Me.PhysicalName)
                File.Move((Me.PhysicalName & ".tmp"), Me.PhysicalName)
            End If
        Else
            If Not Me.IsCompressed Then
                If Me.IsToCompress Then
                    Dim buffer As Byte() = Me.Compress_10FB(Me.m_WriteMemoryStream.GetBuffer)
                    If (buffer Is Nothing) Then
                        Me.m_CurrentCompression = ECompressionMode.None
                        Me.m_RequiredCompression = ECompressionMode.None
                        Me.m_CompressedSize = Me.m_UncompressedSize
                        Return False
                    End If
                    Me.m_RequiredCompression = ECompressionMode.Compressed_10FB
                    Me.m_CompressedSize = buffer.Length
                End If
            ElseIf Not Me.IsToCompress Then
                Dim buffer2 As Byte() = FifaFile.Uncompress_10FB(Me.m_WriteMemoryStream.GetBuffer)
                If (buffer2 Is Nothing) Then
                    Me.m_CurrentCompression = ECompressionMode.None
                    Me.m_RequiredCompression = ECompressionMode.None
                    Me.m_UncompressedSize = Me.m_CompressedSize
                    Return False
                End If
                Me.m_RequiredCompression = ECompressionMode.None
                Me.m_UncompressedSize = buffer2.Length
            End If
            Me.m_ReadMemoryStream = Me.m_WriteMemoryStream
            Me.m_StartPosition = 0
        End If
        Return True
    End Function

    Public Function ReleaseWriter(ByVal w As FileWriter) As Boolean
        If Not Me.m_IsInMemory Then
            w.Close()

            If ((File.GetAttributes(Me.PhysicalName) And FileAttributes.ReadOnly) = FileAttributes.ReadOnly) Then
                File.SetAttributes(Me.PhysicalName, FileAttributes.Archive)
            End If
            File.Delete(Me.m_PhysicalName)
            File.Move((Me.m_PhysicalName & ".temp"), Me.m_PhysicalName)
        Else
            If Not Me.IsCompressed Then
                Me.m_CompressedSize = -1
                Me.m_UncompressedSize = CInt(Me.m_WriteMemoryStream.Length)
            Else
                Me.m_UncompressedSize = -1
                Me.m_CompressedSize = CInt(Me.m_WriteMemoryStream.Length)
            End If
            Me.m_ReadMemoryStream = Me.m_WriteMemoryStream
            Me.m_StartPosition = 0
        End If
        Return True
    End Function

    Public Sub Rename(ByVal name As String)
        Me.m_Name = name
        If (Me.Archive Is Nothing) Then
            Dim destFileName As String = (Path.GetDirectoryName(Me.m_PhysicalName) & "\" & name)
            File.Move(Me.m_PhysicalName, destFileName)
            Me.m_PhysicalName = destFileName
        End If
    End Sub

    Public Sub Save(ByVal w As FileWriter)
        Dim baseStream As Stream = w.BaseStream
        Me.Save(baseStream)
    End Sub

    Public Sub Save(ByVal outputStream As Stream)
        Dim position As Long = outputStream.Position
        If (Me.IsToCompress AndAlso Not Me.IsCompressed) Then
            Me.Compress(outputStream)
        ElseIf (Not Me.IsToCompress AndAlso Me.IsCompressed) Then
            Me.Decompress(outputStream)
        Else
            Dim r As FileReader = Me.GetReader
            Dim count As Integer = If(Me.IsCompressed, Me.m_CompressedSize, Me.m_UncompressedSize)
            Dim num3 As Integer = 1048576
            Do While True
                If (count <= num3) Then
                    outputStream.Write(r.ReadBytes(count), 0, count)
                    Me.ReleaseReader(r)
                    Exit Do
                End If
                outputStream.Write(r.ReadBytes(num3), 0, num3)
                count = (count - num3)
            Loop
        End If
        If (Not Me.m_Archive Is Nothing) Then
            Me.m_PhysicalName = Me.m_Archive.PhysicalName
            Me.m_IsArchived = True
        End If
        Me.m_StartPosition = Convert.ToUInt32(position)
        Me.m_ReadMemoryStream = Nothing
        Me.m_WriteMemoryStream = Nothing
        Me.m_IsInMemory = False
    End Sub

    Public Function SetCompressionMode(ByVal compressionMode As ECompressionMode) As Boolean
        If (Me.m_CurrentCompression <> compressionMode) Then
            If ((Me.m_CurrentCompression <> ECompressionMode.None) AndAlso (compressionMode <> ECompressionMode.None)) Then
                Return False
            End If
            If ((Me.m_CurrentCompression <> ECompressionMode.None) OrElse (compressionMode = ECompressionMode.None)) Then
                Return False
            End If
            Me.m_RequiredCompression = compressionMode
            Me.m_CompressedSize = -1
        End If
        Return True
    End Function

    Public Overrides Function ToString() As String
        Return Me.m_Name
    End Function

    Private Function UnChunklzma(ByVal outputStream As Stream) As Boolean
        Me.Export(FifaEnvironment.ExportFolder, False)
        Dim path As String = (FifaEnvironment.ExportFolder & "\" & Me.Name)
        If Not File.Exists(path) Then
            Return False
        End If
        If (Not path Is Nothing) Then
            FifaFile.s_ProcessUnchunklzma.StartInfo.WorkingDirectory = FifaEnvironment.LaunchDir
            FifaFile.s_ProcessUnchunklzma.StartInfo.FileName = "un_chunlzma"
            FifaFile.s_ProcessUnchunklzma.StartInfo.CreateNoWindow = True
            FifaFile.s_ProcessUnchunklzma.StartInfo.UseShellExecute = False
            FifaFile.s_ProcessUnchunklzma.StartInfo.Arguments = path
            FifaFile.s_ProcessUnchunklzma.StartInfo.RedirectStandardOutput = False
            FifaFile.s_ProcessUnchunklzma.Start()
            FifaFile.s_ProcessUnchunklzma.WaitForExit()
        End If
        Dim str2 As String = ((IO.Path.GetDirectoryName(path) & "\" & IO.Path.GetFileNameWithoutExtension(path)) & "_decompressed" & IO.Path.GetExtension(path))
        If Not File.Exists(str2) Then
            Return False
        End If
        File.Delete(path)
        File.Move(str2, path)
        Dim length As Integer = CInt(New FileInfo(path).Length)
        Me.m_UncompressedSize = length
        Dim buffer As Byte() = New Byte(length - 1) {}
        Dim stream1 As New FileStream(path, FileMode.Open, FileAccess.Read)
        stream1.Read(buffer, 0, length)
        outputStream.Write(buffer, 0, length)
        stream1.Close()
        File.Delete(str2)
        Return True
    End Function

    Private Function UnChunkref(ByVal outputStream As Stream) As Boolean
        If (Me.m_CompressedSize = -1) Then
            Return False
        End If
        Dim r As FileReader = Me.GetReader
        If (New String(r.ReadChars(8)) <> "chunkref") Then
            Return False
        End If
        Me.m_UncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
        FifaUtil.SwapEndian(r.ReadInt32)
        Dim num As Integer = 20
        Do While True
            Dim count As Integer = FifaUtil.SwapEndian(r.ReadInt32)
            num = (num + (4 + count))
            Dim buffer As Byte() = FifaFile.Uncompress_10FB(r.ReadBytes(count))
            outputStream.Write(buffer, 0, buffer.Length)
            If (num >= Me.m_CompressedSize) Then
                Me.ReleaseReader(r)
                Return True
            End If
        Loop
    End Function

    Private Function UnChunkref2(ByVal outputStream As Stream) As Boolean
        If (Me.m_CompressedSize = -1) Then
            Return False
        End If
        Dim r As FileReader = Me.GetReader
        If (New String(r.ReadChars(8)) <> "chunkref") Then
            Return False
        End If
        r.ReadInt32()
        Me.m_UncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
        r.ReadInt32()
        r.ReadInt32()
        r.ReadInt32()
        r.ReadInt32()
        r.ReadInt32()
        r.ReadInt32()
        r.ReadInt32()
        Dim buffer As Byte() = FifaFile.Uncompress_10FB(r.ReadBytes(FifaUtil.SwapEndian(r.ReadInt32)))
        outputStream.Write(buffer, 0, buffer.Length)
        Me.ReleaseReader(r)
        Return True
    End Function

    Private Function UnChunkzip(ByVal outputStream As Stream) As Boolean
        If (Me.m_CompressedSize = -1) Then
            Return False
        End If
        Dim r As FileReader = Me.GetReader
        If (New String(r.ReadChars(8)) <> "chunkzip") Then
            Return False
        End If
        Me.m_UncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
        Me.m_MaxBlockUncompressedSize = FifaUtil.SwapEndian(r.ReadInt32)
        Dim num As Integer = 16
        Do While True
            Dim blockCompressedSize As Integer = FifaUtil.SwapEndian(r.ReadInt32)
            num = (num + (4 + blockCompressedSize))
            Me.InflateBlock(r.BaseStream, outputStream, blockCompressedSize)
            If (num >= Me.m_CompressedSize) Then
                Me.ReleaseReader(r)
                Return True
            End If
        Loop
    End Function

    Private Function UnChunkZip2(ByVal outputStream As Stream) As Boolean
        If Me.m_CompressedSize = -1 Then
            Return False
        End If
        Dim r As BinaryReader = Me.GetReader()
        If New String(r.ReadChars(8)) <> "chunkzip" Then
            Return False
        End If
        r.ReadInt32()
        Me.m_UncompressedSize = FifaUtil.SwapEndian(r.ReadInt32())
        Me.m_MaxBlockUncompressedSize = FifaUtil.SwapEndian(r.ReadInt32())
        FifaUtil.SwapEndian(r.ReadInt32())
        FifaUtil.SwapEndian(r.ReadInt32())
        r.ReadInt32()
        r.ReadInt32()
        r.ReadInt32()
        Dim v As Integer = 40
        Do
            v += 8
            Dim count As Integer = FifaUtil.RoundUp(v, 16) - v
            If count > 0 Then
                r.ReadBytes(count)
                v += count
            End If
            If v < Me.m_CompressedSize Then
                Dim blockCompressedSize As Integer = FifaUtil.SwapEndian(r.ReadInt32())
                r.ReadInt32()
                v += blockCompressedSize
                Me.InflateBlock2(r.BaseStream, outputStream, blockCompressedSize)
            End If
        Loop While v < Me.m_CompressedSize
        Me.ReleaseReader(r)
        Return True
    End Function


    Private Function Uncompress_10FB(ByVal outputStream As Stream) As Boolean
        If (Me.m_CompressedSize = -1) Then
            Return False
        End If
        Dim buffer As Byte() = FifaFile.Uncompress_10FB(Me.Read)
        If (buffer Is Nothing) Then
            Return False
        End If
        outputStream.Write(buffer, 0, Me.m_UncompressedSize)
        Me.m_UncompressedSize = buffer.Length
        Return True
    End Function

    Private Shared Function Uncompress_10FB(ByVal inputBuffer As Byte()) As Byte()
        Dim num3 As Integer = 0
        Dim num4 As Integer = 0
        Dim num9 As Integer = 0
        Dim length As Integer = inputBuffer.Length
        If (length < 8) Then
            num9 = 1
            Return Nothing
        End If
        Dim num6 As Integer = (((CInt(inputBuffer(2)) << 16) + (CInt(inputBuffer(3)) << 8)) + CInt(inputBuffer(4)))
        If (num6 > (length * 128)) Then
            num9 = 2
            Return Nothing
        End If
        Dim buffer As Byte() = New Byte(num6 - 1) {}
        Dim index As Integer = If(((inputBuffer(0) And 1) <> 1), 5, 8)
        Dim num8 As Integer = 0
        Do While ((index < length) AndAlso (inputBuffer(index) < 252))
            Dim num2 As Integer
            Dim num As Integer = inputBuffer(index)
            index += 1
            If ((num And 128) = 0) Then
                If ((index + 1) >= length) Then
                    num9 = 3
                    Exit Do
                End If
                num2 = (num And 3)
                num3 = (((num And 28) >> 2) + 3)
                num4 = ((((num >> 5) << 8) + inputBuffer(index)) + 1)
                index += 1
            ElseIf ((num And 64) = 0) Then
                If ((index + 2) >= length) Then
                    num9 = 4
                    Exit Do
                End If
                Dim num1 As Byte = inputBuffer(index)
                index += 1
                num2 = ((num1 >> 6) And 3)
                num3 = ((num And 63) + 4)
                num4 = ((((num1 And 63) * 256) + inputBuffer(index)) + 1)
                index += 1
            ElseIf ((num And 32) <> 0) Then
                num2 = (((num And 31) * 4) + 4)
                num3 = 0
            Else
                If ((index + 3) >= length) Then
                    num9 = 5
                    Exit Do
                End If
                num2 = (num And 3)
                Dim num10 As Byte = inputBuffer(index)
                index += 1
                num4 = (((((num And 16) << 12) + (256 * num10)) + inputBuffer(index)) + 1)
                index += 1
                num3 = (((((num >> 2) And 3) * 256) + inputBuffer(index)) + 5)
                index += 1
            End If
            If ((index + num2) >= length) Then
                num9 = 6
            ElseIf (((num8 + num2) + num3) > num6) Then
                num9 = 7
            Else
                If (((num8 + num2) - num4) >= 0) Then
                    Dim num11 As Integer = 0
                    Do While True
                        If (num11 >= num2) Then
                            Dim i As Integer
                            For i = 0 To num3 - 1
                                buffer(num8) = buffer((num8 - num4))
                                num8 += 1
                            Next i
                            Exit Do
                        End If
                        buffer(num8) = inputBuffer(index)
                        num8 += 1
                        index += 1
                        num11 += 1
                    Loop
                    Continue Do
                End If
                num9 = 8
            End If
            Exit Do
        Loop
        If ((index < length) AndAlso (num8 < num6)) Then
            Dim num13 As Integer = (inputBuffer(index) And 3)
            If ((index + num13) >= length) Then
                num9 = 9
                num13 = 0
            End If
            If ((num8 + num13) > num6) Then
                num9 = 10
                num13 = 0
            End If
            Dim i As Integer
            For i = 0 To num13 - 1
                index += 1
                buffer(num8) = inputBuffer(index)
                num8 += 1
            Next i
        End If
        Return If(((num9 <> 0) OrElse (num8 <> num6)), Nothing, buffer)
    End Function

    Private Function Uncompress_10FB_Block(ByVal inputStream As Stream, ByVal outputStream As Stream, ByVal blockCompressedSize As Integer) As Integer
        If (blockCompressedSize = 0) Then
            Return 0
        End If
        Dim buffer As Byte() = New Byte(blockCompressedSize - 1) {}
        inputStream.Read(buffer, 0, blockCompressedSize)
        Dim buffer2 As Byte() = FifaFile.Uncompress_10FB(buffer)
        outputStream.Write(buffer2, 0, buffer2.Length)
        Return buffer2.Length
    End Function

    Private Function UnEASF(ByVal outputStream As Stream) As Boolean
        Me.Export(FifaEnvironment.ExportFolder, False)
        Dim path As String = (FifaEnvironment.ExportFolder & "\" & Me.Name).Replace("/", "\")
        If Not File.Exists(path) Then
            Return False
        End If
        If (Not path Is Nothing) Then
            FifaFile.s_ProcessUnEASF.StartInfo.WorkingDirectory = FifaEnvironment.LaunchDir
            FifaFile.s_ProcessUnEASF.StartInfo.FileName = "fifa16_decryptor"
            FifaFile.s_ProcessUnEASF.StartInfo.CreateNoWindow = True
            FifaFile.s_ProcessUnEASF.StartInfo.UseShellExecute = False
            FifaFile.s_ProcessUnEASF.StartInfo.Arguments = ("""" & path & """")
            FifaFile.s_ProcessUnEASF.StartInfo.RedirectStandardOutput = False
            FifaFile.s_ProcessUnEASF.Start()
            FifaFile.s_ProcessUnEASF.WaitForExit()
        End If
        Dim str2 As String = ((IO.Path.GetDirectoryName(path) & "\" & IO.Path.GetFileNameWithoutExtension(path)) & "_decrypted" & IO.Path.GetExtension(path))
        If Not File.Exists(str2) Then
            Return False
        End If
        File.Delete(path)
        File.Move(str2, path)
        Dim length As Integer = CInt(New FileInfo(path).Length)
        Dim buffer As Byte() = New Byte(length - 1) {}
        Dim stream1 As New FileStream(path, FileMode.Open, FileAccess.Read)
        stream1.Read(buffer, 0, length)
        outputStream.Write(buffer, 0, length)
        stream1.Close()
        Return True
    End Function

    Private Function UncompressBms(ByVal outputStream As Stream) As Boolean     ''Chunkzip2, Chunkref2, chunklzx, chunlzma, chunklz4, chunkunc, chunzstd, chunoodl
        Me.Export(FifaEnvironment.ExportFolder, False)
        Dim path As String = (FifaEnvironment.ExportFolder & "\" & Me.Name)
        Dim BmsScript As String = FifaEnvironment.LaunchDir & "\BMS\scripts\chunklzx.bms"
        Dim OutputFolder As String = IO.Path.GetDirectoryName(path)

        If (Not Directory.Exists(OutputFolder)) Then
            Directory.CreateDirectory(OutputFolder)
        End If
        If Not File.Exists(path) Then
            Return False
        End If
        Dim str2 As String = ((IO.Path.GetDirectoryName(path) & "\" & IO.Path.GetFileNameWithoutExtension(path)) & "_decrypted" & IO.Path.GetExtension(path))
        If File.Exists(str2) Then
            File.Delete(str2)
        End If
        If (Not path Is Nothing) Then
            FifaFile.s_ProcessUnBMS.StartInfo.WorkingDirectory = FifaEnvironment.LaunchDir & "\BMS"
            FifaFile.s_ProcessUnBMS.StartInfo.FileName = "quickbms"
            FifaFile.s_ProcessUnBMS.StartInfo.CreateNoWindow = True
            FifaFile.s_ProcessUnBMS.StartInfo.UseShellExecute = True
            FifaFile.s_ProcessUnBMS.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            FifaFile.s_ProcessUnBMS.StartInfo.Arguments = CStr("""" & BmsScript & """ """ & path & """ """ & OutputFolder & """") '"""" & BmsScript & """ """ & path & """ """ & OutputFolder & """" 'path
            FifaFile.s_ProcessUnBMS.StartInfo.RedirectStandardOutput = False
            FifaFile.s_ProcessUnBMS.Start()
            FifaFile.s_ProcessUnBMS.WaitForExit()
        End If
        If Not File.Exists(str2) Then
            Return False
        End If
        File.Delete(path)
        'File.Move(str2, path)
        Dim length As Integer = CInt(New FileInfo(str2).Length)
        Me.m_UncompressedSize = length
        Dim buffer As Byte() = New Byte(length - 1) {}
        Dim stream1 As New FileStream(str2, FileMode.Open, FileAccess.Read)
        stream1.Read(buffer, 0, length)
        outputStream.Write(buffer, 0, length)
        stream1.Close()

        Return True
    End Function

    ' Properties
    Public Property Name As String
        Get
            Return Me.m_Name
        End Get
        Set(ByVal value As String)
            Me.m_Name = value
        End Set
    End Property

    Public Property PhysicalName As String
        Get
            Return Me.m_PhysicalName
        End Get
        Set(ByVal value As String)
            Me.m_PhysicalName = value
        End Set
    End Property

    Public Property StartPosition As UInt32
        Get
            Return Me.m_StartPosition
        End Get
        Set(ByVal value As UInt32)
            Me.m_StartPosition = value
        End Set
    End Property

    Public ReadOnly Property CompressedSize As Integer
        Get
            Return Me.m_CompressedSize
        End Get
    End Property

    Public ReadOnly Property UncompressedSize As Integer
        Get
            Return Me.m_UncompressedSize
        End Get
    End Property

    Public ReadOnly Property BlockInflatedSize As Integer
        Get
            Return Me.m_MaxBlockUncompressedSize
        End Get
    End Property

    Public Property CompressionMode As ECompressionMode
        Get
            Return Me.m_RequiredCompression
        End Get
        Set(ByVal value As ECompressionMode)
            Me.m_RequiredCompression = value
        End Set
    End Property

    Public ReadOnly Property IsToCompress As Boolean
        Get
            Return (Me.m_RequiredCompression <> ECompressionMode.None)
        End Get
    End Property

    Public ReadOnly Property IsCompressed As Boolean
        Get
            Return (Me.m_CurrentCompression <> ECompressionMode.None)
        End Get
    End Property

    Public ReadOnly Property IsArchived As Boolean
        Get
            Return Me.m_IsArchived
        End Get
    End Property

    Public ReadOnly Property Archive As FifaBigFile
        Get
            Return Me.m_Archive
        End Get
    End Property

    Public ReadOnly Property IsInMemory As Boolean
        Get
            Return Me.m_IsInMemory
        End Get
    End Property

    Public ReadOnly Property IsAnArchive As Boolean
        Get
            Return Me.m_IsAnArchive
        End Get
    End Property


    ' Fields
    Private Shared s_ProcessUnchunklzma As Process = New Process
    Private Shared s_ProcessUnEASF As Process = New Process
    Private Shared s_ProcessUnBMS As Process = New Process
    Private m_Name As String
    Private m_PhysicalName As String
    Private m_StartPosition As UInt32
    Private m_ReadMemoryStream As MemoryStream
    Private m_WriteMemoryStream As MemoryStream
    Private m_CompressedSize As Integer
    Private m_UncompressedSize As Integer
    Private m_MaxBlockUncompressedSize As Integer
    Private m_RequiredCompression As ECompressionMode
    Private m_CurrentCompression As ECompressionMode
    Private m_IsArchived As Boolean
    Private m_Archive As FifaBigFile
    Private m_IsInMemory As Boolean
    Private m_IsAnArchive As Boolean
End Class
'End Namespace
