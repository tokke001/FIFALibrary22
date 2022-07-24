Namespace Rx3
    Public Enum SectionHash As UInt32
        'since FIFA 11
        INDEX_BUFFER = 5798132
        VERTEX_BUFFER = 5798561
        BONE_REMAP = 255353250  'PARTIAL_SKELETON
        INDEX_BUFFER_BATCH = 582139446  'INDEX_BUFFERS_HEADER
        TEXTURE_BATCH = 1808827868
        TEXTURE = 2047566042

        MATERIAL = 123459928
        SCENE_LAYER = 230948820 'NODE_NAME
        LOCATION = 685399266    'PROP
        NAME_TABLE = 1285267122
        EDGE_MESH = 1843792843
        SCENE_INSTANCE = 2116321516 'NODE
        SCENE_ANIMATION = 2360999927 'tree_birdchar_ (animated birds ?)
        MORPH_INDEXED = 3247952176
        VERTEX_FORMAT = 3263271920
        SIMPLE_MESH = 3566041216 'PRIMITIVE_TYPE
        ANIMATION_SKIN = 3751472158 'BONE_MATRICES
        COLLISION_TRI_MESH = 4034198449 ' ?
        HOTSPOT = 4116388378
        COLLISION = 4185470741
        'since FIFA 15
        CLOTH_DEF = 283785394
        SKELETON = 1881640942
        'since FIFA 16
        BONE_NAME = 137740398
        QUAD_INDEX_BUFFER = 191347397             'qib
        QUAD_INDEX_BUFFER_BATCH = 341785025    'quadibbatch
        ADJACENCY = 899336811
    End Enum

    Public Enum TextureFormat As Byte
        ' Fields
        ''' <summary>
        ''' BC1 / DXT1 with no alpha. Very widely supported and good compression ratio.
        ''' </summary>
        DXT1 = 0
        ''' <summary>
        ''' BC2 / DXT3 encoding with alpha. Good for sharp alpha transitions.
        ''' </summary>
        DXT3 = 1
        ''' <summary>
        ''' BC3 / DXT5 encoding with alpha. Good for smooth alpha transitions.
        ''' </summary>
        DXT5 = 2
        ''' <summary>
        ''' Raw unsigned byte 32-bit BGRA data.
        ''' </summary>
        B8G8R8A8 = 3            'ARGB
        ''' <summary>
        ''' Raw unsigned byte 8-bit Luminance data.
        ''' </summary>
        L8 = 4                  'GREY8   Luminence Map
        ''' <summary>
        ''' Raw unsigned byte 16-bit Luminance data with Alpha.
        ''' </summary>
        L8A8 = 5                'GREY8ALFA8
        ''' <summary>
        ''' Raw unsigned byte 32-bit RGBA data.
        ''' </summary>
        R8G8B8A8 = 6                'New added: width = width\2
        ''' <summary>
        ''' BC5 dual-channel encoding. Only red and green channels are encoded.
        ''' </summary>
        ATI2 = 7                'DC_XY_NORMAL_MAP, NVTT, also known as BC5 compression
        ''' <summary>
        ''' BC6H / BPTC unsigned float encoding. Can compress HDR textures without alpha. Does not support negative values.
        ''' </summary>
        BC6H_UF16 = 10          'found at FO4 sky textures : https://docs.microsoft.com/en-us/windows/win32/direct3d11/bc6h-format
        ''' <summary>
        ''' BC4 single-channel encoding. Only luminance is encoded.
        ''' </summary>
        ATI1 = 12               'also known as BC4 compression
        ''' <summary>
        ''' Texture format found at FIFA 16 Mobile games (no support).
        ''' </summary>
        UNKNOWN_FIFA16MOBILE = 104           'found at FIFA 16 mobile
        ''' <summary>
        ''' Raw unsigned byte 16-bit BGRA data.
        ''' </summary>
        B4G4R4A4 = 109
        ''' <summary>
        ''' Raw unsigned byte 16-bit BGR data. 5 bits for blue, 6 bits for green, and 5 bits for red.
        ''' </summary>
        B5G6R5 = 120
        ''' <summary>
        ''' Raw unsigned byte 16-bit BGRA data. 5 bits for each color channel and 1-bit alpha.
        ''' </summary>
        B5G5R5A1 = 126
        BIT8 = 123
        ''' <summary>
        ''' Raw unsigned byte 24-bit BGR data.
        ''' </summary>
        B8G8R8 = 127
    End Enum

    Public Module TextureFormatExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Function ToETextureFormat(ByVal Format As Rx3.TextureFormat) As ETextureFormat
            Select Case Format
            ' rx3 + RW4
                Case Rx3.TextureFormat.DXT1
                    Return ETextureFormat.BC1
                Case Rx3.TextureFormat.DXT3
                    Return ETextureFormat.BC2
                Case Rx3.TextureFormat.DXT5
                    Return ETextureFormat.BC3
                Case Rx3.TextureFormat.B8G8R8A8
                    Return ETextureFormat.B8G8R8A8
                Case Rx3.TextureFormat.L8
                    Return ETextureFormat.L8
                Case Rx3.TextureFormat.L8A8
                    Return ETextureFormat.L8A8
                Case Rx3.TextureFormat.ATI2
                    Return ETextureFormat.BC5
                Case Rx3.TextureFormat.B4G4R4A4
                    Return ETextureFormat.B4G4R4A4
                Case Rx3.TextureFormat.B5G6R5
                    Return ETextureFormat.B5G6R5
                Case Rx3.TextureFormat.B5G5R5A1
                    Return ETextureFormat.B5G5R5A1

            'RX3 only
                Case Rx3.TextureFormat.R8G8B8A8
                    Return ETextureFormat.R8G8B8A8
                Case Rx3.TextureFormat.ATI1
                    Return ETextureFormat.BC4
                Case Rx3.TextureFormat.BIT8
                    Return ETextureFormat.BIT8
                Case Rx3.TextureFormat.B8G8R8
                    Return ETextureFormat.B8G8R8
                Case Rx3.TextureFormat.BC6H_UF16
                    Return ETextureFormat.BC6H_UF16

                    'RW4 only
                    'Case Rx3.TextureFormat.DXT1NORMAL
                    'Case Rx3.TextureFormat.A32B32G32R32F

                Case Else
                    MsgBox("Unknown Rx3.TextureFormat found at function ""GetEFromRx3TextureFormat"": " & Format.ToString)
                    Return ETextureFormat.BC1
            End Select
        End Function

        <System.Runtime.CompilerServices.Extension>
        Public Function ToRwSurfaceFormat(ByVal NewRx3Format As Rx3.TextureFormat) As Rw.SurfaceFormat ', ByVal DefaultRWFormat As RWTextureFormat) 

            Select Case NewRx3Format
            ' rx3 + RW4
                Case Rx3.TextureFormat.DXT1
                    Return Rw.SurfaceFormat.FMT_DXT1
                Case Rx3.TextureFormat.DXT3
                    Return Rw.SurfaceFormat.FMT_DXT2_3
                Case Rx3.TextureFormat.DXT5
                    Return Rw.SurfaceFormat.FMT_DXT4_5
                Case Rx3.TextureFormat.B8G8R8A8
                    Return Rw.SurfaceFormat.FMT_8_8_8_8
                Case Rx3.TextureFormat.L8
                    Return Rw.SurfaceFormat.FMT_8
                Case Rx3.TextureFormat.L8A8
                    Return Rw.SurfaceFormat.FMT_8_8
                Case Rx3.TextureFormat.ATI2
                    Return Rw.SurfaceFormat.FMT_DXN
                Case Rx3.TextureFormat.B4G4R4A4
                    Return Rw.SurfaceFormat.FMT_4_4_4_4
                Case Rx3.TextureFormat.B5G6R5
                    Return Rw.SurfaceFormat.FMT_5_6_5
                Case Rx3.TextureFormat.B5G5R5A1
                    Return Rw.SurfaceFormat.FMT_1_5_5_5

                    'RX3 only
                    'Case Rx3.TextureFormat.RGBA
                    'Case Rx3.TextureFormat.ATI1
                    'Case Rx3.TextureFormat.BIT8
                    'Case Rx3.TextureFormat.R8G8B8
                    'case Rx3.TextureFormat.BC6H_UF16

                    'RW4 only
                    'DXT1NORMAL 
                    'A32B32G32R32F 

                Case Else
                    MsgBox("Unknown Rx3.TextureFormat found at function ""GetRWFromRx3TextureFormat"": " & NewRx3Format.ToString)
                    Return Rw.SurfaceFormat.FMT_DXT1
            End Select

        End Function
    End Module
End Namespace