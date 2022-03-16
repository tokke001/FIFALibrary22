Namespace Rw.Core.Arena
    Public Class Buffer
        'rw::Core::Arena::?
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.RWOBJECTTYPE_BUFFER
        'Public Const ALIGNMENT As Integer = 4 (vertexes/indexes) or 4096 (raster)

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Me.Data = New Byte(SectionInfo.Size - 1) {}
        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            w.Write(Me.Data)
        End Sub

        Public Property Data As Byte()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        'Public Overrides Function GetAlignment() As Integer
        '    Return ALIGNMENT
        'End Function
    End Class
End Namespace