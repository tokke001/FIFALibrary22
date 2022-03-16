Namespace Rw.EA
    Public Class HotSpot
        'EA::?? --> not found at dump
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.EA_HOTSPOT
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.Unknown_1 = r.ReadByte
            Me.NumAreas = r.ReadByte
            Me.Unknown_2(0) = r.ReadByte
            Me.Unknown_2(1) = r.ReadByte

            ReDim Me.AreaData(Me.NumAreas - 1)
            For i = 0 To Me.NumAreas - 1
                Me.AreaData(i) = New HotSpotAreaData

                Me.AreaData(i).PointerOffsetAreaName = r.ReadUInt32

                Me.AreaData(i).NumHotSpots = r.ReadByte
                Me.AreaData(i).Unknown = r.ReadBytes(3)

                Me.AreaData(i).PointerOffsetHotSpotNames = r.ReadUInt32
                Me.AreaData(i).PointerHotspotRectangles = r.ReadUInt32
            Next i

            For i = 0 To Me.NumAreas - 1
                'get AreaName
                r.BaseStream.Position = Me.AreaData(i).PointerOffsetAreaName
                Me.AreaData(i).OffsetAreaName = r.ReadUInt32
                r.BaseStream.Position = Me.AreaData(i).OffsetAreaName
                Me.AreaData(i).AreaName = FifaUtil.ReadNullTerminatedString(r)

                ReDim Me.AreaData(i).HotSpots(Me.AreaData(i).NumHotSpots - 1)
                'create list & get ofssets HotSpot names
                r.BaseStream.Position = Me.AreaData(i).PointerOffsetHotSpotNames
                For h = 0 To Me.AreaData(i).NumHotSpots - 1
                    Me.AreaData(i).HotSpots(h) = New HotSpotData
                    Me.AreaData(i).HotSpots(h).OffsetHotSpotName = r.ReadUInt32
                Next h

                'get HotSpot names
                For h = 0 To Me.AreaData(i).NumHotSpots - 1
                    r.BaseStream.Position = Me.AreaData(i).HotSpots(h).OffsetHotSpotName
                    Me.AreaData(i).HotSpots(h).HotSpotName = FifaUtil.ReadNullTerminatedString(r)
                Next h

                'get Hotspot Rectangles
                r.BaseStream.Position = Me.AreaData(i).PointerHotspotRectangles
                For h = 0 To Me.AreaData(i).NumHotSpots - 1
                    Me.AreaData(i).HotSpots(h).HotspotRectangle(0) = r.ReadSingle
                    Me.AreaData(i).HotSpots(h).HotspotRectangle(1) = r.ReadSingle
                    Me.AreaData(i).HotSpots(h).HotspotRectangle(2) = r.ReadSingle
                    Me.AreaData(i).HotSpots(h).HotspotRectangle(3) = r.ReadSingle

                Next h

            Next i

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumAreas = Me.AreaData.Length

            w.Write(Me.Unknown_1)
            w.Write(Me.NumAreas)
            w.Write(Me.Unknown_2(0))
            w.Write(Me.Unknown_2(1))


            Dim BaseOffset As Long = w.BaseStream.Position
            '----write with wrong offsets

            For i = 0 To Me.NumAreas - 1
                Me.AreaData(i).NumHotSpots = Me.AreaData(i).HotSpots.Length

                w.Write(Me.AreaData(i).PointerOffsetAreaName)

                w.Write(Me.AreaData(i).NumHotSpots)
                w.Write(Me.AreaData(i).Unknown)

                w.Write(Me.AreaData(i).PointerOffsetHotSpotNames)
                w.Write(Me.AreaData(i).PointerHotspotRectangles)

            Next i

            'offsets to names
            For i = 0 To Me.NumAreas - 1
                Me.AreaData(i).PointerOffsetAreaName = w.BaseStream.Position
                w.Write(Me.AreaData(i).OffsetAreaName)

                Me.AreaData(i).PointerOffsetHotSpotNames = w.BaseStream.Position
                For h = 0 To Me.AreaData(i).NumHotSpots - 1
                    w.Write(Me.AreaData(i).HotSpots(h).OffsetHotSpotName)
                Next
            Next

            'names
            For i = 0 To Me.NumAreas - 1
                Me.AreaData(i).OffsetAreaName = w.BaseStream.Position
                FifaUtil.WriteNullTerminatedString(w, Me.AreaData(i).AreaName)

                For h = 0 To Me.AreaData(i).NumHotSpots - 1
                    Me.AreaData(i).HotSpots(h).OffsetHotSpotName = w.BaseStream.Position
                    FifaUtil.WriteNullTerminatedString(w, Me.AreaData(i).HotSpots(h).HotSpotName)
                Next
            Next

            'padding    10
            For i = 1 To 10
                w.Write(CByte(0))
            Next i

            'HotspotRectangles
            For i = 0 To Me.NumAreas - 1
                Me.AreaData(i).PointerHotspotRectangles = w.BaseStream.Position

                For h = 0 To Me.AreaData(i).NumHotSpots - 1
                    w.Write(Me.AreaData(i).HotSpots(h).HotspotRectangle(0))
                    w.Write(Me.AreaData(i).HotSpots(h).HotspotRectangle(1))
                    w.Write(Me.AreaData(i).HotSpots(h).HotspotRectangle(2))
                    w.Write(Me.AreaData(i).HotSpots(h).HotspotRectangle(3))
                Next
            Next


            Dim EndOffset As Long = w.BaseStream.Position

            '----write again with correct offsets
            w.BaseStream.Position = BaseOffset

            For i = 0 To Me.NumAreas - 1
                w.Write(Me.AreaData(i).PointerOffsetAreaName)

                w.Write(Me.AreaData(i).NumHotSpots)
                w.Write(Me.AreaData(i).Unknown)

                w.Write(Me.AreaData(i).PointerOffsetHotSpotNames)
                w.Write(Me.AreaData(i).PointerHotspotRectangles)
            Next i

            'offsets to names
            For i = 0 To Me.NumAreas - 1
                w.Write(Me.AreaData(i).OffsetAreaName)

                For h = 0 To Me.AreaData(i).NumHotSpots - 1
                    w.Write(Me.AreaData(i).HotSpots(h).OffsetHotSpotName)
                Next
            Next

            '-----go to end offset
            w.BaseStream.Position = EndOffset

        End Sub


        Public Property Unknown_1 As Byte
        Public Property NumAreas As Byte
        Public Property Unknown_2 As Byte() = New Byte(2 - 1) {}
        Public Property AreaData As HotSpotAreaData()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class HotSpotAreaData

        Public Property PointerOffsetAreaName As UInteger
        Public Property OffsetAreaName As UInteger
        Public Property NumHotSpots As Byte
        Public Property Unknown As Byte() = New Byte(3 - 1) {}
        Public Property PointerOffsetHotSpotNames As UInteger
        Public Property PointerHotspotRectangles As UInteger
        Public Property AreaName As String
        Public Property HotSpots As HotSpotData()

    End Class

    Public Class HotSpotData
        Public Property HotSpotName As String
        Public Property OffsetHotSpotName As UInteger
        Public Property HotspotRectangle As Single() = New Single(4 - 1) {}

    End Class

End Namespace