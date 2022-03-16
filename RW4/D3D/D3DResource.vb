'used at begin of: RW- raster, vertexbuffer, indexbuffer
Namespace Rw.D3D
    Public Class D3DResource
        'D3DResource    0xc0d0
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            '--0x13CA D3DResource
            Me.Common = r.ReadUInt32 '/00/00/00/01 (vertex buffer), /indexsizetype/00/00/02 (index buffer: first byte is indexsizetype !), /00/40/00/03 (raster)
            Me.ReferenceCount = r.ReadUInt32 '/00/00/00/01   (vertex/index buffer), '/01/00/00/00   (raster)
            Me.Fence = r.ReadUInt32 '/00/00/00/00   (always same value)
            Me.ReadFence = r.ReadUInt32 '/00/00/00/00   (always same value)
            Me.Identifier = r.ReadUInt32 '/00/00/00/00   (always same value)
            Me.BaseFlush = r.ReadUInt32 '/FF/FF/00/00   (always same value)

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.Common)
            w.Write(Me.ReferenceCount)
            w.Write(Me.Fence)
            w.Write(Me.ReadFence)
            w.Write(Me.Identifier)
            w.Write(Me.BaseFlush)

        End Sub


        Public Property Common As UInteger
        Public Property ReferenceCount As UInteger
        Public Property Fence As UInteger
        Public Property ReadFence As UInteger
        Public Property Identifier As UInteger
        Public Property BaseFlush As UInteger

        Public Property D3DRESOURCETYPE As D3DRESOURCETYPE  'experimental: offsets not from dump, but found (not-linked) enums
            Get
                Return FifaUtil.GetValueFrom32bit(Common, 0, 5)
            End Get
            Set
                Common = FifaUtil.SetValueTo32bit(Common, Value, 0, 5)
            End Set
        End Property
        Public Property D3DFORMAT As D3DFORMAT  'experimental: offsets not from dump, but found (not-linked) enums
            Get
                Return FifaUtil.GetValueFrom32bit(Common, 29, 3)
            End Get
            Set
                Common = FifaUtil.SetValueTo32bit(Common, Value, 29, 3)
            End Set
        End Property

    End Class
End Namespace