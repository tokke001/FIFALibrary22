Imports System
Imports System.Drawing
Imports System.IO

Public Class Rx2TextureBuffer
    Inherits RawImage
    ' Methods
    'Public Sub New()
    'MyBase.New
    'End Sub
    Public Sub New(ByVal RWRaster As RWGObjectType_Raster, ByVal Size As UInteger, ByVal SwapEndian_DxtBlock As Boolean)

        MyBase.New(RWRaster.Width, RWRaster.Height, GraphicUtil.GetEFromRWTextureFormat(RWRaster.TextureFormat), Size, SwapEndian_DxtBlock, True) 'untile needed
        Me.m_NumLevels = RWRaster.NumLevels
        'Me.m_SwapEndian = SwapEndian
    End Sub

    Public Function GetBitmap() As Bitmap
        Return Me.Bitmap
    End Function

    'Public Function GetBitmap(ByVal TextureFace As Integer, ByVal TextureLevel As Integer) As Bitmap
    'Return If((TextureLevel < Me.TextureFaces(TextureFace).TextureLevels.Length), Me.TextureFaces(TextureFace).TextureLevels(TextureLevel).Bitmap, Nothing)
    'End Function

    Public Function GetDds() As DdsFile
        Dim m_DdsFile As New DdsFile

        m_DdsFile.TextureFormat = MyBase.TextureFormat

        m_DdsFile.DdsHeader.dwSize = 124

        m_DdsFile.DdsHeader.dwFlags = DdsHeaderFlags.DDSD_CAPS Or DdsHeaderFlags.DDSD_HEIGHT Or DdsHeaderFlags.DDSD_WIDTH Or DdsHeaderFlags.DDSD_PIXELFORMAT
        Select Case m_DdsFile.TextureFormat
            Case ETextureFormat.DXT1, ETextureFormat.DXT3, ETextureFormat.DXT5, ETextureFormat.ATI1, ETextureFormat.ATI2    'compressed formats : linearsize
                m_DdsFile.DdsHeader.dwFlags = m_DdsFile.DdsHeader.dwFlags Or DdsHeaderFlags.DDSD_LINEARSIZE
            Case Else   'uncompressed formats : pitch
                m_DdsFile.DdsHeader.dwFlags = m_DdsFile.DdsHeader.dwFlags Or DdsHeaderFlags.DDSD_PITCH
        End Select
        If Me.m_NumLevels > 1 Then
            m_DdsFile.DdsHeader.dwFlags = m_DdsFile.DdsHeader.dwFlags Or DdsHeaderFlags.DDSD_MIPMAPCOUNT
        End If

        m_DdsFile.DdsHeader.dwHeight = MyBase.Height
        m_DdsFile.DdsHeader.dwWidth = MyBase.Width
        If m_DdsFile.DdsHeader.dwFlags.HasFlag(DdsHeaderFlags.DDSD_LINEARSIZE) Then     'the total number of bytes in the top level texture for a compressed texture.
            m_DdsFile.DdsHeader.dwPitchOrLinearSize = GraphicUtil.GetTextureSize(MyBase.Width, MyBase.Height, m_DdsFile.TextureFormat)
        ElseIf m_DdsFile.DdsHeader.dwFlags.HasFlag(DdsHeaderFlags.DDSD_PITCH) Then      'The pitch or number of bytes per scan line in an uncompressed texture
            m_DdsFile.DdsHeader.dwPitchOrLinearSize = GraphicUtil.GetTexturePitch(MyBase.Width, m_DdsFile.TextureFormat)
        End If
        m_DdsFile.DdsHeader.dwDepth = 0   'Depth of a volume texture (in pixels), otherwise unused.
        If Me.m_NumLevels <= 1 Then 'Number of mipmap levels, otherwise unused.
            m_DdsFile.DdsHeader.dwMipMapCount = 0
        Else
            m_DdsFile.DdsHeader.dwMipMapCount = Me.m_NumLevels
        End If
        m_DdsFile.DdsHeader.dwReserved1 = New Integer(11 - 1) {}
        m_DdsFile.DdsHeader.ddspf.dwSize = 32
        Select Case m_DdsFile.TextureFormat
            Case ETextureFormat.DXT1
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                m_DdsFile.DdsHeader.ddspf.dwFourCC = "DXT1"
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 0
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwABitMask = 0
            Case ETextureFormat.DXT3
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                m_DdsFile.DdsHeader.ddspf.dwFourCC = "DXT3"
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 0
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwABitMask = 0
            Case ETextureFormat.DXT5
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                m_DdsFile.DdsHeader.ddspf.dwFourCC = "DXT5"
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 0
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwABitMask = 0
            Case ETextureFormat.A8R8G8B8
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB Or DdsPixelformatFlags.DDPF_ALPHAPIXELS 'FifaUtil.SwapEndian(&H41000000)
                m_DdsFile.DdsHeader.ddspf.dwFourCC = ""
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 32
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = FifaUtil.SwapEndian(&HFF00)
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = FifaUtil.SwapEndian(&HFF0000)
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = FifaUtil.SwapEndian(&HFF000000)
                m_DdsFile.DdsHeader.ddspf.dwABitMask = FifaUtil.SwapEndian(&HFF)
            Case ETextureFormat.GREY8
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_LUMINANCE 'FifaUtil.SwapEndian(&H200)
                m_DdsFile.DdsHeader.ddspf.dwFourCC = ""
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 8
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = FifaUtil.SwapEndian(&HFF000000)
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = FifaUtil.SwapEndian(&HFF000000)
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = FifaUtil.SwapEndian(&HFF000000)
                m_DdsFile.DdsHeader.ddspf.dwABitMask = 0
            Case ETextureFormat.GREY8ALFA8
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_LUMINANCE Or DdsPixelformatFlags.DDPF_ALPHAPIXELS  ' FifaUtil.SwapEndian(&H1000200)
                m_DdsFile.DdsHeader.ddspf.dwFourCC = ""
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 16
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = FifaUtil.SwapEndian(&HFF000000)
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = FifaUtil.SwapEndian(&HFF000000)
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = FifaUtil.SwapEndian(&HFF000000)
                m_DdsFile.DdsHeader.ddspf.dwABitMask = FifaUtil.SwapEndian(&HFF0000)
            Case ETextureFormat.ATI2
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                m_DdsFile.DdsHeader.ddspf.dwFourCC = "ATI2"
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 0
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwABitMask = 0
            Case ETextureFormat.ATI1
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC
                m_DdsFile.DdsHeader.ddspf.dwFourCC = "ATI1"
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 0
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwABitMask = 0
            Case ETextureFormat.A4R4G4B4
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB Or DdsPixelformatFlags.DDPF_ALPHAPIXELS    'FifaUtil.SwapEndian(&H41000000)
                m_DdsFile.DdsHeader.ddspf.dwFourCC = ""
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 16
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = FifaUtil.SwapEndian(&HF0000)
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = FifaUtil.SwapEndian(&HF0000000)
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = FifaUtil.SwapEndian(&HF000000)
                m_DdsFile.DdsHeader.ddspf.dwABitMask = FifaUtil.SwapEndian(&HF00000)
            Case ETextureFormat.R5G6B5
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB    ' FifaUtil.SwapEndian(&H40000000)
                m_DdsFile.DdsHeader.ddspf.dwFourCC = ""
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 16
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = FifaUtil.SwapEndian(&HF80000)
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = FifaUtil.SwapEndian(&HE0070000)
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = FifaUtil.SwapEndian(&H1F000000)
                m_DdsFile.DdsHeader.ddspf.dwABitMask = FifaUtil.SwapEndian(&H0)
            Case ETextureFormat.X1R5G5B5
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB Or DdsPixelformatFlags.DDPF_ALPHAPIXELS    ' FifaUtil.SwapEndian(&H41000000)
                m_DdsFile.DdsHeader.ddspf.dwFourCC = ""
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 16
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = FifaUtil.SwapEndian(&H7C0000)
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = FifaUtil.SwapEndian(&HE0030000)
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = FifaUtil.SwapEndian(&H1F000000)
                m_DdsFile.DdsHeader.ddspf.dwABitMask = FifaUtil.SwapEndian(&H800000)
            Case ETextureFormat.RGBA
                Exit Select
            Case ETextureFormat.BIT8
                Exit Select
            Case ETextureFormat.R8G8B8
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_RGB    ' FifaUtil.SwapEndian(&H40000000)
                m_DdsFile.DdsHeader.ddspf.dwFourCC = ""
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 24
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = FifaUtil.SwapEndian(&HFF00)
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = FifaUtil.SwapEndian(&HFF0000)
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = FifaUtil.SwapEndian(&HFF000000)
                m_DdsFile.DdsHeader.ddspf.dwABitMask = FifaUtil.SwapEndian(&H0)
            Case ETextureFormat.DXT1NORMAL
                Exit Select
            Case ETextureFormat.A32B32G32R32F
                m_DdsFile.DdsHeader.ddspf.dwFlags = DdsPixelformatFlags.DDPF_FOURCC 'FifaUtil.SwapEndian(&H4000000)
                m_DdsFile.DdsHeader.ddspf.dwFourCC = "DX10"
                m_DdsFile.DdsHeader.ddspf.dwRGBBitCount = 0 '128
                m_DdsFile.DdsHeader.ddspf.dwRBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwGBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwBBitMask = 0
                m_DdsFile.DdsHeader.ddspf.dwABitMask = 0
                m_DdsFile.DdsHeader_DX10 = New DDS_HEADER_DXT10
                m_DdsFile.DdsHeader_DX10.dxgiFormat = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT
                m_DdsFile.DdsHeader_DX10.resourceDimension = D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D
                m_DdsFile.DdsHeader_DX10.miscFlag = 0
                m_DdsFile.DdsHeader_DX10.arraySize = 1  'For a 2D texture that is also a cube-map texture, this number represents the number of cubes.
                m_DdsFile.DdsHeader_DX10.miscFlags2 = D3D10_ALPHA_MODE.DDS_ALPHA_MODE_UNKNOWN   '0
        End Select

        m_DdsFile.DdsHeader.dwCaps = DdsHeaderCaps.DDSCAPS_TEXTURE
        If m_DdsFile.DdsHeader.dwDepth > 0 Or m_DdsFile.DdsHeader.dwMipMapCount > 0 Then
            m_DdsFile.DdsHeader.dwCaps = m_DdsFile.DdsHeader.dwCaps Or DdsHeaderCaps.DDSCAPS_COMPLEX
            If m_DdsFile.DdsHeader.dwMipMapCount > 0 Then
                m_DdsFile.DdsHeader.dwCaps = m_DdsFile.DdsHeader.dwCaps Or DdsHeaderCaps.DDSCAPS_MIPMAP
            End If
        End If

        m_DdsFile.DdsHeader.dwCaps2 = 0     'for volume/cube textures
        m_DdsFile.DdsHeader.dwCaps3 = 0     'unused
        m_DdsFile.DdsHeader.dwCaps4 = 0     'unused
        m_DdsFile.DdsHeader.dwReserved2 = 0     'unused

        'create rawimage-data
        Dim size As Integer = GraphicUtil.GetTextureSize(m_DdsFile.DdsHeader.dwWidth, m_DdsFile.DdsHeader.dwHeight, m_DdsFile.TextureFormat) '((((Me.dwWidth / 4) * Me.dwHeight) / 4) * &H10)
        Dim width As Integer = m_DdsFile.DdsHeader.dwWidth
        Dim height As Integer = m_DdsFile.DdsHeader.dwHeight
        m_DdsFile.RawImages = New RawImage(Me.m_NumLevels - 1) {}
        For i = 0 To Me.m_NumLevels - 1
            m_DdsFile.RawImages(i) = New RawImage(width, height, m_DdsFile.TextureFormat, size, False)
            m_DdsFile.RawImages(i).SetRawData(MyBase.GetRawData(False), False) 'dds always little endian
            width = (width \ 2)
            height = (height \ 2)
            size = GraphicUtil.GetTextureSize(width, height, m_DdsFile.TextureFormat)  '(size / 4)
        Next i

        Return m_DdsFile
    End Function

    Public Function Load(ByVal r As BinaryReader) As Boolean
        'If Me.m_SwapEndian Then
        MyBase.Load(r)
        'Else
        'MyBase.Load(r)
        'End If
        Return True
    End Function

    Public Function Save(ByVal w As BinaryWriter, ByVal SwapEndian_DxtBlock As Boolean) As Boolean
        'Me.m_SwapEndian = SwapEndian

        'If Me.m_SwapEndian Then
        MyBase.Save(w, SwapEndian_DxtBlock)
        'Else
        'MyBase.Save(w, SwapEndian_DxtBlock)
        'End If

        Return True
    End Function

    Public Function SetBitmap(ByVal bitmap As Bitmap) As Boolean
        Dim TextureFormat As ETextureFormat = MyBase.TextureFormat
        'Dim NumLevels As UShort = Me.Rx3TextureHeader.NumLevels

        Me.SetBitmap(bitmap, TextureFormat) ', NumLevels)

        Return True
    End Function

    Public Function SetBitmap(ByVal bitmap As Bitmap, ByVal TextureFormat As RWTextureFormat) As Boolean ', ByVal NumLevels As UShort) As Boolean
        'Dim FaceIndex = 0

        'If Me.Rx3TextureHeader.TextureType = ETextureType.TEXTURE_2D Then
        'FaceIndex = 0
        'Else
        'Return False
        'End If

        If (bitmap Is Nothing) Then
            Return False
        End If

        MyBase.Width = bitmap.Width
        MyBase.Width = bitmap.Width
        MyBase.TextureFormat = GraphicUtil.GetEFromRWTextureFormat(TextureFormat)
        'MyBase.NumLevels = NumLevels

        'MyBase.NumLevels = 

        Dim srcBitmap As Bitmap = bitmap
        Me.Bitmap = srcBitmap

        'Me.GenerateMipmaps(NumLevels)
        'For i = 1 To Me.Rx3TextureHeader.NumLevels - 1
        'srcBitmap = GraphicUtil.ReduceBitmap(srcBitmap)
        'Me.TextureFaces(FaceIndex).TextureLevels(i).Bitmap = srcBitmap
        'Next i
        Return True
    End Function

    ' Fields
    'Public Property Pitch As UInteger
    'Public Property Lines As UInteger
    'Public Property Padding As UInteger
    Private m_NumLevels As Long

    'Private m_SwapEndian As Boolean
End Class

