Imports Microsoft.DirectX

Namespace Rw.EA.FxShader
    Public Class ParameterBlock
        'EA::FxShader::ParameterBlock
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.EA_FxShader_ParameterBlock
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            'Public Sub Load(ByVal Descriptor As ParameterBlockDescriptor, ByVal r As FileReader)

            Me.RefCount = r.ReadBytes(4)             'always 1  (little endian) 
            Me.PDescriptor = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), ParameterBlockDescriptor)         'Section Index of HEF0000 ParameterBlockDescriptor
            Me.OffsetDataOffsetArray = r.ReadUInt32    '"mDataPointerArray" pointer to array of pointers to data
            Me.OffsetDataBuffer = r.ReadUInt32      'Offset to start data

            Me.DataBuffer = New Object(PDescriptor.NumParameters - 1) {}
            For i = 0 To PDescriptor.NumParameters - 1
                r.BaseStream.Position = Me.OffsetDataOffsetArray + (i * 4)
                r.BaseStream.Position = r.ReadUInt32
                Me.DataBuffer(i) = LoadData(PDescriptor.ParameterInfos(i), r)
            Next




            'Me.DataBuffer = New ParameterValue(Descriptor.NumParameters - 1) {}
            'For i = 0 To Descriptor.NumParameters - 1
            '    Me.DataBuffer(i) = New ParameterValue
            '    Me.DataBuffer(i).Offset = r.ReadUInt32
            'Next

            ''r.BaseStream.Position = Me.OffsetParameterValues
            'For i = 0 To Descriptor.NumParameters - 1
            '    r.BaseStream.Position = Me.DataBuffer(i).Offset
            '    Me.DataBuffer(i).Value = LoadParametervalue(r, Descriptor.ParameterDescriptors(i))
            'Next

        End Sub

        Private Function LoadData(ByVal ParameterDescriptors As ParameterInfo, ByVal r As FileReader) As Object

            Select Case ParameterDescriptors.ParameterType
                Case EParameterType.int
                    Return r.ReadInt32

                Case EParameterType.float
                    Return r.ReadSingle

                Case EParameterType.string
                    Return FifaUtil.ReadNullTerminatedString(r)

                Case EParameterType.string2
                    Dim m_string As String() = New String(2 - 1) {}
                    For i = 0 To m_string.Length - 1
                        m_string(i) = FifaUtil.ReadNullTerminatedString(r)
                    Next
                    Return m_string

                Case EParameterType.sampler2D   'contains section index of the texture (RWGOBJECTTYPE_RASTER = &H20003)
                    Return New Sampler2D With {
                        .PRaster = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), Rw.Graphics.Raster),
                        .Value_2 = r.ReadUInt32,
                        .Value_3 = r.ReadUInt32,
                        .Value_4 = r.ReadUInt32,
                        .Value_5 = r.ReadUInt32,
                        .Value_6 = r.ReadUInt32,
                        .Value_7 = r.ReadUInt32,
                        .Value_8 = r.ReadUInt32
                    }

                Case EParameterType.string2_2   'might be 2 chars (length is 2) ?
                    Dim chArray As Char() = New Char(2 - 1) {}
                    For i = 0 To chArray.Length - 1
                        chArray(i) = Convert.ToChar(r.ReadByte)
                    Next i
                    Return New String(chArray, 0, 2)

                Case EParameterType.float4
                    Return r.ReadVector4

                Case EParameterType.float4x4
                    Dim m_Float4 As Vector4() = New Vector4(4 - 1) {}
                    For i = 0 To m_Float4.Length - 1
                        m_Float4(i) = r.ReadVector4
                    Next
                    Return m_Float4

                Case EParameterType.float4_180
                    Dim m_Float4 As Vector4() = New Vector4(180 - 1) {}
                    For i = 0 To m_Float4.Length - 1
                        m_Float4(i) = r.ReadVector4
                    Next
                    Return m_Float4

                Case Else
                    MsgBox("EA_FxShader_ParameterBlock - EF0001: Unknown Parameter-value found at loading """ & ParameterDescriptors.ParameterTypeName & """")
                    Return 0
            End Select

        End Function

        Public Overrides Sub Save(ByVal w As FileWriter) ', ByVal ParameterBlockDescriptor As ParameterBlockDescriptor)

            Me.OffsetDataOffsetArray = w.BaseStream.Position + 16
            'Me.OffsetDataBuffer = Me.OffsetDataOffsetArray + (ParameterBlockDescriptor.NumParameters * 4) + allignment_16
            Dim OffsetsDatabuffer As UInteger() = New UInteger(Me.PDescriptor.NumParameters - 1) {}

            w.Write(Me.RefCount)
            w.Write(Me.RwArena.Sections.IndexOf(Me.PDescriptor))
            w.Write(Me.OffsetDataOffsetArray)

            Dim BaseOffset As Long = w.BaseStream.Position
            '-- write with wrong offsets
            w.Write(Me.OffsetDataBuffer)

            For i = 0 To Me.PDescriptor.NumParameters - 1
                w.Write(OffsetsDatabuffer(i))
            Next

            'Write paddings
            FifaUtil.WriteAlignment_16(w)

            Me.OffsetDataBuffer = w.BaseStream.Position
            For i = 0 To Me.PDescriptor.NumParameters - 1
                OffsetsDatabuffer(i) = w.BaseStream.Position
                SaveData(Me.DataBuffer(i), Me.PDescriptor.ParameterInfos(i), w)
            Next

            Dim EndOffset As Long = w.BaseStream.Position

            '-- write again with correct offsets
            w.BaseStream.Position = BaseOffset
            w.Write(Me.OffsetDataBuffer)
            For i = 0 To Me.PDescriptor.NumParameters - 1
                w.Write(OffsetsDatabuffer(i))
            Next

            w.BaseStream.Position = EndOffset
        End Sub

        Private Sub SaveData(ByVal Value As Object, ByVal ParameterDescriptors As ParameterInfo, ByVal w As FileWriter)

            Select Case ParameterDescriptors.ParameterType
                Case EParameterType.int
                    w.Write(CInt(Value))

                Case EParameterType.float
                    w.Write(CSng(Value))

                Case EParameterType.string
                    FifaUtil.WriteNullTerminatedString(w, CStr(Value))

                Case EParameterType.string2
                    For i = 0 To Value.Length - 1
                        FifaUtil.WriteNullTerminatedString(w, CStr(Value(i)))
                    Next

                Case EParameterType.sampler2D   'contains section index of the texture (RWGOBJECTTYPE_RASTER = &H20003)
                    w.Write(Me.RwArena.Sections.IndexOf(CType(Value, Sampler2D).PRaster, 1 << 22))
                    w.Write(CType(Value, Sampler2D).Value_2)
                    w.Write(CType(Value, Sampler2D).Value_3)
                    w.Write(CType(Value, Sampler2D).Value_4)

                    w.Write(CType(Value, Sampler2D).Value_5)
                    w.Write(CType(Value, Sampler2D).Value_6)
                    w.Write(CType(Value, Sampler2D).Value_7)
                    w.Write(CType(Value, Sampler2D).Value_8)

                Case EParameterType.string2_2   'might be 2 chars (length is 2) ?
                    FifaUtil.WriteNullTerminatedString(w, CStr(Value))

                Case EParameterType.float4
                    w.Write(CType(Value, Vector4))

                Case EParameterType.float4x4
                    For i = 0 To CType(Value, Vector4()).Length - 1
                        w.Write(CType(Value(i), Vector4))
                    Next

                Case EParameterType.float4_180
                    For i = 0 To CType(Value, Vector4()).Length - 1
                        w.Write(CType(Value(i), Vector4))
                    Next

                Case Else
                    MsgBox("EA_FxShader_ParameterBlock - EF0001: Unknown Parameter-value found at saving """ & ParameterDescriptors.ParameterTypeName & """")
            End Select

            '-- Write paddings
            FifaUtil.WriteAlignment_16(w)
        End Sub


        Public Property RefCount As Byte() = New Byte(4 - 1) {}                'always 1  (little endian) 
        Public Property PDescriptor As ParameterBlockDescriptor   'section index of ParameterBlockDescriptor ( &HEF0000 )
        Friend Property OffsetDataOffsetArray As UInteger               'offset to (start of) offsets to values
        Friend Property OffsetDataBuffer As UInteger                       'Offset to (start of) values
        Public Property DataBuffer As Object() 'As ParameterValue()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace

Public Class Sampler2D
    Public Property PRaster As Rw.Graphics.Raster   'contains section index of the texture (RWGOBJECTTYPE_RASTER = &H20003), can be "00 40 00 00" : INDEX_NO_OBJECT
    Public Property Value_2 As UInteger
    Public Property Value_3 As UInteger
    Public Property Value_4 As UInteger
    Public Property Value_5 As UInteger
    Public Property Value_6 As UInteger
    Public Property Value_7 As UInteger
    Public Property Value_8 As UInteger

End Class