Namespace Rx3
    Public Class NameTable
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.NAME_TABLE
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Me.NumNames = r.ReadUInt32
            Me.Unknown_1 = r.ReadUInt32
            Me.Unknown_2 = r.ReadUInt32

            ReDim Me.Names(Me.NumNames - 1)
            For i = 0 To Me.NumNames - 1
                Me.Names(i) = New NameTableEntry
                Me.Names(i).m_Type = r.ReadUInt32
                Me.Names(i).NameSize = r.ReadUInt32
                Me.Names(i).Name = FifaUtil.ReadString(r, r.BaseStream.Position, Me.Names(i).NameSize - 1) 'FifaUtil.ReadNullTerminatedString(r)    'fix: null-terminator can be other character (ex. 2E)
                r.ReadByte()  'null terminator
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumNames = Me.Names.Length

            w.Write(Me.TotalSize)
            w.Write(Me.NumNames)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)

            For i = 0 To Me.NumNames - 1
                Me.Names(i).NameSize = Me.Names(i).Name.Length + 1  'size of name string, including null-terminator

                w.Write(CUInt(Me.Names(i).m_Type))
                w.Write(Me.Names(i).NameSize)
                FifaUtil.WriteNullTerminatedString(w, Me.Names(i).Name)
            Next i

            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub

        Public Function GetNameStrings() As String()
            Dim ReturnStr As String() = New String(Me.NumNames - 1) {}

            For i = 0 To Me.NumNames - 1
                ReturnStr(i) = Me.Names(i).Name
            Next

            Return ReturnStr
        End Function

        Public Function GetTypes() As UInteger()
            Dim ReturnInt As UInteger() = New UInteger(Me.NumNames - 1) {}

            For i = 0 To Me.NumNames - 1
                ReturnInt(i) = Me.Names(i).m_Type
            Next

            Return ReturnInt
        End Function

        Public Function GetNameTable() As List(Of FIFALibrary22.NameTable)
            Dim ReturnTable As New List(Of FIFALibrary22.NameTable) '= New FIFALibrary22.NameTable(Me.NumNames - 1) {}

            For i = 0 To Me.NumNames - 1
                ReturnTable.Add(New FIFALibrary22.NameTable With {
                    .m_Type = Me.Names(i).m_Type,
                    .m_Name = Me.Names(i).Name
                })
            Next

            Return ReturnTable
        End Function
        Public Property TotalSize As UInteger
        Public Property NumNames As UInteger
        Public Property Unknown_1 As UInteger
        Public Property Unknown_2 As UInteger
        Public Property Names As NameTableEntry()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class NameTableEntry
        Public Property m_Type As SectionHash
        Public Property NameSize As UInteger
        Public Property Name As String
    End Class
End Namespace
