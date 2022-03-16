Namespace Rw.D3D
    Public Class D3DIndexBuffer
        'D3DIndexBuffer
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            '--0x13CA D3DResource 
            Me.D3dResource = New D3DResource(r)
            'Me.Common = r.ReadUInt32 '/00/00/02 (always same value)
            'Me.ReferenceCount = r.ReadUInt32 '/00/00/00/01 (always same value)
            'Me.Fence = r.ReadUInt32 '/00/00/00/00 (always same value)
            'Me.ReadFence = r.ReadUInt32 '/00/00/00/00 (always same value)
            'Me.Identifier = r.ReadUInt32 '/00/00/00/00 (always same value)
            'Me.BaseFlush = r.ReadUInt32 '/FF/FF/00/00 (always same value)

            Me.BaseAddress = r.ReadUInt32    'reference to sectionindex &H10031 (BUFFER)
            Me.SizeIndexBuffer = r.ReadUInt32

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            'Me.PtrIndexBuffer = RW4Section.GetRWSectionIndex(Rw.SectionTypeCode.index_BUFFER)    'need tweaking
            'Me.SizeIndexBuffer = 'set in Rx3File section

            Me.D3dResource.Save(w)

            w.Write(Me.BaseAddress)
            w.Write(Me.SizeIndexBuffer)

        End Sub

        Public Property D3dResource As D3DResource
        Public Property BaseAddress As UInteger           'PtrIndexBuffer
        Public Property SizeIndexBuffer As UInteger     '"Size" = (IndexStride * NumIndices) + padding_to_allignment_of_16

    End Class
End Namespace