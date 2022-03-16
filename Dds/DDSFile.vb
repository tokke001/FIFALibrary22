Imports System.Drawing
'Imports FifaLibrary
Namespace Dds
    Public Class DDSFile
        ' Methods
        Public Sub New()
        End Sub

        Public Sub New(ByVal fifaFile As FifaFile)
            Me.Load(fifaFile)
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub New(ByVal fileName As String)
            Me.Load(fileName)
        End Sub

        Public Function GetBitmap() As Bitmap
            If ((Me.RawImages IsNot Nothing) AndAlso (Me.RawImages.Length >= 1)) Then
                Return Me.RawImages(0)(0).Bitmap
            End If
            Return Nothing
        End Function

        Public Function Load(ByVal fifaFile As FifaFile) As Boolean
            If fifaFile.IsCompressed Then
                fifaFile.Decompress()
            End If
            Dim r As FileReader = fifaFile.GetReader
            Dim flag As Boolean = Me.Load(r)
            fifaFile.ReleaseReader(r)

            Return flag
        End Function

        Public Function Load(ByVal r As FileReader) As Boolean
            Me.dwMagic = New String(r.ReadChars(4))
            If (Me.dwMagic <> "DDS ") Then  '&H20534444
                Return False
            End If

            Me.DdsHeader = New DDS_HEADER(r)

            If Me.IsDX10() Then    'https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dds-header-dxt10
                Me.DdsHeader_DX10 = New DDS_HEADER_DXT10(r)
            Else
                Me.DdsHeader_DX10 = Nothing
            End If

            Me.TextureFormat = GetTextureFormat(Me.DdsHeader, Me.DdsHeader_DX10)

            Dim NumFaces As Integer = Me.DdsHeader.dwDepth
            If NumFaces = 0 Then NumFaces = 1
            Dim NumLevels As Integer = Me.DdsHeader.dwMipMapCount
            If NumLevels = 0 Then NumLevels = 1

            Me.RawImages = New RawImage(NumFaces - 1)() {}
            For f = 0 To Me.RawImages.Length - 1
                Dim size As Integer = GraphicUtil.GetTextureSize(Me.DdsHeader.dwWidth, Me.DdsHeader.dwHeight, Me.TextureFormat) '((((Me.dwWidth / 4) * Me.dwHeight) / 4) * &H10)
                Dim width As Integer = Me.DdsHeader.dwWidth
                Dim height As Integer = Me.DdsHeader.dwHeight

                Me.RawImages(f) = New RawImage(NumLevels - 1) {}
                For k = 0 To Me.RawImages(f).Length - 1
                    Me.RawImages(f)(k) = New RawImage(width, height, Me.TextureFormat, size, False)
                    Me.RawImages(f)(k).Load(r)
                    width = (width \ 2)
                    height = (height \ 2)
                    size = GraphicUtil.GetTextureSize(width, height, Me.TextureFormat)  '(size \ 4)
                Next k
            Next

            Return True
        End Function
        Private Function GetTextureFormat(ByVal m_DdsHeader As DDS_HEADER, ByVal m_DdsHeader_10 As DDS_HEADER_DXT10) As ETextureFormat
            Dim FourCC As String = m_DdsHeader.ddspf.dwFourCC
            Dim dwRGBBitCount As Integer = m_DdsHeader.ddspf.dwRGBBitCount
            Dim dwRBitMask As Integer = m_DdsHeader.ddspf.dwRBitMask
            Dim dwGBitMask As Integer = m_DdsHeader.ddspf.dwGBitMask
            Dim dwBBitMask As Integer = m_DdsHeader.ddspf.dwBBitMask
            Dim dwABitMask As Integer = m_DdsHeader.ddspf.dwABitMask

            Select Case FourCC
                Case "DXT1"
                    Return ETextureFormat.DXT1
            'Case "DXT2"
            'not used in fifa?
                Case "DXT3"
                    Return ETextureFormat.DXT3
            'Case "DXT4"
            'not used in fifa?
                Case "DXT5"
                    Return ETextureFormat.DXT5
                Case "ATI1"
                    Return ETextureFormat.ATI1
                Case "ATI2"
                    Return ETextureFormat.ATI2
                Case vbNullChar & vbNullChar & vbNullChar & vbNullChar
                    Select Case dwRGBBitCount
                        Case 8
                            If (dwRBitMask = FifaUtil.SwapEndian(&HFF000000) And dwGBitMask = FifaUtil.SwapEndian(&HFF000000) And dwBBitMask = FifaUtil.SwapEndian(&HFF000000) And dwABitMask = 0) _
                            Or (dwRBitMask = FifaUtil.SwapEndian(&HFF000000) And dwGBitMask = 0 And dwBBitMask = 0 And dwABitMask = 0) Then
                                Return ETextureFormat.GREY8
                            End If
                        Case 16
                            If (dwRBitMask = FifaUtil.SwapEndian(&HFF000000) And dwGBitMask = FifaUtil.SwapEndian(&HFF000000) And dwBBitMask = FifaUtil.SwapEndian(&HFF000000) And dwABitMask = FifaUtil.SwapEndian(&HFF0000)) _
                            Or (dwRBitMask = FifaUtil.SwapEndian(&HFF000000) And dwGBitMask = 0 And dwBBitMask = 0 And dwABitMask = FifaUtil.SwapEndian(&HFF0000)) Then
                                Return ETextureFormat.GREY8ALFA8

                            ElseIf dwRBitMask = FifaUtil.SwapEndian(&HF0000) And dwGBitMask = FifaUtil.SwapEndian(&HF0000000) And dwBBitMask = FifaUtil.SwapEndian(&HF000000) And dwABitMask = FifaUtil.SwapEndian(&HF00000) Then
                                Return ETextureFormat.A4R4G4B4
                            ElseIf dwRBitMask = FifaUtil.SwapEndian(&HF80000) And dwGBitMask = FifaUtil.SwapEndian(&HE0070000) And dwBBitMask = FifaUtil.SwapEndian(&H1F000000) And dwABitMask = 0 Then
                                Return ETextureFormat.R5G6B5
                            End If
                        Case 24
                            If dwRBitMask = FifaUtil.SwapEndian(&HFF00) And dwGBitMask = FifaUtil.SwapEndian(&HFF0000) And dwBBitMask = FifaUtil.SwapEndian(&HFF000000) And dwABitMask = 0 Then
                                Return ETextureFormat.R8G8B8
                            End If
                        Case 32
                            If dwRBitMask = FifaUtil.SwapEndian(&HFF00) And dwGBitMask = FifaUtil.SwapEndian(&HFF0000) And dwBBitMask = FifaUtil.SwapEndian(&HFF000000) And dwABitMask = FifaUtil.SwapEndian(&HFF) Then
                                Return ETextureFormat.A8R8G8B8
                            End If
                    End Select

                Case "DX10"     'dx10 format, requires reading from dx10 header
                    If m_DdsHeader_10 IsNot Nothing Then
                        Select Case m_DdsHeader_10.dxgiFormat
                            Case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB
                                Return ETextureFormat.DXT1
                            Case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB
                                Return ETextureFormat.DXT3
                            Case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB
                                Return ETextureFormat.DXT5
                            Case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM
                                Return ETextureFormat.ATI1
                            Case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM
                                Return ETextureFormat.ATI2
                            Case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM
                                Return ETextureFormat.A8R8G8B8
                            Case DXGI_FORMAT.DXGI_FORMAT_R8_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_R8_UNORM, DXGI_FORMAT.DXGI_FORMAT_R8_UINT, DXGI_FORMAT.DXGI_FORMAT_R8_SNORM, DXGI_FORMAT.DXGI_FORMAT_R8_SINT
                                Return ETextureFormat.GREY8
                            Case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16
                                Return ETextureFormat.BC6H_UF16
                                'Case 
                                'Return ETextureFormat.GREY8ALFA8
                        End Select
                    End If

            End Select

            Return Nothing
        End Function
        Public Function Load(ByVal fileName As String) As Boolean
            Dim f As New FileStream(fileName, FileMode.Open, FileAccess.ReadWrite)
            Dim r As New FileReader(f)
            Dim flag As Boolean = Me.Load(r)
            f.Close()
            r.Close()
            Return flag
        End Function
        Public Sub ReplaceBitmap(ByVal bitmap As Bitmap)    '2D texture
            If (Me.DdsHeader.dwMipMapCount > 0) Then
                Dim srcBitmap As Bitmap = bitmap
                Me.RawImages(0)(0).Bitmap = bitmap
                'generate mipmaps
                For i = 0 To Me.DdsHeader.dwMipMapCount - 1
                    If i <> 0 Then
                        srcBitmap = GraphicUtil.ReduceBitmap(srcBitmap)
                        Me.RawImages(0)(i).Bitmap = srcBitmap
                    End If
                Next i
            Else
                Me.RawImages(0)(0).Bitmap = bitmap
            End If
        End Sub

        Public Sub ReplaceBitmap(ByVal bitmap As Bitmap())  'cubic
            If Me.RawImages.Length <> bitmap.Length Then Return

            For f = 0 To Me.RawImages.Length - 1
                If (Me.DdsHeader.dwMipMapCount > 0) Then
                    Dim srcBitmap As Bitmap = bitmap(f)

                    Me.RawImages(f)(0).Bitmap = bitmap(f)
                    'generate mipmaps
                    For i = 0 To Me.DdsHeader.dwMipMapCount - 1
                        If i <> 0 Then
                            srcBitmap = GraphicUtil.ReduceBitmap(srcBitmap)
                            Me.RawImages(f)(i).Bitmap = srcBitmap
                        End If
                    Next i

                Else
                    Me.RawImages(f)(0).Bitmap = bitmap(f)
                End If
            Next f
        End Sub

        Public Function Save(ByVal w As FileWriter) As Boolean
            w.Write("D"c)
            w.Write("D"c)
            w.Write("S"c)
            w.Write(" "c)

            Me.DdsHeader.Save(w)

            If Me.IsDX10() Then    'https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dds-header-dxt10
                Me.DdsHeader_DX10.Save(w)
            End If

            Dim NumFaces As Integer = Me.DdsHeader.dwDepth
            If NumFaces = 0 Then NumFaces = 1
            Dim NumLevels As Integer = Me.DdsHeader.dwMipMapCount
            If NumLevels = 0 Then NumLevels = 1

            For f = 0 To NumFaces - 1
                Dim size As Integer = GraphicUtil.GetTextureSize(Me.DdsHeader.dwWidth, Me.DdsHeader.dwHeight, Me.TextureFormat) '((((Me.dwWidth / 4) * Me.dwHeight) / 4) * &H10)
                Dim width As Integer = Me.DdsHeader.dwWidth
                Dim height As Integer = Me.DdsHeader.dwHeight
                For k = 0 To NumLevels - 1
                    If size <> Me.RawImages(f)(k).RawData.Length Then
                        ReDim Me.RawImages(f)(k).RawData(size - 1)
                    End If
                    Me.RawImages(f)(k).Save(False, w)
                    width = (width \ 2)
                    height = (height \ 2)
                    size = GraphicUtil.GetTextureSize(width, height, Me.TextureFormat)  '(size / 4)
                Next k
            Next f

            Return True
        End Function

        Public Function Save(ByVal fileName As String) As Boolean
            Dim output As New FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)
            Dim w As New FileWriter(output)
            Dim flag As Boolean = Me.Save(w)
            output.Close()
            w.Close()

            Return flag
        End Function

        Public Function IsDX10() As Boolean
            If Me.DdsHeader.ddspf.dwFlags.HasFlag(DDS_PIXELFORMAT.DdsPixelformatFlags.DDPF_FOURCC) AndAlso Me.DdsHeader.ddspf.dwFourCC = "DX10" Then    'https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dds-header-dxt10
                Return True
            Else
                Return False
            End If
        End Function

        ' Fields
        Public Property dwMagic As String
        'Protected Friend m_HeaderSize As UInt32
        'Protected Friend m_HeaderFlags As UInt32
        'Protected Friend dwWidth As Integer
        'Protected Friend dwHeight As Integer
        'Protected Friend dwPitchOrLinearSize As Integer
        'Protected Friend dwDepth As Integer
        'Protected Friend dwMipMapCount As Integer
        'Protected Friend m_PixelFormatSize As Integer
        'Protected Friend m_PixelFormatFlag As Integer
        'Protected Friend dwFourCC As String
        Public Property TextureFormat As ETextureFormat
        'Protected Friend dwRGBBitCount As Integer
        'Protected Friend dwRBitMask As Integer
        'Protected Friend dwGBitMask As Integer
        'Protected Friend dwBBitMask As Integer
        'Protected Friend dwABitMask As Integer
        'Protected Friend dwCaps As Integer
        'Protected Friend dwCaps2 As Integer
        Public Property RawImages As RawImage()()

        'Protected Friend DX10_dxgiFormat As DXGI_FORMAT
        'Protected Friend DX10_resourceDimension As Integer
        'Protected Friend DX10_miscFlag As Integer
        'Protected Friend DX10_arraySize As Integer
        'Protected Friend DX10_miscFlags2 As Integer

        Public Property DdsHeader As DDS_HEADER = New DDS_HEADER
        Public Property DdsHeader_DX10 As DDS_HEADER_DXT10 '= new DDS_HEADER

    End Class
End Namespace