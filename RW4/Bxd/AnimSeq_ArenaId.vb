'-- Present at:  'WC2010 "stadium_213_1_container_0.rx3" 'FIFA09 "festadium_188_4_container_0.rx2" (1) "stadium_29_1_container_0.rx2" (1) "stadium_29_4_container_0.rx2" (1) "stadium_175_1_container_0.rx2"  (array)  
'-- ussually this section appears 1 time, but may (rare) appear 2 times
'-- section &HEB000C contains a list of ids to sections &HEB000D (animation sequence ??)

Namespace Rw.Bxd
    Public Class AnimSeq
        'bxd::tAnimSeq
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.ANIMSEQ_ARENAID
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position

            Me.SequenceTime = r.ReadSingle
            Me.Flags = r.ReadUInt32
            Me.NumChannels = r.ReadInt32

            Me.OffsetChannelIndices = r.ReadUInt32
            Me.OffsetChannelPoses = r.ReadUInt32
            Me.OffsetChannelCurves = r.ReadUInt32

            r.BaseStream.Position = BaseOffset + Me.OffsetChannelIndices
            Me.ChannelIndices = New Integer(Me.NumChannels - 1) {}
            For i = 0 To Me.ChannelIndices.Length - 1
                Me.ChannelIndices(i) = r.ReadInt32
            Next

            r.BaseStream.Position = BaseOffset + Me.OffsetChannelPoses
            Me.ChannelPoses = New Byte(Me.NumChannels - 1)() {} 'New List(Of Byte())
            For i = 0 To Me.ChannelPoses.Length - 1
                Me.ChannelPoses(i) = r.ReadBytes(4)     'should be floats: but h"80 00 00 00" is used as "0" values (will give different values at writing)
            Next

            r.BaseStream.Position = BaseOffset + Me.OffsetChannelCurves
            Me.PChannelCurves = New ChannelCurve(Me.NumChannels - 1) {}
            For i = 0 To Me.PChannelCurves.Length - 1
                Me.PChannelCurves(i) = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), ChannelCurve)   'Values_3_SectIndex_HEB000DChannelCurve
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumChannels = Me.PChannelCurves.Length   '9
            Me.OffsetChannelIndices = 24
            Me.OffsetChannelPoses = Me.OffsetChannelIndices + (Me.NumChannels * 4) '60
            Me.OffsetChannelCurves = Me.OffsetChannelPoses + (Me.NumChannels * 4) '96


            w.Write(Me.SequenceTime)
            w.Write(Me.Flags)
            w.Write(Me.NumChannels)
            w.Write(Me.OffsetChannelIndices)

            w.Write(Me.OffsetChannelPoses)
            w.Write(Me.OffsetChannelCurves)

            For i = 0 To Me.ChannelIndices.Length - 1
                w.Write(Me.ChannelIndices(i))
            Next

            For i = 0 To Me.ChannelPoses.Length - 1
                w.Write(Me.ChannelPoses(i))
            Next

            For i = 0 To Me.PChannelCurves.Length - 1
                w.Write(Me.RwArena.Sections.IndexOf(Me.PChannelCurves(i)))
            Next

        End Sub

        Public Property SequenceTime As Single   'always 00 80 00 00
        Public Property Flags As UInteger   'always "0"
        Public Property NumChannels As Integer     'integer!, always "9"
        Public Property OffsetChannelIndices As UInteger   'always "24"
        Public Property OffsetChannelPoses As UInteger   'always "60"
        Public Property OffsetChannelCurves As UInteger   'always "96"
        Public Property ChannelIndices As Integer()   'integer!,usually "0"    
        Public Property ChannelPoses As Byte()()   'floats, but read as byte: h"80 00 00 00" not supported for readout as float  
        Public Property PChannelCurves As ChannelCurve()      'section index of ChannelCurve_ArenaId (&HEB000D)

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace