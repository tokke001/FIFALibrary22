Imports FIFALibrary22.Rw.D3D

Namespace Rw.Graphics
    Public Class Raster
        'rw::graphics::Raster
        Inherits RWObject
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

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            'Me.TextureFormat =   'Set in RX3 section
            'Me.NumMipLevels =     'Set in RX3 section

            Me.D3d.Save(w)

            w.Write(CByte(Me.m_Type))
            w.Write(Me.Face)
            w.Write(Me.NumMipLevels)
            w.Write(Me.Locked)

        End Sub

        Public Property TextureData As Rw.Core.Arena.Buffer ' pointer to index of buffer
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


