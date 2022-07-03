Namespace Rx3
    Public Class Material
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.MATERIAL
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Dim NumTexMaps As UInteger = r.ReadUInt32
            Me.Unknown(0) = r.ReadUInt32
            Me.Unknown(1) = r.ReadUInt32

            Me.ShaderName = FifaUtil.ReadNullTerminatedString(r)
            Me.TexMaps = New MaterialTexMap(NumTexMaps - 1) {}
            For i = 0 To NumTexMaps - 1
                Me.TexMaps(i) = New MaterialTexMap
                Me.TexMaps(i).TexTypeName = FifaUtil.ReadNullTerminatedString(r)
                Me.TexMaps(i).TexId = r.ReadUInt32
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
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
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub


        Private m_TotalSize As UInteger

        ''' <summary>
        ''' Total section size (ReadOnly). </summary>
        Public Property TotalSize As UInteger
            Get
                Return m_TotalSize
            End Get
            Private Set
                m_TotalSize = Value
            End Set
        End Property
        ''' <summary>
        ''' Returns the number of TexMaps (texture maps descriptors) (ReadOnly). </summary>
        Public ReadOnly Property NumTexMaps As UInteger
            Get
                Return If(TexMaps?.Count, 0)
            End Get
        End Property
        Public Property Unknown As UInteger() = New UInteger(2 - 1) {}  'always 0? maybe padding
        ''' <summary>
        ''' Name of the Shader. </summary>
        Public Property ShaderName As String
        ''' <summary>
        ''' Gets/Sets the texture maps descriptors. </summary>
        Public Property TexMaps As MaterialTexMap()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
    Public Class MaterialTexMap  'texture maps descriptors
        ''' <summary>
        ''' Name of the Texture type. </summary>
        Public Property TexTypeName As String   '"diffuseTexture", "normalMap", ...
        ''' <summary>
        ''' Texture index in names list of the model. </summary>
        Public Property TexId As UInteger       ' texture index in textrure names from model's names section
    End Class
End Namespace