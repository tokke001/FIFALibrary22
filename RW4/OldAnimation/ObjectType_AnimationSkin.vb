Namespace Rw.OldAnimation
    Public Class AnimationSkin
        'rw::oldanimation:: ?? (not found)
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.OBJECTTYPE_ANIMATIONSKIN
        Public Const ALIGNMENT As Integer = 16

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
                Dim pose As New BonePose(r)
                BoneMatrices.Add(pose)
            Next i

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.Offset = CUInt(w.BaseStream.Position + 16)
            Me.NumBones = CUInt(Me.BoneMatrices.Count)

            w.Write(Me.Offset)
            w.Write(Me.NumBones)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)

            For i = 0 To Me.NumBones - 1
                BoneMatrices(i).Save(w)
            Next i

        End Sub

        'Public Function ExportInfos(ByVal FileName As String) As Boolean
        'Dim w As TextWriter = File.CreateText(FileName)
        'Dim flag As Boolean = True

        '1 - write Header infos
        '   w.WriteLine(CStr("BoneId" & vbTab & "Matrix_1_x" & vbTab & "Matrix_1_y" & vbTab & "Matrix_1_Z" & vbTab & "Matrix_1_pos" _
        '                                     & vbTab & "Matrix_2_x" & vbTab & "Matrix_2_y" & vbTab & "Matrix_2_Z" & vbTab & "Matrix_2_pos" _
        '                                     & vbTab & "Matrix_3_x" & vbTab & "Matrix_3_y" & vbTab & "Matrix_3_Z" & vbTab & "Matrix_3_pos" _
        '                                     & vbTab & "Matrix_4_x" & vbTab & "Matrix_4_y" & vbTab & "Matrix_4_Z" & vbTab & "Matrix_4_pos"))

        '2 - Write datas
        'For i = 0 To Me.NumBones - 1
        '       w.Write(CStr(i))
        'For j = 0 To 4 - 1
        '           w.Write(CStr(vbTab))
        '          w.Write(CStr(Me.BoneMatrices(i)(j).X))
        '         w.Write(CStr(vbTab))
        '        w.Write(CStr(Me.BoneMatrices(i)(j).Y))
        '       w.Write(CStr(vbTab))
        '      w.Write(CStr(Me.BoneMatrices(i)(j).Z))
        '     w.Write(CStr(vbTab))
        '    w.Write(CStr(Me.BoneMatrices(i)(j).Pos))

        '    Next j
        '           w.Write(CStr(vbNewLine))
        '  Next
        '
        '   w.Close()

        'Return flag
        'End Function

        Public Property Offset As UInteger
        Public Property NumBones As UInteger
        Public Property Unknown_1 As UInteger   '"0"    
        Public Property Unknown_2 As UInteger   '"0"
        'Public Property BoneMatrices As New List(Of List(Of Vector4x1))
        ''' <summary>
        ''' Gets or Sets the Bone-Matrices. </summary>
        Public Property BoneMatrices As List(Of BonePose) = New List(Of BonePose)()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace