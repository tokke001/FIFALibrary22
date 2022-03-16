Imports FIFALibrary22.FifaUtil
Namespace Rw.D3D
    Public Class D3DVertexBuffer
        'D3DVertexBuffer
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            '--0x13CA D3DResource 
            Me.D3dResource = New D3DResource(r)
            'Me.Common = r.ReadUInt32 '/00/00/00/01   (always same value)
            'Me.ReferenceCount = r.ReadUInt32 '/00/00/00/01   (always same value)
            'Me.Fence = r.ReadUInt32 '/00/00/00/00   (always same value)
            'Me.ReadFence = r.ReadUInt32 '/00/00/00/00   (always same value)
            'Me.Identifier = r.ReadUInt32 '/00/00/00/00   (always same value)
            'Me.BaseFlush = r.ReadUInt32 '/FF/FF/00/00   (always same value)

            '-- Format 0x2050  offset = 24
            Me.Format = New GPUVERTEX_FETCH_CONSTANT(r)

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            Me.D3dResource.Save(w)

            Me.Format.Save(w)

        End Sub


        Public Property D3dResource As D3DResource
        Public Property Format As GPUVERTEX_FETCH_CONSTANT

    End Class

    Public Class GPUVERTEX_FETCH_CONSTANT
        'GPUVERTEX_FETCH_CONSTANT    0x1bfc
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.Value_0 = r.ReadUInt32
            Me.Value_4 = r.ReadUInt32

            'Me.Unknown_7 = r.ReadUInt32 '(dump:) type or BaseAddress maybe --> value increases to the next vertex for example in first vertex 0xB, 2nd 0x13, 3th 0x1B…
            ''0x12BF 0x1BA1
            'Me.Unknown_8 = r.ReadByte '(dump:) maybe Endian, 0x10, 0x10
            'Dim SizeVertexBuffer_multiplier As Byte = r.ReadByte
            'Dim SizeVertexBuffer_value As UShort = r.ReadUInt16
            'Me.SizeVertexBuffer = SizeVertexBuffer_value + (SizeVertexBuffer_multiplier * 65536) 'CInt("&H00" & Hex(SizeVertexBuffer_multiplier) & Hex(SizeVertexBuffer_value)) 'vertex buffer size + 2

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.Value_0)
            w.Write(Me.Value_4)

            'w.Write(Me.Unknown_7)
            'w.Write(Me.Unknown_8)
            'Dim SizeVertexBuffer_multiplier As Byte = 0
            'Dim SizeVertexBuffer_value As UInteger = Me.SizeVertexBuffer
            'Do While SizeVertexBuffer_value > UShort.MaxValue
            '    SizeVertexBuffer_value -= 65536
            '    SizeVertexBuffer_multiplier += 1
            'Loop
            'w.Write(SizeVertexBuffer_multiplier)
            'w.Write(CUShort(SizeVertexBuffer_value))

        End Sub

        Private Value_0 As UInteger = 0
        Private Value_4 As UInteger = 0

        Public Property m_Type As UInteger  '0x12BF
            Get
                Return GetValueFrom32bit(Value_0, 0, 2)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 0, 2)
            End Set
        End Property

        Public Property BaseAddress As UInteger 'PtrVertexBuffer    0x1BA1
            Get
                Return GetValueFrom32bit(Value_0, 2, 30)
            End Get
            Set
                Value_0 = SetValueTo32bit(Value_0, Value, 2, 30)
            End Set
        End Property
        Public Property Endian As UInteger  '0x12BF --> GPUENDIAN 0x101e ??
            Get
                Return GetValueFrom32bit(Value_4, 0, 2)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 0, 2)
            End Set
        End Property
        Public Property SizeVertexBuffer As UInteger    'Size 0x1A0A
            Get
                Return GetValueFrom32bit(Value_4, 2, 24) * 4
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value \ 4, 2, 24)
            End Set
        End Property
        Public Property AddressClamp As UInteger    '0x1371
            Get
                Return GetValueFrom32bit(Value_4, 26, 1)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 26, 1)
            End Set
        End Property
        Public Property RequestSize As UInteger '0x1BFA
            Get
                Return GetValueFrom32bit(Value_4, 28, 2)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 28, 2)
            End Set
        End Property
        Public Property ClampDisable As UInteger    '0x1638
            Get
                Return GetValueFrom32bit(Value_4, 30, 2)
            End Get
            Set
                Value_4 = SetValueTo32bit(Value_4, Value, 30, 2)
            End Set
        End Property
    End Class
End Namespace