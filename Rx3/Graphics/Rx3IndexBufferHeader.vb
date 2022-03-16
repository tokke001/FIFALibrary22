Namespace Rx3
    Public Class IndexBufferHeader
        ' Methods
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Me.NumIndices = r.ReadUInt32
            Me.IndexStride = r.ReadByte
            Me.Padding = r.ReadBytes(7)

        End Sub

        Public Sub Save(ByVal Rx3IndexBufferHeader As IndexBufferHeader, ByVal w As FileWriter)
            Me.TotalSize = Rx3IndexBufferHeader.TotalSize     '(Rx3IndexBuffer.NumIndices * IndexSize) + 16 + paddingsize
            Me.NumIndices = Rx3IndexBufferHeader.NumIndices
            Me.IndexStride = Rx3IndexBufferHeader.IndexStride
            Me.Padding = Rx3IndexBufferHeader.Padding

            w.Write(Me.TotalSize)
            w.Write(Me.NumIndices)
            w.Write(Me.IndexStride)
            w.Write(Me.Padding)

        End Sub

        ' Fields
        Public Property TotalSize As UInteger
        Public Property NumIndices As UInteger
        Public Property IndexStride As Byte
        Public Property Padding As Byte() = New Byte(7 - 1) {}

    End Class

End Namespace