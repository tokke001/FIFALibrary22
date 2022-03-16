Namespace Rw.Core.Arena
    Public Class ArenaSectionTypes
        'rw::core::arena::ArenaSectionTypes
        Inherits ArenaSection
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWOBJECTTYPE_SECTIONTYPES

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
            'Me.NumEntries = m_ArenaSection.NumEntries

            Me.Offset = r.ReadUInt32        '12: offset to

            r.BaseStream.Position = BaseOffset + Me.Offset
            For i = 0 To Me.NumEntries - 1
                Me.TypeCodes.Add(r.ReadUInt32)
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Me.TypeCode = TYPE_CODE
            Me.NumEntries = Me.TypeCodes.Count 'If(Me.TypeCodes Is Nothing, 0, CUInt(Me.TypeCodes.Length)) 'CUInt(Me.TypeCodes.Length)
            Me.Offset = 12

            w.Write(CUInt(Me.TypeCode))

            w.Write(Me.NumEntries)
            w.Write(Me.Offset)

            For i = 0 To Me.NumEntries - 1
                w.Write(CUInt(Me.TypeCodes(i)))
            Next

        End Sub

        Public Sub CreateList(ByVal Objects As List(Of RWObject))
            ' First we need to create the list with all the type codes
            Me.TypeCodes.Clear()
            Me.TypeCodes.Add(0)
            Me.TypeCodes.Add(SectionTypeCode.RWOBJECTTYPE_BUFFER)
            Me.TypeCodes.Add(&H10031)
            Me.TypeCodes.Add(&H10032)
            Me.TypeCodes.Add(&H10010)

            ' We will do it in a separate list because these have to be sorted
            Dim newTypeCodes As ISet(Of SectionTypeCode) = New SortedSet(Of SectionTypeCode)()

            For Each m_object As RWObject In Objects
                ' Don't add the RWOBJECTTYPE_BUFFER one as this is special
                ' Also don't repeat codes
                Dim typeCode As SectionTypeCode = m_object.GetTypeCode()
                If typeCode <> SectionTypeCode.RWOBJECTTYPE_BUFFER Then
                    newTypeCodes.Add(typeCode)
                End If
            Next m_object

            Me.TypeCodes.AddRange(newTypeCodes)
        End Sub

        Public Property Offset As UInteger
        Public Property TypeCodes As New List(Of SectionTypeCode)

        Public Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function
    End Class
End Namespace