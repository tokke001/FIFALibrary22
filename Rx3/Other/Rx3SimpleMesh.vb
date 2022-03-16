Namespace Rx3
    Public Class SimpleMesh
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.SIMPLE_MESH
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.PrimitiveType = r.ReadUInt16
            Me.Unknown_1 = r.ReadUInt16
            Me.Unknown_2 = r.ReadUInt16
            Me.Unknown_3 = r.ReadUInt16

            Me.Padding(0) = r.ReadUInt32
            Me.Padding(1) = r.ReadUInt32

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(CUShort(Me.PrimitiveType))
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            w.Write(Me.Unknown_3)

            w.Write(Me.Padding(0))
            w.Write(Me.Padding(1))

            FifaUtil.WriteAlignment(w, ALIGNMENT)
        End Sub


        Public Property PrimitiveType As Microsoft.DirectX.Direct3D.PrimitiveType
        Public Property Unknown_1 As UShort
        Public Property Unknown_2 As UShort
        Public Property Unknown_3 As UShort
        Public Property Padding As UInteger() = New UInteger(2 - 1) {}

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace