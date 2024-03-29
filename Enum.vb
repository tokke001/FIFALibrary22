﻿
Imports BCnEncoder.Shared

Public Enum ETextureFormat As Byte
    ''' <summary>
    ''' Unknown format
    ''' </summary>
    UNKNOWN
    '-- rx3 + RW4
    ''' <summary>
    ''' BC1 / DXT1 with no alpha. Very widely supported and good compression ratio.
    ''' </summary>
    BC1
    ''' <summary>
    ''' BC2 / DXT3 encoding with alpha. Good for sharp alpha transitions.
    ''' </summary>
    BC2
    ''' <summary>
    ''' BC3 / DXT5 encoding with alpha. Good for smooth alpha transitions.
    ''' </summary>
    BC3
    ''' <summary>
    ''' Raw unsigned byte 32-bit BGRA data.
    ''' </summary>
    B8G8R8A8
    ''' <summary>
    ''' Raw unsigned byte 8-bit Luminance data.
    ''' </summary>
    L8      'GREY8  = DxgiFormatR8Unorm
    ''' <summary>
    ''' Raw unsigned byte 16-bit Luminance data with Alpha.
    ''' </summary>
    L8A8    'GREY8ALFA8
    ''' <summary>
    ''' BC5 dual-channel encoding. Only red and green channels are encoded.
    ''' </summary>
    BC5    'ATI2
    ''' <summary>
    ''' Raw unsigned byte 16-bit BGRA data.
    ''' </summary>
    B4G4R4A4
    ''' <summary>
    ''' Raw unsigned byte 16-bit BGR data. 5 bits for blue, 6 bits for green, and 5 bits for red.
    ''' </summary>
    B5G6R5
    ''' <summary>
    ''' Raw unsigned byte 16-bit BGRA data. 5 bits for each color channel and 1-bit alpha.
    ''' </summary>
    B5G5R5A1

    '-- RX3 only
    ''' <summary>
    ''' Raw unsigned byte 32-bit RGBA data.
    ''' </summary>
    R8G8B8A8
    ''' <summary>
    ''' BC4 single-channel encoding. Only luminance is encoded.
    ''' </summary>
    BC4    'ATI1
    BIT8
    ''' <summary>
    ''' Raw unsigned byte 24-bit BGR data.
    ''' Most texture formats do not support this format. Use <see cref="B8G8R8A8"/> instead.
    ''' </summary>
    B8G8R8
    ''' <summary>
    ''' BC6H / BPTC unsigned float encoding. Can compress HDR textures without alpha. Does not support negative values.
    ''' </summary>
    BC6H_UF16   'BC6

    '-- RW4 only
    CTX1 '= 124    'D3DFMT_CTX1    --->, same as ATI1 maybe?
    ''' <summary>
    ''' Raw floating point 32-bit-per-channel RGBA data.
    ''' </summary>
    R32G32B32A32Float '= 166 '128 size, float values ?? -> Found at game "UEFA Champions League 2006-2007" > file "stadium_159_6_container_0.rx2"

    '-- dds only
    ''' <summary>
    ''' BC7 / BPTC unorm encoding. Very high Quality rgba or rgb encoding. Also very slow.
    ''' </summary>
    BC7
End Enum

Public Enum ETextureType As Byte

    TEXTURE_1D = 0
    TEXTURE_2D = 1
    TEXTURE_3D = 2
    TEXTURE_CUBEMAP = 3     'texture consists of 6 faces
    TEXTURE_VOLUME = 4      'texture consists of multiple layers

End Enum

Public Enum SkeletonImpactEngine As UShort
    Reference = 0
    AITrajectory = 1
    Hips = 2
    RightUpLeg = 3
    RightLeg = 4
    RightFoot = 5
    RightToeBase = 6
    RightFootEnd = 7
    RightLegTwist = 8
    Calf_Right_Helper = 9
    Calf_BackRight_Helper = 10
    RightUpLegTwist = 11
    RightUpLegTwist_Cloth = 12
    Right_Back_Shorts_Cloth = 13
    Right_Back_Shorts_Cloth1 = 14
    Right_Back_Shorts_Cloth2 = 15
    Right_Inside_Shorts_Cloth = 16
    Right_Inside_Shorts_Cloth1 = 17
    Right_Inside_Shorts_Cloth2 = 18
    Right_Front_Shorts_Cloth = 19
    Right_Front_Shorts_Cloth1 = 20
    Right_Front_Shorts_Cloth2 = 21
    Right_Outside_Shorts_Cloth = 22
    Right_Outside_Shorts_Cloth1 = 23
    Right_Outside_Shorts_Cloth2 = 24
    RightButt_Helper = 25
    RightUpLegTwist1 = 26
    RightKnee_Helper = 27
    RightLeg_PSC_Readout = 28
    RightLeg_PSC_Readout1 = 29
    LeftUpLeg = 30
    LeftLeg = 31
    LeftFoot = 32
    LeftToeBase = 33
    LeftFootEnd = 34
    LeftLegTwist = 35
    Calf_Left_Helper = 36
    Calf_BackLeft_Helper = 37
    LeftUpLegTwist = 38
    LeftUpLegTwist_Cloth = 39
    Left_Front_Shorts_Cloth = 40
    Left_Front_Shorts_Cloth1 = 41
    Left_Front_Shorts_Cloth2 = 42
    Left_Inside_Shorts_Cloth = 43
    Left_Inside_Shorts_Cloth1 = 44
    Left_Inside_Shorts_Cloth2 = 45
    Left_Back_Shorts_Cloth = 46
    Left_Back_Shorts_Cloth1 = 47
    Left_Back_Shorts_Cloth2 = 48
    Left_Outside_Shorts_Cloth = 49
    Left_Outside_Shorts_Cloth1 = 50
    Left_Outside_Shorts_Cloth2 = 51
    LeftButt_Helper = 52
    LeftUpLegTwist1 = 53
    LeftKnee_Helper = 54
    LeftLeg_PSC_Readout = 55
    LeftLeg_PSC_Readout1 = 56
    Spine = 57
    Spine1 = 58
    Spine2 = 59
    Spine3 = 60
    LeftShoulder = 61
    LeftArm = 62
    LeftForeArm = 63
    LeftArm_PSC_Readout = 64
    LeftForeArmTwist = 65
    LeftForeArmTwist1 = 66
    LeftForeArmWrist = 67
    LeftHand = 68
    LeftHandThumb1 = 69
    LeftHandThumb2 = 70
    LeftHandThumb3 = 71
    LeftHandThumbEnd = 72
    LeftInHandIndex = 73
    LeftHandIndex1 = 74
    LeftHandIndex2 = 75
    LeftHandIndex3 = 76
    LeftHandIndexEnd = 77
    LeftInHandMiddle = 78
    LeftHandMiddle1 = 79
    LeftHandMiddle2 = 80
    LeftHandMiddle3 = 81
    LeftHandMiddleEnd = 82
    LeftInHandRing = 83
    LeftHandRing1 = 84
    LeftHandRing2 = 85
    LeftHandRing3 = 86
    LeftHandRingEnd = 87
    LeftInHandPinky = 88
    LeftHandPinky1 = 89
    LeftHandPinky2 = 90
    LeftHandPinky3 = 91
    LeftHandPinkyEnd = 92
    LeftArmTwist = 93
    LeftSleeve = 94
    LeftBicep = 95
    LeftArmTwist1 = 96
    LeftElbow_Helper = 97
    LeftArmBare_Bicep_Helper = 98
    LeftArmTwist2 = 99
    Left_Scapula_Helper = 100
    Left_Delt_Helper = 101
    Left_Lat_Helper = 102
    RightShoulder = 103
    RightArm = 104
    RightForeArm = 105
    RightArm_PSC_Readout = 106
    RightForeArmTwist = 107
    RightForeArmTwist1 = 108
    RightForeArmWrist = 109
    RightHand = 110
    RightHandThumb1 = 111
    RightHandThumb2 = 112
    RightHandThumb3 = 113
    RightHandThumbEnd = 114
    RightInHandIndex = 115
    RightHandIndex1 = 116
    RightHandIndex2 = 117
    RightHandIndex3 = 118
    RightHandIndexEnd = 119
    RightInHandMiddle = 120
    RightHandMiddle1 = 121
    RightHandMiddle2 = 122
    RightHandMiddle3 = 123
    RightHandMiddleEnd = 124
    RightInHandRing = 125
    RightHandRing1 = 126
    RightHandRing2 = 127
    RightHandRing3 = 128
    RightHandRingEnd = 129
    RightInHandPinky = 130
    RightHandPinky1 = 131
    RightHandPinky2 = 132
    RightHandPinky3 = 133
    RightHandPinkyEnd = 134
    RightArmTwist = 135
    RightSleeve = 136
    RightBicep = 137
    RightArmTwist1 = 138
    RightElbow_Helper = 139
    RightArmBare_Bicep_Helper = 140
    RightArmTwist2 = 141
    Right_Scapula_Helper = 142
    Right_Delt_Helper = 143
    Right_Lat_Helper = 144
    Neck = 145
    Neck1 = 146
    Head = 147
    HeadEnd = 148
    Sway_Hair = 149
    Left_Side_Hair = 150
    Front_Hair = 151
    Full_Movement_Hair = 152
    Top_Hair = 153
    Back_Hair = 154
    Back_Hair1 = 155
    Right_Side_Hair = 156
    Back_PonyTail_Hair = 157
    Back_PonyTail_Hair1 = 158
    Offset_Left_Throat_Helper = 159
    Left_Throat_Helper = 160
    Offset_Right_Throat_Helper = 161
    Right_Throat_Helper = 162
    DoubleChin_Helper = 163
    NeckBack_Helper = 164
    Face = 165
    Offset_Jaw = 166
    Jaw = 167
    Offset_LeftLowerLip = 168
    LeftLowerLip = 169
    Offset_LowerLip = 170
    LowerLip = 171
    Offset_RightLowerLip = 172
    RightLowerLip = 173
    Offset_Tongue = 174
    Tongue = 175
    Offset_TongueTip = 176
    TongueTip = 177
    Offset_Chin = 178
    Chin = 179
    Offset_LeftEye = 180
    LeftEye = 181
    Offset_RightEye = 182
    RightEye = 183
    Offset_LeftUpCheek = 184
    LeftUpCheek = 185
    Offset_RightUpCheek = 186
    RightUpCheek = 187
    Offset_LeftCheek = 188
    LeftCheek = 189
    Offset_RightCheek = 190
    RightCheek = 191
    Offset_LeftMouth = 192
    LeftMouth = 193
    Offset_LeftUpperLip = 194
    LeftUpperLip = 195
    Offset_UpperLip = 196
    UpperLip = 197
    Offset_RightUpperLip = 198
    RightUpperLip = 199
    Offset_RightMouth = 200
    RightMouth = 201
    Offset_LeftUpEyelid = 202
    LeftUpEyelid = 203
    Offset_RightUpEyelid = 204
    RightUpEyelid = 205
    Offset_LeftLowEyelid = 206
    LeftLowEyelid = 207
    Offset_RightLowEyelid = 208
    RightLowEyelid = 209
    Offset_LeftInnerEyebrow = 210
    LeftInnerEyebrow = 211
    Offset_LeftOuterEyebrow = 212
    LeftOuterEyebrow = 213
    Offset_RightInnerEyebrow = 214
    RightInnerEyebrow = 215
    Offset_RightOuterEyebrow = 216
    RightOuterEyebrow = 217
    Offset_LeftNose = 218
    LeftNose = 219
    Offset_RightNose = 220
    RightNose = 221
    Offset_LeftCrease = 222
    LeftCrease = 223
    Offset_RightCrease = 224
    RightCrease = 225
    NeckFront_Helper = 226
    NeckFront_Helper1 = 227
    Left_NeckFlex = 228
    Right_NeckFlex = 229
    Left_Pec_Helper = 230
    Right_Pec_Helper = 231
    Elbo_Reader_Left = 232
    Elbo_Reader_Right = 233
    LeftBackShirt_Helper = 234
    LeftBackShirt_Helper1 = 235
    RightBackShirt_Helper = 236
    RightBackShirt_Helper1 = 237
    LeftFrontShirt_Helper = 238
    LeftFrontShirt_Helper1 = 239
    RightFrontShirt_Helper = 240
    RightFrontShirt_Helper1 = 241
    BackShirt_Helper = 242
    BackShirt_Helper1 = 243
    Crotch = 244
    Hips_PSC_Readout = 245
    FrontShirt_Helper = 246
    FrontShirt_Helper1 = 247
    Trajectory = 248
    TrajectoryEnd = 249
    LeftAnkleEffectorAux = 250
    RightAnkleEffectorAux = 251

End Enum

