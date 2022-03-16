Namespace Rx3
    Public Class Material
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.MATERIAL
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Me.NumTexMaps = r.ReadUInt32
            Me.Unknown(0) = r.ReadUInt32
            Me.Unknown(1) = r.ReadUInt32

            Me.ShaderName = FifaUtil.ReadNullTerminatedString(r)
            Me.TexMaps = New MaterialTexMap(Me.NumTexMaps - 1) {}
            For i = 0 To Me.NumTexMaps - 1
                Me.TexMaps(i) = New MaterialTexMap
                Me.TexMaps(i).TexTypeName = FifaUtil.ReadNullTerminatedString(r)
                Me.TexMaps(i).TexId = r.ReadUInt32
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumTexMaps = Me.TexMaps.Length


            w.Write(Me.TotalSize)
            w.Write(Me.NumTexMaps)
            w.Write(Me.Unknown(0))
            w.Write(Me.Unknown(1))

            FifaUtil.WriteNullTerminatedString(w, Me.ShaderName)
            For i = 0 To Me.NumTexMaps - 1
                FifaUtil.WriteNullTerminatedString(w, Me.TexMaps(i).TexTypeName)
                w.Write(Me.TexMaps(i).TexId)
            Next i


            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub


        Public Property TotalSize As UInteger
        Public Property NumTexMaps As UInteger
        Public Property Unknown As UInteger() = New UInteger(2 - 1) {}  'always 0? maybe padding
        Public Property ShaderName As String
        Public Property TexMaps As MaterialTexMap()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
    Public Class MaterialTexMap  'texture maps descriptors
        Public Property TexTypeName As String   '"diffuseTexture", "normalMap", ...
        Public Property TexId As UInteger       ' texture index in textrure names from model's names section
    End Class
End Namespace