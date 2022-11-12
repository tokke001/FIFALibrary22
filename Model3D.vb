Imports Microsoft.DirectX
Imports Microsoft.DirectX.Direct3D
Imports System.Drawing
Imports System.Drawing.Imaging
Imports Microsoft.DirectX.Direct3D.CustomVertex

Public Class Model3D
    ' Methods
    Public Sub New()
    End Sub

    Public Sub New(ByVal IndexStream As List(Of UInteger), ByVal VertexStream As List(Of Vertex), ByVal PrimitiveType As PrimitiveType)
        Me.Initialize(IndexStream, VertexStream, PrimitiveType)
    End Sub

    Public Sub New(ByVal indexArray As Rx3.IndexBuffer, ByVal vertexArray As Rx3.VertexBuffer, ByVal PrimitiveType As PrimitiveType)
        Me.Initialize(indexArray.IndexData, vertexArray.VertexData, PrimitiveType)
    End Sub

    Public Sub New(ByVal IndexStream As List(Of UInteger), ByVal VertexStream As List(Of Vertex), ByVal PrimitiveType As PrimitiveType, ByVal textureBitmap As Bitmap)
        Me.Initialize(IndexStream, VertexStream, PrimitiveType)
        Me.m_TextureBitmap = textureBitmap
    End Sub

    Public Sub New(ByVal indexArray As Rx3.IndexBuffer, ByVal vertexArray As Rx3.VertexBuffer, ByVal PrimitiveType As PrimitiveType, ByVal textureBitmap As Bitmap)
        Me.Initialize(indexArray.IndexData, vertexArray.VertexData, PrimitiveType)
        Me.m_TextureBitmap = textureBitmap
    End Sub

    Public Function CanMerge(ByVal model As Model3D) As Boolean
        If (Not model Is Nothing) Then
            If (Me.m_NOriginalVertex < model.NVertex) Then
                Return False
            End If
            If (Me.m_NOriginalIndex < model.NIndex) Then
                Return False
            End If
        End If
        Return True
    End Function

    Public Function CanMorphing(ByVal model2 As Model3D) As Boolean
        Return Model3D.CanMorphing(Me, model2)
    End Function

    Public Shared Function CanMorphing(ByVal model1 As Model3D, ByVal model2 As Model3D) As Boolean
        If ((model1 Is Nothing) OrElse (model2 Is Nothing)) Then
            Return False
        End If
        If (model1.NVertex <> model2.NVertex) Then
            Return False
        End If
        Return True
    End Function

    Public Function Clone() As Model3D
        Dim modeld1 As Model3D = DirectCast(MyBase.MemberwiseClone, Model3D)
        modeld1.m_Index = DirectCast(Me.m_Index.Clone, UShort())
        modeld1.m_IndexStream = DirectCast(Me.m_IndexStream.ToArray.Clone, List(Of UInteger))
        modeld1.m_Vertex = DirectCast(Me.m_Vertex.Clone, PositionNormalTextured())
        Return modeld1
    End Function

    Private Sub ComputeNormals()
        Dim vectorArray As Vector3() = New Vector3(Me.m_NVertex - 1) {}
        Dim numArray As Integer() = New Integer(Me.m_NVertex - 1) {}

        For i = 0 To (Me.m_Index.Length \ 3) - 1
            Dim index As Integer = Me.m_Index((i * 3))
            Dim num3 As Integer = Me.m_Index(((i * 3) + 1))
            Dim num4 As Integer = Me.m_Index(((i * 3) + 2))
            Dim right As New Vector3(Me.m_Vertex(index).X, Me.m_Vertex(index).Y, Me.m_Vertex(index).Z)
            Dim left As New Vector3(Me.m_Vertex(num3).X, Me.m_Vertex(num3).Y, Me.m_Vertex(num3).Z)
            Dim vector3 As Vector3 = Vector3.Subtract(left, right)
            Dim vector4 As Vector3 = Vector3.Normalize(Vector3.Cross(Vector3.Subtract(New Vector3(Me.m_Vertex(num4).X, Me.m_Vertex(num4).Y, Me.m_Vertex(num4).Z), right), vector3))
            vectorArray(index) = Vector3.Add(vectorArray(index), vector4)
            vectorArray(num3) = Vector3.Add(vectorArray(num3), vector4)
            vectorArray(num4) = Vector3.Add(vectorArray(num4), vector4)
            numArray(index) += 1
            numArray(num3) += 1
            numArray(num4) += 1
        Next i
        Dim j As Integer
        For j = 0 To Me.m_NVertex - 1
            Dim vector5 As Vector3 = Vector3.Scale(vectorArray(j), (1.0! / CSng(numArray(j))))
            Me.m_Vertex(j).Nx = vector5.X
            Me.m_Vertex(j).Ny = vector5.Y
            Me.m_Vertex(j).Nz = vector5.Z
        Next j
    End Sub

    Public Sub Initialize(ByVal IndexStream As List(Of UInteger), ByVal VertexStream As List(Of Vertex), ByVal PrimitiveType As PrimitiveType)
        Me.SetVertexStreams(VertexStream)
        Me.SetIndexArray(IndexStream, PrimitiveType)
        'Me.ComputeNormals()
    End Sub

    Public Sub MakeCloser()
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            If (Me.m_Vertex(i).X > 0!) Then
                Me.m_Vertex(i).X = (Me.m_Vertex(i).X - 0.1!)
            ElseIf (Me.m_Vertex(i).X < 0!) Then
                Me.m_Vertex(i).X = (Me.m_Vertex(i).X + 0.1!)
            End If
        Next i
    End Sub

    Public Sub MakeWider()
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            If (Me.m_Vertex(i).X > 0!) Then
                Me.m_Vertex(i).X = (Me.m_Vertex(i).X + 0.1!)
            ElseIf (Me.m_Vertex(i).X < 0!) Then
                Me.m_Vertex(i).X = (Me.m_Vertex(i).X - 0.1!)
            End If
        Next i
    End Sub

    Public Function Merge(ByVal model As Model3D) As Boolean
        If Not Me.CanMerge(model) Then
            Return False
        End If
        Dim nVertex As Integer = 0
        Dim nIndex As Integer = 0
        Dim nFaces As Integer = 0
        If (Not model Is Nothing) Then
            nVertex = model.NVertex
            nIndex = model.NIndex
            nFaces = model.NFaces
        End If
        Dim i As Integer
        For i = nVertex To Me.m_NVertex - 1
            Me.m_Vertex(i).X = 0!
            Me.m_Vertex(i).Y = 0!
            Me.m_Vertex(i).Z = 0!
            Me.m_Vertex(i).Nx = 0!
            Me.m_Vertex(i).Ny = 0!
            Me.m_Vertex(i).Nz = 0!
            Me.m_Vertex(i).Tu = 0!
            Me.m_Vertex(i).Tv = 0!
        Next i
        Me.m_NVertex = nVertex
        Dim j As Integer
        For j = 0 To Me.m_NVertex - 1
            Me.m_Vertex(j).X = model.Vertex(j).X
            Me.m_Vertex(j).Y = model.Vertex(j).Y
            Me.m_Vertex(j).Z = model.Vertex(j).Z
            Me.m_Vertex(j).Nx = model.Vertex(j).Nx
            Me.m_Vertex(j).Ny = model.Vertex(j).Ny
            Me.m_Vertex(j).Nz = model.Vertex(j).Nz
            Me.m_Vertex(j).Tu = model.Vertex(j).Tu
            Me.m_Vertex(j).Tv = model.Vertex(j).Tv
        Next j
        Dim k As Integer
        For k = nIndex To Me.m_NIndex - 1
            Me.m_IndexStream(k) = 0
        Next k
        Me.m_NIndex = nIndex
        Dim m As Integer
        For m = 0 To Me.m_NIndex - 1
            Me.m_IndexStream(m) = model.m_IndexStream(m)
        Next m
        If (nFaces < Me.m_NFaces) Then
            Dim num8 As Integer
            For num8 = (nFaces * 3) To (Me.m_NFaces * 3) - 1
                Me.m_Index(num8) = 0
            Next num8
        Else
            Array.Resize(Of UShort)(Me.m_Index, (nFaces * 3))
        End If
        Me.m_NFaces = nFaces
        Dim n As Integer
        For n = 0 To (Me.m_NFaces * 3) - 1
            Me.m_Index(n) = model.m_Index(n)
        Next n
        Return True
    End Function

    Public Function Morphing(ByVal model As Model3D, ByVal percent As Single) As Boolean
        If Not Me.CanMorphing(model) Then
            Return False
        End If
        Dim num As Single = (1.0! - percent)
        Me.m_NVertex = model.NVertex
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            Me.m_Vertex(i).X = ((Me.m_Vertex(i).X * num) + (model.Vertex(i).X * percent))
            Me.m_Vertex(i).Y = ((Me.m_Vertex(i).Y * num) + (model.Vertex(i).Y * percent))
            Me.m_Vertex(i).Z = ((Me.m_Vertex(i).Z * num) + (model.Vertex(i).Z * percent))
            Me.m_Vertex(i).Tu = ((Me.m_Vertex(i).Tu * num) + (model.Vertex(i).Tu * percent))
            Me.m_Vertex(i).Tv = ((Me.m_Vertex(i).Tv * num) + (model.Vertex(i).Tv * percent))
        Next i
        Return True
    End Function

    Public Function Morphing(ByVal model1 As Model3D, ByVal model2 As Model3D, ByVal percent As Single) As Boolean
        If Not Model3D.CanMorphing(model1, model2) Then
            Return False
        End If
        If Not Me.CanMorphing(model1) Then
            Return False
        End If
        Dim num As Single = (1.0! - percent)
        Dim nVertex As Integer = Me.m_NVertex
        If (model1.NVertex < nVertex) Then
            nVertex = model1.NVertex
        End If
        If (model2.NVertex < nVertex) Then
            nVertex = model2.NVertex
        End If
        Me.m_NVertex = nVertex
        Dim i As Integer
        For i = 0 To nVertex - 1
            Me.m_Vertex(i).X = ((model1.m_Vertex(i).X * num) + (model2.Vertex(i).X * percent))
            Me.m_Vertex(i).Y = ((model1.m_Vertex(i).Y * num) + (model2.Vertex(i).Y * percent))
            Me.m_Vertex(i).Z = ((model1.m_Vertex(i).Z * num) + (model2.Vertex(i).Z * percent))
            Me.m_Vertex(i).Tu = ((model1.m_Vertex(i).Tu * num) + (model2.Vertex(i).Tu * percent))
            Me.m_Vertex(i).Tv = ((model1.m_Vertex(i).Tv * num) + (model2.Vertex(i).Tv * percent))
        Next i
        Return True
    End Function

    Public Sub Morphing(ByVal points As Integer(), ByVal symmetryPoint As Vector3, ByVal delta As Vector3)
        Dim i As Integer
        For i = 0 To points.Length - 1
            Dim index As Integer = points(i)
            If ((index >= 0) AndAlso (index < Me.NVertex)) Then
                If (Me.Vertex(index).X < symmetryPoint.X) Then
                    Me.Vertex(index).X = (Me.Vertex(index).X - delta.X)
                Else
                    Me.Vertex(index).X = (Me.Vertex(index).X + delta.X)
                End If
                If (Me.Vertex(index).Y < symmetryPoint.Y) Then
                    Me.Vertex(index).Y = (Me.Vertex(index).Y - delta.Y)
                Else
                    Me.Vertex(index).Y = (Me.Vertex(index).Y + delta.Y)
                End If
                If (Me.Vertex(index).Z < symmetryPoint.Z) Then
                    Me.Vertex(index).Z = (Me.Vertex(index).Z - delta.Z)
                Else
                    Me.Vertex(index).Z = (Me.Vertex(index).Z + delta.Z)
                End If
            End If
        Next i
    End Sub

    Public Function MorphingThroughUV(ByVal model1 As Model3D, ByVal model2 As Model3D, ByVal percent As Single) As Boolean
        Return Me.MorphingThroughUV(model1, model2, percent, Nothing)
    End Function

    Public Function MorphingThroughUV(ByVal model1 As Model3D, ByVal model2 As Model3D, ByVal percent As Single, ByVal mask As Bitmap) As Boolean
        Dim maxValue As Single
        Dim num4 As Integer
        Dim num5 As Integer
        Dim num As Single = (1.0! - percent)
        Dim num2 As Single = percent
        If (((model1.NVertex >= 3157) AndAlso (model2.NVertex >= 3157)) AndAlso (Me.NVertex >= 3157)) Then
            Dim i As Integer
            For i = 0 To Me.NVertex - 1
                If (Not mask Is Nothing) Then
                    Dim x As Integer = CInt((Me.Vertex(i).Tu * mask.Width))
                    Dim y As Integer = CInt((Me.Vertex(i).Tv * mask.Height))
                    num2 = ((mask.GetPixel(x, y).R * percent) / 255.0!)
                    num = (1.0! - num2)
                End If
                maxValue = Single.MaxValue
                num4 = -1
                Dim j As Integer
                For j = 0 To model1.NVertex - 1
                    Dim num10 As Single = (((Me.Vertex(i).Tu - model1.Vertex(j).Tu) * (Me.Vertex(i).Tu - model1.Vertex(j).Tu)) + ((Me.Vertex(i).Tv - model1.Vertex(j).Tv) * (Me.Vertex(i).Tv - model1.Vertex(j).Tv)))
                    If (num10 < maxValue) Then
                        maxValue = num10
                        num4 = j
                    End If
                Next j
                maxValue = Single.MaxValue
                num5 = -1
                Dim k As Integer
                For k = 0 To model2.NVertex - 1
                    Dim num12 As Single = (((Me.Vertex(i).Tu - model2.Vertex(k).Tu) * (Me.Vertex(i).Tu - model2.Vertex(k).Tu)) + ((Me.Vertex(i).Tv - model2.Vertex(k).Tv) * (Me.Vertex(i).Tv - model2.Vertex(k).Tv)))
                    If (num12 < maxValue) Then
                        maxValue = num12
                        num5 = k
                    End If
                Next k
                If ((num4 <> -1) AndAlso (num5 <> -1)) Then
                    Me.Vertex(i).X = ((model1.Vertex(num4).X * num) + (model2.Vertex(num5).X * num2))
                    Me.Vertex(i).Y = ((model1.Vertex(num4).Y * num) + (model2.Vertex(num5).Y * num2))
                    Me.Vertex(i).Z = ((model1.Vertex(num4).Z * num) + (model2.Vertex(num5).Z * num2))
                End If
            Next i
        Else
            If (((model1.NVertex <> 132) OrElse (model2.NVertex <> 132)) OrElse (Me.NVertex <> 132)) Then
                Return False
            End If
            Dim i As Integer
            For i = 0 To Me.NVertex - 1
                maxValue = Single.MaxValue
                num4 = -1
                Dim j As Integer
                For j = 0 To model1.NVertex - 1
                    If ((Me.Vertex(i).X * model1.Vertex(j).X) > 0!) Then
                        Dim num15 As Single = (((Me.Vertex(i).Tu - model1.Vertex(j).Tu) * (Me.Vertex(i).Tu - model1.Vertex(j).Tu)) + ((Me.Vertex(i).Tv - model1.Vertex(j).Tv) * (Me.Vertex(i).Tv - model1.Vertex(j).Tv)))
                        If (num15 < maxValue) Then
                            maxValue = num15
                            num4 = j
                        End If
                    End If
                Next j
                maxValue = Single.MaxValue
                num5 = -1
                Dim k As Integer
                For k = 0 To model2.NVertex - 1
                    If ((Me.Vertex(i).X * model2.Vertex(k).X) > 0!) Then
                        Dim num17 As Single = (((Me.Vertex(i).Tu - model2.Vertex(k).Tu) * (Me.Vertex(i).Tu - model2.Vertex(k).Tu)) + ((Me.Vertex(i).Tv - model2.Vertex(k).Tv) * (Me.Vertex(i).Tv - model2.Vertex(k).Tv)))
                        If (num17 < maxValue) Then
                            maxValue = num17
                            num5 = k
                        End If
                    End If
                Next k
                If ((num4 <> -1) AndAlso (num5 <> -1)) Then
                    Me.Vertex(i).X = ((model1.Vertex(num4).X * num) + (model2.Vertex(num5).X * num2))
                    Me.Vertex(i).Y = ((model1.Vertex(num4).Y * num) + (model2.Vertex(num5).Y * num2))
                    Me.Vertex(i).Z = ((model1.Vertex(num4).Z * num) + (model2.Vertex(num5).Z * num2))
                End If
            Next i
        End If
        Return True
    End Function

    Public Sub MoveBack()
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            Me.m_Vertex(i).Z = (Me.m_Vertex(i).Z - 0.1!)
        Next i
    End Sub

    Public Sub MoveDown()
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            Me.m_Vertex(i).Y = (Me.m_Vertex(i).Y - 0.1!)
        Next i
    End Sub

    Public Sub MoveForward()
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            Me.m_Vertex(i).Z = (Me.m_Vertex(i).Z + 0.1!)
        Next i
    End Sub

    Public Sub MoveLeft()
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            Me.m_Vertex(i).X = (Me.m_Vertex(i).X + 0.1!)
        Next i
    End Sub

    Public Sub MoveRight()
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            Me.m_Vertex(i).X = (Me.m_Vertex(i).X - 0.1!)
        Next i
    End Sub

    Public Sub MoveUp()
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            Me.m_Vertex(i).Y = (Me.m_Vertex(i).Y + 0.1!)
        Next i
    End Sub

    Public Sub OffsetVertex(ByVal offsetX As Integer, ByVal offsetY As Integer, ByVal offsetZ As Integer)
        If ((Not Me.m_Vertex Is Nothing) AndAlso (Me.m_Vertex.Length >= Me.m_NOriginalVertex)) Then
            Dim i As Integer
            For i = 0 To Me.m_NOriginalVertex - 1
                Me.m_Vertex(i).X = (Me.m_Vertex(i).X - offsetX)
                Me.m_Vertex(i).Y = (Me.m_Vertex(i).Y - offsetY)
                Me.m_Vertex(i).Z = (Me.m_Vertex(i).Z - offsetZ)
            Next i
        End If
    End Sub

    Public Sub SaveXFile(ByVal w As StreamWriter)
        If (Me.m_NVertex > 0) Then
            w.WriteLine("xof 0303txt 0032")
            w.WriteLine("Mesh {")
            w.WriteLine(" {0};", Me.m_NVertex)
            Dim i As Integer
            For i = 0 To (Me.m_NVertex - 1) - 1
                w.WriteLine(" {0:F6};{1:F6};{2:F6};,", Me.m_Vertex(i).X, Me.m_Vertex(i).Y, Me.m_Vertex(i).Z)
            Next i
            w.WriteLine(" {0:F6};{1:F6};{2:F6};;", Me.m_Vertex((Me.m_NVertex - 1)).X, Me.m_Vertex((Me.m_NVertex - 1)).Y, Me.m_Vertex((Me.m_NVertex - 1)).Z)
            w.WriteLine(" {0};", Me.m_NFaces)
            Dim j As Integer
            For j = 0 To (Me.m_NFaces - 1) - 1
                w.WriteLine(" 3;{0},{1},{2};,", Me.m_Index(((j * 3) + 0)), Me.m_Index(((j * 3) + 1)), Me.m_Index(((j * 3) + 2)))
            Next j
            w.WriteLine(" 3;{0},{1},{2};;", Me.m_Index((((Me.m_NFaces - 1) * 3) + 0)), Me.m_Index((((Me.m_NFaces - 1) * 3) + 1)), Me.m_Index((((Me.m_NFaces - 1) * 3) + 2)))
            w.WriteLine(" MeshNormals {")
            w.WriteLine("  {0};", Me.m_NVertex)
            Dim k As Integer
            For k = 0 To (Me.m_NVertex - 1) - 1
                w.WriteLine("  {0:F6};{1:F6};{2:F6};,", Me.m_Vertex(k).Nx, Me.m_Vertex(k).Ny, Me.m_Vertex(k).Nz)
            Next k
            w.WriteLine("  {0:F6};{1:F6};{2:F6};;", Me.m_Vertex((Me.m_NVertex - 1)).Nx, Me.m_Vertex((Me.m_NVertex - 1)).Ny, Me.m_Vertex((Me.m_NVertex - 1)).Nz)
            w.WriteLine(" {0};", Me.m_NFaces)
            Dim m As Integer
            For m = 0 To (Me.m_NFaces - 1) - 1
                w.WriteLine("  3;{0},{1},{2};,", Me.m_Index(((m * 3) + 0)), Me.m_Index(((m * 3) + 1)), Me.m_Index(((m * 3) + 2)))
            Next m
            w.WriteLine("  3;{0},{1},{2};;", Me.m_Index((((Me.m_NFaces - 1) * 3) + 0)), Me.m_Index((((Me.m_NFaces - 1) * 3) + 1)), Me.m_Index((((Me.m_NFaces - 1) * 3) + 2)))
            w.WriteLine(" }")
            w.WriteLine(" MeshTextureCoords {")
            w.WriteLine("  {0};", Me.m_NVertex)
            Dim n As Integer
            For n = 0 To (Me.m_NVertex - 1) - 1
                w.WriteLine("  {0:F6};{1:F6};,", Me.m_Vertex(n).Tu, Me.m_Vertex(n).Tv)
            Next n
            w.WriteLine("  {0:F6};{1:F6};;", Me.m_Vertex((Me.m_NVertex - 1)).Tu, Me.m_Vertex((Me.m_NVertex - 1)).Tv)
            w.WriteLine(" }")
            w.WriteLine(" MeshMaterialList {")
            w.WriteLine("  1;")
            w.WriteLine("  {0};", Me.m_NFaces)
            Dim num6 As Integer
            For num6 = 0 To (Me.m_NFaces - 1) - 1
                w.WriteLine("  0,")
            Next num6
            w.WriteLine("  0;")
            w.WriteLine("  Material {")
            w.WriteLine("   1.000000;1.000000;1.000000;0.000000;;")
            w.WriteLine("   0.000000;")
            w.WriteLine("   0.000000;0.000000;0.000000;;")
            w.WriteLine("   0.000000;0.000000;0.000000;;")
            w.WriteLine("   TextureFilename {")
            w.WriteLine(("   """ & Me.m_TextureFileName & """;"))
            w.WriteLine("   }")
            w.WriteLine("  }")
            w.WriteLine(" }")
            w.WriteLine("}")
            Me.m_TextureBitmap.Save(Me.m_TextureFileName, ImageFormat.Png)
        End If
    End Sub

    Public Sub SaveXFile(ByVal fileName As String)
        Dim stream As New FileStream(fileName, FileMode.Create, FileAccess.Write)
        Dim w As New StreamWriter(stream)
        Me.SaveXFile(w)
        w.Close()
        stream.Close()
    End Sub

    Private Sub SetIndexArray(ByVal IndexStream As List(Of UInteger), ByVal PrimitiveType As PrimitiveType)
        Me.m_NIndex = IndexStream.Count
        Me.m_NOriginalIndex = Me.m_NIndex
        Me.m_IndexStream = IndexStream
        Me.m_NFaces = GetNumFaces(IndexStream, PrimitiveType) 'indexArray.NumFaces
        'Me.m_IsTriangleList = indexArray.IsTriangleList
        Me.m_PrimitiveType = PrimitiveType

        'Select Case indexArray.PrimitiveType
        'Case PrimitiveType.TriangleList
        'Me.m_IsTriangleList = True
        'Case Else
        'Me.m_IsTriangleList = False
        'End Select


        If Me.m_PrimitiveType = PrimitiveType.TriangleList Then 'Not Me.m_IsTriangleList Then

            Me.m_Index = New UShort(Me.m_NIndex - 1) {}
            For i = 0 To Me.m_NIndex - 1
                Me.m_Index(i) = Me.m_IndexStream(i)
            Next i
            'Dim i As Integer = 0
            'Do While (i < Me.m_NIndex)
            'Me.m_Index(i) = Me.m_IndexStream(i)
            'Me.m_Index((i + 1)) = Me.m_IndexStream((i + 1))
            'Me.m_Index((i + 2)) = Me.m_IndexStream((i + 2))
            'i = (i + 3)
            'Loop
        ElseIf Me.m_PrimitiveType = PrimitiveType.TriangleFan Then

            Me.m_Index = New UShort((Me.m_NFaces * 3) - 1) {}
            Dim num2 As Integer = 0
            Dim num3 As Integer = 0 'If((Rx3IndexArray.TriangleListType = ETriangleListType.InvertOdd), 1, 0)
            Dim index As Integer = 0
            Do While True
                If (index < (Me.m_NIndex - 2)) Then
                    Dim num5 As Short = Me.m_IndexStream(index)
                    Dim num6 As Short = Me.m_IndexStream((index + 1))
                    Dim num7 As Short = Me.m_IndexStream((index + 2))
                    If ((num5 <= Me.m_NVertex) AndAlso ((num6 <= Me.m_NVertex) AndAlso ((num7 <= Me.m_NVertex) AndAlso ((num5 >= 0) AndAlso ((num6 >= 0) AndAlso (num7 >= 0)))))) Then
                        If ((num5 <> num6) AndAlso ((num6 <> num7) AndAlso (num5 <> num7))) Then
                            If ((index And 1) = num3) Then
                                Me.m_Index(num2) = num5
                                num2 += 1
                                Me.m_Index(num2) = num6
                                num2 += 1
                                Me.m_Index(num2) = num7
                                num2 += 1
                            Else
                                Me.m_Index(num2) = num5
                                num2 += 1
                                Me.m_Index(num2) = num7
                                num2 += 1
                                Me.m_Index(num2) = num6
                                num2 += 1
                            End If
                        End If
                        index += 1
                        Continue Do
                    End If
                End If
                Me.m_NIndex = (Me.m_NFaces * 3)
                Return
            Loop

        Else

            Me.m_Index = New UShort(Me.m_NIndex - 1) {}
            For i = 0 To Me.m_NIndex - 1
                Me.m_Index(i) = Me.m_IndexStream(i)
            Next i

        End If
    End Sub

    Private Sub SetVertexStreams(ByVal VertexStream As List(Of Vertex))
        Me.m_NVertex = VertexStream.Count
        Me.m_NOriginalVertex = Me.m_NVertex
        Me.m_Vertex = New PositionNormalTextured(Me.m_NVertex - 1) {}
        Dim i As Integer
        For i = 0 To Me.m_NVertex - 1
            Me.m_Vertex(i).X = VertexStream(i).Position.X
            Me.m_Vertex(i).Y = VertexStream(i).Position.Y
            Me.m_Vertex(i).Z = VertexStream(i).Position.Z
            Me.m_Vertex(i).Tu = VertexStream(i).TextureCoordinates(0).U
            Me.m_Vertex(i).Tv = VertexStream(i).TextureCoordinates(0).V
            If VertexStream(i).Normal IsNot Nothing Then
                Me.m_Vertex(i).Nx = VertexStream(i).Normal.Normal_x
                Me.m_Vertex(i).Ny = VertexStream(i).Normal.Normal_y
                Me.m_Vertex(i).Nz = VertexStream(i).Normal.Normal_z
            End If
        Next i
    End Sub

    Public Function GetNumFaces(IndexStream As List(Of UInteger), ByVal m_PrimitiveType As PrimitiveType) As Integer
        Dim NumFaces As Integer = 0

        'https://docs.microsoft.com/en-us/windows/win32/direct3d9/d3dprimitivetype
        If m_PrimitiveType = PrimitiveType.TriangleList Then
            NumFaces = (IndexStream.Count \ 3)

        ElseIf m_PrimitiveType = PrimitiveType.TriangleFan Then
            NumFaces = 0
            For i = 0 To (IndexStream.Count - 2) - 1
                Dim num8 As Short = IndexStream(i)
                Dim num9 As Short = IndexStream((i + 1))
                Dim num10 As Short = IndexStream((i + 2))
                If ((num8 <> num9) AndAlso ((num9 <> num10) AndAlso (num8 <> num10))) Then
                    NumFaces += 1
                End If
            Next i

        End If

        Return NumFaces
    End Function

    ' Properties
    Public Property TextureFileName As String
        Get
            Return Me.m_TextureFileName
        End Get
        Set(ByVal value As String)
            Me.m_TextureFileName = value
        End Set
    End Property

    Public Property TextureBitmap As Bitmap
        Get
            Return Me.m_TextureBitmap
        End Get
        Set(ByVal value As Bitmap)
            Me.m_TextureBitmap = value
        End Set
    End Property

    Public ReadOnly Property NVertex As Integer
        Get
            Return Me.m_NVertex
        End Get
    End Property

    Public ReadOnly Property NIndex As Integer
        Get
            Return Me.m_NIndex
        End Get
    End Property

    Public ReadOnly Property Vertex As PositionNormalTextured()
        Get
            Return Me.m_Vertex
        End Get
    End Property

    Public ReadOnly Property Index As UShort()
        Get
            Return Me.m_Index
        End Get
    End Property

    Public ReadOnly Property NFaces As Integer
        Get
            Return Me.m_NFaces
        End Get
    End Property


    ' Fields
    Private m_TextureFileName As String
    Private m_TextureBitmap As Bitmap
    Private m_NVertex As Integer
    Private m_NOriginalVertex As Integer
    Private m_NIndex As Integer
    Private m_NOriginalIndex As Integer
    'Private m_IsTriangleList As Boolean
    Private m_Vertex As PositionNormalTextured()
    Private m_Index As UShort()
    Private m_IndexStream As List(Of UInteger) 'UShort()
    Private m_NFaces As Integer

    Private m_PrimitiveType As PrimitiveType

End Class

