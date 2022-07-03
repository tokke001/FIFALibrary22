Namespace Rw.Core.Arena
    Public Class Buffer
        'rw::Core::Arena::?
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWOBJECTTYPE_BUFFER
        'Public Const ALIGNMENT As Integer = 4 (vertexes/indexes) or 4096 (raster)

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Me.Data = r.ReadBytes(SectionInfo.Size)
        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            w.Write(Me.Data)
        End Sub

        'Private Sub CreateRawData()
        '    Dim DataType As Type '= Nothing
        '    If Me.m_Data IsNot Nothing AndAlso Me.m_Data.Count > 1 Then
        '        DataType = Me.m_Data(0).GetType()
        '        If Not ((DataType Is GetType(Vertex)) Or (DataType Is GetType(UInteger)) Or (DataType Is GetType(List(Of RawImage)))) Then
        '            Exit Sub
        '        End If
        '    Else
        '        Exit Sub
        '    End If


        '    Me.Data = New Byte() {}
        '    Dim output As New MemoryStream() ', 0, 0)
        '    Dim w As New FileWriter(output, Endian.Big)

        '    Select Case DataType
        '        Case GetType(Vertex)
        '            'Dim m_VertexElements As VertexElement() = Me.PVertexDescriptor.Elements
        '            For i = 0 To Me.m_Data.Count - 1
        '                CType(Me.m_Data(i), Vertex).Save(w)
        '            Next i

        '        Case GetType(UInteger)
        '            Dim IndexStride As Byte
        '            If Me.m_Data.Max > UShort.MaxValue Then 'Me.NumIndices > UShort.MaxValue Then
        '                IndexStride = 4  'RWIndexFormat.FMT_INDEX32
        '            Else
        '                IndexStride = 2  'RWIndexFormat.FMT_INDEX16
        '            End If
        '            'Me.D3dIndexBuffer.SizeIndexBuffer = Me.NumIndices * Me.IndexStride '"Size" = (IndexStride * NumIndices) + padding_to_allignment_of_16

        '            For i = 0 To Me.m_Data.Count - 1
        '                If IndexStride = 2 Then      'RWIndexFormat.FMT_INDEX16
        '                    w.Write(CUShort(Me.m_Data(i)))
        '                ElseIf IndexStride = 4 Then  'RWIndexFormat.FMT_INDEX32
        '                    w.Write(CUInt(Me.m_Data(i)))
        '                End If
        '            Next i

        '        Case GetType(List(Of RawImage))
        '            Dim SwapEndian_DxtBlock As Boolean = True

        '            For f = 0 To m_Data.Count - 1
        '                For i = 0 To m_Data(f).Count - 1
        '                    CType(m_Data(f)(i), RawImage).Save(SwapEndian_DxtBlock, w)
        '                Next i
        '            Next f

        '    End Select

        '    Me.Data = output.GetBuffer

        'End Sub


        'Public Property sRawData As List(Of Object)
        '    Get
        '        If (Me.m_Bitmap Is Nothing) Then
        '            Me.CreateBitmap()
        '        End If
        '        Return Me.m_Bitmap
        '    End Get
        '    Set(ByVal value As Bitmap)
        '        Me.m_Bitmap = value
        '        Me.NeedToSaveRawData = True
        '    End Set
        'End Property


        Public Property Data As Byte()
        'Friend Property m_Data As List(Of Object)
        'Private NeedToSaveRawData As Boolean = False

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        'Public Overrides Function GetAlignment() As Integer
        '    Return ALIGNMENT
        'End Function
    End Class
End Namespace