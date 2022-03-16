Namespace Rw.Bxd
    Public Class Skeleton
        'bxd::tSkeleton
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.MODELSKELETON_ARENAID
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position

            Me.NumBones = r.ReadUInt32
            Me.OffsetBones = r.ReadUInt32

            r.BaseStream.Position = BaseOffset + Me.OffsetBones
            Me.Bones = New SkeletonBone(Me.NumBones - 1) {}
            For i = 0 To Me.Bones.Length - 1
                Me.Bones(i) = New SkeletonBone(r)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumBones = Me.Bones.Length
            Me.OffsetBones = 8

            w.Write(Me.NumBones)
            w.Write(Me.OffsetBones)

            For i = 0 To Me.NumBones - 1
                Me.Bones(i).Save(w)
            Next

        End Sub

        Public Property NumBones As UInteger       'always 1, number of values ?
        Public Property OffsetBones As UInteger    'always 8,
        Public Property Bones As SkeletonBone()    'usually {255, 255, 0, 0}

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class SkeletonBone
        'bxd::tSkeletonBone
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.ParentIndex = r.ReadInt16
            Me.PoseMatIndex = r.ReadInt16
        End Sub
        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.ParentIndex)
            w.Write(Me.PoseMatIndex)
        End Sub

        Public Property ParentIndex As Short    'ussually FF,FF
        Public Property PoseMatIndex As Short   'ussually 00,00
    End Class
End Namespace