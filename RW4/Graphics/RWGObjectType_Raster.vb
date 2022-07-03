Imports System.Drawing
Imports BCnEncoder.Shared
Imports FIFALibrary22.Dds
Imports FIFALibrary22.Ktx
Imports FIFALibrary22.Rw.D3D

Namespace Rw.Graphics
    Public Class Raster
        'rw::graphics::Raster
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWGOBJECTTYPE_RASTER
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.D3d = New D3DBaseTexture(r)  'm_d3d  rw::graphics::Raster::<unnamed-type-m_d3d> > D3DTexture > D3DBaseTexture

            Me.m_Type = r.ReadByte          'always 8
            Me.Face = r.ReadByte            'always 0 ? --> i think is current faceindex, not numfaces !
            Me.NumMipLevels = r.ReadByte
            Me.Locked = r.ReadByte          'always 0

            If MyBase.RwArena.UseRwBuffers AndAlso Me.PBuffer IsNot Nothing Then
                Me.CreateRawImageData()
            End If
        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            If MyBase.RwArena.UseRwBuffers Then   'only do this for rx2 (when rw buffer is used), 
                'Me.SetValues()
                If Me.NeedToSaveRawData Then
                    Me.CreateRawData()
                    Me.NeedToSaveRawData = False
                End If
            End If

            Me.D3d.Save(w)

            w.Write(CByte(Me.m_Type))
            w.Write(Me.Face)
            w.Write(Me.NumMipLevels)
            w.Write(Me.Locked)

        End Sub

        'Private Sub SetValues()
        '    If Me.RawImageData IsNot Nothing Then
        '        Me.SetValues(,,,,)
        '    End If
        'End Sub

        Friend Sub SetValues(ByVal NumFaces As Byte, ByVal NumMipLevels As Byte, ByVal width As Integer, ByVal height As Integer, ByVal TextureFormat As SurfaceFormat) ', ByVal Pitch As UInteger)
            Me.D3d.Format.Depth = NumFaces
            Me.NumMipLevels = NumMipLevels
            Me.D3d.Format.Width = width
            Me.D3d.Format.Height = height
            Me.D3d.Format.TextureFormat = TextureFormat
            Me.D3d.Format.MaxMipLevel = NumMipLevels - 1    'Math.Max(1, NumMipLevels - 1)
            'Me.RW4Section.Sections.Raster(i).D3d.Format.Pitch = Pitch '-->need formula ?
            'Me.RW4Section.Sections.Raster(i).D3d.Format.Endian = 
            'Me.RW4Section.Sections.Raster(i).D3d.Format.Tiled = 
        End Sub

        Public Function GetRawImageData() As List(Of List(Of RawImage))
            If (Me.RawImageData Is Nothing) Then
                Me.CreateRawImageData()
            End If

            Return Me.RawImageData
        End Function

        Public Sub SetRawImageData(ByVal RawImages As List(Of List(Of RawImage)))
            'Me.TextureFormat =   
            'Me.NumMipLevels =     

            Me.RawImageData = RawImages

            Me.NeedToSaveRawData = True
        End Sub

        Public Function GetBitmap() As Bitmap
            Return Me.GetRawImageData(0)(0).Bitmap
        End Function

        Public Function GetBitmap(ByVal TextureFace As Integer, ByVal TextureLevel As Integer) As Bitmap
            Return If((TextureLevel < Me.GetRawImageData(TextureFace).Count), Me.GetRawImageData(TextureFace)(TextureLevel).Bitmap, Nothing)
        End Function

        Public Function GetDds() As DdsFile
            Dim l_RawImages As List(Of List(Of RawImage)) = Me.GetRawImageData()
            '-- convert lists to array
            Dim m_RawImages As RawImage()() = New RawImage(l_RawImages.Count - 1)() {}
            For f = 0 To m_RawImages.Count - 1
                m_RawImages(f) = l_RawImages(f).ToArray
            Next
            '--
            Return DdsUtil.GetDds(m_RawImages) ', Me.D3d.Format.Dimension, GraphicUtil.GetEFromRWTextureFormat(Me.D3d.Format.TextureFormat), Me.D3d.Format.Width, Me.D3d.Format.Height)
        End Function

        Public Function GetKtx() As KtxFile
            Dim l_RawImages As List(Of List(Of RawImage)) = Me.GetRawImageData()
            '-- convert lists to array
            Dim m_RawImages As RawImage()() = New RawImage(l_RawImages.Count - 1)() {}
            For f = 0 To m_RawImages.Count - 1
                m_RawImages(f) = l_RawImages(f).ToArray
            Next
            '--
            Return KtxUtil.GetKtx(m_RawImages) ', Me.D3d.Format.Dimension, GraphicUtil.GetEFromRWTextureFormat(Me.D3d.Format.TextureFormat), Me.D3d.Format.Width, Me.D3d.Format.Height)
        End Function

        Public Function SetBitmap(ByVal bitmap As Bitmap) As Boolean
            Dim TextureFormat As ETextureFormat = Me.D3d.Format.TextureFormat.ToETextureFormat
            Dim NumLevels As UShort = Me.NumMipLevels

            Me.SetBitmap(bitmap, TextureFormat, NumLevels)

            Return True
        End Function

        Public Function SetBitmap(ByVal bitmap As Bitmap, ByVal TextureFormat As Rw.SurfaceFormat, ByVal NumLevels As UShort) As Boolean

            If Me.D3d.Format.Dimension <> GPUDimension.DIMENSION_2D Then
                Return False
            End If

            If (bitmap Is Nothing) Then
                Return False
            End If

            '1 - Generate Texture-Header    'not needed at RW4, RWraster is external section !
            Me.D3d.Format.Height = bitmap.Height
            Me.D3d.Format.Width = bitmap.Width
            Me.D3d.Format.TextureFormat = TextureFormat
            Me.NumMipLevels = NumLevels

            '2 - Set main Bitmap (level 0)
            Dim m_RawImages As New List(Of List(Of RawImage))

            Dim FaceIndex As Integer = 0
            Dim SwapEndian_DxtBlock As Boolean = True
            Dim Tiled360 As Boolean = True
            Dim ETextureFormat As ETextureFormat = TextureFormat.ToETextureFormat
            Dim Size As UInteger = GraphicUtil.GetTextureSize(bitmap.Width, bitmap.Height, ETextureFormat)
            m_RawImages(FaceIndex)(0) = New RawImage(bitmap.Width, bitmap.Height, ETextureFormat, Size, SwapEndian_DxtBlock, Tiled360) 'RawImage(bitmap.Width, bitmap.Height, TextureFormat, SwapEndian_DxtBlock)
            m_RawImages(FaceIndex)(0).Bitmap = bitmap
            '3 - Generate Mipmaps (from main)
            m_RawImages = Me.GenerateMipmaps(m_RawImages, NumLevels, TextureFormat)

            Me.SetRawImageData(m_RawImages)
            Return True
        End Function

        Public Function SetDds(ByVal DdsFile As DdsFile, Optional KeepRx3TextureFormat As Boolean = False) As Boolean
            Dim m_RawImages As RawImage()() = DdsUtil.GetRawImages(DdsFile)
            Return SetRawImages(KeepRx3TextureFormat, m_RawImages)
        End Function

        Public Function SetKtx(ByVal KtxFile As KtxFile, Optional KeepRx3TextureFormat As Boolean = False) As Boolean
            Dim m_RawImages As RawImage()() = KtxUtil.GetRawImages(KtxFile)
            Return SetRawImages(KeepRx3TextureFormat, m_RawImages)
        End Function

        Private Function SetRawImages(KeepRx3TextureFormat As Boolean, m_RawImages As RawImage()()) As Boolean
            'Me.Rx3TextureHeader.Flags_1_TextureEndian = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_LITTLE     '1
            Me.D3d.Format.Width = m_RawImages(0)(0).Width
            Me.D3d.Format.Height = m_RawImages(0)(0).Height
            If KeepRx3TextureFormat = False Then
                Me.D3d.Format.TextureFormat = m_RawImages(0)(0).TextureFormat.ToRwSurfaceFormat
            End If

            Me.D3d.Format.Depth = m_RawImages.Length
            Select Case Me.D3d.Format.Depth
                Case 6
                    Me.D3d.Format.Dimension = GPUDimension.DIMENSION_CUBEMAP
                Case Else
                    Me.D3d.Format.Dimension = GPUDimension.DIMENSION_2D
            End Select
            Me.NumMipLevels = m_RawImages(0).Length

            Dim RawImages_Out As New List(Of List(Of RawImage))

            For f = 0 To Me.D3d.Format.Depth - 1
                Dim m_RawTextureLevels As New List(Of RawImage)
                'Me.TextureFaces(f) = New Rx3TextureFace()
                Dim width As Integer = Me.D3d.Format.Width
                Dim height As Integer = Me.D3d.Format.Height
                Dim ETextureFormat As ETextureFormat = Me.D3d.Format.TextureFormat.ToETextureFormat
                Dim Size As UInteger = GraphicUtil.GetTextureSize(width, height, ETextureFormat)
                Dim SwapEndian_DxtBlock As Boolean = True
                Dim Tiled360 As Boolean = True
                'Dim srcBitmap As Bitmap = Me.TextureFaces(f).TextureLevels(0).Bitmap

                'Me.TextureFaces(f).TextureLevels = New TextureLevel(Me.Rx3TextureHeader.NumMipLevels - 1) {}
                For i = 0 To Me.NumMipLevels - 1

                    Dim m_RawImage As New RawImage(width, height, ETextureFormat, Size, SwapEndian_DxtBlock, Tiled360) '                    Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, Me.Rx3TextureHeader.TextureFormat, SwapEndian_DxtBlock)

                    If Me.D3d.Format.TextureFormat = m_RawImages(0)(0).TextureFormat.ToRwSurfaceFormat Then
                        RawImages_Out(f)(i).RawData(SwapEndian_DxtBlock) = m_RawImages(f)(i).RawData(SwapEndian_DxtBlock)
                    Else
                        Dim m_Bitmap As Bitmap = m_RawImages(f)(0).Bitmap
                        Me.SetBitmap(m_Bitmap)  '---->  this will give problems !!
                    End If

                    m_RawTextureLevels.Add(m_RawImage)

                    width = (width \ 2)
                    height = (height \ 2)
                Next i

                RawImages_Out.Add(m_RawTextureLevels)
            Next f

            Return True
        End Function

        Public Sub SetTextureEndian(SwapEndian_DxtBlock As Boolean)
            'm_SwapEndian_DxtBlock = SwapEndian_DxtBlock
            Dim l_RawImages As List(Of List(Of RawImage)) = Me.GetRawImageData()

            For f = 0 To l_RawImages.Count - 1
                For i = 0 To l_RawImages(f).Count - 1
                    l_RawImages(f)(i).SetEndianFormat(SwapEndian_DxtBlock)
                    'Me.TextureFaces(f).TextureLevels(i) = New Rx3TextureLevelHeader(Me.Rx3TextureHeader.Width, Me.Rx3TextureHeader.Height, Me.Rx3TextureHeader.TextureFormat, Me.m_SwapEndian, SwapEndian_DxtBlock)
                Next
            Next

            Me.SetRawImageData(l_RawImages)
        End Sub

        Public Sub SetTextureTiling(Tiled360 As Boolean)
            'm_Tiled360 = Tiled360
            Dim l_RawImages As List(Of List(Of RawImage)) = Me.GetRawImageData()

            For f = 0 To l_RawImages.Count - 1
                For i = 0 To l_RawImages(f).Count - 1
                    l_RawImages(f)(i).SetTiling360Format(Tiled360)
                    'Me.TextureFaces(f).TextureLevels(i) = New Rx3TextureLevelHeader(Me.Rx3TextureHeader.Width, Me.Rx3TextureHeader.Height, Me.Rx3TextureHeader.TextureFormat, Me.m_SwapEndian, SwapEndian_DxtBlock)
                Next
            Next

            Me.SetRawImageData(l_RawImages)
        End Sub
        'Public Function GenerateMipmaps(ByVal NumLevels As UShort) As Boolean  'generate mipmaps

        'End Function
        Public Sub GenerateMipmaps(ByVal NumLevels As UShort, ByVal TextureFormat As Rw.SurfaceFormat) 'As Boolean  'generate mipmaps
            Me.SetRawImageData(Me.GenerateMipmaps(Me.GetRawImageData(), NumLevels, TextureFormat))
        End Sub

        Private Function GenerateMipmaps(ByVal RawImages As List(Of List(Of RawImage)), ByVal NumLevels As UShort, ByVal TextureFormat As Rw.SurfaceFormat) As List(Of List(Of RawImage))  'generate mipmaps
            'Me.Rx3TextureHeader.NumLevels = NumLevels
            Dim ETextureFormat As ETextureFormat = TextureFormat.ToETextureFormat

            For f = 0 To RawImages.Count - 1
                'Me.TextureFaces(f) = New Rx3TextureFace()
                Dim srcBitmap As Bitmap = RawImages(f)(0).Bitmap
                Dim width As Integer = srcBitmap.Width
                Dim height As Integer = srcBitmap.Height
                Dim SwapEndian_DxtBlock As Boolean = True
                Dim Tiled360 As Boolean = True
                Dim Size As UInteger = GraphicUtil.GetTextureSize(width, height, ETextureFormat)

                'ReDim Preserve Me.TextureFaces(f).TextureLevels(NumLevels - 1)
                RawImages.RemoveRange(1, RawImages(f).Count - 1)
                For i = 0 To NumLevels - 1

                    If i <> 0 Then
                        RawImages(f)(i) = New RawImage(width, height, ETextureFormat, Size, SwapEndian_DxtBlock, Tiled360)
                        'RawImages(f)(i).CalcPitchLinesSize()

                        srcBitmap = GraphicUtil.ReduceBitmap(srcBitmap)
                        RawImages(f)(i).Bitmap = srcBitmap
                    End If

                    width = (width \ 2)
                    height = (height \ 2)
                Next i
            Next f

            Return RawImages
        End Function

        Private Sub CreateRawImageData()      'Read: bytes -> RawImages
            'Dim m_RawImages As New List(Of List(Of RawImage)) '(NumFaces - 1) {}
            Dim m As New MemoryStream(Me.PBuffer.Data)
            Dim r As New FileReader(m, Endian.Big)

            Dim Fix_CreateMipmaps As Boolean = False     'fix: errors at function "ConvertToLinearTexture" with out of array

            Me.RawImageData = New List(Of List(Of RawImage))
            For f = 0 To Me.D3d.Format.Depth - 1
                Dim m_RawTextureLevels As New List(Of RawImage)

                'Me.TextureFaces(f) = New TextureFace()
                Dim width As Integer = Me.D3d.Format.Width    'each face same height/width from header -> so put here
                Dim height As Integer = Me.D3d.Format.Height  'each face same height/width from header -> so put here
                Dim TextureFormat As ETextureFormat = Me.D3d.Format.TextureFormat.ToETextureFormat
                Dim Size As UInteger = GraphicUtil.GetTextureSize(width, height, TextureFormat)
                Dim SwapEndian_DxtBlock As Boolean = True
                Dim Tiled360 As Boolean = True

                For i = 0 To Me.NumMipLevels - 1
                    Dim m_RawImage As New RawImage(width, height, TextureFormat, Size, SwapEndian_DxtBlock, Tiled360)   'Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, Me.m_RWRaster.D3d.Format.TextureFormat, SwapEndian_DxtBlock)
                    If i = 0 Or Fix_CreateMipmaps = False Then
                        m_RawImage.Load(r) 'Me.TextureFaces(f).TextureLevels(i).Load(r)
                    End If

                    m_RawTextureLevels.Add(m_RawImage)

                    width = (width \ 2)
                    height = (height \ 2)
                Next i

                If Fix_CreateMipmaps Then
                    Me.GenerateMipmaps(Me.NumMipLevels, Me.D3d.Format.TextureFormat)
                End If

                Me.RawImageData.Add(m_RawTextureLevels)
            Next f

        End Sub

        Private Sub CreateRawData()     'Write: RawImages -> bytes
            Dim Tmp_Size As UInteger '= (Me.NumIndices * Me.IndexStride)
            For f = 0 To Me.D3d.Format.Depth - 1
                Dim width As Integer = Me.D3d.Format.Width
                Dim height As Integer = Me.D3d.Format.Height
                For i = 0 To Me.NumMipLevels - 1
                    Tmp_Size += GraphicUtil.GetTextureSize(width, height, Me.D3d.Format.TextureFormat.ToETextureFormat)
                    width \= 2
                    height \= 2
                Next i
            Next f
            Do While Tmp_Size Mod 65536 <> 0   'calculate padding: always allignment 16 !
                Tmp_Size += 1
            Loop

            Dim SwapEndian_DxtBlock As Boolean = True
            Me.PBuffer.Data = New Byte(Tmp_Size - 1) {}
            Dim output As New MemoryStream(Me.PBuffer.Data) ', 0, 0)
            Dim w As New FileWriter(output, Endian.Big)

            For f = 0 To Me.RawImageData.Count - 1
                For i = 0 To Me.RawImageData(f).Count - 1
                    Me.RawImageData(f)(i).Save(SwapEndian_DxtBlock, w)
                Next i
            Next f

        End Sub

        Public Property PBuffer As Rw.Core.Arena.Buffer ' pointer to index of buffer
            Get
                Return CType(Me.RwArena.Sections.GetObject(Me.D3d.Format.BaseAddress), Rw.Core.Arena.Buffer)
            End Get
            Set(value As Rw.Core.Arena.Buffer)
                Me.D3d.Format.BaseAddress = Me.RwArena.Sections.IndexOf(value)
            End Set
        End Property

        Public Property D3d As D3DBaseTexture
        Public Property m_Type As EType  'guessed from dump: found these LF_STATICMEMBER s at the section ...
        Public Property Face As Byte
        Public Property NumMipLevels As Byte
        Public Property Locked As Byte

        Private NeedToSaveRawData As Boolean = False
        Private RawImageData As List(Of List(Of RawImage)) = Nothing

        Public Enum Addressing As Integer   'rw::graphics::Raster::Addressing
            ADDRESSING_NA = -1
            ADDRESSING_WRAP = 0
            ADDRESSING_MIRROR = 1
            ADDRESSING_CLAMP = 2
            ADDRESSING_MIRRORONCE = 3
            ADDRESSING_BORDER = 6
            ADDRESSING_FORCEENUMSIZEINT = 2147483647
        End Enum
        Public Enum Filter As Integer   'rw::graphics::Raster::Filter
            FILTER_NA = 514
            FILTER_NEAREST = 512
            FILTER_LINEAR = 513
            FILTER_MIPNEAREST = 0
            FILTER_MIPLINEAR = 1
            FILTER_LINEARMIPNEAREST = 256
            FILTER_LINEARMIPLINEAR = 257
            FILTER_ANISOTROPIC = 516
            FILTER_MIPANISOTROPIC = 4
            FILTER_LINEARMIPANISOTROPIC = 260
            FILTER_FORCEENUMSIZEINT = 2147483647
        End Enum
        Public Enum CubeMapFace As Integer 'rw::graphics::Raster::CubeMapFace
            CUBEMAPFACE_NA = -1
            CUBEMAPFACE_POSITIVE_X = 0
            CUBEMAPFACE_NEGATIVE_X = 1
            CUBEMAPFACE_POSITIVE_Y = 2
            CUBEMAPFACE_NEGATIVE_Y = 3
            CUBEMAPFACE_POSITIVE_Z = 4
            CUBEMAPFACE_NEGATIVE_Z = 5
            CUBEMAPFACE_MAX = 6
            CUBEMAPFACE_FORCEENUMSIZEINT = 2147483647
        End Enum
        'Public Enum Format As Integer   'rw::graphics::Raster::Format
        '    FORMAT_NA = -1
        '    FORMAT_NOTSUPPORTED = -1
        '    FORMAT_555 = 673710403
        '    FORMAT_565 = 673710404
        '    FORMAT_1555 = 405274947
        '    FORMAT_4444 = 405274959
        '    FORMAT_888 = 673710470
        '    FORMAT_8888 = 405275014
        '    FORMAT_LUM4 = -1
        '    FORMAT_LUM8 = 671088898
        '    FORMAT_LUM16 = 671088984
        '    FORMAT_PAL4 = -1
        '    FORMAT_PAL8 = -1
        '    FORMAT_DXT1 = 438305106
        '    FORMAT_LIN_DXT1 = 438304850
        '    FORMAT_DXT2 = 438305107
        '    FORMAT_LIN_DXT2 = 438304851
        '    FORMAT_DXT3 = 438305107
        '    FORMAT_LIN_DXT3 = 438304851
        '    FORMAT_DXT4 = 438305108
        '    FORMAT_LIN_DXT4 = 438304852
        '    FORMAT_DXT5 = 438305108
        '    FORMAT_LIN_DXT5 = 438304852
        '    FORMAT_SRGB_DXT1 = 438337362
        '    FORMAT_SRGB_DXT2 = 438337363
        '    FORMAT_SRGB_DXT3 = 438337363
        '    FORMAT_SRGB_DXT4 = 438337364
        '    FORMAT_SRGB_DXT5 = 438337364
        '    FORMAT_A8 = 76546306
        '    FORMAT_LIN_A8 = 76546050
        '    FORMAT_L8 = 671088898
        '    FORMAT_LIN_L8 = 671088642
        '    FORMAT_R5G6B5 = 673710404
        '    FORMAT_LIN_R5G6B5 = 673710148
        '    FORMAT_R6G5B5 = 673710405
        '    FORMAT_LIN_R6G5B5 = 673710149
        '    FORMAT_L6V5U5 = 706743109
        '    FORMAT_LIN_L6V5U5 = 706742853
        '    FORMAT_X1R5G5B5 = 673710403
        '    FORMAT_LIN_X1R5G5B5 = 673710147
        '    FORMAT_A1R5G5B5 = 405274947
        '    FORMAT_LIN_A1R5G5B5 = 405274691
        '    FORMAT_A4R4G4B4 = 405274959
        '    FORMAT_LIN_A4R4G4B4 = 405274703
        '    FORMAT_X4R4G4B4 = 673710415
        '    FORMAT_LIN_X4R4G4B4 = 673710159
        '    FORMAT_Q4W4V4U4 = 438348623
        '    FORMAT_LIN_Q4W4V4U4 = 438348367
        '    FORMAT_A8L8 = 134218058
        '    FORMAT_LIN_A8L8 = 134217802
        '    FORMAT_G8R8 = 757072202
        '    FORMAT_LIN_G8R8 = 757071946
        '    FORMAT_V8U8 = 757115722
        '    FORMAT_LIN_V8U8 = 757115466
        '    FORMAT_D16 = 438436184
        '    FORMAT_LIN_D16 = 438435928
        '    FORMAT_L16 = 671088984
        '    FORMAT_LIN_L16 = 671088728
        '    FORMAT_R16F = 765635422
        '    FORMAT_LIN_R16F = 765635166
        '    FORMAT_R16F_EXPAND = 765635419
        '    FORMAT_LIN_R16F_EXPAND = 765635163
        '    FORMAT_UYVY = 438305100
        '    FORMAT_LIN_UYVY = 438304844
        '    FORMAT_LE_UYVY = 438305036
        '    FORMAT_LE_LIN_UYVY = 438304780
        '    FORMAT_G8R8_G8B8 = 405274956
        '    FORMAT_LIN_G8R8_G8B8 = 405274700
        '    FORMAT_R8G8_B8G8 = 405274955
        '    FORMAT_LIN_R8G8_B8G8 = 405274699
        '    FORMAT_YUY2 = 438305099
        '    FORMAT_LIN_YUY2 = 438304843
        '    FORMAT_LE_YUY2 = 438305035
        '    FORMAT_LE_LIN_YUY2 = 438304779
        '    FORMAT_A8R8G8B8 = 405275014
        '    FORMAT_LIN_A8R8G8B8 = 405274758
        '    FORMAT_X8R8G8B8 = 673710470
        '    FORMAT_LIN_X8R8G8B8 = 673710214
        '    FORMAT_A8B8G8R8 = 438305158
        '    FORMAT_LIN_A8B8G8R8 = 438304902
        '    FORMAT_X8B8G8R8 = 706740614
        '    FORMAT_LIN_X8B8G8R8 = 706740358
        '    FORMAT_X8L8V8U8 = 706743174
        '    FORMAT_LIN_X8L8V8U8 = 706742918
        '    FORMAT_Q8W8V8U8 = 438348678
        '    FORMAT_LIN_Q8W8V8U8 = 438348422
        '    FORMAT_A2R10G10B10 = 405275062
        '    FORMAT_LIN_A2R10G10B10 = 405274806
        '    FORMAT_X2R10G10B10 = 673710518
        '    FORMAT_LIN_X2R10G10B10 = 673710262
        '    FORMAT_A2B10G10R10 = 438305206
        '    FORMAT_LIN_A2B10G10R10 = 438304950
        '    FORMAT_A2W10V10U10 = 438315958
        '    FORMAT_LIN_A2W10V10U10 = 438315702
        '    FORMAT_A16L16 = 134218137
        '    FORMAT_LIN_A16L16 = 134217881
        '    FORMAT_G16R16 = 757072281
        '    FORMAT_LIN_G16R16 = 757072025
        '    FORMAT_V16U16 = 757115801
        '    FORMAT_LIN_V16U16 = 757115545
        '    FORMAT_R10G11B11 = 673710519
        '    FORMAT_LIN_R10G11B11 = 673710263
        '    FORMAT_R11G11B10 = 673710520
        '    FORMAT_LIN_R11G11B10 = 673710264
        '    FORMAT_W10V11U11 = 706784183
        '    FORMAT_LIN_W10V11U11 = 706783927
        '    FORMAT_W11V11U10 = 706784184
        '    FORMAT_LIN_W11V11U10 = 706783928
        '    FORMAT_G16R16F = 757246879
        '    FORMAT_LIN_G16R16F = 757246623
        '    FORMAT_G16R16F_EXPAND = 757246876
        '    FORMAT_LIN_G16R16F_EXPAND = 757246620
        '    FORMAT_L32 = 671089057
        '    FORMAT_LIN_L32 = 671088801
        '    FORMAT_R32F = 765635492
        '    FORMAT_LIN_R32F = 765635236
        '    FORMAT_A16B16G16R16 = 438305114
        '    FORMAT_LIN_A16B16G16R16 = 438304858
        '    FORMAT_Q16W16V16U16 = 438348634
        '    FORMAT_LIN_Q16W16V16U16 = 438348378
        '    FORMAT_A16B16G16R16F = 438479712
        '    FORMAT_LIN_A16B16G16R16F = 438479456
        '    FORMAT_A16B16G16R16F_EXPAND = 438479709
        '    FORMAT_LIN_A16B16G16R16F_EXPAND = 438479453
        '    FORMAT_A32L32 = 134218146
        '    FORMAT_LIN_A32L32 = 134217890
        '    FORMAT_G32R32 = 757072290
        '    FORMAT_LIN_G32R32 = 757072034
        '    FORMAT_V32U32 = 757115810
        '    FORMAT_LIN_V32U32 = 757115554
        '    FORMAT_G32R32F = 757246885
        '    FORMAT_LIN_G32R32F = 757246629
        '    FORMAT_A32B32G32R32 = 438305187
        '    FORMAT_LIN_A32B32G32R32 = 438304931
        '    FORMAT_Q32W32V32U32 = 438348707
        '    FORMAT_LIN_Q32W32V32U32 = 438348451
        '    FORMAT_A32B32G32R32F = 438479782
        '    FORMAT_LIN_A32B32G32R32F = 438479526
        '    FORMAT_DXN = 438305137
        '    FORMAT_LIN_DXN = 438304881
        '    FORMAT_DXT3A = 438305146
        '    FORMAT_LIN_DXT3A = 438304890
        '    FORMAT_DXT3A_1111 = 438305149
        '    FORMAT_LIN_DXT3A_1111 = 438304893
        '    FORMAT_DXT5A = 438305147
        '    FORMAT_LIN_DXT5A = 438304891
        '    FORMAT_CTX1 = 438305148
        '    FORMAT_LIN_CTX1 = 438304892
        '    FORMAT_D24S8 = 757072278
        '    FORMAT_LIN_D24S8 = 757072022
        '    FORMAT_D24X8 = 765460886
        '    FORMAT_LIN_D24X8 = 765460630
        '    FORMAT_D24FS8 = 438436247
        '    FORMAT_LIN_D24FS8 = 438435991
        '    FORMAT_D32 = 438436257
        '    FORMAT_LIN_D32 = 438436001
        '    FORMAT_LE_X8R8G8B8 = 673710342
        '    FORMAT_LE_A8R8G8B8 = 405274886
        '    FORMAT_LE_X2R10G10B10 = 673710390
        '    FORMAT_LE_A2R10G10B10 = 405274934
        '    FORMAT_A2B10G10R10F_EDRAM = 438436287
        '    FORMAT_G16R16_EDRAM = 757115789
        '    FORMAT_A16B16G16R16_EDRAM = 438348629
        '    FORMAT_FORCEENUMSIZEINT = 2147483647
        'End Enum
        Public Enum EType As Byte 'LF_STATICMEMBER at rw::graphics::Raster ?
            TYPE_CAMERA = 0
            TYPE_ZBUFFER = 1
            TYPE_NORMAL = 2
            TYPE_TEXTURE = 3
            TYPE_BASE_TYPE_MASK = 4
            TYPE_CAMERATEXTURE = 5
            TYPE_ZTEXTURE = 6
            TYPE_DYNAMIC = 7
            TYPE_READ = 8
            TYPE_DONTALLOCATE = 9
            TYPE_XBOX2_CUBE = 10
            TYPE_XBOX2_VOLUME = 11
            TYPE_XBOX2_ARRAY = 12
            TYPE_XBOX2_MULTISAMPLE_2_SAMPLES = 13
            TYPE_XBOX2_MULTISAMPLE_4_SAMPLES = 14
            TYPE_XBOX2_MULTISAMPLE_MASK = 15
            TYPE_XBOX2_MULTISAMPLE_SHIFT = 16
            TYPE_XBOX2_VOLUME_SIZE_MASK = 17
            TYPE_XBOX2_VOLUME_SIZE_SHIFT = 18
            TYPE_XBOX2_ARRAY_SIZE_MASK = 19
            TYPE_XBOX2_ARRAY_SIZE_SHIFT = 20
            TYPE_CUBE = 21
            TYPE_VOLUME = 22
            TYPE_ARRAY = 23
        End Enum

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace


