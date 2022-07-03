Namespace Rx3
    Public Class Adjacency
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.ADJACENCY
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
            Me.Unknown_2 = r.ReadUInt32
            Me.Unknown_3 = r.ReadUInt32

            Dim numAdjacencies As Long = (Me.TotalSize - 16) \ (4 * 16)
            Me.AdjacencyInfos = New AdjacencyInfo(numAdjacencies - 1) {}
            For i = 0 To numAdjacencies - 1
                Me.AdjacencyInfos(i) = New AdjacencyInfo
                Me.AdjacencyInfos(i).NumUsed = r.ReadUInt32
                For j = 0 To 15 - 1
                    Me.AdjacencyInfos(i).Data(j) = r.ReadUInt32
                Next j
            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.TotalSize)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            w.Write(Me.Unknown_3)

            For i = 0 To Me.AdjacencyInfos.Length - 1
                w.Write(Me.AdjacencyInfos(i).NumUsed)
                For j = 0 To 15 - 1
                    w.Write(Me.AdjacencyInfos(i).Data(j))
                Next j
            Next i

            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)  'not needed, always oke

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
        Public Property Unknown_1 As UInteger   'usually 0 (padding ?)
        Public Property Unknown_2 As UInteger   'usually 0 (padding ?)
        Public Property Unknown_3 As UInteger   'usually 0 (padding ?)
        Public Property AdjacencyInfos As AdjacencyInfo()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function

    End Class

    Public Class AdjacencyInfo
        Public Property NumUsed As UInteger
        Public Property Data As UInteger() = New UInteger(15 - 1) {}
    End Class

End Namespace