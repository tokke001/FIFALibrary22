Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Class GraphicUtil
    ' Methods
    Public Shared Sub AddColorOffset(ByVal bitmap As Bitmap, ByVal deltaR As Integer, ByVal deltaG As Integer, ByVal deltaB As Integer)
        If (Not bitmap Is Nothing) Then
            Dim i As Integer
            For i = 0 To bitmap.Width - 1
                Dim j As Integer
                For j = 0 To bitmap.Height - 1
                    Dim pixel As Color = bitmap.GetPixel(i, j)
                    Dim red As Integer = (pixel.R + deltaR)
                    Dim green As Integer = (pixel.G + deltaG)
                    Dim blue As Integer = (pixel.B + deltaB)
                    If (red > &HFF) Then
                        red = &HFF
                    End If
                    If (green > &HFF) Then
                        green = &HFF
                    End If
                    If (blue > &HFF) Then
                        blue = &HFF
                    End If
                    If (red < 0) Then
                        red = 0
                    End If
                    If (green < 0) Then
                        green = 0
                    End If
                    If (blue < 0) Then
                        blue = 0
                    End If
                    bitmap.SetPixel(i, j, Color.FromArgb(pixel.A, red, green, blue))
                Next j
            Next i
        End If
    End Sub

    Public Shared Function AddColorOffsetPreservingAlfa(ByVal sourceBitmap As Bitmap, ByVal dR As Integer, ByVal dG As Integer, ByVal dB As Integer, ByVal preserveAlfa As Boolean) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        If (sourceBitmap.PixelFormat <> PixelFormat.Format32bppArgb) Then
            Return Nothing
        End If
        Dim length As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim destination As Integer() = New Integer(length - 1) {}
        Dim bitmap As Bitmap = DirectCast(sourceBitmap.Clone, Bitmap)
        Dim rect As New Rectangle(0, 0, bitmap.Width, bitmap.Height)
        Dim bitmapdata As BitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat)
        Dim source As IntPtr = bitmapdata.Scan0
        Marshal.Copy(source, destination, 0, length)
        Dim i As Integer
        For i = 0 To length - 1
            Dim color As Color = Color.FromArgb(destination(i))
            Dim r As Integer = color.R
            Dim g As Integer = color.G
            Dim b As Integer = color.B
            Dim a As Integer = color.A
            r = (r + dR)
            g = (g + dG)
            b = (b + dB)
            If (r > &HFF) Then
                r = &HFF
            End If
            If (g > &HFF) Then
                g = &HFF
            End If
            If (b > &HFF) Then
                b = &HFF
            End If
            If (r < 0) Then
                r = 0
            End If
            If (g < 0) Then
                g = 0
            End If
            If (b < 0) Then
                b = 0
            End If
            If preserveAlfa Then
                destination(i) = Color.FromArgb(a, r, g, b).ToArgb
            Else
                destination(i) = Color.FromArgb(&HFF, r, g, b).ToArgb
            End If
        Next i
        Marshal.Copy(destination, 0, source, length)
        bitmap.UnlockBits(bitmapdata)
        Return bitmap
    End Function

    Public Shared Function AddWrinklesBitmap(ByVal sourceBitmap As Bitmap, ByVal wrinkleBitmap As Bitmap) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        If (wrinkleBitmap Is Nothing) Then
            Return DirectCast(sourceBitmap.Clone, Bitmap)
        End If
        Dim length As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim destination As Integer() = New Integer(length - 1) {}
        Dim numArray2 As Integer() = New Integer(length - 1) {}
        If (wrinkleBitmap Is Nothing) Then
            Return DirectCast(sourceBitmap.Clone, Bitmap)
        End If
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Marshal.Copy(bitmapdata.Scan0, destination, 0, length)
        sourceBitmap.UnlockBits(bitmapdata)
        If ((wrinkleBitmap.Width <> sourceBitmap.Width) OrElse (wrinkleBitmap.Height <> sourceBitmap.Height)) Then
            wrinkleBitmap = GraphicUtil.ResizeBitmap(wrinkleBitmap, sourceBitmap.Width, sourceBitmap.Height, InterpolationMode.Bilinear)
        End If
        Marshal.Copy(wrinkleBitmap.LockBits(rect, ImageLockMode.ReadWrite, wrinkleBitmap.PixelFormat).Scan0, numArray2, 0, length)
        wrinkleBitmap.UnlockBits(bitmapdata)
        Dim i As Integer
        For i = 0 To length - 1
            Dim color As Color = Color.FromArgb(destination(i))
            Dim num3 As Integer = (numArray2(i) And &HFF)
            Dim num4 As Integer = ((numArray2(i) And &HFF000000) >> &H18)
            Dim num5 As Integer = ((color.R * num3) \ &HFF)
            Dim num6 As Integer = ((color.G * num3) \ &HFF)
            Dim num7 As Integer = ((color.B * num3) \ &HFF)
            Dim num8 As Integer = ((color.A * num4) \ &HFF)
            destination(i) = ((((((num8 << 8) Or num5) << 8) Or num6) << 8) Or num7)
        Next i
        Dim bitmap As New Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format32bppArgb)
        rect = New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim data2 As BitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat)
        Dim ptr As IntPtr = data2.Scan0
        Marshal.Copy(destination, 0, ptr, length)
        bitmap.UnlockBits(data2)
        Return bitmap
    End Function

    Public Shared Function CanvasSizeBitmap(ByVal sourceBitmap As Bitmap, ByVal width As Integer, ByVal height As Integer) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        Dim image As New Bitmap(width, height, PixelFormat.Format32bppArgb)
        Dim graphics1 As Graphics = Graphics.FromImage(image)
        graphics1.DrawImage(sourceBitmap, New Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel)
        graphics1.Dispose()
        Return image
    End Function

    Public Shared Function CanvasSizeBitmapCentered(ByVal sourceBitmap As Bitmap, ByVal width As Integer, ByVal height As Integer) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        If ((width > sourceBitmap.Width) OrElse (height > sourceBitmap.Height)) Then
            Return Nothing
        End If
        Dim num As Integer = ((sourceBitmap.Width - width) \ 2)
        Dim bitmap As New Bitmap(width, height, sourceBitmap.PixelFormat)
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat)
        rect = New Rectangle(0, 0, bitmap.Width, bitmap.Height)
        Dim data2 As BitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat)
        Dim source As IntPtr = data2.Scan0
        Dim length As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim num3 As Integer = (bitmap.Width * bitmap.Height)
        Dim destination As Integer() = New Integer(length - 1) {}
        Dim numArray2 As Integer() = New Integer(num3 - 1) {}
        Marshal.Copy(bitmapdata.Scan0, destination, 0, length)
        Marshal.Copy(source, numArray2, 0, num3)
        Dim index As Integer = 0
        Dim num5 As Integer = ((((sourceBitmap.Height - height) \ 2) * sourceBitmap.Width) + num)
        Dim i As Integer
        For i = 0 To bitmap.Height - 1
            Dim j As Integer
            For j = 0 To bitmap.Width - 1
                numArray2(index) = destination(num5)
                index += 1
                num5 += 1
            Next j
            num5 = (num5 + (num * 2))
        Next i
        Marshal.Copy(numArray2, 0, source, num3)
        sourceBitmap.UnlockBits(bitmapdata)
        bitmap.UnlockBits(data2)
        Return bitmap
    End Function

    Public Shared Function ColorizeRGB(ByVal sourceBitmap As Bitmap, ByVal color1 As Color, ByVal color2 As Color, ByVal color3 As Color, ByVal preserveArmBand As Boolean) As Boolean
        If (sourceBitmap Is Nothing) Then
            Return False
        End If
        Dim colorArray As Color(,) = New Color(&H30 - 1, &H100 - 1) {}
        preserveArmBand = ((preserveArmBand AndAlso (sourceBitmap.Width = &H400)) AndAlso (sourceBitmap.Height = &H400))
        If preserveArmBand Then
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim i As Integer = &H3CF
            Do While (i <= &H3FE)
                num2 = 0
                Dim j As Integer = &H180
                Do While (j <= &H27F)
                    colorArray(num, num2) = sourceBitmap.GetPixel(j, i)
                    num2 += 1
                    j += 1
                Loop
                num += 1
                i += 1
            Loop
        End If
        Dim flag As Boolean = GraphicUtil.ColorizeRGB(sourceBitmap, color1, color2, color3, 0, sourceBitmap.Height)
        If (flag And preserveArmBand) Then
            Dim num5 As Integer = 0
            Dim num6 As Integer = 0
            Dim i As Integer = &H3CF
            Do While (i <= &H3FE)
                num6 = 0
                Dim j As Integer = &H180
                Do While (j <= &H27F)
                    sourceBitmap.SetPixel(j, i, colorArray(num5, num6))
                    num6 += 1
                    j += 1
                Loop
                num5 += 1
                i += 1
            Loop
        End If
        Return flag
    End Function

    Public Shared Function ColorizeRGB(ByVal sourceBitmap As Bitmap, ByVal color1 As Color, ByVal color2 As Color, ByVal color3 As Color, ByVal preserveArmBand As Boolean, ByVal CoeffBitmap As Bitmap) As Boolean
        If (sourceBitmap Is Nothing) Then
            Return False
        End If
        Dim colorArray As Color(,) = New Color(&H30 - 1, &H100 - 1) {}
        preserveArmBand = ((preserveArmBand AndAlso (sourceBitmap.Width = &H400)) AndAlso (sourceBitmap.Height = &H400))
        If preserveArmBand Then
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim i As Integer = &H3CF
            Do While (i <= &H3FE)
                num2 = 0
                Dim j As Integer = &H180
                Do While (j <= &H27F)
                    colorArray(num, num2) = sourceBitmap.GetPixel(j, i)
                    num2 += 1
                    j += 1
                Loop
                num += 1
                i += 1
            Loop
        End If
        Dim flag As Boolean = GraphicUtil.ColorizeRGB(sourceBitmap, color1, color2, color3, 0, sourceBitmap.Height, CoeffBitmap)
        If (flag And preserveArmBand) Then
            Dim num5 As Integer = 0
            Dim num6 As Integer = 0
            Dim i As Integer = &H3CF
            Do While (i <= &H3FE)
                num6 = 0
                Dim j As Integer = &H180
                Do While (j <= &H27F)
                    sourceBitmap.SetPixel(j, i, colorArray(num5, num6))
                    num6 += 1
                    j += 1
                Loop
                num5 += 1
                i += 1
            Loop
        End If
        Return flag
    End Function

    Public Shared Function ColorizeRGB(ByVal sourceBitmap As Bitmap, ByVal color1 As Color, ByVal color2 As Color, ByVal color3 As Color, ByVal firstRow As Integer, ByVal lastRow As Integer) As Boolean
        If (sourceBitmap Is Nothing) Then
            Return False
        End If
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Dim source As IntPtr = bitmapdata.Scan0
        Dim num As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim destination As Byte() = New Byte((num * 4) - 1) {}
        Marshal.Copy(source, destination, 0, (num * 4))
        Dim i As Integer
        For i = (firstRow * sourceBitmap.Width) To (lastRow * sourceBitmap.Width) - 1
            Dim num4 As Integer = destination((i * 4))
            Dim num3 As Integer = destination(((i * 4) + 1))
            Dim num2 As Integer = destination(((i * 4) + 2))
            Dim Alpha As Integer = destination(((i * 4) + 3))
            Dim num5 As Integer = ((((color1.R * num2) + (color2.R * num3)) + (color3.R * num4)) \ 255) '- (255 - Alpha)
            Dim num6 As Integer = ((((color1.G * num2) + (color2.G * num3)) + (color3.G * num4)) \ 255) '- (255 - Alpha)
            Dim num7 As Integer = ((((color1.B * num2) + (color2.B * num3)) + (color3.B * num4)) \ 255) '- (255 - Alpha)
            If (num5 > &HFF) Then
                num5 = &HFF
            ElseIf (num5 < 0) Then
                num5 = 0
            End If
            If (num6 > &HFF) Then
                num6 = &HFF
            ElseIf (num6 < 0) Then
                num6 = 0
            End If
            If (num7 > &HFF) Then
                num7 = &HFF
            ElseIf (num7 < 0) Then
                num7 = 0
            End If
            destination((i * 4)) = CByte(num7)
            destination(((i * 4) + 1)) = CByte(num6)
            destination(((i * 4) + 2)) = CByte(num5)
        Next i
        Marshal.Copy(destination, 0, source, (num * 4))
        sourceBitmap.UnlockBits(bitmapdata)
        Return True
    End Function

    Public Shared Function ColorizeRGB(ByVal sourceBitmap As Bitmap, ByVal color1 As Color, ByVal color2 As Color, ByVal color3 As Color, ByVal firstRow As Integer, ByVal lastRow As Integer, ByVal CoeffBitmap As Bitmap) As Boolean
        Dim bitmap1 As Bitmap = DirectCast(sourceBitmap.Clone, Bitmap)
        If (sourceBitmap Is Nothing) Then
            Return False
        End If
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Dim source As IntPtr = bitmapdata.Scan0
        Dim num As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim destination As Byte() = New Byte((num * 4) - 1) {}
        Marshal.Copy(source, destination, 0, (num * 4))

        If sourceBitmap.Size <> CoeffBitmap.Size Then
            CoeffBitmap = New Bitmap(CoeffBitmap, sourceBitmap.Width, sourceBitmap.Height)
        End If

        Dim id As Integer = firstRow * sourceBitmap.Width
        For j = firstRow To lastRow - 1
            For i = 0 To sourceBitmap.Width - 1
                'For i As Integer = (firstRow * sourceBitmap.Width) To (lastRow * sourceBitmap.Width) - 1


                'If width = sourceBitmap.Width And height < sourceBitmap.Height Then
                'height += 1
                'ElseIf height = sourceBitmap.Height Then
                'Else
                ' width += 1
                'End If

                Dim CoeffColor As Color = CoeffBitmap.GetPixel(i, j)
                Dim color_transparancy As Byte = CoeffColor.G
                'If color_transparancy = 0 Then color_transparancy = 1
                'If CoeffColor.ToArgb = 0 Then

                'End If


                Dim num4 As Integer = destination((id * 4))
                Dim num3 As Integer = destination(((id * 4) + 1))
                Dim num2 As Integer = destination(((id * 4) + 2))
                'If color_transparancy < 250 Then
                Dim num5 As Integer = ((((color1.R * num2) + (color2.R * num3)) + (color3.R * num4)) \ 255)
                Dim num6 As Integer = ((((color1.G * num2) + (color2.G * num3)) + (color3.G * num4)) \ 255)
                Dim num7 As Integer = ((((color1.B * num2) + (color2.B * num3)) + (color3.B * num4)) \ 255)
                'Dim num5 As Integer = ((((Math.Max(1, color1.R - color_transparancy) * num2) + (Math.Max(1, color2.R - color_transparancy) * num3)) + (Math.Max(1, color3.R - color_transparancy) * num4)) \ &HE2)
                'Dim num6 As Integer = ((((Math.Max(1, color1.G - color_transparancy) * num2) + (Math.Max(1, color2.G - color_transparancy) * num3)) + (Math.Max(1, color3.G - color_transparancy) * num4)) \ &HE2)
                'Dim num7 As Integer = ((((Math.Max(1, color1.B - color_transparancy) * num2) + (Math.Max(1, color2.B - color_transparancy) * num3)) + (Math.Max(1, color3.B - color_transparancy) * num4)) \ &HE2)
                If (num5 > &HFF) Then
                    num5 = &HFF
                ElseIf (num5 < 0) Then
                    num5 = 0
                End If
                If (num6 > &HFF) Then
                    num6 = &HFF
                ElseIf (num6 < 0) Then
                    num6 = 0
                End If
                If (num7 > &HFF) Then
                    num7 = &HFF
                ElseIf (num7 < 0) Then
                    num7 = 0
                End If
                destination((id * 4)) = CByte(num7)
                destination(((id * 4) + 1)) = CByte(num6)
                destination(((id * 4) + 2)) = CByte(num5)
                destination(((id * 4) + 3)) = 255 - color_transparancy



                'End If

                id += 1
            Next i
        Next j
        Marshal.Copy(destination, 0, source, (num * 4))
        sourceBitmap.UnlockBits(bitmapdata)
        'sourceBitmap = DrawOver(bitmap1, bitmap1)
        'Dim image As Bitmap = GraphicUtil.ResizeBitmap(upperBitmap, destRectangle.Width, destRectangle.Height, InterpolationMode.Bicubic)
        'If (Not image Is Nothing) Then
        'Dim bitmap1 As Bitmap = DirectCast(lowerBitmap.Clone, Bitmap)
        'Dim graphics1 As Graphics = Graphics.FromImage(sourceBitmap)
        'graphics1.DrawImage(bitmap1, 0, 0)
        'graphics1.Dispose()
        'Return bitmap1
        'End If

        Return True
    End Function

    Public Shared Function ColorizeWhite(ByVal srcBitmap As Bitmap, ByVal color As Color) As Bitmap
        If (srcBitmap Is Nothing) Then
            Return Nothing
        End If
        Dim r As Integer = color.R
        Dim g As Integer = color.G
        Dim b As Integer = color.B
        Dim i As Integer
        For i = 0 To srcBitmap.Width - 1
            Dim j As Integer
            For j = 0 To srcBitmap.Height - 1
                Dim pixel As Color = srcBitmap.GetPixel(i, j)
                If (pixel <> Color.FromArgb(0, 0, 0, 0)) Then
                    srcBitmap.SetPixel(i, j, Color.FromArgb(pixel.A, r, g, b))
                End If
            Next j
        Next i
        Return srcBitmap
    End Function

    Public Shared Function ColorTuning(ByVal variableBitmap As Bitmap, ByVal referenceBitmap As Bitmap, ByVal rect As Rectangle) As Bitmap
        Dim dominantColor As Color = GraphicUtil.GetDominantColor(variableBitmap, rect)
        Dim color2 As Color = GraphicUtil.GetDominantColor(referenceBitmap, rect)
        Dim deltaR As Integer = (color2.R - dominantColor.R)
        Dim deltaG As Integer = (color2.G - dominantColor.G)
        Dim deltaB As Integer = (color2.B - dominantColor.B)
        Dim bitmap As Bitmap = DirectCast(variableBitmap.Clone, Bitmap)
        GraphicUtil.AddColorOffset(bitmap, deltaR, deltaG, deltaB)
        Return bitmap
    End Function

    Public Shared Function ColorTuning(ByVal variableBitmap As Bitmap, ByVal variableRect As Rectangle, ByVal referenceBitmap As Bitmap, ByVal referenceRect As Rectangle) As Bitmap
        Dim dominantColor As Color = GraphicUtil.GetDominantColor(variableBitmap, variableRect)
        Dim color2 As Color = GraphicUtil.GetDominantColor(referenceBitmap, referenceRect)
        Dim deltaR As Integer = (color2.R - dominantColor.R)
        Dim deltaG As Integer = (color2.G - dominantColor.G)
        Dim deltaB As Integer = (color2.B - dominantColor.B)
        Dim bitmap As Bitmap = DirectCast(variableBitmap.Clone, Bitmap)
        GraphicUtil.AddColorOffset(bitmap, deltaR, deltaG, deltaB)
        Return bitmap
    End Function

    Public Shared Function CreateReferenceBitmap(ByVal sourceBitmap As Bitmap, ByVal c1 As Color, ByVal c2 As Color, ByVal c3 As Color) As Bitmap
        Dim r As Integer
        Dim g As Integer
        Dim b As Integer
        Dim num5 As Integer
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        Dim length As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim destination As Integer() = New Integer(length - 1) {}
        Dim hist As Integer() = New Integer(3 - 1) {}
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Marshal.Copy(bitmapdata.Scan0, destination, 0, length)
        sourceBitmap.UnlockBits(bitmapdata)
        Dim refColors As Color() = New Color() {c1, c2, c3}
        Dim rgb As Integer() = New Integer(3 - 1) {}
        Dim i As Integer
        For i = 0 To length - 1
            Dim tC As Color = Color.FromArgb(destination(i))
            num5 = 0
            r = 0
            g = 0
            b = 0
            If (tC = c1) Then
                r = &HE2
                g = 0
                b = 0
                num5 = &HFF
            ElseIf (tC = c2) Then
                r = 0
                g = &HE2
                b = 0
                num5 = &HFF
            ElseIf (tC = c3) Then
                r = 0
                g = 0
                b = &HE2
                num5 = &HFF
            ElseIf GraphicUtil.UseColorCombination(refColors, tC, rgb, True, hist) Then
                r = rgb(0)
                g = rgb(1)
                b = rgb(2)
                num5 = &HFF
            End If
            If (num5 = &HFF) Then
                destination(i) = ((((((num5 << 8) Or r) << 8) Or g) << 8) Or b)
            Else
                r = tC.R
                g = tC.G
                b = tC.B
                num5 = 0
                destination(i) = ((((((num5 << 8) Or r) << 8) Or g) << 8) Or b)
            End If
        Next i
        Dim j As Integer
        For j = 0 To length - 1
            Dim tC As Color = Color.FromArgb(destination(j))
            If (tC.A = 0) Then
                Dim num10 As Integer
                Dim num8 As Integer = (j \ sourceBitmap.Width)
                Dim num9 As Integer = (j - (num8 * sourceBitmap.Width))
                hist(2) = 0
                num10 = 0
                hist(0) = num10
                hist(1) = num10
                Dim k As Integer = -2
                Do While (k <= 2)
                    Dim m As Integer = -2
                    Do While (m <= 2)
                        Dim num13 As Integer = (num8 + k)
                        Dim num14 As Integer = (num9 + m)
                        If (((num13 >= 0) AndAlso (num13 < sourceBitmap.Height)) AndAlso ((num14 >= 0) AndAlso (num14 < sourceBitmap.Width))) Then
                            Dim color3 As Color = Color.FromArgb(destination(((num13 * sourceBitmap.Width) + num14)))
                            If (color3.A <> 0) Then
                                If (color3.R <> 0) Then
                                    hist(0) += 1
                                ElseIf (color3.G <> 0) Then
                                    hist(1) += 1
                                ElseIf (color3.B <> 0) Then
                                    hist(2) += 1
                                End If
                            End If
                        End If
                        m += 1
                    Loop
                    k += 1
                Loop
                GraphicUtil.UseColorCombination(refColors, tC, rgb, False, hist)
                r = rgb(0)
                g = rgb(1)
                b = rgb(2)
                num5 = &HFF
                destination(j) = ((((((num5 << 8) Or r) << 8) Or g) << 8) Or b)
            End If
        Next j
        Dim bitmap As New Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format32bppArgb)
        rect = New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim data2 As BitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat)
        Dim ptr As IntPtr = data2.Scan0
        Marshal.Copy(destination, 0, ptr, length)
        bitmap.UnlockBits(data2)
        Return bitmap
    End Function

    Public Shared Function CreateReferenceBitmap(ByVal sourceBitmap As Bitmap, ByVal c1 As Color, ByVal c2 As Color, ByVal c3 As Color, ByVal preserveArmBand As Boolean) As Bitmap
        Dim colorArray As Color(,) = New Color(48 - 1, 256 - 1) {}
        preserveArmBand = ((preserveArmBand AndAlso (sourceBitmap.Width = 1024)) AndAlso (sourceBitmap.Height = 1024))
        If preserveArmBand Then
            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim y As Integer = 975
            Do While (y <= 1022)
                num2 = 0
                Dim x As Integer = 384
                Do While True
                    If (x > 639) Then
                        num += 1
                        y += 1
                        Exit Do
                    End If
                    colorArray(num, num2) = sourceBitmap.GetPixel(x, y)
                    num2 += 1
                    x += 1
                Loop
            Loop
        End If
        Dim bitmap As Bitmap = GraphicUtil.CreateReferenceBitmap(sourceBitmap, c1, c2, c3)
        If ((Not bitmap Is Nothing) And preserveArmBand) Then
            Dim num5 As Integer = 0
            Dim num6 As Integer = 0
            Dim y As Integer = 975
            Do While (y <= 1022)
                num6 = 0
                Dim x As Integer = 384
                Do While True
                    If (x > 639) Then
                        num5 += 1
                        y += 1
                        Exit Do
                    End If
                    bitmap.SetPixel(x, y, colorArray(num5, num6))
                    num6 += 1
                    x += 1
                Loop
            Loop
        End If
        Return bitmap
    End Function

    Public Shared Function DrawOver(ByVal belowBitmap As Bitmap, ByVal overBitmap As Bitmap, ByVal PreserveAlpha As Boolean) As Bitmap
        Dim OriginalBitmap As Bitmap = Nothing
        If PreserveAlpha Then
            OriginalBitmap = belowBitmap.Clone
        End If
        RemoveAlfaChannel(belowBitmap)

        Dim graphics1 As Graphics = Graphics.FromImage(belowBitmap)
        graphics1.DrawImage(overBitmap, 0, 0, overBitmap.Width, overBitmap.Height)
        graphics1.Dispose()

        If PreserveAlpha Then
            FixAlphaChannel(belowBitmap, OriginalBitmap)
        End If

        Return belowBitmap
    End Function

    Public Shared Function FixAlphaChannel(ByVal sourceBitmap As Bitmap, ByVal AlphaBitmap As Bitmap) As Boolean
        Dim bitmap1 As Bitmap = DirectCast(sourceBitmap.Clone, Bitmap)
        If (sourceBitmap Is Nothing) Then
            Return False
        End If
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Dim source As IntPtr = bitmapdata.Scan0
        Dim num As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim destination As Byte() = New Byte((num * 4) - 1) {}
        Marshal.Copy(source, destination, 0, (num * 4))
        'Dim width As Integer = 0
        'Dim height As Integer = 0


        Dim id As Integer = 0 'firstRow * sourceBitmap.Width
        For j = 0 To sourceBitmap.Height - 1
            For i = 0 To sourceBitmap.Width - 1
                'For i As Integer = (firstRow * sourceBitmap.Width) To (lastRow * sourceBitmap.Width) - 1


                'If width = sourceBitmap.Width And height < sourceBitmap.Height Then
                'height += 1
                'ElseIf height = sourceBitmap.Height Then
                'Else
                ' width += 1
                'End If

                Dim MyColor As Color = AlphaBitmap.GetPixel(i, j)
                Dim Alpha As Byte = MyColor.A
                'If color_transparancy = 0 Then color_transparancy = 1
                'If CoeffColor.ToArgb = 0 Then

                'End If


                'Dim num4 As Integer = destination((id * 4))
                'Dim num3 As Integer = destination(((id * 4) + 1))
                'Dim num2 As Integer = destination(((id * 4) + 2))
                'Dim num5 As Integer = ((((color1.R * num2) + (color2.R * num3)) + (color3.R * num4)) \ 226) 'or 255?
                'Dim num6 As Integer = ((((color1.G * num2) + (color2.G * num3)) + (color3.G * num4)) \ 226)
                'Dim num7 As Integer = ((((color1.B * num2) + (color2.B * num3)) + (color3.B * num4)) \ 226)

                'If (num5 > &HFF) Then
                'num5 = &HFF
                'End If
                'If (num6 > &HFF) Then
                'num6 = &HFF
                'End If
                'If (num7 > &HFF) Then
                'num7 = &HFF
                'End If
                'destination((id * 4)) = CByte(num7)
                'destination(((id * 4) + 1)) = CByte(num6)
                'destination(((id * 4) + 2)) = CByte(num5)
                destination(((id * 4) + 3)) = Alpha



                'End If

                id += 1
            Next i
        Next j
        Marshal.Copy(destination, 0, source, (num * 4))
        sourceBitmap.UnlockBits(bitmapdata)
        'sourceBitmap = DrawOver(bitmap1, bitmap1)
        'Dim image As Bitmap = GraphicUtil.ResizeBitmap(upperBitmap, destRectangle.Width, destRectangle.Height, InterpolationMode.Bicubic)
        'If (Not image Is Nothing) Then
        'Dim bitmap1 As Bitmap = DirectCast(lowerBitmap.Clone, Bitmap)
        'Dim graphics1 As Graphics = Graphics.FromImage(sourceBitmap)
        'graphics1.DrawImage(bitmap1, 0, 0)
        'graphics1.Dispose()
        'Return bitmap1
        'End If

        Return True
    End Function

    Public Shared Function EmbossBitmap(ByVal sourceBitmap As Bitmap, ByVal embossingBitmap As Bitmap) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        If (embossingBitmap Is Nothing) Then
            Return DirectCast(sourceBitmap.Clone, Bitmap)
        End If
        Dim length As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim destination As Integer() = New Integer(length - 1) {}
        Dim numArray2 As Integer() = New Integer(length - 1) {}
        If (embossingBitmap Is Nothing) Then
            Return DirectCast(sourceBitmap.Clone, Bitmap)
        End If
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Marshal.Copy(bitmapdata.Scan0, destination, 0, length)
        sourceBitmap.UnlockBits(bitmapdata)
        If ((embossingBitmap.Width <> sourceBitmap.Width) OrElse (embossingBitmap.Height <> sourceBitmap.Height)) Then
            embossingBitmap = GraphicUtil.ResizeBitmap(embossingBitmap, sourceBitmap.Width, sourceBitmap.Height, InterpolationMode.Bilinear)
        End If
        Marshal.Copy(embossingBitmap.LockBits(rect, ImageLockMode.ReadWrite, embossingBitmap.PixelFormat).Scan0, numArray2, 0, length)
        embossingBitmap.UnlockBits(bitmapdata)
        Dim i As Integer
        For i = 0 To length - 1
            Dim color As Color = Color.FromArgb(destination(i))
            Dim num3 As Integer = ((&H7F - ((numArray2(i) And &HFF00) >> 8)) \ 2)
            Dim num4 As Integer = (color.R + num3)
            If (num4 > &HFF) Then
                num4 = &HFF
            End If
            If (num4 < 0) Then
                num4 = 0
            End If
            Dim num5 As Integer = (color.G + num3)
            If (num5 > &HFF) Then
                num5 = &HFF
            End If
            If (num5 < 0) Then
                num5 = 0
            End If
            Dim num6 As Integer = (color.B + num3)
            If (num6 > &HFF) Then
                num6 = &HFF
            End If
            If (num6 < 0) Then
                num6 = 0
            End If
            Dim a As Integer = color.A
            destination(i) = ((((((a << 8) Or num4) << 8) Or num5) << 8) Or num6)
        Next i
        Dim bitmap As New Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format32bppArgb)
        rect = New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim data2 As BitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat)
        Dim ptr As IntPtr = data2.Scan0
        Marshal.Copy(destination, 0, ptr, length)
        bitmap.UnlockBits(data2)
        Return bitmap
    End Function

    Public Shared Function Get32bitBitmap(ByVal sourceBitmap As Bitmap) As Bitmap
        Dim image As New Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format32bppArgb)
        Dim graphics1 As Graphics = Graphics.FromImage(image)
        graphics1.DrawImage(sourceBitmap, 0, 0, sourceBitmap.Width, sourceBitmap.Height)
        graphics1.Dispose()
        Return image
    End Function

    Public Shared Function Get32bitPBitmap(ByVal sourceBitmap As Bitmap) As Bitmap
        Dim image As New Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format32bppPArgb)
        Dim graphics1 As Graphics = Graphics.FromImage(image)
        graphics1.DrawImage(sourceBitmap, 0, 0, sourceBitmap.Width, sourceBitmap.Height)
        graphics1.Dispose()
        Return image
    End Function

    Public Shared Function GetAlfaFromChannel(ByVal sourceBitmap As Bitmap, ByVal alfaBitmap As Bitmap, ByVal channel As Integer) As Boolean
        If ((sourceBitmap.Width <> alfaBitmap.Width) OrElse (sourceBitmap.Height <> alfaBitmap.Height)) Then
            Return False
        End If
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        rect = New Rectangle(0, 0, alfaBitmap.Width, alfaBitmap.Height)
        Dim data2 As BitmapData = alfaBitmap.LockBits(rect, ImageLockMode.ReadOnly, alfaBitmap.PixelFormat)
        Dim source As IntPtr = bitmapdata.Scan0
        Dim num As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim destination As Byte() = New Byte((num * 4) - 1) {}
        Dim buffer2 As Byte() = New Byte((num * 4) - 1) {}
        Marshal.Copy(source, destination, 0, (num * 4))
        Marshal.Copy(data2.Scan0, buffer2, 0, (num * 4))
        Dim i As Integer = 3
        Do While (i < (num * 4))
            destination(i) = buffer2((i - channel))
            i = (i + 4)
        Loop
        Marshal.Copy(destination, 0, source, (num * 4))
        sourceBitmap.UnlockBits(bitmapdata)
        alfaBitmap.UnlockBits(data2)
        Return True
    End Function

    Public Shared Function GetDominantColor(ByVal bitmap As Bitmap, ByVal rectangle As Rectangle) As Color
        Dim numArray As Integer() = New Integer(&H100 - 1) {}
        Dim i As Integer
        For i = rectangle.X To (rectangle.X + rectangle.Width) - 1
            Dim m As Integer
            For m = rectangle.Y To (rectangle.Y + rectangle.Height) - 1
                Dim pixel As Color = bitmap.GetPixel(i, m)
                numArray(pixel.R) += 1
            Next m
        Next i
        Dim num As Integer = -1
        Dim red As Integer = 0
        Dim green As Integer = 0
        Dim blue As Integer = 0
        Dim j As Integer
        For j = 0 To &H100 - 1
            If (numArray(j) > num) Then
                num = numArray(j)
                red = j
            End If
        Next j
        Dim num5 As Integer = 0
        Dim num6 As Integer = 0
        Dim k As Integer
        For k = rectangle.X To (rectangle.X + rectangle.Width) - 1
            Dim m As Integer
            For m = rectangle.Y To (rectangle.Y + rectangle.Height) - 1
                Dim pixel As Color = bitmap.GetPixel(k, m)
                If (pixel.R = red) Then
                    num5 = (num5 + pixel.G)
                    num6 = (num6 + pixel.B)
                End If
            Next m
        Next k
        green = (num5 \ num)
        blue = (num6 \ num)
        Return Color.FromArgb(&HFF, red, green, blue)
    End Function

    Public Shared Sub LoadPictureImage(ByVal picture As PictureBox, ByVal bitmap As Bitmap)
        If (bitmap Is Nothing) Then
            picture.Image = bitmap
        ElseIf ((picture.Width = bitmap.Width) AndAlso (picture.Height = bitmap.Height)) Then
            picture.Image = bitmap
        Else
            picture.Image = GraphicUtil.RemapBitmap(bitmap, picture.Width, picture.Height)
        End If
    End Sub

    Public Shared Function MakeAutoTransparent(ByVal bitmap As Bitmap) As Bitmap
        Dim pixel As Color = bitmap.GetPixel(0, 0)
        bitmap.MakeTransparent(pixel)
        Return bitmap
    End Function

    Public Shared Function MultiplyBitmap(ByVal sourceBitmap As Bitmap, ByVal multBitmap As Bitmap) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        If (multBitmap Is Nothing) Then
            Return DirectCast(sourceBitmap.Clone, Bitmap)
        End If
        Dim length As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim destination As Integer() = New Integer(length - 1) {}
        Dim numArray2 As Integer() = New Integer(length - 1) {}
        If (multBitmap Is Nothing) Then
            Return DirectCast(sourceBitmap.Clone, Bitmap)
        End If
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Marshal.Copy(bitmapdata.Scan0, destination, 0, length)
        sourceBitmap.UnlockBits(bitmapdata)
        If ((multBitmap.Width <> sourceBitmap.Width) OrElse (multBitmap.Height <> sourceBitmap.Height)) Then
            multBitmap = GraphicUtil.ResizeBitmap(multBitmap, sourceBitmap.Width, sourceBitmap.Height, InterpolationMode.Bilinear)
        End If
        Marshal.Copy(multBitmap.LockBits(rect, ImageLockMode.ReadWrite, multBitmap.PixelFormat).Scan0, numArray2, 0, length)
        multBitmap.UnlockBits(bitmapdata)
        Dim i As Integer
        For i = 0 To length - 1
            Dim color As Color = Color.FromArgb(destination(i))
            Dim num3 As Integer = (numArray2(i) And &HFF)
            Dim num4 As Integer = ((numArray2(i) And &HFF000000) >> &H18)
            Dim num5 As Integer = ((color.R * num3) \ &HFF)
            Dim num6 As Integer = ((color.G * num3) \ &HFF)
            Dim num7 As Integer = ((color.B * num3) \ &HFF)
            Dim num8 As Integer = ((color.A * num4) \ &HFF)
            destination(i) = ((((((num8 << 8) Or num5) << 8) Or num6) << 8) Or num7)
        Next i
        Dim bitmap As New Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format32bppArgb)
        rect = New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim data2 As BitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat)
        Dim ptr As IntPtr = data2.Scan0
        Marshal.Copy(destination, 0, ptr, length)
        bitmap.UnlockBits(data2)
        Return bitmap
    End Function

    Public Shared Function MultiplyColorToBitmap(ByVal sourceBitmap As Bitmap, ByVal color As Color, ByVal divisor As Integer, ByVal preserveAlfa As Boolean) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        Dim length As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim destination As Integer() = New Integer(length - 1) {}
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Marshal.Copy(bitmapdata.Scan0, destination, 0, length)
        sourceBitmap.UnlockBits(bitmapdata)
        Dim i As Integer
        For i = 0 To length - 1
            Dim color2 As Color = Color.FromArgb(destination(i))
            Dim a As Integer = color2.A
            Dim num4 As Integer = ((color.R * color2.R) \ divisor)
            Dim num5 As Integer = ((color.G * color2.G) \ divisor)
            Dim num6 As Integer = ((color.B * color2.B) \ divisor)
            If (num4 > &HFF) Then
                num4 = &HFF
            End If
            If (num5 > &HFF) Then
                num5 = &HFF
            End If
            If (num6 > &HFF) Then
                num6 = &HFF
            End If
            If Not preserveAlfa Then
                a = &HFF
            End If
            destination(i) = ((((((a << 8) Or num4) << 8) Or num5) << 8) Or num6)
        Next i
        Dim bitmap As New Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format32bppArgb)
        rect = New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim data2 As BitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat)
        Dim ptr As IntPtr = data2.Scan0
        Marshal.Copy(destination, 0, ptr, length)
        bitmap.UnlockBits(data2)
        Return bitmap
    End Function

    Public Shared Function Overlap(ByVal lowerBitmap As Bitmap, ByVal upperBitmap As Bitmap, ByVal destRectangle As Rectangle) As Bitmap
        If ((lowerBitmap Is Nothing) AndAlso (upperBitmap Is Nothing)) Then
            Return Nothing
        End If
        If (lowerBitmap Is Nothing) Then
            Return DirectCast(upperBitmap.Clone, Bitmap)
        End If
        If (upperBitmap Is Nothing) Then
            Return DirectCast(lowerBitmap.Clone, Bitmap)
        End If
        Dim image As Bitmap = GraphicUtil.ResizeBitmap(upperBitmap, destRectangle.Width, destRectangle.Height, InterpolationMode.Bicubic)
        If (Not image Is Nothing) Then
            Dim bitmap1 As Bitmap = DirectCast(lowerBitmap.Clone, Bitmap)
            Dim graphics1 As Graphics = Graphics.FromImage(bitmap1)
            graphics1.DrawImage(image, destRectangle.Left, destRectangle.Top)
            graphics1.Dispose()
            Return bitmap1
        End If
        Return lowerBitmap
    End Function

    Public Shared Function PrepareToColorize(ByVal sourceBitmap As Bitmap, ByVal firstRow As Integer, ByVal lastRow As Integer) As Boolean
        If (sourceBitmap Is Nothing) Then
            Return False
        End If
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Dim source As IntPtr = bitmapdata.Scan0
        Dim num As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim destination As Byte() = New Byte((num * 4) - 1) {}
        Marshal.Copy(source, destination, 0, (num * 4))
        Dim i As Integer
        For i = (firstRow * sourceBitmap.Width) To (lastRow * sourceBitmap.Width) - 1
            Dim num4 As Integer = destination((i * 4))
            Dim num3 As Integer = destination(((i * 4) + 1))
            Dim num2 As Integer = destination(((i * 4) + 2))
            Do While (((num2 + num3) + num4) > &HE2)
                If (num2 > 0) Then
                    num2 -= 1
                End If
                If (num3 > 0) Then
                    num3 -= 1
                End If
                If (num4 > 0) Then
                    num4 -= 1
                End If
            Loop
            destination((i * 4)) = CByte(num4)
            destination(((i * 4) + 1)) = CByte(num3)
            destination(((i * 4) + 2)) = CByte(num2)
        Next i
        Marshal.Copy(destination, 0, source, (num * 4))
        sourceBitmap.UnlockBits(bitmapdata)
        Return True
    End Function

    Public Shared Function ReduceBitmap(ByVal srcBitmap As Bitmap) As Bitmap
        Dim width As Integer = srcBitmap.Width
        Dim height As Integer = srcBitmap.Height
        If ((width * height) = 0) Then
            Return Nothing
        End If
        width = (width \ 2)
        height = (height \ 2)
        If (width = 0) Then
            width = 1
        End If
        If (height = 0) Then
            height = 1
        End If
        Return GraphicUtil.ResizeBitmap(srcBitmap, width, height, InterpolationMode.HighQualityBicubic)
    End Function

    Public Shared Function RemapBitmap(ByVal srcBitmap As Bitmap, ByVal destWidth As Integer, ByVal destHeight As Integer) As Bitmap
        Dim bitmap As New Bitmap(destWidth, destHeight, PixelFormat.Format32bppArgb)
        Dim height As Integer = srcBitmap.Height
        Dim num2 As Single = (CSng(srcBitmap.Width) \ CSng(destWidth))
        Dim num3 As Single = (CSng(height) \ CSng(destHeight))
        Dim i As Integer
        For i = 0 To destWidth - 1
            Dim j As Integer
            For j = 0 To destHeight - 1
                bitmap.SetPixel(i, j, GraphicUtil.RemapPixel(srcBitmap, (i * num2), (j * num3)))
            Next j
        Next i
        Return bitmap
    End Function

    Private Shared Function RemapPixel(ByVal srcBitmap As Bitmap, ByVal x As Single, ByVal y As Single) As Color
        Dim num As Integer = CInt(Math.Floor(CDbl(x)))
        Dim num2 As Integer = CInt(Math.Floor(CDbl(y)))
        Dim num3 As Integer = If((num < srcBitmap.Width), num, (srcBitmap.Width - 1))
        Dim num4 As Integer = If(((num + 1) < srcBitmap.Width), (num + 1), (srcBitmap.Width - 1))
        Dim num5 As Integer = If((num2 < srcBitmap.Height), num2, (srcBitmap.Height - 1))
        Dim num6 As Integer = If(((num2 + 1) < srcBitmap.Height), (num2 + 1), (srcBitmap.Height - 1))
        Dim pixel As Color = srcBitmap.GetPixel(num3, num5)
        Dim color2 As Color = srcBitmap.GetPixel(num4, num5)
        Dim color3 As Color = srcBitmap.GetPixel(num3, num6)
        Dim color4 As Color = srcBitmap.GetPixel(num4, num6)
        Dim num7 As Single = (x - num)
        Dim num8 As Single = (y - num2)
        Dim num9 As Single = ((1.0! - num7) * (1.0! - num8))
        Dim num10 As Single = (num7 * (1.0! - num8))
        Dim num11 As Single = ((1.0! - num7) * num8)
        Dim num12 As Single = (num7 * num8)
        Dim red As Integer = CInt(((((pixel.R * num9) + (color2.R * num10)) + (color3.R * num11)) + (color4.R * num12)))
        Dim green As Integer = CInt(((((pixel.G * num9) + (color2.G * num10)) + (color3.G * num11)) + (color4.G * num12)))
        Dim blue As Integer = CInt(((((pixel.B * num9) + (color2.B * num10)) + (color3.B * num11)) + (color4.B * num12)))
        Return Color.FromArgb(CInt(((((pixel.A * num9) + (color2.A * num10)) + (color3.A * num11)) + (color4.A * num12))), red, green, blue)
    End Function

    Public Shared Sub RemapRectangle(ByVal srcBitmap As Bitmap, ByVal srcRect As Rectangle, ByVal destBitmap As Bitmap, ByVal destRect As Rectangle)
        Dim graphics1 As Graphics = Graphics.FromImage(destBitmap)
        graphics1.InterpolationMode = InterpolationMode.HighQualityBicubic
        graphics1.DrawImage(srcBitmap, destRect, srcRect, GraphicsUnit.Pixel)
        graphics1.Dispose()
    End Sub

    Public Shared Function RemoveAlfaChannel(ByVal sourceBitmap As Bitmap) As Boolean
        If (sourceBitmap Is Nothing) OrElse (sourceBitmap.PixelFormat = PixelFormat.Format24bppRgb) Then
            Return False
        End If
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Dim source As IntPtr = bitmapdata.Scan0
        Dim num As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim destination As Byte() = New Byte((num * 4) - 1) {}
        Marshal.Copy(source, destination, 0, (num * 4))
        Dim i As Integer = 3
        Do While (i < (num * 4))
            destination(i) = &HFF
            i = (i + 4)
        Loop
        Marshal.Copy(destination, 0, source, (num * 4))
        sourceBitmap.UnlockBits(bitmapdata)
        Return True
    End Function

    Public Shared Function SetAlfaChannel(ByVal sourceBitmap As Bitmap, ByVal AlphaValue As Byte) As Boolean
        If (sourceBitmap Is Nothing) Then
            Return False
        End If
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat)
        Dim source As IntPtr = bitmapdata.Scan0
        Dim num As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim destination As Byte() = New Byte((num * 4) - 1) {}
        Marshal.Copy(source, destination, 0, (num * 4))
        Dim i As Integer = 3
        Do While (i < (num * 4))
            destination(i) = AlphaValue
            i = (i + 4)
        Loop
        Marshal.Copy(destination, 0, source, (num * 4))
        sourceBitmap.UnlockBits(bitmapdata)
        Return True
    End Function

    Public Shared Function ResizeBitmap(ByVal sourceBitmap As Bitmap, ByVal width As Integer, ByVal height As Integer, ByVal interpolationMode As InterpolationMode) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        If (width < 0) Then
            width = -width
        End If
        If (height < 0) Then
            height = -height
        End If
        If ((width = 0) OrElse (height = 0)) Then
            Return Nothing
        End If
        If ((sourceBitmap.Width = width) AndAlso (sourceBitmap.Height = height)) Then
            Return sourceBitmap
        End If
        Dim image As New Bitmap(width, height, PixelFormat.Format32bppArgb)
        Dim graphics1 As Graphics = Graphics.FromImage(image)
        graphics1.InterpolationMode = interpolationMode
        graphics1.DrawImage(sourceBitmap, New Rectangle(0, 0, width, height), 0, 0, sourceBitmap.Width, sourceBitmap.Height, GraphicsUnit.Pixel)
        graphics1.Dispose()
        Return image
    End Function

    Public Shared Function SubSampleBitmap(ByVal sourceBitmap As Bitmap, ByVal xStep As Integer, ByVal yStep As Integer) As Bitmap
        If (sourceBitmap Is Nothing) Then
            Return Nothing
        End If
        If ((xStep <= 0) OrElse (yStep <= 0)) Then
            Return Nothing
        End If
        If ((xStep = 1) AndAlso (yStep = 1)) Then
            Return DirectCast(sourceBitmap.Clone, Bitmap)
        End If
        Dim bitmap As New Bitmap((sourceBitmap.Width \ xStep), (sourceBitmap.Height \ yStep), sourceBitmap.PixelFormat)
        Dim rect As New Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height)
        Dim bitmapdata As BitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, sourceBitmap.PixelFormat)
        rect = New Rectangle(0, 0, bitmap.Width, bitmap.Height)
        Dim data2 As BitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat)
        Dim source As IntPtr = data2.Scan0
        Dim length As Integer = (sourceBitmap.Width * sourceBitmap.Height)
        Dim num2 As Integer = (bitmap.Width * bitmap.Height)
        Dim destination As Integer() = New Integer(length - 1) {}
        Dim numArray2 As Integer() = New Integer(num2 - 1) {}
        Marshal.Copy(bitmapdata.Scan0, destination, 0, length)
        Marshal.Copy(source, numArray2, 0, num2)
        Dim index As Integer = 0
        Dim num4 As Integer = 0
        Dim i As Integer
        For i = 0 To bitmap.Height - 1
            Dim j As Integer
            For j = 0 To bitmap.Width - 1
                numArray2(index) = destination(num4)
                index += 1
                num4 = (num4 + xStep)
            Next j
            num4 = (num4 + ((yStep - 1) * sourceBitmap.Width))
        Next i
        Marshal.Copy(numArray2, 0, source, num2)
        sourceBitmap.UnlockBits(bitmapdata)
        bitmap.UnlockBits(data2)
        Return bitmap
    End Function

    Private Shared Function UseColorCombination(ByVal refColors As Color(), ByVal tC As Color, ByRef rgb As Integer(), ByVal useOneColor As Boolean, ByVal hist As Integer()) As Boolean
        Dim num4 As Integer
        Dim numArray As Integer() = New Integer(3 - 1) {}
        rgb(2) = 0
        num4 = 0
        rgb(0) = num4
        rgb(1) = num4
        Dim index As Integer = -1
        Dim num2 As Integer = -1
        Dim num3 As Integer = -1
        If (((hist(0) + hist(1)) + hist(2)) <> 0) Then
            If (((hist(0) > 0) AndAlso (hist(1) = 0)) AndAlso (hist(2) = 0)) Then
                rgb(0) = GraphicUtil.UseOneColor(refColors(0), tC)
                Return True
            End If
            If (((hist(1) > 0) AndAlso (hist(0) = 0)) AndAlso (hist(2) = 0)) Then
                rgb(1) = GraphicUtil.UseOneColor(refColors(1), tC)
                Return True
            End If
            If (((hist(2) > 0) AndAlso (hist(0) = 0)) AndAlso (hist(1) = 0)) Then
                rgb(2) = GraphicUtil.UseOneColor(refColors(2), tC)
                Return True
            End If
            If Not useOneColor Then
                If ((hist(0) >= hist(2)) AndAlso (hist(1) >= hist(2))) Then
                    If GraphicUtil.UseTwoColors(refColors(0), refColors(1), tC, rgb(0), rgb(1)) Then
                        Return True
                    End If
                ElseIf ((hist(0) >= hist(1)) AndAlso (hist(2) >= hist(1))) Then
                    If GraphicUtil.UseTwoColors(refColors(0), refColors(2), tC, rgb(0), rgb(2)) Then
                        Return True
                    End If
                ElseIf (((hist(1) >= hist(0)) AndAlso (hist(2) >= hist(0))) AndAlso GraphicUtil.UseTwoColors(refColors(1), refColors(2), tC, rgb(1), rgb(2))) Then
                    Return True
                End If
            End If
        End If
        Dim i As Integer
        For i = 0 To 3 - 1
            rgb(i) = 0
            numArray(i) = ((((tC.R - refColors(i).R) * (tC.R - refColors(i).R)) + ((tC.G - refColors(i).G) * (tC.G - refColors(i).G))) + ((tC.B - refColors(i).B) * (tC.B - refColors(i).B)))
            If (refColors(i).A = 0) Then
                numArray(i) = &H7FFFFFFF
            End If
        Next i
        If ((numArray(0) <= numArray(1)) AndAlso (numArray(0) <= numArray(2))) Then
            index = 0
            If (numArray(1) < numArray(2)) Then
                num2 = 1
                num3 = 2
            Else
                num2 = 2
                num3 = 1
            End If
        ElseIf ((numArray(1) <= numArray(0)) AndAlso (numArray(1) <= numArray(2))) Then
            index = 1
            If (numArray(0) < numArray(2)) Then
                num2 = 0
                num3 = 2
            Else
                num2 = 2
                num3 = 0
            End If
        Else
            index = 2
            If (numArray(0) < numArray(1)) Then
                num2 = 0
                num3 = 1
            Else
                num2 = 1
                num3 = 0
            End If
        End If
        If ((numArray(index) * 8) < numArray(num2)) Then
            rgb(index) = GraphicUtil.UseOneColor(refColors(index), tC)
            Return True
        End If
        If useOneColor Then
            Return False
        End If
        If (((numArray(index) * 8) <= numArray(num2)) OrElse Not GraphicUtil.UseTwoColors(refColors(index), refColors(num2), tC, rgb(index), rgb(num2))) Then
            If (((numArray(index) * 8) > numArray(num3)) AndAlso GraphicUtil.UseTwoColors(refColors(index), refColors(num3), tC, rgb(index), rgb(num3))) Then
                Return True
            End If
            rgb(index) = GraphicUtil.UseOneColor(refColors(index), tC)
        End If
        Return True
    End Function

    Private Shared Function UseOneColor(ByVal refColor As Color, ByVal targetColor As Color) As Integer
        Dim num3 As Integer
        Dim num As Integer = (((refColor.R + refColor.G) + refColor.B) \ 3)
        Dim num2 As Integer = (((targetColor.R + targetColor.G) + targetColor.B) \ 3)
        If (num2 > num) Then
            num3 = (&HE2 + ((30 * (num2 - num)) \ (&HFF - num)))
        Else
            num3 = ((&HE2 * num2) \ num)
        End If
        If (num3 < 0) Then
            num3 = 0
        End If
        If (num3 > &HFF) Then
            num3 = &HFF
        End If
        Return num3
    End Function

    Private Shared Function UseTwoColors(ByVal c1 As Color, ByVal c2 As Color, ByVal tc As Color, <Out> ByRef w1 As Integer, <Out> ByRef w2 As Integer) As Boolean
        Dim num As Integer = Math.Abs(CInt((c1.R - c2.R)))
        Dim num2 As Integer = Math.Abs(CInt((c1.G - c2.G)))
        Dim num3 As Integer = Math.Abs(CInt((c1.B - c2.B)))
        If ((num >= num2) AndAlso (num >= num3)) Then
            If (c1.R < c2.R) Then
                w1 = (((c2.R - tc.R) * &HE2) \ num)
                w2 = (((tc.R - c1.R) * &HE2) \ num)
            Else
                w1 = (((tc.R - c2.R) * &HE2) \ num)
                w2 = (((c1.R - tc.R) * &HE2) \ num)
            End If
        ElseIf ((num2 >= num) AndAlso (num2 >= num3)) Then
            If (c1.G < c2.G) Then
                w1 = (((c2.G - tc.G) * &HE2) \ num2)
                w2 = (((tc.G - c1.G) * &HE2) \ num2)
            Else
                w1 = (((tc.G - c2.G) * &HE2) \ num2)
                w2 = (((c1.G - tc.G) * &HE2) \ num2)
            End If
        ElseIf (c1.B < c2.B) Then
            w1 = (((c2.B - tc.B) * &HE2) \ num3)
            w2 = (((tc.B - c1.B) * &HE2) \ num3)
        Else
            w1 = (((tc.B - c2.B) * &HE2) \ num3)
            w2 = (((c1.B - tc.B) * &HE2) \ num3)
        End If
        If (w1 < 0) Then
            w1 = 0
        End If
        If (w1 > &HFF) Then
            w1 = &HFF
        End If
        If (w2 < 0) Then
            w2 = 0
        End If
        If (w2 > &HFF) Then
            w2 = &HFF
        End If
        Return ((w1 >= 0) AndAlso (w2 >= 0))
    End Function

    Public Shared Function GetTextureSize(ByVal m_width As Integer, ByVal m_height As Integer, ByVal m_TextureFormat As ETextureFormat) As Integer
        Dim m_Size As Integer

        Select Case m_TextureFormat
            Case ETextureFormat.BC1, ETextureFormat.BC4
                If m_width < 4 Then
                    m_width = 4
                End If
                If m_height < 4 Then
                    m_height = 4
                End If
                m_Size = m_width * m_height \ 2
            Case ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC5
                If m_width < 4 Then
                    m_width = 4
                End If
                If m_height < 4 Then
                    m_height = 4
                End If
                m_Size = m_width * m_height
            Case ETextureFormat.B8G8R8A8
                If m_width < 1 Then
                    m_width = 1
                End If
                If m_height < 1 Then
                    m_height = 1
                End If
                m_Size = m_width * m_height * 4
            Case ETextureFormat.B8G8R8  'no samples found, confirmed with gimp
                If m_width < 1 Then
                    m_width = 1
                End If
                If m_height < 1 Then
                    m_height = 1
                End If
                m_Size = m_width * m_height * 3
            Case ETextureFormat.L8
                If m_width < 1 Then
                    m_width = 1
                End If
                If m_height < 1 Then
                    m_height = 1
                End If
                m_Size = m_width * m_height
            Case ETextureFormat.L8A8  'FO3 (F11) jersey_bump texts
                If m_width < 1 Then
                    m_width = 1
                End If
                If m_height < 1 Then
                    m_height = 1
                End If
                m_Size = m_width * m_height * 2
            Case ETextureFormat.B4G4R4A4      'no samples found, confirmed with gimp
                If m_width < 1 Then
                    m_width = 1
                End If
                If m_height < 1 Then
                    m_height = 1
                End If
                m_Size = m_width * m_height * 2

            Case ETextureFormat.B5G6R5, ETextureFormat.B5G5R5A1  'no samples found, confirmed with gimp
                If m_width < 1 Then
                    m_width = 1
                End If
                If m_height < 1 Then
                    m_height = 1
                End If
                m_Size = m_width * m_height * 2

            Case ETextureFormat.R32G32B32A32Float
                If m_width < 1 Then
                    m_width = 1
                End If
                If m_height < 1 Then
                    m_height = 1
                End If
                m_Size = m_width * m_height * 16

            Case ETextureFormat.BC6H_UF16
                If m_width < 4 Then
                    m_width = 4
                End If
                If m_height < 4 Then
                    m_height = 4
                End If
                m_Size = m_width * m_height

            Case ETextureFormat.BC7
                If m_width < 4 Then
                    m_width = 4
                End If
                If m_height < 4 Then
                    m_height = 4
                End If
                m_Size = m_width * m_height

                'ETextureFormat.RGBA,   ETextureFormat.BIT8, ETextureFormat.DXT1NORMAL
                'no samples found
            Case Else
                m_Size = m_width * m_height
                MsgBox("unknown format")
        End Select

        Return m_Size
    End Function

    Public Shared Function GetTexturePitch(ByVal m_width As Integer, ByVal m_TextureFormat As ETextureFormat) As Integer
        'https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide
        'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/page-2#post-6600051

        Dim m_Pitch As Integer

        'pitch
        Select Case m_TextureFormat
            Case ETextureFormat.BC1, ETextureFormat.BC4
                If m_width < 4 Then
                    m_width = 4
                End If
                m_Pitch = Math.Max(1UI, ((m_width + 3) \ 4)) * 8
            Case ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC5
                If m_width < 4 Then
                    m_width = 4
                End If
                m_Pitch = Math.Max(1UI, ((m_width + 3) \ 4)) * 16
            Case ETextureFormat.B8G8R8A8
                If m_width < 1 Then
                    m_width = 1
                End If
                Dim bitsperpixel As Integer = 32
                m_Pitch = (m_width * bitsperpixel + 7) \ 8
            Case ETextureFormat.B8G8R8  'no samples found, experimental (pitch from microsoft docs)
                If m_width < 1 Then
                    m_width = 1
                End If
                Dim bitsperpixel As Integer = 24
                m_Pitch = (m_width * bitsperpixel + 7) \ 8
            Case ETextureFormat.L8
                If m_width < 1 Then
                    m_width = 1
                End If
                Dim bitsperpixel As Integer = 8
                m_Pitch = (m_width * bitsperpixel + 7) \ 8
            Case ETextureFormat.L8A8
                If m_width < 1 Then
                    m_width = 1
                End If
                Dim bitsperpixel As Integer = 16
                m_Pitch = (m_width * bitsperpixel + 7) \ 8
            Case ETextureFormat.B5G6R5, ETextureFormat.B4G4R4A4, ETextureFormat.B5G5R5A1  'no samples found, experimental
                If m_width < 1 Then
                    m_width = 1
                End If
                Dim bitsperpixel As Integer = 16
                m_Pitch = (m_width * bitsperpixel + 7) \ 8

            Case ETextureFormat.R32G32B32A32Float
                If m_width < 1 Then
                    m_width = 1
                End If
                Dim bitsperpixel As Integer = 128
                m_Pitch = (m_width * bitsperpixel + 7) \ 8

                'ETextureFormat.RGBA, ETextureFormat.BIT8, 
                'no samples found

            Case ETextureFormat.BC6H_UF16
                If m_width < 4 Then
                    m_width = 4
                End If
                m_Pitch = Math.Max(1UI, ((m_width + 3) \ 4)) * 16

            Case ETextureFormat.BC7
                If m_width < 4 Then
                    m_width = 4
                End If
                m_Pitch = Math.Max(1UI, ((m_width + 3) \ 4)) * 16

            Case Else
                MsgBox("unknown format")

        End Select

        Return m_Pitch
    End Function

End Class


