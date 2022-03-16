Namespace Rw.Bxd
    Public Class Skeletonpose
        'bxd::tSkeletonpose
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.MODELSKELETONPOSE_ARENAID
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position

            Me.NumMats = r.ReadInt32
            Me.OffsetM4Mats = r.ReadUInt32

            Me.Pad = r.ReadIntegers(2) ' padding of 2 integers

            r.BaseStream.Position = BaseOffset + Me.OffsetM4Mats
            Me.M4Mats = New Matrix4x4(Me.NumMats - 1) {}
            For i = 0 To Me.M4Mats.Length - 1
                Me.M4Mats(i) = New Matrix4x4(r)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumMats = Me.M4Mats.Length
            Me.OffsetM4Mats = 16

            w.Write(Me.NumMats)
            w.Write(Me.OffsetM4Mats)

            w.Write(Me.Pad) '2 integers

            For i = 0 To Me.NumMats - 1
                Me.M4Mats(i).Save(w)
            Next

        End Sub

        Public Property NumMats As Integer       'always 1, number of M4Mats
        Public Property OffsetM4Mats As UInteger    'always 16, offset to M4Mats 
        Public Property Pad As Integer() = New Integer(2 - 1) {}       'always 0,0 padding
        Public Property M4Mats As Matrix4x4()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace