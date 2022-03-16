Namespace Rw.Graphics
    Public Class VertexDescriptor
        'rw::graphics::VertexDescriptor     
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.D3dVertexDeclaration = r.ReadUInt32 'always 0, i think pointer to "D3DResource" (maybe unused at FIFA ?)
            Me.TypesFlags = r.ReadUInt32            'flags of elementtypes used
            Me.NumElements = r.ReadByte
            Me.VertexStride = r.ReadByte
            Me.NumTextureCoordinates = r.ReadByte   'number of texture-coordinaes
            Me.Pad = r.ReadByte                     'value 0 - padding ?
            Me.ElementHash = r.ReadUInt32           'hash of elementtypes used

            Me.Elements = New Element(Me.NumElements - 1) {}
            For i = 0 To Me.NumElements - 1
                Me.Elements(i) = New Element(r)
            Next i

        End Sub

        Public Sub CalcFlagsAndHash()   'Calulate Me.TypesFlags and Me.ElementHash from Me.Elements array
            Me.TypesFlags = 0
            Me.ElementHash = 0

            For i = 0 To Me.Elements.Length - 1
                Select Case Me.Elements(i).TypeCode
                    Case ElementType.ELEMENTTYPE_XBOX2_XYZ
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_XYZ
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_XYZ
                    Case ElementType.ELEMENTTYPE_XBOX2_XYZRHW
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_XYZRHW
                    Case ElementType.ELEMENTTYPE_XBOX2_NORMAL
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_NORMAL
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_NORMAL
                    Case ElementType.ELEMENTTYPE_XBOX2_VERTEXCOLOR
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_VERTEXCOLOR
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_VERTEXCOLOR
                    Case ElementType.ELEMENTTYPE_XBOX2_PRELIT
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_PRELIT
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX0
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TEX0
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TEX0
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX1
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TEX1
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TEX1
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX2
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TEX2
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TEX2
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX3
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TEX3
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TEX3
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX4
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TEX4
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TEX4
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX5
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TEX5
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TEX5
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX6
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TEX6
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TEX6
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX7
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TEX7
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TEX7
                    Case ElementType.ELEMENTTYPE_XBOX2_INDICES
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_INDICES
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_INDICES
                    Case ElementType.ELEMENTTYPE_XBOX2_WEIGHTS
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_WEIGHTS
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_WEIGHTS
                    Case ElementType.ELEMENTTYPE_XBOX2_XYZ2
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_XYZ2
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_NORMAL2
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_NORMAL2
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_XZ
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_XZ
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_Y
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_Y
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_Y2
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_Y2
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_TANGENT
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_TANGENT
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_TANGENT
                    Case ElementType.ELEMENTTYPE_XBOX2_BINORMAL
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_BINORMAL
                        Me.ElementHash = Me.ElementHash Or EElementHash.ELEMENTHASH_BINORMAL
                    Case ElementType.ELEMENTTYPE_XBOX2_SPECULAR
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_SPECULAR
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_FOG
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_FOG
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_PSIZE
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_PSIZE
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_INDICES2
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_INDICES2
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_WEIGHTS2
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_WEIGHTS2
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_MAX
                        Me.TypesFlags = Me.TypesFlags Or ETypeFlags.TYPEFLAG_MAX
                        'Me.ElementHash = Me.ElementHash or EElementHash.
                End Select
            Next

        End Sub

        'Private Sub debug()
        '    Select Case ElementHash
        '        Case 1359413505 '3 uvs
        '        Case 16843009   'ball no ani
        '        Case 1359151361 '2 uvs
        '        Case 1359020289 '1 uv
        '        Case 16974081   '2 uvs no ani
        '        Case 16973825   '2 uvs no ani no tangent
        '        Case 196609     '2 uvs no ani no tangent no normal
        '        Case 16974097   '2 uvs normal, binormal, tangent
        '        Case 65537      'XYZ, 1UV
        '        Case 4097       'XYZ, vertexcolor
        '        Case 16843025   '1 uvs normal, binormal, tangent
        '        Case 16977921   'XYZ, 2 UV, Color, normal
        '        Case 16978193   'XYZ, 2 UV, Color, normal, tangent, binormal
        '        Case 69633      'XYZ, 1 uv, vertexcolor
        '        Case 200705     'XYZ, 2 uvs, color
        '        Case Else

        '    End Select
        'End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumElements = CUInt(Me.Elements.Length)
            'Me.CalcFlagsAndHash()  --> left out to save time at writing 

            w.Write(Me.D3dVertexDeclaration)
            w.Write(CUInt(Me.TypesFlags))
            w.Write(Me.NumElements)
            w.Write(Me.VertexStride)
            w.Write(Me.NumTextureCoordinates)
            w.Write(Me.Pad)
            w.Write(CUInt(Me.ElementHash))


            For i = 0 To Me.NumElements - 1
                Me.Elements(i).Save(w)
            Next i

        End Sub

        Public Property D3dVertexDeclaration As UInteger
        Public Property TypesFlags As ETypeFlags
        Public Property NumElements As Byte
        Public Property VertexStride As Byte  'm_stride
        Public Property NumTextureCoordinates As Byte
        Public Property Pad As Byte = 0 'padding maybe??
        Public Property ElementHash As EElementHash
        Public Property Elements As Element()

        Public Class Element
            Inherits VertexElement
            'rw::graphics::VertexDescriptor::Element
            Public Sub New()

            End Sub

            Public Sub New(ByVal r As FileReader)
                Me.Load(r)
            End Sub

            Public Sub Load(ByVal r As FileReader)

                Me.Stream = r.ReadUInt16
                MyBase.Offset = r.ReadUInt16
                MyBase.DataType = r.ReadUInt32  'Format

                Me.Method = r.ReadByte
                MyBase.Usage = r.ReadByte
                MyBase.UsageIndex = r.ReadByte
                Me.TypeCode = r.ReadByte   'RW DataTypeCode

            End Sub

            Public Sub Save(ByVal w As FileWriter)

                w.Write(Me.Stream)
                w.Write(MyBase.Offset)
                w.Write(CUInt(MyBase.DataType))

                w.Write(CByte(Me.Method))
                w.Write(CByte(MyBase.Usage))
                w.Write(MyBase.UsageIndex)
                w.Write(CByte(Me.TypeCode))

            End Sub

            Public Property Stream As UShort
            'Public Property Offset As UShort
            'Public Property DataType As Rw.D3D.D3DDECLTYPE
            Public Property Method As Microsoft.DirectX.Direct3D.DeclarationMethod
            'Public Property Usage As Microsoft.DirectX.Direct3D.DeclarationUsage
            'Public Property UsageIndex As Byte
            Public Property TypeCode As ElementType

        End Class

        <Flags>
        Public Enum ETypeFlags As UInteger  'guessed, but seems same as rw::graphics::VertexDescriptor::ElementType !
            TYPEFLAG_NA = 1
            TYPEFLAG_XYZ = 2
            TYPEFLAG_XYZRHW = 4
            TYPEFLAG_NORMAL = 8
            TYPEFLAG_VERTEXCOLOR = 16
            TYPEFLAG_PRELIT = 32
            TYPEFLAG_TEX0 = 64
            TYPEFLAG_TEX1 = 128
            TYPEFLAG_TEX2 = 256
            TYPEFLAG_TEX3 = 512
            TYPEFLAG_TEX4 = 1024
            TYPEFLAG_TEX5 = 2048
            TYPEFLAG_TEX6 = 4096
            TYPEFLAG_TEX7 = 8192
            TYPEFLAG_INDICES = 16384
            TYPEFLAG_WEIGHTS = 32768
            TYPEFLAG_XYZ2 = 65536
            TYPEFLAG_NORMAL2 = 131072
            TYPEFLAG_XZ = 262144
            TYPEFLAG_Y = 524288
            TYPEFLAG_Y2 = 1048576
            TYPEFLAG_TANGENT = 2097152
            TYPEFLAG_BINORMAL = 4194304
            TYPEFLAG_SPECULAR = 8388608
            TYPEFLAG_FOG = 16777216
            TYPEFLAG_PSIZE = 33554432
            TYPEFLAG_INDICES2 = 67108864
            TYPEFLAG_WEIGHTS2 = 134217728
            TYPEFLAG_MAX = 268435456
        End Enum

        <Flags>
        Public Enum EElementHash As UInteger  'guessed, values not found at dump
            ELEMENTHASH_XYZ = 1
            ELEMENTHASH_BINORMAL = 16
            ELEMENTHASH_TANGENT = 256
            ELEMENTHASH_VERTEXCOLOR = 4096
            ELEMENTHASH_TEX0 = 65536
            ELEMENTHASH_TEX1 = 131072
            ELEMENTHASH_TEX2 = 262144
            ELEMENTHASH_TEX3 = 524288    'guessed: not confirmed, because not found
            ELEMENTHASH_TEX4 = 1048576   'guessed: not confirmed, because not found
            ELEMENTHASH_TEX5 = 2097152   'guessed: not confirmed, because not found
            ELEMENTHASH_TEX6 = 4194304   'guessed: not confirmed, because not found
            ELEMENTHASH_TEX7 = 8388608   'guessed: not confirmed, because not found
            ELEMENTHASH_NORMAL = 16777216
            ELEMENTHASH_INDICES = 268435456     'indices & weights may be switched, but they r alaways together !
            ELEMENTHASH_WEIGHTS = 1073741824    'guessed: indices & weights may be switched, but they r alaways together !
        End Enum

        Public Enum ElementType As Byte 'rw::graphics::VertexDescriptor::ElementType
            ELEMENTTYPE_NA = 0
            ELEMENTTYPE_XBOX2_XYZ = 1
            ELEMENTTYPE_XBOX2_XYZRHW = 2
            ELEMENTTYPE_XBOX2_NORMAL = 3
            ELEMENTTYPE_XBOX2_VERTEXCOLOR = 4
            ELEMENTTYPE_XBOX2_PRELIT = 5
            ELEMENTTYPE_XBOX2_TEX0 = 6
            ELEMENTTYPE_XBOX2_TEX1 = 7
            ELEMENTTYPE_XBOX2_TEX2 = 8
            ELEMENTTYPE_XBOX2_TEX3 = 9
            ELEMENTTYPE_XBOX2_TEX4 = 10
            ELEMENTTYPE_XBOX2_TEX5 = 11
            ELEMENTTYPE_XBOX2_TEX6 = 12
            ELEMENTTYPE_XBOX2_TEX7 = 13
            ELEMENTTYPE_XBOX2_INDICES = 14
            ELEMENTTYPE_XBOX2_WEIGHTS = 15
            ELEMENTTYPE_XBOX2_XYZ2 = 16
            ELEMENTTYPE_XBOX2_NORMAL2 = 17
            ELEMENTTYPE_XBOX2_XZ = 18
            ELEMENTTYPE_XBOX2_Y = 19
            ELEMENTTYPE_XBOX2_Y2 = 20
            ELEMENTTYPE_XBOX2_TANGENT = 21
            ELEMENTTYPE_XBOX2_BINORMAL = 22
            ELEMENTTYPE_XBOX2_SPECULAR = 23
            ELEMENTTYPE_XBOX2_FOG = 24
            ELEMENTTYPE_XBOX2_PSIZE = 25
            ELEMENTTYPE_XBOX2_INDICES2 = 26
            ELEMENTTYPE_XBOX2_WEIGHTS2 = 27
            ELEMENTTYPE_XBOX2_MAX = 28
            'ELEMENTTYPE_XYZ = 1
            'ELEMENTTYPE_UV = 6
            'ELEMENTTYPE_RGB = 5
            'ELEMENTTYPE_RGBA = 4
            'ELEMENTTYPE_NORMAL = 3
            'ELEMENTTYPE_TANGENT = 21
            'ELEMENTTYPE_BINORMAL = 22
            'ELEMENTTYPE_IND8 = 0
            'ELEMENTTYPE_IND16 = 0
            'ELEMENTTYPE_IND32 = 0
        End Enum

        'Public Enum VertexElementUsage As Integer 'rw::graphics::VertexDescriptor::VertexElementUsage  --> unused/wrong values !
        '    ELEMENTUSAGE_POSITION = 0
        '    ELEMENTUSAGE_UV = 1
        '    ELEMENTUSAGE_COLOR = 2
        '    ELEMENTUSAGE_NORMAL = 3
        '    ELEMENTUSAGE_TANGENT = 4
        '    ELEMENTUSAGE_BINORMAL = 5
        '    ELEMENTUSAGE_INDICES = 6
        '    ELEMENTUSAGE_WEIGHTS = 7
        '    ELEMENTUSAGE_ATTR = 8
        '    ELEMENTUSAGE_NA = 9
        'End Enum
        'Public Enum VertexElementFormat As Integer 'enum name = rw::graphics::VertexDescriptor::VertexElementFormat  --> unused/wrong values !
        '    ELEMENTFORMAT_FLOAT1 = 0
        '    ELEMENTFORMAT_FLOAT2 = 1
        '    ELEMENTFORMAT_FLOAT3 = 2
        '    ELEMENTFORMAT_FLOAT4 = 3
        '    ELEMENTFORMAT_SHORT1 = 4
        '    ELEMENTFORMAT_SHORT2 = 5
        '    ELEMENTFORMAT_SHORT3 = 6
        '    ELEMENTFORMAT_SHORT4 = 7
        '    ELEMENTFORMAT_SHORT1N = 8
        '    ELEMENTFORMAT_SHORT2N = 9
        '    ELEMENTFORMAT_SHORT3N = 10
        '    ELEMENTFORMAT_SHORT4N = 11
        '    ELEMENTFORMAT_USHORT1 = 12
        '    ELEMENTFORMAT_USHORT2 = 13
        '    ELEMENTFORMAT_USHORT3 = 14
        '    ELEMENTFORMAT_USHORT4 = 15
        '    ELEMENTFORMAT_USHORT1N = 16
        '    ELEMENTFORMAT_USHORT2N = 17
        '    ELEMENTFORMAT_USHORT3N = 18
        '    ELEMENTFORMAT_USHORT4N = 19
        '    ELEMENTFORMAT_UBYTE1 = 20
        '    ELEMENTFORMAT_UBYTE2 = 21
        '    ELEMENTFORMAT_UBYTE3 = 22
        '    ELEMENTFORMAT_UBYTE4 = 23
        '    ELEMENTFORMAT_BYTE1N = 24
        '    ELEMENTFORMAT_BYTE2N = 25
        '    ELEMENTFORMAT_BYTE3N = 26
        '    ELEMENTFORMAT_BYTE4N = 27
        '    ELEMENTFORMAT_UBYTE1N = 28
        '    ELEMENTFORMAT_UBYTE2N = 29
        '    ELEMENTFORMAT_UBYTE3N = 30
        '    ELEMENTFORMAT_UBYTE4N = 31
        '    ELEMENTFORMAT_BYTE4 = 32
        '    ELEMENTFORMAT_UDEC3N = 33
        '    ELEMENTFORMAT_UDEC3 = 34
        '    ELEMENTFORMAT_DEC3N = 35
        '    ELEMENTFORMAT_DEC3 = 36
        '    ELEMENTFORMAT_HALF1 = 37
        '    ELEMENTFORMAT_HALF2 = 38
        '    ELEMENTFORMAT_HALF3 = 39
        '    ELEMENTFORMAT_HALF4 = 40
        '    ELEMENTFORMAT_COLOR = 41
        '    ELEMENTFORMAT_COLOR_RGB = 42
        '    ELEMENTFORMAT_PACKED3N = 43
        '    ELEMENTFORMAT_FLOATXYWN = 44
        '    ELEMENTFORMAT_NA = 45
        'End Enum

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace