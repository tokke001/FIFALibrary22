Namespace Rx3
    Public Class QuadIndexBufferHeader
        ' Methods
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Me.NumIndices = r.ReadUInt32
            Me.IndexSize = r.ReadByte
            Me.Padding = r.ReadBytes(7)

        End Sub

        Public Sub Save(ByVal Rx3QIndexBufferHeader As QuadIndexBufferHeader, ByVal w As FileWriter)
            Me.TotalSize = Rx3QIndexBufferHeader.TotalSize     '(Rx3IndexBuffer.NumIndices * IndexSize) + 16 + paddingsize
            Me.NumIndices = Rx3QIndexBufferHeader.NumIndices
            Me.IndexSize = Rx3QIndexBufferHeader.IndexSize
            Me.Padding = Rx3QIndexBufferHeader.Padding

            w.Write(Me.TotalSize)
            w.Write(Me.NumIndices)
            w.Write(Me.IndexSize)
            w.Write(Me.Padding)

        End Sub


        ' Fields
        Public Property TotalSize As UInteger
        Public Property NumIndices As UInteger
        Public Property IndexSize As Byte
        Public Property Padding As Byte() = New Byte(7 - 1) {}

    End Class
End Namespace