Namespace Rw.Bxd
    Public Class SceneLayer
        'bxd::tSceneLayer
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.SCENELAYER_ARENAID
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

            Me.Id = r.ReadUInt32
            Me.m_Flags = r.ReadUInt32         '0
            Me.NumInstances = r.ReadInt32
            Me.OffsetInstances = r.ReadUInt32         '16 

            r.BaseStream.Position = BaseOffset + Me.OffsetInstances
            Me.PInstances = New Instance(Me.NumInstances - 1) {}
            For i = 0 To Me.NumInstances - 1
                Me.PInstances(i) = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), Instance) 'SectIndex_HEB0000ModelInstance
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumInstances = Me.PInstances.Length
            Me.OffsetInstances = 16

            w.Write(Me.Id)
            w.Write(Me.m_Flags)
            w.Write(Me.NumInstances)
            w.Write(Me.OffsetInstances)
            For i = 0 To Me.NumInstances - 1
                w.Write(Me.RwArena.Sections.IndexOf(Me.PInstances(i)))
            Next

        End Sub

        Public Property Id As UInteger     'first section "0", always goes +1
        Public Property m_Flags As Flags      'always 0
        Public Property NumInstances As Integer          'integer! number of PInstances
        Public Property OffsetInstances As UInteger      'always 16
        Public Property PInstances As Instance()   'section index of MODELINSTANCE_ARENAID ( &HEB0000)

        <Flags>
        Public Enum Flags As Integer    'bxd::tSceneLayer::Flags 
            REFLECTION = 1
            REFLECTION_ONLY = 2
            NORENDER = 4
        End Enum

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace