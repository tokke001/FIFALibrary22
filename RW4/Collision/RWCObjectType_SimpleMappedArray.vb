Namespace Rw.Collision
    Public Class SimpleMappedArray
        'rw::collision::SimpleMappedArray
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWCOBJECTTYPE_SIMPLEMAPPEDARRAY
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            '--rw::collision::MappedArray   (0x00044ef1)
            Me.m_Aggregate = New Aggregate(r)  'rw::collision::Aggregate

            Me.OffsetVolumes = r.ReadUInt32
            Me.Padma = r.ReadUIntegers(3)     'padding

            r.BaseStream.Position = Me.OffsetVolumes
            Me.Volumes = New Volume(Me.m_Aggregate.NumVolumes - 1) {}
            For i = 0 To Me.m_Aggregate.NumVolumes - 1
                Me.Volumes(i) = New Volume(Me.RwArena, r)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.m_Aggregate.NumVolumes = Me.Volumes.Length

            Me.m_Aggregate.Save(w)

            Me.OffsetVolumes = w.BaseStream.Position + 16
            w.Write(Me.OffsetVolumes)
            w.Write(Me.Padma)

            For i = 0 To Me.m_Aggregate.NumVolumes - 1
                Me.Volumes(i).Save(w)
            Next

        End Sub

        Public Property m_Aggregate As Aggregate    'same values as at section "ModelCollision_ArenaId" (&HEB0004)
        Public Property OffsetVolumes As UInteger        '
        Public Property Padma As UInteger() = New UInteger(3 - 1) {}   'always "0,0,0", padding
        Public Property Volumes As Volume()   'array of "rw::collision::Volume"

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class Aggregate
        'rw::collision::Aggregate
        '= RWCOBJECTTYPE_MESHOPAGGREGATE ???
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            '0x00044c6f
            Me.AABBox = New AABBoxTemplate(r)    'AABBoxTemplate<rw::math::vpu::Matrix44Affine,rw::math::vpu::Vector3,rw::collision::AABBoxMemoryDumpPolicy>

            Me.VTable = r.ReadUInt32        'large changing values?? --> should be pointer to "rw::collision::Aggregate::VTable" ?? 
            Me.NumTagBits = r.ReadUInt32
            Me.NumVolumes = r.ReadUInt32
            Me.Pad = r.ReadUInt32

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            Me.AABBox.Save(w)

            w.Write(Me.VTable)
            w.Write(Me.NumTagBits)
            w.Write(Me.NumVolumes)
            w.Write(Me.Pad)

        End Sub

        Public Property AABBox As AABBoxTemplate    'same values as at section "ModelCollision_ArenaId" (&HEB0004)
        Public Property VTable As UInteger     '(0) = 184 or 232 or 36 ..., (3) = 4 or 6 or 7             usually "3092885764" (B8 59 B1 04), or "3092902406" (B8 59 F2 06), 3092897798 
        Public Property NumTagBits As UInteger           '"5" or "6" , "10" ,3, 4,11
        Public Property NumVolumes As UInteger       'Number of arrays
        Public Property Pad As UInteger           'always "0", padding

    End Class

End Namespace