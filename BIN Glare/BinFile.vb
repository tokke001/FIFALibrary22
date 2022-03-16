Imports Microsoft.DirectX

Namespace GlareBin
    'GlareData (dump nhl)
    'nhl, old FIFA08, ... - glare.bin files
    Public Class BinFile
        Public Function Load(ByVal FileName As String) As Boolean
            Dim f As New FileStream(FileName, FileMode.Open, FileAccess.ReadWrite)
            Dim r As New FileReader(f, Endian.Big)
            Dim flag As Boolean = Me.Load(r)
            f.Close()
            r.Close()
            Return flag
        End Function
        Public Function Load(ByVal fifaFile As FifaFile) As Boolean
            If fifaFile.IsCompressed Then
                fifaFile.Decompress()
            End If
            Dim r As FileReader = fifaFile.GetReader
            r.Endianness = Endian.Big
            Dim flag As Boolean = Me.Load(r)
            fifaFile.ReleaseReader(r)

            Return flag
        End Function
        Public Overridable Function Load(ByVal r As FileReader) As Boolean

            Me.Version = r.ReadInt32
            Me.Id = r.ReadInt32
            Me.Texture = FifaUtil.Read64SizedString(r)
            Me.StartSize = r.ReadSingle
            Me.EndSize = r.ReadSingle
            Me.Quantity = r.ReadInt32
            Me.Offset = r.ReadSingle
            Me.StartColor = r.ReadVector4
            Me.EndColor = r.ReadVector4
            Me.Sensitivity = r.ReadSingle
            Me.BloomScale = r.ReadSingle
            Me.ZTest = r.ReadUInt32
            Me.Transform = New Matrix4x4(r)
            Me.OffsetStart = r.ReadSingle
            Me.OffsetScale = r.ReadSingle
            Me.EdgeSensitivityStart = r.ReadSingle
            Me.EdgeSensitivityEnd = r.ReadSingle
            Me.OcclusionSensitivity = r.ReadSingle
            Me.OcclusionOffsetX = r.ReadSingle
            Me.OcclusionOffsetY = r.ReadSingle
            Me.OcclusionOffsetZ = r.ReadSingle
            Me.XRot = r.ReadSingle
            Me.YRot = r.ReadSingle
            Me.ZRot = r.ReadSingle
            Me.StartSizeRandomness = r.ReadSingle
            Me.EndSizeRandomness = r.ReadSingle
            Me.QuantityRandomness = r.ReadSingle
            Me.OffsetRandomness = r.ReadSingle
            Me.StartColorRandomness = r.ReadSingle
            Me.EndColorRandomness = r.ReadSingle
            Me.OpacityRandomness = r.ReadSingle
            Me.SensitivityRandomness = r.ReadSingle
            Me.EdgeSensitivityRandomness = r.ReadSingle
            Me.BloomScaleRandomness = r.ReadSingle
            Me.Seed = r.ReadInt32
            Me.BlendType = r.ReadInt32
            Me.Rotation = r.ReadSingle
            Me.Group = r.ReadInt32
            Me.RotationRandomness = r.ReadSingle
            Me.OcclusionQueryOverride = r.ReadUInt32
            Me.RotationRate = r.ReadSingle
            Me.Xscale = r.ReadSingle
            Me.Yscale = r.ReadSingle
            Me.FutureExpansion = r.ReadIntegers(9)

            Return True
        End Function

        'Public Function ToXml(ByVal FileName As String, Optional Bin_RepetitionPools As RepetitionPoolsFile = Nothing, Optional Bin_Eventsytem As EventSystemFile = Nothing) As String



        '    Return ""
        'End Function


        Public Property Version As Integer
        Public Property Id As Integer
        Public Property Texture As String   'chars of 64 size
        Public Property StartSize As Single
        Public Property EndSize As Single
        Public Property Quantity As Integer
        Public Property Offset As Single
        Public Property StartColor As Vector4   'rw::math::fpu::Vector4Template<float>
        Public Property EndColor As Vector4     'rw::math::fpu::Vector4Template<float>
        Public Property Sensitivity As Single
        Public Property BloomScale As Single
        Public Property ZTest As UInteger
        Public Property Transform As Matrix4x4    'rw::math::fpu::Matrix44Template<float>
        Public Property OffsetStart As Single
        Public Property OffsetScale As Single
        Public Property EdgeSensitivityStart As Single
        Public Property EdgeSensitivityEnd As Single
        Public Property OcclusionSensitivity As Single
        Public Property OcclusionOffsetX As Single
        Public Property OcclusionOffsetY As Single
        Public Property OcclusionOffsetZ As Single
        Public Property XRot As Single
        Public Property YRot As Single
        Public Property ZRot As Single
        Public Property StartSizeRandomness As Single
        Public Property EndSizeRandomness As Single
        Public Property QuantityRandomness As Single
        Public Property OffsetRandomness As Single
        Public Property StartColorRandomness As Single
        Public Property EndColorRandomness As Single
        Public Property OpacityRandomness As Single
        Public Property SensitivityRandomness As Single
        Public Property EdgeSensitivityRandomness As Single
        Public Property BloomScaleRandomness As Single
        Public Property Seed As Integer
        Public Property BlendType As Integer
        Public Property Rotation As Single
        Public Property Group As Integer
        Public Property RotationRandomness As Single
        Public Property OcclusionQueryOverride As UInteger
        Public Property RotationRate As Single
        Public Property Xscale As Single
        Public Property Yscale As Single
        Public Property FutureExpansion As Integer() = New Integer(9 - 1) {}

    End Class
End Namespace