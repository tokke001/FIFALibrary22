Namespace Rx3
    Public Class AnimationSkin
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.ANIMATION_SKIN
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
            Me.NumBones = r.ReadUInt32
            Me.Unknown(0) = r.ReadUInt32
            Me.Unknown(1) = r.ReadUInt32

            For i = 0 To Me.NumBones - 1
                Dim pose As New BonePose(r)
                Me.BoneMatrices.Add(pose)
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumBones = Me.BoneMatrices.Count

            w.Write(Me.TotalSize)
            w.Write(Me.NumBones)
            w.Write(Me.Unknown(0))
            w.Write(Me.Unknown(1))

            For i = 0 To Me.NumBones - 1
                Me.BoneMatrices(i).Save(w)
            Next i


            'Padding   'usually not needed: already fits oke
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub

        'Public Function ExportInfos(ByVal FileName As String) As Boolean
        'Dim w As TextWriter = File.CreateText(FileName)
        'Dim flag As Boolean = True
        '
        '1 - write Header infos
        '    w.WriteLine(CStr("BoneId" & vbTab & "Matrix_1_x" & vbTab & "Matrix_1_y" & vbTab & "Matrix_1_Z" & vbTab & "Matrix_1_pos" _
        '                                     & vbTab & "Matrix_2_x" & vbTab & "Matrix_2_y" & vbTab & "Matrix_2_Z" & vbTab & "Matrix_2_pos" _
        '                                    & vbTab & "Matrix_3_x" & vbTab & "Matrix_3_y" & vbTab & "Matrix_3_Z" & vbTab & "Matrix_3_pos" _
        '                                   & vbTab & "Matrix_4_x" & vbTab & "Matrix_4_y" & vbTab & "Matrix_4_Z" & vbTab & "Matrix_4_pos"))

        '2 - Write datas
        'For i = 0 To Me.NumBones - 1
        '       w.Write(CStr(i))
        'For j = 0 To 4 - 1
        '           w.Write(CStr(vbTab))
        '
        'Me.BoneMatrices(i).absBindPose.m()


        '           w.Write(CStr(Me.BoneMatrices(i)(j).X))
        '          w.Write(CStr(vbTab))
        '         w.Write(CStr(Me.BoneMatrices(i)(j).Y))
        '        w.Write(CStr(vbTab))
        '       w.Write(CStr(Me.BoneMatrices(i)(j).Z))
        '      w.Write(CStr(vbTab))
        '     w.Write(CStr(Me.BoneMatrices(i)(j).Pos))

        '   Next j
        '          w.Write(CStr(vbNewLine))
        ' Next
        '
        ' w.Close()

        '    Return flag
        '   End Function

        Public Property TotalSize As UInteger   'total size of this section
        Public Property Unknown As UInteger() = New UInteger(2 - 1) {}   '{ 0, 0 }, maybe padding
        Public Property NumBones As UInteger
        Public Property BoneMatrices As List(Of BonePose) = New List(Of BonePose)()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace