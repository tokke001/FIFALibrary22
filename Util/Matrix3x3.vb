
Imports Microsoft.DirectX

Public Class Matrix3x3
    ' Methods
    Public Sub New(ByVal c00 As Integer, ByVal c01 As Integer, ByVal c02 As Integer, ByVal c10 As Integer, ByVal c11 As Integer, ByVal c12 As Integer, ByVal c20 As Integer, ByVal c21 As Integer, ByVal c22 As Integer)
        Me.a = c00
        Me.b = c01
        Me.c = c02
        Me.d = c10
        Me.e = c11
        Me.f = c12
        Me.g = c20
        Me.h = c21
        Me.i = c22
    End Sub

    Public Sub New(ByVal c00 As Single, ByVal c01 As Single, ByVal c02 As Single, ByVal c10 As Single, ByVal c11 As Single, ByVal c12 As Single, ByVal c20 As Single, ByVal c21 As Single, ByVal c22 As Single)
        Me.a = c00
        Me.b = c01
        Me.c = c02
        Me.d = c10
        Me.e = c11
        Me.f = c12
        Me.g = c20
        Me.h = c21
        Me.i = c22
    End Sub

    Public Function ComputeDeterminant() As Single
        Me.m_Determinant = (((Me.a * ((Me.e * Me.i) - (Me.f * Me.h))) - (Me.b * ((Me.i * Me.d) - (Me.f * Me.g)))) + (Me.c * ((Me.d * Me.h) - (Me.e * Me.g))))
        Return Me.m_Determinant
    End Function

    Public Function Invert() As Matrix3x3
        Me.ComputeDeterminant()

        If (Me.m_Determinant <> 0!) Then
            Dim num As Single = (((Me.f * Me.g) - (Me.d * Me.i)) / Me.m_Determinant)
            Dim num2 As Single = (((Me.d * Me.h) - (Me.e * Me.g)) / Me.m_Determinant)
            Dim num3 As Single = (((Me.c * Me.h) - (Me.b * Me.i)) / Me.m_Determinant)
            Dim num4 As Single = (((Me.a * Me.i) - (Me.c * Me.g)) / Me.m_Determinant)
            Dim num5 As Single = (((Me.b * Me.g) - (Me.a * Me.h)) / Me.m_Determinant)
            Dim num6 As Single = (((Me.b * Me.f) - (Me.c * Me.e)) / Me.m_Determinant)
            Dim num7 As Single = (((Me.c * Me.d) - (Me.a * Me.f)) / Me.m_Determinant)
            Return New Matrix3x3((((Me.e * Me.i) - (Me.f * Me.h)) / Me.m_Determinant), num, num2, num3, num4, num5, num6, num7, (((Me.a * Me.e) - (Me.b * Me.d)) / Me.m_Determinant))
        End If
        Return Nothing
    End Function

    Public Function PostMultiply(ByVal v As Vector3) As Vector3
        Dim num As Single = (((Me.d * v.X) + (Me.e * v.Y)) + (Me.f * v.Z))
        Return New Vector3((((Me.a * v.X) + (Me.b * v.Y)) + (Me.c * v.Z)), num, (((Me.g * v.X) + (Me.h * v.Y)) + (Me.i * v.Z)))
    End Function

    Public Function PreMultiply(ByVal v As Vector3) As Vector3
        Dim num As Single = (((Me.b * v.X) + (Me.e * v.Y)) + (Me.h * v.Z))
        Return New Vector3((((Me.a * v.X) + (Me.d * v.Y)) + (Me.g * v.Z)), num, (((Me.c * v.X) + (Me.f * v.Y)) + (Me.i * v.Z)))
    End Function


    ' Properties
    Public ReadOnly Property Determinant As Single
        Get
            Return Me.m_Determinant
        End Get
    End Property


    ' Fields
    Private a As Single
    Private b As Single
    Private c As Single
    Private d As Single
    Private e As Single
    Private f As Single
    Private g As Single
    Private h As Single
    Private i As Single
    Private m_Determinant As Single
End Class


