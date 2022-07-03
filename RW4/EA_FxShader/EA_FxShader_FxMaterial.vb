Namespace Rw.EA.FxShader
    Public Class FxMaterial
        'EA::FxShader::FxMaterial
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.EA_FxShader_FxMaterial
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.OffsetName = r.ReadUInt32
            Me.EffectName = FifaUtil.Read64SizedString(r)
            Me.NumParameterBlocks = r.ReadUInt32

            Me.ParameterBlocks = New FxMaterialBlock(Me.NumParameterBlocks - 1) {}
            For i = 0 To Me.NumParameterBlocks - 1
                Me.ParameterBlocks(i) = New FxMaterialBlock
                Me.ParameterBlocks(i).OffsetName = r.ReadUInt32  'offsets
            Next

            Me.PParameterBlock = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), ParameterBlock)   'SectIndex_HEF0001ParameterBlock
            Me.Unknown_1 = r.ReadUInt16
            Me.Unknown_2 = r.ReadUInt16

            r.BaseStream.Position = Me.OffsetName
            Me.Name = FifaUtil.ReadNullTerminatedString(r)
            For i = 0 To Me.NumParameterBlocks - 1
                r.BaseStream.Position = Me.ParameterBlocks(i).OffsetName
                Me.ParameterBlocks(i).Name = FifaUtil.ReadNullTerminatedString(r) 'Names
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumParameterBlocks = Me.ParameterBlocks.Length

            Dim BaseOffset As Long = w.BaseStream.Position
            '-- write with wrong offsets
            w.Write(Me.OffsetName)
            FifaUtil.Write64SizedString(w, Me.EffectName)
            w.Write(Me.NumParameterBlocks)

            For i = 0 To Me.NumParameterBlocks - 1
                w.Write(Me.ParameterBlocks(i).OffsetName)
            Next

            w.Write(Me.RwArena.Sections.IndexOf(Me.PParameterBlock))
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)

            Me.OffsetName = w.BaseStream.Position
            FifaUtil.WriteNullTerminatedString(w, Me.Name)
            For i = 0 To Me.NumParameterBlocks - 1
                Me.ParameterBlocks(i).OffsetName = w.BaseStream.Position
                FifaUtil.WriteNullTerminatedString(w, Me.ParameterBlocks(i).Name)
            Next

            Dim EndOffset As Long = w.BaseStream.Position

            '-- write again with correct offsets
            w.BaseStream.Position = BaseOffset
            w.Write(Me.OffsetName)
            FifaUtil.Write64SizedString(w, Me.EffectName)
            w.Write(Me.NumParameterBlocks)
            For i = 0 To Me.NumParameterBlocks - 1
                w.Write(Me.ParameterBlocks(i).OffsetName)
            Next
            w.BaseStream.Position = EndOffset

        End Sub

        Public Property OffsetName As UInteger
        Public Property EffectName As String      '".FX" name, always 64 byte string
        Public Property NumParameterBlocks As UInteger    'guessed: usually 2
        Public Property PParameterBlock As ParameterBlock   'section index of EA_FxShader_ParameterBlock ( &HEF0001 )
        Public Property Unknown_1 As UShort     'usually "64" value (indicates size of String_1 ??)
        Public Property Unknown_2 As UShort     'usually "0"
        Public Property Name As String  ''adboarddigital', 'adboarddigitalglow', ...
        Public Property ParameterBlocks As FxMaterialBlock() 'always 2 names: always "default", then "dynamic"

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
    Public Class FxMaterialBlock
        Public Property OffsetName As UInteger
        Public Property Name As String
    End Class
End Namespace