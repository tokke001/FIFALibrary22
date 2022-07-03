Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Class RawImage
    ' Methods
    'Public Sub New()

    'End Sub
    Public Sub New(ByVal width As UInteger, ByVal height As UInteger, ByVal TextureFormat As ETextureFormat, ByVal size As UInteger, ByVal SwapEndian_DxtBlock As Boolean, Optional Tiled360 As Boolean = False)
        Me.m_Width = width
        Me.m_Height = height
        Me.m_TextureFormat = TextureFormat
        Me.m_Size = size
        Me.m_SwapEndian_DxtBlock = SwapEndian_DxtBlock
        Me.m_Tiled360 = Tiled360
        Me.m_Bitmap = Nothing
    End Sub

    Private Sub CreateBitmap()
        Dim RawDataFixed As Byte() = Me.m_RawData.Clone
        If Me.m_Tiled360 Then
            ' Untile the data
            RawDataFixed = ConvertToLinearTexture(Me.m_RawData.Clone, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
        End If
        If Me.m_SwapEndian_DxtBlock Then
            Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_TextureFormat)
            For x As Integer = 0 To RawDataFixed.Length - 1 Step SwapNumBytes
                Array.Reverse(RawDataFixed, x, SwapNumBytes)
            Next x
        End If

        If (Me.m_Width < 1) Then
            Me.m_Width = 1
        End If
        If (Me.m_Height < 1) Then
            Me.m_Height = 1
        End If
        Select Case Me.m_TextureFormat
            Case ETextureFormat.BC1, ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC5, ETextureFormat.BC4, ETextureFormat.BC6H_UF16, ETextureFormat.BC7
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadDxtToBitmap(RawDataFixed)

            Case ETextureFormat.B8G8R8A8
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadB8G8R8A8ToBitmap(RawDataFixed)

            Case ETextureFormat.B8G8R8
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadB8G8R8ToBitmap(RawDataFixed)

            Case ETextureFormat.B4G4R4A4
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadB4G4R4A4ToBitmap(RawDataFixed)

            Case ETextureFormat.B5G6R5
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadB5G6R5ToBitmap(RawDataFixed)

            Case ETextureFormat.B5G5R5A1
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadB5G5R5A1ToBitmap(RawDataFixed)

            Case ETextureFormat.L8
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadL8ToBitmap(RawDataFixed)

            Case ETextureFormat.L8A8
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadL8A8ToBitmap(RawDataFixed)

            Case ETextureFormat.R32G32B32A32Float
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadR32G32B32A32FToBitmap(RawDataFixed)

            Case ETextureFormat.R8G8B8A8
                Me.m_Bitmap = New Bitmap(Me.m_Width, Me.m_Height, PixelFormat.Format32bppArgb)
                Me.ReadR8G8B8A8ToBitmap(RawDataFixed)

                'Case ETextureFormat.BIT8
                'Case ETextureFormat.CTX1

            Case Else
                Return
        End Select
    End Sub

    Private Sub ReadR8G8B8A8ToBitmap(RawDataFixed() As Byte)
        Dim Index As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim red As Integer = RawDataFixed(Index)
                Index += 1
                Dim green As Integer = RawDataFixed(Index)
                Index += 1
                Dim blue As Integer = RawDataFixed(Index)
                Index += 1
                Dim alpha As Integer = RawDataFixed(Index)
                Index += 1

                Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, red, green, blue))
            Next j
        Next i
    End Sub

    Private Sub ReadR32G32B32A32FToBitmap(RawDataFixed() As Byte)
        Dim f As New MemoryStream(RawDataFixed)
        Dim r As New FileReader(f, Endian.Little)

        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim red As Single = Math.Min(r.ReadSingle * 255, 255)
                Dim green As Single = Math.Min(r.ReadSingle * 255, 255)
                Dim blue As Single = Math.Min(r.ReadSingle * 255, 255)
                Dim alpha As Single = Math.Min(r.ReadSingle * 255, 255)

                Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, red, green, blue))
            Next j
        Next i
    End Sub

    Private Sub ReadL8A8ToBitmap(RawDataFixed() As Byte)
        Dim Index As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim Lum As Integer = RawDataFixed(Index)
                Index += 1
                Dim alpha As Integer = RawDataFixed(Index)
                Index += 1

                Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, Lum, Lum, Lum))
            Next j
        Next i
    End Sub

    Private Sub ReadL8ToBitmap(RawDataFixed() As Byte)
        Dim Index As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim Lum As Integer = RawDataFixed(Index)
                Index += 1
                Dim alpha As Integer = 255

                Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, Lum, Lum, Lum))
            Next j
        Next i
    End Sub

    Private Sub ReadB5G5R5A1ToBitmap(RawDataFixed() As Byte)
        Dim f As New MemoryStream(RawDataFixed)
        Dim r As New FileReader(f, Endian.Little)

        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim Value As UShort = r.ReadUInt16

                Dim blue As Integer = (FifaUtil.GetValueFrom16bit(Value, 0, 5) / 31) * 255
                Dim green As Integer = (FifaUtil.GetValueFrom16bit(Value, 5, 5) / 31) * 255
                Dim red As Integer = (FifaUtil.GetValueFrom16bit(Value, 10, 5) / 31) * 255
                Dim alpha As Integer = FifaUtil.GetValueFrom16bit(Value, 15, 1) * 255

                Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, red, green, blue))
            Next j
        Next i
    End Sub

    Private Sub ReadB5G6R5ToBitmap(RawDataFixed() As Byte)
        Dim f As New MemoryStream(RawDataFixed)
        Dim r As New FileReader(f, Endian.Little)

        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim Value As UShort = r.ReadUInt16

                Dim blue As Integer = (FifaUtil.GetValueFrom16bit(Value, 0, 5) / 31) * 255
                Dim green As Integer = (FifaUtil.GetValueFrom16bit(Value, 5, 6) / 63) * 255
                Dim red As Integer = (FifaUtil.GetValueFrom16bit(Value, 11, 5) / 31) * 255
                Dim alpha As Integer = 255

                Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, red, green, blue))
            Next j
        Next i
    End Sub

    Private Sub ReadB4G4R4A4ToBitmap(RawDataFixed() As Byte)
        Dim f As New MemoryStream(RawDataFixed)
        Dim r As New FileReader(f, Endian.Little)

        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim Value As UShort = r.ReadUInt16

                Dim blue As Integer = (FifaUtil.GetValueFrom16bit(Value, 0, 4) / 15) * 255
                Dim green As Integer = (FifaUtil.GetValueFrom16bit(Value, 4, 4) / 15) * 255
                Dim red As Integer = (FifaUtil.GetValueFrom16bit(Value, 8, 4) / 15) * 255
                Dim alpha As Integer = (FifaUtil.GetValueFrom16bit(Value, 12, 4) / 15) * 255

                Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, red, green, blue))
            Next j
        Next i
    End Sub

    Private Sub ReadB8G8R8ToBitmap(RawDataFixed() As Byte)
        Dim Index As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim blue As Integer = RawDataFixed(Index)
                Index += 1
                Dim green As Integer = RawDataFixed(Index)
                Index += 1
                Dim red As Integer = RawDataFixed(Index)
                Index += 1
                Dim alpha As Integer = 255

                Me.m_Bitmap.SetPixel(j, i, Color.FromArgb(alpha, red, green, blue))
            Next j
        Next i
    End Sub

    Private Sub ReadB8G8R8A8ToBitmap(RawDataFixed() As Byte)
        Dim rect As New Rectangle(0, 0, Me.m_Bitmap.Width, Me.m_Bitmap.Height)
        Dim bitmapdata As BitmapData = Me.m_Bitmap.LockBits(rect, ImageLockMode.WriteOnly, Me.m_Bitmap.PixelFormat)
        Dim destination As IntPtr = bitmapdata.Scan0
        Dim num As Integer = (Me.m_Bitmap.Width * Me.m_Bitmap.Height)
        Marshal.Copy(RawDataFixed, 0, destination, (num * 4))
        Me.m_Bitmap.UnlockBits(bitmapdata)
    End Sub

    Private Sub CreateRawData()
        If (Me.m_Width < 1) Then
            Me.m_Width = 1
        End If
        If (Me.m_Height < 1) Then
            Me.m_Height = 1
        End If
        If GraphicUtil.GetTextureSize(Me.m_Bitmap.Width, Me.m_Bitmap.Height, Me.m_TextureFormat) > Me.m_RawData.Length Then
            Exit Sub
        End If
        If (Me.m_Bitmap.PixelFormat <> PixelFormat.Format32bppArgb) Then
            Me.m_Bitmap = GraphicUtil.Get32bitBitmap(Me.m_Bitmap)
        End If
        Select Case Me.m_TextureFormat
            Case ETextureFormat.BC1, ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC5, ETextureFormat.BC4, ETextureFormat.BC6H_UF16, ETextureFormat.BC7
                Me.WriteBitmapToDxt()

            Case ETextureFormat.B8G8R8A8
                Me.WriteBitmapToB8G8R8A8()

            Case ETextureFormat.B8G8R8
                Me.WriteBitmapToB8G8R8()

            Case ETextureFormat.B4G4R4A4
                Me.WriteBitmapToB4G4R4A4()

            Case ETextureFormat.B5G6R5
                Me.WriteBitmapToB5G6R5()

            Case ETextureFormat.B5G5R5A1
                Me.WriteBitmapToB5G5R5A1()

            Case ETextureFormat.L8
                Me.WriteBitmapToL8()

            Case ETextureFormat.L8A8
                Me.WriteBitmapToL8A8()

            Case ETextureFormat.R32G32B32A32Float
                Me.WriteBitmapToR32G32B32A32F()

            Case ETextureFormat.R8G8B8A8
                Me.WriteBitmapToR8G8B8A8()

                'Case ETextureFormat.BIT8
                'Case ETextureFormat.CTX1

            Case Else
                Return
        End Select
    End Sub

    Public Function Load(ByVal r As FileReader) As Boolean
        'Dim size As Integer = Me.m_Size
        'If (Me._TextureFormat = ETextureFormat.A8R8G8B8) Then
        'Size = ((Me._Width * Me._Height) * 4)
        'End If
        Me.m_RawData = r.ReadBytes(Me.m_Size)

        Me.NeedToSaveRawData = False
        Return True
    End Function

    Private Sub ReadDxtToBitmap(ByVal RawDataFixed As Byte())
        Dim decoder As New BCnEncoder.Decoder.BcDecoder()
        Dim Format As BCnEncoder.Shared.CompressionFormat = GetBCnEncoderFormat(Me.m_TextureFormat)
        'decoder.InputOptions.ddsBc1ExpectAlpha = False
        'decoder.OutputOptions.redAsLuminance = False
        decoder.OutputOptions.blueRecalculate = True

        Me.m_Bitmap = decoder.DecodeRawData(RawDataFixed, Me.m_Width, Me.m_Height, Format)
    End Sub

    'Private Sub ReadDxtToBitmap_OLD(ByVal RawDataFixed As Byte())
    '    Dim rect As New Rectangle(0, 0, Me._Width, Me._Height)
    '    Dim bitmapdata As BitmapData = Me.m_Bitmap.LockBits(rect, ImageLockMode.WriteOnly, Me.m_Bitmap.PixelFormat)
    '    Dim destination As IntPtr = bitmapdata.Scan0
    '    'Dim length As Integer = (Me.m_Bitmap.Width * Me.m_Bitmap.Height)

    '    Dim decoder As New BCnEncoder.Decoder.BcDecoder()
    '    Dim Format As BCnEncoder.Shared.CompressionFormat = GetBCnEncoderFormat(Me._TextureFormat)
    '    'decoder.InputOptions.ddsBc1ExpectAlpha = False
    '    'decoder.OutputOptions.redAsLuminance = False
    '    'Dim source As BCnEncoder.Shared.ColorRgba32() = decoder.DecodeRaw(RawDataFixed, Me._Width, Me._Height, Format)
    '    'Dim Source_bytes As Byte() = New Byte((source.Length * 4) - 1) {}

    '    'For i = 0 To source.Length - 1

    '    '    If Format = BCnEncoder.Shared.CompressionFormat.Bc5 Then
    '    '        'https://github.com/Nominom/BCnEncoder.NET/issues/59
    '    '        'https://forums.unrealengine.com/t/the-math-behind-combining-bc5-normals/365189
    '    '        'Dim test = Math.Sqrt(1 - (Math.Sqrt(m_R ) + Math.Sqrt(m_G ))) 
    '    '        source(i).b = 255
    '    '    End If

    '    '    Source_bytes(i * 4) = source(i).b
    '    '    Source_bytes((i * 4) + 1) = source(i).g
    '    '    Source_bytes((i * 4) + 2) = source(i).r
    '    '    Source_bytes((i * 4) + 3) = source(i).a
    '    'Next

    '    'Marshal.Copy(Source_bytes, 0, destination, source.Length)


    '    Dim source As Byte() = decoder.DecodeRawData(RawDataFixed, Me._Width, Me._Height, Format)
    '    For i = 0 To source.Length - 1 Step 4
    '        Dim m_R As Byte = source(i)
    '        Dim m_G As Byte = source(i + 1)
    '        Dim m_B As Byte = source(i + 2)
    '        'Dim alpha As Byte = source(i + 3)

    '        If Format = BCnEncoder.Shared.CompressionFormat.BC5 Then
    '            'https://github.com/Nominom/BCnEncoder.NET/issues/59
    '            'https://forums.unrealengine.com/t/the-math-behind-combining-bc5-normals/365189
    '            'Dim test = Math.Sqrt(1 - (Math.Sqrt(m_R ) + Math.Sqrt(m_G ))) 
    '            m_B = 255
    '        End If

    '        source(i) = m_B
    '        'source(i + 1) = m_G
    '        source(i + 2) = m_R
    '        'source(i) = alpha
    '    Next

    '    Marshal.Copy(source, 0, destination, source.Length)
    '    Me.m_Bitmap.UnlockBits(bitmapdata)

    'End Sub

    Private Function GetBCnEncoderFormat(ByVal m_TextureFormat As ETextureFormat) As BCnEncoder.Shared.CompressionFormat
        Select Case m_TextureFormat
            Case ETextureFormat.BC1
                Return BCnEncoder.Shared.CompressionFormat.BC1WithAlpha
                'Case ETextureFormat.BC1WithAlpha
                'Return BCnEncoder.Shared.CompressionFormat.BC1WithAlpha
            Case ETextureFormat.BC2
                Return BCnEncoder.Shared.CompressionFormat.BC2
            Case ETextureFormat.BC3
                Return BCnEncoder.Shared.CompressionFormat.BC3
            Case ETextureFormat.BC4
                Return BCnEncoder.Shared.CompressionFormat.BC4
            Case ETextureFormat.BC5
                Return BCnEncoder.Shared.CompressionFormat.BC5
            Case ETextureFormat.BC6H_UF16
                Return BCnEncoder.Shared.CompressionFormat.BC6  '--> currently not supported by BCnEncoder !
            Case ETextureFormat.BC7
                Return BCnEncoder.Shared.CompressionFormat.BC7
        End Select

        Return Nothing
    End Function

    Public Function Save(ByVal SwapEndian_DxtBlock As Boolean, ByVal w As FileWriter) As Boolean

        If Me.NeedToSaveRawData Then    '1 - when new bitmap added, rawdata need be created
            'Dim size As Integer = Me.m_Size
            'If (Me._TextureFormat = ETextureFormat.A8R8G8B8) Then
            'size = ((Me._Width * Me._Height) * 4)
            'End If
            Me.m_RawData = New Byte(Me.m_Size - 1) {}

            Me.CreateRawData()  '2 - this is always little endian

            If SwapEndian_DxtBlock Then    '3 - if big endian needed -> swap !
                Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_TextureFormat)
                For x As Integer = 0 To Me.m_RawData.Length - 1 Step SwapNumBytes
                    Array.Reverse(Me.m_RawData, x, SwapNumBytes)
                Next x
            End If

            Me.NeedToSaveRawData = False
        Else    'when no rawdata update needed, but endian formats (source vs request) are different
            If Me.m_SwapEndian_DxtBlock <> SwapEndian_DxtBlock Then
                Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_TextureFormat)
                For x As Integer = 0 To Me.m_RawData.Length - 1 Step SwapNumBytes
                    Array.Reverse(Me.m_RawData, x, SwapNumBytes)
                Next x
            End If
        End If

        Me.m_SwapEndian_DxtBlock = SwapEndian_DxtBlock

        w.Write(Me.m_RawData)
        Return True
    End Function

    Private Sub WriteBitmapToB8G8R8A8()
        Dim num As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)

                Me.m_RawData(num) = pixel.B
                num += 1
                Me.m_RawData(num) = pixel.G
                num += 1
                Me.m_RawData(num) = pixel.R
                num += 1
                Me.m_RawData(num) = pixel.A
                num += 1
            Next j
        Next i
    End Sub

    Private Sub WriteBitmapToR8G8B8A8()
        Dim num As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)

                Me.m_RawData(num) = pixel.R
                num += 1
                Me.m_RawData(num) = pixel.G
                num += 1
                Me.m_RawData(num) = pixel.B
                num += 1
                Me.m_RawData(num) = pixel.A
                num += 1
            Next j
        Next i
    End Sub

    Private Sub WriteBitmapToB8G8R8()
        Dim num As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
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
    End Sub

    Private Sub WriteBitmapToB4G4R4A4()
        Dim output As New MemoryStream(Me.m_RawData) ', 0, 0)
        Dim w As New FileWriter(output, Endian.Little)

        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim Value As UShort = 0
                Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)

                Value = FifaUtil.SetValueTo16bit(Value, (pixel.B / 255) * 15, 0, 4)
                Value = FifaUtil.SetValueTo16bit(Value, (pixel.G / 255) * 15, 4, 4)
                Value = FifaUtil.SetValueTo16bit(Value, (pixel.R / 255) * 15, 8, 4)
                Value = FifaUtil.SetValueTo16bit(Value, (pixel.A / 255) * 15, 12, 4)

                w.Write(Value)
            Next j
        Next i
    End Sub

    Private Sub WriteBitmapToB5G6R5()
        Dim output As New MemoryStream(Me.m_RawData) ', 0, 0)
        Dim w As New FileWriter(output, Endian.Little)

        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim Value As UShort = 0
                Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)

                Value = FifaUtil.SetValueTo16bit(Value, (pixel.B / 255) * 31, 0, 5)
                Value = FifaUtil.SetValueTo16bit(Value, (pixel.G / 255) * 63, 5, 6)
                Value = FifaUtil.SetValueTo16bit(Value, (pixel.R / 255) * 31, 11, 5)

                w.Write(CUShort(Value))
            Next j
        Next i
    End Sub

    Private Sub WriteBitmapToB5G5R5A1()
        Dim output As New MemoryStream(Me.m_RawData) ', 0, 0)
        Dim w As New FileWriter(output, Endian.Little)

        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim Value As UShort = 0
                Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)

                Value = FifaUtil.SetValueTo16bit(Value, (pixel.B / 255) * 31, 0, 5)
                Value = FifaUtil.SetValueTo16bit(Value, (pixel.G / 255) * 31, 5, 5)
                Value = FifaUtil.SetValueTo16bit(Value, (pixel.R / 255) * 31, 10, 5)
                Value = FifaUtil.SetValueTo16bit(Value, (pixel.A / 255), 15, 1)

                w.Write(Value)
            Next j
        Next i
    End Sub

    Private Sub WriteBitmapToR32G32B32A32F()    '128-bit float format using 32 bits for the each channel (alpha, blue, green, red)
        Dim output As New MemoryStream(Me.m_RawData) ', 0, 0)
        Dim w As New FileWriter(output, Endian.Little)

        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)

                w.Write(CSng(pixel.R / 255))
                w.Write(CSng(pixel.G / 255))
                w.Write(CSng(pixel.B / 255))
                w.Write(CSng(pixel.A / 255))
            Next j
        Next i
    End Sub

    Private Sub WriteBitmapToDxt()
        Dim Encoder As New BCnEncoder.Encoder.BcEncoder()
        Encoder.OutputOptions.quality = BCnEncoder.Encoder.CompressionQuality.BestQuality
        Encoder.OutputOptions.format = GetBCnEncoderFormat(Me.m_TextureFormat)
        'Encoder.InputOptions.luminanceAsRed = True
        Me.m_RawData = Encoder.EncodeToRawBytes(Me.m_Bitmap, Me.m_Width, Me.m_Height, Me.m_Width, Me.m_Height)

    End Sub

    Private Sub WriteBitmapToL8()
        Dim num As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim b As Byte = Me.m_Bitmap.GetPixel(j, i).B
                Me.m_RawData(num) = b
                num += 1
            Next j
        Next i
    End Sub

    Private Sub WriteBitmapToL8A8()
        Dim num As Integer = 0
        For i = 0 To Me.m_Bitmap.Height - 1
            For j = 0 To Me.m_Bitmap.Width - 1
                Dim pixel As Color = Me.m_Bitmap.GetPixel(j, i)
                Me.m_RawData(num) = pixel.B
                num += 1
                Me.m_RawData(num) = pixel.A
                num += 1
            Next j
        Next i
    End Sub

    Private Function ConvertToLinearTexture(ByVal data As Byte(), ByVal m_width As Integer, ByVal m_height As Integer, ByVal m_TextureFormat As ETextureFormat) As Byte()
        Dim destData(data.Length - 1) As Byte

        Dim blockSize As Integer
        Dim texelPitch As Integer

        Select Case m_TextureFormat
            Case ETextureFormat.BC1, ETextureFormat.BC4
                blockSize = 4
                texelPitch = 8
            Case ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC5
                'Return destData 'gives bugs sometimes
                blockSize = 4
                texelPitch = 16
            Case ETextureFormat.B8G8R8A8
                blockSize = 1
                texelPitch = 4
            Case ETextureFormat.L8
                blockSize = 1
                texelPitch = 1
            Case ETextureFormat.L8A8  'no samples found, experimental
                blockSize = 1
                texelPitch = 2
            Case ETextureFormat.B8G8R8  'no samples found, experimental
                blockSize = 1
                texelPitch = 2
            Case ETextureFormat.B5G6R5, ETextureFormat.B4G4R4A4  'no samples found, experimental
                blockSize = 2
                texelPitch = 2

            Case ETextureFormat.R32G32B32A32Float
                blockSize = 1
                texelPitch = 16

            Case ETextureFormat.BC6H_UF16
                blockSize = 4
                texelPitch = 16

            Case ETextureFormat.BC7
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
            Case ETextureFormat.BC1, ETextureFormat.BC4
                blockSize = 4
                texelPitch = 8
            Case ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC5
                blockSize = 4
                texelPitch = 16
            Case ETextureFormat.B8G8R8A8
                blockSize = 1
                texelPitch = 4
            Case ETextureFormat.L8
                blockSize = 1
                texelPitch = 1
            Case ETextureFormat.L8A8  'no samples found, experimental
                blockSize = 1
                texelPitch = 2
            Case ETextureFormat.B8G8R8  'no samples found, experimental
                blockSize = 1
                texelPitch = 2
            Case ETextureFormat.B5G6R5, ETextureFormat.B4G4R4A4  'no samples found, experimental
                blockSize = 2
                texelPitch = 2
            Case ETextureFormat.R32G32B32A32Float
                blockSize = 1
                texelPitch = 16
            Case ETextureFormat.BC6H_UF16
                blockSize = 4
                texelPitch = 16
            Case ETextureFormat.BC7
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
            Case ETextureFormat.BC1, ETextureFormat.BC4, ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC5
                SwapNumBytes = 2
            Case ETextureFormat.B8G8R8A8, ETextureFormat.B8G8R8
                SwapNumBytes = 4
            Case ETextureFormat.L8
                SwapNumBytes = 1
            Case ETextureFormat.B5G6R5, ETextureFormat.B4G4R4A4, ETextureFormat.L8A8
                SwapNumBytes = 2
            Case ETextureFormat.R32G32B32A32Float
                SwapNumBytes = 16
            Case ETextureFormat.BC6H_UF16
                SwapNumBytes = 2
            Case ETextureFormat.BC7
                SwapNumBytes = 2
        End Select

        Return SwapNumBytes
    End Function

    Friend Sub SetEndianFormat(ByVal BigEndian As Boolean)

        If Me.m_SwapEndian_DxtBlock <> BigEndian Then
            Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_TextureFormat)
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
                Me.m_RawData = ConvertFromLinearTexture(Me.m_RawData, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
            ElseIf Tiled360 = False Then
                ' Untile the data
                Me.m_RawData = ConvertToLinearTexture(Me.m_RawData, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
            End If

            Me.m_Tiled360 = Tiled360
        End If
    End Sub

    Private Function GetRawData(ByVal BigEndian As Boolean, Optional ByVal Tiled360 As Boolean = False) As Byte()
        Dim RawDataFixed As Byte() '= Nothing '= Me.m_RawData '.Clone
        Dim Current_Tiled360 As Boolean = Me.m_Tiled360
        Dim Current_SwapEndian_DxtBlock As Boolean = Me.m_SwapEndian_DxtBlock

        If Me.NeedToSaveRawData Then    '1 - when new bitmap added, rawdata need be created
            Me.m_RawData = New Byte(Me.m_Size - 1) {}
            Me.CreateRawData()  '2 - this is always untiled / little endian
            RawDataFixed = Me.m_RawData.Clone
            Current_Tiled360 = False
            Current_SwapEndian_DxtBlock = False

            '3 - Fix Me.m_RawData (local bytes) in correct format
            If Current_Tiled360 <> Tiled360 Then
                If Tiled360 = True Then
                    ' tile the data
                    Me.m_RawData = ConvertFromLinearTexture(Me.m_RawData, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
                ElseIf Tiled360 = False Then
                    ' Untile the data
                    Me.m_RawData = ConvertToLinearTexture(Me.m_RawData, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
                End If
            End If
            If Current_SwapEndian_DxtBlock <> BigEndian Then
                Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_TextureFormat)
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
                RawDataFixed = ConvertFromLinearTexture(RawDataFixed, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
            ElseIf Tiled360 = False Then
                ' Untile the data
                RawDataFixed = ConvertToLinearTexture(RawDataFixed, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
            End If
        End If
        If Current_SwapEndian_DxtBlock <> BigEndian Then
            Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_TextureFormat)
            For x As Integer = 0 To RawDataFixed.Length - 1 Step SwapNumBytes
                Array.Reverse(RawDataFixed, x, SwapNumBytes)
            Next x
        End If

        Return RawDataFixed
    End Function
    Private Sub SetRawData(ByVal Data As Byte(), ByVal BigEndian As Boolean, Optional ByVal Tiled360 As Boolean = False)
        Me.NeedToSaveRawData = False
        If Me.m_Tiled360 <> Tiled360 Then
            If Tiled360 = True Then
                ' tile the data
                Data = ConvertFromLinearTexture(Data, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
            ElseIf Tiled360 = False Then
                ' Untile the data
                Data = ConvertToLinearTexture(Data, Me.m_Width, Me.m_Height, Me.m_TextureFormat)
            End If
        End If
        If Me.m_SwapEndian_DxtBlock <> BigEndian Then
            Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_TextureFormat)
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

    Public Property RawData(ByVal BigEndian As Boolean, Optional ByVal Tiled360 As Boolean = False) As Byte()
        Get
            Return GetRawData(BigEndian, Tiled360)
        End Get
        Set
            SetRawData(Value, BigEndian, Tiled360)
        End Set
    End Property

    Public Property Width As UInteger
        Get
            Return m_Width
        End Get
        Private Set
            m_Width = Value
        End Set
    End Property

    Public Property Height As UInteger
        Get
            Return m_Height
        End Get
        Private Set
            m_Height = Value
        End Set
    End Property

    Public Property TextureFormat As ETextureFormat
        Get
            Return m_TextureFormat
        End Get
        Private Set
            m_TextureFormat = Value
        End Set
    End Property

    Protected Property Size As UInteger
        Get
            Return m_Size
        End Get
        Set
            m_Size = Value
        End Set
    End Property

    Protected NeedToSaveRawData As Boolean

    ' Fields
    Private m_SwapEndian_DxtBlock As Boolean  'DxtBlock - Endian Format : always little endian, unless some (xbox WC10 game, ..)
    Private m_Tiled360 As Boolean
    Private m_Bitmap As Bitmap
    Private m_RawData As Byte()
    Private m_Height As UInteger
    Private m_Width As UInteger
    Private m_TextureFormat As ETextureFormat
    Private m_Size As UInteger
End Class

