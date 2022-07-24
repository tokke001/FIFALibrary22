Namespace Rx3
    Public Class SceneLayer
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.SCENE_LAYER
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
            Dim m_NumInstances As UInteger = r.ReadUInt32
            Me.Pad = r.ReadBytes(8)

            Me.Name = FifaUtil.ReadNullTerminatedString(r)
            Me.Instances = r.ReadUIntegers(m_NumInstances)

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.TotalSize)
            w.Write(CUInt(Me.NumInstances))
            w.Write(Me.Pad)

            FifaUtil.WriteNullTerminatedString(w, Me.Name)
            w.Write(Me.Instances)

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
        ''' Returns the number of Instances (ReadOnly). </summary>
        Public ReadOnly Property NumInstances As UInteger
            Get
                Return If(Instances?.Count, 0)
            End Get
        End Property
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Pad As Byte() = New Byte(8 - 1) {}  'always 0? maybe padding
        ''' <summary>
        ''' Name of SceneLayer. </summary>
        Public Property Name As String
        ''' <summary>
        ''' Indexes of the Rx3.SceneInstance object this name corresponds to. </summary>
        Public Property Instances As UInteger()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace