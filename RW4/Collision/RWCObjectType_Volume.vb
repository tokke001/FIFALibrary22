Namespace Rw.Collision
    Public Class Volume
        'rw::collision::Volume
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWCOBJECTTYPE_VOLUME
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

            Me.Transform = New Matrix4x4Affine(r) 'rw::math::vpu::Matrix44Affine 0x00024938

            Me.VTypeID = r.ReadInt32    'pointer to "rw::collision::Volume::VTable" > 
            Me.VData = LoadData(Me.VTypeID, r)  'aggregateData, sphereData, capsuleData, triangleData, boxData, cylinderData (depending on type "6" ??) 

            r.BaseStream.Position = BaseOffset + 80
            Me.Radius = r.ReadSingle
            Me.GroupID = r.ReadUInt32
            Me.SurfaceID = r.ReadUInt32
            Me.Flags = r.ReadInt32

        End Sub

        Private Function LoadData(m_VolumeType As VolumeType, ByVal r As FileReader) As Object
            Select Case m_VolumeType
            'Case VolumeType.VOLUMETYPENULL
                Case VolumeType.VOLUMETYPESPHERE
                    Return New SphereSpecificData(r) 'sphereData      0x00044E8C
                Case VolumeType.VOLUMETYPECAPSULE
                    Return New CapsuleSpecificData(r) 'capsuleData    0x00044E8D
                Case VolumeType.VOLUMETYPETRIANGLE
                    Return New TriangleSpecificData(r) 'triangleData   0x00044E8E
                Case VolumeType.VOLUMETYPEBOX
                    Return New BoxSpecificData(r) 'boxData        0x00044E8F
                Case VolumeType.VOLUMETYPECYLINDER
                    Return New CylinderSpecificData(r) 'cylinderData   0x00044E90
                Case VolumeType.VOLUMETYPEAGGREGATE
                    Return New AggregateSpecificData(Me.RwArena, r) 'aggregateData  0x00044E8B
                    'Case VolumeType.VOLUMETYPENUMINTERNALTYPES
                    'Case VolumeType.VOLUMETYPEFORCEENUMSIZEINT
                Case Else
                    Return New UInteger(3 - 1) {r.ReadUInt32, r.ReadUInt32, r.ReadUInt32}
            End Select
        End Function

        Public Overrides Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.VTypeID = GetTypeId(Me.VData)

            Me.Transform.Save(w)

            w.Write(CInt(Me.VTypeID))
            SaveData(Me.VData, w)
            Do While w.BaseStream.Position <> BaseOffset + 80   'me.Vdata must always be size of 3 integers 
                w.Write(CInt(0))
            Loop

            w.Write(Me.Radius)
            w.Write(Me.GroupID)
            w.Write(Me.SurfaceID)
            w.Write(CInt(Me.Flags))

        End Sub

        Private Sub SaveData(m_Data As Object, ByVal w As FileWriter)
            Select Case m_Data.GetType

            'Case VolumeType.VOLUMETYPENULL
                Case GetType(SphereSpecificData) 'VolumeType.VOLUMETYPESPHERE
                    CType(m_Data, SphereSpecificData).Save(w)
                Case GetType(CapsuleSpecificData) 'VolumeType.VOLUMETYPECAPSULE
                    CType(m_Data, CapsuleSpecificData).Save(w)
                Case GetType(TriangleSpecificData) 'VolumeType.VOLUMETYPETRIANGLE
                    CType(m_Data, TriangleSpecificData).Save(w)
                Case GetType(BoxSpecificData) 'VolumeType.VOLUMETYPEBOX
                    CType(m_Data, BoxSpecificData).Save(w)
                Case GetType(CylinderSpecificData) 'VolumeType.VOLUMETYPECYLINDER
                    CType(m_Data, CylinderSpecificData).Save(w)
                Case GetType(AggregateSpecificData) 'VolumeType.VOLUMETYPEAGGREGATE
                    CType(m_Data, AggregateSpecificData).Save(w)
                'Case VolumeType.VOLUMETYPENUMINTERNALTYPES
                'Case VolumeType.VOLUMETYPEFORCEENUMSIZEINT
                Case GetType(UInteger())
                    For i = 0 To CType(m_Data, UInteger()).Length
                        w.Write(CUInt(m_Data(i)))
                    Next
                Case Else
                    w.Write(New UInteger(3 - 1) {})
            End Select

        End Sub
        Private Function GetTypeId(m_Data As Object) As VolumeType
            Select Case m_Data.GetType

            'Case VolumeType.VOLUMETYPENULL
                Case GetType(SphereSpecificData) 'VolumeType.VOLUMETYPESPHERE
                    Return VolumeType.VOLUMETYPESPHERE
                Case GetType(CapsuleSpecificData)
                    Return VolumeType.VOLUMETYPECAPSULE
                Case GetType(TriangleSpecificData)
                    Return VolumeType.VOLUMETYPETRIANGLE
                Case GetType(BoxSpecificData)
                    Return VolumeType.VOLUMETYPEBOX
                Case GetType(CylinderSpecificData)
                    Return VolumeType.VOLUMETYPECYLINDER
                Case GetType(AggregateSpecificData)
                    Return VolumeType.VOLUMETYPEAGGREGATE
                    'Case VolumeType.VOLUMETYPENUMINTERNALTYPES
                    'Case VolumeType.VOLUMETYPEFORCEENUMSIZEINT
                Case Else
                    Return VolumeType.VOLUMETYPENULL
            End Select

        End Function
        Public Property Transform As Matrix4x4Affine '() = New Float4(4 - 1) {}

        Public Property VTypeID As VolumeType   'usually "6" at section RWCObjectType_Volume, 3 at section RWCObjectType_SimpleMappedArray
        Public Property VData As Object      'aggregateData, sphereData, capsuleData, triangleData, boxData, cylinderData (depending on type "6" ??) 

        Public Property Radius As Single   'usually 0
        Public Property GroupID As UInteger   'usually 0
        Public Property SurfaceID As UInteger   'usually 0
        Public Property Flags As VolumeFlag   'usually 1 at section RWCObjectType_Volume, 483 at section RWCObjectType_SimpleMappedArray

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class AggregateSpecificData
        'rw::collision::AggregateSpecificData   0x00044f1c
        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            Me.RwArena = RwArena
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.PAgg = Me.RwArena.Sections.GetObject(r.ReadInt32)       'pointer to "rw::collision::Aggregate", wich can be found at section RWCObjectType_SimpleMappedArray --> index of RWCObjectType_SimpleMappedArray !!
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.RwArena.Sections.IndexOf(Me.PAgg))
        End Sub

        Private RwArena As Rw.Core.Arena.Arena
        Public Property PAgg As RWObject     'pointer to "rw::collision::Aggregate", wich can be found at section RWCObjectType_SimpleMappedArray --> index of RWCObjectType_SimpleMappedArray !!
    End Class

    Public Class SphereSpecificData
        'rw::collision::SphereSpecificData  0x000450bc
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            'Me.m_Nothing = r.ReadUInt32
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            'w.Write(Me.m_Nothing)
        End Sub

        'Public Property m_Nothing As UInteger
    End Class
    Public Class CapsuleSpecificData
        'rw::collision::CapsuleSpecificData   0x00044f74
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Hh = r.ReadSingle
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Hh)
        End Sub

        Public Property Hh As Single
    End Class
    Public Class TriangleSpecificData
        'rw::collision::TriangleSpecificData    0x00044f72
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.EdgeCos0 = r.ReadSingle
            Me.EdgeCos1 = r.ReadSingle
            Me.EdgeCos2 = r.ReadSingle
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.EdgeCos0)
            w.Write(Me.EdgeCos1)
            w.Write(Me.EdgeCos2)
        End Sub

        Public Property EdgeCos0 As Single
        Public Property EdgeCos1 As Single
        Public Property EdgeCos2 As Single

    End Class
    Public Class BoxSpecificData
        'rw::collision::BoxSpecificData 0x000450be
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Hx = r.ReadSingle
            Me.Hy = r.ReadSingle
            Me.Hz = r.ReadSingle
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Hx)
            w.Write(Me.Hy)
            w.Write(Me.Hz)
        End Sub

        Public Property Hx As Single
        Public Property Hy As Single
        Public Property Hz As Single
    End Class
    Public Class CylinderSpecificData
        'rw::collision::CylinderSpecificData    0x00044fa3
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Hh = r.ReadSingle
            Me.InnerRadius = r.ReadSingle
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Hh)
            w.Write(Me.InnerRadius)
        End Sub

        Public Property Hh As Single
        Public Property InnerRadius As Single
    End Class
    Public Enum VolumeType As Integer   'rw::collision::VolumeType
        VOLUMETYPENULL = 0
        VOLUMETYPESPHERE = 1
        VOLUMETYPECAPSULE = 2
        VOLUMETYPETRIANGLE = 3
        VOLUMETYPEBOX = 4
        VOLUMETYPECYLINDER = 5
        VOLUMETYPEAGGREGATE = 6
        VOLUMETYPENUMINTERNALTYPES = 7
        VOLUMETYPEFORCEENUMSIZEINT = 2147483647
    End Enum
    <Flags>
    Public Enum VolumeFlag As Integer   'rw::collision::VolumeFlag
        VOLUMEFLAG_ISENABLED = 1
        VOLUMEFLAG_TRIANGLENORMALISDIRTY = 2
        VOLUMEFLAG_TRIANGLEONESIDED = 16
        VOLUMEFLAG_TRIANGLEEDGE0CONVEX = 32
        VOLUMEFLAG_TRIANGLEEDGE1CONVEX = 64
        VOLUMEFLAG_TRIANGLEEDGE2CONVEX = 128
        VOLUMEFLAG_TRIANGLEUSEEDGECOS = 256
        VOLUMEFLAG_TRIANGLEVERT0DISABLE = 512
        VOLUMEFLAG_TRIANGLEVERT1DISABLE = 1024
        VOLUMEFLAG_TRIANGLEVERT2DISABLE = 2048
        VOLUMEFLAG_TRIANGLEDEFAULT = 481
        VOLUMEFLAG_FORCEENUMSIZEINT = 2147483647
    End Enum
End Namespace