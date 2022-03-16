Namespace Rw.Core.Arena
    Public Class ArenaSectionAtoms
        'rw::core::arena::ArenaSectionAtoms
        Inherits ArenaSection
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWOBJECTTYPE_SECTIONATOMS

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

        Public Sub Load(ByVal r As FileReader) ', ByVal m_ArenaSection As ArenaSection)
            Dim BaseOffset As Long = r.BaseStream.Position - 8
            'Me.TypeCode = m_ArenaSection.TypeCode
            'Me.NumEntries = m_ArenaSection.NumEntries

            Me.Offset = r.ReadUInt32        'offset to atomTable

            If Me.NumEntries <> 0 Then
                r.BaseStream.Position = BaseOffset + Me.Offset
                For i = 0 To Me.NumEntries - 1
                    Me.AtomTable.Add(r.ReadUInt32)
                Next
            End If

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Me.TypeCode = TYPE_CODE
            Me.NumEntries = Me.AtomTable.Count 'If(Me.AtomTable Is Nothing, 0, CUInt(Me.AtomTable.Length)) 'CUInt(Me.Offsets.Length)
            If Me.NumEntries = 0 Then
                Me.Offset = 0
            Else
                Me.Offset = 12
            End If

            w.Write(CUInt(Me.TypeCode))
            w.Write(Me.NumEntries)

            w.Write(Me.Offset)

            For i = 0 To Me.NumEntries - 1
                w.Write(Me.AtomTable(i))
            Next

        End Sub


        'Public Property TypeCode As SectionTypeCode
        'Public Property NumEntries As UInteger
        Public Property Offset As UInteger
        Public Property AtomTable As New List(Of UInteger)

        Public Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function
    End Class

End Namespace