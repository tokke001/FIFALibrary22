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
        DXT1 = 0
        DXT3 = 1
        DXT5 = 2
        A8R8G8B8 = 3            'ARGB
        GREY8 = 4               'Luminence Map
        GREY8ALFA8 = 5
        RGBA = 6                'New added: width = width\2
        ATI2 = 7                'DC_XY_NORMAL_MAP, NVTT, also known as BC5 compression
        BC6H_UF16 = 10          'found at FO4 sky textures : https://docs.microsoft.com/en-us/windows/win32/direct3d11/bc6h-format
        ATI1 = 12               'also known as BC4 compression
        A4R4G4B4 = 109
        R5G6B5 = 120
        X1R5G5B5 = 126
        BIT8 = 123
        R8G8B8 = 127
    End Enum
End Namespace