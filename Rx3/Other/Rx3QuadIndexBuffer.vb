Imports Microsoft.DirectX.Direct3D
Namespace Rx3
    Public Class QuadIndexBuffer
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.QUAD_INDEX_BUFFER
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.Rx3QIndexBufferHeader = New QuadIndexBufferHeader(r)

            Me.IndexStream = New UInteger(Me.Rx3QIndexBufferHeader.NumIndices - 1) {}
            For i = 0 To Me.Rx3QIndexBufferHeader.NumIndices - 1
                If Me.Rx3QIndexBufferHeader.IndexSize = 2 Then
                    Me.IndexStream(i) = r.ReadUInt16
                ElseIf Me.Rx3QIndexBufferHeader.IndexSize = 4 Then
                    Me.IndexStream(i) = Me.IndexStream(i) = r.ReadUInt32
                End If
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.Rx3QIndexBufferHeader.NumIndices = Me.IndexStream.Length

            If Me.Rx3QIndexBufferHeader.NumIndices > UShort.MaxValue Then
                Me.Rx3QIndexBufferHeader.IndexSize = 4
            Else
                Me.Rx3QIndexBufferHeader.IndexSize = 2
            End If

            Me.Rx3QIndexBufferHeader.Save(Me.Rx3QIndexBufferHeader, w)

            For i = 0 To Me.Rx3QIndexBufferHeader.NumIndices - 1
                If Me.Rx3QIndexBufferHeader.IndexSize = 2 Then
                    w.Write(CUShort(Me.IndexStream(i)))
                ElseIf Me.Rx3QIndexBufferHeader.IndexSize = 4 Then
                    w.Write(CUInt(Me.IndexStream(i)))
                End If
            Next i


            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.Rx3QIndexBufferHeader.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub

        Public Function GetNumFaces(ByVal m_PrimitiveType As PrimitiveType) As Integer
            Dim NumFaces As Integer = 0

            'https://docs.microsoft.com/en-us/windows/win32/direct3d9/d3dprimitivetype
            If m_PrimitiveType = PrimitiveType.TriangleList Then
                NumFaces = (Me.Rx3QIndexBufferHeader.NumIndices \ 3)

            ElseIf m_PrimitiveType = PrimitiveType.TriangleFan Then
                NumFaces = 0
                For i = 0 To (Me.Rx3QIndexBufferHeader.NumIndices - 2) - 1
                    Dim num8 As Short = Me.IndexStream(i)
                    Dim num9 As Short = Me.IndexStream((i + 1))
                    Dim num10 As Short = Me.IndexStream((i + 2))
                    If ((num8 <> num9) AndAlso ((num9 <> num10) AndAlso (num8 <> num10))) Then
                        NumFaces += 1
                    End If
                Next i

            End If

            Return NumFaces
        End Function

        ' Properties
        Public Property IndexStream As UInteger()
        'Public Property NumFaces As UInteger
        'Public Property PrimitiveType As PrimitiveType
        Public Property Rx3QIndexBufferHeader As QuadIndexBufferHeader

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace