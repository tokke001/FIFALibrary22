Imports Microsoft.DirectX.Direct3D
Namespace Rx3
    Public Class IndexBuffer
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.INDEX_BUFFER
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.Rx3IndexBufferHeader = New IndexBufferHeader(r)

            Me.IndexData = New UInteger(Me.Rx3IndexBufferHeader.NumIndices - 1) {}
            For i = 0 To Me.Rx3IndexBufferHeader.NumIndices - 1
                If Me.Rx3IndexBufferHeader.IndexStride = 2 Then
                    Me.IndexData(i) = r.ReadUInt16
                ElseIf Me.Rx3IndexBufferHeader.IndexStride = 4 Then
                    Me.IndexData(i) = Me.IndexData(i) = r.ReadUInt32
                End If
            Next i


            'Me.NumFaces = GetNumFaces()    'disabled, to save loading time

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.Rx3IndexBufferHeader.NumIndices = Me.IndexData.Length

            If Me.Rx3IndexBufferHeader.NumIndices > UShort.MaxValue Then
                Me.Rx3IndexBufferHeader.IndexStride = 4
            Else
                Me.Rx3IndexBufferHeader.IndexStride = 2
            End If


            Me.Rx3IndexBufferHeader.Save(Me.Rx3IndexBufferHeader, w)

            For i = 0 To Me.Rx3IndexBufferHeader.NumIndices - 1
                If Me.Rx3IndexBufferHeader.IndexStride = 2 Then
                    w.Write(CUShort(Me.IndexData(i)))
                ElseIf Me.Rx3IndexBufferHeader.IndexStride = 4 Then
                    w.Write(CUInt(Me.IndexData(i)))
                End If
            Next i


            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.Rx3IndexBufferHeader.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub

        Public Function GetNumFaces(ByVal m_PrimitiveType As PrimitiveType) As Integer
            Dim NumFaces As Integer = 0

            'https://docs.microsoft.com/en-us/windows/win32/direct3d9/d3dprimitivetype
            If m_PrimitiveType = PrimitiveType.TriangleList Then
                NumFaces = (Me.Rx3IndexBufferHeader.NumIndices \ 3)

            ElseIf m_PrimitiveType = PrimitiveType.TriangleFan Then
                NumFaces = 0
                For i = 0 To (Me.Rx3IndexBufferHeader.NumIndices - 2) - 1
                    Dim num8 As Short = Me.IndexData(i)
                    Dim num9 As Short = Me.IndexData((i + 1))
                    Dim num10 As Short = Me.IndexData((i + 2))
                    If ((num8 <> num9) AndAlso ((num9 <> num10) AndAlso (num8 <> num10))) Then
                        NumFaces += 1
                    End If
                Next i

            End If

            Return NumFaces
        End Function

        ' Properties
        Public Property IndexData As UInteger()
        'Public Property NumFaces As UInteger
        'Public Property PrimitiveType As PrimitiveType
        Public Property Rx3IndexBufferHeader As IndexBufferHeader

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace