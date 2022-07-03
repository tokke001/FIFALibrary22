Namespace Rw
    Public Enum SectionTypeCode As UInt32
        'RWOBJECTTYPE
        RWOBJECTTYPE_NULL = &H3
        RWOBJECTTYPE_NA = &H10000
        RWOBJECTTYPE_ARENA = &H10001
        RWOBJECTTYPE_RAW = &H10002
        RWOBJECTTYPE_SUBREFERENCE = &H10003
        RWOBJECTTYPE_SECTIONMANIFEST = &H10004        'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSectionManifest.java
        RWOBJECTTYPE_SECTIONTYPES = &H10005           'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSectionTypes.java
        RWOBJECTTYPE_SECTIONEXTERNALARENAS = &H10006 'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSectionExternalArenas.java
        RWOBJECTTYPE_SECTIONSUBREFERENCES = &H10007   'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSectionSubReferences.java
        RWOBJECTTYPE_SECTIONATOMS = &H10008           'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSectionAtoms.java
        RWOBJECTTYPE_DEFARENAIMPORTS = &H10009
        RWOBJECTTYPE_DEFARENAEXPORTS = &H1000A
        RWOBJECTTYPE_DEFARENATYPES = &H1000B
        RWOBJECTTYPE_DEFARENADEFINEDARENAID = &H1000C
        RWOBJECTTYPE_ATTRIBUTEPACKET = &H1000D
        RWOBJECTTYPE_ATTRIBUTEPACKET_DELEGATE = &H1000E
        RWOBJECTTYPE_BITTABLE = &H1000F
        RWOBJECTTYPE_ARENALOCALATOMTABLE = &H10010
        RWOBJECTTYPE_BASERESOURCE_START = &H10030       'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWBaseResource.java
        RWOBJECTTYPE_BUFFER = &H10031    'texture / vertex / index buffer
        RWOBJECTTYPE_UNKNOWN_1 = &H10032    'found at: stadiums, SectionsManifest > Types > TypesCodes
        RWOBJECTTYPE_UNKNOWN_2 = &H10033    'found at: stadiums, SectionsManifest > Types > TypesCodes
        RWOBJECTTYPE_BASERESOURCE_RESERVEDTO = &H1003F
        'OBJECTTYPE     'rw::oldanimation::
        OBJECTTYPE_NA = &H70000
        OBJECTTYPE_KEYFRAMEANIM = &H70001                'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWKeyframeAnim.java
        OBJECTTYPE_SKELETON = &H70002                      'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSkeleton.java
        OBJECTTYPE_ANIMATIONSKIN = &H70003 'BONE_MATRICES 'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWAnimationSkin.java
        OBJECTTYPE_INTERPOLATOR = &H70004
        OBJECTTYPE_FEATHERINTERPOLATOR = &H70005
        OBJECTTYPE_ONEBONEIK = &H70006
        OBJECTTYPE_TWOBONEIK = &H70007
        OBJECTTYPE_BLENDER = &H70008
        OBJECTTYPE_WEIGHTEDBLENDER = &H70009
        OBJECTTYPE_REMAPPER = &H7000A
        OBJECTTYPE_SKELETONSINK = &H7000B                'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSkeletonsInK.java
        OBJECTTYPE_SKINSINK = &H7000C                    'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSkinsInK.java
        OBJECTTYPE_LIGHTSINK = &H7000D
        OBJECTTYPE_CAMERASINK = &H7000E
        OBJECTTYPE_SKINMATRIXBUFFER = &H7000F            'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWSkinMatrixBuffer.java
        OBJECTTYPE_TWEAKCONTROLLER = &H70010
        OBJECTTYPE_SHADERSINK = &H70011
        'RWGOBJECTTYPE = rw::graphics::RwgObjectType
        RWGOBJECTTYPE_NA = &H20000
        RWGOBJECTTYPE_CAMERA = &H20001
        RWGOBJECTTYPE_PALETTE = &H20002
        RWGOBJECTTYPE_RASTER = &H20003    'or TEXTURE?        'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWRaster.java
        RWGOBJECTTYPE_VERTEXDESCRIPTOR = &H20004             'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWVertexDescription.java
        RWGOBJECTTYPE_VERTEXBUFFER = &H20005                 'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWVertexBuffer.java
        RWGOBJECTTYPE_INDEXDESCRIPTOR = &H20006
        RWGOBJECTTYPE_INDEXBUFFER = &H20007                  'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWIndexBuffer.java
        RWGOBJECTTYPE_LIGHT = &H20008
        RWGOBJECTTYPE_MESH = &H20009                          'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWMesh.java
        RWGOBJECTTYPE_SHADER = &H2000A
        RWGOBJECTTYPE_COMPILEDSTATE = &H2000B                'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWCompiledState.java
        RWGOBJECTTYPE_RENDEROBJECT = &H2000C
        RWGOBJECTTYPE_GSDATA = &H2000D
        RWGOBJECTTYPE_UPDATELOCATOR = &H2007F
        RWGOBJECTTYPE_VERTEXSHADER_2 = &H2000E
        RWGOBJECTTYPE_VERTEXDATA = &H2000F
        RWGOBJECTTYPE_INDEXDATA = &H20010
        RWGOBJECTTYPE_RASTERDATA = &H20011
        RWGOBJECTTYPE_BUILDSTATE = &H20012
        RWGOBJECTTYPE_PIXELSHADER = &H20013
        RWGOBJECTTYPE_VERTEXSHADER = &H20014
        RWGOBJECTTYPE_PROFILEMESH = &H20015
        RWGOBJECTTYPE_DESIGNVIEWOBJECT = &H20016
        RWGOBJECTTYPE_PROFILERENDEROBJECT = &H20017
        RWGOBJECTTYPE_IMAGE = &H20018
        RWGOBJECTTYPE_RENDEROBJECTCONTAINER = &H20019
        RWGOBJECTTYPE_MESHCOMPILEDSTATELINK = &H2001A      'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWMeshCompiledStateLink.java
        RWGOBJECTTYPE_SHADERCODE = &H2001B
        RWGOBJECTTYPE_FONT = &H20020
        RWGOBJECTTYPE_GLYPHTABLE = &H20021
        RWGOBJECTTYPE_KERNTABLE = &H20022
        RWGOBJECTTYPE_PAGETABLE = &H20023
        RWGOBJECTTYPE_RASTERTEXTURE = &H20024
        RWGOBJECTTYPE_FACENAME = &H20025
        'RWCOBJECTTYPE = rw::collision::ObjectType
        RWCOBJECTTYPE_VOLUME = &H80001
        RWCOBJECTTYPE_SIMPLEMAPPEDARRAY = &H80002
        RWCOBJECTTYPE_TRIANGLEKDTREEPROCEDURAL = &H80003   'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWTriangleKDTreeProcedural.java
        RWCOBJECTTYPE_KDTREEMAPPEDARRAY = &H80004
        RWCOBJECTTYPE_BBOX = &H80005                          'https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWBBox.java
        RWCOBJECTTYPE_CLUSTEREDMESH = &H80006
        RWCOBJECTTYPE_MESHOPAGGREGATE = &H80007
        RWCOBJECTTYPE_OCTREE = &H80008
        'ARENAID
        MODELINSTANCE_ARENAID = &HEB0000
        MODELSKELETON_ARENAID = &HEB0001
        MODELSKELETONPOSE_ARENAID = &HEB0002
        MODELRENDER_ARENAID = &HEB0003  'GROUP
        MODELCOLLISION_ARENAID = &HEB0004
        MESHTRICOLLISION_ARENAID = &HEB0005     'not added: not found at FIFA (bxd::tCollisionTriMesh)
        MESHSPHERECOLLISION_ARENAID = &HEB0006  'not added: not found at FIFA (bxd::tCollisionSphereMesh)
        SPLINE_ARENAID = &HEB0007
        SCENELAYER_ARENAID = &HEB0008
        LOCATION_ARENAID = &HEB0009
        LIGHT_ARENAID = &HEB000A                'not added: not found at FIFA
        CAMERA_ARENAID = &HEB000B
        ANIMSEQ_ARENAID = &HEB000C
        CHANNELCURVE_ARENAID = &HEB000D
        CULLINFO_ARENAID = &HEB000E
        'EA::FxShader
        EA_FxShader_ParameterBlockDescriptor = &HEF0000 'MATERIAL_DESCRIPTOR
        EA_FxShader_ParameterBlock = &HEF0001           'MATERIAL_DESCRIPTOR_DATA
        EA_FxShader_FxRenderableSimple = &HEF0004
        EA_FxShader_FxMaterial = &HEF0005   'MATERIAL_INDEX
        'EA
        EA_HOTSPOT = &HEC0003  'Kits hotspots
        EA_ArenaDictionary = &HEC0010   'NAME_SECTION

    End Enum

    Public Enum SurfaceFormat As Integer     '0x5679
        FMT_1_REVERSE = 0
        FMT_1 = 1
        FMT_8 = 2
        FMT_1_5_5_5 = 3
        FMT_5_6_5 = 4
        FMT_6_5_5 = 5
        FMT_8_8_8_8 = 6
        FMT_2_10_10_10 = 7
        FMT_8_A = 8
        FMT_8_B = 9
        FMT_8_8 = 10
        FMT_Cr_Y1_Cb_Y0_REP = 11
        FMT_Y1_Cr_Y0_Cb_REP = 12
        FMT_16_16_EDRAM = 13
        FMT_8_8_8_8_A = 14
        FMT_4_4_4_4 = 15
        FMT_10_11_11 = 16
        FMT_11_11_10 = 17
        FMT_DXT1 = 18
        FMT_DXT2_3 = 19
        FMT_DXT4_5 = 20
        FMT_16_16_16_16_EDRAM = 21
        FMT_24_8 = 22
        FMT_24_8_FLOAT = 23
        FMT_16 = 24
        FMT_16_16 = 25
        FMT_16_16_16_16 = 26
        FMT_16_EXPAND = 27
        FMT_16_16_EXPAND = 28
        FMT_16_16_16_16_EXPAND = 29
        FMT_16_FLOAT = 30
        FMT_16_16_FLOAT = 31
        FMT_16_16_16_16_FLOAT = 32
        FMT_32 = 33
        FMT_32_32 = 34
        FMT_32_32_32_32 = 35
        FMT_32_FLOAT = 36
        FMT_32_32_FLOAT = 37
        FMT_32_32_32_32_FLOAT = 38
        FMT_32_AS_8 = 39
        FMT_32_AS_8_8 = 40
        FMT_16_MPEG = 41
        FMT_16_16_MPEG = 42
        FMT_8_INTERLACED = 43
        FMT_32_AS_8_INTERLACED = 44
        FMT_32_AS_8_8_INTERLACED = 45
        FMT_16_INTERLACED = 46
        FMT_16_MPEG_INTERLACED = 47
        FMT_16_16_MPEG_INTERLACED = 48
        FMT_DXN = 49
        FMT_8_8_8_8_AS_16_16_16_16 = 50
        FMT_DXT1_AS_16_16_16_16 = 51
        FMT_DXT2_3_AS_16_16_16_16 = 52
        FMT_DXT4_5_AS_16_16_16_16 = 53
        FMT_2_10_10_10_AS_16_16_16_16 = 54
        FMT_10_11_11_AS_16_16_16_16 = 55
        FMT_11_11_10_AS_16_16_16_16 = 56
        FMT_32_32_32_FLOAT = 57
        FMT_DXT3A = 58
        FMT_DXT5A = 59
        FMT_CTX1 = 60
        FMT_DXT3A_AS_1_1_1_1 = 61
        FMT_8_8_8_8_GAMMA_EDRAM = 62
        FMT_2_10_10_10_FLOAT_EDRAM = 63

    End Enum

    Public Enum GPUDimension As Integer  'GPUDIMENSION (0x109f)
        DIMENSION_1D = 0
        DIMENSION_2D = 1
        DIMENSION_3D = 2
        DIMENSION_CUBEMAP = 3
        'DIMENSION_VOLUME = 4   'not confirmed, guessed ?!

    End Enum

    Public Enum BoneNameHash As UInt32
        'FIFA 11 pc only
        sleeveOutLf = &HDF1B6E51UI
        sleeveTopLf_end = &HCD363CA6UI
        sleeveBackLf = &H6D75ECCAUI
        sleeveBackLf_end = &H9E1D1740UI
        sleeveInLf = &H5B52082EUI
        sleeveBottomLf_end = &H7E368C08UI
        sleeveFrontLf = &H87630B78UI
        sleeveFrontLf_end = &H3CBF60FEUI
        ReversePec_Left = &H50B62091UI
        ScapLf = &HA9A53CAEUI
        LatUpLeft = &H4881AE0CUI
        sleeveInRt = &H4951EB86UI
        sleeveBottomRt_end = &H69325D28UI
        sleeveBackRt = &H83760F3AUI
        sleeveBackRt_end = &H254D8F10UI
        sleeveOutRt = &HE51B77DDUI
        sleeveTopRt_end = &HB8743086UI
        sleeveFrontRt = &H91631AA8UI
        sleeveFrontRt_end = &HA80BC44EUI
        ReversePec_Right = &HD198EF04UI
        ScapRt = &H97A52006UI
        LatUpRight = &HACDC7B0BUI
        elboReaderGrpLf = &H74445AFDUI
        elboReaderLf = &HD0E11DDCUI
        elboReaderGrpRt = &H62443E31UI
        elboReaderRt = &HB2E0EEF4UI
        BicepRollLeft = &HBFEC50C4UI
        BicepRollRight = &H8DCEF823UI
        PecLf = &H815BED01UI
        PecRt = &H675BC40DUI
        ShirtRoot = &H402D4A3UI
        shirtBackRoot = &H884C688EUI
        shirtBackTip = &HB279B5A5UI
        shirtLfRoot = &H15175E35UI
        shirtLfTip = &HC0DA30F4UI
        shirtFrontRoot = &H35D45D74UI
        shirtFrontTip = &H86390963UI
        shirtRtRoot = &H705303F9UI
        shirtRtTip = &H641C13E0UI
        RightLeg_Cloth = &H970D54F2UI
        RightLeg_Front_rotate = &HA4D80FDBUI
        RightLeg_Front_offset = &H1175257UI
        RightLeg_Front_counter = &H86022626UI
        RightLeg_Inside_Front_rotate = &HA6E06CD8UI
        RightLeg_Inside_Front_offset = &HA474D758UI
        RightLeg_Inside_Front_counter = &H87B161FUI
        RightLeg_Inside_Back_rotate = &H96C093EEUI
        RightLeg_Inside_Back_offset = &H79C3772EUI
        RightLeg_Inside_Back_counter = &HE3E6F891UI
        RightLeg_Back_rotate = &H3AA7E7A7UI
        RightLeg_Back_offset = &HB5E4668BUI
        RightLeg_Back_counter = &H4D08392UI
        RightLeg_Outside_Back_rotate = &HC953302BUI
        RightLeg_Outside_Back_offset = &H100493C7UI
        RightLeg_Outside_Back_counter = &HC9550616UI
        RightLeg_Outside_rotate = &H888DDD03UI
        RightLeg_Outside_offset = &H1B752DBFUI
        RightLeg_Outside_counter = &HC920BACEUI
        RightLeg_Outside_Front_rotate = &H1B4F78A7UI
        RightLeg_Outside_Front_offset = &H968BF78BUI
        RightLeg_Outside_Front_counter = &HAC99C692UI
        RightLeg_Inside_rotate = &H65338B24UI
        RightLeg_Inside_offset = &H4E053F0CUI
        RightLeg_Inside_counter = &HC897BF4BUI
        RightLegReader = &H41E6B3B2UI
        LeftLeg_Cloth = &H25A6FEC3UI
        LeftLeg_Front_rotate = &H9BADE550UI
        LeftLeg_Front_offset = &HE4D6ECB0UI
        LeftLeg_Front_counter = &H6DC37297UI
        LeftLeg_Outside_Front_rotate = &HDF065CC0UI
        LeftLeg_Outside_Front_offset = &H4037AC40UI
        LeftLeg_Outside_Front_counter = &H18E581A7UI
        LeftLeg_Outside_rotate = &HEE601C1CUI
        LeftLeg_Outside_offset = &H25E226C4UI
        LeftLeg_Outside_counter = &HDF60A8C3UI
        LeftLeg_Outside_Back_rotate = &HEDAF7CB6UI
        LeftLeg_Outside_Back_offset = &H758DBD6UI
        LeftLeg_Outside_Back_counter = &H90E3B6F9UI
        LeftLeg_Back_rotate = &HD72C84E6UI
        LeftLeg_Back_offset = &HC664F0E6UI
        LeftLeg_Back_counter = &H902EB7C9UI
        LeftLeg_Inside_Back_rotate = &HFD5E95B1UI
        LeftLeg_Inside_Back_offset = &H2C00B725UI
        LeftLeg_Inside_Back_counter = &H5B2440E8UI
        LeftLeg_Inside_Front_rotate = &HA68BE865UI
        LeftLeg_Inside_Front_offset = &H493EEA51UI
        LeftLeg_Inside_Front_counter = &H24AC60CCUI
        LeftLeg_Inside_rotate = &H7985B74DUI
        LeftLeg_Inside_offset = &H66B18B79UI
        LeftLeg_Inside_counter = &H67CA5B94UI
        LeftLegReader = &HB01CDA8FUI
        HipsReader = &HD8D77B2AUI
        hips_Loc = &HD7FEF290UI
        HipsController = &H3FBFF043UI
        LLeg_Loc = &H3733BF2EUI
        LeftLeg_Controller = &H20F0A679UI
        RLeg_Loc = &HCF4EAA64UI
        RightLeg_Controller = &H74C750FAUI
        LArm_Loc = &H676B6F5CUI
        LArmReader = &H4EB03AB6UI
        RArm_Loc = &H79704C7AUI
        RArmReader = &HD95246F8UI
        Head_Loc = &H2B95A8C8UI
        Hair_Readout = &HDD862240UI

        'FIFA 11 ps3 (ALL) = SkeletonImpactEngine
        Reference = &HBA3A6344UI
        AITrajectory = &H465A9F84UI
        Hips = &H27311C9FUI
        RightUpLeg = &H548F750EUI
        RightLeg = &H4D2DC107UI
        RightFoot = &H1C1DAC51UI
        RightToeBase = &H103428CUI
        RightFootEnd = &H879A6A30UI
        RightLegTwist = &H44C8ABA0UI
        Calf_Right_Helper = &H8B4DF453UI
        Calf_BackRight_Helper = &H1D0DF88EUI
        RightUpLegTwist = &HC14CEF1FUI
        RightUpLegTwist_Cloth = &H6969BFAAUI
        Right_Back_Shorts_Cloth = &HCF824B48UI
        Right_Back_Shorts_Cloth1 = &HF21C8269UI
        Right_Back_Shorts_Cloth2 = &HF21C826AUI
        Right_Inside_Shorts_Cloth = &HEF5E3343UI
        Right_Inside_Shorts_Cloth1 = &H144AB248UI
        Right_Inside_Shorts_Cloth2 = &H144AB24BUI
        Right_Front_Shorts_Cloth = &HC6ECD8E4UI
        Right_Front_Shorts_Cloth1 = &HAD96EDDUI
        Right_Front_Shorts_Cloth2 = &HAD96EDEUI
        Right_Outside_Shorts_Cloth = &HFD3C3D84UI
        Right_Outside_Shorts_Cloth1 = &H29D4D6FDUI
        Right_Outside_Shorts_Cloth2 = &H29D4D6FEUI
        RightButt_Helper = &H6A187999UI
        RightUpLegTwist1 = &H6B1C6DFCUI
        RightKnee_Helper = &H15F3044FUI
        RightLeg_PSC_Readout = &HE540FB45UI
        RightLeg_PSC_Readout1 = &H2A4B8DAEUI
        LeftUpLeg = &HBDF1ADE7UI
        LeftLeg = &HA666ED36UI
        LeftFoot = &HD26EEB32UI
        LeftToeBase = &H5D28C6BDUI
        LeftFootEnd = &HC769CC1UI
        LeftLegTwist = &H6B638447UI
        Calf_Left_Helper = &HDD7C5314UI
        Calf_BackLeft_Helper = &H1FD9EDA7UI
        LeftUpLegTwist = &HD7F65840UI
        LeftUpLegTwist_Cloth = &H9E7F4A21UI
        Left_Front_Shorts_Cloth = &H79A0C2C5UI
        Left_Front_Shorts_Cloth1 = &H3D129C2EUI
        Left_Front_Shorts_Cloth2 = &H3D129C2DUI
        Left_Inside_Shorts_Cloth = &H9F1BF89CUI
        Left_Inside_Shorts_Cloth1 = &H15085DA5UI
        Left_Inside_Shorts_Cloth2 = &H15085DA6UI
        Left_Back_Shorts_Cloth = &H168E8D57UI
        Left_Back_Shorts_Cloth1 = &HD9687FC4UI
        Left_Back_Shorts_Cloth2 = &HD9687FC7UI
        Left_Outside_Shorts_Cloth = &H5D0DF689UI
        Left_Outside_Shorts_Cloth1 = &H5FB199AUI
        Left_Outside_Shorts_Cloth2 = &H5FB1999UI
        LeftButt_Helper = &HB06E0CCCUI
        LeftUpLegTwist1 = &H38CCECF1UI
        LeftKnee_Helper = &H9F1E7276UI
        LeftLeg_PSC_Readout = &HEEE56024UI
        LeftLeg_PSC_Readout1 = &H3716589DUI
        Spine = &H7D572F96UI
        Spine1 = &HE63FE913UI
        Spine2 = &HE63FE910UI
        Spine3 = &HE63FE911UI
        LeftShoulder = &H3CA15756UI
        LeftArm = &HA9603628UI
        LeftForeArm = &HC284D726UI
        LeftArm_PSC_Readout = &H453FB75EUI
        LeftForeArmTwist = &H83E36F97UI
        LeftForeArmTwist1 = &H3608AA84UI
        LeftForeArmWrist = &HF309F73FUI
        LeftHand = &HC7E96519UI
        LeftHandThumb1 = &HB65B68B0UI
        LeftHandThumb2 = &HB65B68B3UI
        LeftHandThumb3 = &HB65B68B2UI
        LeftHandThumbEnd = &H9AF3E7A2UI
        LeftInHandIndex = &HFBEAEF50UI
        LeftHandIndex1 = &H43B16F00UI
        LeftHandIndex2 = &H43B16F03UI
        LeftHandIndex3 = &H43B16F02UI
        LeftHandIndexEnd = &HBFCC8592UI
        LeftInHandMiddle = &H2EF5674BUI
        LeftHandMiddle1 = &HEBA66CB7UI
        LeftHandMiddle2 = &HEBA66CB4UI
        LeftHandMiddle3 = &HEBA66CB5UI
        LeftHandMiddleEnd = &H609CEFD1UI
        LeftInHandRing = &H21B9899AUI
        LeftHandRing1 = &H35C688DCUI
        LeftHandRing2 = &H35C688DFUI
        LeftHandRing3 = &H35C688DEUI
        LeftHandRingEnd = &H4ADD82C6UI
        LeftInHandPinky = &H7B193081UI
        LeftHandPinky1 = &H57ACF777UI
        LeftHandPinky2 = &H57ACF774UI
        LeftHandPinky3 = &H57ACF775UI
        LeftHandPinkyEnd = &HE1AEED11UI
        LeftArmTwist = &H998BA31DUI
        LeftSleeve = &H3A2C1A3AUI
        LeftBicep = &H7D34B343UI
        LeftArmTwist1 = &HD3D1C696UI
        LeftElbow_Helper = &H8EB2F056UI
        LeftArmBare_Bicep_Helper = &HA98A4237UI
        LeftArmTwist2 = &HD3D1C695UI
        Left_Scapula_Helper = &HCF6D7ECFUI
        Left_Delt_Helper = &HBCC66B6FUI
        Left_Lat_Helper = &HB97FB34FUI
        RightShoulder = &H693846A5UI
        RightArm = &H7234B709UI
        RightForeArm = &H13C48AABUI
        RightArm_PSC_Readout = &HDE847DCFUI
        RightForeArmTwist = &HB85390C4UI
        RightForeArmTwist1 = &HEF8CE4BDUI
        RightForeArmWrist = &H4F90D4BCUI
        RightHand = &HD8D7B6B6UI
        RightHandThumb1 = &HB3F6A953UI
        RightHandThumb2 = &HB3F6A950UI
        RightHandThumb3 = &HB3F6A951UI
        RightHandThumbEnd = &H1F156935UI
        RightInHandIndex = &HB9C0E005UI
        RightHandIndex1 = &HFE776E07UI
        RightHandIndex2 = &HFE776E04UI
        RightHandIndex3 = &HFE776E05UI
        RightHandIndexEnd = &H9685AF41UI
        RightInHandMiddle = &H4F45D9CCUI
        RightHandMiddle1 = &H283A08B6UI
        RightHandMiddle2 = &H283A08B5UI
        RightHandMiddle3 = &H283A08B4UI
        RightHandMiddleEnd = &HA77D77FCUI
        RightInHandRing = &HF2D8A605UI
        RightHandRing1 = &HF66C21A9UI
        RightHandRing2 = &HF66C21AAUI
        RightHandRing3 = &HF66C21ABUI
        RightHandRingEnd = &H4E36A1CFUI
        RightInHandPinky = &H9DFB0CA0UI
        RightHandPinky1 = &HD7ACDF0CUI
        RightHandPinky2 = &HD7ACDF0FUI
        RightHandPinky3 = &HD7ACDF0EUI
        RightHandPinkyEnd = &H5563D96UI
        RightArmTwist = &H8983ECB6UI
        RightSleeve = &H7F64A291UI
        RightBicep = &H190A50FEUI
        RightArmTwist1 = &H30ADA2B3UI
        RightElbow_Helper = &H720769CDUI
        RightArmBare_Bicep_Helper = &H191DD27CUI
        RightArmTwist2 = &H30ADA2B0UI
        Right_Scapula_Helper = &H658D1C4AUI
        Right_Delt_Helper = &H3544F320UI
        Right_Lat_Helper = &H53681EBEUI
        Neck = &HA69AE588UI
        Neck1 = &HCDD75529UI
        Head = &H30131667UI
        HeadEnd = &H715E13AEUI
        Sway_Hair = &HEC7F85F0UI
        Left_Side_Hair = &HEC31220BUI
        Front_Hair = &H9DC790A1UI
        Full_Movement_Hair = &H5C9F5A21UI
        Top_Hair = &H434BA119UI
        Back_Hair = &HE4227A83UI
        Back_Hair1 = &HA546DC08UI
        Right_Side_Hair = &HBE5F712CUI
        Back_PonyTail_Hair = &H193A4B4CUI
        Back_PonyTail_Hair1 = &H2C48895UI
        Offset_Left_Throat_Helper = &HB5A75054UI
        Left_Throat_Helper = &HA1B79E66UI
        Offset_Right_Throat_Helper = &H18E3ACF3UI
        Right_Throat_Helper = &HF814F73DUI
        DoubleChin_Helper = &H23B4A041UI
        NeckBack_Helper = &HD9BB60A4UI
        Face = &H5C696F2UI
        Offset_Jaw = &HC25829E7UI
        Jaw = &H2E36959DUI
        Offset_LeftLowerLip = &H939B3292UI
        LeftLowerLip = &H9D8D3964UI
        Offset_LowerLip = &H124DB01DUI
        LowerLip = &H49E31663UI
        Offset_RightLowerLip = &HFDDD705UI
        RightLowerLip = &HF3E84DCFUI
        Offset_Tongue = &HCACC6E3UI
        Tongue = &H3D7DD919UI
        Offset_TongueTip = &H8DE41A4AUI
        TongueTip = &H2B65476CUI
        Offset_Chin = &HFA61CAF7UI
        Chin = &H6152AB49UI
        Offset_LeftEye = &H248E2313UI
        LeftEye = &HA655A455UI
        Offset_RightEye = &H158C1692UI
        RightEye = &H653F302CUI
        Offset_LeftUpCheek = &H24AA9C33UI
        LeftUpCheek = &H5CA2C5F5UI
        Offset_RightUpCheek = &H788E3F7AUI
        RightUpCheek = &H531FF8BCUI
        Offset_LeftCheek = &HEA7E940AUI
        LeftCheek = &H76DC6B54UI
        Offset_RightCheek = &HA48068BBUI
        RightCheek = &HE61EAB15UI
        Offset_LeftMouth = &HD842AE2DUI
        LeftMouth = &HE413DD93UI
        Offset_LeftUpperLip = &H845D7C97UI
        LeftUpperLip = &H7D222841UI
        Offset_UpperLip = &H633A0BB4UI
        UpperLip = &H251ABC2UI
        Offset_RightUpperLip = &HACAD699CUI
        RightUpperLip = &HB4C1EB2EUI
        Offset_RightMouth = &HC9B1A768UI
        RightMouth = &HBDC4E26EUI
        Offset_LeftUpEyelid = &H3B5B0819UI
        LeftUpEyelid = &HAA6A2987UI
        Offset_RightUpEyelid = &H5E9E816UI
        RightUpEyelid = &H86F211BCUI
        Offset_LeftLowEyelid = &HC99E32DCUI
        LeftLowEyelid = &HEBAEFF66UI
        Offset_RightLowEyelid = &H60BC45UI
        RightLowEyelid = &H35F31167UI
        Offset_LeftInnerEyebrow = &HDDE69515UI
        LeftInnerEyebrow = &HFCC8C0D7UI
        Offset_LeftOuterEyebrow = &HE1B9DEF6UI
        LeftOuterEyebrow = &H20AF0ACCUI
        Offset_RightInnerEyebrow = &HF7A7A44AUI
        RightInnerEyebrow = &HCF400714UI
        Offset_RightOuterEyebrow = &H460004D9UI
        RightOuterEyebrow = &HFB2F1BBFUI
        Offset_LeftNose = &H9E68E44DUI
        LeftNose = &H44DD42C7UI
        Offset_RightNose = &H6100E0A2UI
        RightNose = &HB95E297CUI
        Offset_LeftCrease = &HE9A5B0EDUI
        LeftCrease = &H20510487UI
        Offset_RightCrease = &HFEDD61CAUI
        RightCrease = &H7312B7ACUI
        NeckFront_Helper = &H12102940UI
        NeckFront_Helper1 = &HAF70EFF1UI
        Left_NeckFlex = &H34AE0D89UI
        Right_NeckFlex = &H7B15DE98UI
        Left_Pec_Helper = &HF79B11F6UI
        Right_Pec_Helper = &H16983D27UI
        Elbo_Reader_Left = &H5C8AA61UI
        Elbo_Reader_Right = &HF4A746D4UI
        LeftBackShirt_Helper = &HED73FDC4UI
        LeftBackShirt_Helper1 = &H91987BBDUI
        RightBackShirt_Helper = &H37C69F3BUI
        RightBackShirt_Helper1 = &H8ACA9D0UI
        LeftFrontShirt_Helper = &H98FA46D0UI
        LeftFrontShirt_Helper1 = &HA1FD7941UI
        RightFrontShirt_Helper = &H66FA28B5UI
        RightFrontShirt_Helper1 = &HD0CE14DEUI
        BackShirt_Helper = &HCFA9B40FUI
        BackShirt_Helper1 = &HF72673ACUI
        Crotch = &H2E7FD3B6UI
        Hips_PSC_Readout = &HC20E048DUI
        FrontShirt_Helper = &HABBCF421UI
        FrontShirt_Helper1 = &H7B744FC2UI
        Trajectory = &H3690720AUI
        TrajectoryEnd = &H19A1DA39UI
        LeftAnkleEffectorAux = &HCD1358BUI
        RightAnkleEffectorAux = &H98C77BC4UI

    End Enum


    Public Module TextureFormatHelpers
        <System.Runtime.CompilerServices.Extension>
        Public Function ToETextureFormat(ByVal Format As Rw.SurfaceFormat) As ETextureFormat ', ByVal DefaultRx3Format As ETextureFormat) 

            Select Case Format
            ' rx3 + RW4
                Case Rw.SurfaceFormat.FMT_DXT1
                    Return ETextureFormat.BC1
                Case Rw.SurfaceFormat.FMT_DXT2_3
                    Return ETextureFormat.BC2
                Case Rw.SurfaceFormat.FMT_DXT4_5
                    Return ETextureFormat.BC3
                Case Rw.SurfaceFormat.FMT_8_8_8_8
                    Return ETextureFormat.B8G8R8A8
                Case Rw.SurfaceFormat.FMT_8
                    Return ETextureFormat.L8
                Case Rw.SurfaceFormat.FMT_8_8
                    Return ETextureFormat.L8A8
                Case Rw.SurfaceFormat.FMT_DXN
                    Return ETextureFormat.BC5
                Case Rw.SurfaceFormat.FMT_4_4_4_4
                    Return ETextureFormat.B4G4R4A4
                Case Rw.SurfaceFormat.FMT_5_6_5
                    Return ETextureFormat.B5G6R5
                Case Rw.SurfaceFormat.FMT_1_5_5_5
                    Return ETextureFormat.B5G5R5A1

            'RX3 only
                'RGBA
                'ATI1
                'BIT8
                'R8G8B8
                'BC6H_UF16

            'RW4 only
                Case Rw.SurfaceFormat.FMT_CTX1
                    Return ETextureFormat.CTX1
                Case Rw.SurfaceFormat.FMT_32_32_32_32_FLOAT
                    Return ETextureFormat.R32G32B32A32Float

                Case Else
                    MsgBox("Unknown RWTextureFormat found at function ""GetEFromRWTextureFormat"": " & Format.ToString)
                    Return ETextureFormat.BC1
            End Select

        End Function

        <System.Runtime.CompilerServices.Extension>
        Public Function ToRx3TextureFormat(ByVal Format As Rw.SurfaceFormat) As Rx3.TextureFormat ', ByVal DefaultRx3Format As Rx3.TextureFormat) 

            Select Case Format
            ' rx3 + RW4
                Case Rw.SurfaceFormat.FMT_DXT1
                    Return Rx3.TextureFormat.DXT1
                Case Rw.SurfaceFormat.FMT_DXT2_3
                    Return Rx3.TextureFormat.DXT3
                Case Rw.SurfaceFormat.FMT_DXT4_5
                    Return Rx3.TextureFormat.DXT5
                Case Rw.SurfaceFormat.FMT_8_8_8_8
                    Return Rx3.TextureFormat.B8G8R8A8
                Case Rw.SurfaceFormat.FMT_8
                    Return Rx3.TextureFormat.L8
                Case Rw.SurfaceFormat.FMT_8_8
                    Return Rx3.TextureFormat.L8A8
                Case Rw.SurfaceFormat.FMT_DXN
                    Return Rx3.TextureFormat.ATI2
                Case Rw.SurfaceFormat.FMT_4_4_4_4
                    Return Rx3.TextureFormat.B4G4R4A4
                Case Rw.SurfaceFormat.FMT_5_6_5
                    Return Rx3.TextureFormat.B5G6R5
                Case Rw.SurfaceFormat.FMT_1_5_5_5
                    Return Rx3.TextureFormat.B5G5R5A1

                    'RX3 only
                    'RGBA
                    'ATI1
                    'BIT8
                    'R8G8B8
                    'BC6H_UF16

                    'RW4 only
                    'Case RWTextureFormat.DXT1NORMAL
                    'Case RWTextureFormat.A32B32G32R32F

                Case Else
                    MsgBox("Unknown RWTextureFormat found at function ""GetRx3FromRWTextureFormat"": " & Format.ToString)
                    Return Rx3.TextureFormat.DXT1
            End Select

        End Function
    End Module
End Namespace
