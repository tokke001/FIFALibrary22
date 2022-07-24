Namespace Rx3
    Public Class AnimationSkin
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.ANIMATION_SKIN
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Dim m_NumBones As UInteger = r.ReadUInt32
            Me.Pad = r.ReadBytes(8)

            For i = 0 To m_NumBones - 1
                Dim pose As New BonePose(r)
                Me.BoneMatrices.Add(pose)
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.TotalSize)
            w.Write(Me.NumBones)
            w.Write(Me.Pad)

            For i = 0 To Me.NumBones - 1
                Me.BoneMatrices(i).Save(w)
            Next i

            'Padding   'usually not needed: already fits oke
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

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

        Private m_TotalSize As UInteger

        ''' <summary>
        ''' Total section size (ReadOnly). </summary>
        Public Property TotalSize As UInteger
            Get
                Return m_TotalSize
            End Get
            Private Set
                m_TotalSize = Value
            End Set
        End Property
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Pad As Byte() = New Byte(8 - 1) {}    'always 0, padding
        ''' <summary>
        ''' Number of BoneMatrices (ReadOnly). </summary>
        Public ReadOnly Property NumBones As UInteger
            Get
                Return If(BoneMatrices?.Count, 0)
            End Get
        End Property
        ''' <summary>
        ''' Gets or Sets the Bone-Matrices. </summary>
        Public Property BoneMatrices As New List(Of BonePose)

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace