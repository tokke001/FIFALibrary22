Imports Microsoft.DirectX.Direct3D
Namespace Rx3
    Public Class IndexBuffer
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.INDEX_BUFFER
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.Header.Load(r) ' = New IndexBufferHeader(r)

            For i = 0 To Me.Header.NumIndices_FileLoad - 1
                If Me.Header.IndexStride_FileLoad = 2 Then
                    Me.IndexData.Add(r.ReadUInt16)
                ElseIf Me.Header.IndexStride_FileLoad = 4 Then
                    Me.IndexData.Add(r.ReadUInt32)
                End If
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Me.Header.Save(w)

            Dim m_indexStride As Byte = Me.Header.IndexStride   'using variable: to save time when calculating the stride in each loop ! 

            For i = 0 To Me.Header.NumIndices - 1
                If m_indexStride = 2 Then
                    w.Write(CUShort(Me.IndexData(i)))
                ElseIf m_indexStride = 4 Then
                    w.Write(CUInt(Me.IndexData(i)))
                End If
            Next i

            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.Header.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub
        ''' <summary>
        ''' Returns the number of faces at the index data. </summary>
        Public Function GetNumFaces(ByVal m_PrimitiveType As PrimitiveType) As Integer
            Dim NumFaces As Integer = 0

            'https://docs.microsoft.com/en-us/windows/win32/direct3d9/d3dprimitivetype
            If m_PrimitiveType = PrimitiveType.TriangleList Then
                NumFaces = (Me.Header.NumIndices \ 3)

            ElseIf m_PrimitiveType = PrimitiveType.TriangleFan Then
                NumFaces = 0
                For i = 0 To (Me.Header.NumIndices - 2) - 1
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

        Private m_IndexData As New List(Of UInteger)

        ' Properties
        ''' <summary>
        ''' Gets/Sets the list of indices. </summary>
        Public Property IndexData As List(Of UInteger)
            Get
                Return m_IndexData
            End Get
            Set
                m_IndexData = Value
                Me.Header = New IndexBufferHeader(m_IndexData)
            End Set
        End Property

        Public Property Header As New IndexBufferHeader(Me.IndexData)

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace