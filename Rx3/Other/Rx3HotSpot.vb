Namespace Rx3
    Public Class HotSpot
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.HOTSPOT
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

            Me.Unknown_1 = r.ReadByte
            Dim m_NumAreaDatas As Byte = r.ReadByte
            Me.Unknown_2(0) = r.ReadByte
            Me.Unknown_2(1) = r.ReadByte

            Me.Unknown_3(0) = r.ReadUInt32
            Me.Unknown_3(1) = r.ReadUInt32

            ReDim Me.AreaDatas(m_NumAreaDatas - 1)
            For i = 0 To m_NumAreaDatas - 1
                Me.AreaDatas(i) = New HotSpotAreaData(r)
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.TotalSize)

            w.Write(Me.Unknown_1)
            w.Write(Me.NumAreaDatas)
            w.Write(Me.Unknown_2(0))
            w.Write(Me.Unknown_2(1))

            w.Write(Me.Unknown_3(0))
            w.Write(Me.Unknown_3(1))

            For i = 0 To Me.NumAreaDatas - 1
                Me.AreaDatas(i).Save(w)
            Next i

            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub

        Public Function ToRw4Hotspot(ByVal RwArena As Rw.Core.Arena.Arena) As Rw.EA.HotSpot
            Dim m_Rw4Hotspot As New Rw.EA.HotSpot(RwArena)

            m_Rw4Hotspot.AreaData = New Rw.EA.HotSpotAreaData(Me.AreaDatas.Length - 1) {}
            For i = 0 To m_Rw4Hotspot.AreaData.Length - 1
                m_Rw4Hotspot.AreaData(i) = New Rw.EA.HotSpotAreaData
                m_Rw4Hotspot.AreaData(i).AreaName = Me.AreaDatas(i).AreaName
                m_Rw4Hotspot.AreaData(i).HotSpots = New Rw.EA.HotSpotData(Me.AreaDatas(i).HotSpots.Length - 1) {}

                For j = 0 To m_Rw4Hotspot.AreaData(i).HotSpots.Length - 1
                    m_Rw4Hotspot.AreaData(i).HotSpots(j) = New Rw.EA.HotSpotData With {
                        .HotSpotName = Me.AreaDatas(i).HotSpots(j).HotSpotName,
                        .HotspotRectangle = Me.AreaDatas(i).HotSpots(j).HotspotRectangle}
                Next
            Next

            Return m_Rw4Hotspot
        End Function

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
        Public Property Unknown_1 As Byte = 1   'always 1 value
        ''' <summary>
        ''' Returns the number of AreaDatas (ReadOnly). </summary>
        Public ReadOnly Property NumAreaDatas As Byte
            Get
                Return If(AreaDatas?.Count, 0)
            End Get
        End Property
        Public Property Unknown_2 As Byte() = New Byte(2 - 1) {}            'maybe padding (0)
        Public Property Unknown_3 As UInteger() = New UInteger(2 - 1) {}    'maybe padding (0)
        ''' <summary>
        ''' Gets/Sets the AreaDatas. </summary>
        Public Property AreaDatas As HotSpotAreaData()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
    Public Class HotSpotAreaData
        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.AreaName = FifaUtil.ReadNullTerminatedString(r)
            Dim m_NumHotSpots As Byte = r.ReadByte

            ReDim Me.HotSpots(m_NumHotSpots - 1)
            For h = 0 To m_NumHotSpots - 1
                Me.HotSpots(h) = New HotSpotData(r)
            Next h

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            FifaUtil.WriteNullTerminatedString(w, Me.AreaName)
            w.Write(Me.NumHotSpots)

            For h = 0 To Me.NumHotSpots - 1
                Me.HotSpots(h).Save(w)
            Next h

        End Sub

        ''' <summary>
        ''' Name of the Area. </summary>
        Public Property AreaName As String
        ''' <summary>
        ''' Returns the number of HotSpots (ReadOnly). </summary>
        Public ReadOnly Property NumHotSpots As Byte
            Get
                Return If(HotSpots?.Count, 0)
            End Get
        End Property
        ''' <summary>
        ''' Gets/Sets the HotSpots. </summary>
        Public Property HotSpots As HotSpotData()

    End Class

    Public Class HotSpotData
        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.HotSpotName = FifaUtil.ReadNullTerminatedString(r)
            Me.HotspotRectangle(0) = r.ReadSingle
            Me.HotspotRectangle(1) = r.ReadSingle
            Me.HotspotRectangle(2) = r.ReadSingle
            Me.HotspotRectangle(3) = r.ReadSingle
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            FifaUtil.WriteNullTerminatedString(w, Me.HotSpotName)
            w.Write(Me.HotspotRectangle(0))
            w.Write(Me.HotspotRectangle(1))
            w.Write(Me.HotspotRectangle(2))
            w.Write(Me.HotspotRectangle(3))
        End Sub
        ''' <summary>
        ''' Name of the HotSpot. </summary>
        Public Property HotSpotName As String
        ''' <summary>
        ''' Gets/Sets the Hotspot values. </summary>
        Public Property HotspotRectangle As Single() = New Single(4 - 1) {}

    End Class
End Namespace