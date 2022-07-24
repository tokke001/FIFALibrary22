Namespace Rx3
    Public Class IndexBufferBatch
        ' Methods
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.INDEX_BUFFER_BATCH
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Dim m_NumIndexbuffers = r.ReadUInt32
            Me.Pad = r.ReadBytes(12)

            Me.IndexBufferHeaders = New IndexBufferHeader(m_NumIndexbuffers - 1) {}
            For i = 0 To m_NumIndexbuffers - 1
                Me.IndexBufferHeaders(i) = New IndexBufferHeader(r, Nothing)
            Next i

        End Sub

        Public Sub Save(ByVal Rx3IndexBuffers As List(Of IndexBuffer), ByVal w As FileWriter)
            w.Write(If(Rx3IndexBuffers?.Count, 0))
            w.Write(Me.Pad)

            Me.IndexBufferHeaders = New IndexBufferHeader(Rx3IndexBuffers.Count - 1) {}
            For i = 0 To Rx3IndexBuffers.Count - 1
                Me.IndexBufferHeaders(i) = Rx3IndexBuffers(i).Header
                Me.IndexBufferHeaders(i).Save(w)
            Next i

            FifaUtil.WriteAlignment(w, ALIGNMENT)  'not needed
        End Sub


        ' Properties
        ''' <summary>
        ''' Number of IndexBuffer-Headers (ReadOnly). </summary>
        Public ReadOnly Property NumIndexbuffers As UInteger
            Get
                Return If(IndexBufferHeaders?.Count, 0)
            End Get
        End Property

        Public Property IndexBufferHeaders As IndexBufferHeader()
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Pad As Byte() = New Byte(12 - 1) {}   ' padding (0)

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace