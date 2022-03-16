Namespace Rw.Bxd
    Public Class CollisionModel
        'bxd::tCollisionModel
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.MODELCOLLISION_ARENAID
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

            Me.BBox = New BBox(r) 'm_v3Min, m_v3Max

            Me.NumSpaces = r.ReadInt32
            Me.OffsetSpaces = r.ReadUInt32      'pointer to bxd::tCollisionSpace
            Me.Padding = r.ReadUIntegers(2)     'padding i think: always 0

            r.BaseStream.Position = BaseOffset + Me.OffsetSpaces
            'Spaces
            Me.Spaces = New CollisionSpace(Me.NumSpaces - 1) {}
            For i = 0 To Me.Spaces.Length - 1
                Me.Spaces(i) = New CollisionSpace(BaseOffset, Me.RwArena, r)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumSpaces = Me.Spaces.Length
            Me.OffsetSpaces = 48

            Me.BBox.Save(w)

            w.Write(Me.NumSpaces)
            w.Write(Me.OffsetSpaces)
            w.Write(Me.Padding)

            For i = 0 To Me.NumSpaces - 1
                Me.Spaces(i).Save(BaseOffset, w)
            Next

        End Sub

        Public Property BBox As BBox              '2xVector4 (min, max) ???
        Public Property NumSpaces As Integer           'always 1 (offset to Bbox_1 ?? )
        Public Property OffsetSpaces As UInteger        'always 48
        Public Property Padding As UInteger() = New UInteger(2 - 1) {}       'always 0,0: padding
        Public Property Spaces As CollisionSpace()         'bxd::tCollisionSpace -array  of size NumSpaces

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class CollisionSpace
        'bxd::tCollisionSpace
        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            Me.RwArena = RwArena
        End Sub

        Public Sub New(ByVal BaseOffset As Long, ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.Load(BaseOffset, r)
        End Sub

        Public Sub Load(ByVal BaseOffset As Long, ByVal r As FileReader)

            Me.BBox = New BBox(r)   'm_v3Min, m_v3Max --> usually same values as ModelCollision_ArenaId.BBox

            Me.Flags = r.ReadUInt32
            Me.BoneIndex = r.ReadInt32
            Me.NumShapes = r.ReadInt32
            Me.OffsetShapes = r.ReadUInt32

            r.BaseStream.Position = BaseOffset + Me.OffsetShapes
            Me.Shapes = New CollisionShape(Me.NumShapes - 1) {}
            For i = 0 To Me.Shapes.Length - 1
                Me.Shapes(i) = New CollisionShape(Me.RwArena, r)
            Next

        End Sub

        Public Sub Save(ByVal BaseOffset As Long, ByVal w As FileWriter)
            Me.NumShapes = Me.Shapes.Length

            Me.BBox.Save(w)

            w.Write(Me.Flags)
            w.Write(Me.BoneIndex)
            w.Write(Me.NumShapes)
            Me.OffsetShapes = w.BaseStream.Position - BaseOffset + 4
            w.Write(Me.OffsetShapes)

            For i = 0 To Me.NumShapes - 1
                Me.Shapes(i).Save(w)
            Next

        End Sub

        Private RwArena As Rw.Core.Arena.Arena

        Public Property BBox As New BBox         '2x Vector4 (min, max) ???   --> usually same values as BBox_1
        Public Property Flags As UInteger       'always 0
        Public Property BoneIndex As Integer       'always 0
        Public Property NumShapes As Integer       'always 1
        Public Property OffsetShapes As UInteger       'always 96
        Public Property Shapes As CollisionShape()       'always 3

    End Class

    Public Class CollisionShape
        'bxd::tCollisionShape
        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            Me.RwArena = RwArena
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.eType = r.ReadUInt32
            Me.PShape = Me.RwArena.Sections.GetObject(r.ReadInt32)
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.eType)
            w.Write(Me.RwArena.Sections.IndexOf(Me.PShape))
        End Sub

        Private RwArena As Rw.Core.Arena.Arena

        Public Property eType As ECollisionShape  'always 3 (Volume)
        Public Property PShape As RWObject      'if type 3 : section index of RWCOBJECTTYPE_VOLUME (&H80001)

        Public Enum ECollisionShape As Integer
            eCS_Undefined = -1
            eCS_TriMesh = 0     '--> might be added --> bxd::tCollisionTriMesh ? --> MESHTRICOLLISION_ARENAID = &HEB0005, OR RWCOBJECTTYPE_TRIANGLEKDTREEPROCEDURAL = &H80003 ?? https://github.com/emd4600/SporeModder-FX/blob/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4/RWTriangleKDTreeProcedural.java
            eCS_SphereTree = 1  '--> bxd::tCollisionSphereMesh --> empty at dump ?
            eCS_ConvexHull = 2
            eCS_Volume = 3      '--> RWCOBJECTTYPE_VOLUME (&H80001)
            eCS_Aggregate = 4   '--> RWCOBJECTTYPE_MESHOPAGGREGATE = &H80007 ?
            eCS_Num = 5
        End Enum
    End Class
End Namespace