Imports System.Runtime.InteropServices
Imports System.Text

Public Class FifaUtil
    ' Methods
    Private Shared Function adler32(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt64 = 1
        Dim num2 As UInt64 = 0
        Dim i As Integer
        For i = 0 To length - 1
            num = ((num + str(i)) Mod CULng(&HFFF1))
            num2 = ((num2 + num) Mod CULng(&HFFF1))
        Next i
        Return Convert.ToUInt32(((num2 << &H10) Or num))
    End Function

    Private Shared Function APHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = 2863311530
        Dim i As Integer
        For i = 0 To length - 1
            num = (num Xor If(((i And 1) = 0), ((num << 7) Xor (str(i) * (num >> 3))), Not ((num << 11) + (str(i) Xor (num >> 5)))))
        Next i
        Return num
    End Function

    Private Shared Function BKDRHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = &H83
        Dim num2 As UInt32 = 0
        Dim i As Integer
        For i = 0 To length - 1
            num2 = ((num2 * num) + str(i))
        Next i
        Return num2
    End Function

    Private Shared Function BPHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = 0
        Dim i As Integer
        For i = 0 To length - 1
            num = ((num << 7) Xor str(i))
        Next i
        Return num
    End Function

    Public Shared Function CompareWildcardString(ByVal pattern As String, ByVal target As String) As Boolean
        Dim num As Integer = 0
        Dim num2 As Integer = 0
        Dim num3 As Integer = 0
        pattern = pattern.ToLower
        target = target.ToLower
        num2 = 0
        Do While ((num2 < target.Length) AndAlso (num < pattern.Length))
            Select Case pattern(num)
                Case "?"c
                    num += 1
                    Exit Select
                Case "\"c, "/"c
                    If (num3 = 0) Then
                        If (("\"c <> target(num2)) AndAlso ("/"c <> target(num2))) Then
                            Return False
                        End If
                        num += 1
                    ElseIf (("\"c = target(num2)) OrElse ("/"c = target(num2))) Then
                        num3 = 0
                        num += 1
                    End If
                    Exit Select
                Case "*"c
                    If (num = (pattern.Length - 1)) Then
                        Return True
                    End If
                    num += 1
                    num3 = 1
                    Exit Select
                Case Else
                    If (num3 = 0) Then
                        If (pattern(num) <> target(num2)) Then
                            Return False
                        End If
                        num += 1
                    ElseIf (pattern(num) = target(num2)) Then
                        num3 = 0
                        num += 1
                    End If
                    Exit Select
            End Select
            num2 += 1
        Loop
        Return ((num2 = target.Length) AndAlso (num = pattern.Length))
    End Function

    Public Shared Function ComputeAlignement(ByVal v As Integer) As Integer
        Dim num As Integer = 1
        If (v = 0) Then
            Return 1
        End If
        Dim i As Integer
        For i = 0 To &H1F - 1
            If ((v And num) <> 0) Then
                Return ((num + 1) \ 2)
            End If
            num = ((num * 2) + 1)
        Next i
        Return 0
    End Function

    Public Shared Function ComputeAlignementLong(ByVal v As Long) As Integer
        Dim num As Integer = 1
        If (v = 0) Then
            Return 1
        End If
        Dim i As Integer
        For i = 0 To &H3F - 1
            If ((v And num) <> 0) Then
                Return ((num + 1) \ 2)
            End If
            num = ((num * 2) + 1)
        Next i
        Return 0
    End Function

    Public Shared Function ComputeBhHash(ByVal name As String) As UInt64
        Dim num As UInt64 = &H1505
        Dim i As Integer
        For i = 0 To name.Length - 1
            Dim num2 As Integer = AscW(name(i))
            num = (((num << 5) + num) + CULng(num2))
        Next i
        Return num
    End Function

    Public Shared Function ComputeBhHash(ByVal name As Byte(), ByVal length As Integer) As UInt64
        Dim num As UInt64 = &H1505
        Dim i As Integer
        For i = 0 To length - 1
            Dim num2 As Integer = name(i)
            num = (((num << 5) + num) + CULng(num2))
        Next i
        Return num
    End Function

    Public Shared Function ComputeBitUsed(ByVal range As UInt32) As Integer
        If (range = 0) Then
            Return 1
        End If
        Dim i As Integer = &H20
        Do While (i > 0)
            Dim num2 As UInt32 = (Convert.ToUInt32(1) << (i - 1))
            If ((range And num2) <> 0) Then
                Return i
            End If
            i -= 1
        Loop
        Return 0
    End Function

    Public Shared Function ComputeBucket(ByVal hash As Integer, ByVal extension As String) As Integer
        Dim num As Integer
        extension.ToLower()

        If extension.Equals(".fsh") Then
            num = 0
        ElseIf extension.Equals(".jdi") Then
            num = &H20
        ElseIf extension.Equals(".ini") Then
            num = &H20
        ElseIf extension.Equals(".tvb") Then
            num = &H40
        ElseIf extension.Equals(".irr") Then
            num = &H40
        ElseIf extension.Equals(".loc") Then
            num = &H60
        ElseIf extension.Equals(".cs") Then
            num = &H60
        ElseIf extension.Equals(".shd") Then
            num = &H80
        ElseIf extension.Equals(".txt") Then
            num = &H80
        ElseIf extension.Equals(".dat") Then
            num = &H80
        ElseIf extension.Equals(".hud") Then
            num = &H80
        ElseIf extension.Equals(".ttf") Then
            num = &HC0
        ElseIf extension.Equals(".bin") Then
            num = &HC0
        ElseIf extension.Equals(".skn") Then
            num = &HC0
        ElseIf extension.Equals(".o") Then
            num = &HE0
        ElseIf extension.Equals(".big") Then
            num = &HE0
        ElseIf extension.Equals(".ebo") Then
            num = &HE0
        Else
            Return 0
        End If
        Return ((((&H21 * hash) + num) Mod &H100) And &HFF)
    End Function

    Public Shared Function ComputeCrcDb11(ByVal bytes As Byte()) As Integer
        Dim num2 As Integer = -1
        Dim i As UInt32
        For i = 0 To bytes.Length - 1
            Dim num As Integer = 7
            num2 = (num2 Xor (bytes(i) << &H18))
            Do
                If (num2 >= 0) Then
                    num2 = (num2 * 2)
                Else
                    num2 = (num2 * 2)
                    num2 = (num2 Xor &H4C11DB7)
                End If
                num -= 1
            Loop While (num >= 0)
        Next i
        Return num2
    End Function

    Public Shared Function ComputeCrcDb11(ByVal [text] As String) As Integer
        Return FifaUtil.ComputeCrcDb11(FifaUtil.ue.GetBytes([text]))
    End Function

    Public Shared Function ComputeHash(ByVal fileName As String) As Integer
        Dim num As Integer = &H47B8A2
        Dim chArray As Char() = fileName.ToCharArray(0, fileName.Length)
        Dim length As Integer = chArray.Length

        For index = 0 To length - 1
            num = num + AscW(chArray(index))
            num = (num * &H21)
        Next index
        Return num
    End Function

    Public Shared Function ComputeLanguageHash(ByVal name As String) As UInt32
        Dim bytes As Byte() = FifaUtil.ue.GetBytes(name)
        Return FifaUtil.EAHash(bytes, bytes.Length)
    End Function

    Public Shared Function ConvertBytesToString(ByVal bytes As Byte()) As String
        Return FifaUtil.ue.GetString(bytes)
    End Function

    '------test--------------------------------------------------------
    'source : https://stackoverflow.com/questions/34952297/ieee754-half-precision-16-bit-floating-point-data-in-vb-net
    Private Shared BaseTable As UShort() = GenerateBaseTable()
    Private Shared ShiftTable As SByte() = GenerateShiftTable()

    Private Shared Function GenerateBaseTable() As UShort()
        Dim intBaseTable = New UShort(511) {}
        For i = 0 To 255
            Dim e = CSByte(127 - i)
            If e > 24 Then
                ' Very small numbers map to zero
                intBaseTable(i Or &H0) = &H0
                intBaseTable(i Or &H100) = &H8000
            ElseIf e > 14 Then
                ' Small numbers map to denorms
                intBaseTable(i Or &H0) = CUShort(&H400 >> (18 + e))
                intBaseTable(i Or &H100) = CUShort((&H400 >> (18 + e)) Or &H8000)
            ElseIf e >= -15 Then
                ' Normal numbers just lose precision
                intBaseTable(i Or &H0) = CUShort((15 - e) << 10)
                intBaseTable(i Or &H100) = CUShort(((15 - e) << 10) Or &H8000)
            ElseIf e > -128 Then
                ' Large numbers map to Infinity
                intBaseTable(i Or &H0) = &H7C00
                intBaseTable(i Or &H100) = &HFC00
            Else
                ' Infinity and NaN's stay Infinity and NaN's
                intBaseTable(i Or &H0) = &H7C00
                intBaseTable(i Or &H100) = &HFC00
            End If
        Next

        Return intBaseTable
    End Function

    Private Shared Function GenerateShiftTable() As SByte()
        Dim intShiftTable = New SByte(511) {}
        For i = 0 To 255
            Dim e = CSByte(127 - i)
            If e > 24 Then
                ' Very small numbers map to zero
                intShiftTable(i Or &H0) = 24
                intShiftTable(i Or &H100) = 24
            ElseIf e > 14 Then
                ' Small numbers map to denorms
                intShiftTable(i Or &H0) = CSByte(e - 1)
                intShiftTable(i Or &H100) = CSByte(e - 1)
            ElseIf e >= -15 Then
                ' Normal numbers just lose precision
                intShiftTable(i Or &H0) = 13
                intShiftTable(i Or &H100) = 13
            ElseIf e > -128 Then
                ' Large numbers map to Infinity
                intShiftTable(i Or &H0) = 24
                intShiftTable(i Or &H100) = 24
            Else
                ' Infinity and NaN's stay Infinity and NaN's
                intShiftTable(i Or &H0) = 13
                intShiftTable(i Or &H100) = 13
            End If
        Next

        Return intShiftTable
    End Function

    Public Shared Function ConvertFloat16ToShort(f As Single) As UShort 'function SingleToHalf
        Dim value As UInteger = BitConverter.ToUInt32(BitConverter.GetBytes(f), 0)

        Dim result = CUShort(BaseTable((value >> 23) And &H1FF) + ((value And &H7FFFFF) >> ShiftTable(value >> 23)))
        Return result
    End Function

    '--------------------------
    ' Public Shared Function ConvertFloat16ToShort_old(ByVal f As Single) As UInt16
    'bug : -7,99844 becomes -4 at writing 

    'Dim num5 As UInt16
    'If (f = 0!) Then
    'Return 0
    'End If
    'Dim num As Integer = 1
    'If (f < 0!) Then
    '       f = -f
    '      num = -1
    'End If
    'Dim num2 As Single = (f * 32768.0!)
    'Dim num3 As Integer = 0
    'Do While (num2 >= 2)
    '       num2 = (num2 / 2.0!)
    '      num3 += 1
    'Loop
    'Dim num4 As UInt16 = 0
    'If (num < 0) Then
    '       num4 = 32768
    'End If
    'If (num3 > 31) Then
    '       num3 = 31
    'End If
    '   num4 = CUShort((num4 Or (num3 << 10)))
    'If (num2 >= 1.0!) Then
    '       num5 = Convert.ToUInt16(CDbl(((num2 - 1.0!) * 1024)))
    'Else
    '       num5 = 1
    'End If
    'Return CUShort((num4 Or num5))
    'End Function

    Public Shared Function ConvertFromDate(ByVal [date] As DateTime) As Integer
        Dim time As New DateTime(&H62E, 10, 14, 0, 0, 0)
        Dim span As TimeSpan = DirectCast(([date] - time), TimeSpan)
        Return span.Days
    End Function

    Public Shared Function ConvertStringToBytes(ByVal str As String) As Byte()
        Return FifaUtil.ue.GetBytes(str)
    End Function

    Public Shared Function ConvertToDate(ByVal gregorian As Integer) As DateTime
        Dim time As New DateTime(&H62E, 10, 14, 12, 0, 0)
        If (gregorian < 0) Then
            Return time
        End If
        Return time.AddDays(CDbl(gregorian))
    End Function

    Public Shared Function ConvertToFloat(ByVal float16Bit As Short) As Single
        Dim num As Integer = If(((float16Bit And &H8000) = 0), 1, -1)
        Dim num2 As Integer = ((float16Bit And &H7C00) >> 10)
        Dim num3 As Integer = ((float16Bit And &H3FF) + &H400)
        If ((num2 = 0) AndAlso (num3 = 0)) Then
            Return 0!
        End If
        If (num2 = &H1F) Then
            Return Single.NaN
        End If
        num2 = (num2 - 15)
        Dim num4 As Single = CSng(Math.Pow(2, CDbl(num2)))
        Return ((CSng((num * num3)) / 1024.0!) * num4)
    End Function

    Public Shared Function decompress(ByVal float16Bit As Short) As Single
        Dim s As Integer = Int((float16Bit >> 15) And &H1)    '# sign
        Dim e As Integer = Int((float16Bit >> 10) And &H1F)    '# exponent
        Dim f As Integer = Int(float16Bit And &H3FF)            '# fraction

        If e = 0 Then
            If f = 0 Then
                Return Int(s << 31)
            Else
                While Not (f And &H400)
                    f = f << 1
                    e -= 1
                End While
                e += 1
                f = f And Not &H400
                '# print(s,e,f)
            End If
        ElseIf e = 31 Then

            If f = 0 Then
                Return Int((s << 31) Or &H7F800000)
            Else
                Return Int((s << 31) Or &H7F800000 Or (f << 13))
            End If
        End If
        e = e + (127 - 15)
        f = f << 13
        Return Int((s << 31) Or (e << 23) Or f)
    End Function


    Private Shared Function DEKHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = Convert.ToUInt32(length)
        Dim i As Integer
        For i = 0 To length - 1
            num = (((num << 5) Xor (num >> &H1B)) Xor str(i))
        Next i
        Return num
    End Function

    Private Shared Function DJBHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = &H1505
        Dim i As Integer
        For i = 0 To length - 1
            num = (((num << 5) + num) + str(i))
        Next i
        Return num
    End Function

    Private Shared Function EAHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = 0
        Dim i As Integer
        For i = 0 To length - 1
            Dim index As UInt32 = str(i)
            index = (index And &HDF)
            index = (index Xor num)
            index = (index And &HFF)
            num = (num >> 8)
            num = (num Xor FifaUtil.c_LanguageHashtable(index))
        Next i
        Return (num Xor &H80000000)
    End Function

    Private Shared Function ELFHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = 0
        Dim num2 As UInt32 = 0
        Dim i As Integer
        For i = 0 To length - 1
            num = ((num << 4) + str(i))
            num2 = (num And &HF0000000)
            If (num2 <> 0) Then
                num = (num Xor (num2 >> &H18))
            End If
            num = (num And Not num2)
        Next i
        Return num
    End Function

    Private Shared Function fletcher32(ByVal str As Byte(), ByVal len As Integer) As UInt32
        Dim num As UInt32 = 0
        Dim num2 As UInt32 = 0
        Dim i As Integer
        For i = 0 To len - 1
            num = (num + str(i))
            If (num >= &HFFFF) Then
                num = (num - &HFFFF)
            End If
            num2 = (num2 + num)
            If (num2 >= &HFFFF) Then
                num2 = (num2 - &HFFFF)
            End If
        Next i
        Return ((num2 << &H10) Or num)
    End Function

    Public Shared Function FNVHash_2(ByVal str As String, ByVal length As Integer) As ULong
        Dim num As ULong = 2166136261

        For Each c In str


            num = (num * 16777619)
            num = (num Xor AscW(c))
        Next c


        'For i = 0 To length - 1
        'Dim num3 As Byte = str(i)
        'If ((num3 >= 65) AndAlso (num3 <= 90)) Then
        'num3 = CByte((num3 + 32))
        'End If

        'num = (num * 16777619)
        'num = (num Xor num3)
        'Next i
        Return num
    End Function

    Public Shared Function FNVHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = 0
        num = 2166136261
        Dim i As Integer
        For i = 0 To length - 1
            Dim num3 As Byte = str(i)
            If ((num3 >= 65) AndAlso (num3 <= 90)) Then
                num3 = CByte((num3 + 32))
            End If
            num = (num Xor num3)
            num = (num * 16777619)
        Next i
        Return num
    End Function

    Public Shared Function IsFileLocked(ByVal filePath As String) As Boolean
        Try
            Using File.Open(filePath, FileMode.Open)
            End Using
        Catch exception1 As IOException
            Dim num As Integer = (Marshal.GetHRForException(exception1) And &HFFFF)
            Return ((num = &H20) OrElse (num = &H21))
        End Try
        Return False
    End Function

    Private Shared Function jenkins_one_at_a_time_hash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num2 As UInt32 = 0
        Dim num As UInt32 = 0
        Do While (num2 < length)
            num = (num + str(num2))
            num = (num + (num << 10))
            num = (num Xor (num >> 6))
            num2 += 1
        Loop
        num = (num + (num << 3))
        num = (num Xor (num >> 11))
        Return (num + (num << 15))
    End Function

    Private Shared Function JSHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = &H4E67C6A7
        Dim i As Integer
        For i = 0 To length - 1
            num = (num Xor (((num << 5) + str(i)) + (num >> 2)))
        Next i
        Return num
    End Function

    Public Shared Function Limit(ByVal val As Integer, ByVal min As Integer, ByVal max As Integer) As Integer
        If (val < min) Then
            Return min
        End If
        If (val > max) Then
            Return max
        End If
        Return val
    End Function

    Private Shared Function MurmurHash2(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = Convert.ToUInt32(length)
        Dim index As Integer = 0
        Do While (length >= 4)
            Dim num3 As UInt32 = str(index)
            num3 = ((num3 * &H100) + str((index + 1)))
            num3 = ((num3 * &H100) + str((index + 2)))
            num3 = ((num3 * &H100) + str((index + 3)))
            num3 = (num3 * &H5BD1E995)
            num3 = (num3 Xor (num3 >> &H18))
            num3 = (num3 * &H5BD1E995)
            num = (num * &H5BD1E995)
            num = (num Xor num3)
            index = (index + 4)
            length = (length - 4)
        Loop
        If (length = 3) Then
            num = (num Xor Convert.ToUInt32((str((index + 2)) << &H10)))
        End If
        If (length >= 2) Then
            num = (num Xor Convert.ToUInt32((str((index + 1)) << 8)))
        End If
        num = (num Xor str(index))
        num = (num * &H5BD1E995)
        num = (num Xor (num >> 13))
        num = (num * &H5BD1E995)
        Return (num Xor (num >> 15))
    End Function

    Public Shared Function PadBlanks(ByVal s As String, ByVal len As Integer) As String
        If (len > s.Length) Then
            Dim num As Integer = (len - s.Length)
            Dim i As Integer
            For i = 0 To num - 1
                s = (" " & s)
            Next i
        End If
        Return s
    End Function

    Private Shared Function PJWHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = &H20
        Dim num2 As UInt32 = ((num * 3) \ 4)
        Dim num3 As UInt32 = (num \ 8)
        Dim num4 As UInt32 = (Convert.ToUInt32(-1) << (num - num3))
        Dim num5 As UInt32 = 0
        Dim num6 As UInt32 = 0
        Dim i As Integer
        For i = 0 To length - 1
            num5 = ((num5 << num3) + str(i))
            num6 = (num5 And num4)
            If (num6 <> 0) Then
                num5 = ((num5 Xor (num6 >> num2)) And Not num4)
            End If
        Next i
        Return num5
    End Function

    Public Shared Function ReadNullPaddedString(ByVal r As BinaryReader, ByVal length As Integer) As String
        Dim bytes As Byte() = r.ReadBytes(length)
        Dim index As Integer = 0
        Do While (index < length)
            If (bytes(index) = 0) Then
                Exit Do
            End If
            index += 1
        Loop
        If (index = 0) Then
            Return String.Empty
        End If
        Return FifaUtil.ue.GetString(bytes, 0, index)
    End Function

    Public Shared Function ReadNullTerminatedByteArray(ByVal r As BinaryReader, ByVal length As Integer) As String
        Dim chArray As Char() = New Char(length - 1) {}
        Dim num2 As Integer = 0
        Dim i As Integer
        For i = 0 To length - 1
            Dim num As Byte
            If (r.PeekChar = -1) Then
                num2 = i
                Exit For
            End If
            num = r.ReadByte()
            If ((num = 0) AndAlso (num2 = 0)) Then
                num2 = i
            End If
            chArray(i) = Convert.ToChar(num)
        Next i
        Return New String(chArray, 0, num2)
    End Function

    Public Shared Function ReadNullTerminatedString(ByVal r As BinaryReader) As String
        Dim num As Byte
        Dim chArray As Char() = New Char() {} '= New Char(256 - 1) {}
        Dim index As Integer = 0
        Do While (r.PeekChar <> 0)
            num = r.ReadByte
            ReDim Preserve chArray(index)
            chArray(index) = Convert.ToChar(num)
            index += 1
            'If (index = 256) Then
            'Return Nothing
            'End If
        Loop
        r.ReadByte()  'null terminator
        Return New String(chArray, 0, index)
    End Function

    Public Shared Function ReadNullTerminatedString(ByVal r As BinaryReader, ByVal padding As Integer) As String
        Dim text1 As String = FifaUtil.ReadNullTerminatedString(r)
        Dim num As Integer = ((text1.Length + 1) Mod padding)
        If (num <> 0) Then
            r.ReadBytes((padding - num))
        End If
        Return text1
    End Function

    Public Shared Function ReadString(ByVal r As FileReader, ByVal offset As Integer) As String
        Dim position As Long = r.BaseStream.Position
        r.BaseStream.Position = offset
        Dim count As Integer = r.ReadInt16
        r.BaseStream.Position = position
        Return FifaUtil.ue.GetString(r.ReadBytes(count))
    End Function

    Public Shared Function ReadString(ByVal r As FileReader, ByVal offset As Long, ByVal length As Integer) As String
        Dim position As Long = r.BaseStream.Position
        r.BaseStream.Position = offset
        r.BaseStream.Position = position
        Return FifaUtil.ue.GetString(r.ReadBytes(length))
    End Function

    Public Shared Function RoundUp(ByVal v As Integer, ByVal align As Integer) As Integer
        Return ((v + (align - 1)) And Not (align - 1))
    End Function

    Public Shared Function RoundUp(ByVal v As Long, ByVal align As Integer) As Long
        Return ((v + (align - 1)) And Not (align - 1))
    End Function

    Public Shared Function RoundUp4(ByVal v As Integer) As Integer
        Return ((v + 3) And -4)
    End Function

    Private Shared Function RSHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = &H5C6B7
        Dim num2 As UInt32 = &HF8C9
        Dim num3 As UInt32 = 0
        Dim i As Integer
        For i = 0 To length - 1
            num3 = ((num3 * num2) + str(i))
            num2 = (num2 * num)
        Next i
        Return num3
    End Function

    Private Shared Function sdbm(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = 0
        Dim i As Integer
        For i = 0 To length - 1
            Dim num3 As UInt32 = (num << 6)
            num3 = (num3 + (num << &H10))
            num3 = (num3 - num)
            num = (str(i) + num3)
        Next i
        Return num
    End Function

    Private Shared Function SDBMHash(ByVal str As Byte(), ByVal length As Integer) As UInt32
        Dim num As UInt32 = 0
        Dim i As Integer
        For i = 0 To length - 1
            num = (((str(i) + (num << 6)) + (num << &H10)) - num)
        Next i
        Return num
    End Function

    Public Shared Function StringSize(ByVal s As String) As Integer
        Return FifaUtil.RoundUp4((CShort(FifaUtil.ue.GetByteCount(s)) + 2))
    End Function

    'Public Shared Function SwapAndConvertToFloat(ByVal r As FileReader) As Single
    '    Dim buffer As Byte() = New Byte(4 - 1) {}
    '    buffer(3) = r.ReadByte
    '    buffer(2) = r.ReadByte
    '    buffer(1) = r.ReadByte
    '    buffer(0) = r.ReadByte
    '    Dim input As New MemoryStream(buffer)
    '    'input.Close
    '    Return New FileReader(input).ReadSingle
    'End Function

    'Public Shared Sub SwapAndWriteFloat(ByVal w As BinaryWriter, ByVal f As Single)
    '    Dim buffer As Byte() = New Byte(4 - 1) {}
    '    Dim output As New MemoryStream(buffer)
    '    Dim w2 As New BinaryWriter(output)
    '    w2.Write(f)

    '    output.Close()
    '    w.Write(buffer(3))
    '    w.Write(buffer(2))
    '    w.Write(buffer(1))
    '    w.Write(buffer(0))
    'End Sub

    'Public Shared Function SwapEndian(ByVal x As Short) As Short
    '    Dim x_bytes As Byte() = BitConverter.GetBytes(x)

    '    Array.Reverse(x_bytes)

    '    Return BitConverter.ToInt16(x_bytes, 0)

    '    'Dim num As Byte = CByte((x And &HFF))
    '    'Return CShort((CByte(((x And &HFF00) >> 8)) + (num * &H100)))
    'End Function

    Public Shared Function SwapEndian(ByVal x As Integer) As Integer
        'Dim x As Byte = CByte(y)
        Dim x_bytes As Byte() = BitConverter.GetBytes(x)

        Array.Reverse(x_bytes)

        Return BitConverter.ToInt32(x_bytes, 0)

    End Function

    Public Shared Function SwapEndian(ByVal x As Long) As Long

        Dim x_bytes As Byte() = BitConverter.GetBytes(x)

        Array.Reverse(x_bytes)

        Return BitConverter.ToInt64(x_bytes, 0)

        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'Return (((((((((((((((x And &HFF) * &H100) + (x And &HFF)) * &H100) + (x And &HFF)) * &H100) + (x And &HFF)) * &H100) + (x And &HFF)) * &H100) + (x And &HFF)) * &H100) + (x And &HFF)) * &H100) + (x And &HFF))
    End Function

    Public Shared Function SwapEndian(ByVal x As UInt16) As UInt16
        Dim x_bytes As Byte() = BitConverter.GetBytes(x)

        Array.Reverse(x_bytes)

        Return BitConverter.ToUInt16(x_bytes, 0)


        'Dim num As Byte = CByte((x And &HFF))
        'Return CUShort((CByte(((x And &HFF00) >> 8)) + (num * &H100)))
    End Function

    Public Shared Function SwapEndian(ByVal x As UInt32) As UInt32

        Dim x_bytes As Byte() = BitConverter.GetBytes(x)

        Array.Reverse(x_bytes)

        Return BitConverter.ToUInt32(x_bytes, 0)

        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'Return (((((((x And &HFF) * &H100) + (x And &HFF)) * &H100) + (x And &HFF)) * &H100) + (x And &HFF))
    End Function

    Public Shared Function SwapEndian(ByVal x As UInt64) As UInt64
        Dim x_bytes As Byte() = BitConverter.GetBytes(x)

        Array.Reverse(x_bytes)

        Return BitConverter.ToUInt64(x_bytes, 0)

        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'x = (x >> 8)
        'Return (((((((((((((((x And CULng(&HFF)) * CULng(&H100)) + (x And CULng(&HFF))) * CULng(&H100)) + (x And CULng(&HFF))) * CULng(&H100)) + (x And CULng(&HFF))) * CULng(&H100)) + (x And CULng(&HFF))) * CULng(&H100)) + (x And CULng(&HFF))) * CULng(&H100)) + (x And CULng(&HFF))) * CULng(&H100)) + (x And CULng(&HFF)))
    End Function

    Public Shared Function TryAllaCrc32(ByVal bytes As Byte(), ByVal expected As UInt32) As Boolean
        Dim length As Integer = bytes.Length
        Dim num3 As UInt32 = FifaUtil.SwapEndian(expected)
        Dim num2 As UInt32 = FifaUtil.sdbm(bytes, length)
        If ((num2 <> expected) AndAlso (num2 <> num3)) Then
            num2 = FifaUtil.RSHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.JSHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.PJWHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.ELFHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.BKDRHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.SDBMHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.DJBHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.DEKHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.BPHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            'num2 = FifaUtil.FNVHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.APHash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.adler32(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.fletcher32(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = FifaUtil.jenkins_one_at_a_time_hash(bytes, length)
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = Convert.ToUInt32(FifaUtil.ComputeBhHash(bytes, length))
            If ((num2 = expected) OrElse (num2 = num3)) Then
                Return True
            End If
            num2 = Convert.ToUInt32(FifaUtil.ComputeCrcDb11(bytes))
            If ((num2 <> expected) AndAlso (num2 <> num3)) Then
                Return False
            End If
        End If
        Return True
    End Function

    Public Shared Sub WriteNullPaddedString(ByVal w As BinaryWriter, ByVal str As String, ByVal length As Integer)
        If (str Is Nothing) Then
            str = String.Empty
        End If
        Dim bytes As Byte() = FifaUtil.ue.GetBytes(str)
        If (bytes.Length > length) Then
            w.Write(bytes, 0, length)
        Else
            w.Write(bytes)
            Dim i As Integer
            For i = bytes.Length To length - 1
                w.Write(CByte(0))
            Next i
        End If
    End Sub

    Public Shared Sub WriteNullTerminatedByteArray(ByVal w As BinaryWriter, ByVal s As String, ByVal nBytes As Integer)
        Dim chArray As Char() = s.ToCharArray(0, s.Length)
        Dim i As Integer
        For i = 0 To nBytes - 1
            If (i < chArray.Length) Then
                w.Write(CByte(AscW(chArray(i))))
            Else
                w.Write(CByte(0))
            End If
        Next i
    End Sub

    Public Shared Sub WriteNullTerminatedString(ByVal w As BinaryWriter, ByVal s As String)
        Dim chArray As Char() = s.ToCharArray(0, s.Length)
        Dim i As Integer
        For i = 0 To chArray.Length - 1
            w.Write(CByte(AscW(chArray(i))))
        Next i
        w.Write(CByte(0))
    End Sub

    Public Shared Function WriteString(ByVal w As FileWriter, ByVal offset As Integer, ByVal s As String) As Integer
        Dim position As Long = w.BaseStream.Position
        w.BaseStream.Position = offset
        If (s Is Nothing) Then
            s = " "
        End If
        Dim byteCount As Short = CShort(FifaUtil.ue.GetByteCount(s))
        Dim num3 As Integer = (byteCount + 2)
        w.Write(byteCount)
        w.Write(FifaUtil.ue.GetBytes(s))
        If ((num3 And 3) <> 0) Then
            Dim num4 As Integer = (4 - (num3 And 3))
            Dim i As Integer
            For i = 0 To num4 - 1
                w.Write(CByte(0))
            Next i
        End If
        w.BaseStream.Position = position
        Return CInt(w.BaseStream.Position)
    End Function

    Public Shared Function DEC3NtoFloats_v2(ByVal var32Bit As UInt32) As Single()
        'https://stackoverflow.com/questions/8935419/how-do-i-unpack-dec3n-udec3-format
        'https://www.khronos.org/registry/OpenGL/extensions/OES/OES_vertex_type_10_10_10_2.txt
        'https://www.gamedev.net/forums/topic/642334-float-normals-to-dec3n/

        Dim x_value As Single
        Dim y_value As Single
        Dim z_value As Single
        Dim w_value As Single

        'Dim value_test As Integer = GetValueFrom32bit(var32Bit, 0, 2)
        x_value = GetValueFrom32bit(var32Bit, 0, 10)
        y_value = GetValueFrom32bit(var32Bit, 10, 10)
        z_value = GetValueFrom32bit(var32Bit, 20, 10)
        w_value = GetValueFrom32bit(var32Bit, 30, 2)
        'x_value = GetValueFrom32bit(var32Bit, 22, 10)
        'y_value = GetValueFrom32bit(var32Bit, 12, 10)
        'z_value = GetValueFrom32bit(var32Bit, 2, 10)
        'w_value = GetValueFrom32bit(var32Bit, 0, 2)

        'If w_value <> 0 Then MsgBox("test")

        x_value = GetFloatFrom10Bit_v2(x_value) - 1 'GetFloatFrom10Bit(var32Bit >> 22)
        y_value = GetFloatFrom10Bit_v2(y_value) - 1 'GetFloatFrom10Bit(var32Bit >> 12)
        z_value = GetFloatFrom10Bit_v2(z_value) - 1 'GetFloatFrom10Bit(var32Bit >> 2)
        w_value = GetFloatFrom2Bit_v2(w_value)
        'x = ((2 * (var32Bit >> 22) + 1) And (1024 - 1)) / (1024 - 1)
        'y = ((2 * (var32Bit >> 12) + 1) And (1024 - 1)) / (1024 - 1)
        'z = ((2 * (var32Bit >> 2) + 1) And (1024 - 1)) / (1024 - 1)
        'w = ((2 * (var32Bit) + 1) And (4 - 1)) / (4 - 1)
        '(2c + 1)/(2^2 - 1)

        Return New Single() {x_value, y_value, z_value, w_value}
    End Function

    Public Shared Function GetFloatFrom10Bit_v2(ByVal var10Bit As UInt32) As Single
        Return var10Bit / 511 '((2 * var10Bit) + 1) / (1024 - 1)
    End Function
    Public Shared Function GetFloatFrom2Bit_v2(ByVal var2Bit As UInt32) As Single
        Return var2Bit '(2 * var2Bit + 1) / (4 - 1)
    End Function
    Public Shared Function FloatsToDEC3N_v2(ByVal X_Value As Single, ByVal Y_Value As Single, ByVal Z_Value As Single) As UInteger
        'Dim test As Integer = (PackFloatTo10Bit(X_Value) << 22) + (PackFloatTo10Bit(Y_Value) << 12) + (PackFloatTo10Bit(Z_Value) << 2)
        Dim ReturnValue As UInteger = 0

        ReturnValue = SetValueTo32bit(ReturnValue, PackFloatTo10Bit_v2(X_Value), 0, 10)  '((test) >> 22)
        ReturnValue = SetValueTo32bit(ReturnValue, PackFloatTo10Bit_v2(Y_Value), 10, 10)  '((test) >> 12)
        ReturnValue = SetValueTo32bit(ReturnValue, PackFloatTo10Bit_v2(Z_Value), 20, 10)  '((test) >> 2)
        'ReturnValue = SetValueTo32bit(ReturnValue, PackWFloatTo2Bit(1), 30, 2)

        'Dim x_bytes As Byte() = BitConverter.GetBytes(ReturnValue)

        'Array.Reverse(x_bytes)

        'ReturnValue = BitConverter.ToUInt32(x_bytes, 0)

        Return CInt(ReturnValue)  '(PackFloatTo10Bit(X_Value) << 22) + (PackFloatTo10Bit(Y_Value) << 12) + (PackFloatTo10Bit(Z_Value) << 2)
    End Function
    Public Shared Function PackFloatTo10Bit_v2(ByVal value As Single) As Integer
        'value = (CInt(2 * (var10Bit) + 1) And CInt(1024 - 1)) / (1024 - 1)

        'value = ((2 * var10Bit) + 1) / (1024 - 1)
        'value * (1024 - 1) = (2 * var10Bit) + 1
        '(value * (1024 - 1)) - 1 = (2 * var10Bit) 


        Return CInt(((CInt(value * (1024 - 1)) Or CInt(512)) - 1) / 2)   '= var10Bit


        'If value < 0.0F Then
        'Return CInt(Math.Round(value * 512.0F))     'normalized uintiger
        'End If
        'Return CInt(Math.Round(value * 511.0F))         'normalized intiger

    End Function
    Public Shared Function DEC3NtoFloats(ByVal var32Bit As Int32) As Single()
        'https://stackoverflow.com/questions/8935419/how-do-i-unpack-dec3n-udec3-format
        'https://www.khronos.org/registry/OpenGL/extensions/OES/OES_vertex_type_10_10_10_2.txt
        'https://www.gamedev.net/forums/topic/642334-float-normals-to-dec3n/

        Dim x_value As Single
        Dim y_value As Single
        Dim z_value As Single
        'Dim w As Single
        'Dim x_bytes As Byte() = BitConverter.GetBytes(var32Bit)

        'Array.Reverse(x_bytes)

        'var32Bit = BitConverter.ToUInt32(x_bytes, 0)

        x_value = (var32Bit And 1023) / 511
        y_value = (var32Bit >> 10 And 1023) / 511
        z_value = (var32Bit >> 20 And 1023) / 511

        'x = var32Bit And 1023 'GetFloatFrom10Bit(var32Bit)
        'y = var32Bit And 1023 'GetFloatFrom10Bit(var32Bit >> 10)
        'z = var32Bit And 1023 'GetFloatFrom10Bit(var32Bit >> 20)
        'x = ((2 * (var32Bit >> 22) + 1) And (1024 - 1)) / (1024 - 1)
        'y = ((2 * (var32Bit >> 12) + 1) And (1024 - 1)) / (1024 - 1)
        'z = ((2 * (var32Bit >> 2) + 1) And (1024 - 1)) / (1024 - 1)
        'w = ((2 * (var32Bit) + 1) And (4 - 1)) / (4 - 1)
        '(2c + 1)/(2^2 - 1)

        Return New Single() {x_value, y_value, z_value} ', w}
    End Function

    Public Shared Function DEC3NtoFloats_OLD(ByVal var32Bit As UInt32) As Single()
        'https://stackoverflow.com/questions/8935419/how-do-i-unpack-dec3n-udec3-format
        'https://www.khronos.org/registry/OpenGL/extensions/OES/OES_vertex_type_10_10_10_2.txt
        'https://www.gamedev.net/forums/topic/642334-float-normals-to-dec3n/

        Dim x As Single
        Dim y As Single
        Dim z As Single
        'Dim w As Single
        'Dim x_bytes As Byte() = BitConverter.GetBytes(var32Bit)

        'Array.Reverse(x_bytes)

        'var32Bit = BitConverter.ToUInt32(x_bytes, 0)

        'x = (var32Bit And 1023) / 1023
        'y = (var32Bit >> 10 And 1023) / 1023
        'z = (var32Bit >> 20 And 1023) / 1023

        x = GetFloatFrom10Bit(var32Bit >> 22)
        y = GetFloatFrom10Bit(var32Bit >> 12)
        z = GetFloatFrom10Bit(var32Bit >> 2)
        'x = ((2 * (var32Bit >> 22) + 1) And (1024 - 1)) / (1024 - 1)
        'y = ((2 * (var32Bit >> 12) + 1) And (1024 - 1)) / (1024 - 1)
        'z = ((2 * (var32Bit >> 2) + 1) And (1024 - 1)) / (1024 - 1)
        'w = ((2 * (var32Bit) + 1) And (4 - 1)) / (4 - 1)
        '(2c + 1)/(2^2 - 1)

        Return New Single() {x, y, z} ', w}
    End Function

    Public Shared Function GetFloatFrom10Bit(ByVal var10Bit As UInt32) As Single
        'Return (((var10Bit)) And 511) / (511)
        'If (((((var10Bit)) And (1024 - 1)) / (1024 - 1)) * 2) - 1 < 0 Then MsgBox(((2 * (var10Bit) + 1) And (1024 - 1)) / (1024 - 1))
        'Return var10Bit / 511
        'Return ((((var10Bit)) And (1024 - 1)) / (1024 - 1) * 2) - 1
        'Return ((2 * (var10Bit) + 1)) / (1024 - 1)
        'Return (var10Bit And 511) / 511
        Return (CInt(2 * (var10Bit) + 1) And CInt(1024 - 1)) / (1024 - 1)

    End Function

    Public Shared Function FloatsToDEC3N(ByVal X_Value As Single, ByVal Y_Value As Single, ByVal Z_Value As Single) As UInteger
        'Dim test As Integer = (PackFloatTo10Bit(X_Value) << 22) + (PackFloatTo10Bit(Y_Value) << 12) + (PackFloatTo10Bit(Z_Value) << 2)
        'Dim x As Single = ((test) >> 22)
        'Dim y As Single = ((test) >> 12)
        'Dim z As Single = ((test) >> 2)
        'Dim test As Integer = ((PackFloatTo10Bit(X_Value)) + (PackFloatTo10Bit(Y_Value) << 10) + (PackFloatTo10Bit(Z_Value) << 20))
        'Dim test_2 As Integer = CInt((PackFloatTo10Bit(X_Value)) Or CInt(PackFloatTo10Bit(Y_Value) << 10) Or CInt(PackFloatTo10Bit(Z_Value) << 20))
        'If test <> test_2 Then
        'MsgBox("test")
        'End If
        Return CInt(PackFloatTo10Bit(X_Value)) Or CInt((PackFloatTo10Bit(Y_Value) << 10)) Or CInt((PackFloatTo10Bit(Z_Value) << 20))
        'Return (PackFloatTo10Bit(X_Value)) + (PackFloatTo10Bit(Y_Value) << 10) + (PackFloatTo10Bit(Z_Value) << 20)
        'Return (PackFloatTo10Bit(X_Value + 1)) + (PackFloatTo10Bit(Y_Value + 1) << 10) + (PackFloatTo10Bit(Z_Value + 1) << 20)
    End Function

    Public Shared Function FloatsToDEC3N_OLD(ByVal X_Value As Single, ByVal Y_Value As Single, ByVal Z_Value As Single) As UInteger
        'Dim test As Integer = (PackFloatTo10Bit(X_Value) << 22) + (PackFloatTo10Bit(Y_Value) << 12) + (PackFloatTo10Bit(Z_Value) << 2)
        'Dim x As Single = ((test) >> 22)
        'Dim y As Single = ((test) >> 12)
        'Dim z As Single = ((test) >> 2)

        Return (PackFloatTo10Bit(X_Value) << 22) + (PackFloatTo10Bit(Y_Value) << 12) + (PackFloatTo10Bit(Z_Value) << 2)
    End Function

    Public Shared Function PackFloatTo10Bit(ByVal value As Single) As Integer
        If value < 0.0F Then
            Return CInt(Math.Round(value * 512.0F))     'normalized uintiger
        End If
        Return CInt(Math.Round(value * 511.0F))         'normalized intiger

    End Function

    Public Shared Function PackWFloatTo2Bit(ByVal value As Single) As Integer
        Return CInt(Math.Truncate(value * 3.0F))

    End Function

    Public Shared Sub WriteAlignment_16(ByVal w As BinaryWriter)
        WriteAlignment(w, 16)
    End Sub

    Public Shared Sub WriteAlignment(ByVal w As BinaryWriter, ByVal Alignment As UInteger)
        Dim position As Long = w.BaseStream.Position

        Do While position Mod Alignment <> 0
            w.Write(CByte(0))
            position = w.BaseStream.Position
        Loop

    End Sub
    ' Public Shared Sub WriteAlignment_OLD(ByVal w As BinaryWriter, ByVal Alignment As UInteger)
    'Dim position As Long = w.BaseStream.Position

    'Dim NumPaddings As Long = ((position + Alignment - 1) And Not (Alignment - 1)) - position

    'For i = 0 To NumPaddings - 1
    '       w.Write(CByte(0))
    'Next i

    'End Sub

    Public Shared Sub WriteValue(ByVal w As FileWriter, ByVal Value As UInt32, ByVal Offset As Long)
        Dim CurrentOffset As Long = w.BaseStream.Position

        w.BaseStream.Position = Offset
        w.Write(Value)

        w.BaseStream.Position = CurrentOffset

    End Sub

    Public Shared Function WriteSectionTotalSize(ByVal w As FileWriter, ByVal OffsetStart As Long, ByVal OffsetEnd As Long) As UInt32
        Dim TotalSize As Long = OffsetEnd - OffsetStart

        WriteValue(w, CUInt(TotalSize), OffsetStart)

        Return TotalSize
    End Function

    Public Shared Sub WriteNullPaddings(ByVal w As BinaryWriter, ByVal nBytes As Long)
        For i = 0 To nBytes - 1
            w.Write(CByte(0))
        Next

    End Sub

    Public Shared Function Read64SizedString(ByVal r As BinaryReader) As String
        Dim num As Byte
        Dim chArray As Char() = New Char() {} '= New Char(256 - 1) {}
        Dim index As Integer = 0
        Do While (r.PeekChar <> 0) And (index < 64)
            num = r.ReadByte
            ReDim Preserve chArray(index)
            chArray(index) = Convert.ToChar(num)
            index += 1

        Loop

        Dim NumPadding As Integer = 64 - chArray.Length
        r.ReadBytes(NumPadding)  'skip null values

        Return New String(chArray, 0, index)
    End Function

    Public Shared Sub Write64SizedString(ByVal w As BinaryWriter, ByVal s As String)
        Dim chArray As Char() = s.ToCharArray(0, s.Length)
        For i = 0 To chArray.Length - 1
            w.Write(CByte(AscW(chArray(i))))
        Next i

        Dim NumPadding As Integer = 64 - chArray.Length
        For j = 0 To NumPadding - 1
            w.Write(CByte(0))
        Next j
    End Sub

    Public Shared Function GetValueFrom32bit(Value As UInteger, Offset As Byte, Length As Byte) As UInteger
        'https://stackoverflow.com/questions/14401242/im-new-to-visual-basic-and-trying-to-understand-how-to-set-individual-bits-in-a
        'Dim x As Byte
        'x = &H65
        'Dim ba As New BitArray(BitConverter.GetBytes(Value))
        'Dim ba2 As New BitArray(Length)
        'For i = 0 To Length - 1
        'ba2.Set(i, ba.Get(Offset + i))
        'Next

        'Dim array(0) As Integer
        'ba2.CopyTo(array, 0)


        'Return array(0)

        '        ba2.

        'ba.CopyTo(ba2, Offset)
        'array.Copy(ba, Offset, ba2, 0, Length)

        ' Return BitConverter.ToUInt32(ba2)

        If Offset > 0 Then
            Value = Value >> Offset
        End If

        Value = Value And CUInt(((2 ^ (Length)) - 1))    ' 6   65536

        Return Value
    End Function

    Public Shared Function SetValueTo32bit(TargetValue As UInteger, Value As UInteger, Offset As Byte, Length As Byte) As UInteger

        '--clear old value
        Dim Mask As UInteger = 4294967295 - (((2 ^ Length) - 1) << Offset) ' if n is 3, mask results in 111111111110111
        TargetValue = TargetValue And Mask

        '--set new value
        Value = Value << Offset
        TargetValue = TargetValue Or Value

        Return TargetValue
    End Function

    ' Fields
    Private Shared c_LanguageHashtable As UInt32() = New UInt32() {0, 1996959894, 3993919788, 2567524794, 124634137, 1886057615, 3915621685, 2657392035, 249268274, 2044508324, 3772115230, 2547177864, 162941995, 2125561021, 3887607047, 2428444049, 498536548, 1789927666, 4089016648, 2227061214, 450548861, 1843258603, 4107580753, 2211677639, 325883990, 1684777152, 4251122042, 2321926636, 335633487, 1661365465, 4195302755, 2366115317, 997073096, 1281953886, 3579855332, 2724688242, 1006888145, 1258607687, 3524101629, 2768942443, 901097722, 1119000684, 3686517206, 2898065728, 853044451, 1172266101, 3705015759, 2882616665, 651767980, 1373503546, 3369554304, 3218104598, 565507253, 1454621731, 3485111705, 3099436303, 671266974, 1594198024, 3322730930, 2970347812, 795835527, 1483230225, 3244367275, 3060149565, 1994146192, 31158534, 2563907772, 4023717930, 1907459465, 112637215, 2680153253, 3904427059, 2013776290, 251722036, 2517215374, 3775830040, 2137656763, 141376813, 2439277719, 3865271297, 1802195444, 476864866, 2238001368, 4066508878, 1812370925, 453092731, 2181625025, 4111451223, 1706088902, 314042704, 2344532202, 4240017532, 1658658271, 366619977, 2362670323, 4224994405, 1303535960, 984961486, 2747007092, 3569037538, 1256170817, 1037604311, 2765210733, 3554079995, 1131014506, 879679996, 2909243462, 3663771856, 1141124467, 855842277, 2852801631, 3708648649, 1342533948, 654459306, 3188396048, 3373015174, 1466479909, 544179635, 3110523913, 3462522015, 1591671054, 702138776, 2966460450, 3352799412, 1504918807, 783551873, 3082640443, 3233442989, 3988292384, 2596254646, 62317068, 1957810842, 3939845945, 2647816111, 81470997, 1943803523, 3814918930, 2489596804, 225274430, 2053790376, 3826175755, 2466906013, 167816743, 2097651377, 4027552580, 2265490386, 503444072, 1762050814, 4150417245, 2154129355, 426522225, 1852507879, 4275313526, 2312317920, 282753626, 1742555852, 4189708143, 2394877945, 397917763, 1622183637, 3604390888, 2714866558, 953729732, 1340076626, 3518719985, 2797360999, 1068828381, 1219638859, 3624741850, 2936675148, 906185462, 1090812512, 3747672003, 2825379669, 829329135, 1181335161, 3412177804, 3160834842, 628085408, 1382605366, 3423369109, 3138078467, 570562233, 1426400815, 3317316542, 2998733608, 733239954, 1555261956, 3268935591, 3050360625, 752459403, 1541320221, 2607071920, 3965973030, 1969922972, 40735498, 2617837225, 3943577151, 1913087877, 83908371, 2512341634, 3803740692, 2075208622, 213261112, 2463272603, 3855990285, 2094854071, 198958881, 2262029012, 4057260610, 1759359992, 534414190, 2176718541, 4139329115, 1873836001, 414664567, 2282248934, 4279200368, 1711684554, 285281116, 2405801727, 4167216745, 1634467795, 376229701, 2685067896, 3608007406, 1308918612, 956543938, 2808555105, 3495958263, 1231636301, 1047427035, 2932959818, 3654703836, 1088359270, 936918000, 2847714899, 3736837829, 1202900863, 817233897, 3183342108, 3401237130, 1404277552, 615818150, 3134207493, 3453421203, 1423857449, 601450431, 3009837614, 3294710456, 1567103746, 711928724, 3020668471, 3272380065, 1510334235, 755167117}
    Private Shared ue As UTF8Encoding = New UTF8Encoding
End Class

