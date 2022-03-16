Imports FIFALibrary22.Rw.D3D

Namespace Rw.Graphics
    Public Class VertexBuffer
        'rw::graphics::VertexBuffer
        Inherits RWObject
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

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)

            'Me.D3dVertexBuffer.SizeVertexBuffer = 'set in Rx3File section
            'Me.VertexStride = 'set in Rx3File section
            'Me.NumVertices = 'set in Rx3File section
            'Me.PtrVertexDescriptor = RW4Section.GetRWSectionIndex(Rw.SectionTypeCode.VERTEX_BUFFER)    'need tweaking

            Me.D3dVertexBuffer.Save(w)

            w.Write(Me.VertexStride)
            w.Write(CByte(Me.m_Type))
            w.Write(Me.LockedFlags)
            w.Write(Me.NumVertices)
            w.Write(Me.RwArena.Sections.IndexOf(Me.PVertexDescriptor))

        End Sub

        'Public Function GetBuffer() As Rw.Core.Arena.Buffer
        '    Return CType(Me.RwArena.Sections.GetObject(Me.D3dVertexBuffer.Format.BaseAddress), Rw.Core.Arena.Buffer)
        'End Function

        Public Property VertexData As Rw.Core.Arena.Buffer ' pointer to index of buffer
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
