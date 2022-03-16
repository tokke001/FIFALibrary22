Namespace Rx3
    Public Class EdgeMesh
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.EDGE_MESH
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
            Me.Unknown_1 = r.ReadUInt32
            Me.Unknown_2 = r.ReadUInt32
            Me.Unknown_3 = r.ReadUInt32

            Me.Data = r.ReadBytes(Me.TotalSize - 16)    'size - values 

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position

            w.Write(Me.TotalSize)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            w.Write(Me.Unknown_3)

            w.Write(Me.Data)


            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub


        Public Property TotalSize As UInteger
        Public Property Unknown_1 As UInteger   '"1" value ?
        Public Property Unknown_2 As UInteger   '"2" value numsections? 
        Public Property Unknown_3 As UInteger   '"0" value padding ?
        Public Property Data As Byte()   'rest of (unknown) data

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace