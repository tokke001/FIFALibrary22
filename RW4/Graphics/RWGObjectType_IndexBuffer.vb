Imports FIFALibrary22.Rw.D3D

Namespace Rw.Graphics
    Public Class IndexBuffer
        'rw::graphics::IndexBuffer
        Inherits RWObject
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

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            'Me.NumIndices = 'set in Rx3File section

            Me.D3dIndexBuffer.Save(w)

            w.Write(Me.NumIndices)

        End Sub

        Public Property IndexData As Rw.Core.Arena.Buffer ' pointer to index of buffer
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

                'If BitConverter.GetBytes(Me.D3dIndexBuffer.D3dResource.Common)(0) = RWIndexFormat.FMT_INDEX32 Then
                'Return 4
                'End If

                Return 2    ''RWIndexFormat.FMT_INDEX16
            End Get
            Set
                'Dim m_bytes As Byte() = BitConverter.GetBytes(Me.D3dIndexBuffer.D3dResource.Common)
                'If Value = 4 Then
                '    m_bytes(4 - 1) = CByte(RWIndexFormat.FMT_INDEX32)
                'Else
                '    m_bytes(4 - 1) = CByte(RWIndexFormat.FMT_INDEX16)
                'End If
                'Me.D3dIndexBuffer.D3dResource.Common = BitConverter.ToUInt32(m_bytes, 0)
                'Dim m_bytes As Byte() = BitConverter.GetBytes(Me.D3dIndexBuffer.D3dResource.Common)
                If Value = 4 Then
                    Me.D3dIndexBuffer.D3dResource.D3DFORMAT = D3DFORMAT.D3DFMT_INDEX32
                Else
                    Me.D3dIndexBuffer.D3dResource.D3DFORMAT = D3DFORMAT.D3DFMT_INDEX16
                End If
            End Set
        End Property

        Public Property D3dIndexBuffer As D3DIndexBuffer
        Public Property NumIndices As UInteger

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