'-- Present at: WC2010 "stadium_213_1_container_0.rx3"  'FIFA09 "festadium_188_4_container_0.rx2" "stadium_29_1_container_0.rx2" "stadium_29_4_container_0.rx2"
'-- section &HEB000C contains a list of ids to sections &HEB000D (animation sequence ??)
Namespace Rw.Bxd
    Public Class ChannelCurve
        'bxd::tChannelCurve
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.CHANNELCURVE_ARENAID
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

            Me.BeginTime = r.ReadSingle
            Me.EndTime = r.ReadSingle
            Me.CurveTime = r.ReadSingle
            Me.m_Flags = r.ReadUInt32

            Me.NumKeys = r.ReadUInt32
            Me.OffsetKeys = r.ReadUInt32    'pointer to keys

            '--keys may follow ? --> usually not found because NumKeys is always "0" ?!
            r.BaseStream.Position = BaseOffset + Me.OffsetKeys
            Me.Keys = New UInteger(Me.NumKeys - 1) {}
            For i = 0 To Me.Keys.Length - 1
                Me.Keys(i) = r.ReadUInt32
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)

            w.Write(Me.BeginTime)
            w.Write(Me.EndTime)
            w.Write(Me.CurveTime)
            w.Write(Me.m_Flags)

            w.Write(Me.NumKeys)
            w.Write(Me.OffsetKeys)

        End Sub

        Public Property BeginTime As Single   'always "0"
        Public Property EndTime As Single   'always "0"
        Public Property CurveTime As Single   'always "0"
        Public Property m_Flags As Flags   'usually "73", rarely "9"
        Public Property NumKeys As UInteger   'always "0"
        Public Property OffsetKeys As UInteger   'always "24"   
        Public Property Keys As UInteger()

        <Flags>
        Public Enum Flags As Integer    'bxd::tChannelCurve::Flags
            PREINF_CONSTANT = 1
            PREINF_CYCLE = 2
            PREINF_ADD_CYCLE = 4
            POSTINF_CONSTANT = 8
            POSTINF_CYCLE = 16
            POSTINF_ADD_CYCLE = 32
            CHANNEL_CONTINUOUS_F1 = 64
            CHANNEL_DISCRETE_F1 = 128
            CHANNEL_DISCRETE_F3 = 256
        End Enum
        Public Enum Constants As Integer    'bxd::tChannelCurve::Constants
            FPS = 30
        End Enum

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace