Namespace Rw.EA.FxShader
    Public Class ParameterBlockDescriptor
        'EA::FxShader::ParameterBlockDescriptor
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.EA_FxShader_ParameterBlockDescriptor
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)    '----- ALWAYS little endian !!! -----
            Dim m_CurrentEndianness = r.Endianness  'Get File Endian
            r.Endianness = Endian.Little            'Set new Endian

            Me.RefCount = r.ReadUInt32             'always 2   (number of values header ?)
            Me.SizeData = r.ReadUInt32   'Size of all parameter values (at &HEF0001 EA_FxShader_ParameterBlock)
            Me.NumParameters = r.ReadUInt32
            Me.Padding = r.ReadUInt32             'always 0 (padding ?)

            Me.ParameterInfos = New ParameterInfo(Me.NumParameters - 1) {}
            For i = 0 To Me.NumParameters - 1
                Me.ParameterInfos(i) = New ParameterInfo
                Me.ParameterInfos(i).Id_1 = r.ReadByte                    'unknown Id????   'starts at 0, goes +1 but not always
                Me.ParameterInfos(i).OffsetParameterValue = r.ReadUInt16  'Offset to ParameterValue (at &HEF0001 EA_FxShader_ParameterBlock, 0 is first ParameterValue)
                Me.ParameterInfos(i).Id_Index = r.ReadByte                   'index?   'always + 1 (starting 0)
            Next i

            For i = 0 To Me.NumParameters - 1
                Me.ParameterInfos(i).OffsetParameterName1 = r.ReadUInt32  'offset to Name1 (nameTable)
            Next
            For i = 0 To Me.NumParameters - 1
                Me.ParameterInfos(i).OffsetParameterName2 = r.ReadUInt32  'offset to Name2: name can be empty string, but always Null-terminator (nameTable)
            Next
            For i = 0 To Me.NumParameters - 1
                Me.ParameterInfos(i).OffsetParameterTypeName = r.ReadUInt32  'offset to Name of ParameterType (nameTable)
            Next
            For i = 0 To Me.NumParameters - 1
                Me.ParameterInfos(i).ParameterType = r.ReadUInt32  'Value wich defines the parameter type ?
            Next

            'string names follow 
            For i = 0 To Me.NumParameters - 1
                r.BaseStream.Position = Me.ParameterInfos(i).OffsetParameterName1
                Me.ParameterInfos(i).ParameterName1 = FifaUtil.ReadNullTerminatedString(r)        'Null-terminator
                r.BaseStream.Position = Me.ParameterInfos(i).OffsetParameterName2
                Me.ParameterInfos(i).ParameterName2 = FifaUtil.ReadNullTerminatedString(r)        'name can be empty string, but always Null-terminator
                r.BaseStream.Position = Me.ParameterInfos(i).OffsetParameterTypeName
                Me.ParameterInfos(i).ParameterTypeName = FifaUtil.ReadNullTerminatedString(r)     'Null-terminator
            Next

            r.Endianness = m_CurrentEndianness      'Set File Endian
        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Dim m_CurrentEndianness = w.Endianness  'Get File Endian
            w.Endianness = Endian.Little            'Set new Endian
            Me.NumParameters = Me.ParameterInfos.Length

            w.Write(Me.RefCount)
            w.Write(Me.SizeData)
            w.Write(Me.NumParameters)
            w.Write(Me.Padding)

            For i = 0 To Me.NumParameters - 1
                w.Write(Me.ParameterInfos(i).Id_1)
                w.Write(Me.ParameterInfos(i).OffsetParameterValue)
                w.Write(Me.ParameterInfos(i).Id_Index)
            Next i

            Dim BaseOffset As Long = w.BaseStream.Position
            'write with wrong offsets
            For i = 0 To Me.NumParameters - 1
                w.Write(Me.ParameterInfos(i).OffsetParameterName1)
            Next
            For i = 0 To Me.NumParameters - 1
                w.Write(Me.ParameterInfos(i).OffsetParameterName2)
            Next
            For i = 0 To Me.NumParameters - 1
                w.Write(Me.ParameterInfos(i).OffsetParameterTypeName)
            Next
            For i = 0 To Me.NumParameters - 1
                w.Write(CUInt(Me.ParameterInfos(i).ParameterType))
            Next

            For i = 0 To Me.NumParameters - 1
                Me.ParameterInfos(i).OffsetParameterName1 = w.BaseStream.Position
                FifaUtil.WriteNullTerminatedString(w, Me.ParameterInfos(i).ParameterName1)
                Me.ParameterInfos(i).OffsetParameterName2 = w.BaseStream.Position
                FifaUtil.WriteNullTerminatedString(w, Me.ParameterInfos(i).ParameterName2)
                Me.ParameterInfos(i).OffsetParameterTypeName = w.BaseStream.Position
                FifaUtil.WriteNullTerminatedString(w, Me.ParameterInfos(i).ParameterTypeName)
            Next

            Dim EndOffset As Long = w.BaseStream.Position

            'write again with correct offsets
            w.BaseStream.Position = BaseOffset
            For i = 0 To Me.NumParameters - 1
                w.Write(Me.ParameterInfos(i).OffsetParameterName1)
            Next
            For i = 0 To Me.NumParameters - 1
                w.Write(Me.ParameterInfos(i).OffsetParameterName2)
            Next
            For i = 0 To Me.NumParameters - 1
                w.Write(Me.ParameterInfos(i).OffsetParameterTypeName)
            Next
            w.BaseStream.Position = EndOffset

            w.Endianness = m_CurrentEndianness      'Set File Endian
        End Sub


        Public Property RefCount As UInteger
        Public Property SizeData As UInteger
        Public Property NumParameters As UInteger
        Public Property Padding As UInteger
        Public Property ParameterInfos As ParameterInfo()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class ParameterInfo
        'EA::FxShader::ParameterBlockBuilder::ParameterInfo ????
        Public Property Id_1 As Byte                    'unknown Id????   'starts at 0, goes +1 but not always
        Public Property OffsetParameterValue As UShort  'Offset to ParameterValue (at &HEF0001 EA_FxShader_ParameterBlock, 0 is first ParameterValue)
        Public Property Id_Index As Byte                   'index?        'always + 1 (starting 0)
        Public Property OffsetParameterName1 As UInteger         'offset to Name1 (nameTable)
        Public Property OffsetParameterName2 As UInteger         'offset to Name2: name can be empty string, but always Null-terminator (nameTable)
        Public Property OffsetParameterTypeName As UInteger 'offset to Name of ParameterType (nameTable)
        Public Property ParameterType As EParameterType       'Value wich defines the parameter type ?
        Public Property ParameterName1 As String                 'Null-terminator --> mName ?? 
        Public Property ParameterName2 As String                 'name can be empty string, but always Null-terminator  --> mType ??
        Public Property ParameterTypeName As String     'Null-terminator --> mResource ??

    End Class

    Public Enum EParameterType As UInteger 'EA::FxShader::DataTypeEnum??, _D3DXPARAMETER_TYPE ??
        int = 2
        float = 3
        [string] = 4
        string2 = 12
        sampler2D = 13
        string2_2 = 36  'might be 2 chars (length is 2) ?
        float4 = 99
        float4x4 = 123
        float4_180 = 23011
    End Enum
End Namespace