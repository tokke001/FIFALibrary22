Namespace Rx3
    Public Class SceneLayer
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.SCENE_LAYER
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
            Me.m_Type = r.ReadUInt32
            Me.Unknown(0) = r.ReadUInt32
            Me.Unknown(1) = r.ReadUInt32

            Me.Name = FifaUtil.ReadNullTerminatedString(r)
            Me.SceneIndex = r.ReadUInt32

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            'Me.Num = Me..Length

            w.Write(Me.TotalSize)
            w.Write(CUInt(Me.m_Type))
            w.Write(Me.Unknown(0))
            w.Write(Me.Unknown(1))

            FifaUtil.WriteNullTerminatedString(w, Me.Name)
            w.Write(Me.SceneIndex)

            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub


        Public Property TotalSize As UInteger
        Public Property m_Type As TypeCode    '0 for collision name, 1 for regular node name
        Public Property Unknown As UInteger() = New UInteger(2 - 1) {}  'always 0? maybe padding
        Public Property Name As String
        Public Property SceneIndex As UInteger  'index of the scene/node this name corresponds to - starts with 0 (it's usually 0 if name is related to collision - might be collision part index)

        Public Enum TypeCode As UInteger
            COLLISION_NAME = 0
            REGULAR_SCENE_NAME = 1  'regular node name
        End Enum

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace