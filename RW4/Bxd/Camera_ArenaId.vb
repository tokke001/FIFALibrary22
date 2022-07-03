Namespace Rw.Bxd
    Public Class Camera        'euro08 "stadium_167_3_container_0.rx2" "stadium_167_4_container_0.rx2"
        'bxd::tCamera
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.CAMERA_ARENAID
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position

            '   0   
            Me.Intersect = r.ReadUIntegers(2)   'array of 2 uint4
            '     8
            Me.Flags = r.ReadUInt32
            Me.Padding_1 = r.ReadUInt32 'dump: skipped, unused/padding/reserved for more flags?
            ' 16  
            Me.V3Direction = r.ReadVector4     'rw::math::vpu::Vector4
            '   32  
            Me.V3Position = r.ReadVector4     'rw::math::vpu::Vector4
            '   48  
            Me.AnimSeqMap = New AnimSeqMap(r)   'to "bxd::tAnimSeqMap" (m_iNumTransforms)
            ' 52  
            Me.NumAnimSeqs = r.ReadInt32
            '   56  
            Me.OffsetAnimSeqs = r.ReadUInt32    ''pointer to pointer of bxd::tAnimSeq, value: 80 (end of section)
            '   60  
            Me.NumAttribChannels = r.ReadInt32
            '   64 
            Me.OffsetAttribChannels = r.ReadUInt32  'pointer to bxd::tCameraAttribChannel, value: 80 (end of section)

            Me.Padding_2 = r.ReadUIntegers(3)   'padding of 3 integers?  (unused at dump)

            '-- "bxd::tAnimSeq" array of size NumAnimSeqs  0x0006F96C
            r.BaseStream.Position = BaseOffset + Me.OffsetAnimSeqs
            Me.AnimSeqs = New AnimSeq(Me.NumAnimSeqs - 1) {}
            For i = 0 To Me.AnimSeqs.Length - 1
                Me.AnimSeqs(i) = New AnimSeq(Me.RwArena, r)
            Next

            '-- "bxd::tCameraAttribChannel" array of size NumAttribChannels  0x0006F96E
            r.BaseStream.Position = BaseOffset + Me.OffsetAttribChannels
            Me.AttribChannels = New CameraAttribChannel(Me.NumAttribChannels - 1) {}
            For i = 0 To Me.AttribChannels.Length - 1
                Me.AttribChannels(i) = New CameraAttribChannel(r)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumAnimSeqs = Me.AnimSeqs.Length
            Me.OffsetAnimSeqs = 80
            Me.NumAttribChannels = Me.AttribChannels.Length
            'Me.OffsetAnimSeqs =  '--> calculated 

            w.Write(Me.Intersect)
            w.Write(Me.Flags)
            w.Write(Me.Padding_1)

            w.Write(Me.V3Direction)

            w.Write(Me.V3Position)

            Me.AnimSeqMap.Save(w)

            w.Write(Me.NumAnimSeqs)
            w.Write(Me.OffsetAnimSeqs)

            w.Write(Me.NumAttribChannels)
            w.Write(Me.OffsetAttribChannels)

            w.Write(Me.Padding_2)

            For i = 0 To Me.NumAnimSeqs - 1
                Me.AnimSeqs(i).Save(w)
            Next

            Me.OffsetAttribChannels = w.BaseStream.Position - BaseOffset
            For i = 0 To Me.NumAttribChannels - 1
                Me.AttribChannels(i).Save(w)
            Next
            Dim EndOffset As Long = w.BaseStream.Position

            'rewrite correct offset
            w.BaseStream.Position = BaseOffset + 64
            w.Write(Me.OffsetAttribChannels)
            w.BaseStream.Position = EndOffset

        End Sub

        Public Property Intersect As UInteger() = New UInteger(2 - 1) {}   '"0" "0" (empty/padding?)
        Public Property Flags As UInteger   '"0"
        Public Property Padding_1 As UInteger   '"0"
        Public Property V3Direction As Microsoft.DirectX.Vector4   '"0", 80 00 00 00, "0", "1"
        Public Property V3Position As Microsoft.DirectX.Vector4   '"0", "0", "0" , "1"
        Public Property AnimSeqMap As AnimSeqMap   '"1"
        Public Property NumAnimSeqs As Integer   '"0"
        Public Property OffsetAnimSeqs As UInteger   '"80"
        Public Property NumAttribChannels As Integer   '"0"
        Public Property OffsetAttribChannels As UInteger   '"80"
        Public Property Padding_2 As UInteger() = New UInteger(3 - 1) {}   '"0"
        Public Property AnimSeqs As AnimSeq()
        Public Property AttribChannels As CameraAttribChannel()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class AnimSeqMap
        'bxd::tAnimSeqMap
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.NumTransforms = r.ReadUInt32
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.NumTransforms)
        End Sub

        Public Property NumTransforms As UInteger   '"1"

        Public Enum Channel As Integer 'bxd::tAnimSeqMap::Channel   0x6fa24
            TRANSLATE_X = 0
            TRANSLATE_Y = 1
            TRANSLATE_Z = 2
            ROTATE_X = 3
            ROTATE_Y = 4
            ROTATE_Z = 5
            SCALE_X = 6
            SCALE_Y = 7
            SCALE_Z = 8
            NUM_CHANNELS = 9
        End Enum

    End Class

    Public Class CameraAttribChannel
        'bxd::tCameraAttribChannel
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.m_Type = r.ReadInt32
            Me.PtrChannel = r.ReadInt32 'pointer (offset or index to section??) to ChannelCurve_ArenaId (&HEB000D)
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.m_Type)
            w.Write(Me.PtrChannel)
        End Sub

        Public Property m_Type As Type
        Public Property PtrChannel As Integer   ''pointer (offset or index to section??) to ChannelCurve_ArenaId (&HEB000D)

        Public Enum Type As Integer
            TYPE_NA = -1
            TYPE_CAMERA_SCALE = 0
            TYPE_FOCAL_LENGTH = 1
            TYPE_NEAR_CLIP_PLANE = 2
            TYPE_FAR_CLIP_PLANE = 3
            TYPE_HORIZONTAL_FILM_APERTURE = 4
            TYPE_VERTICAL_FILM_APERTURE = 5
            TYPE_LENS_SQUEEZE_RATIO = 6
            TYPE_FILM_FIT_OFFSET = 7
            TYPE_VERTICAL_FILM_OFFSET = 8
            TYPE_HORIZONTAL_FILM_OFFSET = 9
            TYPE_PRE_SCALE = 10
            TYPE_HORIZONTAL_FILM_TRANSLATE = 11
            TYPE_VERTICAL_FILM_TRANSLATE = 12
            TYPE_HORIZONTAL_ROLL_PIVOT = 13
            TYPE_VERTICAL_ROLL_PIVOT = 14
            TYPE_FILM_ROLL_VALUE = 15
            TYPE_POST_SCALE = 16
            TYPE_FOCUS_DISTANCE = 17
            TYPE_FSTOP = 18
            TYPE_FOCUS_REGION_SCALE = 19
            TYPE_THRESHOLD = 20
            TYPE_BACKGROUND_COLOR_R = 21
            TYPE_BACKGROUND_COLOR_G = 22
            TYPE_BACKGROUND_COLOR_B = 23
            TYPE_SHUTTER_ANGLE = 24
            TYPE_OVER_SCAN = 25
            TYPE_CENTER_OF_INTEREST = 26
            TYPE_TUMBLE_PIVOT_X = 27
            TYPE_TUMBLE_PIVOT_Y = 28
            TYPE_TUMBLE_PIVOT_Z = 29
            TYPE_ORTHOGRAPHIC_WIDTH = 30
            TYPE_GHOST_PRE_STEPS = 31
            TYPE_GHOST_POST_STEPS = 32
            TYPE_GHOST_STEP_SIZE = 33
            TYPE_GHOST_RANGE_START = 34
            TYPE_GHOST_RANGE_END = 35
            TYPE_APERTURE_0 = 36
            TYPE_APERTURE_1 = 37
            TYPE_DEPTH_OF_FIELD_0 = 38
            TYPE_DEPTH_OF_FIELD_1 = 39
            TYPE_FRAME_EXTENSION = 40
        End Enum

    End Class
End Namespace