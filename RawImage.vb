Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class RawImage
    ' Methods
    'Public Sub New()

    'End Sub
    Public Sub New(ByVal width As UInteger, ByVal height As UInteger, ByVal TextureFormat As ETextureFormat, ByVal size As UInteger, ByVal SwapEndian_DxtBlock As Boolean, Optional Tiled360 As Boolean = False)
        Me.Width = width
        Me.Height = height
        Me.TextureFormat = TextureFormat
        Me.Size = size
        Me.m_SwapEndian_DxtBlock = SwapEndian_DxtBlock
        Me.m_Tiled360 = Tiled360
        Me.m_Bitmap = Nothing
    End Sub

    Private Sub CreateBitmap()
        Dim num2 As Integer

        Dim RawDataFixed As Byte() = Me.m_RawData.Clone
        If Me.m_Tiled360 Then
            ' Untile the data
            RawDataFixed = ConvertToLinearTexture(Me.m_RawData.Clone, Me.Width, Me.Height, Me.TextureFormat)
        End If
        If Me.m_SwapEndian_DxtBlock Then
            Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.TextureFormat)
            For x As Integer = 0 To RawDataFixed.Length - 1 Step SwapNumBytes
                Array.Reverse(RawDataFixed, x, SwapNumBytes)
            Next x
        End If

        If (Me.Width < 1) Then
            Me.Width = 1
        End If
        If (Me.Height < 1) Then
            Me.Height = 1
        End If
        Select Case Me.TextureFormat
            Case ETextureFormat.DXT1, ETextureFormat.DXT3, ETextureFormat.DXT5, ETextureFormat.ATI2, ETextureFormat.ATI1, ETextureFormat.BC6H_UF16
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format32bppArgb)
                Me.ReadDxtToBitmap(RawDataFixed)
                Return
            Case ETextureFormat.A8R8G8B8
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format32bppArgb)
                Dim rect As New Rectangle(0, 0, Me.m_Bitmap.Width, Me.m_Bitmap.Height)
                Dim bitmapdata As BitmapData = Me.m_Bitmap.LockBits(rect, ImageLockMode.WriteOnly, Me.m_Bitmap.PixelFormat)
                Dim destination As IntPtr = bitmapdata.Scan0
                Dim num As Integer = (Me.m_Bitmap.Width * Me.m_Bitmap.Height)
                Marshal.Copy(RawDataFixed, 0, destination, (num * 4))
                Me.m_Bitmap.UnlockBits(bitmapdata)
                Return
            Case ETextureFormat.R8G8B8
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format24bppRgb)
                Dim rect As New Rectangle(0, 0, Me.m_Bitmap.Width, Me.m_Bitmap.Height)
                Dim bitmapdata As BitmapData = Me.m_Bitmap.LockBits(rect, ImageLockMode.WriteOnly, Me.m_Bitmap.PixelFormat)
                Dim destination As IntPtr = bitmapdata.Scan0
                Dim num As Integer = (Me.m_Bitmap.Width * Me.m_Bitmap.Height)
                Marshal.Copy(RawDataFixed, 0, destination, (num * 3))
                Me.m_Bitmap.UnlockBits(bitmapdata)
                'Me.m_Bitmap = GraphicUtil.Get32bitBitmap(Me.m_Bitmap)  'convert from 24bbp to 32bbp
                Return
            Case ETextureFormat.A4R4G4B4
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format16bppGrayScale) '??
                Dim rect As New Rectangle(0, 0, Me.m_Bitmap.Width, Me.m_Bitmap.Height)
                Dim bitmapdata As BitmapData = Me.m_Bitmap.LockBits(rect, ImageLockMode.WriteOnly, Me.m_Bitmap.PixelFormat)
                Dim destination As IntPtr = bitmapdata.Scan0
                Dim num As Integer = (Me.m_Bitmap.Width * Me.m_Bitmap.Height)
                Marshal.Copy(RawDataFixed, 0, destination, (num * 2))
                Me.m_Bitmap.UnlockBits(bitmapdata)
                Return
            Case ETextureFormat.R5G6B5
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format16bppRgb565)
                Dim rect As New Rectangle(0, 0, Me.m_Bitmap.Width, Me.m_Bitmap.Height)
                Dim bitmapdata As BitmapData = Me.m_Bitmap.LockBits(rect, ImageLockMode.WriteOnly, Me.m_Bitmap.PixelFormat)
                Dim destination As IntPtr = bitmapdata.Scan0
                Dim num As Integer = (Me.m_Bitmap.Width * Me.m_Bitmap.Height)
                Marshal.Copy(RawDataFixed, 0, destination, (num * 2))
                Me.m_Bitmap.UnlockBits(bitmapdata)
                'Me.m_Bitmap = GraphicUtil.Get32bitBitmap(Me.m_Bitmap)  'convert  to 32bbp
                Return
            Case ETextureFormat.X1R5G5B5
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format16bppArgb1555)
                Dim rect As New Rectangle(0, 0, Me.m_Bitmap.Width, Me.m_Bitmap.Height)
                Dim bitmapdata As BitmapData = Me.m_Bitmap.LockBits(rect, ImageLockMode.WriteOnly, Me.m_Bitmap.PixelFormat)
                Dim destination As IntPtr = bitmapdata.Scan0
                Dim num As Integer = (Me.m_Bitmap.Width * Me.m_Bitmap.Height)
                Marshal.Copy(RawDataFixed, 0, destination, (num * 2))
                Me.m_Bitmap.UnlockBits(bitmapdata)
                'Me.m_Bitmap = GraphicUtil.Get32bitBitmap(Me.m_Bitmap)  'convert  to 32bbp
                Return
            Case ETextureFormat.GREY8
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format32bppArgb)
                num2 = 0
                Dim i As Integer
                For i = 0 To Me.m_Bitmap.Height - 1
                    Dim j As Integer
                    For j = 0 To Me.m_Bitmap.Width - 1
                        Dim num6 As Integer
                        Dim num7 As Integer
                        Dim alpha As Integer = 255
                        Dim blue As Integer = RawDataFixed(num2)
                        num7 = RawDataFixed(num2)
                        num6 = RawDataFixed(num2)
                        num2 += 1
                        Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, num6, num7, blue))
                    Next j
                Next i
                Return
            Case ETextureFormat.GREY8ALFA8
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format32bppArgb)
                num2 = 0
                Dim i As Integer
                For i = 0 To Me.m_Bitmap.Height - 1
                    Dim j As Integer
                    For j = 0 To Me.m_Bitmap.Width - 1
                        Dim num12 As Integer
                        Dim num13 As Integer
                        Dim alpha As Integer = RawDataFixed(num2)
                        num2 += 1
                        Dim blue As Integer = RawDataFixed(num2)
                        num13 = RawDataFixed(num2)
                        num12 = RawDataFixed(num2)
                        num2 += 1
                        Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, num12, num13, blue))
                    Next j
                Next i
                Return
            Case ETextureFormat.A32B32G32R32F
                Me.m_Bitmap = New Bitmap(Me.Width, Me.Height, PixelFormat.Format32bppArgb)
                num2 = 0
                For i = 0 To Me.m_Bitmap.Height - 1
                    For j = 0 To Me.m_Bitmap.Width - 1
                        Dim red As Byte() = New Byte(4 - 1) {}
                        Array.Copy(RawDataFixed, num2, red, 0, 4)
                        num2 += 4
                        Dim green As Byte() = New Byte(4 - 1) {}
                        Array.Copy(RawDataFixed, num2, green, 0, 4)
                        num2 += 4
                        Dim blue As Byte() = New Byte(4 - 1) {}
                        Array.Copy(RawDataFixed, num2, blue, 0, 4)
                        num2 += 4
                        Dim alpha As Byte() = New Byte(4 - 1) {}
                        Array.Copy(RawDataFixed, num2, alpha, 0, 4)
                        num2 += 4

                        Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(Math.Min(BitConverter.ToSingle(alpha, 0) * 255, 255), Math.Min(BitConverter.ToSingle(red, 0) * 255, 255), Math.Min(BitConverter.ToSingle(green, 0) * 255, 255), Math.Min(BitConverter.ToSingle(blue, 0) * 255, 255)))     'values too big (alpha value 2048 for example) --> need be devided?? https://stackoverflow.com/questions/12839758/why-is-this-128bit-color-format-being-converted-to-32bit
                    Next j
                Next i
                Return


            Case Else
                Return
        End Select
    End Sub

    Private Sub CreateRawData()
        If (Me.Width < 1) Then
            Me.Width = 1
        End If
        If (Me.Height < 1) Then
            Me.Height = 1
        End If
        Select Case Me.TextureFormat
            Case ETextureFormat.DXT1, ETextureFormat.DXT3, ETextureFormat.DXT5, ETextureFormat.ATI2, ETextureFormat.ATI1, ETextureFormat.BC6H_UF16
                Me.WriteBitmapToDxt()
                Return
            Case ETextureFormat.A8R8G8B8
                Me.WriteBitmapToA8R8G8B8()
                Return
            Case ETextureFormat.R8G8B8
                Me.WriteBitmapToR8G8B8()
                Return
                'Case ETextureFormat.A4R4G4B4
                'Me.WriteBitmapToA4R4G4B4()
                'Return
                'Case ETextureFormat.R5G6B5
                'Me.WriteBitmapToR5G6B5()
                'Return
                'Case ETextureFormat.X1R5G5B5
            Case ETextureFormat.GREY8
                Me.WriteBitmapToGrey8()
                Return
            Case ETextureFormat.GREY8ALFA8
                Me.WriteBitmapToGrey8Alfa8()
                Exit Select
                'Case (ETextureFormat.GREY8 Or ETextureFormat.DXT5)
                'Exit Select
            Case ETextureFormat.A32B32G32R32F
                Me.WriteBitmapToA32B32G32R32F()
                Exit Select
            Case Else
                Return
        End Select
    End Sub

    Public Function Load(ByVal r As FileReader) As Boolean
        'Dim size As Integer = Me.Size
        'If (Me.TextureFormat = ETextureFormat.A8R8G8B8) Then
        'Size = ((Me.Width * Me.Height) * 4)
        'End If
        Me.m_RawData = r.ReadBytes(Me.Size)

        Me.NeedToSaveRawData = False
        Return True
    End Function

    Private Sub ReadDxtToBitmap(ByVal RawDataFixed As Byte())
        'Dim RawData_2 As Byte() = RawData
        Dim block As New DxtBlock(CInt(Me.TextureFormat), Me.m_SwapEndian_DxtBlock)
        Dim input As New MemoryStream(RawDataFixed)
        Dim br As New FileReader(input, Endian.Little)
        Dim rect As New Rectangle(0, 0, Me.Width, Me.Height)
        Dim bitmapdata As BitmapData = Me.m_Bitmap.LockBits(rect, ImageLockMode.WriteOnly, Me.m_Bitmap.PixelFormat)
        Dim destination As IntPtr = bitmapdata.Scan0
        Dim length As Integer = (Me.m_Bitmap.Width * Me.m_Bitmap.Height)
        Dim source As Integer() = New Integer(length - 1) {}
        Dim i As Integer
        For i = 0 To (Me.m_Bitmap.Height \ 4) - 1
            Dim j As Integer
            For j = 0 To (Me.m_Bitmap.Width \ 4) - 1
                block.Load(br)
                Dim index As Integer = ((i * 4) * Me.m_Bitmap.Width)
                index = (index + (j * 4))
                source(index) = block.Colors(0, 0).ToArgb
                source((index + 1)) = block.Colors(1, 0).ToArgb
                source((index + 2)) = block.Colors(2, 0).ToArgb
                source((index + 3)) = block.Colors(3, 0).ToArgb
                index = (index + Me.m_Bitmap.Width)
                source(index) = block.Colors(0, 1).ToArgb
                source((index + 1)) = block.Colors(1, 1).ToArgb
                source((index + 2)) = block.Colors(2, 1).ToArgb
                source((index + 3)) = block.Colors(3, 1).ToArgb
                index = (index + Me.m_Bitmap.Width)
                source(index) = block.Colors(0, 2).ToArgb
                source((index + 1)) = block.Colors(1, 2).ToArgb
                source((index + 2)) = block.Colors(2, 2).ToArgb
                source((index + 3)) = block.Colors(3, 2).ToArgb
                index = (index + Me.m_Bitmap.Width)
                source(index) = block.Colors(0, 3).ToArgb
                source((index + 1)) = block.Colors(1, 3).ToArgb
                source((index + 2)) = block.Colors(2, 3).ToArgb
                source((index + 3)) = block.Colors(3, 3).ToArgb
            Next j
        Next i
        Marshal.Copy(source, 0, destination, length)
        Me.m_Bitmap.UnlockBits(bitmapdata)
        source = Nothing
        br.Close()
        input.Close()
    End Sub

    Public Function Save(ByVal SwapEndian_DxtBlock As Boolean, ByVal w As FileWriter) As Boolean

        If Me.NeedToSaveRawData Then    '1 - when new bitmap added, rawdata need be created
            'Dim size As Integer = Me.Size
            'If (Me.TextureFormat = ETextureFormat.A8R8G8B8) Then
            'size = ((Me.Width * Me.Height) * 4)
            'End If
            Me.m_RawData = New Byte(Me.Size - 1) {}

            Me.CreateRawData()  '2 - this is always little endian

            If SwapEndian_DxtBlock Then    '3 - if big endian needed -> swap !
                Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.TextureFormat)
                For x As Integer = 0 To Me.m_RawData.Length - 1 Step SwapNumBytes
                    Array.Reverse(Me.m_RawData, x, SwapNumBytes)
                Next x
            End If

            Me.NeedToSaveRawData = False
        Else    'when no rawdata update needed, but endian formats (source vs request) are different
            If Me.m_SwapEndian_DxtBlock <> SwapEndian_DxtBlock Then
                Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.TextureFormat)
                For x As Integer = 0 To Me.m_RawData.Length - 1 Step SwapNumBytes
                    Array.Reverse(Me.m_RawData, x, SwapNumBytes)
                Next x
            End If
        End If

        Me.m_SwapEndian_DxtBlock = SwapEndian_DxtBlock

        w.Write(Me.m_RawData)
        Return True
    End Function

    Private Sub WriteBitmapToA8R8G8B8()
        If (((Me.m_Bitmap.Height * Me.m_Bitmap.Width) * 4) <= Me.m_RawData.Length) Then
            Dim num As Integer = 0
            Dim i As Integer
            For i = 0 To Me.m_Bitmap.Height - 1
                Dim j As Integer
                For j = 0 To Me.m_Bitmap.Width - 1
                    Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)
                    Dim b As Byte = pixel.B
                    Dim g As Byte = pixel.G
                    Dim r As Byte = pixel.R
                    Dim a As Byte = pixel.A
                    Me.m_RawData(num) = b
                    num += 1
                    Me.m_RawData(num) = g
                    num += 1
                    Me.m_RawData(num) = r
                    num += 1
                    Me.m_RawData(num) = a
                    num += 1
                Next j
            Next i
        End If
    End Sub

    Private Sub WriteBitmapToR8G8B8()
        If (((Me.m_Bitmap.Height * Me.m_Bitmap.Width) * 3) <= Me.m_RawData.Length) Then
            Dim num As Integer = 0
            Dim i As Integer
            For i = 0 To Me.m_Bitmap.Height - 1
                Dim j As Integer
                For j = 0 To Me.m_Bitmap.Width - 1
                    Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)
                    Dim b As Byte = pixel.B
                    Dim g As Byte = pixel.G
                    Dim r As Byte = pixel.R
                    Me.m_RawData(num) = b
                    num += 1
                    Me.m_RawData(num) = g
                    num += 1
                    Me.m_RawData(num) = r
                    num += 1
                Next j
            Next i
        End If
    End Sub

    Private Sub WriteBitmapToA4R4G4B4() 'not tested https://forum.unity.com/threads/understanding-argb4444-bytes-format.459057/
        If (((Me.m_Bitmap.Height * Me.m_Bitmap.Width) * 2) <= Me.m_RawData.Length) Then
            Dim num As Integer = 0
            Dim i As Integer
            For i = 0 To Me.m_Bitmap.Height - 1
                Dim j As Integer
                For j = 0 To Me.m_Bitmap.Width - 1
                    Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)

                    Dim r4 As Integer = (pixel.R >> 4)
                    Dim g4 As Integer = (pixel.G >> 4)
                    Dim b4 As Integer = (pixel.B >> 4)
                    Dim a4 As Integer = (pixel.A >> 4)
                    Me.m_RawData(num) = CByte((g4 << 4) Or b4)
                    num += 1
                    Me.m_RawData(num) = CByte((a4 << 4) Or r4)
                    num += 1

                Next j
            Next i
        End If
    End Sub

    Private Sub WriteBitmapToR5G6B5() 'not tested   https://forum.unity.com/threads/understanding-argb4444-bytes-format.459057/
        If (((Me.m_Bitmap.Height * Me.m_Bitmap.Width) * 2) <= Me.m_RawData.Length) Then
            Dim num As Integer = 0
            Dim i As Integer
            For i = 0 To Me.m_Bitmap.Height - 1
                Dim j As Integer
                For j = 0 To Me.m_Bitmap.Width - 1
                    Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)

                    Dim r As Integer = pixel.R
                    Dim g As Integer = pixel.G
                    Dim b As Integer = pixel.B

                    'int rgb = (b << 10) | (g << 10) | (r << 5);
                    'int rgb = (r << 16) | (g << 8) | (b);
                    'int rgb = (r << 5) | (g << 6) | (b << 5);
                    'int rgb = r | (g << 8) | (b << 16);
                    'int rgb = (r << 11) | (g << 5) | b;
                    'int rgb = (r >> 5) | (g >> 6) | (b >> 5);
                    Dim rgb As Integer = (CByte(CByte(r) >> 3) << 11) Or (CByte(CByte(g) >> 2) << 5) Or (CByte(b) >> 3)

                    Me.m_RawData(num) = CByte(rgb)
                    num += 1
                    Me.m_RawData(num) = CByte(rgb >> 8)
                    num += 1

                Next j
            Next i
        End If

    End Sub

    Private Sub WriteBitmapToA32B32G32R32F()    '128-bit float format using 32 bits for the each channel (alpha, blue, green, red)
        If (((Me.m_Bitmap.Height * Me.m_Bitmap.Width) * 16) <= Me.m_RawData.Length) Then
            Dim num As Integer = 0
            Dim i As Integer
            For i = 0 To Me.m_Bitmap.Height - 1
                Dim j As Integer
                For j = 0 To Me.m_Bitmap.Width - 1
                    Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)
                    Dim r As Byte = pixel.R
                    Dim g As Byte = pixel.G
                    Dim b As Byte = pixel.B
                    Dim a As Byte = pixel.A
                    Me.m_RawData.CopyTo(BitConverter.GetBytes(CSng(r)), num)
                    num += 4
                    Me.m_RawData.CopyTo(BitConverter.GetBytes(CSng(g)), num)
                    num += 4
                    Me.m_RawData.CopyTo(BitConverter.GetBytes(CSng(b)), num)
                    num += 4
                    Me.m_RawData.CopyTo(BitConverter.GetBytes(CSng(a)), num)
                    num += 4
                Next j
            Next i
        End If
    End Sub
    Private Sub WriteBitmapToDxt()
        Dim output As New MemoryStream(Me.m_RawData)
        Dim bw As New FileWriter(output, Endian.Little)
        'Dim block As New FifaLibrary.DxtBlock(CInt(Me.TextureFormat))   'use external FifaLibrary16.dll because of errors
        Dim block As New DxtBlock(CInt(Me.TextureFormat), Me.m_SwapEndian_DxtBlock)
        Dim num As Integer = ((Me.m_Bitmap.Height + 3) \ 4)
        Dim num2 As Integer = ((Me.m_Bitmap.Width + 3) \ 4)
        If ((Me.m_Bitmap.Height < 4) OrElse (Me.m_Bitmap.Width < 4)) Then
            block.Colors(0, 0) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(0, 1) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(0, 2) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(0, 3) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(1, 0) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(1, 1) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(1, 2) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(1, 3) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(2, 0) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(2, 1) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(2, 2) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(2, 3) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(3, 0) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(3, 1) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(3, 2) = Color.FromArgb(0, 128, 128, 128)
            block.Colors(3, 3) = Color.FromArgb(0, 128, 128, 128)
            Dim i As Integer
            For i = 0 To Me.m_Bitmap.Width - 1
                Dim j As Integer
                For j = 0 To Me.m_Bitmap.Height - 1
                    If (((i >= 0) AndAlso (j >= 0)) AndAlso ((i < 4) AndAlso (j < 4))) Then
                        block.Colors(i, j) = Me.m_Bitmap.GetPixel(i, j)
                    End If
                Next j
            Next i
            block.Save(bw)
        Else
            Dim i As Integer
            For i = 0 To num - 1
                Dim num6 As Integer = (i * 4)
                Dim j As Integer
                For j = 0 To num2 - 1
                    Dim num8 As Integer = (j * 4)
                    block.Colors(0, 0) = Me.m_Bitmap.GetPixel((num8 + 0), (num6 + 0))
                    block.Colors(0, 1) = Me.m_Bitmap.GetPixel((num8 + 0), (num6 + 1))
                    block.Colors(0, 2) = Me.m_Bitmap.GetPixel((num8 + 0), (num6 + 2))
                    block.Colors(0, 3) = Me.m_Bitmap.GetPixel((num8 + 0), (num6 + 3))
                    block.Colors(1, 0) = Me.m_Bitmap.GetPixel((num8 + 1), (num6 + 0))
                    block.Colors(1, 1) = Me.m_Bitmap.GetPixel((num8 + 1), (num6 + 1))
                    block.Colors(1, 2) = Me.m_Bitmap.GetPixel((num8 + 1), (num6 + 2))
                    block.Colors(1, 3) = Me.m_Bitmap.GetPixel((num8 + 1), (num6 + 3))
                    block.Colors(2, 0) = Me.m_Bitmap.GetPixel((num8 + 2), (num6 + 0))
                    block.Colors(2, 1) = Me.m_Bitmap.GetPixel((num8 + 2), (num6 + 1))
                    block.Colors(2, 2) = Me.m_Bitmap.GetPixel((num8 + 2), (num6 + 2))
                    block.Colors(2, 3) = Me.m_Bitmap.GetPixel((num8 + 2), (num6 + 3))
                    block.Colors(3, 0) = Me.m_Bitmap.GetPixel((num8 + 3), (num6 + 0))
                    block.Colors(3, 1) = Me.m_Bitmap.GetPixel((num8 + 3), (num6 + 1))
                    block.Colors(3, 2) = Me.m_Bitmap.GetPixel((num8 + 3), (num6 + 2))
                    block.Colors(3, 3) = Me.m_Bitmap.GetPixel((num8 + 3), (num6 + 3))
                    block.Save(bw)
                Next j
            Next i
        End If
        bw.Close()
        output.Close()
    End Sub


    Private Sub WriteBitmapToGrey8()
        If ((Me.m_Bitmap.Height * Me.m_Bitmap.Width) <= Me.m_RawData.Length) Then
            Dim num As Integer = 0
            Dim i As Integer
            For i = 0 To Me.m_Bitmap.Height - 1
                Dim j As Integer
                For j = 0 To Me.m_Bitmap.Width - 1
                    Dim b As Byte = Me.m_Bitmap.GetPixel(j, i).B
                    Me.m_RawData(num) = b
                    num += 1
                Next j
            Next i
        End If
    End Sub

    Private Sub WriteBitmapToGrey8Alfa8()
        If (((Me.m_Bitmap.Height * Me.m_Bitmap.Width) * 4) <= Me.m_RawData.Length) Then
            Dim num As Integer = 0
            Dim i As Integer
            For i = 0 To Me.m_Bitmap.Height - 1
                Dim j As Integer
                For j = 0 To Me.m_Bitmap.Width - 1
                    Dim b As Byte = Me.m_Bitmap.GetPixel(j, i).B
                    Me.m_RawData(num) = b
                    num += 1
                Next j
            Next i
        End If
    End Sub

    Private Function ConvertToLinearTexture(ByVal data As Byte(), ByVal m_width As Integer, ByVal m_height As Integer, ByVal m_TextureFormat As ETextureFormat) As Byte()
        Dim destData(data.Length - 1) As Byte

        Dim blockSize As Integer
        Dim texelPitch As Integer

        Select Case m_TextureFormat
            Case ETextureFormat.DXT1, ETextureFormat.ATI1
                blockSize = 4
                texelPitch = 8
            Case ETextureFormat.DXT3, ETextureFormat.DXT5, ETextureFormat.ATI2
                'Return destData 'gives bugs sometimes
                blockSize = 4
                texelPitch = 16
            Case ETextureFormat.A8R8G8B8
                blockSize = 1
                texelPitch = 4
            Case ETextureFormat.GREY8
                blockSize = 1
                texelPitch = 1
            Case ETextureFormat.GREY8ALFA8  'no samples found, experimental
                blockSize = 1
                texelPitch = 2
            Case ETextureFormat.R8G8B8  'no samples found, experimental
                blockSize = 1
                texelPitch = 2
            Case ETextureFormat.R5G6B5, ETextureFormat.A4R4G4B4  'no samples found, experimental
                blockSize = 2
                texelPitch = 2

            Case ETextureFormat.A32B32G32R32F
                blockSize = 1
                texelPitch = 16

            Case ETextureFormat.BC6H_UF16
                blockSize = 4
                texelPitch = 16

            Case Else
                Throw New ArgumentOutOfRangeException("Bad dxt type!")
        End Select

        Dim blockWidth As Integer = m_width \ blockSize
        Dim blockHeight As Integer = m_height \ blockSize

        For j As Integer = 0 To blockHeight - 1
            For i As Integer = 0 To blockWidth - 1
                Dim blockOffset As Integer = j * blockWidth + i

                Dim x As Integer = XGAddress2DTiledX(blockOffset, blockWidth, texelPitch)
                Dim y As Integer = XGAddress2DTiledY(blockOffset, blockWidth, texelPitch)

                Dim srcOffset As Integer = j * blockWidth * texelPitch + i * texelPitch
                Dim destOffset As Integer = y * blockWidth * texelPitch + x * texelPitch
                If destOffset <= destData.Length - 1 Then
                    Array.Copy(data, srcOffset, destData, destOffset, texelPitch)
                End If
            Next i
        Next j

        'remove 0's at end
        ReDim Preserve destData(GraphicUtil.GetTextureSize(m_width, m_height, m_TextureFormat) - 1)

        Return destData
    End Function
    Private Function ConvertFromLinearTexture(ByVal data As Byte(), ByVal width As Integer, ByVal height As Integer, ByVal texture As ETextureFormat) As Byte()
        Dim destData(data.Length - 1) As Byte

        Dim blockSize As Integer
        Dim texelPitch As Integer

        Select Case texture
            Case ETextureFormat.DXT1, ETextureFormat.ATI1
                blockSize = 4
                texelPitch = 8
            Case ETextureFormat.DXT3, ETextureFormat.DXT5, ETextureFormat.ATI2
                blockSize = 4
                texelPitch = 16
            Case ETextureFormat.A8R8G8B8
                blockSize = 1
                texelPitch = 4
            Case ETextureFormat.GREY8
                blockSize = 1
                texelPitch = 1
            Case ETextureFormat.GREY8ALFA8  'no samples found, experimental
                blockSize = 1
                texelPitch = 2
            Case ETextureFormat.R8G8B8  'no samples found, experimental
                blockSize = 1
                texelPitch = 2
            Case ETextureFormat.R5G6B5, ETextureFormat.A4R4G4B4  'no samples found, experimental
                blockSize = 2
                texelPitch = 2
            Case ETextureFormat.A32B32G32R32F
                blockSize = 1
                texelPitch = 16
            Case ETextureFormat.BC6H_UF16
                blockSize = 4
                texelPitch = 16
            Case Else
                Throw New ArgumentOutOfRangeException("Bad dxt type!")
        End Select

        Dim blockWidth As Integer = width \ blockSize
        Dim blockHeight As Integer = height \ blockSize

        For j As Integer = 0 To blockHeight - 1
            Dim i As Integer = 0
            Do While i < blockWidth
                Dim blockOffset As Integer = j * blockWidth + i

                Dim x As Integer = XGAddress2DTiledX(blockOffset, blockWidth, texelPitch)
                Dim y As Integer = XGAddress2DTiledY(blockOffset, blockWidth, texelPitch)

                Dim srcOffset As Integer = j * blockWidth * texelPitch + i * texelPitch
                Dim destOffset As Integer = y * blockWidth * texelPitch + x * texelPitch
                If srcOffset <= destData.Length - 1 Then
                    Array.Copy(data, destOffset, destData, srcOffset, texelPitch)
                End If

                i += 1
            Loop
        Next j

        Return destData
    End Function

    Private Function XGAddress2DTiledX(ByVal Offset As Integer, ByVal Width As Integer, ByVal TexelPitch As Integer) As Integer
        Dim AlignedWidth As Integer = (Width + 31) And Not 31

        Dim LogBpp As Integer = (TexelPitch >> 2) + ((TexelPitch >> 1) >> (TexelPitch >> 2))
        Dim OffsetB As Integer = Offset << LogBpp
        Dim OffsetT As Integer = ((OffsetB And Not 4095) >> 3) + ((OffsetB And 1792) >> 2) + (OffsetB And 63)
        Dim OffsetM As Integer = OffsetT >> (7 + LogBpp)

        Dim MacroX As Integer = ((OffsetM Mod (AlignedWidth >> 5)) << 2)
        Dim Tile As Integer = ((((OffsetT >> (5 + LogBpp)) And 2) + (OffsetB >> 6)) And 3)
        Dim Macro As Integer = (MacroX + Tile) << 3
        Dim Micro As Integer = ((((OffsetT >> 1) And Not 15) + (OffsetT And 15)) And ((TexelPitch << 3) - 1)) >> LogBpp

        Return Macro + Micro
    End Function

    Private Function XGAddress2DTiledY(ByVal Offset As Integer, ByVal Width As Integer, ByVal TexelPitch As Integer) As Integer
        Dim AlignedWidth As Integer = (Width + 31) And Not 31

        Dim LogBpp As Integer = (TexelPitch >> 2) + ((TexelPitch >> 1) >> (TexelPitch >> 2))
        Dim OffsetB As Integer = Offset << LogBpp
        Dim OffsetT As Integer = ((OffsetB And Not 4095) >> 3) + ((OffsetB And 1792) >> 2) + (OffsetB And 63)
        Dim OffsetM As Integer = OffsetT >> (7 + LogBpp)

        Dim MacroY As Integer = ((OffsetM \ (AlignedWidth >> 5)) << 2)
        Dim Tile As Integer = ((OffsetT >> (6 + LogBpp)) And 1) + (((OffsetB And 2048) >> 10))
        Dim Macro As Integer = (MacroY + Tile) << 3
        Dim Micro As Integer = ((((OffsetT And (((TexelPitch << 6) - 1) And Not 31)) + ((OffsetT And 15) << 1)) >> (3 + LogBpp)) And Not 1)

        Return Macro + Micro + ((OffsetT And 16) >> 4)
    End Function

    Private Function GetSwapNumBytes(ByVal TextureFormat As ETextureFormat) As Integer
        Dim SwapNumBytes As Integer = 2

        Select Case TextureFormat
            Case ETextureFormat.DXT1, ETextureFormat.ATI1, ETextureFormat.DXT3, ETextureFormat.DXT5, ETextureFormat.ATI2
                SwapNumBytes = 2
            Case ETextureFormat.A8R8G8B8, ETextureFormat.R8G8B8
                SwapNumBytes = 4
            Case ETextureFormat.GREY8, ETextureFormat.GREY8ALFA8
                SwapNumBytes = 1
            Case ETextureFormat.R5G6B5, ETextureFormat.A4R4G4B4
                SwapNumBytes = 2
            Case ETextureFormat.A32B32G32R32F
                SwapNumBytes = 16
            Case ETextureFormat.BC6H_UF16
                SwapNumBytes = 2
        End Select

        Return SwapNumBytes
    End Function

    Friend Sub SetEndianFormat(ByVal BigEndian As Boolean)

        If Me.m_SwapEndian_DxtBlock <> BigEndian Then
            Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.TextureFormat)
            If SwapNumBytes > 1 Then
                For x As Integer = 0 To m_RawData.Length - 1 Step SwapNumBytes
                    Array.Reverse(Me.m_RawData, x, SwapNumBytes)
                Next x
            End If
            Me.m_SwapEndian_DxtBlock = BigEndian
        End If

    End Sub
    Friend Sub SetTiling360Format(ByVal Tiled360 As Boolean)
        If Me.m_Tiled360 <> Tiled360 Then

            If Tiled360 = True Then
                ' tile the data
                Me.m_RawData = ConvertFromLinearTexture(Me.m_RawData, Me.Width, Me.Height, Me.TextureFormat)
            ElseIf Tiled360 = False Then
                ' Untile the data
                Me.m_RawData = ConvertToLinearTexture(Me.m_RawData, Me.Width, Me.Height, Me.TextureFormat)
            End If

            Me.m_Tiled360 = Tiled360
        End If
    End Sub

    Public Function GetRawData(ByVal BigEndian As Boolean, Optional ByVal Tiled360 As Boolean = False) As Byte()
        Dim RawDataFixed As Byte() '= Nothing '= Me.m_RawData '.Clone
        Dim Current_Tiled360 As Boolean = Me.m_Tiled360
        Dim Current_SwapEndian_DxtBlock As Boolean = Me.m_SwapEndian_DxtBlock

        If Me.NeedToSaveRawData Then    '1 - when new bitmap added, rawdata need be created
            Me.m_RawData = New Byte(Me.Size - 1) {}
            Me.CreateRawData()  '2 - this is always untiled / little endian
            RawDataFixed = Me.m_RawData.Clone
            Current_Tiled360 = False
            Current_SwapEndian_DxtBlock = False

            '3 - Fix Me.m_RawData (local bytes) in correct format
            If Current_Tiled360 <> Tiled360 Then
                If Tiled360 = True Then
                    ' tile the data
                    Me.m_RawData = ConvertFromLinearTexture(Me.m_RawData, Me.Width, Me.Height, Me.TextureFormat)
                ElseIf Tiled360 = False Then
                    ' Untile the data
                    Me.m_RawData = ConvertToLinearTexture(Me.m_RawData, Me.Width, Me.Height, Me.TextureFormat)
                End If
            End If
            If Current_SwapEndian_DxtBlock <> BigEndian Then
                Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.TextureFormat)
                For x As Integer = 0 To Me.m_RawData.Length - 1 Step SwapNumBytes
                    Array.Reverse(Me.m_RawData, x, SwapNumBytes)
                Next x
            End If

            Me.NeedToSaveRawData = False
        Else
            RawDataFixed = Me.m_RawData.Clone
        End If


        '3 - Fix RawDataFixed (output bytes) in correct format
        If Current_Tiled360 <> Tiled360 Then
            If Tiled360 = True Then
                ' tile the data
                RawDataFixed = ConvertFromLinearTexture(RawDataFixed, Me.Width, Me.Height, Me.TextureFormat)
            ElseIf Tiled360 = False Then
                ' Untile the data
                RawDataFixed = ConvertToLinearTexture(RawDataFixed, Me.Width, Me.Height, Me.TextureFormat)
            End If
        End If
        If Current_SwapEndian_DxtBlock <> BigEndian Then
            Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.TextureFormat)
            For x As Integer = 0 To RawDataFixed.Length - 1 Step SwapNumBytes
                Array.Reverse(RawDataFixed, x, SwapNumBytes)
            Next x
        End If

        Return RawDataFixed
    End Function
    Public Sub SetRawData(ByVal Data As Byte(), ByVal BigEndian As Boolean, Optional ByVal Tiled360 As Boolean = False)
        Me.NeedToSaveRawData = False
        If Me.m_Tiled360 <> Tiled360 Then
            If Tiled360 = True Then
                ' tile the data
                Data = ConvertFromLinearTexture(Data, Me.Width, Me.Height, Me.TextureFormat)
            ElseIf Tiled360 = False Then
                ' Untile the data
                Data = ConvertToLinearTexture(Data, Me.Width, Me.Height, Me.TextureFormat)
            End If
        End If
        If Me.m_SwapEndian_DxtBlock <> BigEndian Then
            Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.TextureFormat)
            For x As Integer = 0 To Data.Length - 1 Step SwapNumBytes
                Array.Reverse(Data, x, SwapNumBytes)
            Next x
        End If
        Me.m_RawData = Data
    End Sub

    ' Properties
    Public Property Bitmap As Bitmap
        Get
            If (Me.m_Bitmap Is Nothing) Then
                Me.CreateBitmap()
            End If
            Return Me.m_Bitmap
        End Get
        Set(ByVal value As Bitmap)
            Me.m_Bitmap = value
            Me.NeedToSaveRawData = True
        End Set
    End Property

    Friend Property RawData As Byte()
        Get
            Return Me.m_RawData
        End Get
        Set
            Me.m_RawData = Value
        End Set
    End Property

    ' Fields
    Private m_Bitmap As Bitmap
    Protected Width As UInteger
    Protected Height As UInteger
    Protected TextureFormat As ETextureFormat
    Protected NeedToSaveRawData As Boolean
    Protected Size As UInteger

    Private m_SwapEndian_DxtBlock As Boolean  'DxtBlock - Endian Format : always little endian, unless some (xbox WC10 game, ..)
    Private m_Tiled360 As Boolean
    Private m_RawData As Byte()
End Class

