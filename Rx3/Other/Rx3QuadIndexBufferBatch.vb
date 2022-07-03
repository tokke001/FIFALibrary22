Namespace Rx3
    Public Class QuadIndexBufferBatch
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.QUAD_INDEX_BUFFER_BATCH
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Dim m_NumQIndexBuffers = r.ReadUInt32
            Me.Unknown(0) = r.ReadUInt32
            Me.Unknown(1) = r.ReadUInt32
            Me.Unknown(2) = r.ReadUInt32

            Me.QIndexBufferHeaders = New QuadIndexBufferHeader(m_NumQIndexBuffers - 1) {}
            For i = 0 To m_NumQIndexBuffers - 1
                Me.QIndexBufferHeaders(i) = New QuadIndexBufferHeader(r, Nothing)
            Next i

        End Sub

        Public Sub Save(ByVal Rx3QIndexBuffers As List(Of QuadIndexBuffer), ByVal w As FileWriter)
            w.Write(If(Rx3QIndexBuffers?.Count, 0))
            w.Write(Me.Unknown(0))
            w.Write(Me.Unknown(1))
            w.Write(Me.Unknown(2))

            Me.QIndexBufferHeaders = New QuadIndexBufferHeader(Rx3QIndexBuffers.Count - 1) {}
            For i = 0 To Rx3QIndexBuffers.Count - 1
                Me.QIndexBufferHeaders(i) = Rx3QIndexBuffers(i).Header
                Me.QIndexBufferHeaders(i).Save(w)
            Next i

            FifaUtil.WriteAlignment(w, ALIGNMENT)
        End Sub


        ' Properties
        ''' <summary>
        ''' Number of QuadIndexBuffer-Headers (ReadOnly). </summary>
        Public ReadOnly Property NumQIndexBuffers As UInteger
            Get
                Return If(QIndexBufferHeaders?.Count, 0)
            End Get
        End Property
        Public Property QIndexBufferHeaders As QuadIndexBufferHeader()
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Unknown As UInteger() = New UInteger(3 - 1) {}

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace