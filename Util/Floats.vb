
Public Class Float3
    Public Sub New()
    End Sub
    Public Sub New(ByVal r As FileReader)
        Me.Value_1 = r.ReadSingle
        Me.Value_2 = r.ReadSingle
        Me.Value_3 = r.ReadSingle
    End Sub

    Public Sub Save(ByVal w As FileWriter)
        w.Write(Me.Value_1)
        w.Write(Me.Value_2)
        w.Write(Me.Value_3)
    End Sub
    Public Property Value_1 As Single
    Public Property Value_2 As Single
    Public Property Value_3 As Single

End Class
Public Class Float4
    Public Sub New()
    End Sub
    Public Sub New(ByVal r As FileReader)
        Me.Value_1 = r.ReadSingle
        Me.Value_2 = r.ReadSingle
        Me.Value_3 = r.ReadSingle
        Me.Value_4 = r.ReadSingle
    End Sub
    Public Sub Save(ByVal w As FileWriter)
        w.Write(Me.Value_1)
        w.Write(Me.Value_2)
        w.Write(Me.Value_3)
        w.Write(Me.Value_4)
    End Sub

    Public Property Value_1 As Single
    Public Property Value_2 As Single
    Public Property Value_3 As Single
    Public Property Value_4 As Single

End Class

'Public Class Vector4_      'used directx ddl instead
'    'rw::math::vpu::Vector4 (0x1538) (0x00024b30)
'    Public Sub New()
'    End Sub
'    Public Sub New(ByVal r As FileReader)
'        Me.m_X = r.ReadSingle
'        Me.m_Y = r.ReadSingle
'        Me.m_Z = r.ReadSingle
'        Me.m_W = r.ReadSingle
'    End Sub
'    Public Sub Save(ByVal w As FileWriter)
'        w.Write(Me.m_X)
'        w.Write(Me.m_Y)
'        w.Write(Me.m_Z)
'        w.Write(Me.m_W)
'    End Sub

'    Public Property m_X As Single
'    Public Property m_Y As Single
'    Public Property m_Z As Single
'    Public Property m_W As Single

'End Class

'Public Class Vector3_      'used directx ddl instead
'    'rw::math::vpu::Vector3 (0x000248e9)
'    Public Sub New()
'    End Sub
'    Public Sub New(ByVal r As FileReader)
'        Me.m_X = r.ReadSingle
'        Me.m_Y = r.ReadSingle
'        Me.m_Z = r.ReadSingle
'    End Sub
'    Public Sub Save(ByVal w As FileWriter)
'        w.Write(Me.m_X)
'        w.Write(Me.m_Y)
'        w.Write(Me.m_Z)
'    End Sub

'    Public Property m_X As Single
'    Public Property m_Y As Single
'    Public Property m_Z As Single

'End Class