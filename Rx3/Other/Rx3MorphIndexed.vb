Namespace Rx3
    Public Class MorphIndexed   '-- checked by me tokke001
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.MORPH_INDEXED
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Me.Unknown_1 = r.ReadUInt32
            Dim m_NumEntries As UInteger = r.ReadUInt32
            Me.Unknown_2 = r.ReadUInt32
            Me.Unknown_3 = r.ReadUInt32

            Me.Entries = New MorphIndexedEntry(m_NumEntries - 1) {}
            For i = 0 To m_NumEntries - 1
                Me.Entries(i) = New MorphIndexedEntry(r)
            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.TotalSize)
            w.Write(Me.Unknown_1)
            w.Write(Me.NumEntries)
            w.Write(Me.Unknown_2)
            w.Write(Me.Unknown_3)

            For i = 0 To Me.NumEntries - 1
                Me.Entries(i).Save(w)
            Next

            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub


        Private m_TotalSize As UInteger

        ''' <summary>
        ''' Total section size (ReadOnly). </summary>
        Public Property TotalSize As UInteger
            Get
                Return m_TotalSize
            End Get
            Private Set
                m_TotalSize = Value
            End Set
        End Property
        Public Property Unknown_1 As UInteger   'always 1: endian maybe ?
        ''' <summary>
        ''' Returns the number of Entries (ReadOnly). </summary>
        Public ReadOnly Property NumEntries As UInteger
            Get
                Return If(Entries?.Count, 0)
            End Get
        End Property
        Public Property Unknown_2 As UInteger   'always 0
        Public Property Unknown_3 As UInteger   'always 0
        ''' <summary>
        ''' Gets/Sets the Entries. </summary>
        Public Property Entries As MorphIndexedEntry()   'rest of (unknown) data

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function


    End Class

    Public Class MorphIndexedEntry
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As FileReader)
            Dim m_NumValues As UInteger = r.ReadUInt32

            Me.Indices = New UShort(m_NumValues - 1) {}
            For i = 0 To m_NumValues - 1
                Me.Indices(i) = r.ReadUInt16
            Next

            Me.UnkValues = New MorphIndexedEntryUnk(m_NumValues - 1) {}
            For i = 0 To m_NumValues - 1
                Me.UnkValues(i) = New MorphIndexedEntryUnk(r)
            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.NumValues)
            For i = 0 To Me.NumValues - 1
                w.Write(Me.Indices(i))
            Next
            For i = 0 To Me.NumValues - 1
                Me.UnkValues(i).Save(w)
            Next

        End Sub
        ''' <summary>
        ''' Returns the number of UnkValues (and Indices) (ReadOnly). </summary>
        Public ReadOnly Property NumValues As UShort
            Get
                Return If(UnkValues?.Count, 0)
            End Get
        End Property
        ''' <summary>
        ''' Gets/Sets the Indices. </summary>
        Public Property Indices As UShort()
        ''' <summary>
        ''' Gets/Sets the UnkValues. </summary>
        Public Property UnkValues As MorphIndexedEntryUnk()
    End Class

    Public Class MorphIndexedEntryUnk
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.Data = r.ReadBytes(12)
        End Sub
        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Data)
        End Sub
        ''' <summary>
        ''' Gets/Sets the Data. </summary>
        Public Property Data As Byte() = New Byte(12 - 1) {}
    End Class
End Namespace