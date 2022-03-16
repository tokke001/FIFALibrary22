Namespace Rx3
    Public Class SceneAnimation
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.SCENE_ANIMATION
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
            Me.NumLocations = r.ReadUInt32
            Me.Unknown_1 = r.ReadUInt32
            Me.Unknown_2 = r.ReadUInt32
            Me.String_1 = FifaUtil.ReadNullTerminatedString(r)
            Me.Data = r.ReadBytes(Me.TotalSize - 16 - (String_1.Length - 1))    'size - values - (string.size - nullterminator)

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position

            w.Write(Me.TotalSize)
            w.Write(Me.NumLocations)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            FifaUtil.WriteNullTerminatedString(w, Me.String_1)
            w.Write(Me.Data)

            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub


        Public Property TotalSize As UInteger
        Public Property NumLocations As UInteger
        Public Property Unknown_1 As UInteger   'usually 1
        Public Property Unknown_2 As UInteger   'usually 0
        Public Property String_1 As String   'ex. "tree"
        Public Property Data As Byte()   'rest of (unknown) data

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace