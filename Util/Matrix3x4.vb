Imports Microsoft.DirectX

Public Class Matrix3x4

    Public ReadOnly NUM_ROWS As Integer = 3
    Public ReadOnly NUM_COLS As Integer = 4

    Public ReadOnly m()() As Single = {New Single(3) {}, New Single(3) {}, New Single(3) {}}

    Public Sub New()
        For i As Integer = 0 To NUM_ROWS - 1
            Dim row(NUM_COLS - 1) As Single
            If i < NUM_COLS Then
                row(i) = 1.0F
            End If
            m(i) = row
        Next i
    End Sub

    ''' <summary>
    ''' Returns a copy of the rotation part of this matrix;
    ''' @return
    ''' </summary>
    Public Overridable Property Rotation As Matrix
        Get
            Dim matrix As Matrix = Matrix.Identity()
            For i As Integer = 0 To 2
                For j As Integer = 0 To 2
                    matrix.m(i)(j) = m(i)(j)
                Next j
            Next i
            Return matrix
        End Get
        Set(ByVal matrix As Matrix)
            For i As Integer = 0 To 2
                For j As Integer = 0 To 2
                    m(i)(j) = matrix.m(i)(j)
                Next j
            Next i
        End Set
    End Property

    ''' <summary>
    ''' Returns a copy of the offset (translation) part of this matrix.
    ''' @return
    ''' </summary>
    Public Overridable Property Offset As Vector3
        Get
            Return New Vector3(m(0)(3), m(1)(3), m(2)(3))
        End Get
        Set(ByVal vector As Vector3)
            m(0)(3) = vector.X
            m(1)(3) = vector.Y
            m(2)(3) = vector.Z
        End Set
    End Property



    Public Overridable Sub Load(ByVal r As FileReader)
        For i = 0 To m.Length - 1
            'Dim dst As Single() = m(i)

            For j = 0 To m(i).Length - 1
                'Dim dst2 As Single = dst(j)
                m(i)(j) = r.ReadSingle
            Next j
        Next i
    End Sub

    Public Overridable Sub Save(ByVal w As FileWriter)
        For i = 0 To m.Length - 1
            'Dim dst As Single() = m(i)

            For j = 0 To m(i).Length - 1
                'Dim dst2 As Single = dst(i1)
                w.Write(m(i)(j))
            Next j

        Next i
    End Sub
End Class
