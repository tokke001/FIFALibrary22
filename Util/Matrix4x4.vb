Imports Microsoft.DirectX

Public Class Matrix4x4
    'rw::math::vpu::Matrix44: 0x00024bf7

    'Public ReadOnly NUM_ROWS As Integer = 4
    'Public ReadOnly NUM_COLS As Integer = 4

    'Public ReadOnly m()() As Single = {New Single(3) {}, New Single(3) {}, New Single(3) {}}
    Public Sub New()

    End Sub
    Public Sub New(ByVal r As FileReader)
        Me.X_Axis = r.ReadVector4
        Me.Y_Axis = r.ReadVector4
        Me.Z_Axis = r.ReadVector4
        Me.W_Axis = r.ReadVector4
    End Sub

    Public Sub Save(ByVal w As FileWriter)
        w.Write(Me.X_Axis)
        w.Write(Me.Y_Axis)
        w.Write(Me.Z_Axis)
        w.Write(Me.W_Axis)
    End Sub

    Public Property X_Axis As Vector4
    Public Property Y_Axis As Vector4
    Public Property Z_Axis As Vector4
    Public Property W_Axis As Vector4

End Class

Public Class Matrix4x4Affine
    'rw::math::vpu::Matrix44Affine: 0x00024938

    'Public ReadOnly NUM_ROWS As Integer = 4
    'Public ReadOnly NUM_COLS As Integer = 4

    'Public ReadOnly m()() As Single = {New Single(3) {}, New Single(3) {}, New Single(3) {}}
    Public Sub New()

    End Sub
    Public Sub New(ByVal r As FileReader)
        Me.X_Axis = r.ReadVector3  'offset 0
        Me.Unused_1 = r.ReadUInt32
        Me.Y_Axis = r.ReadVector3  'offset 16
        Me.Unused_2 = r.ReadUInt32
        Me.Z_Axis = r.ReadVector3  'offset 32
        Me.Unused_3 = r.ReadUInt32
        Me.W_Axis = r.ReadVector3  'offset 48
        Me.Unused_4 = r.ReadUInt32
    End Sub

    Public Sub Save(ByVal w As FileWriter)
        w.Write(Me.X_Axis)
        w.Write(Me.Unused_1)
        w.Write(Me.Y_Axis)
        w.Write(Me.Unused_2)
        w.Write(Me.Z_Axis)
        w.Write(Me.Unused_3)
        w.Write(Me.W_Axis)
        w.Write(Me.Unused_4)
    End Sub

    Public Property X_Axis As Vector3
    Private Unused_1 As UInteger = 0
    Public Property Y_Axis As Vector3
    Private Unused_2 As UInteger = 0
    Public Property Z_Axis As Vector3
    Private Unused_3 As UInteger = 0
    Public Property W_Axis As Vector3
    Private Unused_4 As UInteger = 0

End Class
Public Class Matrix4x3Affine
    'rw::math::vpu::Matrix44Affine: 0x00024938

    'Public ReadOnly NUM_ROWS As Integer = 4
    'Public ReadOnly NUM_COLS As Integer = 4

    'Public ReadOnly m()() As Single = {New Single(3) {}, New Single(3) {}, New Single(3) {}}
    Public Sub New()

    End Sub
    Public Sub New(ByVal r As FileReader)
        Me.X_Axis = r.ReadVector3  'offset 0
        Me.Unused_1 = r.ReadUInt32
        Me.Y_Axis = r.ReadVector3  'offset 16
        Me.Unused_2 = r.ReadUInt32
        Me.Z_Axis = r.ReadVector3  'offset 32
        Me.Unused_3 = r.ReadUInt32
    End Sub

    Public Sub Save(ByVal w As FileWriter)
        w.Write(Me.X_Axis)
        w.Write(Me.Unused_1)
        w.Write(Me.Y_Axis)
        w.Write(Me.Unused_2)
        w.Write(Me.Z_Axis)
        w.Write(Me.Unused_3)
    End Sub

    Public Property X_Axis As Vector3
    Private Unused_1 As UInteger = 0
    Public Property Y_Axis As Vector3
    Private Unused_2 As UInteger = 0
    Public Property Z_Axis As Vector3
    Private Unused_3 As UInteger = 0

End Class