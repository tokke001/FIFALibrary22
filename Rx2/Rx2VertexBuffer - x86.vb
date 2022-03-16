Imports Microsoft.DirectX.Direct3D
Namespace Rx2
    Partial Public Class VertexBuffer

        Public Function SetVertex(ByVal newVertexes As CustomVertex.PositionNormalTextured()) As Boolean
            Me.m_NumVertices = newVertexes.Length

            Me.VertexData = New Vertex(Me.m_NumVertices - 1) {}
            For i = 0 To Me.m_NumVertices - 1
                Me.VertexData(i) = New Vertex

                Me.VertexData(i).Positions(0).X = newVertexes(i).X
                Me.VertexData(i).Positions(0).Y = newVertexes(i).Y
                Me.VertexData(i).Positions(0).Z = newVertexes(i).Z

                Me.VertexData(i).TextureCoordinates(0).U = newVertexes(i).Tu
                Me.VertexData(i).TextureCoordinates(0).V = newVertexes(i).Tv

                Me.VertexData(i).Normals(0).Normal_x = newVertexes(i).Nx
                Me.VertexData(i).Normals(0).Normal_y = newVertexes(i).Ny
                Me.VertexData(i).Normals(0).Normal_z = newVertexes(i).Nz

            Next i
            Return True
        End Function

    End Class

End Namespace