Public Enum SkeletonFIFA15 As UShort
    Reference = 0
    AITrajectory = 1
    Hips = 2
    Spine = 3
    Spine1 = 4
    Spine2 = 5
    Spine3 = 6
    Neck = 7
    Neck1 = 8
    Head = 9
    HeadEnd = 10
    RM_NeckBack_Upper = 11
    RM_Sway_Hair = 12
    RM_Left_Side_Hair = 13
    RM_Front_Hair = 14
    RM_Full_Movement_Hair = 15
    RM_Top_Hair = 16
    RM_Back_Hair_01 = 17
    RM_Back_Hair_02 = 18
    RM_Right_Side_Hair = 19
    RM_Back_PonyTail_Hair_01 = 20
    RM_Back_PonyTail_Hair_02 = 21
    Face = 22
    Jaw = 23
    Tongue = 24
    TongueMiddle = 25
    TongueTip = 26
    LowerTeeth = 27
    JawClench = 28
    Chin = 29
    LeftUpInnerCheek = 30
    LeftUpperNasalFold = 31
    LeftMiddleNasalFold = 32
    LeftMidCheek = 33
    LeftCheek = 34
    LeftLowCheek = 35
    RightUpInnerCheek = 36
    RightUpperNasalFold = 37
    RightMiddleNasalFold = 38
    RightMidCheek = 39
    RightCheek = 40
    RightLowCheek = 41
    MiddleNose = 42
    LeftNose = 43
    RightNose = 44
    MiddleNoseCartilage = 45
    LowerLip = 46
    UpperMiddleBrow = 47
    LeftInnerEyebrow = 48
    LeftMiddleEyebrow = 49
    LeftOuterEyebrow = 50
    LeftLowMiddleEyebrow = 51
    LeftLowOuterEyebrow = 52
    LeftOuterEyeCorner = 53
    LeftUpCheek = 54
    LeftCheekFold = 55
    RightInnerEyebrow = 56
    RightMiddleEyebrow = 57
    RightOuterEyebrow = 58
    RightLowMiddleEyebrow = 59
    RightLowOuterEyebrow = 60
    RightOuterEyeCorner = 61
    RightUpCheek = 62
    RightCheekFold = 63
    LeftUpEyelid = 64
    LeftLowEyelid = 65
    LeftLowEyelidCrease = 66
    RightUpEyelid = 67
    RightLowEyelid = 68
    RightLowEyelidCrease = 69
    LeftEye = 70
    RightEye = 71
    UpperLipRoll = 72
    UpperLipTip = 73
    LowerLipRoll = 74
    LowerLipTip = 75
    LeftMouthLowerRoll = 76
    LeftMouthLowerTip = 77
    LeftMouthUpperRoll = 78
    LeftMouthUpperTip = 79
    RightMouthUpperRoll = 80
    RightMouthUpperTip = 81
    RightMouthLowerRoll = 82
    RightMouthLowerTip = 83
    LeftUpperLipRoll = 84
    LeftUpperLipTip = 85
    LeftLowerLipRoll = 86
    LeftLowerLipTip = 87
    RightUpperLipRoll = 88
    RightUpperLipTip = 89
    RightLowerLipRoll = 90
    RightLowerLipTip = 91
    LeftMouth = 92
    RightMouth = 93
    LeftUpperLip = 94
    RightUpperLip = 95
    LeftLowerLip = 96
    RightLowerLip = 97
    UpperLip = 98
    RM_LeftSterno_Offset = 99
    RM_LeftSterno = 100
    RM_LeftSterno_End = 101
    RM_RightSterno_Offset = 102
    RM_RightSterno = 103
    RM_RightSterno_End = 104
    RM_Hyoid = 105
    RM_NeckBack_Lower = 106
    RM_AdamsApple = 107
    RM_AdamsApple_End = 108
    RM_LeftTrap_Offset = 109
    RM_LeftTrap = 110
    RM_LeftTrap_End = 111
    RM_RightTrap_Offset = 112
    RM_RightTrap = 113
    RM_RightTrap_End = 114
    LeftShoulder = 115
    LeftArm = 116
    LeftForeArm = 117
    LeftHand = 118
    LeftHandThumb1 = 119
    LeftHandThumb2 = 120
    LeftHandThumb3 = 121
    LeftHandThumbEnd = 122
    LeftInHandIndex = 123
    LeftHandIndex1 = 124
    LeftHandIndex2 = 125
    LeftHandIndex3 = 126
    LeftHandIndexEnd = 127
    LeftInHandMiddle = 128
    LeftHandMiddle1 = 129
    LeftHandMiddle2 = 130
    LeftHandMiddle3 = 131
    LeftHandMiddleEnd = 132
    LeftInHandRing = 133
    LeftHandRing1 = 134
    LeftHandRing2 = 135
    LeftHandRing3 = 136
    LeftHandRingEnd = 137
    LeftInHandPinky = 138
    LeftHandPinky1 = 139
    LeftHandPinky2 = 140
    LeftHandPinky3 = 141
    LeftHandPinkyEnd = 142
    RM_LeftForeArmTwist1 = 143
    RM_LeftForeArmTwist1_Root = 144
    RM_LeftForeArmTwist1_Lower_Offset = 145
    RM_LeftForeArmTwist1_Lower = 146
    RM_LeftRadialis_Lower_Offset = 147
    RM_LeftRadialis_Lower = 148
    RM_LeftForeArmTwist2 = 149
    RM_LeftForeArmTwist2_Root = 150
    RM_LeftRadius_Offset = 151
    RM_LeftRadius = 152
    RM_LeftUlna_Offset = 153
    RM_LeftUlna = 154
    RM_LeftForeArmTwist3 = 155
    RM_LeftForeArmTwist3_Root = 156
    RM_LeftWristCorrect_Upper = 157
    RM_LeftWristCorrect_Lower = 158
    RM_LeftArmTwist1 = 159
    RM_LeftArmTwist1_Root = 160
    RM_LeftPecReverse_Offset = 161
    RM_LeftPecReverse = 162
    RM_LeftPecReverse_End = 163
    RM_LeftBackArmCorrect_Offset = 164
    RM_LeftBackArmCorrect = 165
    RM_LeftBackArmCorrect_End = 166
    RM_LeftArmTwist2 = 167
    RM_LeftArmTwist2_Root = 168
    RM_LeftBicep_Upper_Offset = 169
    RM_LeftBicep_Upper = 170
    RM_LeftTricep_Offset = 171
    RM_LeftTricep = 172
    RM_LeftArmTwist3 = 173
    RM_LeftArmTwist3_Root = 174
    RM_LeftBicep_Lower_Offset = 175
    RM_LeftBicep_Lower = 176
    RM_LeftRadialis_Upper_Offset = 177
    RM_LeftRadialis_Upper = 178
    RM_LeftElbow = 179
    RM_LeftDelt_All = 180
    RM_LeftDelt_Offset = 181
    RM_LeftDelt = 182
    RM_LeftDelt_End = 183
    RM_LeftScap_Offset = 184
    RM_LeftScap = 185
    RM_LeftScap_End = 186
    RightShoulder = 187
    RightArm = 188
    RightForeArm = 189
    RightHand = 190
    RightHandThumb1 = 191
    RightHandThumb2 = 192
    RightHandThumb3 = 193
    RightHandThumbEnd = 194
    RightInHandIndex = 195
    RightHandIndex1 = 196
    RightHandIndex2 = 197
    RightHandIndex3 = 198
    RightHandIndexEnd = 199
    RightInHandMiddle = 200
    RightHandMiddle1 = 201
    RightHandMiddle2 = 202
    RightHandMiddle3 = 203
    RightHandMiddleEnd = 204
    RightInHandRing = 205
    RightHandRing1 = 206
    RightHandRing2 = 207
    RightHandRing3 = 208
    RightHandRingEnd = 209
    RightInHandPinky = 210
    RightHandPinky1 = 211
    RightHandPinky2 = 212
    RightHandPinky3 = 213
    RightHandPinkyEnd = 214
    RM_RightForeArmTwist1 = 215
    RM_RightForeArmTwist1_Root = 216
    RM_RightForeArmTwist1_Lower_Offset = 217
    RM_RightForeArmTwist1_Lower = 218
    RM_RightRadialis_Lower_Offset = 219
    RM_RightRadialis_Lower = 220
    RM_RightForeArmTwist2 = 221
    RM_RightForeArmTwist2_Root = 222
    RM_RightUlna_Offset = 223
    RM_RightUlna = 224
    RM_RightRadius_Offset = 225
    RM_RightRadius = 226
    RM_RightForeArmTwist3 = 227
    RM_RightForeArmTwist3_Root = 228
    RM_RightWristCorrect_Upper = 229
    RM_RightWristCorrect_Lower = 230
    RM_RightArmTwist1 = 231
    RM_RightArmTwist1_Root = 232
    RM_RightPecReverse_Offset = 233
    RM_RightPecReverse = 234
    RM_RightPecReverse_End = 235
    RM_RightBackArmCorrect_Offset = 236
    RM_RightBackArmCorrect = 237
    RM_RightBackArmCorrect_End = 238
    RM_RightArmTwist2 = 239
    RM_RightArmTwist2_Root = 240
    RM_RightBicep_Upper_Offset = 241
    RM_RightBicep_Upper = 242
    RM_RightTricep_Offset = 243
    RM_RightTricep = 244
    RM_RightArmTwist3 = 245
    RM_RightArmTwist3_Root = 246
    RM_RightBicep_Lower_Offset = 247
    RM_RightBicep_Lower = 248
    RM_RightRadialis_Upper_Offset = 249
    RM_RightRadialis_Upper = 250
    RM_RightElbow = 251
    RM_RightDelt_All = 252
    RM_RightDelt_Offset = 253
    RM_RightDelt = 254
    RM_RightDelt_End = 255
    RM_RightScap_Offset = 256
    RM_RightScap = 257
    RM_RightScap_End = 258
    RM_Spine3_Jiggle = 259
    RM_RightArmPit = 260
    RM_LeftArmPit = 261
    RM_LeftLat_Upper_Offset = 262
    RM_LeftLat_Upper = 263
    RM_LeftLat_Upper_End = 264
    RM_RightLat_Upper_Offset = 265
    RM_RightLat_Upper = 266
    RM_RightLat_Upper_End = 267
    RM_MiddleBreather = 268
    RM_LeftBreather = 269
    RM_LeftPecAll = 270
    RM_LeftUpperPec_Offset = 271
    RM_LeftUpperPec = 272
    RM_LeftUpperPec_End = 273
    RM_LeftLowerPec_Offset = 274
    RM_LeftLowerPec = 275
    RM_LeftLowerPec_End = 276
    RM_RightBreather = 277
    RM_RightPecAll = 278
    RM_RightLowerPec_Offset = 279
    RM_RightLowerPec = 280
    RM_RightLowerPec_End = 281
    RM_RightUpperPec_Offset = 282
    RM_RightUpperPec = 283
    RM_RightUpperPec_End = 284
    RM_Spine2_Jiggle = 285
    RM_LeftLat_Middle_Offset = 286
    RM_LeftLat_Middle = 287
    RM_LeftLat_Middle_End = 288
    RM_RightLat_Middle_Offset = 289
    RM_RightLat_Middle = 290
    RM_RightLat_Middle_End = 291
    RM_Spine1_Jiggle = 292
    RM_LeftLat_Lower_Offset = 293
    RM_LeftLat_Lower = 294
    RM_LeftLat_Lower_End = 295
    RM_RightLat_Lower_Offset = 296
    RM_RightLat_Lower = 297
    RM_RightLat_Lower_End = 298
    RM_LeftRibCage_Offset = 299
    RM_LeftRibCage = 300
    RM_RightRibCage_Offset = 301
    RM_RightRibCage = 302
    RM_BellyBreather2_Offset = 303
    RM_BellyBreather2 = 304
    RM_Spine_Jiggle = 305
    RM_BellyBreather1_Offset = 306
    RM_BellyBreather1 = 307
    LeftUpLeg = 308
    LeftLeg = 309
    LeftFoot = 310
    LeftToeBase = 311
    LeftFootEnd = 312
    RM_LeftAnkleTwist = 313
    RM_LeftAnkleTwist_Root = 314
    RM_LeftLegTwist = 315
    RM_LeftLegTwist_Root = 316
    RM_LeftLegInnerTendon_Lower = 317
    RM_LeftLegOuterTendon_Lower = 318
    RM_LeftInnerCalf_Offset = 319
    RM_LeftInnerCalf = 320
    RM_LeftOuterCalf_Offset = 321
    RM_LeftOuterCalf = 322
    RM_LeftKnee_Lower = 323
    RM_LeftUpLegTwist1 = 324
    RM_LeftUpLegTwist1_Root = 325
    RM_LeftSartorius = 326
    RM_LeftUpLegTwist2 = 327
    RM_LeftUpLegTwist2_Root = 328
    RM_LeftThigh_Offset = 329
    RM_LeftThigh = 330
    RM_LeftUpLegTwist3 = 331
    RM_LeftUpLegTwist3_Root = 332
    RM_LeftLegInnerTendon_Upper_Offset = 333
    RM_LeftLegInnerTendon_Upper = 334
    RM_LeftLegOuterTendon_Upper_Offset = 335
    RM_LeftLegOuterTendon_Upper = 336
    RM_LeftCondyle_Offset = 337
    RM_LeftCondyle = 338
    RM_LeftKnee_Upper = 339
    RightUpLeg = 340
    RightLeg = 341
    RightFoot = 342
    RightToeBase = 343
    RightFootEnd = 344
    RM_RightAnkleTwist = 345
    RM_RightAnkleTwist_Root = 346
    RM_RightLegTwist = 347
    RM_RightLegTwist_Root = 348
    RM_RightLegInnerTendon_Lower = 349
    RM_RightLegOuterTendon_Lower = 350
    RM_RightOuterCalf_Offset = 351
    RM_RightOuterCalf = 352
    RM_RightInnerCalf_Offset = 353
    RM_RightInnerCalf = 354
    RM_RightKnee_Lower = 355
    RM_RightUpLegTwist1 = 356
    RM_RightUpLegTwist1_Root = 357
    RM_RightSartorius = 358
    RM_RightUpLegTwist2 = 359
    RM_RightUpLegTwist2_Root = 360
    RM_RightThigh_Offset = 361
    RM_RightThigh = 362
    RM_RightUpLegTwist3 = 363
    RM_RightUpLegTwist3_Root = 364
    RM_RightLegInnerTendon_Upper_Offset = 365
    RM_RightLegInnerTendon_Upper = 366
    RM_RightLegOuterTendon_Upper_Offset = 367
    RM_RightLegOuterTendon_Upper = 368
    RM_RightCondyle_Offset = 369
    RM_RightCondyle = 370
    RM_RightKnee_Upper = 371
    RM_LeftShirt_Front = 372
    RM_LeftShirt_Side = 373
    RM_LeftButt_Offset = 374
    RM_LeftButt = 375
    RM_RightShirt_Front = 376
    RM_RightShirt_Side = 377
    RM_RightButt_Offset = 378
    RM_RightButt = 379
    RM_Shorts_Crotch = 380
    RM_Shorts_Back = 381
    RM_Shorts_Crack = 382
    RM_RightShirt_Back = 383
    RM_LeftShirt_Back = 384
    RM_LeftShorts = 385
    RM_RightShorts = 386
    Trajectory = 387
    TrajectoryEnd = 388
    Prop = 389
    LeftAnkleEffectorAux = 390
    RightAnkleEffectorAux = 391

