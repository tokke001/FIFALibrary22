Imports System
Imports System.Drawing
Imports System.IO
Imports System.Text

Namespace KUtility
    Public Class DDSImage
        ' Methods
        Public Sub New(ByVal rawdata As Byte())
            Using stream As MemoryStream = New MemoryStream(rawdata)
                Using reader As BinaryReader = New BinaryReader(stream)
                    Me.dwMagic = reader.ReadInt32
                    If (Me.dwMagic <> &H20534444) Then
                        Throw New Exception("This is not a DDS!")
                    End If
                    Me.Read_DDS_HEADER(Me.header, reader)
                    If (((Me.header.ddspf.dwFlags And 4) <> 0) AndAlso (Me.header.ddspf.dwFourCC = &H30315844)) Then
                        Throw New Exception("DX10 not supported yet!")
                    End If
                    Dim dwMipMapCount As Integer = 1
                    If ((Me.header.dwFlags And &H20000) <> 0) Then
                        dwMipMapCount = Me.header.dwMipMapCount
                    End If
                    Me.images = New Bitmap(dwMipMapCount  - 1) {}
                    Dim i As Integer
                    For i = 0 To dwMipMapCount - 1
                        Dim w As Integer = CInt((CDbl(Me.header.dwWidth) / Math.Pow(2, CDbl(i))))
                        Dim h As Integer = CInt((CDbl(Me.header.dwHeight) / Math.Pow(2, CDbl(i))))
                        Dim count As Integer = ((w * h) * 4)
                        If ((Me.header.ddspf.dwFlags And &H40) <> 0) Then
                            Me.bdata = reader.ReadBytes(count)
                            Me.images(i) = Me.readLinearImage(Me.bdata, w, h)
                        ElseIf ((Me.header.ddspf.dwFlags And 4) <> 0) Then
                            Me.bdata = reader.ReadBytes(Me.header.dwPitchOrLinearSize)
                            Me.images(i) = Me.readBlockImage(Me.bdata, w, h)
                        ElseIf ((((((Me.header.ddspf.dwFlags And 4) = 0) AndAlso (Me.header.ddspf.dwRGBBitCount = &H10)) AndAlso ((Me.header.ddspf.dwRBitMask = &HFF) AndAlso (Me.header.ddspf.dwGBitMask = &HFF00))) AndAlso (Me.header.ddspf.dwBBitMask = 0)) AndAlso (Me.header.ddspf.dwABitMask = 0)) Then
                            Me.bdata = reader.ReadBytes(Me.header.dwPitchOrLinearSize)
                            Me.images(i) = Me.UncompressV8U8(Me.bdata, w, h)
                        End If
                    Next i
                End Using
            End Using
        End Sub

        Public Shared Function ByteArrayToString(ByVal ba As Byte()) As String
            Dim builder As New StringBuilder((ba.Length * 2))
            Dim num As Byte
            For Each num In ba
                builder.AppendFormat("{0:x2}", num)
            Next
            Return builder.ToString
        End Function

        Private Sub DecompressBlockDXT1(ByVal x As Integer, ByVal y As Integer, ByVal blockStorage As Byte(), ByVal image As Bitmap)
            Dim num As UInt16 = CUShort((blockStorage(0) Or (blockStorage(1) << 8)))
            Dim num2 As UInt16 = CUShort((blockStorage(2) Or (blockStorage(3) << 8)))
            Dim num3 As Integer = (((num >> 11) * &HFF) + &H10)
            Dim red As Byte = CByte((((num3 \ &H20) + num3) \ &H20))
            num3 = ((((num And &H7E0) >> 5) * &HFF) + &H20)
            Dim green As Byte = CByte((((num3 \ &H40) + num3) \ &H40))
            num3 = (((num And &H1F) * &HFF) + &H10)
            Dim blue As Byte = CByte((((num3 \ &H20) + num3) \ &H20))
            num3 = (((num2 >> 11) * &HFF) + &H10)
            Dim num7 As Byte = CByte((((num3 \ &H20) + num3) \ &H20))
            num3 = ((((num2 And &H7E0) >> 5) * &HFF) + &H20)
            Dim num8 As Byte = CByte((((num3 \ &H40) + num3) \ &H40))
            num3 = (((num2 And &H1F) * &HFF) + &H10)
            Dim num9 As Byte = CByte((((num3 \ &H20) + num3) \ &H20))
            'Dim num10 As UInt32 = DirectCast((((blockStorage(4) Or (blockStorage(5) << 8)) Or (blockStorage(6) << &H10)) Or (blockStorage(7) << &H18)), UInt32)
            Dim num10 As UInteger = CUInt(((blockStorage(4) Or (blockStorage(5) << 8)) Or (blockStorage(6) << 16)) Or (blockStorage(7) << 24))
            Dim i As Integer
            For i = 0 To 4 - 1
                Dim j As Integer
                For j = 0 To 4 - 1
                    Dim color As Color = Color.FromArgb(0)
                    Dim num13 As Byte = CByte(((num10 >> (2 * ((4 * i) + j))) And 3))
                    If (num > num2) Then
                        Select Case num13
                            Case 0
                                color = Color.FromArgb(&HFF, red, green, blue)
                                Exit Select
                            Case 1
                                color = Color.FromArgb(&HFF, num7, num8, num9)
                                Exit Select
                            Case 2
                                color = Color.FromArgb(&HFF, (((2 * red) + num7) \ 3), (((2 * green) + num8) \ 3), (((2 * blue) + num9) \ 3))
                                Exit Select
                            Case 3
                                color = Color.FromArgb(&HFF, ((red + (2 * num7)) \ 3), ((green + (2 * num8)) \ 3), ((blue + (2 * num9)) \ 3))
                                Exit Select
                        End Select
                    Else
                        Select Case num13
                            Case 0
                                color = Color.FromArgb(&HFF, red, green, blue)
                                Exit Select
                            Case 1
                                color = Color.FromArgb(&HFF, num7, num8, num9)
                                Exit Select
                            Case 2

                                Dim test_1 = &HFF
                                Dim test_2 = ((red + num7) / 2)
                                Dim test_3 = ((green + num8) / 2)
                                'Dim test_4 As Integer = CInt((blue) + (num9)) / 2I


                                color = Color.FromArgb(&HFFI, ((CInt(red) + CInt(num7)) \ 2I), ((CInt(green) + CInt(num8)) \ 2I), ((CInt(blue) + CInt(num9)) \ 2I))
                                Exit Select
                            Case 3
                                color = Color.FromArgb(&HFF, 0, 0, 0)
                                Exit Select
                        End Select
                    End If
                    image.SetPixel((x + j), (y + i), color)
                Next j
            Next i
        End Sub

        Private Sub DecompressBlockDXT5(ByVal x As Integer, ByVal y As Integer, ByVal blockStorage As Byte(), ByVal image As Bitmap)
            Dim num As Byte = blockStorage(0)
            Dim num2 As Byte = blockStorage(1)
            Dim index As Integer = 2
            Dim num4 As UInt32 = CUInt((((blockStorage((index + 2)) Or (blockStorage((index + 3)) << 8)) Or (blockStorage((index + 4)) << &H10)) Or (blockStorage((index + 5)) << &H18)))
            Dim num5 As UInt16 = CUShort((blockStorage(index) Or (blockStorage((index + 1)) << 8)))
            Dim num6 As UInt16 = CUShort((blockStorage(8) Or (blockStorage(9) << 8)))
            Dim num7 As UInt16 = CUShort((blockStorage(10) Or (blockStorage(11) << 8)))
            Dim num8 As Integer = (((num6 >> 11) * &HFF) + &H10)
            Dim red As Byte = CByte((((num8 \ &H20) + num8) \ &H20))
            num8 = ((((num6 And &H7E0) >> 5) * &HFF) + &H20)
            Dim green As Byte = CByte((((num8 \ &H40) + num8) \ &H40))
            num8 = (((num6 And &H1F) * &HFF) + &H10)
            Dim blue As Byte = CByte((((num8 \ &H20) + num8) \ &H20))
            num8 = (((num7 >> 11) * &HFF) + &H10)
            Dim num12 As Byte = CByte((((num8 \ &H20) + num8) \ &H20))
            num8 = ((((num7 And &H7E0) >> 5) * &HFF) + &H20)
            Dim num13 As Byte = CByte((((num8 \ &H40) + num8) \ &H40))
            num8 = (((num7 And &H1F) * &HFF) + &H10)
            Dim num14 As Byte = CByte((((num8 \ &H20) + num8) \ &H20))
            Dim num15 As UInt32 = CUInt((((blockStorage(12) Or (blockStorage(13) << 8)) Or (blockStorage(14) << &H10)) Or (blockStorage(15) << &H18)))
            Dim i As Integer
            For i = 0 To 4 - 1
                Dim j As Integer
                For j = 0 To 4 - 1
                    Dim num19 As Integer
                    Dim num20 As Byte
                    Dim num18 As Integer = (3 * ((4 * i) + j))
                    If (num18 <= 12) Then
                        num19 = ((num5 >> num18) And 7)
                    ElseIf (num18 = 15) Then
                        num19 = ((num5 >> 15) Or CInt(((num4 << 1) And 6)))
                    Else
                        num19 = (CInt((num4 >> (num18 - &H10))) And 7)
                    End If
                    If (num19 = 0) Then
                        num20 = num
                    ElseIf (num19 = 1) Then
                        num20 = num2
                    ElseIf (num > num2) Then
                        num20 = CByte(((((8 - num19) * num) + ((num19 - 1) * num2)) \ 7))
                    ElseIf (num19 = 6) Then
                        num20 = 0
                    ElseIf (num19 = 7) Then
                        num20 = &HFF
                    Else
                        num20 = CByte(((((6 - num19) * num) + ((num19 - 1) * num2)) \ 5))
                    End If
                    Dim num21 As Byte = CByte(((num15 >> (2 * ((4 * i) + j))) And 3))
                    Dim color As New Color
                    Select Case num21
                        Case 0
                            color = Color.FromArgb(num20, red, green, blue)
                            Exit Select
                        Case 1
                            color = Color.FromArgb(num20, num12, num13, num14)
                            Exit Select
                        Case 2
                            color = Color.FromArgb(num20, (((2 * red) + num12) \ 3), (((2 * green) + num13) \ 3), (((2 * blue) + num14) \ 3))
                            Exit Select
                        Case 3
                            color = Color.FromArgb(num20, ((red + (2 * num12)) \ 3), ((green + (2 * num13)) \ 3), ((blue + (2 * num14)) \ 3))
                            Exit Select
                    End Select
                    image.SetPixel((x + j), (y + i), color)
                Next j
            Next i
        End Sub

        Private Sub Read_DDS_HEADER(ByVal h As DDS_HEADER, ByVal r As BinaryReader)
            h.dwSize = r.ReadInt32
            h.dwFlags = r.ReadInt32
            h.dwHeight = r.ReadInt32
            h.dwWidth = r.ReadInt32
            h.dwPitchOrLinearSize = r.ReadInt32
            h.dwDepth = r.ReadInt32
            h.dwMipMapCount = r.ReadInt32
            Dim i As Integer
            For i = 0 To 11 - 1
                h.dwReserved1(i) = r.ReadInt32
            Next i
            Me.Read_DDS_PIXELFORMAT(h.ddspf, r)
            h.dwCaps = r.ReadInt32
            h.dwCaps2 = r.ReadInt32
            h.dwCaps3 = r.ReadInt32
            h.dwCaps4 = r.ReadInt32
            h.dwReserved2 = r.ReadInt32
        End Sub

        Private Sub Read_DDS_PIXELFORMAT(ByVal p As DDS_PIXELFORMAT, ByVal r As BinaryReader)
            p.dwSize = r.ReadInt32
            p.dwFlags = r.ReadInt32
            p.dwFourCC = r.ReadInt32
            p.dwRGBBitCount = r.ReadInt32
            p.dwRBitMask = r.ReadInt32
            p.dwGBitMask = r.ReadInt32
            p.dwBBitMask = r.ReadInt32
            p.dwABitMask = r.ReadInt32
        End Sub

        Private Function readBlockImage(ByVal data As Byte(), ByVal w As Integer, ByVal h As Integer) As Bitmap
            Select Case Me.header.ddspf.dwFourCC
                Case &H31545844
                    Return Me.UncompressDXT1(data, w, h)
                Case &H35545844
                    Return Me.UncompressDXT5(data, w, h)
            End Select
            Throw New Exception($"0x{Me.header.ddspf.dwFourCC.ToString("X")} texture compression not implemented.")
        End Function

        Private Function readLinearImage(ByVal data As Byte(), ByVal w As Integer, ByVal h As Integer) As Bitmap
            Dim bitmap As New Bitmap(w, h)
            Using stream As MemoryStream = New MemoryStream(data)
                Using reader As BinaryReader = New BinaryReader(stream)
                    Dim i As Integer
                    For i = 0 To h - 1
                        Dim j As Integer
                        For j = 0 To w - 1
                            bitmap.SetPixel(j, i, Color.FromArgb(reader.ReadInt32))
                        Next j
                    Next i
                End Using
            End Using
            Return bitmap
        End Function

        Private Function UncompressDXT1(ByVal data As Byte(), ByVal w As Integer, ByVal h As Integer) As Bitmap
            Dim image As New Bitmap(If((w < 4), 4, w), If((h < 4), 4, h))
            Using stream As MemoryStream = New MemoryStream(data)
                Using reader As BinaryReader = New BinaryReader(stream)
                    Dim i As Integer = 0
                    Do While (i < h)
                        Dim j As Integer = 0
                        Do While (j < w)
                            Me.DecompressBlockDXT1(j, i, reader.ReadBytes(8), image)
                            j = (j + 4)
                        Loop
                        i = (i + 4)
                    Loop
                End Using
            End Using
            Return image
        End Function

        Private Function UncompressDXT5(ByVal data As Byte(), ByVal w As Integer, ByVal h As Integer) As Bitmap
            Dim image As New Bitmap(If((w < 4), 4, w), If((h < 4), 4, h))
            Using stream As MemoryStream = New MemoryStream(data)
                Using reader As BinaryReader = New BinaryReader(stream)
                    Dim i As Integer = 0
                    Do While (i < h)
                        Dim j As Integer = 0
                        Do While (j < w)
                            Me.DecompressBlockDXT5(j, i, reader.ReadBytes(&H10), image)
                            j = (j + 4)
                        Loop
                        i = (i + 4)
                    Loop
                End Using
            End Using
            Return image
        End Function

        Private Function UncompressV8U8(ByVal data As Byte(), ByVal w As Integer, ByVal h As Integer) As Bitmap
            Dim bitmap As New Bitmap(w, h)
            Using stream As MemoryStream = New MemoryStream(data)
                Using reader As BinaryReader = New BinaryReader(stream)
                    Dim i As Integer
                    For i = 0 To h - 1
                        Dim j As Integer
                        For j = 0 To w - 1
                            Dim num3 As SByte = reader.ReadSByte
                            Dim num4 As SByte = reader.ReadSByte
                            Dim blue As Byte = &HFF
                            bitmap.SetPixel(j, i, Color.FromArgb((&H7F - num3), (&H7F - num4), blue))
                        Next j
                    Next i
                End Using
            End Using
            Return bitmap
        End Function


        ' Fields
        Private Const DDPF_ALPHAPIXELS As Integer = 1
        Private Const DDPF_ALPHA As Integer = 2
        Private Const DDPF_FOURCC As Integer = 4
        Private Const DDPF_RGB As Integer = &H40
        Private Const DDPF_YUV As Integer = &H200
        Private Const DDPF_LUMINANCE As Integer = &H20000
        Private Const DDSD_MIPMAPCOUNT As Integer = &H20000
        Private Const FOURCC_DXT1 As Integer = &H31545844
        Private Const FOURCC_DX10 As Integer = &H30315844
        Private Const FOURCC_DXT5 As Integer = &H35545844
        Public dwMagic As Integer
        Private header As DDS_HEADER = New DDS_HEADER
        Private header10 As DDS_HEADER_DXT10 = Nothing
        Public bdata As Byte()
        Public bdata2 As Byte()
        Public images As Bitmap()
    End Class
End Namespace

