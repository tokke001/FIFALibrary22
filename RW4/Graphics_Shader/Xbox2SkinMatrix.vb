
Namespace Rw.Graphics.Shader
<Serializable> Public Class Xbox2SkinMatrix
        'rw::graphics::Shader::Xbox2SkinMatrix

        Public Sub New()

        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.Right_x = r.ReadSingle
            Me.Up_x = r.ReadSingle
            Me.At_x = r.ReadSingle
            Me.Pos_x = r.ReadSingle
            Me.Right_y = r.ReadSingle
            Me.Up_y = r.ReadSingle
            Me.At_y = r.ReadSingle
            Me.Pos_y = r.ReadSingle
            Me.Right_z = r.ReadSingle
            Me.Up_z = r.ReadSingle
            Me.At_z = r.ReadSingle
            Me.Pos_z = r.ReadSingle
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Right_x)
            w.Write(Me.Up_x)
            w.Write(Me.At_x)
            w.Write(Me.Pos_x)
            w.Write(Me.Right_y)
            w.Write(Me.Up_y)
            w.Write(Me.At_y)
            w.Write(Me.Pos_y)
            w.Write(Me.Right_z)
            w.Write(Me.Up_z)
            w.Write(Me.At_z)
            w.Write(Me.Pos_z)
        End Sub

        Public Property Right_x As Single
        Public Property Up_x As Single
        Public Property At_x As Single
        Public Property Pos_x As Single
        Public Property Right_y As Single
        Public Property Up_y As Single
        Public Property At_y As Single
        Public Property Pos_y As Single
        Public Property Right_z As Single
        Public Property Up_z As Single
        Public Property At_z As Single
        Public Property Pos_z As Single

    End Class
End Namespace