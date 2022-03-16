Namespace Rx3
    Partial Public Class VertexBuffer
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.VERTEX_BUFFER
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal VertexElements As VertexElement(), ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.m_VertexElements = VertexElements
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Me.NumVertices = r.ReadUInt32
            Me.VertexStride = r.ReadUInt32
            Me.VertexEndianness = r.ReadByte    '0 = Big endian, 1=little (FIFA 11: always big, little not supported !)
            Me.Padding = r.ReadBytes(3)

            Dim m_CurrentEndianness = r.Endianness                  'Get File Endian
            r.Endianness = GetFileEndianness(Me.VertexEndianness)   'Set Vertex Endian
            Me.VertexData = New Vertex(Me.NumVertices - 1) {}         'Loop vertices
            For i = 0 To Me.NumVertices - 1
                Me.VertexData(i) = New Vertex(Me.m_VertexElements, r)
            Next i
            r.Endianness = m_CurrentEndianness                      'Set File Endian

        End Sub

        Private Function GetFileEndianness(ByVal m_VertexEndianness As EVertexEndian) As Endian
            If m_VertexEndianness = EVertexEndian.Big_Endian Then
                Return Endian.Big
            End If

            Return Endian.Little
        End Function

        Public Sub Save(ByVal VertexElements As VertexElement(), ByVal w As FileWriter)
            Me.m_VertexElements = VertexElements

            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumVertices = Me.VertexData.Length
            'Me.VertexSize =   '--> calculate based on Me.VertexElements.DataTypes sizes

            w.Write(Me.TotalSize)
            w.Write(Me.NumVertices)
            w.Write(Me.VertexStride)
            w.Write(CByte(Me.VertexEndianness))
            w.Write(Me.Padding)

            Dim m_CurrentEndianness = w.Endianness                  'Get File Endian
            w.Endianness = GetFileEndianness(Me.VertexEndianness)   'Set Vertex Endian
            For i = 0 To Me.NumVertices - 1                         'Loop vertices
                Me.VertexData(i).Save(Me.m_VertexElements, w)
            Next i
            w.Endianness = m_CurrentEndianness                      'Set File Endian

            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub

        ' Properties
        Public Property NumVertices As UInteger
        Public Property VertexData As Vertex()
        Public Property VertexStride As UInteger
        Public Property TotalSize As UInteger
        Public Property VertexEndianness As EVertexEndian
        Public Property Padding As Byte() = New Byte(3 - 1) {}

        Private m_VertexElements As VertexElement()

        Public Enum EVertexEndian As Byte
            Big_Endian = 0
            Little_Endian = 1
        End Enum

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace