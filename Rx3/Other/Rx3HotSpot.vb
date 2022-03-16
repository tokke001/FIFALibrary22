Namespace Rx3
    Public Class HotSpot
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.HOTSPOT
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

            Me.Unknown_1 = r.ReadByte
            Me.NumAreas = r.ReadByte
            Me.Unknown_2(0) = r.ReadByte
            Me.Unknown_2(1) = r.ReadByte

            Me.Unknown_3(0) = r.ReadUInt32
            Me.Unknown_3(1) = r.ReadUInt32

            ReDim Me.AreaDatas(Me.NumAreas - 1)
            For i = 0 To Me.NumAreas - 1
                Me.AreaDatas(i) = New HotSpotAreaData
                Me.AreaDatas(i).AreaName = FifaUtil.ReadNullTerminatedString(r)
                Me.AreaDatas(i).NumHotSpots = r.ReadByte

                ReDim Me.AreaDatas(i).HotSpots(Me.AreaDatas(i).NumHotSpots - 1)
                For h = 0 To Me.AreaDatas(i).NumHotSpots - 1
                    Me.AreaDatas(i).HotSpots(h) = New HotSpotData
                    Me.AreaDatas(i).HotSpots(h).HotSpotName = FifaUtil.ReadNullTerminatedString(r)
                    Me.AreaDatas(i).HotSpots(h).HotspotRectangle(0) = r.ReadSingle
                    Me.AreaDatas(i).HotSpots(h).HotspotRectangle(1) = r.ReadSingle
                    Me.AreaDatas(i).HotSpots(h).HotspotRectangle(2) = r.ReadSingle
                    Me.AreaDatas(i).HotSpots(h).HotspotRectangle(3) = r.ReadSingle

                Next h

            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumAreas = Me.AreaDatas.Length


            w.Write(Me.TotalSize)

            w.Write(Me.Unknown_1)
            w.Write(Me.NumAreas)
            w.Write(Me.Unknown_2(0))
            w.Write(Me.Unknown_2(1))

            w.Write(Me.Unknown_3(0))
            w.Write(Me.Unknown_3(1))

            For i = 0 To Me.NumAreas - 1
                Me.AreaDatas(i).NumHotSpots = Me.AreaDatas(i).HotSpots.Length

                FifaUtil.WriteNullTerminatedString(w, Me.AreaDatas(i).AreaName)
                w.Write(Me.AreaDatas(i).NumHotSpots)

                For h = 0 To Me.AreaDatas(i).NumHotSpots - 1
                    FifaUtil.WriteNullTerminatedString(w, Me.AreaDatas(i).HotSpots(h).HotSpotName)
                    w.Write(Me.AreaDatas(i).HotSpots(h).HotspotRectangle(0))
                    w.Write(Me.AreaDatas(i).HotSpots(h).HotspotRectangle(1))
                    w.Write(Me.AreaDatas(i).HotSpots(h).HotspotRectangle(2))
                    w.Write(Me.AreaDatas(i).HotSpots(h).HotspotRectangle(3))

                Next h

            Next i


            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub


        Public Property TotalSize As UInteger
        Public Property Unknown_1 As Byte   'always 1 value
        Public Property NumAreas As Byte
        Public Property Unknown_2 As Byte() = New Byte(2 - 1) {}            'maybe padding (0)
        Public Property Unknown_3 As UInteger() = New UInteger(2 - 1) {}    'maybe padding (0)
        Public Property AreaDatas As HotSpotAreaData()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
    Public Class HotSpotAreaData
        Public Property AreaName As String
        Public Property NumHotSpots As Byte
        Public Property HotSpots As HotSpotData()

    End Class

    Public Class HotSpotData
        Public Property HotSpotName As String
        Public Property HotspotRectangle As Single() = New Single(4 - 1) {}

    End Class
End Namespace