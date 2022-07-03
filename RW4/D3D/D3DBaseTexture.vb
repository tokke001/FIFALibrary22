Imports FIFALibrary22.FifaUtil
Namespace Rw.D3D
    Public Class D3DBaseTexture
        '0x1469 D3DBaseTexture
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            '--0x13CA D3DResource  m_d3d --> similar to D3dResource (vertex/index buffer), but ReferenceCount is in different endian ?
            Me.D3dResource = New D3DResource(r)
            'Me.Common = r.ReadUInt32    '0 or 64     sectionsize? , '3       textureflags ?
            'Me.ReferenceCount = r.ReadUInt32     'x01 00 00 00 
            'Me.Fence = r.ReadUInt32         '0
            'Me.ReadFence = r.ReadUInt32    '0
            'Me.Identifier = r.ReadUInt32    '0
            'Me.BaseFlush = r.ReadUInt32     'FF FF 00 00 

            Me.MipFlush = r.ReadUInt32 'FF FF 00 00  

            Me.Format = New GPUTEXTURE_FETCH_CONSTANT(r)

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            Me.D3dResource.Save(w)
            'w.Write(Me.D3dResource)
            'w.Write(Me.ReferenceCount)
            'w.Write(Me.Fence)
            'w.Write(Me.ReadFence)
            'w.Write(Me.Identifier)
            'w.Write(Me.BaseFlush)

            w.Write(Me.MipFlush)

            Me.Format.Save(w)

        End Sub


        Public Property D3dResource As D3DResource
        Public Property MipFlush As UInteger
        Public Property Format As GPUTEXTURE_FETCH_CONSTANT

    End Class

    Public Class GPUTEXTURE_FETCH_CONSTANT
        'GPUTEXTURE_FETCH_CONSTANT  0x1c4c
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)


            Me.Value_0 = r.ReadUInt32
            Me.Value_4 = r.ReadUInt32
            Me.Value_8 = r.ReadUInt32
            Me.Value_12 = r.ReadUInt32
            Me.Value_16 = r.ReadUInt32
            Me.Value_20 = r.ReadUInt32
            'DataFormat
            'Me.TextureFormat = GetValueFromInteger(Value_4, 0, 6) 'Value_4 And (65536 - 1) 'Decimal.GetBits(Value_4)(0)
            'Me.Endian = GetValueFromInteger(Value_4, 6, 2)

            'Dim OneD As Integer = GetValueFromInteger(Value_8, 0, 24) 'Width= -> bits = 24, starting position = 0
            'Dim TwoD As Integer() = New Integer(2 - 1) {} '= 0x00001ad6
            'TwoD(0) = GetValueFromInteger(Value_8, 0, 13)
            'TwoD(1) = GetValueFromInteger(Value_8, 13, 13)
            'Dim ThreeD As Integer() = New Integer(3 - 1) {} '= 0x00002fe5
            'ThreeD(0) = GetValueFromInteger(Value_8, 0, 11)     'width
            'ThreeD(1) = GetValueFromInteger(Value_8, 11, 11)    'height
            'ThreeD(2) = GetValueFromInteger(Value_8, 22, 10)    'depth
            'Dim Stack As Integer() = New Integer(3 - 1) {}  '= 0x00001d92
            'Me.Width = GetValueFromInteger(Value_8, 0, 13) + 1     'width  0x1AD3
            'Me.Height = GetValueFromInteger(Value_8, 13, 13) + 1    'height 0x1AD4
            'Me.Depth = GetValueFromInteger(Value_8, 26, 6) + 1    'depth  0x136D

            '-------TEST ALL---------
            'Dim m_TextureFormat As Rw.SurfaceFormat = Me.TextureFormat
            'Dim m_Width As Integer = Me.Width
            'Dim m_Height As Integer = Me.Height
            'Dim m_Depth As Integer = Me.Depth
            'Dim m_Pitch As Integer = Me.Pitch
            'Dim m_Pitch_calculated As Integer = GraphicUtil.GetTexturePitch(m_Width, GraphicUtil.GetEFromRWTextureFormat(Me.TextureFormat))
            'Dim m_RequestSize As Integer = Me.RequestSize
            'Dim m_Endian As Integer = Me.Endian
            'Dim m_Tiled As Integer = Me.Tiled
            'Dim m_Dimension As RWDimension = Me.Dimension

            'Dim m_AnisoBias As Integer = Me.AnisoBias
            'Dim m_AnisoFilter As Integer = Me.AnisoFilter
            'Dim m_BaseAddress As Integer = Me.PtrTextureBuffer
            'Dim m_BorderColor As Integer = Me.BorderColor
            'Dim m_BorderSize As Integer = Me.BorderSize
            'Dim m_ClampPolicy As Integer = Me.ClampPolicy
            'Dim m_ClampX As Integer = Me.ClampX
            'Dim m_ClampY As Integer = Me.ClampY
            'Dim m_ClampZ As Integer = Me.ClampZ
            'Dim m_ExpAdjust As Integer = Me.ExpAdjust
            'Dim m_ForceBCWToMax As Integer = Me.ForceBCWToMax
            'Dim m_GradExpAdjustH As Integer = Me.GradExpAdjustH
            'Dim m_GradExpAdjustV As Integer = Me.GradExpAdjustV
            'Dim m_LODBias As Integer = Me.LODBias
            'Dim m_MagAnisoWalk As Integer = Me.MagAnisoWalk
            'Dim m_MagFilter As Integer = Me.MagFilter
            'Dim m_MaxMipLevel As Integer = Me.MaxMipLevel
            'Dim m_MinAnisoWalk As Integer = Me.MinAnisoWalk
            'Dim m_MinFilter As Integer = Me.MinFilter
            'Dim m_MinMipLevel As Integer = Me.MinMipLevel
            'Dim m_MipAddress As Integer = Me.MipAddress
            'Dim m_MipFilter As Integer = Me.MipFilter
            'Dim m_NumFormat As Integer = Me.NumFormat
            'Dim m_PackedMips As Integer = Me.PackedMips
            'Dim m_SignW As Integer = Me.SignW
            'Dim m_SignX As Integer = Me.SignX
            'Dim m_SignY As Integer = Me.SignY
            'Dim m_SignZ As Integer = Me.SignZ
            'Dim m_Stacked As Integer = Me.Stacked
            'Dim m_SwizzleW As Integer = Me.SwizzleW
            'Dim m_SwizzleX As Integer = Me.SwizzleX
            'Dim m_SwizzleY As Integer = Me.SwizzleY
            'Dim m_SwizzleZ As Integer = Me.SwizzleZ
            'Dim m_TriClamp As Integer = Me.TriClamp
            'Dim m_VolMagFilter As Integer = Me.VolMagFilter
            'Dim m_VolMinFilter As Integer = Me.VolMinFilter
            'Dim m_m_Type As Integer = Me.m_Type
            '-------------------


            'Me.Unknown_16 = r.ReadByte
            'Dim TmpValue_height As Byte = r.ReadByte
            'Dim TmpValue_weight As Short = r.ReadInt16
            'If TmpValue_height = 0 AndAlso TmpValue_weight = 24579 Then '4x4 size
            'Me.Height = 4
            'Else
            'Me.Height = (TmpValue_height + 1) * 8
            'End If
            'Me.Width = ((TmpValue_weight + 1) And 8191)


        End Sub
        Public Sub Save(ByVal w As FileWriter)
            'Me.TextureFormat =   'Set in RX3 section
            'Me.NumLevels =     'Set in RX3 section

            'Me.D3dResource.Save(w)
            'w.Write(Me.D3dResource)
            'w.Write(Me.ReferenceCount)
            'w.Write(Me.Fence)
            'w.Write(Me.ReadFence)
            'w.Write(Me.Identifier)
            'w.Write(Me.BaseFlush)


            'w.Write(Me.MipFlush)

            w.Write(Me.Value_0)
            w.Write(Me.Value_4)
            w.Write(Me.Value_8)
            w.Write(Me.Value_12)
            w.Write(Me.Value_16)
            w.Write(Me.Value_20)

            'w.Write(Me.HiControl)

            'w.Write(Me.Unknown_10)

            'w.Write(Me.Index)

            'w.Write(CByte(Me.TextureFormat))

            'If Me.Unknown_13 = 6 Then
            'w.Write(CByte(20))
            'w.Write(CByte((Me.Height \ 8) - 1))
            'Else
            'w.Write(CByte(Me.Unknown_16))
            'If Me.Height = 4 And Me.Width = 4 Then
            'w.Write(CByte(0))
            'w.Write(CShort(24579))
            'Else
            'w.Write(CByte(Math.Max(0, (Me.Height \ 8) - 1)))
            'w.Write(CShort(-(-(Me.Width - 1) And 8191)))
            'End If

            'w.Write(Me.Format)

            'w.Write(Me.Size)

            'w.Write(Me.Face)
            'w.Write(Me.NumMipLevels)
            'w.Write(Me.Locked)

        End Sub

        Private Value_0 As UInteger = 0
        Private Value_4 As UInteger = 0
        Private Value_8 As UInteger = 0
        Private Value_12 As UInteger = 0
        Private Value_16 As UInteger = 0
        Private Value_20 As UInteger = 0

        Public Property m_Type As UInteger
            Get
                Return GetValueFrom32bit(Value_0, 0, 2)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 0, 2)
            End Set
        End Property
        Public Property SignX As UInteger   '0x19b3
            Get
                Return GetValueFrom32bit(Value_0, 2, 2)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 2, 2)
            End Set
        End Property
        Public Property SignY As UInteger   '0x19b4
            Get
                Return GetValueFrom32bit(Value_0, 4, 2)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 4, 2)
            End Set
        End Property
        Public Property SignZ As UInteger   '0x19b5
            Get
                Return GetValueFrom32bit(Value_0, 6, 2)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 6, 2)
            End Set
        End Property
        Public Property SignW As UInteger   '0x1523
            Get
                Return GetValueFrom32bit(Value_0, 8, 2)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 8, 2)
            End Set
        End Property
        Public Property ClampX As UInteger   '0x1C38
            Get
                Return GetValueFrom32bit(Value_0, 10, 3)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 10, 3)
            End Set
        End Property
        Public Property ClampY As UInteger   '0x1C39
            Get
                Return GetValueFrom32bit(Value_0, 13, 3)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 13, 3)
            End Set
        End Property
        Public Property ClampZ As UInteger   '0x1C3A
            Get
                Return GetValueFrom32bit(Value_0, 16, 3)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 16, 3)
            End Set
        End Property
        Public Property Pitch As UInteger   '0x1c3b
            Get
                'low changing values :
                '4 = 512
                '16 = 1024
                '32 = 2048
                Return GetValueFrom32bit(Value_0, 22, 9)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 22, 9)
            End Set
        End Property

        Public Property Tiled As UInteger   '0x1376 --> SurfaceTiling 0x567e ? 
            Get
                Return GetValueFrom32bit(Value_0, 31, 1)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 31, 1)
            End Set
        End Property

        Public Property TextureFormat As SurfaceFormat  '0x1365 DataFormat
            Get
                Return GetValueFrom32bit(Value_4, 0, 6)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 0, 6)
            End Set
        End Property

        Public Property Endian As UInteger
            Get
                Return GetValueFrom32bit(Value_4, 6, 2)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 6, 2)
            End Set
        End Property
        Public Property RequestSize As UInteger '0x1523
            Get
                Return GetValueFrom32bit(Value_4, 8, 2)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 8, 2)
            End Set
        End Property
        Public Property Stacked As UInteger '0x16F3
            Get
                Return GetValueFrom32bit(Value_4, 10, 1)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 10, 1)
            End Set
        End Property
        Public Property ClampPolicy As UInteger '0x16F4
            Get
                Return GetValueFrom32bit(Value_4, 11, 1)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 11, 1)
            End Set
        End Property
        Public Property BaseAddress As UInteger '0x12C8 --> pointer to index of texture-buffer 
            Get
                Return GetValueFrom32bit(Value_4, 12, 20)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 12, 20)
            End Set
        End Property
        Public Property Width As UInteger
            Get
                Return GetValueFrom32bit(Value_8, 0, 13) + 1     'width  0x1AD3
            End Get
            Set
                Value_8 = SetValueTo32bit(Value_8, Value - 1, 0, 13)
            End Set
        End Property

        Public Property Height As UInteger
            Get
                Return GetValueFrom32bit(Value_8, 13, 13) + 1    'height 0x1AD4
            End Get
            Set
                Value_8 = SetValueTo32bit(Value_8, Value - 1, 13, 13)
            End Set
        End Property

        Public Property Depth As UInteger
            Get
                Return GetValueFrom32bit(Value_8, 26, 6) + 1    'depth  0x136D
            End Get
            Set
                Value_8 = SetValueTo32bit(Value_8, Value - 1, 26, 6)
            End Set
        End Property

        Public Property NumFormat As UInteger    '0x15D2, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 0, 1)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 0, 1)
            End Set
        End Property
        Public Property SwizzleX As GPUSWIZZLE    '0x1C3D, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 1, 3)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 1, 3)
            End Set
        End Property
        Public Property SwizzleY As GPUSWIZZLE    '0x1698, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 4, 3)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 4, 3)
            End Set
        End Property
        Public Property SwizzleZ As GPUSWIZZLE    '0x1C3E, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 7, 3)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 7, 3)
            End Set
        End Property
        Public Property SwizzleW As GPUSWIZZLE    '0x1C38, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 10, 3)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 10, 3)
            End Set
        End Property
        Public Property ExpAdjust As UInteger    '0x1C3F, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 13, 6)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 13, 6)
            End Set
        End Property
        Public Property MagFilter As UInteger    '0x1C40, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 19, 2)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 19, 2)
            End Set
        End Property
        Public Property MinFilter As UInteger    '0x19F3, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 21, 2)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 21, 2)
            End Set
        End Property
        Public Property MipFilter As UInteger    '0x1C41, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 23, 2)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 23, 2)
            End Set
        End Property
        Public Property AnisoFilter As UInteger    '0x1C42, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 25, 3)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 25, 3)
            End Set
        End Property
        Public Property BorderSize As UInteger    '0x1376, offset = 12
            Get
                Return GetValueFrom32bit(Value_12, 31, 1)
            End Get
            Set
                Value_12 = SetValueTo32bit(Value_12, Value, 31, 1)
            End Set
        End Property
        Public Property VolMagFilter As UInteger    '0x15D2, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 0, 1)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 0, 1)
            End Set
        End Property
        Public Property VolMinFilter As UInteger    '0x19D8, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 1, 1)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 1, 1)
            End Set
        End Property
        Public Property MinMipLevel As UInteger    '0x1C43, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 2, 4)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 2, 4)
            End Set
        End Property
        Public Property MaxMipLevel As UInteger    '0x19C4, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 6, 4)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 6, 4)
            End Set
        End Property
        Public Property MagAnisoWalk As UInteger    '0x16F3, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 10, 1)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 10, 1)
            End Set
        End Property
        Public Property MinAnisoWalk As UInteger    '0x16F4, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 11, 1)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 11, 1)
            End Set
        End Property
        Public Property LODBias As UInteger    '0x1C44, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 12, 10)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 12, 10)
            End Set
        End Property
        Public Property GradExpAdjustH As UInteger    '0x1C45, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 22, 5)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 22, 5)
            End Set
        End Property
        Public Property GradExpAdjustV As UInteger    '0x1C46, offset = 16
            Get
                Return GetValueFrom32bit(Value_16, 27, 5)
            End Get
            Set
                Value_16 = SetValueTo32bit(Value_16, Value, 27, 5)
            End Set
        End Property
        Public Property BorderColor As UInteger    '0x12BF, offset = 20
            Get
                Return GetValueFrom32bit(Value_20, 0, 2)
            End Get
            Set
                Value_20 = SetValueTo32bit(Value_20, Value, 0, 2)
            End Set
        End Property
        Public Property ForceBCWToMax As UInteger    '0x12C0, offset = 20
            Get
                Return GetValueFrom32bit(Value_20, 2, 1)
            End Get
            Set
                Value_20 = SetValueTo32bit(Value_20, Value, 2, 1)
            End Set
        End Property
        Public Property TriClamp As UInteger    '0x1C47, offset = 20
            Get
                Return GetValueFrom32bit(Value_20, 3, 2)
            End Get
            Set
                Value_20 = SetValueTo32bit(Value_20, Value, 3, 2)
            End Set
        End Property
        Public Property AnisoBias As UInteger    '0x1C48, offset = 20
            Get
                Return GetValueFrom32bit(Value_20, 5, 4)
            End Get
            Set
                Value_20 = SetValueTo32bit(Value_20, Value, 5, 4)
            End Set
        End Property
        Public Property Dimension As GPUDimension    '0x1C49, offset = 20 -->not sure, but i think enum GPUDIMENSION: 0x109f ??   
            Get
                Return GetValueFrom32bit(Value_20, 9, 2)
            End Get
            Set
                Value_20 = SetValueTo32bit(Value_20, Value, 9, 2)
            End Set
        End Property
        Public Property PackedMips As UInteger    '0x16F4, offset = 20
            Get
                Return GetValueFrom32bit(Value_20, 11, 1)
            End Get
            Set
                Value_20 = SetValueTo32bit(Value_20, Value, 11, 1)
            End Set
        End Property
        Public Property MipAddress As UInteger    '0x12C8, offset = 20
            Get
                Return GetValueFrom32bit(Value_20, 12, 20)
            End Get
            Set
                Value_20 = SetValueTo32bit(Value_20, Value, 12, 20)
            End Set
        End Property
    End Class

    Public Enum D3DSAMPLERSTATETYPE As Byte '_D3DSAMPLERSTATETYPE
        D3DSAMP_ADDRESSU = 0
        D3DSAMP_ADDRESSV = 4
        D3DSAMP_ADDRESSW = 8
        D3DSAMP_BORDERCOLOR = 12
        D3DSAMP_MAGFILTER = 16
        D3DSAMP_MINFILTER = 20
        D3DSAMP_MIPFILTER = 24
        D3DSAMP_MIPMAPLODBIAS = 28
        D3DSAMP_MAXMIPLEVEL = 32
        D3DSAMP_MAXANISOTROPY = 36
        D3DSAMP_MAGFILTERZ = 40
        D3DSAMP_MINFILTERZ = 44
        D3DSAMP_SEPARATEZFILTERENABLE = 48
        D3DSAMP_MINMIPLEVEL = 52
        D3DSAMP_TRILINEARTHRESHOLD = 56
        D3DSAMP_ANISOTROPYBIAS = 60
        D3DSAMP_HGRADIENTEXPBIAS = 64
        D3DSAMP_VGRADIENTEXPBIAS = 68
        D3DSAMP_WHITEBORDERCOLORW = 72
        D3DSAMP_POINTBORDERENABLE = 76
        D3DSAMP_MAX = 80
        'D3DSAMP_FORCE_DWORD = 2147483647
    End Enum
    Public Enum GPUSWIZZLE As Integer   'GPUSWIZZLE
        GPUSWIZZLE_X = 0
        GPUSWIZZLE_Y = 1
        GPUSWIZZLE_Z = 2
        GPUSWIZZLE_W = 3
        GPUSWIZZLE_0 = 4
        GPUSWIZZLE_1 = 5
        GPUSWIZZLE_KEEP = 7
    End Enum
End Namespace