End Enum

Public Enum SkeletonFrostbite As UShort
    Reference = 0
    AITrajectory = 1
    Hips = 2
    Spine = 3
    Spine1 = 4
    Spine2 = 5
    Spine3 = 6
    Neck = 7
    Neck1 = 8
    Head = 9
    HeadEnd = 10
    RM_LeftSterno_Offset = 11
    RM_LeftSterno = 12
    RM_LeftSterno_End = 13
    RM_RightSterno_Offset = 14
    RM_RightSterno = 15
    RM_RightSterno_End = 16
    Face = 17
    Jaw = 18
    Tongue = 19
    TongueTip = 20
    TongueMiddle = 21
    LowerTeeth = 22
    JawClench = 23
    Chin = 24
    ChinPuff = 25
    LeftUpInnerCheek = 26
    LeftUpperNasalFold = 27
    LeftMiddleNasalFold = 28
    LeftMidCheek = 29
    LeftCheek = 30
    LeftLowCheek = 31
    RightUpInnerCheek = 32
    RightUpperNasalFold = 33
    RightMiddleNasalFold = 34
    RightMidCheek = 35
    RightCheek = 36
    RightLowCheek = 37
    MiddleNose = 38
    LeftNose = 39
    RightNose = 40
    MiddleNoseCartilage = 41
    UpperLipPuff = 42
    UpperLip = 43
    UpperLipTip = 44
    LowerLip = 45
    LowerLipTip = 46
    LeftUpperLip = 47
    LeftUpperLipTip = 48
    LeftLowerLip = 49
    LeftLowerLipTip = 50
    RightUpperLip = 51
    RightUpperLipTip = 52
    RightLowerLip = 53
    RightLowerLipTip = 54
    LeftMouth = 55
    LeftMouthUpper = 56
    LeftMouthLower = 57
    RightMouth = 58
    RightMouthUpper = 59
    RightMouthLower = 60
    UpperMiddleBrow = 61
    LeftInnerEyebrow = 62
    LeftMiddleEyebrow = 63
    LeftOuterEyebrow = 64
    LeftLowMiddleEyebrow = 65
    LeftLowOuterEyebrow = 66
    LeftOuterEyeCorner = 67
    LeftUpCheek = 68
    LeftCheekFold = 69
    RightInnerEyebrow = 70
    RightMiddleEyebrow = 71
    RightOuterEyebrow = 72
    RightLowMiddleEyebrow = 73
    RightLowOuterEyebrow = 74
    RightOuterEyeCorner = 75
    RightUpCheek = 76
    RightCheekFold = 77
    LeftUpEyelid = 78
    LeftLowEyelid = 79
    LeftLowEyelidCrease = 80
    RightUpEyelid = 81
    RightLowEyelid = 82
    RightLowEyelidCrease = 83
    LeftEye = 84
    RightEye = 85
    LeftCheekFat = 86
    RightCheekFat = 87
    RM_Neck1_Jiggle = 88
    RM_Hyoid = 89
    RM_AdamsApple = 90
    RM_AdamsApple_End = 91
    RM_NeckBack_Upper = 92
    RM_Right_NeckHelper = 93
    RM_Left_NeckHelper = 94
    RM_LeftTrap_Offset = 95
    RM_LeftTrap = 96
    RM_LeftTrap_Helper_Offset = 97
    RM_LeftTrap_Helper = 98
    RM_LeftTrap_End = 99
    RM_RightTrap_Offset = 100
    RM_RightTrap = 101
    RM_RightTrap_Helper_Offset = 102
    RM_RightTrap_Helper = 103
    RM_RightTrap_End = 104
    RM_NeckBack_Lower = 105
    LeftShoulder = 106
    LeftArm = 107
    LeftForeArm = 108
    RM_LeftForeArmTwist1 = 109
    RM_LeftForeArmTwist1_Root = 110
    RM_LeftRadialis_Lower_Offset = 111
    RM_LeftRadialis_Lower = 112
    RM_LeftForeArmTwist1_Lower_Offset = 113
    RM_LeftForeArmTwist1_Lower = 114
    RM_LeftForeArmTwist2 = 115
    RM_LeftForeArmTwist2_Root = 116
    RM_LeftForeArmTwist3 = 117
    RM_LeftForeArmTwist3_Root = 118
    RM_LeftForeArmTwist4 = 119
    RM_LeftWrist_Upper = 120
    RM_LeftWrist_Lower = 121
    LeftHand = 122
    LeftHandThumb1 = 123
    LeftHandThumb2 = 124
    LeftHandThumb3 = 125
    LeftHandThumbEnd = 126
    LeftInHandIndex = 127
    LeftHandIndex1 = 128
    LeftHandIndex2 = 129
    LeftHandIndex3 = 130
    LeftHandIndexEnd = 131
    LeftInHandMiddle = 132
    LeftHandMiddle1 = 133
    LeftHandMiddle2 = 134
    LeftHandMiddle3 = 135
    LeftHandMiddleEnd = 136
    LeftInHandRing = 137
    LeftHandRing1 = 138
    LeftHandRing2 = 139
    LeftHandRing3 = 140
    LeftHandRingEnd = 141
    LeftInHandPinky = 142
    LeftHandPinky1 = 143
    LeftHandPinky2 = 144
    LeftHandPinky3 = 145
    LeftHandPinkyEnd = 146
    Prop_LeftProp = 147
    RM_LeftArmTwist1 = 148
    RM_LeftArmTwist1_Root = 149
    RM_LeftBackArmCorrect_Offset = 150
    RM_LeftBackArmCorrect = 151
    RM_LeftBackArmCorrect_End = 152
    RM_LeftArmTwist1_Sleeve = 153
    RM_LeftPecReverse_Offset = 154
    RM_LeftPecReverse = 155
    RM_LeftPecReverse_End = 156
    RM_LeftArmTwist2 = 157
    RM_LeftArmTwist2_Root = 158
    RM_LeftBicep_Upper_Offset = 159
    RM_LeftBicep_Upper = 160
    RM_LeftTricep_Offset = 161
    RM_LeftTricep = 162
    RM_LeftArmTwist2_Sleeve = 163
    RM_LeftBicep_Upper_Sleeve_Offset = 164
    RM_LeftBicep_Upper_Sleeve = 165
    RM_LeftTricep_Sleeve_Offset = 166
    RM_LeftTricep_Sleeve = 167
    RM_LeftArmTwist3 = 168
    RM_LeftArmTwist3_Root = 169
    RM_LeftBicep_Lower_Offset = 170
    RM_LeftBicep_Lower = 171
    RM_LeftRadialis_Upper_Offset = 172
    RM_LeftRadialis_Upper = 173
    RM_LeftArmTwist3_Sleeve = 174
    RM_LeftBicep_Lower_Sleeve_Offset = 175
    RM_LeftBicep_Lower_Sleeve = 176
    RM_LeftRadialis_Upper_Sleeve_Offset = 177
    RM_LeftRadialis_Upper_Sleeve = 178
    RM_LeftElbow = 179
    RM_LeftDelt_All = 180
    RM_LeftDelt_Offset = 181
    RM_LeftDelt = 182
    RM_LeftDelt_End = 183
    RM_LeftScap_Offset = 184
    RM_LeftScap = 185
    RM_LeftScap_End = 186
    RM_LeftArmPit = 187
    RightShoulder = 188
    RightArm = 189
    RightForeArm = 190
    RM_RightForeArmTwist1 = 191
    RM_RightForeArmTwist1_Root = 192
    RM_RightRadialis_Lower_Offset = 193
    RM_RightRadialis_Lower = 194
    RM_RightForeArmTwist1_Lower_Offset = 195
    RM_RightForeArmTwist1_Lower = 196
    RM_RightForeArmTwist2 = 197
    RM_RightForeArmTwist2_Root = 198
    RM_RightForeArmTwist3 = 199
    RM_RightForeArmTwist3_Root = 200
    RM_RightForeArmTwist4 = 201
    RM_RightWrist_Upper = 202
    RM_RightWrist_Lower = 203
    RightHand = 204
    RightHandThumb1 = 205
    RightHandThumb2 = 206
    RightHandThumb3 = 207
    RightHandThumbEnd = 208
    RightInHandIndex = 209
    RightHandIndex1 = 210
    RightHandIndex2 = 211
    RightHandIndex3 = 212
    RightHandIndexEnd = 213
    RightInHandMiddle = 214
    RightHandMiddle1 = 215
    RightHandMiddle2 = 216
    RightHandMiddle3 = 217
    RightHandMiddleEnd = 218
    RightInHandRing = 219
    RightHandRing1 = 220
    RightHandRing2 = 221
    RightHandRing3 = 222
    RightHandRingEnd = 223
    RightInHandPinky = 224
    RightHandPinky1 = 225
    RightHandPinky2 = 226
    RightHandPinky3 = 227
    RightHandPinkyEnd = 228
    Prop_RightProp = 229
    RM_RightArmTwist1 = 230
    RM_RightArmTwist1_Root = 231
    RM_RightBackArmCorrect_Offset = 232
    RM_RightBackArmCorrect = 233
    RM_RightBackArmCorrect_End = 234
    RM_RightArmTwist1_Sleeve = 235
    RM_RightPecReverse_Offset = 236
    RM_RightPecReverse = 237
    RM_RightPecReverse_End = 238
    RM_RightArmTwist2 = 239
    RM_RightArmTwist2_Root = 240
    RM_RightBicep_Upper_Offset = 241
    RM_RightBicep_Upper = 242
    RM_RightTricep_Offset = 243
    RM_RightTricep = 244
    RM_RightArmTwist2_Sleeve = 245
    RM_RightBicep_Upper_Sleeve_Offset = 246
    RM_RightBicep_Upper_Sleeve = 247
    RM_RightTricep_Sleeve_Offset = 248
    RM_RightTricep_Sleeve = 249
    RM_RightArmTwist3 = 250
    RM_RightArmTwist3_Root = 251
    RM_RightBicep_Lower_Offset = 252
    RM_RightBicep_Lower = 253
    RM_RightRadialis_Upper_Offset = 254
    RM_RightRadialis_Upper = 255
    RM_RightArmTwist3_Sleeve = 256
    RM_RightBicep_Lower_Sleeve_Offset = 257
    RM_RightBicep_Lower_Sleeve = 258
    RM_RightRadialis_Upper_Sleeve_Offset = 259
    RM_RightRadialis_Upper_Sleeve = 260
    RM_RightElbow = 261
    RM_RightDelt_All = 262
    RM_RightDelt_Offset = 263
    RM_RightDelt = 264
    RM_RightDelt_End = 265
    RM_RightScap_Offset = 266
    RM_RightScap = 267
    RM_RightScap_End = 268
    RM_RightArmPit = 269
    RM_Spine3_Jiggle = 270
    RM_LeftLat_Upper_Offset = 271
    RM_LeftLat_Upper = 272
    RM_LeftLat_Upper_End = 273
    RM_RightLat_Upper_Offset = 274
    RM_RightLat_Upper = 275
    RM_RightLat_Upper_End = 276
    RM_MiddleBreather = 277
    RM_LeftBreather = 278
    RM_LeftPecAll = 279
    RM_LeftUpperPec_Offset = 280
    RM_LeftUpperPec = 281
    RM_LeftUpperPec_End = 282
    RM_LeftLowerPec_Offset = 283
    RM_LeftLowerPec = 284
    RM_LeftLowerPec_End = 285
    RM_RightBreather = 286
    RM_RightPecAll = 287
    RM_RightLowerPec_Offset = 288
    RM_RightLowerPec = 289
    RM_RightLowerPec_End = 290
    RM_RightUpperPec_Offset = 291
    RM_RightUpperPec = 292
    RM_RightUpperPec_End = 293
    RM_Spine3_Back_Helper = 294
    AJ_LeftCollar = 295
    AJ_LeftCollar_End = 296
    AJ_RightCollar = 297
    AJ_RightCollar_End = 298
    RM_Spine2_Jiggle = 299
    RM_LeftLat_Middle_Offset = 300
    RM_LeftLat_Middle = 301
    RM_LeftLat_Middle_End = 302
    RM_RightLat_Middle_Offset = 303
    RM_RightLat_Middle = 304
    RM_RightLat_Middle_End = 305
    RM_Spine1_Jiggle = 306
    RM_LeftLat_Lower_Offset = 307
    RM_LeftLat_Lower = 308
    RM_LeftLat_Lower_End = 309
    RM_RightLat_Lower_Offset = 310
    RM_RightLat_Lower = 311
    RM_RightLat_Lower_End = 312
    RM_LeftRibCage_Offset = 313
    RM_LeftRibCage = 314
    RM_RightRibCage_Offset = 315
    RM_RightRibCage = 316
    RM_BellyBreather2_Offset = 317
    RM_BellyBreather2 = 318
    RM_ShirtHigh_Back = 319
    RM_LeftShirtHigh_Back = 320
    RM_LeftShirtHigh_Side = 321
    RM_LeftShirtHigh_Front = 322
    RM_RightShirtHigh_Back = 323
    RM_RightShirtHigh_Side = 324
    RM_RightShirtHigh_Front = 325
    RM_Spine_Jiggle = 326
    RM_BellyBreather1_Offset = 327
    RM_BellyBreather1 = 328
    AJ_SideBag = 329
    LeftUpLeg = 330
    LeftLeg = 331
    LeftFoot = 332
    LeftToeBase = 333
    LeftFootEnd = 334
    RM_LeftLegTwist1 = 335
    RM_LeftLegTwist1_Root = 336
    RM_LeftKnee_Lower = 337
    RM_LeftLegInnerTendon_Lower = 338
    RM_LeftLegOuterTendon_Lower = 339
    RM_LeftLegTwist2 = 340
    RM_LeftLegTwist2_Root = 341
    RM_LeftInnerCalf_Offset = 342
    RM_LeftInnerCalf = 343
    RM_LeftOuterCalf_Offset = 344
    RM_LeftOuterCalf = 345
    RM_LeftLegTwist3 = 346
    RM_LeftLegTwist3_Root = 347
    RM_LeftAnkleTwist = 348
    RM_LeftAnkle_Front = 349
    RM_LeftAnkle_Back = 350
    RM_LeftUpLegTwist1 = 351
    RM_LeftUpLegTwist2 = 352
    RM_LeftAdductor_Offset = 353
    RM_LeftAdductor = 354
    RM_LeftSuit_Front = 355
    RM_LeftUpLegTwist3 = 356
    RM_LeftKnee_Upper = 357
    RM_LeftLegInnerTendon_Upper_Offset = 358
    RM_LeftLegInnerTendon_Upper = 359
    RM_LeftLegOuterTendon_Upper_Offset = 360
    RM_LeftLegOuterTendon_Upper = 361
    RM_LeftCondyle_Offset = 362
    RM_LeftCondyle = 363
    RM_LeftUpLegTwist1_Shorts = 364
    RM_LeftUpLegTwist2_Shorts = 365
    RM_LeftInsideShorts_Upper_Offset = 366
    RM_LeftInsideShorts_Upper = 367
    RM_LeftUpLegTwist3_Shorts = 368
    RM_LeftInsideShorts_Lower_Offset = 369
    RM_LeftInsideShorts_Lower = 370
    RightUpLeg = 371
    RightLeg = 372
    RightFoot = 373
    RightToeBase = 374
    RightFootEnd = 375
    RM_RightLegTwist1 = 376
    RM_RightLegTwist1_Root = 377
    RM_RightKnee_Lower = 378
    RM_RightLegInnerTendon_Lower = 379
    RM_RightLegOuterTendon_Lower = 380
    RM_RightLegTwist2 = 381
    RM_RightLegTwist2_Root = 382
    RM_RightInnerCalf_Offset = 383
    RM_RightInnerCalf = 384
    RM_RightOuterCalf_Offset = 385
    RM_RightOuterCalf = 386
    RM_RightLegTwist3 = 387
    RM_RightLegTwist3_Root = 388
    RM_RightAnkleTwist = 389
    RM_RightAnkle_Front = 390
    RM_RightAnkle_Back = 391
    RM_RightUpLegTwist1 = 392
    RM_RightUpLegTwist2 = 393
    RM_RightAdductor_Offset = 394
    RM_RightAdductor = 395
    RM_RightSuit_Front = 396
    RM_RightUpLegTwist3 = 397
    RM_RightKnee_Upper = 398
    RM_RightLegInnerTendon_Upper_Offset = 399
    RM_RightLegInnerTendon_Upper = 400
    RM_RightLegOuterTendon_Upper_Offset = 401
    RM_RightLegOuterTendon_Upper = 402
    RM_RightCondyle_Offset = 403
    RM_RightCondyle = 404
    RM_RightUpLegTwist1_Shorts = 405
    RM_RightUpLegTwist2_Shorts = 406
    RM_RightInsideShorts_Upper_Offset = 407
    RM_RightInsideShorts_Upper = 408
    RM_RightUpLegTwist3_Shorts = 409
    RM_RightInsideShorts_Lower_Offset = 410
    RM_RightInsideShorts_Lower = 411
    RM_LeftButt_Offset = 412
    RM_LeftButt = 413
    RM_RightButt_Offset = 414
    RM_RightButt = 415
    RM_LeftShirtLow_Front = 416
    RM_RightShirtLow_Front = 417
    RM_LeftShirtLow_Side = 418
    RM_RightShirtLow_Side = 419
    RM_LeftShirtLow_Back = 420
    RM_RightShirtLow_Back = 421
    RM_LeftShorts = 422
    RM_RightShorts = 423
    RM_Shorts_Crotch = 424
    RM_Shorts_Back = 425
    RM_Shorts_Crack = 426
    RM_LeftShirtMid_Front = 427
    RM_LeftShirtMid_Side = 428
    RM_LeftShirtMid_Back = 429
    RM_ShirtMid_Back = 430
    RM_RightShirtMid_Front = 431
    RM_RightShirtMid_Side = 432
    RM_RightShirtMid_Back = 433
    AJ_HipBag = 434
    Trajectory = 435
    TrajectoryEnd = 436
    Prop = 437
