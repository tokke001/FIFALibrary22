Imports Microsoft.DirectX.Direct3D
Namespace Rx3
    Partial Public Class VertexBuffer

        Public Function SetVertex(ByVal newVertexes As CustomVertex.PositionNormalTextured()) As Boolean
            Me.VertexData.Clear()

            For i = 0 To Me.NumVertices - 1
                Me.VertexData.Add(New Vertex With {
                                  .Position = New Position With {
                                  .X = newVertexes(i).X,
                                  .Y = newVertexes(i).Y,
                                  .Z = newVertexes(i).Z
                                  },
                                  .TextureCoordinates = New TextureCoordinate() {
                                  New TextureCoordinate With {
                                  .U = newVertexes(i).Tu,
                                  .V = newVertexes(i).Tv}
                                  },
                                  .Normal = New Normal With {
                                  .Normal_x = newVertexes(i).Nx,
                                  .Normal_y = newVertexes(i).Ny,
                                  .Normal_z = newVertexes(i).Nz}
                })
            Next i
            Return True
        End Function


    End Class


End Namespace