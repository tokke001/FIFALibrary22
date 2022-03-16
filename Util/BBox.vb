
Public Class BBox
    Public Sub New()
    End Sub
    Public Sub New(ByVal r As FileReader)
        Me.Min = r.ReadVector4
        Me.Max = r.ReadVector4
    End Sub
    Public Sub Save(ByVal w As FileWriter)
        w.Write(Me.Min)
        w.Write(Me.Max)
    End Sub

    Public Property Min As Microsoft.DirectX.Vector4   'Vector4 (min)
    Public Property Max As Microsoft.DirectX.Vector4    'Vector4 (max)

End Class

Public Class AABBoxTemplate
    'AABBoxTemplate<rw::math::vpu::Matrix44Affine,rw::math::vpu::Vector3,rw::collision::AABBoxMemoryDumpPolicy>
    Public Sub New()
    End Sub
    Public Sub New(ByVal r As FileReader)
        Me.Min = r.ReadVector3
        Me.Unused_1 = r.ReadUInt32
        Me.Max = r.ReadVector3
        Me.Unused_2 = r.ReadUInt32
    End Sub
    Public Sub Save(ByVal w As FileWriter)
        w.Write(Me.Min)
        w.Write(Me.Unused_1)
        w.Write(Me.Max)
        w.Write(Me.Unused_2)
    End Sub

    Public Property Min As Microsoft.DirectX.Vector3   'Vector4 (min)
    Private Unused_1 As UInteger = 0
    Public Property Max As Microsoft.DirectX.Vector3    'Vector4 (max)
    Private Unused_2 As UInteger = 0

End Class