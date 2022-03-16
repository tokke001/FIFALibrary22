Namespace Rx3
    Public Class Skeleton
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.SKELETON
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

            Me.SkeletonBoneInfo = New SkeletonBoneInfo(Me.NumBones - 1) {}
            For i = 0 To Me.NumBones - 1
                Me.SkeletonBoneInfo(i) = New SkeletonBoneInfo With {
                        .Parent = r.ReadInt16
                        }
                '.FirstChild = r.ReadInt16

            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumBones = Me.SkeletonBoneInfo.Length

            w.Write(Me.TotalSize)
            w.Write(Me.NumBones)
            w.Write(Me.Unknown(0))
            w.Write(Me.Unknown(1))

            For i = 0 To Me.NumBones - 1
                w.Write(Me.SkeletonBoneInfo(i).Parent)
                'w.Write(Me.SkeletonBoneInfo(i).FirstChild)

            Next i

            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub

        Public Function ExportSkeletonInfos(ByVal FileName As String, Nametable As NameTable) As Boolean
            Dim w As TextWriter = File.CreateText(FileName)
            Dim flag As Boolean = True

            '1 - write Header infos
            w.WriteLine(CStr("BoneId" & vbTab & "BoneName" & vbTab & "Parent"))

            '2 - Write datas
            For i = 0 To Me.NumBones - 1
                w.Write(CStr(i))
                w.Write(CStr(vbTab))
                If Nametable.Names.Length - 1 >= i Then
                    If Nametable.Names(i).m_Type = SectionHash.BONE_NAME Then
                        w.Write(CStr(Nametable.Names(i).Name))
                    End If
                End If
                w.Write(CStr(vbTab))
                w.Write(CStr(Me.SkeletonBoneInfo(i).Parent))
                w.Write(CStr(vbNewLine))
            Next

            w.Close()

            Return flag
        End Function

        Public Property TotalSize As UInteger
        Public Property NumBones As UInteger
        Public Property Unknown As UInteger() = New UInteger(2 - 1) {}   '{ 0, 0 }, maybe padding
        Public Property SkeletonBoneInfo As SkeletonBoneInfo()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class SkeletonBoneInfo
        Public Property Parent As Short        '-1 for root bone   'shorts (not ushorts ! )
        'Property FirstChild As Short    'or sibling?       'shorts (not ushorts ! ) 

        'Property FirstChild : not sure if exists !
        'F14 & F20 : 1 byte
        'F16 : 2byte
    End Class

End Namespace