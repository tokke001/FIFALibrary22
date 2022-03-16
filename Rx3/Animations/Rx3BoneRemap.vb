Namespace Rx3
    Public Class BoneRemap
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.BONE_REMAP
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
            Me.NumUsedBones = r.ReadByte

            Me.Padding = r.ReadBytes(11)    '0 : padding

            Me.UsedBones = r.ReadBytes((Me.TotalSize - 16) \ 2)
            Me.UsedBonesPositions = r.ReadBytes((Me.TotalSize - 16) \ 2)


            'Me.FixShortBoneIndices()   disabled: UsedBonesPositions are always bytes, not shorts...

        End Sub

        Private Sub FixShortBoneIndices()
            If Me.UsedBonesPositions(0) = 1 Then
                For i = 0 To Me.NumUsedBones - 1
                    If i + 1 <= Me.NumUsedBones - 1 Then
                        Me.UsedBonesPositions(i) = Me.UsedBonesPositions(i + 1) + 256
                    End If
                Next

                Me.UsedBonesPositions(Me.NumUsedBones - 1) = 0
                Me.NumUsedBones -= 1
            End If
        End Sub

        Public Sub Save(ByVal Rx3VertexBuffer As VertexBuffer, ByVal BoneIndicesIsShort As Boolean, ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.CreateFromVertexBuffer(Rx3VertexBuffer, BoneIndicesIsShort) 'creates NumUsedBones, UsedBones, UsedBonesPositions


            w.Write(Me.TotalSize)
            w.Write(Me.NumUsedBones)

            w.Write(Me.Padding)

            w.Write(Me.UsedBones)
            w.Write(Me.UsedBonesPositions)


            'Padding    'unused (TotalSize always a fixed size of 528)
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub

        Public Sub CreateFromVertexBuffer(ByVal VertexBuffer As VertexBuffer, ByVal BoneIndicesIsShort As Boolean)
            Dim HighValue_Found As Boolean = False
            Dim LowValue_Found As Boolean = False
            Me.UsedBones = New Byte(256 - 1) {}
            Me.UsedBonesPositions = New Byte(256 - 1) {}
            Dim UsedBonesPositions_low As Byte() = New Byte(256 - 1) {}
            Dim UsedBonesPositions_high As Byte() = New Byte(256 - 1) {}
            Dim List_UsedBonesPositions = New UShort(256 - 1) {}

            '1 - Create List 'UsedBonesPositions'
            Me.NumUsedBones = 0
            Dim NumUsedBones_low As Byte = 0
            Dim NumUsedBones_high As Byte = 0



            For i = 0 To VertexBuffer.VertexData.Length - 1
                If VertexBuffer.VertexData(i).BlendIndices IsNot Nothing Then
                    For j = 0 To VertexBuffer.VertexData(i).BlendIndices.Length - 1

                        'order of boneremap: reverse order of vertex (index_4 -> index_1)

                        If VertexBuffer.VertexData(i).BlendIndices(j).Index_4 > Byte.MaxValue Then
                            If BoneIndexExists(List_UsedBonesPositions, VertexBuffer.VertexData(i).BlendIndices(j).Index_4 - 256, NumUsedBones_high + NumUsedBones_low) = False Then
                                List_UsedBonesPositions(NumUsedBones_high + NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_4 - 256
                                UsedBonesPositions_high(NumUsedBones_high) = VertexBuffer.VertexData(i).BlendIndices(j).Index_4 - 256
                                HighValue_Found = True
                                NumUsedBones_high += 1
                            End If
                        Else
                            If BoneIndexExists(List_UsedBonesPositions, VertexBuffer.VertexData(i).BlendIndices(j).Index_4, NumUsedBones_high + NumUsedBones_low) = False Then
                                List_UsedBonesPositions(NumUsedBones_high + NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_4
                                UsedBonesPositions_low(NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_4
                                LowValue_Found = True
                                NumUsedBones_low += 1
                            End If
                        End If
                        If VertexBuffer.VertexData(i).BlendIndices(j).Index_3 > Byte.MaxValue Then
                            If BoneIndexExists(List_UsedBonesPositions, VertexBuffer.VertexData(i).BlendIndices(j).Index_3 - 256, NumUsedBones_high + NumUsedBones_low) = False Then
                                List_UsedBonesPositions(NumUsedBones_high + NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_3 - 256
                                UsedBonesPositions_high(NumUsedBones_high) = VertexBuffer.VertexData(i).BlendIndices(j).Index_3 - 256
                                HighValue_Found = True
                                NumUsedBones_high += 1
                            End If
                        Else
                            If BoneIndexExists(List_UsedBonesPositions, VertexBuffer.VertexData(i).BlendIndices(j).Index_3, NumUsedBones_high + NumUsedBones_low) = False Then
                                List_UsedBonesPositions(NumUsedBones_high + NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_3
                                UsedBonesPositions_low(NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_3
                                LowValue_Found = True
                                NumUsedBones_low += 1
                            End If
                        End If
                        If VertexBuffer.VertexData(i).BlendIndices(j).Index_2 > Byte.MaxValue Then
                            If BoneIndexExists(List_UsedBonesPositions, VertexBuffer.VertexData(i).BlendIndices(j).Index_2 - 256, NumUsedBones_high + NumUsedBones_low) = False Then
                                List_UsedBonesPositions(NumUsedBones_high + NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_2 - 256
                                UsedBonesPositions_high(NumUsedBones_high) = VertexBuffer.VertexData(i).BlendIndices(j).Index_2 - 256
                                HighValue_Found = True
                                NumUsedBones_high += 1
                            End If
                        Else
                            If BoneIndexExists(List_UsedBonesPositions, VertexBuffer.VertexData(i).BlendIndices(j).Index_2, NumUsedBones_high + NumUsedBones_low) = False Then
                                List_UsedBonesPositions(NumUsedBones_high + NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_2
                                UsedBonesPositions_low(NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_2
                                LowValue_Found = True
                                NumUsedBones_low += 1
                            End If
                        End If
                        If VertexBuffer.VertexData(i).BlendIndices(j).Index_1 > Byte.MaxValue Then
                            If BoneIndexExists(List_UsedBonesPositions, VertexBuffer.VertexData(i).BlendIndices(j).Index_1 - 256, NumUsedBones_high + NumUsedBones_low) = False Then
                                List_UsedBonesPositions(NumUsedBones_high + NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_1 - 256
                                UsedBonesPositions_high(NumUsedBones_high) = VertexBuffer.VertexData(i).BlendIndices(j).Index_1 - 256
                                HighValue_Found = True
                                NumUsedBones_high += 1
                            End If
                        Else
                            If BoneIndexExists(List_UsedBonesPositions, VertexBuffer.VertexData(i).BlendIndices(j).Index_1, NumUsedBones_high + NumUsedBones_low) = False Then
                                List_UsedBonesPositions(NumUsedBones_high + NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_1
                                UsedBonesPositions_low(NumUsedBones_low) = VertexBuffer.VertexData(i).BlendIndices(j).Index_1
                                LowValue_Found = True
                                NumUsedBones_low += 1
                            End If
                        End If



                    Next j
                End If
            Next i
            '1.1 - Add 00/01 in front if short values are found
            If BoneIndicesIsShort Then
                If LowValue_Found Then
                    Me.UsedBonesPositions(0) = 0
                    For i = 0 To NumUsedBones_low - 1
                        Me.UsedBonesPositions(i + 1) = UsedBonesPositions_low(i)
                    Next
                    Me.NumUsedBones = NumUsedBones_low + 1
                Else
                    Me.UsedBonesPositions(0) = 1
                    For i = 0 To NumUsedBones_high - 1
                        Me.UsedBonesPositions(i + 1) = UsedBonesPositions_high(i)
                    Next
                    Me.NumUsedBones = NumUsedBones_high + 1
                End If
            Else
                Me.UsedBonesPositions = UsedBonesPositions_low
                Me.NumUsedBones = NumUsedBones_low
            End If

            '2 - Update list UsedBones
            Dim num2 As Integer = 0
            For j = 0 To Me.UsedBonesPositions.Length - 1
                If num2 <= NumUsedBones - 1 Then
                    Me.UsedBones(Me.UsedBonesPositions(j)) = num2
                    num2 += 1
                End If
            Next j

        End Sub

        Private Function BoneIndexExists(ListIndices As UShort(), ByVal FindBoneIndex As UShort, ByVal SearchLength As Long) As Boolean
            If SearchLength = 0 Then
                Return False
            End If

            For i = 0 To SearchLength - 1
                If ListIndices(i) = FindBoneIndex Then
                    Return True
                End If
            Next i

            Return False
        End Function

        Public Property TotalSize As UInteger
        Public Property Padding As Byte() = New Byte(11 - 1) {}
        Public Property NumUsedBones As Byte
        Public Property UsedBones As Byte()
        Public Property UsedBonesPositions As Byte()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace