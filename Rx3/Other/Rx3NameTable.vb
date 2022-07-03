Namespace Rx3
    Public Class NameTable
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.NAME_TABLE
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
            Me.NumNames = r.ReadUInt32
            Me.Unknown_1 = r.ReadUInt32
            Me.Unknown_2 = r.ReadUInt32

            ReDim Me.Names(Me.NumNames - 1)
            For i = 0 To Me.NumNames - 1
                Me.Names(i) = New NameTableEntry
                Me.Names(i).m_Type = r.ReadUInt32
                Dim m_NameSize As UInteger = r.ReadUInt32()  'Me.Names(i).NameSize = 
                Me.Names(i).Name = FifaUtil.ReadString(r, r.BaseStream.Position, m_NameSize - 1) 'FifaUtil.ReadNullTerminatedString(r)    'fix: null-terminator can be other character (ex. 2E)
                r.ReadByte()  'null terminator
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Me.NumNames = Me.Names.Length

            w.Write(Me.TotalSize)
            w.Write(Me.NumNames)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)

            For i = 0 To Me.NumNames - 1
                w.Write(CUInt(Me.Names(i).m_Type))
                w.Write(Me.Names(i).NameSize)
                FifaUtil.WriteNullTerminatedString(w, Me.Names(i).Name)
            Next i

            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

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

        Public Function GetNameByType(ByVal m_Type As SectionHash, ByVal IndexOfType As UInteger) As String
            Dim NumFound As UInteger = 0
            For i = 0 To Me.NumNames - 1
                If Me.Names(i).m_Type = m_Type Then
                    If NumFound = IndexOfType Then
                        Return Me.Names(i).Name
                    End If
                    NumFound += 1
                End If
            Next

            Return ""
        End Function

        Public Sub SetNameByType(ByVal Value As String, ByVal m_Type As SectionHash, ByVal IndexOfType As UInteger)
            Dim NumFound As UInteger = 0
            For i = 0 To Me.NumNames - 1
                If Me.Names(i).m_Type = m_Type Then
                    If NumFound = IndexOfType Then
                        Me.Names(i).Name = Value
                        Exit Sub
                    End If
                    NumFound += 1
                End If
            Next
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
        ''' <summary>
        ''' Section type (SectionHash) the name is referring to. </summary>
        Public Property m_Type As SectionHash
        ''' <summary>
        ''' Size of the name string, including null-terminator (ReadOnly). </summary>
        Public ReadOnly Property NameSize As UInteger
            Get
                Return Name.Length + 1 'size of name string, including null-terminator
            End Get
        End Property
        Public Property Name As String
    End Class
End Namespace
