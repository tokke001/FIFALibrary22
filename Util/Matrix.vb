Imports Microsoft.DirectX

Public Class Matrix
    Public m()() As Single = {New Single(2) {}, New Single(2) {}, New Single(2) {}}

    ' user should use getIdentity instead;
    Private Sub New()
    End Sub

    Public Sub New(ByVal other As Matrix)
        copy(other)
    End Sub

    Public Shared ReadOnly Property Identity As Matrix
        Get
            Dim m_matrix As New Matrix()
            m_matrix.m(0)(0) = 1.0F
            m_matrix.m(1)(1) = 1.0F
            m_matrix.m(2)(2) = 1.0F
            Return m_matrix
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return "[[" & m(0)(0) & ", " & m(0)(1) & ", " & m(0)(2) & "]," & vbLf & "[" & m(1)(0) & ", " & m(1)(1) & ", " & m(1)(2) & "]," & vbLf & "[" & m(2)(0) & ", " & m(2)(1) & ", " & m(2)(2) & "]]"
    End Function

    Public Overridable Function [Get](ByVal row As Integer, ByVal column As Integer) As Single
        Return m(row)(column)
    End Function

    Public Overridable Sub copy(ByVal other As Matrix)
        m(0)(0) = other.m(0)(0)
        m(0)(1) = other.m(0)(1)
        m(0)(2) = other.m(0)(2)
        m(1)(0) = other.m(1)(0)
        m(1)(1) = other.m(1)(1)
        m(1)(2) = other.m(1)(2)
        m(2)(0) = other.m(2)(0)
        m(2)(1) = other.m(2)(1)
        m(2)(2) = other.m(2)(2)
    End Sub

    Public Overridable Function multiply(ByVal vector As Vector3) As Vector3
        Dim output As New Vector3()

        output.X = m(0)(0) * vector.X + m(0)(1) * vector.Y + m(0)(2) * vector.Z
        output.Y = m(1)(0) * vector.X + m(1)(1) * vector.Y + m(1)(2) * vector.Z
        output.Z = m(2)(0) * vector.X + m(2)(1) * vector.Y + m(2)(2) * vector.Z

        Return output
    End Function

    'TODO it should be transposed?
    ''' <summary>
    ''' Rotates the matrix by the specified angles, in radians. </summary>
    ''' <param name="eulerRadiansX"> The 'bank' rotation, around the X axis, in radians. </param>
    ''' <param name="eulerRaidansY"> The 'heading' rotation, around the Y axis, in radians. </param>
    ''' <param name="eulerRadiansZ"> The 'attitude' rotation, around the Z axis, in radians.
    ''' @return </param>
    Public Overridable Function rotate(ByVal eulerRadiansX As Double, ByVal eulerRaidansY As Double, ByVal eulerRadiansZ As Double) As Matrix

        Dim ci As Double = Math.Cos(eulerRadiansX)
        Dim cj As Double = Math.Cos(eulerRaidansY)
        Dim ch As Double = Math.Cos(eulerRadiansZ)
        Dim si As Double = Math.Sin(eulerRadiansX)
        Dim sj As Double = Math.Sin(eulerRaidansY)
        Dim sh As Double = Math.Sin(eulerRadiansZ)

        Dim cc As Double = ci * ch
        Dim cs As Double = ci * sh
        Dim sc As Double = si * ch
        Dim ss As Double = si * sh

        m(0)(0) = CSng(cj * ch)
        m(0)(1) = CSng(sj * sc - cs)
        m(0)(2) = CSng(sj * cc + ss)
        m(1)(0) = CSng(cj * sh)
        m(1)(1) = CSng(sj * ss + cc)
        m(1)(2) = CSng(sj * cs - sc)
        m(2)(0) = CSng(-sj)
        m(2)(1) = CSng(cj * si)
        m(2)(2) = CSng(cj * ci)

        Return Me
    End Function

    Public Overridable Function ToEulerDegrees() As Single()
        Dim rotation(2) As Single
        rotation(0) = CSng(RadianToDegree(Math.Atan2(m(2)(1), m(2)(2))))
        rotation(1) = CSng(RadianToDegree(-Math.Asin(m(2)(0))))
        rotation(2) = CSng(RadianToDegree(Math.Atan2(m(1)(0), m(0)(0))))
        Return rotation
    End Function
    Private Function RadianToDegree(angle As Double) As Double
        Return angle * (180.0 / Math.PI)
    End Function

    Public Overridable Function Transposed() As Matrix
        Dim temp As New Matrix()
        For i As Integer = 0 To m.Length - 1
            Dim j As Integer = 0
            Do While j < m(0).Length
                temp.m(j)(i) = m(i)(j)
                j += 1
            Loop
        Next i

        Return temp
    End Function

    Public Overridable Function Read(ByVal r As FileReader) As Matrix
        For i As Integer = 0 To 2
            For f As Integer = 0 To 2
                m(i)(f) = r.ReadSingle 'r.readLEFloat()
            Next f
        Next i
        Return Me
    End Function

    Public Overridable Function Write(ByVal w As FileWriter) As Matrix
        For i As Integer = 0 To 2
            For f As Integer = 0 To 2
                w.Write(m(i)(f)) 'stream.writeLEFloat(m(i)(f))
            Next f
        Next i
        Return Me
    End Function

    Public Shared Function FromQuaternion(ByVal q As Vector4) As Matrix
        Dim result As Matrix = Matrix.Identity()

        Dim sqw As Double = q.W * q.W
        Dim sqx As Double = q.X * q.X
        Dim sqy As Double = q.Y * q.Y
        Dim sqz As Double = q.Z * q.Z

        ' invs (inverse square length) is only required if quaternion is not already normalised
        Dim invs As Double = 1 / (sqx + sqy + sqz + sqw)
        result.m(0)(0) = CSng((sqx - sqy - sqz + sqw) * invs) ' since sqw + sqx + sqy + sqz =1/invs*invs
        result.m(1)(1) = CSng((-sqx + sqy - sqz + sqw) * invs)
        result.m(2)(2) = CSng((-sqx - sqy + sqz + sqw) * invs)

        Dim tmp1 As Double = q.X * q.Y
        Dim tmp2 As Double = q.Z * q.W
        result.m(1)(0) = CSng(2.0 * (tmp1 + tmp2) * invs)
        result.m(0)(1) = CSng(2.0 * (tmp1 - tmp2) * invs)

        tmp1 = q.X * q.Z
        tmp2 = q.Y * q.W
        result.m(2)(0) = CSng(2.0 * (tmp1 - tmp2) * invs)
        result.m(0)(2) = CSng(2.0 * (tmp1 + tmp2) * invs)
        tmp1 = q.Y * q.Z
        tmp2 = q.X * q.W
        result.m(2)(1) = CSng(2.0 * (tmp1 + tmp2) * invs)
        result.m(1)(2) = CSng(2.0 * (tmp1 - tmp2) * invs)

        Return result
    End Function

End Class
