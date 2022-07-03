Namespace Rw.EA
    Public Class ArenaDictionary 'old name : RW4NameSection
        'EA::ArenaDictionary
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.EA_ArenaDictionary
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.Header = New ArenaDictionaryHeader(r)    'EA::ArenaDictionaryHeader

            ReDim Me.Entries(Me.Header.NumEntries - 1)
            For i = 0 To Me.Header.NumEntries - 1
                Me.Entries(i) = New ArenaDictionaryEntry(Me.RwArena, r)
            Next i

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.Header.NumEntries = CUShort(Me.Entries.Length)

            Me.Header.Save(w)

            Dim BaseOffset As Long = w.BaseStream.Position
            'write infos with wrong offsets
            For i = 0 To Me.Header.NumEntries - 1
                Me.Entries(i).SaveInfos(w)
            Next i
            'write names (+ get offsets)
            For i = 0 To Me.Header.NumEntries - 1
                Me.Entries(i).SaveNames(w)
            Next i
            Dim EndOffset As Long = w.BaseStream.Position
            'write again with correct offsets
            w.BaseStream.Position = BaseOffset
            For i = 0 To Me.Header.NumEntries - 1
                Me.Entries(i).SaveInfos(w)
            Next i

            w.BaseStream.Position = EndOffset
        End Sub

        Public Function GetNameStrings() As String()
            Dim ReturnStr As String() = New String(Me.Header.NumEntries - 1) {}

            For i = 0 To Me.Header.NumEntries - 1
                ReturnStr(i) = Me.Entries(i).m_Name
            Next

            Return ReturnStr
        End Function

        Public Function GetTypes() As UInteger()
            Dim ReturnInt As UInteger() = New UInteger(Me.Header.NumEntries - 1) {}

            For i = 0 To Me.Header.NumEntries - 1
                ReturnInt(i) = Me.Entries(i).m_Type
            Next

            Return ReturnInt
        End Function

        Public Function GetNameTable() As List(Of NameTable)
            Dim ReturnTable As New List(Of NameTable)

            For i = 0 To Me.Header.NumEntries - 1
                ReturnTable.Add(New NameTable With {
                    .m_Type = Me.Entries(i).m_Type,
                    .m_Name = Me.Entries(i).m_Name
                })
            Next

            Return ReturnTable
        End Function

        Public Function GetNameByType(ByVal m_Type As SectionTypeCode, ByVal IndexOfType As UInteger) As String
            Dim NumFound As UInteger = 0
            For i = 0 To Me.Header.NumEntries - 1
                If Me.Entries(i).m_Type = m_Type Then
                    If NumFound = IndexOfType Then
                        Return Me.Entries(i).m_Name
                    End If
                    NumFound += 1
                End If
            Next

            Return ""
        End Function

        Public Sub SetNameByType(ByVal Value As String, ByVal m_Type As SectionTypeCode, ByVal IndexOfType As UInteger)
            Dim NumFound As UInteger = 0
            For i = 0 To Me.Header.NumEntries - 1
                If Me.Entries(i).m_Type = m_Type Then
                    If NumFound = IndexOfType Then
                        Me.Entries(i).m_Name = Value
                        Exit Sub
                    End If
                    NumFound += 1
                End If
            Next
        End Sub

        Public Property Header As ArenaDictionaryHeader
        Public Property Entries As ArenaDictionaryEntry()

        'Public Enum Unnamed_tag    'EA::ArenaDictionary::<unnamed-tag>
        '    OBJECT_TYPE = 15466512
        'End Enum

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class ArenaDictionaryHeader
        'EA::ArenaDictionaryHeader
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Version = r.ReadByte     '00 or 01 
            Me.Flags = r.ReadByte       '00 
            Me.NumEntries = r.ReadUInt16  'EntryCount
            Me.HashSeed = r.ReadUInt32  '00 00 15 05    Always same
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Version)
            w.Write(Me.Flags)
            w.Write(Me.NumEntries)
            w.Write(Me.HashSeed)
        End Sub

        Public Property Version As Byte         '00 or 01
        Public Property Flags As Byte           '00
        Public Property NumEntries As UShort
        Public Property HashSeed As UInteger    'always 00 00 15 05

    End Class

    Public Class ArenaDictionaryEntry
        'EA::ArenaDictionaryEntry
        'Public Sub New()
        'End Sub
        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            Me.RwArena = RwArena
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.OffsetName = r.ReadUInt32
            Me.m_Type = r.ReadUInt32 'usually 020003 (raster)
            Me.m_PObject = Me.RwArena.Sections.GetObject(r.ReadInt32)

            Dim BaseOffset As Long = r.BaseStream.Position
            r.BaseStream.Position = Me.OffsetName
            Me.m_Name = FifaUtil.ReadNullTerminatedString(r)

            r.BaseStream.Position = BaseOffset
        End Sub

        Public Sub SaveInfos(ByVal w As FileWriter)
            w.Write(Me.OffsetName)
            w.Write(CUInt(Me.m_Type))
            w.Write(Me.RwArena.Sections.IndexOf(Me.m_PObject))
        End Sub

        Public Sub SaveNames(ByVal w As FileWriter)
            Me.OffsetName = w.BaseStream.Position
            FifaUtil.WriteNullTerminatedString(w, Me.m_Name)
        End Sub

        Private RwArena As Rw.Core.Arena.Arena
        Public Property OffsetName As UInteger
        Public Property m_Type As SectionTypeCode
        Public Property m_PObject As RwObject   'refers to indexes at sections
        Public Property m_Name As String

    End Class

End Namespace