Namespace Rw.OldAnimation
    Public Class SkinMatrixBuffer
        'rw::oldanimation:: ?? (not found)
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.OBJECTTYPE_SKINMATRIXBUFFER
        Public Const ALIGNMENT As Integer = 128

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Me.Offset = r.ReadUInt32
            Me.NumBones = r.ReadUInt32
            Me.Unknown_1 = r.ReadUInt32     'padding?
            Me.Unknown_2 = r.ReadUInt32     'padding?

            r.BaseStream.Position = Me.Offset
            For i = 0 To Me.NumBones - 1
                Dim m_matrix As New Matrix4x3Affine(r)
                Me.BoneMatrices.Add(m_matrix)
            Next i

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.Offset = CUInt(w.BaseStream.Position + 16 + 112)
            Me.NumBones = CUInt(Me.BoneMatrices.Count)



            w.Write(Me.Offset)
            w.Write(Me.NumBones)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)

            'write padding
            FifaUtil.WriteNullPaddings(w, 112)

            For i = 0 To Me.NumBones - 1
                Me.BoneMatrices(i).Save(w)
            Next i


            'write padding
            FifaUtil.WriteAlignment(w, 128)    'usually also 112 paddings!

        End Sub


        Public Property Offset As UInteger
        Public Property NumBones As UInteger
        Public Property Unknown_1 As UInteger
        Public Property Unknown_2 As UInteger
        Public Property BoneMatrices As List(Of Matrix4x3Affine) = New List(Of Matrix4x3Affine)()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace