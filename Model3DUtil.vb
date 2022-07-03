
Imports Microsoft.DirectX.Direct3D

Public Class Model3DUtil
    Public Shared Function GetListFromIndices(ByVal indexArray As Rx3.IndexBuffer, ByVal NumVertices As UInteger, ByVal m_PrimitiveType As PrimitiveType) As UInteger()
        Dim m_NIndex As UInteger = indexArray.Header.NumIndices
        'Dim m_NOriginalIndex As UInteger = m_NIndex
        Dim m_Index As UInteger()
        Dim m_IndexStream As List(Of UInteger) = indexArray.IndexData
        Dim m_NFaces As Integer = indexArray.GetNumFaces(m_PrimitiveType) 'indexArray.NumFaces


        If m_PrimitiveType = Microsoft.DirectX.Direct3D.PrimitiveType.TriangleList Then

            m_Index = New UInteger(m_NIndex - 1) {}
            'For i = 0 To m_NIndex - 1
            'm_Index(i) = m_IndexStream(i)
            'Next i
            Dim i As Integer = 0
            Do While (i < m_NIndex)
                m_Index(i) = m_IndexStream(i)
                m_Index((i + 1)) = m_IndexStream((i + 1))
                m_Index((i + 2)) = m_IndexStream((i + 2))
                i += 3
            Loop
        ElseIf m_PrimitiveType = Microsoft.DirectX.Direct3D.PrimitiveType.TriangleFan Then

            m_Index = New UInteger((m_NFaces * 3) - 1) {}
            Dim num2 As Integer = 0
            Dim num3 As Integer = 0 'If((Rx3IndexArray.TriangleListType = ETriangleListType.InvertOdd), 1, 0)
            Dim index As Integer = 0
            Do While True
                If (index < (m_NIndex - 2)) Then
                    Dim num5 As Short = m_IndexStream(index)
                    Dim num6 As Short = m_IndexStream((index + 1))
                    Dim num7 As Short = m_IndexStream((index + 2))
                    If ((num5 <= NumVertices) AndAlso ((num6 <= NumVertices) AndAlso ((num7 <= NumVertices) AndAlso ((num5 >= 0) AndAlso ((num6 >= 0) AndAlso (num7 >= 0)))))) Then
                        If ((num5 <> num6) AndAlso ((num6 <> num7) AndAlso (num5 <> num7))) Then
                            If ((index And 1) = num3) Then
                                m_Index(num2) = num5
                                num2 += 1
                                m_Index(num2) = num6
                                num2 += 1
                                m_Index(num2) = num7
                                num2 += 1
                            Else
                                m_Index(num2) = num5
                                num2 += 1
                                m_Index(num2) = num7
                                num2 += 1
                                m_Index(num2) = num6
                                num2 += 1
                            End If
                        End If
                        index += 1
                        Continue Do
                    End If
                End If
                'm_NIndex = (m_NFaces * 3)
                'Return m_Index
            Loop

        Else

            m_Index = New UInteger(m_NIndex - 1) {}
            For i = 0 To m_NIndex - 1
                m_Index(i) = m_IndexStream(i)
            Next i

        End If

        Return m_Index
    End Function

    Public Shared Function GetIndicesFromList(ByVal FaceList As UInteger(), ByVal m_PrimitiveType As PrimitiveType) As UInteger()
        Dim m_IndexStream As UInteger()  '= indexArray.IndexStream
        Dim m_NIndex As UInteger = FaceList.Length


        If m_PrimitiveType = Microsoft.DirectX.Direct3D.PrimitiveType.TriangleList Then

            m_IndexStream = New UInteger(m_NIndex - 1) {}
            'For i = 0 To m_NIndex - 1
            'm_IndexStream(i) = FaceList(i)
            'Next i
            Dim i As Integer = 0
            Do While (i < m_NIndex)
                m_IndexStream(i) = FaceList(i)
                m_IndexStream((i + 1)) = FaceList((i + 1))
                m_IndexStream((i + 2)) = FaceList((i + 2))
                i += 3
            Loop

            'ElseIf m_PrimitiveType = Microsoft.DirectX.Direct3D.PrimitiveType.TriangleFan Then

            'no idea :/
        Else

            m_IndexStream = New UInteger(m_NIndex - 1) {}
            For i = 0 To m_NIndex - 1
                m_IndexStream(i) = FaceList(i)
            Next i

        End If

        Return m_IndexStream
    End Function

    'Public Shared Function GetNumFaces(ByVal m_Rx3IndexBuffer As Rx3IndexBuffer, ByVal m_PrimitiveType As PrimitiveType) As Integer
    'Return m_Rx3IndexBuffer.GetNumFaces(m_PrimitiveType)
    'End Function
End Class

