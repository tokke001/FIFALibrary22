Namespace Rx3
    Public Class QuadIndexBufferBatch
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.QUAD_INDEX_BUFFER_BATCH
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.NumQIndexBuffers = r.ReadUInt32
            Me.Unknown(0) = r.ReadUInt32
            Me.Unknown(1) = r.ReadUInt32
            Me.Unknown(2) = r.ReadUInt32

            Me.Rx3QIndexBufferHeaders = New QuadIndexBufferHeader(Me.NumQIndexBuffers - 1) {}
            For i = 0 To Me.NumQIndexBuffers - 1
                Me.Rx3QIndexBufferHeaders(i) = New QuadIndexBufferHeader(r)
            Next i

        End Sub

        Public Sub Save(ByVal Rx3QIndexBuffers As List(Of QuadIndexBuffer), ByVal w As FileWriter)
            Me.NumQIndexBuffers = Rx3QIndexBuffers.Count

            w.Write(Me.NumQIndexBuffers)
            w.Write(Me.Unknown(0))
            w.Write(Me.Unknown(1))
            w.Write(Me.Unknown(2))

            Me.Rx3QIndexBufferHeaders = New QuadIndexBufferHeader(Me.NumQIndexBuffers - 1) {}
            For i = 0 To Me.NumQIndexBuffers - 1
                Me.Rx3QIndexBufferHeaders(i) = New QuadIndexBufferHeader
                Me.Rx3QIndexBufferHeaders(i).Save(Rx3QIndexBuffers(i).Rx3QIndexBufferHeader, w)
            Next i

            FifaUtil.WriteAlignment(w, ALIGNMENT)
        End Sub


        ' Properties
        Public Property NumQIndexBuffers As UInteger
        Public Property Rx3QIndexBufferHeaders As QuadIndexBufferHeader()
        Public Property Unknown As UInteger() = New UInteger(3 - 1) {}

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace