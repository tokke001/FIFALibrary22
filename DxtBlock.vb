Imports System.Drawing

Public Class DxtBlock
    ' Methods
    Public Sub New(ByVal dxtType As Integer, ByVal SwapEndian As Boolean)
        'Me.m_SwapEndian = SwapEndian
        Me.m_ColorLut = New Integer(4 - 1, 4 - 1) {}
        Me.m_AlfaLut = New Integer(4 - 1, 4 - 1) {}
        Me.m_Colors = New Color(4 - 1, 4 - 1) {}
        Me.m_Alfa = New Integer(4 - 1, 4 - 1) {}
        Me.Init(dxtType)
    End Sub

    Public Sub New(ByVal dxtType As Byte, ByVal br As FileReader, ByVal SwapEndian As Boolean)
        'Me.m_SwapEndian = SwapEndian
        Me.m_DxtType = DirectCast(dxtType, ETextureFormat)
        Me.m_ColorLut = New Integer(4 - 1, 4 - 1) {}
        Me.m_Colors = New Color(4 - 1, 4 - 1) {}
        Me.Load(br)
    End Sub

    Private Sub CleanLuts()
        For i = 0 To 4 - 1
            For j = 0 To 4 - 1
                Me.m_AlfaLut(i, j) = 0
                Me.m_ColorLut(i, j) = 0
            Next j
        Next i
    End Sub

    Private Function ComputeInterpolatedAlfa(ByVal alfa0 As Integer, ByVal alfa1 As Integer) As Integer()
        Dim numArray As Integer() = New Integer(8 - 1) {}
        numArray(0) = alfa0
        numArray(1) = alfa1
        For i = 2 To 8 - 1
            Dim num2 As Integer = (8 - i)
            Dim num3 As Integer = (i - 1)
            numArray(i) = (((num2 * alfa0) + (num3 * alfa1)) \ 7)
        Next i
        Return numArray
    End Function

    Private Function ComputeInterpolatedRgb(ByVal col0 As Color, ByVal col1 As Color, ByVal nColors As Integer) As Integer(,)
        Dim numArray As Integer(,) = New Integer(4 - 1, 3 - 1) {}
        numArray(0, 0) = col0.R
        numArray(0, 1) = col0.G
        numArray(0, 2) = col0.B
        numArray(1, 0) = col1.R
        numArray(1, 1) = col1.G
        numArray(1, 2) = col1.B
        If (nColors <= 3) Then
            nColors = 3
            Dim j As Integer
            For j = 2 To nColors - 1
                Dim num2 As Integer = (nColors - j)
                Dim num3 As Integer = (j - 1)
                numArray(j, 0) = (((num2 * col0.R) + (num3 * col1.R)) \ 2)
                numArray(j, 1) = (((num2 * col0.G) + (num3 * col1.G)) \ 2)
                numArray(j, 2) = (((num2 * col0.B) + (num3 * col1.B)) \ 2)
            Next j
            numArray(3, 0) = 0
            numArray(3, 1) = 0
            numArray(3, 2) = 0
            Return numArray
        End If
        nColors = 4
        Dim i As Integer
        For i = 2 To nColors - 1
            Dim num5 As Integer = (nColors - i)
            Dim num6 As Integer = (i - 1)
            numArray(i, 0) = (((num5 * col0.R) + (num6 * col1.R)) \ 3)
            numArray(i, 1) = (((num5 * col0.G) + (num6 * col1.G)) \ 3)
            numArray(i, 2) = (((num5 * col0.B) + (num6 * col1.B)) \ 3)
        Next i
        Return numArray
    End Function

    Private Sub ConvertFrom3DC(ByVal rgb As Integer)
        Dim num As Integer = 255
        Dim num2 As Integer = 0
        If (rgb = 0) Then
            Dim i As Integer
            For i = 0 To 4 - 1
                Dim j As Integer
                For j = 0 To 4 - 1
                    Me.m_Alfa(i, j) = Me.m_Colors(i, j).R
                    If (Me.m_Alfa(i, j) < num) Then
                        num = Me.m_Alfa(i, j)
                    End If
                    If (Me.m_Alfa(i, j) > num2) Then
                        num2 = Me.m_Alfa(i, j)
                    End If
                Next j
            Next i
        ElseIf (rgb = 1) Then
            Dim i As Integer
            For i = 0 To 4 - 1
                Dim j As Integer
                For j = 0 To 4 - 1
                    Me.m_Alfa(i, j) = Me.m_Colors(i, j).G
                    If (Me.m_Alfa(i, j) < num) Then
                        num = Me.m_Alfa(i, j)
                    End If
                    If (Me.m_Alfa(i, j) > num2) Then
                        num2 = Me.m_Alfa(i, j)
                    End If
                Next j
            Next i
        End If
        Me.CleanLuts()

        If ((num2 = 0) AndAlso (num = 0)) Then
            Me.m_Alfa0 = 0
            Me.m_Alfa1 = 1
        End If
        If ((Me.m_DxtType = ETextureFormat.ATI2) AndAlso ((num <> 0) OrElse (num2 <> 0))) Then
            Me.m_Alfa1 = CByte(num)
            Me.m_Alfa0 = CByte(num2)
            Dim num7 As Integer = (num2 - num)
            Dim numArray As Integer() = New Integer(8 - 1) {}
            If (num7 <> 0) Then
                Dim j As Integer
                For j = 2 To 8 - 1
                    numArray(j) = (((Me.m_Alfa0 * (8 - j)) + (Me.m_Alfa1 * (j - 1))) \ 7)
                Next j
            End If
            Dim num8 As Integer = ((num7 \ 7) \ 2)
            Dim i As Integer
            For i = 0 To 4 - 1
                Dim j As Integer
                For j = 0 To 4 - 1
                    Dim num12 As Integer = Me.m_Alfa(i, j)
                    If (num12 <= (num + num8)) Then
                        Me.m_AlfaLut(i, j) = 1
                    ElseIf (num12 >= (num2 - num8)) Then
                        Me.m_AlfaLut(i, j) = 0
                    ElseIf (num7 <> 0) Then
                        Me.m_AlfaLut(i, j) = 0
                        Dim k As Integer
                        For k = 2 To 8 - 1
                            If (num12 > (numArray(k) - num8)) Then
                                Me.m_AlfaLut(i, j) = k
                                Exit For
                            End If
                        Next k
                    Else
                        Me.m_AlfaLut(i, j) = 0
                    End If
                Next j
            Next i
        End If
    End Sub

    Private Sub ConvertFrom4x4()
        Dim colorArray As Color() = New Color(16 - 1) {}
        Dim numArray1 As Integer() = New Integer(16 - 1) {}
        Dim flag As Boolean = False
        Dim index As Integer = 0
        Dim a1 As Integer = 255
        Dim a2 As Integer = 0
        Dim nColors As Integer = 4
        For i = 0 To 4 - 1
            For m = 0 To 4 - 1
                Dim num9 As Integer = CInt(Me.m_Colors(i, m).ToArgb() And -459528)
                Me.m_Colors(i, m) = Color.FromArgb(CInt(num9))
            Next m
        Next i
        For j = 0 To 4 - 1
            For m = 0 To 4 - 1
                Dim num12 As Integer
                Dim flag2 As Boolean = False
                If (Me.m_DxtType = ETextureFormat.DXT1) Then
                    If (Me.m_Colors(j, m).A <> 0) Then
                        GoTo Label_010B
                    End If
                    flag = True
                    nColors = 3
                    Continue For
                End If
                If (Me.m_Colors(j, m).A < a1) Then
                    a1 = Me.m_Colors(j, m).A
                End If
                If (Me.m_Colors(j, m).A > a2) Then
                    a2 = Me.m_Colors(j, m).A
                End If
Label_010B:
                num12 = 0
                Do While (num12 < index)
                    If (((Me.m_Colors(j, m).R = colorArray(num12).R) AndAlso (Me.m_Colors(j, m).G = colorArray(num12).G)) AndAlso (Me.m_Colors(j, m).B = colorArray(num12).B)) Then
                        flag2 = True
                        Exit Do
                    End If
                    num12 += 1
                Loop
                If Not flag2 Then
                    colorArray(index) = Me.m_Colors(j, m)
                    index += 1
                End If
            Next m
        Next j
        Dim num5 As Integer = 16777215
        Me.m_TempLut = New Integer(4 - 1, 4 - 1) {}
        If ((Me.m_DxtType = ETextureFormat.DXT1) AndAlso flag) Then
            Select Case index
                Case 0
                    Dim m As Integer
                    For m = 0 To 4 - 1
                        Dim n As Integer
                        For n = 0 To 4 - 1
                            Me.m_ColorLut(m, n) = 3
                        Next n
                    Next m
                    Me.m_Col0 = 0
                    Me.m_Col1 = 0
                    Return
                Case 1
                    Dim m As Integer
                    For m = 0 To 4 - 1
                        Dim n As Integer
                        For n = 0 To 4 - 1
                            If (Me.m_Colors(m, n).A = 0) Then
                                Me.m_ColorLut(m, n) = 3
                            Else
                                Me.m_ColorLut(m, n) = 0
                            End If
                        Next n
                    Next m
                    Me.m_Col0 = CUShort(((((colorArray(0).R And 248) << 8) Or ((colorArray(0).G And 252) << 3)) Or ((colorArray(0).B And 248) >> 3)))
                    Me.m_Col1 = CUShort(((((colorArray(0).R And 248) << 8) Or ((colorArray(0).G And 252) << 3)) Or ((colorArray(0).B And 248) >> 3)))
                    Return
                Case 2
                    Me.m_Col0 = CUShort(((((colorArray(0).R And 248) << 8) Or ((colorArray(0).G And 252) << 3)) Or ((colorArray(0).B And 248) >> 3)))
                    Me.m_Col1 = CUShort(((((colorArray(1).R And 248) << 8) Or ((colorArray(1).G And 252) << 3)) Or ((colorArray(1).B And 248) >> 3)))
                    If (Me.m_Col0 >= Me.m_Col1) Then
                        Dim num18 As UInt16 = Me.m_Col0
                        Me.m_Col0 = Me.m_Col1
                        Me.m_Col1 = num18
                        Dim color As Color = colorArray(0)
                        colorArray(0) = colorArray(1)
                        colorArray(1) = color
                    End If
                    Dim m As Integer
                    For m = 0 To 4 - 1
                        Dim n As Integer
                        For n = 0 To 4 - 1
                            If (Me.m_Colors(m, n).A = 0) Then
                                Me.m_ColorLut(m, n) = 3
                            ElseIf (Me.m_Colors(m, n) = colorArray(0)) Then
                                Me.m_ColorLut(m, n) = 0
                            Else
                                Me.m_ColorLut(m, n) = 1
                            End If
                        Next n
                    Next m
                    Return
            End Select
        End If
        Me.CleanLuts()

        If ((a1 = 0) AndAlso (a2 = 0)) Then
            Me.m_Alfa0 = 0
            Me.m_Col0 = 0
            Me.m_Alfa1 = 1
            Me.m_Col1 = 1
            index = 0
        End If
        If (index = 1) Then
            Dim m As Integer
            For m = 0 To 4 - 1
                Dim n As Integer
                For n = 0 To 4 - 1
                    Me.m_ColorLut(m, n) = 0
                Next n
            Next m
            Me.m_Col0 = CUShort(((((colorArray(0).R And 248) << 8) Or ((colorArray(0).G And 252) << 3)) Or ((colorArray(0).B And 248) >> 3)))
            Me.m_Col1 = CUShort(((((colorArray(0).R And 248) << 8) Or ((colorArray(0).G And 252) << 3)) Or ((colorArray(0).B And 248) >> 3)))
        End If
        Dim k As Integer
        For k = 0 To index - 1
            Dim m As Integer
            For m = (k + 1) To index - 1
                Dim num6 As Integer
                Dim num27 As UInt16
                Dim num28 As UInt16
                Dim num29 As Integer
                Dim num30 As Integer
                Dim num25 As UInt16 = CUShort(((((colorArray(k).R And 248) << 8) Or ((colorArray(k).G And 252) << 3)) Or ((colorArray(k).B And 248) >> 3)))
                Dim num26 As UInt16 = CUShort(((((colorArray(m).R And 248) << 8) Or ((colorArray(m).G And 252) << 3)) Or ((colorArray(m).B And 248) >> 3)))
                If (num25 < num26) Then
                    num27 = num25
                    num28 = num26
                    num29 = k
                    num30 = m
                Else
                    num27 = num26
                    num28 = num25
                    num29 = m
                    num30 = k
                End If
                If (nColors = 4) Then
                    num6 = Me.ScoreColors(colorArray(num30), colorArray(num29), nColors)
                    If (num6 < num5) Then
                        num5 = num6
                        Dim n As Integer
                        For n = 0 To 4 - 1
                            Dim num32 As Integer
                            For num32 = 0 To 4 - 1
                                Me.m_ColorLut(n, num32) = Me.m_TempLut(n, num32)
                            Next num32
                        Next n
                        Me.m_Col0 = num28
                        Me.m_Col1 = num27
                        If (num6 = 0) Then
                            Exit For
                        End If
                    End If
                End If
                If (Me.m_DxtType = ETextureFormat.DXT1) Then
                    num6 = Me.ScoreColors(colorArray(num29), colorArray(num30), 3)
                    If (num6 < num5) Then
                        num5 = num6
                        Dim n As Integer
                        For n = 0 To 4 - 1
                            Dim num34 As Integer
                            For num34 = 0 To 4 - 1
                                Me.m_ColorLut(n, num34) = Me.m_TempLut(n, num34)
                            Next num34
                        Next n
                        Me.m_Col0 = num27
                        Me.m_Col1 = num28
                        If (num6 = 0) Then
                            Exit For
                        End If
                    End If
                End If
            Next m
        Next k
        If ((Me.m_DxtType = ETextureFormat.DXT3) OrElse (Me.m_DxtType = ETextureFormat.DXT3)) Then
            Dim m As Integer
            For m = 0 To 4 - 1
                Dim n As Integer
                For n = 0 To 4 - 1
                    Me.m_Alfa(m, n) = (Me.m_Colors(m, n).A And 240)
                Next n
            Next m
        End If
        If (((Me.m_DxtType = ETextureFormat.DXT5) OrElse (Me.m_DxtType = ETextureFormat.DXT5)) AndAlso ((a1 <> 0) OrElse (a2 <> 0))) Then
            Me.m_Alfa1 = CByte(a1)
            Me.m_Alfa0 = CByte(a2)
            Dim num37 As Integer = (a2 - a1)
            Dim numArray As Integer() = New Integer(8 - 1) {}
            If (num37 <> 0) Then
                Dim n As Integer
                For n = 2 To 8 - 1
                    numArray(n) = (((Me.m_Alfa0 * (8 - n)) + (Me.m_Alfa1 * (n - 1))) \ 7)
                Next n
            End If
            Dim num38 As Integer = ((num37 \ 7) \ 2)
            For m = 0 To 4 - 1
                Dim n As Integer
                For n = 0 To 4 - 1
                    Dim a As Integer = Me.m_Colors(m, n).A
                    If (a <= (a + num38)) Then
                        Me.m_AlfaLut(m, n) = 1
                    ElseIf (a >= (a - num38)) Then
                        Me.m_AlfaLut(m, n) = 0
                    ElseIf (num37 <> 0) Then
                        Me.m_AlfaLut(m, n) = 0
                        Dim num43 As Integer
                        For num43 = 2 To 8 - 1
                            If (a > (numArray(num43) - num38)) Then
                                Me.m_AlfaLut(m, n) = num43
                                Exit For
                            End If
                        Next num43
                    Else
                        Me.m_AlfaLut(m, n) = 0
                    End If
                Next n
            Next m
        End If
    End Sub

    Private Sub ConvertTo3DC(ByVal rgb As Integer)  'ETextureFormat.ATI2
        Dim numArray As Integer() = New Integer(8 - 1) {}
        numArray(0) = Me.m_Alfa0
        numArray(1) = Me.m_Alfa1
        If (Me.m_Alfa0 > Me.m_Alfa1) Then
            For j = 2 To 8 - 1
                numArray(j) = (((Me.m_Alfa0 * (8 - j)) + (Me.m_Alfa1 * (j - 1))) \ 7)
            Next j
        Else
            For j = 2 To 6 - 1
                numArray(j) = (((Me.m_Alfa0 * (6 - j)) + (Me.m_Alfa1 * (j - 1))) \ 5)
            Next j
            numArray(6) = 0
            numArray(7) = 255
        End If
        For i = 0 To 4 - 1
            For j = 0 To 4 - 1
                Dim index As Integer = Me.m_AlfaLut(i, j)
                Me.m_Alfa(i, j) = numArray(index)
            Next j
        Next i
        If (rgb = 0) Then
            For j = 0 To 4 - 1
                For k = 0 To 4 - 1
                    Me.m_Colors(j, k) = Color.FromArgb(numArray(Me.m_AlfaLut(j, k)), 0, 0)
                Next k
            Next j
        End If
        If (rgb = 1) Then
            For j = 0 To 4 - 1
                For k = 0 To 4 - 1
                    Me.m_Colors(j, k) = Color.FromArgb(Me.m_Colors(j, k).R, numArray(Me.m_AlfaLut(j, k)), 255)
                Next k
            Next j
        End If
    End Sub

    Private Sub ConvertTo3DCPlus()  'ETextureFormat.ATI1
        Dim numArray As Integer() = New Integer(8 - 1) {}
        numArray(0) = Me.m_Alfa0
        numArray(1) = Me.m_Alfa1
        If (Me.m_Alfa0 > Me.m_Alfa1) Then
            Dim j As Integer
            For j = 2 To 8 - 1
                numArray(j) = (((Me.m_Alfa0 * (8 - j)) + (Me.m_Alfa1 * (j - 1))) \ 7)
            Next j
        Else
            Dim j As Integer
            For j = 2 To 6 - 1
                numArray(j) = (((Me.m_Alfa0 * (6 - j)) + (Me.m_Alfa1 * (j - 1))) \ 5)
            Next j
            numArray(6) = 0
            numArray(7) = 255
        End If
        Dim i As Integer
        For i = 0 To 4 - 1
            Dim j As Integer
            For j = 0 To 4 - 1
                Dim index As Integer = Me.m_AlfaLut(i, j)
                Me.m_Alfa(i, j) = numArray(index)
            Next j
        Next i

        For j = 0 To 4 - 1
            For k = 0 To 4 - 1
                Me.m_Colors(j, k) = Color.FromArgb(numArray(Me.m_AlfaLut(j, k)), numArray(Me.m_AlfaLut(j, k)), numArray(Me.m_AlfaLut(j, k)))
            Next k
        Next j

    End Sub

    Private Sub ConvertTo4x4()
        Dim numArray As Integer() = New Integer(8 - 1) {}
        numArray(0) = Me.m_Alfa0
        numArray(1) = Me.m_Alfa1
        If (Me.m_DxtType = ETextureFormat.DXT5) Then
            If (Me.m_Alfa0 > Me.m_Alfa1) Then
                Dim j As Integer
                For j = 2 To 8 - 1
                    numArray(j) = (((Me.m_Alfa0 * (8 - j)) + (Me.m_Alfa1 * (j - 1))) \ 7)
                Next j
            Else
                Dim j As Integer
                For j = 2 To 6 - 1
                    numArray(j) = (((Me.m_Alfa0 * (6 - j)) + (Me.m_Alfa1 * (j - 1))) \ 5)
                Next j
                numArray(6) = 0
                numArray(7) = 255
            End If
        End If
        If (Me.m_DxtType = ETextureFormat.DXT5) Then
            Dim j As Integer
            For j = 0 To 4 - 1
                Dim k As Integer
                For k = 0 To 4 - 1
                    Dim index As Integer = Me.m_AlfaLut(j, k)
                    Me.m_Alfa(j, k) = numArray(index)
                Next k
            Next j
        End If
        If (Me.m_DxtType = ETextureFormat.DXT1) Then
            Dim j As Integer
            For j = 0 To 4 - 1
                Dim k As Integer
                For k = 0 To 4 - 1
                    Dim num8 As Integer = Me.m_ColorLut(j, k)
                    If ((Me.m_Col0 <= Me.m_Col1) AndAlso (num8 = 3)) Then
                        Me.m_Alfa(j, k) = 0
                    Else
                        Me.m_Alfa(j, k) = 255
                    End If
                Next k
            Next j
        End If
        Dim numArray2 As Integer() = New Integer(4 - 1) {}
        Dim numArray3 As Integer() = New Integer(4 - 1) {}
        Dim numArray4 As Integer() = New Integer(4 - 1) {}
        numArray4(0) = (8 * (Me.m_Col0 And 31))
        numArray3(0) = (4 * ((Me.m_Col0 >> 5) And 63))
        numArray2(0) = (8 * (Me.m_Col0 >> 11))
        numArray4(1) = (8 * (Me.m_Col1 And 31))
        numArray3(1) = (4 * ((Me.m_Col1 >> 5) And 63))
        numArray2(1) = (8 * (Me.m_Col1 >> 11))
        If ((Me.m_Col0 > Me.m_Col1) OrElse (Me.m_DxtType <> ETextureFormat.DXT1)) Then
            numArray2(2) = (((2 * numArray2(0)) + numArray2(1)) \ 3)
            numArray3(2) = (((2 * numArray3(0)) + numArray3(1)) \ 3)
            numArray4(2) = (((2 * numArray4(0)) + numArray4(1)) \ 3)
            numArray2(3) = ((numArray2(0) + (2 * numArray2(1))) \ 3)
            numArray3(3) = ((numArray3(0) + (2 * numArray3(1))) \ 3)
            numArray4(3) = ((numArray4(0) + (2 * numArray4(1))) \ 3)
        Else
            numArray2(2) = ((numArray2(0) + numArray2(1)) \ 2)
            numArray3(2) = ((numArray3(0) + numArray3(1)) \ 2)
            numArray4(2) = ((numArray4(0) + numArray4(1)) \ 2)
            numArray2(3) = 0
            numArray3(3) = 0
            numArray4(3) = 0
        End If
        Dim i As Integer
        For i = 0 To 4 - 1
            Dim j As Integer
            For j = 0 To 4 - 1
                Dim index As Integer = Me.m_ColorLut(i, j)
                Me.m_Colors(i, j) = Color.FromArgb(Me.m_Alfa(i, j), numArray2(index), numArray3(index), numArray4(index))
            Next j
        Next i
    End Sub


    Private Sub Init(ByVal dxtType As Byte)
        Me.m_DxtType = DirectCast(dxtType, ETextureFormat)
        Me.m_Col0 = 0
        Me.m_Col1 = 0
        For i = 0 To 4 - 1
            For j = 0 To 4 - 1
                Me.m_ColorLut(i, j) = 0
                Me.m_Alfa(i, j) = 255
                Me.m_Colors(i, j) = Color.FromArgb(0, 0, 0)
            Next j
        Next i
    End Sub

    Public Sub Load(ByVal br As FileReader)
        Select Case Me.m_DxtType
            Case ETextureFormat.DXT1
                Me.ReadColorLut(br)
                Me.ConvertTo4x4()
                Return
            Case ETextureFormat.DXT3
                Me.ReadAlfaChannel(br)
                Me.ReadColorLut(br)
                Me.ConvertTo4x4()
                Return
            Case ETextureFormat.DXT5
                Me.ReadAlfaLut(br)
                Me.ReadColorLut(br)
                Me.ConvertTo4x4()
                Return
            Case ETextureFormat.ATI1    'new
                Me.ReadAlfaLut(br)
                Me.ConvertTo3DCPlus()
                Return
            Case ETextureFormat.ATI2
                Me.ReadAlfaLut(br)
                Me.ConvertTo3DC(0)
                Me.ReadAlfaLut(br)
                Me.ConvertTo3DC(1)
                Return
        End Select
    End Sub

    Private Sub ReadAlfaChannel(ByVal br As FileReader)
        Dim i As Integer
        For i = 0 To 4 - 1
            Dim num2 As Integer
            'If Me.m_SwapEndian Then
            'num2 = FifaUtil.SwapEndian(br.ReadUInt16)
            'Else
            num2 = br.ReadUInt16
            'End If
            Me.m_Alfa(0, i) = ((num2 And 15) << 4)
            Me.m_Alfa(1, i) = (((num2 >> 4) And 15) << 4)
            Me.m_Alfa(2, i) = (((num2 >> 8) And 15) << 4)
            Me.m_Alfa(3, i) = (((num2 >> 12) And 15) << 4)
        Next i
    End Sub

    Private Sub ReadAlfaLut(ByVal br As FileReader)
        Me.m_Alfa0 = br.ReadByte
        Me.m_Alfa1 = br.ReadByte
        Dim buffer As Byte() = br.ReadBytes(6)
        Dim num As UInt64 = 0
        Dim i As Integer = 5
        Do While (i >= 0)
            num = ((num * CULng(256)) Or buffer(i))
            i -= 1
        Loop
        For j As Integer = 0 To 4 - 1
            Dim k As Integer
            For k = 0 To 4 - 1
                Me.m_AlfaLut(k, j) = CInt((num And CULng(7)))
                num = (num >> 3)
            Next k
        Next j
    End Sub

    Private Sub ReadColorLut(ByVal br As FileReader)
        'If Me.m_SwapEndian Then
        'Me.m_Col0 = FifaUtil.SwapEndian(br.ReadUInt16)
        'Me.m_Col1 = FifaUtil.SwapEndian(br.ReadUInt16)
        'Else
        Me.m_Col0 = br.ReadUInt16
            Me.m_Col1 = br.ReadUInt16
        'End If

        'Dim array1 As Byte() = br.ReadBytes(4)

        'If Me.m_SwapEndian Then
        'Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_DxtType)
        'For x As Integer = 0 To array1.Length - 1 Step SwapNumBytes
        'Array.Reverse(array1, x, SwapNumBytes)
        'Next x
        'End If

        For i As Integer = 0 To 4 - 1
            Dim num2 As Byte = br.ReadByte
            Me.m_ColorLut(0, i) = (num2 And 3)
            Me.m_ColorLut(1, i) = ((num2 >> 2) And 3)
            Me.m_ColorLut(2, i) = ((num2 >> 4) And 3)
            Me.m_ColorLut(3, i) = ((num2 >> 6) And 3)
        Next i
    End Sub

    Private Sub ReadNormalLut(ByVal br As FileReader)
        Me.m_Col0 = br.ReadByte
        Me.m_Col1 = br.ReadByte
        Dim buffer As Byte() = br.ReadBytes(6)
        Me.m_ColorLut(0, 0) = (buffer(0) And 7)
        Me.m_ColorLut(1, 0) = ((buffer(0) >> 3) And 7)
        Me.m_ColorLut(2, 0) = (((buffer(0) >> 6) And 3) + ((buffer(1) And 1) << 2))
        Me.m_ColorLut(3, 0) = ((buffer(1) >> 1) And 7)
        Me.m_ColorLut(0, 1) = ((buffer(1) >> 4) And 7)
        Me.m_ColorLut(1, 1) = (((buffer(1) >> 7) And 1) + ((buffer(2) And 3) << 1))
        Me.m_ColorLut(2, 1) = ((buffer(2) >> 2) And 7)
        Me.m_ColorLut(3, 1) = ((buffer(2) >> 5) And 7)
        Me.m_ColorLut(0, 2) = (buffer(0) And 7)
        Me.m_ColorLut(1, 2) = ((buffer(3) >> 3) And 7)
        Me.m_ColorLut(2, 2) = (((buffer(3) >> 6) And 3) + ((buffer(4) And 1) << 2))
        Me.m_ColorLut(3, 2) = ((buffer(3) >> 1) And 7)
        Me.m_ColorLut(0, 3) = ((buffer(4) >> 4) And 7)
        Me.m_ColorLut(1, 3) = (((buffer(4) >> 7) And 1) + ((buffer(5) And 3) << 1))
        Me.m_ColorLut(2, 3) = ((buffer(5) >> 2) And 7)
        Me.m_ColorLut(3, 3) = ((buffer(5) >> 5) And 7)
    End Sub

    Public Sub Save(ByVal bw As FileWriter) ', ByVal SwapEndian As Boolean)
        'Me.m_SwapEndian = SwapEndian   'not needed: sub "new" always called before sub "save"

        Select Case Me.m_DxtType
            Case ETextureFormat.DXT1
                Me.ConvertFrom4x4()
                Me.WriteColorLut(bw)
                Return
            Case ETextureFormat.DXT3
                Me.ConvertFrom4x4()
                Me.WriteAlfaChannel(bw)
                Me.WriteColorLut(bw)
                Return
            Case ETextureFormat.DXT5
                Me.ConvertFrom4x4()
                Me.WriteAlfaLut(bw)
                Me.WriteColorLut(bw)
                Return
            Case ETextureFormat.ATI1    'new added
                Me.ConvertFrom3DC(0)
                Me.WriteAlfaLut(bw)
                Return
            Case ETextureFormat.ATI2
                Me.ConvertFrom3DC(0)
                Me.WriteAlfaLut(bw)
                Me.ConvertFrom3DC(1)
                Me.WriteAlfaLut(bw)
                Return
        End Select
    End Sub

    Private Function ScoreColors(ByVal col0 As Color, ByVal col1 As Color, ByVal nColors As Integer) As Integer
        Dim numArray As Integer(,) = Me.ComputeInterpolatedRgb(col0, col1, nColors)
        Dim num2 As Integer = 0
        Dim i As Integer
        For i = 0 To 4 - 1
            Dim j As Integer
            For j = 0 To 4 - 1
                Dim num As Integer = 16777215
                Dim r As Integer = Me.m_Colors(i, j).R
                Dim g As Integer = Me.m_Colors(i, j).G
                Dim b As Integer = Me.m_Colors(i, j).B
                If ((Me.m_Colors(i, j).A = 0) AndAlso (nColors = 3)) Then
                    Me.m_TempLut(i, j) = 3
                    Continue For
                End If
                Dim k As Integer
                For k = 0 To nColors - 1
                    Dim num9 As Integer = (numArray(k, 0) - r)
                    Dim num10 As Integer = (numArray(k, 1) - g)
                    Dim num11 As Integer = (numArray(k, 2) - b)
                    Dim num12 As Integer = (((num9 * num9) + (num10 * num10)) + (num11 * num11))
                    If (num12 < num) Then
                        num = num12
                        Me.m_TempLut(i, j) = k
                        If (num12 = 0) Then
                            Exit For
                        End If
                    End If
                Next k
                num2 = (num2 + num)
            Next j
        Next i
        Return num2
    End Function

    Private Sub WriteAlfaChannel(ByVal bw As FileWriter)
        For i = 0 To 4 - 1
            Dim num2 As UInt16 = 0
            num2 = CUShort(((Me.m_Alfa(0, i) And 240) >> 4))
            num2 = CUShort((num2 Or CUShort((Me.m_Alfa(1, i) And 240))))
            num2 = CUShort((num2 Or CUShort(((Me.m_Alfa(2, i) And 240) << 4))))
            num2 = CUShort((num2 Or CUShort(((Me.m_Alfa(3, i) And 240) << 8))))
            'If Me.m_SwapEndian Then
            'bw.Write(FifaUtil.SwapEndian(num2))
            'Else
            bw.Write(num2)
            'End If
        Next i
    End Sub

    Private Sub WriteAlfaLut(ByVal bw As FileWriter)
        'If Me.m_SwapEndian Then
        'bw.Write(Me.m_Alfa1)
        'bw.Write(Me.m_Alfa0)
        'Else
        bw.Write(Me.m_Alfa0)
            bw.Write(Me.m_Alfa1)
        'End If
        Dim num As UInt64 = 0
        Dim i As Integer = 3
        Do While (i >= 0)
            Dim k As Integer = 3
            Do While (k >= 0)
                num = (num << 3)
                num = (num Or CULng((Me.m_AlfaLut(k, i) And 7)))
                k -= 1
            Loop
            i -= 1
        Loop
        Dim buffer As Byte() = New Byte(6 - 1) {}
        Dim j As Integer
        For j = 0 To 6 - 1
            buffer(j) = CByte((num And CULng(255)))
            num = (num >> 8)
        Next j

        'If Me.m_SwapEndian Then
        'Dim SwapNumBytes As Integer = GetSwapNumBytes(Me.m_DxtType)
        'For x As Integer = 0 To buffer.Length - 1 Step SwapNumBytes
        'Array.Reverse(buffer, x, SwapNumBytes)
        'Next x
        'End If

        bw.Write(buffer)

    End Sub

    Private Sub WriteColorLut(ByVal bw As FileWriter)
        'If Me.m_SwapEndian Then
        'bw.Write(FifaUtil.SwapEndian(Me.m_Col0))
        'bw.Write(FifaUtil.SwapEndian(Me.m_Col1))
        'Else
        bw.Write(Me.m_Col0)
        bw.Write(Me.m_Col1)
        'End If

        'Dim array1 As Byte() = New Byte(4 - 1) {}

        For i As Integer = 0 To 4 - 1
            Dim num2 As Byte = 0
            num2 = CByte((Me.m_ColorLut(0, i) And 3))
            num2 = CByte((num2 Or CByte(((Me.m_ColorLut(1, i) And 3) << 2))))
            num2 = CByte((num2 Or CByte(((Me.m_ColorLut(2, i) And 3) << 4))))
            num2 = CByte((num2 Or CByte(((Me.m_ColorLut(3, i) And 3) << 6))))
            bw.Write(num2)
        Next i

    End Sub

    ' Properties
    Public Property Colors As Color(,)
        Get
            Return Me.m_Colors
        End Get
        Set(ByVal value As Color(,))
            Me.m_Colors = value
        End Set
    End Property


    ' Fields
    Private m_DxtType As ETextureFormat
    Private m_Col0 As UInt16
    Private m_Col1 As UInt16
    Private m_Alfa0 As Byte
    Private m_Alfa1 As Byte
    Private m_ColorLut As Integer(,)
    Private m_AlfaLut As Integer(,)
    Private m_Alfa As Integer(,)
    Private m_TempLut As Integer(,)
    Private m_Colors As Color(,)

    'Private m_SwapEndian As Boolean
End Class


