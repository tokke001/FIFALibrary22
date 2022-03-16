Imports Microsoft.DirectX

Public Class BonePose
    Public Sub New()
    End Sub
    Public Sub New(ByVal r As FileReader)

        Dim matrix As New Matrix3x4
        matrix.Load(r)
        Me.absBindPose.copy(matrix.Rotation())
        Me.Unknown_1 = matrix.Offset()

        Me.invPoseTranslation = r.ReadVector3

        Me.Unknown_2 = r.ReadInt32
    End Sub

    Public Sub Save(ByVal w As FileWriter)

        Dim matrix As New Matrix3x4
        matrix.Rotation = Me.absBindPose
        matrix.Offset = Me.Unknown_1
        matrix.Save(w)

        w.Write(Me.invPoseTranslation)

        w.Write(Me.Unknown_2)
    End Sub

    Public absBindPose As Matrix = Matrix.Identity()
    Public invPoseTranslation As New Vector3
    Public Unknown_1 As Vector3     'the offset (translation) part of this matrix --> at rx3: usually '0', RW4 usually large integers
    Public Unknown_2 As Integer     'at rx3: usually '0', RW4 usually integer "-2" 
End Class