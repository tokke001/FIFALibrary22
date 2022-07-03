Imports FIFALibrary22.Rw.D3D

Namespace Rw.Graphics
    Public Class VertexBuffer
        'rw::graphics::VertexBuffer
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.D3dVertexBuffer = New D3DVertexBuffer(r)

            Me.VertexStride = r.ReadUInt16 'Vertex block Size 'stride
            Me.m_Type = r.ReadByte            '2
            Me.LockedFlags = r.ReadByte     '0
            Me.NumVertices = r.ReadUInt32    'vertexcount
            Me.PVertexDescriptor = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), VertexDescriptor)    'reference to sectionindex &H20004 (VERTEX_DESCRIPTOR)

            If MyBase.RwArena.UseRwBuffers AndAlso Me.PBuffer IsNot Nothing Then
                Me.CreateVertexData()
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

            Me.D3dVertexBuffer.Save(w)

            w.Write(Me.VertexStride)
            w.Write(CByte(Me.m_Type))
            w.Write(Me.LockedFlags)
            w.Write(Me.NumVertices)
            w.Write(Me.RwArena.Sections.IndexOf(Me.PVertexDescriptor))

        End Sub

        Private Sub SetValues()
            If Me.VertexData IsNot Nothing Then
                Me.SetValues(Me.VertexData.Count, Me.PVertexDescriptor.VertexStride)
            End If
        End Sub

        Friend Sub SetValues(ByVal NumVertices As UInteger, ByVal VertexStride As UShort)
            Me.NumVertices = NumVertices
            Me.VertexStride = VertexStride

            Dim Tmp_VertexSize As UInteger = (VertexStride * NumVertices)   'always vertex buffer size + 2  ???
            Do While Tmp_VertexSize Mod 32 <> 0   'calculate padding: always allignment 32 !
                Tmp_VertexSize += 1
            Loop
            Me.D3dVertexBuffer.Format.SizeVertexBuffer = Tmp_VertexSize

        End Sub

        Public Function GetVertexData() As List(Of Vertex)
            If (Me.VertexData Is Nothing) Then
                Me.CreateVertexData()
            End If

            Return Me.VertexData
        End Function

        Public Sub SetVertexData(ByVal VertexData As List(Of Vertex))
            Me.SetVertexData(VertexData, Me.PVertexDescriptor)
        End Sub

        Public Sub SetVertexData(ByVal VertexData As List(Of Vertex), ByVal VertexDescriptor As Rw.Graphics.VertexDescriptor)
            Me.NumVertices = VertexData.Count
            Me.VertexStride = VertexDescriptor.VertexStride

            Dim Tmp_VertexSize As UInteger = (Me.VertexStride * Me.NumVertices)   'always vertex buffer size + 2  ???
            Do While Tmp_VertexSize Mod 32 <> 0   'calculate padding: always allignment 32 !
                Tmp_VertexSize += 1
            Loop
            Me.D3dVertexBuffer.Format.SizeVertexBuffer = Tmp_VertexSize

            Me.VertexData = VertexData
            Me.PVertexDescriptor = VertexDescriptor

            Me.NeedToSaveRawData = True
        End Sub

        Private Sub CreateVertexData()      'Read: bytes -> Vertices
            If Me.PBuffer.Data.Length >= Me.NumVertices * Me.VertexStride Then
                Dim f As New MemoryStream(Me.PBuffer.Data)
                Dim r As New FileReader(f, Endian.Big)

                Me.VertexData = New List(Of Vertex)
                For i = 0 To Me.NumVertices - 1
                    Me.VertexData.Add(New Vertex(Me.PVertexDescriptor.Elements, r))
                Next i
            End If
        End Sub

        Private Sub CreateRawData()     'Write: Vertices -> bytes
            Dim Tmp_VertexSize As UInteger = (Me.VertexStride * Me.NumVertices)
            Do While Tmp_VertexSize Mod 32 <> 0   'calculate padding: always allignment 32 !
                Tmp_VertexSize += 1
            Loop

            Me.PBuffer.Data = New Byte(Tmp_VertexSize - 1) {}
            Dim output As New MemoryStream(Me.PBuffer.Data) ', 0, 0)
            Dim w As New FileWriter(output, Endian.Big)

            Dim m_VertexElements As VertexElement() = Me.PVertexDescriptor.Elements

            For i = 0 To Me.NumVertices - 1
                Me.VertexData(i).Save(m_VertexElements, w)
            Next i

        End Sub

        Public Property PBuffer As Rw.Core.Arena.Buffer ' pointer to index of buffer
            Get
                Return CType(Me.RwArena.Sections.GetObject(Me.D3dVertexBuffer.Format.BaseAddress), Rw.Core.Arena.Buffer)
            End Get
            Set(value As Rw.Core.Arena.Buffer)
                Me.D3dVertexBuffer.Format.BaseAddress = Me.RwArena.Sections.IndexOf(value)
            End Set
        End Property

        Public Property D3dVertexBuffer As D3DVertexBuffer
        Public Property VertexStride As UShort
        Public Property m_Type As EType     'guessed from dump: found these LF_STATICMEMBER s at the section ...
        Public Property LockedFlags As Byte
        Public Property NumVertices As UInteger
        Public Property PVertexDescriptor As VertexDescriptor

        Private NeedToSaveRawData As Boolean = False
        Private VertexData As List(Of Vertex) = Nothing

        'Public Enum Life As Integer 'rw::graphics::VertexBuffer::Life
        '    LIFE_NA = -1
        '    LIFE_STATIC = 0
        '    LIFE_DYNAMIC = 1
        '    LIFE_STATIC_READ = 2
        '    LIFE_MAX = 4
        'End Enum
        Public Enum EType As Byte 'LF_STATICMEMBER at rw::graphics::VertexBuffer ?
            TYPE_STATIC = 0
            TYPE_DYNAMIC = 1
            TYPE_READ = 2
            TYPE_MASK = 3
            TYPE_MAX = 4
        End Enum

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace
