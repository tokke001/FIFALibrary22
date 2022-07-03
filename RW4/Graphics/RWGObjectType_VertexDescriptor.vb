Namespace Rw.Graphics
    Public Class VertexDescriptor
        'rw::graphics::VertexDescriptor     
        Inherits RwObject
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

            Me.D3dVertexDeclaration = r.ReadUInt32  'always 0, i think pointer to "D3DResource" (maybe unused at FIFA ?)
            r.ReadUInt32()                          'Me.TypesFlags = flags of elementtypes used
            Dim m_NumElements As Byte = r.ReadByte
            r.ReadByte()                            'Me.VertexStride
            r.ReadByte()                            'Me.NumTextureCoordinates = number of texture-coordinaes
            Me.Pad = r.ReadByte                     'value 0 - padding ?
            r.ReadUInt32()                          'Me.ElementHash = hash of elementtypes used

            Me.Elements = New Element(m_NumElements - 1) {}
            For i = 0 To m_NumElements - 1
                Me.Elements(i) = New Element(r)
            Next i

        End Sub

        Public Function GetTypesFlags(ByVal m_Elements As Element()) As ETypeFlags  'Calulate TypesFlags from Elements array
            Dim m_TypesFlags As ETypeFlags = 0

            For i = 0 To m_Elements.Length - 1
                Select Case m_Elements(i).TypeCode
                    Case ElementType.ELEMENTTYPE_XBOX2_XYZ
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_XYZ
                    Case ElementType.ELEMENTTYPE_XBOX2_XYZRHW
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_XYZRHW
                    Case ElementType.ELEMENTTYPE_XBOX2_NORMAL
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_NORMAL
                    Case ElementType.ELEMENTTYPE_XBOX2_VERTEXCOLOR
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_VERTEXCOLOR
                    Case ElementType.ELEMENTTYPE_XBOX2_PRELIT
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_PRELIT
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX0
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TEX0
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX1
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TEX1
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX2
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TEX2
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX3
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TEX3
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX4
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TEX4
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX5
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TEX5
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX6
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TEX6
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX7
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TEX7
                    Case ElementType.ELEMENTTYPE_XBOX2_INDICES
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_INDICES
                    Case ElementType.ELEMENTTYPE_XBOX2_WEIGHTS
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_WEIGHTS
                    Case ElementType.ELEMENTTYPE_XBOX2_XYZ2
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_XYZ2
                    Case ElementType.ELEMENTTYPE_XBOX2_NORMAL2
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_NORMAL2
                    Case ElementType.ELEMENTTYPE_XBOX2_XZ
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_XZ
                    Case ElementType.ELEMENTTYPE_XBOX2_Y
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_Y
                    Case ElementType.ELEMENTTYPE_XBOX2_Y2
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_Y2
                    Case ElementType.ELEMENTTYPE_XBOX2_TANGENT
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_TANGENT
                    Case ElementType.ELEMENTTYPE_XBOX2_BINORMAL
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_BINORMAL
                    Case ElementType.ELEMENTTYPE_XBOX2_SPECULAR
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_SPECULAR
                    Case ElementType.ELEMENTTYPE_XBOX2_FOG
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_FOG
                    Case ElementType.ELEMENTTYPE_XBOX2_PSIZE
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_PSIZE
                    Case ElementType.ELEMENTTYPE_XBOX2_INDICES2
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_INDICES2
                    Case ElementType.ELEMENTTYPE_XBOX2_WEIGHTS2
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_WEIGHTS2
                    Case ElementType.ELEMENTTYPE_XBOX2_MAX
                        m_TypesFlags = m_TypesFlags Or ETypeFlags.TYPEFLAG_MAX
                End Select
            Next

            Return m_TypesFlags
        End Function

        Public Function GetElementHash(ByVal m_Elements As Element()) As EElementHash  'Calulate m_ElementHash from Elements array
            Dim m_ElementHash As EElementHash = 0

            For i = 0 To m_Elements.Length - 1
                Select Case m_Elements(i).TypeCode
                    Case ElementType.ELEMENTTYPE_XBOX2_XYZ
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_XYZ
                    'Case ElementType.ELEMENTTYPE_XBOX2_XYZRHW

                    Case ElementType.ELEMENTTYPE_XBOX2_NORMAL
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_NORMAL
                    Case ElementType.ELEMENTTYPE_XBOX2_VERTEXCOLOR
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_VERTEXCOLOR
                    'Case ElementType.ELEMENTTYPE_XBOX2_PRELIT
                        'm_ElementHash = m_ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX0
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TEX0
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX1
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TEX1
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX2
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TEX2
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX3
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TEX3
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX4
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TEX4
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX5
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TEX5
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX6
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TEX6
                    Case ElementType.ELEMENTTYPE_XBOX2_TEX7
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TEX7
                    Case ElementType.ELEMENTTYPE_XBOX2_INDICES
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_INDICES
                    Case ElementType.ELEMENTTYPE_XBOX2_WEIGHTS
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_WEIGHTS
                    'Case ElementType.ELEMENTTYPE_XBOX2_XYZ2
                    '    'm_ElementHash = m_ElementHash or EElementHash.
                    'Case ElementType.ELEMENTTYPE_XBOX2_NORMAL2
                    '    'm_ElementHash = m_ElementHash or EElementHash.
                    'Case ElementType.ELEMENTTYPE_XBOX2_XZ
                    '    'm_ElementHash = m_ElementHash or EElementHash.
                    'Case ElementType.ELEMENTTYPE_XBOX2_Y
                    '    'm_ElementHash = m_ElementHash or EElementHash.
                    'Case ElementType.ELEMENTTYPE_XBOX2_Y2
                    '    'm_ElementHash = m_ElementHash or EElementHash.
                    Case ElementType.ELEMENTTYPE_XBOX2_TANGENT
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_TANGENT
                    Case ElementType.ELEMENTTYPE_XBOX2_BINORMAL
                        m_ElementHash = m_ElementHash Or EElementHash.ELEMENTHASH_BINORMAL
                        'Case ElementType.ELEMENTTYPE_XBOX2_SPECULAR
                        '    'm_ElementHash = m_ElementHash or EElementHash.
                        'Case ElementType.ELEMENTTYPE_XBOX2_FOG
                        '    'm_ElementHash = m_ElementHash or EElementHash.
                        'Case ElementType.ELEMENTTYPE_XBOX2_PSIZE
                        '    'm_ElementHash = m_ElementHash or EElementHash.
                        'Case ElementType.ELEMENTTYPE_XBOX2_INDICES2
                        '    'm_ElementHash = m_ElementHash or EElementHash.
                        'Case ElementType.ELEMENTTYPE_XBOX2_WEIGHTS2
                        '    'm_ElementHash = m_ElementHash or EElementHash.
                        'Case ElementType.ELEMENTTYPE_XBOX2_MAX
                        '    'm_ElementHash = m_ElementHash or EElementHash.
                End Select
            Next

            Return m_ElementHash
        End Function

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

        Public Shared Function GetNumTextCoos(ByVal ListVertexElements As FIFALibrary22.VertexElement()) As Integer
            Dim ReturnValue As Integer = 0

            For i = 0 To ListVertexElements.Length - 1
                If ListVertexElements(i).Usage = Microsoft.DirectX.Direct3D.DeclarationUsage.TextureCoordinate Then
                    ReturnValue += 1
                End If
            Next i

            Return ReturnValue
        End Function

        Public Overrides Sub Save(ByVal w As FileWriter)
            'Me.NumElements = CUInt(Me.Elements.Length)
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

        Public Property D3dVertexDeclaration As UInteger = 0
        ''' <summary>
        ''' Returns the flags of ElementTypes used (ReadOnly) . </summary>
        Public ReadOnly Property TypesFlags As ETypeFlags
            Get
                Return GetTypesFlags(Me.Elements)
            End Get
        End Property
        ''' <summary>
        ''' Returns the number of Vertex-Format Elements (ReadOnly) . </summary>
        Public ReadOnly Property NumElements As Byte
            Get
                Return If(Elements?.Count, 0)
            End Get
        End Property
        ''' <summary>
        ''' Size of 1 Vertex (ReadOnly). </summary>
        Public ReadOnly Property VertexStride As Byte  'm_stride
            Get
                Return FifaUtil.GetVertexStride(Me.Elements)
            End Get
        End Property

        ''' <summary>
        ''' Returns the number of Texture-Coordinates in the VertexFormat (ReadOnly) . </summary>
        Public ReadOnly Property NumTextureCoordinates As Byte
            Get
                Return GetNumTextCoos(Me.Elements)
            End Get
        End Property
        ''' <summary>
        ''' Empty 0-value. </summary>
        Public Property Pad As Byte = 0 'padding maybe??
        ''' <summary>
        ''' Returns the Hash of ElementTypes used (ReadOnly) . </summary>
        Public ReadOnly Property ElementHash As EElementHash
            Get
                Return GetElementHash(Me.Elements)
            End Get
        End Property
        ''' <summary>
        ''' Gets/Sets the VertexFormat Elements. </summary>
        Public Property Elements As Element()

        Public Class Element
            Inherits VertexElement

            'rw::graphics::VertexDescriptor::Element
            Public Sub New()

            End Sub

            Public Sub New(ByVal m_VertexElement As VertexElement, Optional Stream As UShort = 0, Optional Method As Microsoft.DirectX.Direct3D.DeclarationMethod = Microsoft.DirectX.Direct3D.DeclarationMethod.Default)
                MyBase.Offset = m_VertexElement.Offset
                MyBase.DataType = m_VertexElement.DataType
                MyBase.Usage = m_VertexElement.Usage
                MyBase.UsageIndex = m_VertexElement.UsageIndex
                Me.Stream = Stream
                Me.Method = Method
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
                r.ReadByte()                      'Me.TypeCode = RW DataTypeCode

            End Sub

            Private Function GetTypeCode(ByVal m_Usage As Microsoft.DirectX.Direct3D.DeclarationUsage, ByVal UsageIndex As Byte) As ElementType
                Select Case m_Usage
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.Position
                        If UsageIndex = 1 Then
                            Return ElementType.ELEMENTTYPE_XBOX2_XYZ2
                        Else
                            Return ElementType.ELEMENTTYPE_XBOX2_XYZ
                        End If
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.Sample
                        Return 0
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.Depth
                        Return 0
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.Fog
                        Return ElementType.ELEMENTTYPE_XBOX2_FOG
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.Color
                        Return ElementType.ELEMENTTYPE_XBOX2_VERTEXCOLOR
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.PositionTransformed
                        Return 0
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.TessellateFactor
                        Return 0
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.BiNormal
                        Return ElementType.ELEMENTTYPE_XBOX2_BINORMAL
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.Tangent
                        Return ElementType.ELEMENTTYPE_XBOX2_TANGENT
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.TextureCoordinate
                        Select Case UsageIndex
                            Case 0
                                Return ElementType.ELEMENTTYPE_XBOX2_TEX0
                            Case 1
                                Return ElementType.ELEMENTTYPE_XBOX2_TEX1
                            Case 2
                                Return ElementType.ELEMENTTYPE_XBOX2_TEX2
                            Case 3
                                Return ElementType.ELEMENTTYPE_XBOX2_TEX3
                            Case 4
                                Return ElementType.ELEMENTTYPE_XBOX2_TEX4
                            Case 5
                                Return ElementType.ELEMENTTYPE_XBOX2_TEX5
                            Case 6
                                Return ElementType.ELEMENTTYPE_XBOX2_TEX6
                            Case 7
                                Return ElementType.ELEMENTTYPE_XBOX2_TEX7
                        End Select
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.BlendIndices
                        If UsageIndex = 1 Then
                            Return ElementType.ELEMENTTYPE_XBOX2_INDICES2
                        Else
                            Return ElementType.ELEMENTTYPE_XBOX2_INDICES
                        End If
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.BlendWeight
                        If UsageIndex = 1 Then
                            Return ElementType.ELEMENTTYPE_XBOX2_WEIGHTS2
                        Else
                            Return ElementType.ELEMENTTYPE_XBOX2_WEIGHTS
                        End If
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.PointSize
                        Return ElementType.ELEMENTTYPE_XBOX2_PSIZE
                    Case Microsoft.DirectX.Direct3D.DeclarationUsage.Normal
                        If UsageIndex = 1 Then
                            Return ElementType.ELEMENTTYPE_XBOX2_NORMAL2
                        Else
                            Return ElementType.ELEMENTTYPE_XBOX2_NORMAL
                        End If
                End Select
            End Function

            Public Sub Save(ByVal w As FileWriter)

                w.Write(Me.Stream)
                w.Write(CUShort(MyBase.Offset))
                w.Write(CUInt(MyBase.DataType))

                w.Write(CByte(Me.Method))
                w.Write(CByte(MyBase.Usage))
                w.Write(MyBase.UsageIndex)
                w.Write(CByte(Me.TypeCode))

            End Sub

            ''' <summary>
            ''' Retrieves or sets the stream number (or index) to use. </summary>
            Public Property Stream As UShort    'Stream number (or index) to use --> https://docs.microsoft.com/en-us/previous-versions/ms847453(v=msdn.10)
            'Public Property Offset As UShort
            'Public Property DataType As Rw.D3D.D3DDECLTYPE
            ''' <summary>
            ''' Retrieves or sets the tessellator processing method. </summary>
            Public Property Method As Microsoft.DirectX.Direct3D.DeclarationMethod
            'Public Property Usage As Microsoft.DirectX.Direct3D.DeclarationUsage
            'Public Property UsageIndex As Byte
            ''' <summary>
            ''' Retrieves or sets the Rw Typecode that define the intended use of the vertex data. </summary>
            Public ReadOnly Property TypeCode As ElementType
                Get
                    Return GetTypeCode(MyBase.Usage, MyBase.UsageIndex)
                End Get
            End Property
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