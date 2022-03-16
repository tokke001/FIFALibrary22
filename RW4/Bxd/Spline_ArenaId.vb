Namespace Rw.Bxd
    Public Class Spline
        'bxd::tSpline
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.SPLINE_ARENAID
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position

            Me.BBox = New BBox(r)

            Me.Flags = r.ReadUInt32
            Me.NumSegments = r.ReadInt32
            Me.OffsetHeaderSeg = r.ReadUInt32
            Me.Padding = r.ReadUInt32

            'bxd::tSplineSegment
            r.BaseStream.Position = BaseOffset + Me.OffsetHeaderSeg
            Me.Segments = New SplineSegment(Me.NumSegments - 1) {}
            For i = 0 To Me.NumSegments - 1
                If i > 0 Then
                    r.BaseStream.Position = BaseOffset + Me.Segments(i - 1).OffsetNextSeg
                End If
                Me.Segments(i) = New SplineSegment(r)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumSegments = Me.Segments.Length
            Me.OffsetHeaderSeg = 48

            Me.BBox.Save(w)

            w.Write(Me.Flags)
            w.Write(Me.NumSegments)
            w.Write(Me.OffsetHeaderSeg)
            w.Write(Me.Padding)

            For i = 0 To Me.Segments.Length - 1
                If i = Me.NumSegments - 1 Then
                    Me.Segments(i).OffsetNextSeg = 0     'last index, so no next one (0 value)
                Else
                    Me.Segments(i).OffsetNextSeg = Me.OffsetHeaderSeg + (160 * (i + 1))
                End If
                If i = 0 Then
                    Me.Segments(i).OffsetPrevSeg = 0 'first index, so no previous one (0 value)
                Else
                    Me.Segments(i).OffsetPrevSeg = Me.OffsetHeaderSeg + (160 * (i - 1))
                End If

                Me.Segments(i).Save(w)
            Next

        End Sub

        Public Property BBox As BBox   'Vector4 (min, max) 
        Public Property Flags As UInteger       '"0"
        Public Property NumSegments As Integer
        Public Property OffsetHeaderSeg As UInteger
        Public Property Padding As UInteger = 0      '"0"    padding?
        Public Property Segments As SplineSegment()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class SplineSegment
        'bxd::tSplineSegment
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.Intersect = r.ReadUIntegers(3)
            Me.FLength = r.ReadSingle

            Me.M4BasisMat = New Matrix4x4(r)

            Me.V4Inv = r.ReadVector4

            Me.OffsetPrevSeg = r.ReadUInt32
            Me.OffsetNextSeg = r.ReadUInt32
            Me.OffsetParent = r.ReadUInt32
            Me.Padding_1 = r.ReadUInt32

            Me.BBox = New BBox(r)
            Me.Distance = r.ReadSingle
            Me.Padding_2 = r.ReadUIntegers(2)

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.Intersect)
            w.Write(Me.FLength)

            Me.M4BasisMat.Save(w)

            w.Write(Me.V4Inv)

            w.Write(Me.OffsetPrevSeg)
            w.Write(Me.OffsetNextSeg)
            w.Write(Me.OffsetParent)
            w.Write(Me.Padding_1)

            Me.BBox.Save(w)
            w.Write(Me.Distance)
            w.Write(Me.Padding_2)

        End Sub


        Public Property Intersect As UInteger() = New UInteger(3 - 1) {}  'always 3 "0" values (padding)
        Public Property FLength As Single           'float length
        Public Property M4BasisMat As Matrix4x4       '4x4 FLoat4  
        Public Property V4Inv As Microsoft.DirectX.Vector4
        Public Property OffsetPrevSeg As UInteger       'Offset to previous Spline-Segment
        Public Property OffsetNextSeg As UInteger       'Offset to next Spline-Segment
        Public Property OffsetParent As UInteger       '"0" --> offset to parent section (bxd::tSpline)
        Public Property Padding_1 As UInteger = 0         '"0" --> padding before BBox?
        Public Property BBox As BBox  '2 FLoat4  -> m_v3Min, m_v3Max
        Public Property Distance As Single
        Public Property Padding_2 As UInteger() = New UInteger(3 - 1) {}  'always 3 "0" values (padding?)

    End Class
End Namespace