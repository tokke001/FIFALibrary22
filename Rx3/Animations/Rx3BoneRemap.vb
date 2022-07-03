Namespace Rx3
    Public Class BoneRemap
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.BONE_REMAP
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
            Me.NumUsedBones = r.ReadByte

            Me.Padding = r.ReadBytes(11)    '0 : padding

            Me.ReservedSize = (Me.TotalSize - 16) \ 2
            Me.UsedBones = r.ReadBytes(Me.ReservedSize)
            Me.UsedBonesPositions = FixShortBoneIndices(r.ReadBytes(Me.ReservedSize))

        End Sub

        ''' <summary>
        ''' Converts bytes to short values (if first byte value is "1") </summary>
        Private Function FixShortBoneIndices(ByVal UsedBonesPositions As Byte()) As UShort()
            If UsedBonesPositions(0) = 1 Then
                Dim ReturnList As UShort() = New UShort(Me.NumUsedBones - 1 - 1) {}
                For i = 0 To Me.NumUsedBones - 1 - 1
                    'If i + 1 <= Me.NumUsedBones - 1 Then
                    ReturnList(i) = UsedBonesPositions(i + 1) + 256
                    'End If
                Next

                'Me.UsedBonesPositions(Me.NumUsedBones - 1) = 0
                Me.NumUsedBones -= 1
                Return ReturnList
            Else
                Dim ReturnList As UShort() = New UShort(Me.NumUsedBones - 1) {}
                For i = 0 To Me.NumUsedBones - 1
                    ReturnList(i) = UsedBonesPositions(i)
                Next

                Return ReturnList
            End If
        End Function

        Public Sub Save(ByVal Rx3VertexBuffer As VertexBuffer, ByVal BoneIndicesIsShort As Boolean, ByVal w As FileWriter)
            Dim m_UsedBonesPositions As Byte() = New Byte(Me.ReservedSize - 1) {}
            Me.CreateUsedBonesPositions(m_UsedBonesPositions, Me.NumUsedBones, Rx3VertexBuffer, BoneIndicesIsShort)    'creates UsedBones, NumUsedBones
            Me.CreateUsedBones(Me.UsedBones, m_UsedBonesPositions)                                                     'creates UsedBonesPositions

            w.Write(Me.TotalSize)
            w.Write(Me.NumUsedBones)

            w.Write(Me.Padding)

            w.Write(Me.UsedBones)
            w.Write(m_UsedBonesPositions)


            'Padding    'unused (TotalSize always a fixed size of 528)
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub

        ''' <summary>
        ''' Create UsedBonesPositions array with short-values as bytes. 
        ''' And calculate NumUsedBones (Number of used bones).
        ''' </summary>
        Public Sub CreateUsedBonesPositions(ByRef UsedBonesPositions As Byte(), ByRef NumUsedBones As Byte, ByVal VertexBuffer As VertexBuffer, ByVal BoneIndicesIsShort As Boolean)
            Dim HighValue_Found As Boolean = False
            Dim LowValue_Found As Boolean = False
            UsedBonesPositions = New Byte(UsedBonesPositions.Length - 1) {}
            Dim UsedBonesPositions_low As Byte() = New Byte(UsedBonesPositions.Length - 1) {}
            Dim UsedBonesPositions_high As Byte() = New Byte(UsedBonesPositions.Length - 1) {}
            Dim List_UsedBonesPositions = New UShort(UsedBonesPositions.Length - 1) {}

            '1 - Create List 'UsedBonesPositions'
            NumUsedBones = 0
            Dim NumUsedBones_low As Byte = 0
            Dim NumUsedBones_high As Byte = 0


            For i = 0 To VertexBuffer.VertexData.Count - 1
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
                    UsedBonesPositions(0) = 0
                    For i = 0 To NumUsedBones_low - 1
                        UsedBonesPositions(i + 1) = UsedBonesPositions_low(i)
                    Next
                    NumUsedBones = NumUsedBones_low + 1
                Else
                    UsedBonesPositions(0) = 1
                    For i = 0 To NumUsedBones_high - 1
                        UsedBonesPositions(i + 1) = UsedBonesPositions_high(i)
                    Next
                    NumUsedBones = NumUsedBones_high + 1
                End If
            Else
                UsedBonesPositions = UsedBonesPositions_low
                NumUsedBones = NumUsedBones_low
            End If



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

        Private Sub CreateUsedBones(ByRef UsedBones As Byte(), ByVal UsedBonesPositions As Byte())
            '2 - Update list UsedBones
            UsedBones = New Byte(Me.ReservedSize - 1) {}
            Dim num2 As Integer = 0
            For j = 0 To UsedBonesPositions.Length - 1
                If num2 <= Me.NumUsedBones - 1 Then
                    UsedBones(UsedBonesPositions(j)) = num2
                    num2 += 1
                End If
            Next j
        End Sub

        Private m_TotalSize As UInteger
        Private m_UsedBones As Byte()
        Private m_NumUsedBones As Byte
        Private m_UsedBonesPositions As UShort()

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
        Public Property Padding As Byte() = New Byte(11 - 1) {}
        ''' <summary>
        ''' Number of used bones (ReadOnly). </summary>
        Public Property NumUsedBones As Byte
            Get
                Return m_NumUsedBones
            End Get
            Private Set
                m_NumUsedBones = Value
            End Set
        End Property

        ''' <summary>
        ''' Reserved size for UsedBones and UsedBonesPositions arrays (usually 256 or more). </summary>
        Public Property ReservedSize As UShort = 256
        ''' <summary>
        ''' Used Bones array (ReadOnly). </summary>
        Public Property UsedBones As Byte()
            Get
                Return m_UsedBones
            End Get
            Private Set
                m_UsedBones = Value
            End Set
        End Property
        ''' <summary>
        ''' Used Bones-positions array (ReadOnly). </summary>
        Public Property UsedBonesPositions As UShort()
            Get
                Return m_UsedBonesPositions
            End Get
            Private Set
                m_UsedBonesPositions = Value
            End Set
        End Property

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace