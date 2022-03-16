'beta by Dmitri : http://soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/post-6652209
Namespace Rx3
    Public Class ClothDef
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.CLOTH_DEF
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal SectionSize As Long, ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(SectionSize, r)
        End Sub

        Public Sub Load(ByVal SectionSize As Long, ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position

            '--header - 284 bytes
            Me.TotalSize = r.ReadUInt32
            Me.Version = r.ReadUInt32

            If Me.Version <> 20803 Then     'in case the section format is different, it will be read out as byte-data : example FIFA online 4 !!
                Me.LoadAsByteData(BaseOffset, SectionSize, r)
                Exit Sub
            End If

            Me.Name = FifaUtil.Read64SizedString(r)
            Me.Unk18 = New Integer(30 - 1) {}
            For i = 0 To Me.Unk18.Length - 1
                Me.Unk18(i) = r.ReadInt32
            Next
            Me.Count1 = r.ReadUInt32
            Me.Unk49 = r.ReadUInt32
            Me.Count2 = r.ReadUInt32
            Me.Unk51 = r.ReadUInt32
            Me.Count3 = r.ReadUInt32
            Me.Unk53 = r.ReadUInt32
            Me.Count4 = r.ReadUInt32
            Me.Count5 = r.ReadUInt32
            Me.Count6 = r.ReadUInt32
            Me.Count7 = r.ReadUInt32
            Me.Count8 = r.ReadUInt32
            Me.Count9 = r.ReadUInt32
            Me.Unk60 = r.ReadUInt32
            Me.Count10 = r.ReadUInt32
            Me.Numclothadjacencytris = r.ReadUInt32
            Me.Numclothadjacencyverts = r.ReadUInt32
            Me.Count11 = r.ReadUInt32
            Me.Numjointnames = r.ReadUInt32
            Me.Numcolliders = r.ReadUInt32
            Me.Unk67 = r.ReadUInt32
            Me.Count12 = r.ReadUInt32
            Me.Count13 = r.ReadUInt32
            Me.Count14 = r.ReadUInt32

            '--data
            Me.Unk71 = New Single(Me.Count1 - 1) {}
            For i = 0 To Me.Unk71.Length - 1
                Me.Unk71(i) = r.ReadSingle
            Next
            Me.Unk72 = New Float4(Me.Count2 - 1) {}
            For i = 0 To Me.Unk72.Length - 1
                Me.Unk72(i) = New Float4(r)
            Next
            Me.Unk73 = New Float4(Me.Count3 - 1) {}
            For i = 0 To Me.Unk73.Length - 1
                Me.Unk73(i) = New Float4(r)
            Next
            Me.Unk74 = New ClothDefUnk8(Me.Count4 - 1) {}
            For i = 0 To Me.Unk74.Length - 1
                Me.Unk74(i) = New ClothDefUnk8(r)
            Next
            Me.Unk75 = New ClothDefUnk8(Me.Count5 - 1) {}
            For i = 0 To Me.Unk75.Length - 1
                Me.Unk75(i) = New ClothDefUnk8(r)
            Next
            Me.Unk76 = New ClothDefUnk8(Me.Count6 - 1) {}
            For i = 0 To Me.Unk76.Length - 1
                Me.Unk76(i) = New ClothDefUnk8(r)
            Next
            Me.Unk77 = New Single(Me.Count7 - 1) {}
            For i = 0 To Me.Unk77.Length - 1
                Me.Unk77(i) = r.ReadSingle
            Next
            Me.Unk78 = New Single(Me.Count8 - 1) {}
            For i = 0 To Me.Unk78.Length - 1
                Me.Unk78(i) = r.ReadSingle
            Next
            Me.Unk79 = New ClothDefUnk8(Me.Count9 - 1) {}
            For i = 0 To Me.Unk79.Length - 1
                Me.Unk79(i) = New ClothDefUnk8(r)
            Next
            Me.Unk80 = New UShort(Me.Count10 - 1) {}
            For i = 0 To Me.Unk80.Length - 1
                Me.Unk80(i) = r.ReadUInt16
            Next

            'Cloth Adjacency Tris
            Me.ClothAdjacencyTris = New ClothDefTri(Me.Numclothadjacencytris - 1) {}
            For i = 0 To Me.ClothAdjacencyTris.Length - 1
                Me.ClothAdjacencyTris(i) = New ClothDefTri(r)
            Next
            Me.ClothAdjacencyVerts = New Float3(Me.Numclothadjacencyverts - 1) {}
            For i = 0 To Me.ClothAdjacencyVerts.Length - 1
                Me.ClothAdjacencyVerts(i) = New Float3(r)
            Next
            Me.NumBonesPerVertex = r.ReadUInt32
            Me.NumBoneIndices = r.ReadUInt32

            'Cloth Anchor Bone Indices
            Me.AnchorBoneIndices = New UShort((Me.NumBonesPerVertex * Me.NumBoneIndices) - 1) {}
            For i = 0 To Me.AnchorBoneIndices.Length - 1
                Me.AnchorBoneIndices(i) = r.ReadUInt16
            Next
            Me.AnchorBoneWeights = New Single((Me.NumBonesPerVertex * Me.NumBoneIndices) - 1) {}
            For i = 0 To Me.AnchorBoneWeights.Length - 1
                Me.AnchorBoneWeights(i) = r.ReadSingle
            Next
            Me.Unk81 = New UShort(Me.Count11 - 1) {}
            For i = 0 To Me.Unk81.Length - 1
                Me.Unk81(i) = r.ReadUInt16
            Next

            'Cloth Joint Name Table
            Me.JointNameTable = New String(Me.Numjointnames - 1) {}
            For i = 0 To Me.JointNameTable.Length - 1
                Me.JointNameTable(i) = FifaUtil.Read64SizedString(r)
            Next

            'Cloth Colliders
            Me.Colliders = New ClothDefCollider(Me.Numcolliders - 1) {}
            For i = 0 To Me.Colliders.Length - 1
                Me.Colliders(i) = New ClothDefCollider(r)
            Next
            Me.Unk82 = New UInteger(Me.Numcolliders - 1) {}
            For i = 0 To Me.Unk82.Length - 1
                Me.Unk82(i) = r.ReadUInt32
            Next
            Me.Unk83 = New Single(Me.Count12 - 1) {}
            For i = 0 To Me.Unk83.Length - 1
                Me.Unk83(i) = r.ReadSingle
            Next
            Me.Unk84 = New Single(Me.Count13 - 1) {}
            For i = 0 To Me.Unk84.Length - 1
                Me.Unk84(i) = r.ReadSingle
            Next
            Me.Unk85 = New Single(Me.Count14 - 1) {}
            For i = 0 To Me.Unk85.Length - 1
                Me.Unk85(i) = r.ReadSingle
            Next

        End Sub

        Private Sub LoadAsByteData(ByVal StartOffset As Long, ByVal Size As Long, ByVal r As FileReader)
            Me.TotalSize = 0
            Me.Version = 0

            r.BaseStream.Position = StartOffset
            Me.Name = FifaUtil.Read64SizedString(r)
            r.BaseStream.Position = StartOffset
            Me._ByteData = r.ReadBytes(Size)
        End Sub

        Public Sub Save(ByVal w As FileWriter)

            If Me._ByteData IsNot Nothing Then  'in case the section format is different, it will be read out as byte-data : example FIFA online 4 !!
                w.Write(Me._ByteData)
                'Padding   
                FifaUtil.WriteAlignment_16(w)
                Exit Sub
            End If

            Dim BaseOffset As Long = w.BaseStream.Position


            '--header - 284 bytes
            w.Write(Me.TotalSize)
            w.Write(Me.Version)
            FifaUtil.Write64SizedString(w, Me.Name)
            For i = 0 To Me.Unk18.Length - 1
                w.Write(Me.Unk18(i))
            Next
            w.Write(Me.Count1)
            w.Write(Me.Unk49)
            w.Write(Me.Count2)
            w.Write(Me.Unk51)
            w.Write(Me.Count3)
            w.Write(Me.Unk53)
            w.Write(Me.Count4)
            w.Write(Me.Count5)
            w.Write(Me.Count6)
            w.Write(Me.Count7)
            w.Write(Me.Count8)
            w.Write(Me.Count9)
            w.Write(Me.Unk60)
            w.Write(Me.Count10)
            w.Write(Me.Numclothadjacencytris)
            w.Write(Me.Numclothadjacencyverts)
            w.Write(Me.Count11)
            w.Write(Me.Numjointnames)
            w.Write(Me.Numcolliders)
            w.Write(Me.Unk67)
            w.Write(Me.Count12)
            w.Write(Me.Count13)
            w.Write(Me.Count14)

            '--data
            For i = 0 To Me.Unk71.Length - 1
                w.Write(Me.Unk71(i))
            Next
            For i = 0 To Me.Unk72.Length - 1
                Me.Unk72(i).Save(w)
            Next
            For i = 0 To Me.Unk73.Length - 1
                Me.Unk73(i).Save(w)
            Next
            For i = 0 To Me.Unk74.Length - 1
                Me.Unk74(i).Save(w)
            Next
            For i = 0 To Me.Unk75.Length - 1
                Me.Unk75(i).Save(w)
            Next
            For i = 0 To Me.Unk76.Length - 1
                Me.Unk76(i).Save(w)
            Next
            For i = 0 To Me.Unk77.Length - 1
                w.Write(Me.Unk77(i))
            Next
            For i = 0 To Me.Unk78.Length - 1
                w.Write(Me.Unk78(i))
            Next
            For i = 0 To Me.Unk79.Length - 1
                Me.Unk79(i).Save(w)
            Next
            For i = 0 To Me.Unk80.Length - 1
                w.Write(Me.Unk80(i))
            Next

            'Cloth Adjacency Tris
            For i = 0 To Me.ClothAdjacencyTris.Length - 1
                Me.ClothAdjacencyTris(i).Save(w)
            Next
            For i = 0 To Me.ClothAdjacencyVerts.Length - 1
                Me.ClothAdjacencyVerts(i).Save(w)
            Next
            w.Write(Me.NumBonesPerVertex)
            w.Write(Me.NumBoneIndices)

            'Cloth Anchor Bone Indices
            For i = 0 To Me.AnchorBoneIndices.Length - 1
                w.Write(Me.AnchorBoneIndices(i))
            Next
            For i = 0 To Me.AnchorBoneWeights.Length - 1
                w.Write(Me.AnchorBoneWeights(i))
            Next
            For i = 0 To Me.Unk81.Length - 1
                w.Write(Me.Unk81(i))
            Next

            'Cloth Joint Name Table
            For i = 0 To Me.JointNameTable.Length - 1
                FifaUtil.Write64SizedString(w, Me.JointNameTable(i))
            Next

            'Cloth Colliders
            For i = 0 To Me.Colliders.Length - 1
                Me.Colliders(i).Save(w)
            Next
            For i = 0 To Me.Unk82.Length - 1
                w.Write(Me.Unk82(i))
            Next
            For i = 0 To Me.Unk83.Length - 1
                w.Write(Me.Unk83(i))
            Next
            For i = 0 To Me.Unk84.Length - 1
                w.Write(Me.Unk84(i))
            Next
            For i = 0 To Me.Unk85.Length - 1
                w.Write(Me.Unk85(i))
            Next


            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub


        Public Property _ByteData As Byte() = Nothing   'in case the section format is different, it will be read out as byte-data : example FIFA online 4 !!
        'header - 284 bytes
        Public Property TotalSize As UInteger
        Public Property Version As UInteger     '0x5143
        Public Property Name As String          '64 size string
        Public Property Unk18 As Integer() = New Integer(30 - 1) {}
        Public Property Count1 As UInteger    '128
        Public Property Unk49 As UInteger    '42
        Public Property Count2 As UInteger    '126
        Public Property Unk51 As UInteger    '0
        Public Property Count3 As UInteger    '126
        Public Property Unk53 As UInteger    '0
        Public Property Count4 As UInteger    '144
        Public Property Count5 As UInteger    '104
        Public Property Count6 As UInteger    '56
        Public Property Count7 As UInteger    '126
        Public Property Count8 As UInteger    '126
        Public Property Count9 As UInteger    '84
        Public Property Unk60 As UInteger    '0
        Public Property Count10 As UInteger    '126
        Public Property Numclothadjacencytris As UInteger    '94
        Public Property Numclothadjacencyverts As UInteger    '140
        Public Property Count11 As UInteger    '1
        Public Property Numjointnames As UInteger    '2
        Public Property Numcolliders As UInteger    '3
        Public Property Unk67 As UInteger    '8
        Public Property Count12 As UInteger    '84
        Public Property Count13 As UInteger    '128
        Public Property Count14 As UInteger    '128

        'data
        Public Property Unk71 As Single()   'array of count1    '0 and 1
        Public Property Unk72 As Float4()   'array of count2    
        Public Property Unk73 As Float4()   'array of count3    
        Public Property Unk74 As ClothDefUnk8()   'array of count4    
        Public Property Unk75 As ClothDefUnk8()   'array of count5    
        Public Property Unk76 As ClothDefUnk8()   'array of count6    
        Public Property Unk77 As Single()   'array of count7    
        Public Property Unk78 As Single()   'array of count8    
        Public Property Unk79 As ClothDefUnk8()   'array of count9    
        Public Property Unk80 As UShort()   'array of count10    

        'Cloth Adjacency Tris
        Public Property ClothAdjacencyTris As ClothDefTri()   'array of numClothAdjacencyTris    
        Public Property ClothAdjacencyVerts As Float3()   'array of numClothAdjacencyVerts    
        Public Property NumBonesPerVertex As UInteger
        Public Property NumBoneIndices As UInteger

        'Cloth Anchor Bone Indices
        Public Property AnchorBoneIndices As UShort()   'array of (numBonesPerVertex * numBoneIndices)
        Public Property AnchorBoneWeights As Single()   'array of (numBonesPerVertex * numBoneIndices)
        Public Property Unk81 As UShort()               'array of count11

        'Cloth Joint Name Table
        Public Property JointNameTable As String()      'array of numJointNames

        'Cloth Colliders
        Public Property Colliders As ClothDefCollider()         'array of numColliders
        Public Property Unk82 As UInteger()             'array of numColliders
        Public Property Unk83 As Single()               'array of count12
        Public Property Unk84 As Single()               'array of count13
        Public Property Unk85 As Single()               'array of count14

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
    Public Class ClothDefUnk8
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.Value_1 = r.ReadUInt16
            Me.Value_2 = r.ReadUInt16
            Me.Value_3 = r.ReadSingle
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Value_1)
            w.Write(Me.Value_2)
            w.Write(Me.Value_3)
        End Sub
        Public Property Value_1 As UShort
        Public Property Value_2 As UShort
        Public Property Value_3 As Single
    End Class

    Public Class ClothDefTri
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.Value_1 = r.ReadUInt16
            Me.Value_2 = r.ReadUInt16
            Me.Value_3 = r.ReadUInt16
            Me.Value_4 = r.ReadUInt16
        End Sub
        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Value_1)
            w.Write(Me.Value_2)
            w.Write(Me.Value_3)
            w.Write(Me.Value_4)
        End Sub
        Public Property Value_1 As UShort
        Public Property Value_2 As UShort
        Public Property Value_3 As UShort
        Public Property Value_4 As UShort
    End Class

    Public Class ClothDefCollider
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.Unk_1 = r.ReadSingle
            Me.Unk_2 = r.ReadBytes(24)
            For i = 0 To Me.Unk_3.Length - 1
                Me.Unk_3(i) = r.ReadSingle
            Next
            Me.Unk_4 = r.ReadInt32
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Unk_1)
            w.Write(Me.Unk_2)
            For i = 0 To Me.Unk_3.Length - 1
                w.Write(Me.Unk_3(i))
            Next
            w.Write(Me.Unk_4)
        End Sub
        Public Property Unk_1 As Single
        Public Property Unk_2 As Byte() = New Byte(24 - 1) {}    'array of 24
        Public Property Unk_3 As Single() = New Single(8 - 1) {}   'array of 8
        Public Property Unk_4 As Integer
    End Class
End Namespace