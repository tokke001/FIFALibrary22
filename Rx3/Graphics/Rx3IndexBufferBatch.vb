Namespace Rx3
    Public Class IndexBufferBatch
        ' Methods
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.INDEX_BUFFER_BATCH
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.NumIndexbuffers = r.ReadUInt32
            Me.Unknown(0) = r.ReadUInt32                       'usually: = 0
            Me.Unknown(1) = r.ReadUInt32                       'usually: = 0
            Me.Unknown(2) = r.ReadUInt32                       'usually: = 0

            Me.Rx3IndexBufferHeaders = New IndexBufferHeader(Me.NumIndexbuffers - 1) {}
            For i = 0 To Me.NumIndexbuffers - 1
                Me.Rx3IndexBufferHeaders(i) = New IndexBufferHeader(r)
            Next i

        End Sub

        Public Sub Save(ByVal Rx3IndexBuffers As List(Of IndexBuffer), ByVal w As FileWriter)
            Me.NumIndexbuffers = Rx3IndexBuffers.Count

            w.Write(Me.NumIndexbuffers)
            w.Write(Me.Unknown(0))
            w.Write(Me.Unknown(1))
            w.Write(Me.Unknown(2))

            Me.Rx3IndexBufferHeaders = New IndexBufferHeader(Me.NumIndexbuffers - 1) {}
            For i = 0 To Me.NumIndexbuffers - 1
                Me.Rx3IndexBufferHeaders(i) = New IndexBufferHeader
                Me.Rx3IndexBufferHeaders(i).Save(Rx3IndexBuffers(i).Rx3IndexBufferHeader, w)
            Next i

            FifaUtil.WriteAlignment(w, ALIGNMENT)  'not needed

        End Sub


        ' Properties
        Public Property NumIndexbuffers As UInteger
        Public Property Rx3IndexBufferHeaders As IndexBufferHeader()
        Public Property Unknown As UInteger() = New UInteger(3 - 1) {}

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace