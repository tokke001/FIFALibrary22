Namespace Rw.Core.Arena
    Public Class ArenaSection
        'rw::core::arena::ArenaSection
        Protected Friend RwArena As Rw.Core.Arena.Arena

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.TypeCode = r.ReadUInt32
            Me.NumEntries = r.ReadUInt32
        End Sub

        Public Sub New(ByVal ArenaSection As ArenaSection, ByVal RwArena As Rw.Core.Arena.Arena) 'ByVal ArenaSection As ArenaSection, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.TypeCode = ArenaSection.TypeCode
            Me.NumEntries = ArenaSection.NumEntries
        End Sub

        'Public MustOverride Sub Load(ByVal r As FileReader)
        'Public MustOverride Sub Save(ByVal w As FileWriter)

        'Public MustOverride Function GetTypeCode() As Rw.SectionTypeCode

        Public Function GetRwArena() As Rw.Core.Arena.Arena
            Return RwArena
        End Function

        Friend Property TypeCode As SectionTypeCode 'TypeReg
        Public Property NumEntries As UInteger
    End Class
End Namespace