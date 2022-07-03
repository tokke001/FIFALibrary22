Namespace Rw.Bxd
    Public Class Location
        'bxd::tLocation
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.LOCATION_ARENAID
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.Intersect = r.ReadUIntegers(4) ' New UInteger(4 - 1) {}   'padding ?

            Me.V3Translation = r.ReadVector4
            Me.V3RadianEulerRot = r.ReadVector4    'rotation?
            Me.V3OBBTranslation = r.ReadVector4
            Me.V3OBBRadianEulerRot = r.ReadVector4 'rotation?
            Me.V3OBBScale = r.ReadVector4

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)

            w.Write(Me.Intersect)

            w.Write(Me.V3Translation)
            w.Write(Me.V3RadianEulerRot)
            w.Write(Me.V3OBBTranslation)
            w.Write(Me.V3OBBRadianEulerRot)
            w.Write(Me.V3OBBScale)

        End Sub

        Public Property Intersect As UInteger() = New UInteger(4 - 1) {}       'always "0", tested !
        Public Property V3Translation As Microsoft.DirectX.Vector4
        Public Property V3RadianEulerRot As Microsoft.DirectX.Vector4
        Public Property V3OBBTranslation As Microsoft.DirectX.Vector4
        Public Property V3OBBRadianEulerRot As Microsoft.DirectX.Vector4
        Public Property V3OBBScale As Microsoft.DirectX.Vector4

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace