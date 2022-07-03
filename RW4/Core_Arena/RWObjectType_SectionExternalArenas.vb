Namespace Rw.Core.Arena
    Public Class ArenaSectionExternalArenas
        'rw::core::arena::ArenaSectionExternalArenas
        Inherits ArenaSection
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWOBJECTTYPE_SECTIONEXTERNALARENAS

        Public Sub New(ByVal ArenaSection As ArenaSection, ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(ArenaSection, RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena, r)
            Me.Load(r)
        End Sub

        Public Sub New(ByVal ArenaSection As ArenaSection, ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(ArenaSection, RwArena)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position - 8
            'Me.TypeCode = m_ArenaSection.TypeCode
            'Me.NumEntries = m_ArenaSection.NumEntries       '3

            Me.Offset = r.ReadUInt32                        '24
            Me.Unknown_1 = r.ReadUInt32                     '67108864
            Me.Unknown_2 = r.ReadUInt32                     '4289724416
            Me.Unknown_3 = r.ReadUInt32                     '67108864

            'pointers to rw::core::arena::ArenaDictEntry (unused 0 at FIFA 11)
            r.BaseStream.Position = BaseOffset + Me.Offset
            For i = 0 To Me.NumEntries - 1
                Me.Dict.Add(r.ReadUInt32)
            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Me.TypeCode = TYPE_CODE
            Me.NumEntries = Me.Dict.Count 'If(Me.Dict Is Nothing, 0, CUInt(Me.Dict.Length)) 'CUInt(Me.Offsets.Length)
            Me.Offset = 24

            w.Write(CUInt(Me.TypeCode))
            w.Write(Me.NumEntries)

            w.Write(Me.Offset)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            w.Write(Me.Unknown_3)

            For i = 0 To Me.NumEntries - 1
                w.Write(Me.Dict(i))
            Next

        End Sub


        Public Property Offset As UInteger
        Public Property Unknown_1 As UInteger
        Public Property Unknown_2 As UInteger
        Public Property Unknown_3 As UInteger
        Public Property Dict As New List(Of UInteger)

        Public Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function
    End Class

End Namespace