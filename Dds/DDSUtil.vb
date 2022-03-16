Imports FIFALibrary22.Dds.DDS_HEADER
Imports FIFALibrary22.Dds.DDS_HEADER_DXT10
Imports FIFALibrary22.Dds.DDS_PIXELFORMAT

Namespace Dds
    Public Class DDSUtil

        Public Shared Function GetDds(ByVal RawImages As RawImage()(), ByVal TextureType As ETextureType, ByVal TextureFormat As ETextureFormat, ByVal m_Width As UInteger, ByVal m_Height As UInteger) As DDSFile
            Dim NumFaces As UShort = RawImages.Length
            Dim NumLevels As UShort = RawImages(0).Length

            Dim m_DdsFile As New DDSFile

            m_DdsFile.TextureFormat = TextureFormat

            m_DdsFile.DdsHeader.dwSize = 124

            m_DdsFile.DdsHeader.dwFlags = GetDdsHeaderFlags(TextureType, TextureFormat, NumFaces, NumLevels)

            m_DdsFile.DdsHeader.dwHeight = m_Height
            m_DdsFile.DdsHeader.dwWidth = m_Width

            m_DdsFile.DdsHeader.dwPitchOrLinearSize = GetPitchOrLinearSize(m_DdsFile.DdsHeader.dwFlags, m_Width, m_Height, TextureFormat)

            m_DdsFile.DdsHeader.dwDepth = GetDepth(m_DdsFile.DdsHeader.dwFlags, NumFaces)               'Depth of a volume texture (in pixels), otherwise unused.
            m_DdsFile.DdsHeader.dwMipMapCount = GetMipMapCount(m_DdsFile.DdsHeader.dwFlags, NumLevels)  'Number of mipmap levels, otherwise unused.

            m_DdsFile.DdsHeader.dwReserved1 = New Integer(11 - 1) {}

            '-- Create PixelFormat header
            m_DdsFile.DdsHeader.ddspf = GetDdsPixelformat(TextureFormat)

            m_DdsFile.DdsHeader.dwCaps = GetDdsHeaderCaps(TextureType, NumFaces, NumLevels)
            m_DdsFile.DdsHeader.dwCaps2 = GetDdsHeaderCaps2(TextureType, NumFaces)     'for volume/cube textures
            m_DdsFile.DdsHeader.dwCaps3 = 0     'unused
            m_DdsFile.DdsHeader.dwCaps4 = 0     'unused
            m_DdsFile.DdsHeader.dwReserved2 = 0     'unused

            '-- DdsHeader_DX10 (if needed)
            If m_DdsFile.IsDX10 Then
                m_DdsFile.DdsHeader_DX10 = GetDX10Header(TextureType, TextureFormat, NumFaces)
            End If

            'create rawimage-data
            m_DdsFile.RawImages = New RawImage(NumFaces - 1)() {}
            For f = 0 To m_DdsFile.RawImages.Length - 1
                Dim size As Integer = GraphicUtil.GetTextureSize(m_DdsFile.DdsHeader.dwWidth, m_DdsFile.DdsHeader.dwHeight, TextureFormat) '((((Me.dwWidth / 4) * Me.dwHeight) / 4) * &H10)
                Dim width As Integer = m_DdsFile.DdsHeader.dwWidth
                Dim height As Integer = m_DdsFile.DdsHeader.dwHeight

                m_DdsFile.RawImages(f) = New RawImage(NumLevels - 1) {}
                For i = 0 To m_DdsFile.RawImages(f).Length - 1
                    m_DdsFile.RawImages(f)(i) = New RawImage(width, height, TextureFormat, size, False)
                    m_DdsFile.RawImages(f)(i).SetRawData(RawImages(f)(i).GetRawData(False), False) 'dds always little endian
                    width = (width \ 2)
                    height = (height \ 2)
                    size = GraphicUtil.GetTextureSize(width, height, TextureFormat)  '(size / 4)
                Next i
            Next f
            Return m_DdsFile
        End Function

        Private Shared Function GetDdsHeaderFlags(ByVal TextureType As ETextureType, ByVal TextureFormat As ETextureFormat, ByVal NumFaces As UShort, ByVal NumLevels As UShort) As DdsHeaderFlags
            Dim dwFlags As DdsHeaderFlags = DdsHeaderFlags.DDSD_CAPS Or DdsHeaderFlags.DDSD_HEIGHT Or DdsHeaderFlags.DDSD_WIDTH Or DdsHeaderFlags.DDSD_PIXELFORMAT

            Select Case TextureFormat
                Case ETextureFormat.DXT1, ETextureFormat.DXT3, ETextureFormat.DXT5, ETextureFormat.ATI1, ETextureFormat.ATI2, ETextureFormat.BC6H_UF16    'compressed formats : linearsize
                    dwFlags = dwFlags Or DdsHeaderFlags.DDSD_LINEARSIZE
                Case Else   'uncompressed formats : pitch
                    dwFlags = dwFlags Or DdsHeaderFlags.DDSD_PITCH
            End Select
            If (NumFaces > 1 Or TextureType = ETextureType.TEXTURE_VOLUME) And Not TextureType = ETextureType.TEXTURE_CUBEMAP Then
                dwFlags = dwFlags Or DdsHeaderFlags.DDSD_DEPTH
            End If
            If NumLevels > 1 Then
                dwFlags = dwFlags Or DdsHeaderFlags.DDSD_MIPMAPCOUNT
            End If

            Return dwFlags
        End Function

        Private Shared Function GetPitchOrLinearSize(ByVal m_DdsHeaderFlags As DdsHeaderFlags, ByVal m_Width As UInteger, ByVal m_Height As UInteger, ByVal TextureFormat As ETextureFormat) As Integer
            If m_DdsHeaderFlags.HasFlag(DdsHeaderFlags.DDSD_LINEARSIZE) Then     'the total number of bytes in the top level texture for a compressed texture.
                Return GraphicUtil.GetTextureSize(m_Width, m_Height, TextureFormat)
            ElseIf m_DdsHeaderFlags.HasFlag(DdsHeaderFlags.DDSD_PITCH) Then      'The pitch or number of bytes per scan line in an uncompressed texture
                Return GraphicUtil.GetTexturePitch(m_Width, TextureFormat)
            End If

            Return 0
        End Function

        Private Shared Function GetDepth(ByVal m_DdsHeaderFlags As DdsHeaderFlags, ByVal num As UInteger) As Integer
            If m_DdsHeaderFlags.HasFlag(DdsHeaderFlags.DDSD_DEPTH) And num > 1 Then
                Return num
            End If

            Return 0
        End Function

        Private Shared Function GetMipMapCount(ByVal m_DdsHeaderFlags As DdsHeaderFlags, ByVal num As UInteger) As Integer
            If m_DdsHeaderFlags.HasFlag(DdsHeaderFlags.DDSD_MIPMAPCOUNT) And num > 1 Then
                Return num
            End If

            Return 0
        End Function
        Public Shared Function GetDdsPixelformat(ByVal TextureFormat As ETextureFormat) As DDS_PIXELFORMAT
            Dim ddspf As New DDS_PIXELFORMAT

            ddspf.dwSize = 32
            Select Case TextureFormat
                Case ETextureFormat.DXT1
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                    ddspf.dwFourCC = "DXT1"
                    ddspf.dwRGBBitCount = 0
                    ddspf.dwRBitMask = 0
                    ddspf.dwGBitMask = 0
                    ddspf.dwBBitMask = 0
                    ddspf.dwABitMask = 0
                Case ETextureFormat.DXT3
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                    ddspf.dwFourCC = "DXT3"
                    ddspf.dwRGBBitCount = 0
                    ddspf.dwRBitMask = 0
                    ddspf.dwGBitMask = 0
                    ddspf.dwBBitMask = 0
                    ddspf.dwABitMask = 0
                Case ETextureFormat.DXT5
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                    ddspf.dwFourCC = "DXT5"
                    ddspf.dwRGBBitCount = 0
                    ddspf.dwRBitMask = 0
                    ddspf.dwGBitMask = 0
                    ddspf.dwBBitMask = 0
                    ddspf.dwABitMask = 0
                Case ETextureFormat.A8R8G8B8
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB Or DdsPixelformatFlags.DDPF_ALPHAPIXELS 'FifaUtil.SwapEndian(&H41000000)
                    ddspf.dwFourCC = ""
                    ddspf.dwRGBBitCount = 32
                    ddspf.dwRBitMask = FifaUtil.SwapEndian(&HFF00)
                    ddspf.dwGBitMask = FifaUtil.SwapEndian(&HFF0000)
                    ddspf.dwBBitMask = FifaUtil.SwapEndian(&HFF000000)
                    ddspf.dwABitMask = FifaUtil.SwapEndian(&HFF)
                Case ETextureFormat.GREY8
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_LUMINANCE 'FifaUtil.SwapEndian(&H200)
                    ddspf.dwFourCC = ""
                    ddspf.dwRGBBitCount = 8
                    ddspf.dwRBitMask = FifaUtil.SwapEndian(&HFF000000)
                    ddspf.dwGBitMask = FifaUtil.SwapEndian(&HFF000000)
                    ddspf.dwBBitMask = FifaUtil.SwapEndian(&HFF000000)
                    ddspf.dwABitMask = 0
                Case ETextureFormat.GREY8ALFA8
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_LUMINANCE Or DdsPixelformatFlags.DDPF_ALPHAPIXELS  ' FifaUtil.SwapEndian(&H1000200)
                    ddspf.dwFourCC = ""
                    ddspf.dwRGBBitCount = 16
                    ddspf.dwRBitMask = FifaUtil.SwapEndian(&HFF000000)
                    ddspf.dwGBitMask = FifaUtil.SwapEndian(&HFF000000)
                    ddspf.dwBBitMask = FifaUtil.SwapEndian(&HFF000000)
                    ddspf.dwABitMask = FifaUtil.SwapEndian(&HFF0000)
                Case ETextureFormat.ATI2
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                    ddspf.dwFourCC = "ATI2"
                    ddspf.dwRGBBitCount = 0
                    ddspf.dwRBitMask = 0
                    ddspf.dwGBitMask = 0
                    ddspf.dwBBitMask = 0
                    ddspf.dwABitMask = 0
                Case ETextureFormat.ATI1
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                    ddspf.dwFourCC = "ATI1"
                    ddspf.dwRGBBitCount = 0
                    ddspf.dwRBitMask = 0
                    ddspf.dwGBitMask = 0
                    ddspf.dwBBitMask = 0
                    ddspf.dwABitMask = 0
                Case ETextureFormat.A4R4G4B4
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB Or DdsPixelformatFlags.DDPF_ALPHAPIXELS    'FifaUtil.SwapEndian(&H41000000)
                    ddspf.dwFourCC = ""
                    ddspf.dwRGBBitCount = 16
                    ddspf.dwRBitMask = FifaUtil.SwapEndian(&HF0000)
                    ddspf.dwGBitMask = FifaUtil.SwapEndian(&HF0000000)
                    ddspf.dwBBitMask = FifaUtil.SwapEndian(&HF000000)
                    ddspf.dwABitMask = FifaUtil.SwapEndian(&HF00000)
                Case ETextureFormat.R5G6B5
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB    ' FifaUtil.SwapEndian(&H40000000)
                    ddspf.dwFourCC = ""
                    ddspf.dwRGBBitCount = 16
                    ddspf.dwRBitMask = FifaUtil.SwapEndian(&HF80000)
                    ddspf.dwGBitMask = FifaUtil.SwapEndian(&HE0070000)
                    ddspf.dwBBitMask = FifaUtil.SwapEndian(&H1F000000)
                    ddspf.dwABitMask = FifaUtil.SwapEndian(&H0)
                Case ETextureFormat.X1R5G5B5
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB Or DdsPixelformatFlags.DDPF_ALPHAPIXELS    ' FifaUtil.SwapEndian(&H41000000)
                    ddspf.dwFourCC = ""
                    ddspf.dwRGBBitCount = 16
                    ddspf.dwRBitMask = FifaUtil.SwapEndian(&H7C0000)
                    ddspf.dwGBitMask = FifaUtil.SwapEndian(&HE0030000)
                    ddspf.dwBBitMask = FifaUtil.SwapEndian(&H1F000000)
                    ddspf.dwABitMask = FifaUtil.SwapEndian(&H800000)
                Case ETextureFormat.RGBA
                    Exit Select
                Case ETextureFormat.BIT8
                    Exit Select
                Case ETextureFormat.R8G8B8
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB    ' FifaUtil.SwapEndian(&H40000000)
                    ddspf.dwFourCC = ""
                    ddspf.dwRGBBitCount = 24
                    ddspf.dwRBitMask = FifaUtil.SwapEndian(&HFF00)
                    ddspf.dwGBitMask = FifaUtil.SwapEndian(&HFF0000)
                    ddspf.dwBBitMask = FifaUtil.SwapEndian(&HFF000000)
                    ddspf.dwABitMask = FifaUtil.SwapEndian(&H0)
                Case ETextureFormat.CTX1
                    Exit Select
                Case ETextureFormat.A32B32G32R32F
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC 'FifaUtil.SwapEndian(&H4000000)
                    ddspf.dwFourCC = Chr(116) & Chr(0) & Chr(0) & Chr(0) 'FifaUtil.SwapEndian(&H74000000) '"DX10"
                    ddspf.dwRGBBitCount = 0 '128
                    ddspf.dwRBitMask = 0
                    ddspf.dwGBitMask = 0
                    ddspf.dwBBitMask = 0
                    ddspf.dwABitMask = 0
                Case ETextureFormat.BC6H_UF16
                    ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC 'FifaUtil.SwapEndian(&H4000000)
                    ddspf.dwFourCC = "DX10" 'FifaUtil.SwapEndian(&H74000000) '"DX10"
                    ddspf.dwRGBBitCount = 0 '128
                    ddspf.dwRBitMask = 0
                    ddspf.dwGBitMask = 0
                    ddspf.dwBBitMask = 0
                    ddspf.dwABitMask = 0
            End Select

            Return ddspf
        End Function

        Public Shared Function GetDdsHeaderCaps(ByVal TextureType As ETextureType, ByVal NumFaces As UShort, ByVal NumLevels As UShort) As DdsHeaderCaps
            Dim dwCaps As DdsHeaderCaps = DdsHeaderCaps.DDSCAPS_TEXTURE

            If NumFaces > 1 Or NumLevels > 1 Or (TextureType = ETextureType.TEXTURE_CUBEMAP Or TextureType = ETextureType.TEXTURE_VOLUME) Then
                dwCaps = dwCaps Or DdsHeaderCaps.DDSCAPS_COMPLEX
                If NumLevels > 1 Then
                    dwCaps = dwCaps Or DdsHeaderCaps.DDSCAPS_MIPMAP
                End If
            End If

            Return dwCaps
        End Function

        Public Shared Function GetDdsHeaderCaps2(ByVal TextureType As ETextureType, ByVal NumFaces As UShort) As DdsHeaderCaps2
            Dim dwCaps2 As DdsHeaderCaps2 = 0

            If TextureType = ETextureType.TEXTURE_CUBEMAP Then
                dwCaps2 = DdsHeaderCaps2.DDSCAPS2_CUBEMAP
                dwCaps2 = dwCaps2 Or DdsHeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEX Or DdsHeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEY Or DdsHeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEZ Or DdsHeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEX Or DdsHeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEY Or DdsHeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEZ
                'DDSCAPS2_CUBEMAP, and one or more of DSCAPS2_CUBEMAP_POSITIVEX/Y/Z and/or DDSCAPS2_CUBEMAP_NEGATIVEX/Y/Z should be set.
                'The faces are written in the order: positive x, negative x, positive y, negative y, positive z, negative z, with any missing faces omitted.
                'Each face is written with its main image, followed by any mipmap levels.
            Else
                If NumFaces > 1 Or TextureType = ETextureType.TEXTURE_VOLUME Then
                    dwCaps2 = DdsHeaderCaps2.DDSCAPS2_VOLUME
                End If
            End If

            Return dwCaps2
        End Function
        Public Shared Function GetDX10Header(ByVal TextureType As ETextureType, ByVal TextureFormat As ETextureFormat, ByVal NumFaces As UShort) As DDS_HEADER_DXT10
            Dim DdsHeader_DX10 As New DDS_HEADER_DXT10

            Select Case TextureFormat
                Case ETextureFormat.A32B32G32R32F
                    DdsHeader_DX10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT
                Case ETextureFormat.BC6H_UF16
                    DdsHeader_DX10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16

            End Select

            DdsHeader_DX10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D
            If TextureType = ETextureType.TEXTURE_CUBEMAP Then
                DdsHeader_DX10.miscFlag = D3D10_RESOURCE_MISC.DDS_RESOURCE_MISC_TEXTURECUBE
                DdsHeader_DX10.arraySize = NumFaces  'For a 2D texture that is also a cube-map texture, this number represents the number of cubes.
            Else
                DdsHeader_DX10.miscFlag = 0
                DdsHeader_DX10.arraySize = 1  'For a 2D texture that is also a cube-map texture, this number represents the number of cubes.
            End If
            DdsHeader_DX10.miscFlags2 = D3D10_ALPHA_MODE.DDS_ALPHA_MODE_UNKNOWN   '0

            Return DdsHeader_DX10
        End Function

    End Class
End Namespace