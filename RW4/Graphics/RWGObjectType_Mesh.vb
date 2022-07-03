Namespace Rw.Graphics
    Public Class EmbeddedMesh
        'rw::graphics::EmbeddedMesh
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWGOBJECTTYPE_MESH
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.InstancedSize = r.ReadUInt16     '0x0018    'section size ? 
            Me.NumStreams = r.ReadUInt16     '0x0001    --> number of PtrsVertexBuffer, wich is an array according to dump file ?
            Me.NumVertices = r.ReadUInt32
            Me.Start = r.ReadUInt32     'zero

            Me.NumIndices = r.ReadUInt32
            Me.PIndexBuffer = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), IndexBuffer)   'reference to sectionindex H20007 (Index Buffer)

            Me.PVertexBuffers = New VertexBuffer(Me.NumStreams - 1) {}
            For i = 0 To Me.PVertexBuffers.Length - 1
                Me.PVertexBuffers(i) = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), VertexBuffer)  'Array (of NumStreams?)  : reference to sectionindex H20005 (Vertex Buffer)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            'Me.NumStreams = Me.VertexBufferPtrs.length --> not 100% sure !?
            'Me.InstancedSize = 20 + (4 *  Me.NumStreams)
            'Me.NumVertices = 'set in Rx3File section
            'Me.NumIndices = 'set in Rx3File section
            'Me.SectIndex_H20007IndexBuffer = RW4Section.GetRWSectionIndex(Rw.SectionTypeCode.VERTEX_BUFFER)    'need tweaking
            'Me.SectIndex_H20005VertexBuffer = RW4Section.GetRWSectionIndex(Rw.SectionTypeCode.VERTEX_BUFFER)    'need tweaking

            w.Write(Me.InstancedSize)
            w.Write(Me.NumStreams)
            w.Write(Me.NumVertices)
            w.Write(Me.Start)

            w.Write(Me.NumIndices)
            w.Write(Me.RwArena.Sections.IndexOf(Me.PIndexBuffer))

            For i = 0 To Me.PVertexBuffers.Length - 1
                w.Write(Me.RwArena.Sections.IndexOf(Me.PVertexBuffers(i)))
            Next

        End Sub

        Public Property InstancedSize As UShort
        Public Property NumStreams As UShort
        Public Property NumVertices As UInteger
        Public Property Start As UInteger
        Public Property NumIndices As UInteger
        Public Property PIndexBuffer As IndexBuffer  'PtrIndexBuffer
        Public Property PVertexBuffers As VertexBuffer()  'PtrsVertexBuffer

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace