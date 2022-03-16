Namespace Rw.OldAnimation
    Public Class Skeleton
        'rw::oldanimation::Skeleton
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.OBJECTTYPE_SKELETON
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.OffsetBoneFlags = r.ReadUInt32
            Me.OffsetBoneParents = r.ReadUInt32
            Me.OffsetBoneNames = r.ReadUInt32
            Me.NumBones = r.ReadUInt32
            Me.SkeletonID = r.ReadUInt32
            r.BaseStream.Seek(4, SeekOrigin.Current)    'bone count (me.NumBones) again

            ReDim Me.Bones(Me.NumBones - 1)

            r.BaseStream.Position = Me.OffsetBoneNames
            For n = 0 To Me.NumBones - 1
                Me.Bones(n) = New SkeletonBone
                Me.Bones(n).Name = r.ReadUInt32
            Next n

            r.BaseStream.Position = Me.OffsetBoneFlags
            For f = 0 To Me.NumBones - 1
                Me.Bones(f).Flags = r.ReadUInt32    'value  0 to 3 
            Next f

            r.BaseStream.Position = Me.OffsetBoneParents
            For p = 0 To Me.NumBones - 1
                Me.Bones(p).Parent = r.ReadInt32
            Next p

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumBones = CUInt(Me.Bones.Length)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.OffsetBoneFlags = CUInt(BaseOffset + 24 + (Me.NumBones * 4))
            Me.OffsetBoneParents = CUInt(BaseOffset + 24 + (Me.NumBones * 4) + (Me.NumBones * 4))
            Me.OffsetBoneNames = CUInt(BaseOffset + 24)

            w.Write(Me.OffsetBoneFlags)
            w.Write(Me.OffsetBoneParents)
            w.Write(Me.OffsetBoneNames)
            w.Write(Me.NumBones)
            w.Write(Me.SkeletonID)
            w.Write(Me.NumBones)

            For n = 0 To Me.NumBones - 1
                w.Write(Me.Bones(n).Name)
            Next n

            For f = 0 To Me.NumBones - 1
                w.Write(Me.Bones(f).Flags)
            Next f

            For p = 0 To Me.NumBones - 1
                w.Write(Me.Bones(p).Parent)
            Next p

        End Sub

        Public Function ExportSkeletonInfos(ByVal FileName As String) As Boolean
            Dim w As TextWriter = File.CreateText(FileName)
            Dim flag As Boolean = True

            '1 - write Header infos
            w.WriteLine(CStr("BoneId" & vbTab & "BoneName_Hashed" & vbTab & "BoneName_String" & vbTab & "Parent" & vbTab & "Flags"))

            '2 - Write datas
            For i = 0 To Me.NumBones - 1
                w.Write(CStr(i))
                w.Write(CStr(vbTab))
                w.Write(CStr(Hex(Me.Bones(i).Name)))
                w.Write(CStr(vbTab))
                If [Enum].IsDefined(GetType(BoneNameHash), Me.Bones(i).Name) Then
                    w.Write(CStr(Me.Bones(i).Name.ToString))
                End If
                w.Write(CStr(vbTab))
                w.Write(CStr(Me.Bones(i).Parent))
                w.Write(CStr(vbTab))
                w.Write(CStr(Me.Bones(i).Flags))
                w.Write(CStr(vbNewLine))
            Next

            w.Close()

            Return flag
        End Function

        Public Property OffsetBoneFlags As UInteger
        Public Property OffsetBoneParents As UInteger
        Public Property OffsetBoneNames As UInteger
        Public Property NumBones As UInteger
        Public Property SkeletonID As UInteger
        Public Property Bones As SkeletonBone()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class SkeletonBone

        Public Property Name As BoneNameHash    'hashed : FNV132 --> https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function
        Public Property Flags As UInteger     'value  0 to 3 
        Public Property Parent As Integer   'integer (not uintegers ! )

    End Class
End Namespace
'Public Enum ESkeletonBones As Integer   'ESkeletonBones 0x00060e30
'    HEAD_BONE = 0
'    LOW_BACK_BONE = 1
'    NUM_SKEL_BONES = 2
'End Enum

'Enum BoneFlags as uinteger      'from spore game !?
'    kBoneRoot = 0
'    kBoneLeaf = 1      'bot blad
'    kBoneBranch = 2    'bot tak? 
'    kBoneRootLeaf = 3
'End Enum