Imports FIFALibrary22.Rw.D3D
Imports Microsoft.DirectX.Direct3D

Namespace Rw.Graphics
    Public Class IndexBuffer
        'rw::graphics::IndexBuffer
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.D3dIndexBuffer = New D3DIndexBuffer(r)

            '--offset 32
            Me.NumIndices = r.ReadUInt32

            If MyBase.RwArena.UseRwBuffers AndAlso Me.PBuffer IsNot Nothing Then
                Me.CreateIndexData()
            End If

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            If MyBase.RwArena.UseRwBuffers Then   'only do this for rx2 (when rw buffer is used), 
                Me.SetValues()
                If Me.NeedToSaveRawData Then
                    Me.CreateRawData()
                    Me.NeedToSaveRawData = False
                End If
            End If

            Me.D3dIndexBuffer.Save(w)
            w.Write(Me.NumIndices)
        End Sub

        Private Sub SetValues()
            If Me.IndexData IsNot Nothing Then
                Me.SetValues(Me.IndexData.Count, FifaUtil.GetIndexStride(Me.IndexData))
            End If
        End Sub

        Friend Sub SetValues(ByVal NumIndices As UInteger, ByVal IndexStride As Byte)
            Me.NumIndices = NumIndices
            Me.IndexStride = IndexStride

            Dim Tmp_IndexSize As UInteger = IndexStride * NumIndices
            Do While Tmp_IndexSize Mod 16 <> 0   'calculate padding
                Tmp_IndexSize += 1
            Loop
            Me.D3dIndexBuffer.SizeIndexBuffer = Tmp_IndexSize
        End Sub

        Public Function GetIndexData() As List(Of UInteger)
            If (Me.IndexData Is Nothing) Then
                Me.CreateIndexData()
            End If

            Return Me.IndexData
        End Function

        Public Sub SetIndexData(ByVal IndexData As List(Of UInteger))
            Me.NumIndices = IndexData.Count
            Me.IndexStride = FifaUtil.GetIndexStride(IndexData)

            Me.D3dIndexBuffer.SizeIndexBuffer = Me.NumIndices * Me.IndexStride '"Size" = (IndexStride * NumIndices) + padding_to_allignment_of_16
            Do While Me.D3dIndexBuffer.SizeIndexBuffer Mod 16 <> 0   'calculate padding
                Me.D3dIndexBuffer.SizeIndexBuffer += 1
            Loop

            Me.IndexData = IndexData

            Me.NeedToSaveRawData = True
        End Sub

        Public Function GetNumFaces(ByVal m_PrimitiveType As PrimitiveType) As Integer
            Dim NumFaces As Integer = 0
            Dim m_IndexData As List(Of UInteger) = Me.GetIndexData

            'https://docs.microsoft.com/en-us/windows/win32/direct3d9/d3dprimitivetype
            If m_PrimitiveType = PrimitiveType.TriangleFan Then
                NumFaces = 0
                For i = 0 To (Me.NumIndices - 2) - 1
                    Dim num8 As Short = m_IndexData(i)
                    Dim num9 As Short = m_IndexData((i + 1))
                    Dim num10 As Short = m_IndexData((i + 2))
                    If ((num8 <> num9) AndAlso ((num9 <> num10) AndAlso (num8 <> num10))) Then
                        NumFaces += 1
                    End If
                Next i

            ElseIf m_PrimitiveType = PrimitiveType.TriangleList Then
                NumFaces = (Me.NumIndices \ 3)
            End If

            Return NumFaces
        End Function

        Private Sub CreateIndexData()      'Read: bytes -> Indices
            If Me.PBuffer.Data.Length >= Me.NumIndices * Me.IndexStride Then
                Dim f As New MemoryStream(Me.PBuffer.Data)
                Dim r As New FileReader(f, Endian.Big)

                Me.IndexData = New List(Of UInteger)
                For i = 0 To Me.NumIndices - 1
                    If Me.IndexStride = 2 Then      'RWIndexFormat.FMT_INDEX16
                        Me.IndexData.Add(r.ReadUInt16)
                    ElseIf Me.IndexStride = 4 Then  'RWIndexFormat.FMT_INDEX32
                        Me.IndexData.Add(r.ReadUInt32)
                    End If
                Next i
            End If
        End Sub

        Private Sub CreateRawData()     'Write: Indices -> bytes
            Dim Tmp_IndexSize As UInteger = (Me.NumIndices * Me.IndexStride)
            Do While Tmp_IndexSize Mod 16 <> 0   'calculate padding: always allignment 16 !
                Tmp_IndexSize += 1
            Loop

            Me.PBuffer.Data = New Byte(Tmp_IndexSize - 1) {}
            Dim output As New MemoryStream(Me.PBuffer.Data)
            Dim w As New FileWriter(output, Endian.Big)

            For i = 0 To Me.NumIndices - 1
                If Me.IndexStride = 2 Then      'RWIndexFormat.FMT_INDEX16
                    w.Write(CUShort(Me.IndexData(i)))
                ElseIf Me.IndexStride = 4 Then  'RWIndexFormat.FMT_INDEX32
                    w.Write(CUInt(Me.IndexData(i)))
                End If
            Next i

        End Sub

        Public Property PBuffer As Rw.Core.Arena.Buffer ' pointer to index of buffer
            Get
                Return CType(Me.RwArena.Sections.GetObject(Me.D3dIndexBuffer.BaseAddress), Rw.Core.Arena.Buffer)
            End Get
            Set(value As Rw.Core.Arena.Buffer)
                Me.D3dIndexBuffer.BaseAddress = Me.RwArena.Sections.IndexOf(value)
            End Set
        End Property

        Public Property IndexStride As Byte  'get Stride
            Get
                If Me.D3dIndexBuffer.D3dResource.D3DFORMAT = D3DFORMAT.D3DFMT_INDEX32 Then
                    Return 4
                End If

                Return 2    ''RWIndexFormat.FMT_INDEX16
            End Get
            Set
                If Value = 4 Then
                    Me.D3dIndexBuffer.D3dResource.D3DFORMAT = D3DFORMAT.D3DFMT_INDEX32
                Else
                    Me.D3dIndexBuffer.D3dResource.D3DFORMAT = D3DFORMAT.D3DFMT_INDEX16
                End If
            End Set
        End Property

        Public Property D3dIndexBuffer As D3DIndexBuffer
        Public Property NumIndices As UInteger

        Private NeedToSaveRawData As Boolean = False
        Private IndexData As List(Of UInteger) = Nothing

        <Flags>
        Public Enum Flags As Integer    'rw::graphics::IndexBuffer::Flags --> unused ?
            FLAGS_NA = -1
            FLAGS_STATIC = 0
            FLAGS_DYNAMIC = 1
            FLAGS_STATIC_READ = 2
            FLAGS_FORCEENUMSIZEINT = 2147483647
        End Enum

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace

'Public Enum RWIndexFormat As Byte   'not official naming : no documentation found, but values confirmed by testing
'FMT_INDEX16 = 32
'FMT_INDEX32 = 192
'End Enum