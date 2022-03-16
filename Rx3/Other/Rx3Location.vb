Imports Microsoft.DirectX

Namespace Rx3
    Public Class Location
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.LOCATION
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
            Me.Position = New Vector3 With {
                .X = r.ReadSingle,
                .Y = r.ReadSingle,
                .Z = r.ReadSingle
                }
            Me.Rotation = New Vector3 With {
                .X = r.ReadSingle,
                .Y = r.ReadSingle,
                .Z = r.ReadSingle
                }
            Me.Unknown = r.ReadUInt32

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position

            w.Write(Me.TotalSize)
            w.Write(Me.Position.X)
            w.Write(Me.Position.Y)
            w.Write(Me.Position.Z)

            w.Write(Me.Rotation.X)
            w.Write(Me.Rotation.Y)
            w.Write(Me.Rotation.Z)
            w.Write(Me.Unknown)

            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub


        Public Property TotalSize As UInteger
        Public Property Position As Vector3
        Public Property Rotation As Vector3
        Public Property Unknown As UInteger         'always 0? maybe padding

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace