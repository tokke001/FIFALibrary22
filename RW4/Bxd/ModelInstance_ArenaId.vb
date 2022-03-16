Namespace Rw.Bxd
    Public Class Instance
        'bxd::tInstance
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.MODELINSTANCE_ARENAID
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

            Me.Intersect = r.ReadUIntegers(2)     'empty arrays ?

            Me.m_Flags = r.ReadUInt32
            Me.AID = r.ReadUInt32


            'rw::math::vpu::Matrix44: 0x00024bf7
            Me.M4Transform = New Matrix4x4(r)

            'V3Min & V3Max: 2 x vector4
            Me.BBox = New BBox(r)

            Me.PRenderModel = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), RenderModel)   'SectIndex_HEB0003ModelRender
            Me.PSkeleton = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), Bxd.Skeleton) 'SectIndex_HEB0001ModelSkeleton
            Me.PDefaultPose = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), Skeletonpose) 'SectIndex_HEB0002ModelSkeletonPose
            Me.PCollisionModel = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), CollisionModel)    'SectIndex_HEB0004ModelCollision

            Me.AnimSeqMap = New AnimSeqMap(r)
            Me.NumAnimSeqs = r.ReadUInt32
            Me.OffsetPtrsAnimSeqs = r.ReadUInt32
            Me.Padding = r.ReadUInt32   'padding?

            r.BaseStream.Position = BaseOffset + Me.OffsetPtrsAnimSeqs
            Me.PAnimSeqs = New AnimSeq(Me.NumAnimSeqs - 1) {}
            For i = 0 To Me.PAnimSeqs.Length - 1
                Me.PAnimSeqs(i) = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), AnimSeq)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumAnimSeqs = Me.PAnimSeqs.Length
            Me.OffsetPtrsAnimSeqs = 144

            For i = 0 To Me.Intersect.Length - 1
                w.Write(Me.Intersect(i))
            Next
            w.Write(Me.m_Flags)
            w.Write(Me.AID)

            Me.M4Transform.Save(w)

            Me.BBox.Save(w)

            w.Write(Me.RwArena.Sections.IndexOf(Me.PRenderModel))
            w.Write(Me.RwArena.Sections.IndexOf(Me.PSkeleton))
            w.Write(Me.RwArena.Sections.IndexOf(Me.PDefaultPose))
            w.Write(Me.RwArena.Sections.IndexOf(Me.PCollisionModel))

            Me.AnimSeqMap.Save(w)
            w.Write(Me.NumAnimSeqs)
            w.Write(Me.OffsetPtrsAnimSeqs)
            w.Write(Me.Padding)
            For i = 0 To Me.NumAnimSeqs - 1
                w.Write(Me.RwArena.Sections.IndexOf(Me.PAnimSeqs(i)))
            Next

        End Sub

        Public Property Intersect As UInteger() = New UInteger(2 - 1) {}    'always 0
        Public Property m_Flags As Flags       'always 2
        Public Property AID As UInteger       'always 0

        Public Property M4Transform As Matrix4x4 'usually an identity matrix
        Public Property BBox As BBox   '2xVector4 (min, max)
        Public Property PRenderModel As RenderModel        'section index of MODELRENDER_ARENAID ( &HEB0003), "-1" value if not used
        Public Property PSkeleton As Bxd.Skeleton      'section index of MODELSKELETON_ARENAID ( &HEB0001), "-1" value if not used
        Public Property PDefaultPose As Skeletonpose  'section index of MODELSKELETONPOSE_ARENAID ( &HEB0002), "-1" value if not used
        Public Property PCollisionModel As CollisionModel     'section index of MODELCOLLISION_ARENAID ( &HEB0004), "-1" value if not used

        Public Property AnimSeqMap As AnimSeqMap       '0 or 1, 0 when NumAnimSeqs is 0 --> "enable value" ?? may defines if there are AnimSeqs
        Public Property NumAnimSeqs As UInteger       'usually 0, but can be 1
        Public Property OffsetPtrsAnimSeqs As UInteger       'always 144 (section size ?)
        Public Property Padding As UInteger = 0      'always 0
        Public Property PAnimSeqs As AnimSeq()       'section indexes of  ANIMSEQ_ARENAID ( &HEB000C), array of size NumAnimSeqs

        <Flags>
        Public Enum Flags As Integer    'bxd::tInstance::Flags ?
            FLAG_ALPHA = 1
            FLAG_FX = 2
        End Enum

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace