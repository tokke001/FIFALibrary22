Namespace Rw.Core.Arena
    Public Class ArenaSectionManifest
        'rw::core::arena::ArenaSectionManifest
        Inherits ArenaSection
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWOBJECTTYPE_SECTIONMANIFEST

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
            'Me.TypeCode = r.ReadUInt32                  'typeReg, pointer to 'rw::core::arena::ArenaTypeReg'
            'Me.NumEntries = r.ReadUInt32                '4: 4 offsets

            Me.PntrOffsets = r.ReadUInt32               '12: offset to

            r.BaseStream.Position = BaseOffset + Me.PntrOffsets
            Me.Offsets = New UInteger(Me.NumEntries - 1) {}
            For i = 0 To Me.Offsets.Length - 1
                Me.Offsets(i) = r.ReadUInt32
            Next

            For i = 0 To Me.NumEntries - 1
                r.BaseStream.Position = BaseOffset + Me.Offsets(i)

                Dim m_ArenaSection As New ArenaSection(Me.RwArena, r)
                Select Case m_ArenaSection.TypeCode
                    Case SectionTypeCode.RWOBJECTTYPE_SECTIONTYPES
                        Me.Sections(i) = New ArenaSectionTypes(m_ArenaSection, Me.RwArena, r)
                    Case SectionTypeCode.RWOBJECTTYPE_SECTIONEXTERNALARENAS
                        Me.Sections(i) = New ArenaSectionExternalArenas(m_ArenaSection, Me.RwArena, r)
                    Case SectionTypeCode.RWOBJECTTYPE_SECTIONSUBREFERENCES
                        Me.Sections(i) = New ArenaSectionSubreferences(m_ArenaSection, Me.RwArena, r)
                    Case SectionTypeCode.RWOBJECTTYPE_SECTIONATOMS
                        Me.Sections(i) = New ArenaSectionAtoms(m_ArenaSection, Me.RwArena, r)
                    Case Else
                        Me.Sections(i) = Nothing
                End Select

            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Dim bytearray As Byte() = New Byte() {}

            Me.TypeCode = TYPE_CODE
            Me.NumEntries = If(Me.Sections Is Nothing, 0, CUInt(Me.Sections.Length)) 'CUInt(Me.Sections.Length)
            Me.PntrOffsets = 12

            'We will have to rewrite it anyways, so just write padding
            w.Write(New Byte(28 - 1) {})

            Me.Offsets = New UInteger(Me.NumEntries - 1) {}
            For i = 0 To Me.NumEntries - 1
                Me.Offsets(i) = w.BaseStream.Position - BaseOffset

                Select Case Me.Sections(i).GetType
                    Case GetType(ArenaSectionTypes)
                        CType(Me.Sections(i), ArenaSectionTypes).Save(w)
                    Case GetType(ArenaSectionExternalArenas)
                        CType(Me.Sections(i), ArenaSectionExternalArenas).Save(w)
                    Case GetType(ArenaSectionSubreferences)
                        CType(Me.Sections(i), ArenaSectionSubreferences).Save(w)
                    Case GetType(ArenaSectionAtoms)
                        CType(Me.Sections(i), ArenaSectionAtoms).Save(w)
                        'Case Else
                End Select
            Next

            Dim EndOffset As Long = w.BaseStream.Position

            'write offsets at beginning of section
            w.BaseStream.Position = BaseOffset

            w.Write(Me.TypeCode)
            w.Write(Me.NumEntries)
            w.Write(Me.PntrOffsets)
            For i = 0 To Me.NumEntries - 1
                w.Write(Me.Offsets(i))
            Next

            w.BaseStream.Position = EndOffset
        End Sub

        Private Function GetSection(ByVal m_Type As Rw.SectionTypeCode) As ArenaSection
            For Each m_object As ArenaSection In Me.Sections
                If m_object.TypeCode = m_Type Then
                    Return m_object
                End If
            Next m_object

            Return Nothing
        End Function

        Private Function IndexOf(ByVal m_Type As Rw.SectionTypeCode) As Integer

            For i As Integer = 0 To Sections.Length - 1
                If Sections(i).TypeCode = m_Type Then
                    Return i
                End If
            Next i

            Return -1
        End Function


        Public Property PntrOffsets As UInteger
        Public Property Offsets As List(Of UInteger)
        Private Property Sections As List(Of ArenaSection)
        Public Property Types As ArenaSectionTypes
            Get
                Return GetSection(SectionTypeCode.RWOBJECTTYPE_SECTIONTYPES)
            End Get
            Set
                Me.Sections(IndexOf(SectionTypeCode.RWOBJECTTYPE_SECTIONTYPES)) = Value
            End Set
        End Property

        Public Property ExternalArenas As ArenaSectionExternalArenas
            Get
                Return GetSection(SectionTypeCode.RWOBJECTTYPE_SECTIONEXTERNALARENAS)
            End Get
            Set
                Me.Sections(IndexOf(SectionTypeCode.RWOBJECTTYPE_SECTIONEXTERNALARENAS)) = Value
            End Set
        End Property

        Public Property SubReferences As ArenaSectionSubreferences
            Get
                Return GetSection(SectionTypeCode.RWOBJECTTYPE_SECTIONSUBREFERENCES)
            End Get
            Set
                Me.Sections(IndexOf(SectionTypeCode.RWOBJECTTYPE_SECTIONSUBREFERENCES)) = Value
            End Set
        End Property

        Public Property Atoms As ArenaSectionAtoms
            Get
                Return GetSection(SectionTypeCode.RWOBJECTTYPE_SECTIONATOMS)
            End Get
            Set
                Me.Sections(IndexOf(SectionTypeCode.RWOBJECTTYPE_SECTIONATOMS)) = Value
            End Set
        End Property

        Public Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function
    End Class

End Namespace