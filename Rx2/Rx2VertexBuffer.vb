Namespace Rx2
    Partial Public Class VertexBuffer
        ' Methods

        Public Sub New()

        End Sub

        Public Sub New(ByVal VertexElements As VertexElement(), ByVal NumVertices As Long, ByVal r As FileReader)
            Me.m_VertexElements = VertexElements
            Me.m_NumVertices = NumVertices
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.VertexData = New Vertex(Me.m_NumVertices - 1) {}
            For i = 0 To Me.m_NumVertices - 1
                Me.VertexData(i) = New Vertex(Me.m_VertexElements, r)
            Next i

        End Sub


        Public Sub Save(ByVal VertexElements As VertexElement(), ByVal w As FileWriter)
            Me.m_VertexElements = VertexElements
            Me.m_NumVertices = Me.VertexData.Length

            For i = 0 To Me.m_NumVertices - 1
                Me.VertexData(i).Save(Me.m_VertexElements, w)
            Next i

        End Sub

        ' Properties

        Public Property VertexData As Vertex()

        Private m_VertexElements As VertexElement()
        Private m_NumVertices As Long

    End Class

End Namespace