Namespace Rx3
    Public Class IndexBufferHeader
        ' Methods
        Public Sub New()

        End Sub

        Public Sub New(ByRef IndexData As List(Of UInteger))
            Me.m_IndexData = IndexData
        End Sub

        Public Sub New(ByVal r As FileReader, ByRef IndexData As List(Of UInteger))
            Me.m_IndexData = IndexData
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.TotalSize = r.ReadUInt32
            Me.NumIndices_FileLoad = r.ReadUInt32
            Me.IndexStride_FileLoad = r.ReadByte
            Me.Pad = r.ReadBytes(7)
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.TotalSize)
            w.Write(Me.NumIndices)
            w.Write(Me.IndexStride)
            w.Write(Me.Pad)
        End Sub

        Private m_IndexData As List(Of UInteger)
        Private m_TotalSize As UInteger
        Private _NumIndices_FileLoad As UInteger
        Private _IndexStride_FileLoad As Byte = 2

        Friend Property NumIndices_FileLoad As UInteger
            Get
                Return _NumIndices_FileLoad
            End Get
            Private Set
                _NumIndices_FileLoad = Value
            End Set
        End Property

        Friend Property IndexStride_FileLoad As Byte
            Get
                Return _IndexStride_FileLoad
            End Get
            Private Set
                _IndexStride_FileLoad = Value
            End Set
        End Property

        ' Fields
        ''' <summary>
        ''' Total section size (ReadOnly). </summary>
        Public Property TotalSize As UInteger
            Get
                Return m_TotalSize
            End Get
            Friend Set
                m_TotalSize = Value
            End Set
        End Property
        ''' <summary>
        ''' Returns the number of Indices (ReadOnly). </summary>
        Public ReadOnly Property NumIndices As UInteger
            Get
                Return If(m_IndexData?.Count, Me.NumIndices_FileLoad)
            End Get
        End Property
        ''' <summary>
        ''' Size of 1 index (ReadOnly). </summary>
        Public ReadOnly Property IndexStride As Byte
            Get
                Return If(m_IndexData IsNot Nothing, FifaUtil.GetIndexStride(m_IndexData), Me.IndexStride_FileLoad)
            End Get
        End Property
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Pad As Byte() = New Byte(7 - 1) {}

    End Class

End Namespace