End Enum

Public Module TextureFormatHelpers
    <System.Runtime.CompilerServices.Extension>
    Public Function IsCompressedFormat(ByVal Format As ETextureFormat) As Boolean
        Select Case Format
            Case ETextureFormat.BC1, ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC4, ETextureFormat.BC5, ETextureFormat.BC6H_UF16, ETextureFormat.BC7
                Return True
            Case Else
                Return False
        End Select
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function SupportsAlpha(ByVal Format As ETextureFormat) As Boolean
        Select Case Format
            Case ETextureFormat.B8G8R8A8, ETextureFormat.L8A8, ETextureFormat.B4G4R4A4, ETextureFormat.B5G5R5A1, ETextureFormat.R8G8B8A8, ETextureFormat.R32G32B32A32Float, ETextureFormat.BC1, ETextureFormat.BC2, ETextureFormat.BC3, ETextureFormat.BC7
                Return True
            Case Else
                Return False
        End Select
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function ToRx3TextureFormat(ByVal Format As ETextureFormat) As Rx3.TextureFormat
        Select Case Format
            ' rx3 + RW4
            Case ETextureFormat.BC1
                Return Rx3.TextureFormat.DXT1
            Case ETextureFormat.BC2
                Return Rx3.TextureFormat.DXT3
            Case ETextureFormat.BC3
                Return Rx3.TextureFormat.DXT5
            Case ETextureFormat.B8G8R8A8
                Return Rx3.TextureFormat.B8G8R8A8
            Case ETextureFormat.L8
                Return Rx3.TextureFormat.L8
            Case ETextureFormat.L8A8
                Return Rx3.TextureFormat.L8A8
            Case ETextureFormat.BC5
                Return Rx3.TextureFormat.ATI2
            Case ETextureFormat.B4G4R4A4
                Return Rx3.TextureFormat.B4G4R4A4
            Case ETextureFormat.B5G6R5
                Return Rx3.TextureFormat.B5G6R5
            Case ETextureFormat.B5G5R5A1
                Return Rx3.TextureFormat.B5G5R5A1

            'RX3 only
            Case ETextureFormat.R8G8B8A8
                Return Rx3.TextureFormat.R8G8B8A8
            Case ETextureFormat.BC4
                Return Rx3.TextureFormat.ATI1
            Case ETextureFormat.BIT8
                Return Rx3.TextureFormat.BIT8
            Case ETextureFormat.B8G8R8
                Return Rx3.TextureFormat.B8G8R8
            Case ETextureFormat.BC6H_UF16
                Return Rx3.TextureFormat.BC6H_UF16

                'RW4 only
                'Case ETextureFormat.DXT1NORMAL
                'Case ETextureFormat.A32B32G32R32F

            Case Else
                MsgBox("Unknown ETextureFormat found at function ""GetRx3FromETextureFormat"": " & Format.ToString)
                Return Rx3.TextureFormat.DXT1
        End Select
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function ToRwSurfaceFormat(ByVal Format As ETextureFormat) As Rw.SurfaceFormat ', ByVal DefaultRWFormat As RWTextureFormat) 

        Select Case Format
            ' rx3 + RW4
            Case ETextureFormat.BC1
                Return Rw.SurfaceFormat.FMT_DXT1
            Case ETextureFormat.BC2
                Return Rw.SurfaceFormat.FMT_DXT2_3
            Case ETextureFormat.BC3
                Return Rw.SurfaceFormat.FMT_DXT4_5
            Case ETextureFormat.B8G8R8A8
                Return Rw.SurfaceFormat.FMT_8_8_8_8
            Case ETextureFormat.L8
                Return Rw.SurfaceFormat.FMT_8
            Case ETextureFormat.L8A8
                Return Rw.SurfaceFormat.FMT_8_8
            Case ETextureFormat.BC5
                Return Rw.SurfaceFormat.FMT_DXN
            Case ETextureFormat.B4G4R4A4
                Return Rw.SurfaceFormat.FMT_4_4_4_4
            Case ETextureFormat.B5G6R5
                Return Rw.SurfaceFormat.FMT_5_6_5
            Case ETextureFormat.B5G5R5A1
                Return Rw.SurfaceFormat.FMT_1_5_5_5

            'RX3 only
            'Case ETextureFormat.RGBA
            'Case ETextureFormat.ATI1
            'Case ETextureFormat.BIT8
            'Case ETextureFormat.R8G8B8
            'Case ETextureFormat.BC6H_UF16

            'RW4 only
            Case ETextureFormat.CTX1
                Return Rw.SurfaceFormat.FMT_CTX1
            Case ETextureFormat.R32G32B32A32Float
                Return Rw.SurfaceFormat.FMT_32_32_32_32_FLOAT

            Case Else
                MsgBox("Unknown ETextureFormat found at function ""GetRWFromETextureFormat"": " & Format.ToString)
                Return Rw.SurfaceFormat.FMT_DXT1
        End Select

    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function ToDxgiFormat(ByVal Format As ETextureFormat) As DXGI_FORMAT
        Select Case Format
            Case ETextureFormat.BC1
                Return DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM
            Case ETextureFormat.BC2
                Return DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM
            Case ETextureFormat.BC3
                Return DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM
            Case ETextureFormat.BC4
                Return DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM
            Case ETextureFormat.BC5
                Return DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM
            Case ETextureFormat.BC6H_UF16
                Return DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16
            Case ETextureFormat.BC7
                Return DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM

            Case ETextureFormat.B8G8R8A8
                Return DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM
            Case ETextureFormat.B8G8R8
                Return DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM
            Case ETextureFormat.R8G8B8A8
                Return DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM

            Case ETextureFormat.L8
                Return DXGI_FORMAT.DXGI_FORMAT_R8_UNORM
            Case ETextureFormat.L8A8
                Return DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM

            Case ETextureFormat.B4G4R4A4
                Return DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM

            Case ETextureFormat.B5G6R5
                Return DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM
            Case ETextureFormat.B5G5R5A1
                Return DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM

            Case ETextureFormat.R32G32B32A32Float
                Return DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT

                'Case ETextureFormat.BIT8
                '    Return DXGI_FORMAT
                'Case ETextureFormat.CTX1
                '    Return DXGI_FORMAT
        End Select

        Return DXGI_FORMAT.DXGI_FORMAT_UNKNOWN
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function ToETextureFormat(ByVal DxgiFormat As BCnEncoder.Shared.DXGI_FORMAT) As ETextureFormat
        Select Case DxgiFormat
            Case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT
                Return ETextureFormat.R32G32B32A32Float
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G32_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32G8X24_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_X32_TYPELESS_G8X24_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R11G11B10_FLOAT
                    '    Exit Select
            Case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB
                Return ETextureFormat.R8G8B8A8
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16_FLOAT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16_UNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16_SNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16G16_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R32_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R24G8_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R24_UNORM_X8_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_X24_TYPELESS_G8_UINT
                    '    Exit Select
            Case DXGI_FORMAT.DXGI_FORMAT_R8G8_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM
                Return ETextureFormat.L8A8
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8G8_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8G8_SNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8G8_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16_TYPELESS
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16_FLOAT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_D16_UNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16_UNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16_SNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R16_SINT
                    '    Exit Select
            Case DXGI_FORMAT.DXGI_FORMAT_R8_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_R8_UNORM
                Return ETextureFormat.L8
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8_UINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8_SNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8_SINT
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_A8_UNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R1_UNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R9G9B9E5_SHAREDEXP
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_R8G8_B8G8_UNORM
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_G8R8_G8B8_UNORM
                    '    Exit Select
            Case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB
                Return ETextureFormat.BC1
            Case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB
                Return ETextureFormat.BC2
            Case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB
                Return ETextureFormat.BC3
            Case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM
                Return ETextureFormat.BC4
            Case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM
                Return ETextureFormat.BC5
            Case DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM
                Return ETextureFormat.B5G6R5
            Case DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM
                Return ETextureFormat.B5G5R5A1
            Case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB
                Return ETextureFormat.B8G8R8A8
            Case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM, DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB
                Return ETextureFormat.B8G8R8
                    'Case DXGI_FORMAT.DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM
                    '    Exit Select

            Case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16
                Return ETextureFormat.BC6H_UF16
                    'Case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16
                    'Exit Select
            Case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS, DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM, DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB
                Return ETextureFormat.BC7
                    'Case DXGI_FORMAT.DXGI_FORMAT_AYUV
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_Y410
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_Y416
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_NV12
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_P010
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_P016
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_420_OPAQUE
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_YUY2
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_Y210
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_Y216
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_NV11
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_AI44
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_IA44
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_P8
                    '    Exit Select
                    'Case DXGI_FORMAT.DXGI_FORMAT_A8P8
                    '    Exit Select
            Case DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM
                Return ETextureFormat.B4G4R4A4
                'Case DXGI_FORMAT.DXGI_FORMAT_P208
                '    Exit Select
                'Case DXGI_FORMAT.DXGI_FORMAT_V208
                '    Exit Select
                'Case DXGI_FORMAT.DXGI_FORMAT_V408
                '    Exit Select
                'Case DXGI_FORMAT.DXGI_FORMAT_FORCE_UINT
                '    Exit Select
        End Select

        Return ETextureFormat.UNKNOWN
    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function ToGlFormat(ByVal TextureFormat As ETextureFormat) As (GLFormat, GlInternalFormat, GLType)
        Select Case TextureFormat
            Case ETextureFormat.BC1
                Return (GLFormat.GL_RGB, GlInternalFormat.GL_COMPRESSED_RGB_S3TC_DXT1_EXT, GLType.NONE)   'BC1
                    'Return (GLFormat.GlRgba, GlInternalFormat.GL_CompressedRgbaS3TcDxt1Ext, GLType.None) 'BC1WithAlpha
            Case ETextureFormat.BC2
                Return (GLFormat.GL_RGBA, GlInternalFormat.GL_COMPRESSED_RGBA_S3TC_DXT3_EXT, GLType.NONE)
            Case ETextureFormat.BC3
                Return (GLFormat.GL_RGBA, GlInternalFormat.GL_COMPRESSED_RGBA_S3TC_DXT5_EXT, GLType.NONE)
            Case ETextureFormat.BC4
                Return (GLFormat.GL_RED, GlInternalFormat.GL_COMPRESSED_RED_RGTC1_EXT, GLType.NONE)
            Case ETextureFormat.BC5
                Return (GLFormat.GL_RG, GlInternalFormat.GL_COMPRESSED_RED_GREEN_RGTC2_EXT, GLType.NONE)
            Case ETextureFormat.BC6H_UF16
                Return (GLFormat.GL_RGB, GlInternalFormat.GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB, GLType.NONE)
                'Case ETextureFormat.Bc6S
                '    Return (GLFormat.GL_RGB, GlInternalFormat.GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB, GLType.NONE)
            Case ETextureFormat.BC7
                Return (GLFormat.GL_RGBA, GlInternalFormat.GL_COMPRESSED_RGBA_BPTC_UNORM_ARB, GLType.NONE)


            Case ETextureFormat.B8G8R8A8
                Return (GLFormat.GL_BGRA, GlInternalFormat.GL_BGRA8_EXTENSION, GLType.GL_UNSIGNED_BYTE)

                'Case ETextureFormat.B8G8R8
                    'Return (GLFormat.GL_RGB, GlInternalFormat.GL_bgr8, GLType.GL_UNSIGNED_BYTE)

            Case ETextureFormat.R8G8B8A8
                Return (GLFormat.GL_RGBA, GlInternalFormat.GL_RGBA8, GLType.GL_UNSIGNED_BYTE)


            Case ETextureFormat.L8
                Return (GLFormat.GL_RED, GlInternalFormat.GL_R8, GLType.GL_UNSIGNED_BYTE)
            Case ETextureFormat.L8A8
                Return (GLFormat.GL_RG, GlInternalFormat.GL_RG8, GLType.GL_UNSIGNED_BYTE)

                'Case ETextureFormat.B4G4R4A4
                '    Return (GLFormat.GL_BGRA, GlInternalFormat.GL_BGRA4, GLType.GL_UNSIGNED_BYTE)


                'Case ETextureFormat.B5G6R5
                '    Return (GLFormat.GL_BGR, GlInternalFormat.GL_BGR565, GLType.GL_UNSIGNED_SHORT_5_6_5)

                'Case ETextureFormat.B5G5R5A1
                '    Return (GLFormat.GL_BGRA, GlInternalFormat.GL_BGR5_A1, GLType.GL_UNSIGNED_SHORT_5_5_5_1)

            Case ETextureFormat.R32G32B32A32Float
                Return (GLFormat.GL_RGBA, GlInternalFormat.GL_RGBA32F, GLType.GL_FLOAT)


            Case Else
                Return (0, 0, 0)
        End Select

    End Function

    <System.Runtime.CompilerServices.Extension>
    Public Function ToETextureFormat(ByVal InternalFormat As GlInternalFormat) As ETextureFormat
        Select Case InternalFormat
            Case GlInternalFormat.GL_COMPRESSED_RGB_S3TC_DXT1_EXT, GlInternalFormat.GL_COMPRESSED_SRGB_S3TC_DXT1_EXT
                Return ETextureFormat.BC1

            Case GlInternalFormat.GL_COMPRESSED_RGBA_S3TC_DXT1_EXT, GlInternalFormat.GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT
                Return ETextureFormat.BC1   'ETextureFormat.BC1WithAlpha

            Case GlInternalFormat.GL_COMPRESSED_RGBA_S3TC_DXT3_EXT, GlInternalFormat.GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT
                Return ETextureFormat.BC2

            Case GlInternalFormat.GL_COMPRESSED_RGBA_S3TC_DXT5_EXT, GlInternalFormat.GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT
                Return ETextureFormat.BC3

            Case GlInternalFormat.GL_COMPRESSED_RED_RGTC1_EXT, GlInternalFormat.GL_COMPRESSED_SIGNED_RED_RGTC1_EXT
                Return ETextureFormat.BC4

            Case GlInternalFormat.GL_COMPRESSED_RED_GREEN_RGTC2_EXT, GlInternalFormat.GL_COMPRESSED_SIGNED_RED_GREEN_RGTC2_EXT
                Return ETextureFormat.BC5

            Case GlInternalFormat.GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT_ARB
                Return ETextureFormat.BC6H_UF16

            'Case GlInternalFormat.GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT_ARB
            '    Return ETextureFormat.Bc6S

                ' TODO: Not sure what to do with SRGB input.
            Case GlInternalFormat.GL_COMPRESSED_RGBA_BPTC_UNORM_ARB, GlInternalFormat.GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM_ARB
                Return ETextureFormat.BC7

                ' HINT: Bgra is not supported by default. The format enum is added by an extension by Apple.
            Case GlInternalFormat.GL_BGRA8_EXTENSION
                Return ETextureFormat.B8G8R8A8

            Case GlInternalFormat.GL_RGBA8, GlInternalFormat.GL_RGBA8I, GlInternalFormat.GL_RGBA8UI, GlInternalFormat.GL_RGBA8_SNORM
                Return ETextureFormat.R8G8B8A8

            Case GlInternalFormat.GL_R8, GlInternalFormat.GL_R8I, GlInternalFormat.GL_R8UI, GlInternalFormat.GL_R8_SNORM
                Return ETextureFormat.L8

            Case GlInternalFormat.GL_RG8, GlInternalFormat.GL_RG8I, GlInternalFormat.GL_RG8UI, GlInternalFormat.GL_RG8_SNORM
                Return ETextureFormat.L8A8

            Case GlInternalFormat.GL_RGBA32F
                Return ETextureFormat.R32G32B32A32Float

            Case Else
                Return ETextureFormat.UNKNOWN
        End Select
    End Function

End Module
