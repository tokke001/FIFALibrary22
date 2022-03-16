Namespace Rw.Core.Arena
    Public Class ArenaSectionSubreferences
        'rw::core::arena::ArenaSectionSubreferences
        '-- this section contains values if u want get a sub-part of a section, 
        '-- used for stadiums: RenderModel.Meshes(i).RenderObjects(j).PMeshCullInfo 
        '-- ex. :
        '-- ObjectId = 0 --> section index 0 (Cullinfo)
        '-- Offset = 688 --> reads this section at offset, wich is a "MeshCullInfo"
        Inherits ArenaSection
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWOBJECTTYPE_SECTIONSUBREFERENCES

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
            'Me.TypeCode = m_ArenaSection.TypeCode
            'Me.NumEntries = m_ArenaSection.NumEntries

            Me.DictAfterRefix = r.ReadUInt32        '0
            Me.RecordsAfterRefix = r.ReadUInt32     '0

            Me.OffsetEndObjects = r.ReadUInt32      'in dump named 'dict' : offset to 'rw::core::arena::ArenaDictEntry' --> but this is not at FIFA 11, ArenaDictEntry is located earlier in file, not at end
            Me.OffsetRecords = r.ReadUInt32         'records

            Me.NumUsed = r.ReadUInt32             'number of entries used (ussually same as NumEntries)

            Me.LoadRecords(r)

        End Sub

        Public Sub LoadRecords(ByVal r As FileReader)
            If Me.NumEntries > 0 Then
                Dim BaseOffset As Long = r.BaseStream.Position

                r.BaseStream.Position = Me.OffsetRecords
                For i = 0 To Me.NumEntries - 1
                    Me.Records.Add(New ArenaSectionSubreferencesRecord(r))
                Next i

                r.BaseStream.Position = BaseOffset
            End If
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Me.TypeCode = TYPE_CODE
            Me.NumEntries = Me.Records.Count 'If(Me.Records Is Nothing, 0, CUInt(Me.Records.Count)) 
            ' Me.Offset = 'set at sub SaveReferences
            Me.OffsetEndObjects = CUInt(Me.OffsetRecords + (Me.NumEntries * 8))

            w.Write(CUInt(Me.TypeCode))
            w.Write(Me.NumEntries)
            w.Write(Me.DictAfterRefix)
            w.Write(Me.RecordsAfterRefix)
            w.Write(Me.OffsetEndObjects)
            w.Write(Me.OffsetRecords)
            w.Write(Me.NumUsed)

        End Sub

        Public Sub SaveRecords(ByVal w As FileWriter)

            Me.OffsetRecords = w.BaseStream.Position

            For i = 0 To Me.NumEntries - 1
                Me.Records(i).Save(w)
            Next i

            Me.OffsetEndObjects = w.BaseStream.Position

        End Sub


        Public Property DictAfterRefix As UInteger
        Public Property RecordsAfterRefix As UInteger
        Public Property OffsetEndObjects As UInteger
        Public Property OffsetRecords As UInteger
        Public Property Records As New List(Of ArenaSectionSubreferencesRecord)
        Public Property NumUsed As UInteger

        Public Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function
    End Class

    Public Class ArenaSectionSubreferencesRecord
        'rw::core::arena::ArenaSectionSubreferencesRecord
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.ObjectId = r.ReadUInt32
            Me.Offset = r.ReadUInt32
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.ObjectId)
            w.Write(Me.Offset)
        End Sub

        Public Property ObjectId As UInteger    'section index 0 value (ex. stadiums)
        Public Property Offset As UInteger

    End Class

End Namespace