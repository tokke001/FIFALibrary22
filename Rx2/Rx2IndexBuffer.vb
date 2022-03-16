Imports Microsoft.DirectX.Direct3D
Namespace Rx2
    Public Class IndexBuffer
        Public Sub New()

        End Sub

        Public Sub New(ByVal RWIndexBuffer As Rw.Graphics.IndexBuffer, ByVal r As FileReader)  'ByVal NumIndices As Long,
            Me.m_NumIndices = RWIndexBuffer.NumIndices
            Me.m_IndexSize = RWIndexBuffer.IndexStride
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.IndexData = New UInteger(Me.m_NumIndices - 1) {}
            For i = 0 To Me.m_NumIndices - 1
                If Me.m_IndexSize = 2 Then      'RWIndexFormat.FMT_INDEX16
                    Me.IndexData(i) = r.ReadUInt16
                ElseIf Me.m_IndexSize = 4 Then  'RWIndexFormat.FMT_INDEX32
                    Me.IndexData(i) = r.ReadUInt32
                End If
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Me.m_NumIndices = Me.IndexData.Length

            If Me.m_NumIndices > UShort.MaxValue Then
                Me.m_IndexSize = 4  'RWIndexFormat.FMT_INDEX32
            Else
                Me.m_IndexSize = 2  'RWIndexFormat.FMT_INDEX16
            End If

            For i = 0 To Me.m_NumIndices - 1
                If Me.m_IndexSize = 2 Then      'RWIndexFormat.FMT_INDEX16
                    w.Write(CUShort(Me.IndexData(i)))
                ElseIf Me.m_IndexSize = 4 Then  'RWIndexFormat.FMT_INDEX32
                    w.Write(CUInt(Me.IndexData(i)))
                End If
            Next i

        End Sub

        Public Function GetNumFaces(ByVal m_PrimitiveType As PrimitiveType) As Integer
            Dim NumFaces As Integer = 0

            'https://docs.microsoft.com/en-us/windows/win32/direct3d9/d3dprimitivetype
            If m_PrimitiveType = PrimitiveType.TriangleFan Then
                NumFaces = 0
                For i = 0 To (Me.m_NumIndices - 2) - 1
                    Dim num8 As Short = Me.IndexData(i)
                    Dim num9 As Short = Me.IndexData((i + 1))
                    Dim num10 As Short = Me.IndexData((i + 2))
                    If ((num8 <> num9) AndAlso ((num9 <> num10) AndAlso (num8 <> num10))) Then
                        NumFaces += 1
                    End If
                Next i

            ElseIf m_PrimitiveType = PrimitiveType.TriangleList Then
                NumFaces = (Me.m_NumIndices \ 3)
            End If

            Return NumFaces
        End Function

        ' Properties
        Public Property IndexData As UInteger()
        'Public Property NumFaces As UInteger
        'Public Property PrimitiveType As PrimitiveType

        Private m_NumIndices As Long
        Private m_IndexSize As Byte = 2 'RWIndexFormat = RWIndexFormat.FMT_INDEX16

    End Class

End Namespace
