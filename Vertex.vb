Imports BCnEncoder.Shared
Imports FIFALibrary22.Rw.D3D
Imports Microsoft.DirectX.Direct3D

Public Class Vertex
    ' Methods

    Public Sub New()

    End Sub

    Public Sub New(ByVal VertexElements As VertexElement(), ByVal r As FileReader)
        Me.m_Endianness = r.Endianness
        Me.m_VertexElements = VertexElements
        Me.Load(r)
    End Sub

    ' ----- info -----
    'confirmed (at FO3) : DEC3N is uint32 byteswap !
    'https://docs.microsoft.com/en-us/windows/win32/direct3d9/d3ddecltype

    Public Sub Load(ByVal r As FileReader)
        'Dim num_textcords As Integer = 0

        For Each VElement In Me.m_VertexElements
            Dim flag_Unknown_Usage As Boolean = False
            Dim flag_Unknown_Type As Boolean = False

            Select Case VElement.Usage
                Case DeclarationUsage.Position
                    Select Case VElement.DataType
                        Case D3DDECLTYPE.FLOAT3
                            Me.LoadPosition_3f32(r)
                        Case D3DDECLTYPE.FLOAT16_4
                            Me.LoadPosition_4f16(r)
                        Case Else
                            flag_Unknown_Type = True
                    End Select

                Case DeclarationUsage.BlendWeight
                    If VElement.DataType = D3DDECLTYPE.UBYTE4N Then '4u8n
                        Me.LoadWeight_4u8n(VElement.UsageIndex, r)
                    ElseIf VElement.DataType = D3DDECLTYPE.USHORT4N Then
                        Me.LoadWeight_4u16n(VElement.UsageIndex, r)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case DeclarationUsage.BlendIndices
                    Select Case VElement.DataType
                        Case D3DDECLTYPE.UBYTE4
                            Me.LoadIndices_4u8(VElement.UsageIndex, r)
                        Case D3DDECLTYPE.USHORT4
                            Me.LoadIndices_4u16(VElement.UsageIndex, r)
                        Case Else
                            flag_Unknown_Type = True
                    End Select

                Case DeclarationUsage.Normal
                    If VElement.DataType = D3DDECLTYPE.DEC3N Then '3s10n
                        Me.LoadNormal_3s10n(r)
                    ElseIf VElement.DataType = D3DDECLTYPE.FLOAT3 Then
                        Me.LoadNormal_3f32(r)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case DeclarationUsage.TextureCoordinate
                    Select Case VElement.DataType
                        Case D3DDECLTYPE.FLOAT2
                            Me.LoadTextureCoordinate_2f32(VElement.UsageIndex, r)
                        Case D3DDECLTYPE.FLOAT16_2
                            Me.LoadTextureCoordinate_2f16(VElement.UsageIndex, r)
                        Case D3DDECLTYPE.USHORT2N
                            Me.LoadTextureCoordinate_2u16n(VElement.UsageIndex, r)
                        Case D3DDECLTYPE.SHORT2N
                            Me.LoadTextureCoordinate_216n(VElement.UsageIndex, r)
                        Case D3DDECLTYPE.FLOAT3
                            Me.LoadTextureCoordinate_3f32(VElement.UsageIndex, r)
                        Case Else
                            flag_Unknown_Type = True
                    End Select

                Case DeclarationUsage.Tangent
                    If VElement.DataType = D3DDECLTYPE.DEC3N Then '3s10n
                        Me.LoadTangent_3s10n(r)
                    ElseIf VElement.DataType = D3DDECLTYPE.FLOAT3 Then
                        Me.LoadTangent_3f32(r)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case DeclarationUsage.BiNormal
                    If VElement.DataType = D3DDECLTYPE.DEC3N Then '3s10n
                        Me.LoadBiNormal_3s10n(r)
                    ElseIf VElement.DataType = D3DDECLTYPE.FLOAT3 Then
                        Me.LoadBiNormal_3f32(r)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case DeclarationUsage.Color
                    If VElement.DataType = D3DDECLTYPE.UBYTE4N Then '4u8n
                        Me.LoadColor_4u8n(r)
                    ElseIf VElement.DataType = D3DDECLTYPE.D3DCOLOR Then
                        Me.LoadColor_D3DC(r)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case Else
                    flag_Unknown_Usage = True


            End Select

            If flag_Unknown_Usage Or flag_Unknown_Type Then
                MsgBox("At vertex reading: Unknown combination found: UsageType = " & VElement.Usage.ToString & ", DataType = " & VElement.DataType.ToString)
            End If
        Next VElement

    End Sub

    Private Sub LoadPosition_3f32(ByVal r As FileReader)
        Me.Position = New Position

        If Me.m_Endianness = Endian.Big Then
            Me.Position.X = r.ReadSingle
            Me.Position.Y = r.ReadSingle
            Me.Position.Z = r.ReadSingle
        Else
            Me.Position.X = r.ReadSingle
            Me.Position.Y = r.ReadSingle
            Me.Position.Z = r.ReadSingle
        End If

    End Sub

    Private Sub LoadPosition_4f16(ByVal r As FileReader)
        Me.Position = New Position

        If Me.m_Endianness = Endian.Big Then
            Me.Position.X = r.ReadHalf
            Me.Position.Y = r.ReadHalf
            Me.Position.Z = r.ReadHalf
            Me.Position.W = r.ReadHalf
        Else
            Me.Position.X = r.ReadHalf
            Me.Position.Y = r.ReadHalf
            Me.Position.Z = r.ReadHalf
            Me.Position.W = r.ReadHalf
        End If

    End Sub

    Private Sub LoadWeight_4u8n(ByVal UsageIndex As Integer, ByVal r As FileReader)     '4 ubytes normalized (sum should be 255), swapped bytes (big/little endian) possible !
        ReDim Preserve Me.BlendWeights(UsageIndex)
        Me.BlendWeights(UsageIndex) = New BlendWeights

        If Me.m_Endianness = Endian.Big Then
            Me.BlendWeights(UsageIndex).Weight_4 = r.ReadByte / 255
            Me.BlendWeights(UsageIndex).Weight_3 = r.ReadByte / 255
            Me.BlendWeights(UsageIndex).Weight_2 = r.ReadByte / 255
            Me.BlendWeights(UsageIndex).Weight_1 = r.ReadByte / 255
        Else
            Me.BlendWeights(UsageIndex).Weight_1 = r.ReadByte / 255
            Me.BlendWeights(UsageIndex).Weight_2 = r.ReadByte / 255
            Me.BlendWeights(UsageIndex).Weight_3 = r.ReadByte / 255
            Me.BlendWeights(UsageIndex).Weight_4 = r.ReadByte / 255
        End If
    End Sub

    Private Sub LoadWeight_4u16n(ByVal UsageIndex As Integer, ByVal r As FileReader)     '4 ushorts normalized (sum should be 65535), swapped bytes (big/little endian) possible !
        ReDim Preserve Me.BlendWeights(UsageIndex)
        Me.BlendWeights(UsageIndex) = New BlendWeights

        If Me.m_Endianness = Endian.Big Then
            Me.BlendWeights(UsageIndex).Weight_1 = r.ReadUInt16 / 65535    'NO endian swapping at FIFA07 !!!! bytes 
            Me.BlendWeights(UsageIndex).Weight_2 = r.ReadUInt16 / 65535
            Me.BlendWeights(UsageIndex).Weight_3 = r.ReadUInt16 / 65535
            Me.BlendWeights(UsageIndex).Weight_4 = r.ReadUInt16 / 65535
        Else
            Me.BlendWeights(UsageIndex).Weight_1 = r.ReadUInt16 / 65535
            Me.BlendWeights(UsageIndex).Weight_2 = r.ReadUInt16 / 65535
            Me.BlendWeights(UsageIndex).Weight_3 = r.ReadUInt16 / 65535
            Me.BlendWeights(UsageIndex).Weight_4 = r.ReadUInt16 / 65535
        End If
    End Sub

    Private Sub LoadIndices_4u8(ByVal UsageIndex As Integer, ByVal r As FileReader)     '4 Ubyte
        ReDim Preserve Me.BlendIndices(UsageIndex)
        Me.BlendIndices(UsageIndex) = New BlendIndices

        If Me.m_Endianness = Endian.Big Then
            Me.BlendIndices(UsageIndex).Index_4 = CUShort(r.ReadByte)
            Me.BlendIndices(UsageIndex).Index_3 = CUShort(r.ReadByte)
            Me.BlendIndices(UsageIndex).Index_2 = CUShort(r.ReadByte)
            Me.BlendIndices(UsageIndex).Index_1 = CUShort(r.ReadByte)
        Else
            Me.BlendIndices(UsageIndex).Index_1 = CUShort(r.ReadByte)
            Me.BlendIndices(UsageIndex).Index_2 = CUShort(r.ReadByte)
            Me.BlendIndices(UsageIndex).Index_3 = CUShort(r.ReadByte)
            Me.BlendIndices(UsageIndex).Index_4 = CUShort(r.ReadByte)
        End If

    End Sub

    Private Sub LoadIndices_4u16(ByVal UsageIndex As Integer, ByVal r As FileReader)    '4 ushort
        ReDim Preserve Me.BlendIndices(UsageIndex)
        Me.BlendIndices(UsageIndex) = New BlendIndices

        If Me.m_Endianness = Endian.Big Then
            Me.BlendIndices(UsageIndex).Index_4 = r.ReadUInt16
            Me.BlendIndices(UsageIndex).Index_3 = r.ReadUInt16
            Me.BlendIndices(UsageIndex).Index_2 = r.ReadUInt16
            Me.BlendIndices(UsageIndex).Index_1 = r.ReadUInt16
        Else
            Me.BlendIndices(UsageIndex).Index_1 = r.ReadUInt16
            Me.BlendIndices(UsageIndex).Index_2 = r.ReadUInt16
            Me.BlendIndices(UsageIndex).Index_3 = r.ReadUInt16
            Me.BlendIndices(UsageIndex).Index_4 = r.ReadUInt16
        End If

    End Sub

    Private Sub LoadNormal_3s10n(ByVal r As FileReader)    'DEC3N
        Me.Normal = New Normal

        Dim SngValues As Single() = FifaUtil.DEC3NtoFloats(r.ReadInt32)
        'Dim SngValues_v2 As Single() = FifaUtil.DEC3NtoFloats_v2(Me.Normals.DEC3N)
        'Dim SngValues_v3 As Single() = FifaUtil.DEC3NtoFloats_v3(Me.Normals.DEC3N)  '<--
        'FloatsToDEC3N_v2
        'Me.Normals.DEC3N = FifaUtil.FloatsToDEC3N(SngValues(0), SngValues(1), SngValues(2))
        'Dim SngValues2 = FifaUtil.DEC3NtoFloats(test)
        'Dim test As Integer = FifaUtil.FloatsToDEC3N_OLD(SngValues(0), SngValues(1), SngValues(2)) ', SngValues(3))
        'Dim test2 As Integer = FifaUtil.FloatsToDEC3N(SngValues(0), SngValues(1), SngValues(2)) ', SngValues(3))
        'Dim test3 As Integer = FifaUtil.FloatsToDEC3N(SngValues_v2(0), SngValues_v2(1), SngValues_v2(2)) ', SngValues(3))
        'Dim difference As Integer = Me.Normals.DEC3N - test
        'Dim SngValues_2 As Single() = FifaUtil.DEC3NtoFloats(test)
        Me.Normal.Normal_x = SngValues(0)
        Me.Normal.Normal_y = SngValues(1)
        Me.Normal.Normal_z = SngValues(2)

    End Sub

    Private Sub LoadNormal_3f32(ByVal r As FileReader)
        Me.Normal = New Normal

        If Me.m_Endianness = Endian.Big Then
            Me.Normal.Normal_x = r.ReadSingle
            Me.Normal.Normal_y = r.ReadSingle
            Me.Normal.Normal_z = r.ReadSingle
        Else
            Me.Normal.Normal_x = r.ReadSingle
            Me.Normal.Normal_y = r.ReadSingle
            Me.Normal.Normal_z = r.ReadSingle
        End If

    End Sub

    Private Sub LoadTextureCoordinate_2f32(ByVal UsageIndex As Integer, ByVal r As FileReader)
        ReDim Preserve Me.TextureCoordinates(UsageIndex)
        Me.TextureCoordinates(UsageIndex) = New TextureCoordinate

        If Me.m_Endianness = Endian.Big Then
            Me.TextureCoordinates(UsageIndex).U = r.ReadSingle
            Me.TextureCoordinates(UsageIndex).V = r.ReadSingle
        Else
            Me.TextureCoordinates(UsageIndex).U = r.ReadSingle
            Me.TextureCoordinates(UsageIndex).V = r.ReadSingle
        End If
    End Sub

    Private Sub LoadTextureCoordinate_3f32(ByVal UsageIndex As Integer, ByVal r As FileReader)
        ReDim Preserve Me.TextureCoordinates(UsageIndex)
        Me.TextureCoordinates(UsageIndex) = New TextureCoordinate

        If Me.m_Endianness = Endian.Big Then
            Me.TextureCoordinates(UsageIndex).U = r.ReadSingle
            Me.TextureCoordinates(UsageIndex).V = r.ReadSingle
            Me.TextureCoordinates(UsageIndex).Xtra_Value = r.ReadSingle
        Else
            Me.TextureCoordinates(UsageIndex).U = r.ReadSingle
            Me.TextureCoordinates(UsageIndex).V = r.ReadSingle
            Me.TextureCoordinates(UsageIndex).Xtra_Value = r.ReadSingle
        End If
    End Sub

    Private Sub LoadTextureCoordinate_2f16(ByVal UsageIndex As Integer, ByVal r As FileReader)
        ReDim Preserve Me.TextureCoordinates(UsageIndex)
        Me.TextureCoordinates(UsageIndex) = New TextureCoordinate

        If Me.m_Endianness = Endian.Big Then
            Me.TextureCoordinates(UsageIndex).U = r.ReadHalf
            Me.TextureCoordinates(UsageIndex).V = r.ReadHalf
        Else
            Me.TextureCoordinates(UsageIndex).U = r.ReadHalf
            Me.TextureCoordinates(UsageIndex).V = r.ReadHalf
        End If

    End Sub

    Private Sub LoadTextureCoordinate_2u16n(ByVal UsageIndex As Integer, ByVal r As FileReader)    'USHORT2N  swapped bytes possible (U,V <> V,U ??) ?
        ReDim Preserve Me.TextureCoordinates(UsageIndex)
        Me.TextureCoordinates(UsageIndex) = New TextureCoordinate

        If Me.m_Endianness = Endian.Big Then 'NO endian swapping at FIFA07 !!!! bytes (U,V <> V,U ??) ?
            Me.TextureCoordinates(UsageIndex).U = r.ReadUInt16 / 65535
            Me.TextureCoordinates(UsageIndex).V = r.ReadUInt16 / 65535
        Else
            Me.TextureCoordinates(UsageIndex).U = r.ReadUInt16 / 65535
            Me.TextureCoordinates(UsageIndex).V = r.ReadUInt16 / 65535
        End If
    End Sub

    Private Sub LoadTextureCoordinate_216n(ByVal UsageIndex As Integer, ByVal r As FileReader)    'SHORT2N  swapped bytes possible (U,V <> V,U ??) ?
        ReDim Preserve Me.TextureCoordinates(UsageIndex)
        Me.TextureCoordinates(UsageIndex) = New TextureCoordinate

        If Me.m_Endianness = Endian.Big Then    'NO endian swapping at FIFA07 !!!! bytes (U,V <> V,U ??) ?
            Me.TextureCoordinates(UsageIndex).U = r.ReadUInt16 / 32767
            Me.TextureCoordinates(UsageIndex).V = r.ReadUInt16 / 32767
        Else
            Me.TextureCoordinates(UsageIndex).U = r.ReadUInt16 / 32767
            Me.TextureCoordinates(UsageIndex).V = r.ReadUInt16 / 32767
        End If
    End Sub
    Private Sub LoadTangent_3s10n(ByVal r As FileReader)   'DEC3N
        Me.Tangent = New Tangent

        Dim SngValues As Single() = FifaUtil.DEC3NtoFloats(r.ReadInt32)
        Me.Tangent.Tangent_x = -SngValues(0)
        Me.Tangent.Tangent_y = -SngValues(1)
        Me.Tangent.Tangent_z = -SngValues(2)
    End Sub

    Private Sub LoadTangent_3f32(ByVal r As FileReader)
        Me.Tangent = New Tangent

        If Me.m_Endianness = Endian.Big Then
            Me.Tangent.Tangent_x = r.ReadSingle
            Me.Tangent.Tangent_y = r.ReadSingle
            Me.Tangent.Tangent_z = r.ReadSingle
        Else
            Me.Tangent.Tangent_x = r.ReadSingle
            Me.Tangent.Tangent_y = r.ReadSingle
            Me.Tangent.Tangent_z = r.ReadSingle
        End If
    End Sub

    Private Sub LoadBiNormal_3s10n(ByVal r As FileReader)
        Me.Binormal = New Binormal

        Dim SngValues As Single() = FifaUtil.DEC3NtoFloats(r.ReadInt32)
        Me.Binormal.Binormal_x = SngValues(0)
        Me.Binormal.Binormal_y = SngValues(1)
        Me.Binormal.Binormal_z = SngValues(2)
    End Sub

    Private Sub LoadBiNormal_3f32(ByVal r As FileReader)
        Me.Binormal = New Binormal

        If Me.m_Endianness = Endian.Big Then
            Me.Binormal.Binormal_x = r.ReadSingle
            Me.Binormal.Binormal_y = r.ReadSingle
            Me.Binormal.Binormal_z = r.ReadSingle
        Else
            Me.Binormal.Binormal_x = r.ReadSingle
            Me.Binormal.Binormal_y = r.ReadSingle
            Me.Binormal.Binormal_z = r.ReadSingle
        End If
    End Sub

    Private Sub LoadColor_4u8n(ByVal r As FileReader)     '4 ubytes normalized (sum should be 255), swapped bytes (big/little endian) possible !
        Me.Color = New VertexColor

        If Me.m_Endianness = Endian.Big Then
            Me.Color.Value_A = r.ReadByte / 255
            Me.Color.Value_B = r.ReadByte / 255
            Me.Color.Value_G = r.ReadByte / 255
            Me.Color.Value_R = r.ReadByte / 255
        Else
            Me.Color.Value_R = r.ReadByte / 255
            Me.Color.Value_G = r.ReadByte / 255
            Me.Color.Value_B = r.ReadByte / 255
            Me.Color.Value_A = r.ReadByte / 255
        End If
    End Sub

    Private Sub LoadColor_D3DC(ByVal r As FileReader)    'confirmed: ABGR at big endian vertex format !
        Me.Color = New VertexColor

        If Me.m_Endianness = Endian.Big Then    'argb at rx2 --> confirmed by beedy
            Me.Color.Value_A = r.ReadByte / 255
            Me.Color.Value_R = r.ReadByte / 255
            Me.Color.Value_G = r.ReadByte / 255
            Me.Color.Value_B = r.ReadByte / 255
        Else
            Me.Color.Value_R = r.ReadByte / 255
            Me.Color.Value_G = r.ReadByte / 255
            Me.Color.Value_B = r.ReadByte / 255
            Me.Color.Value_A = r.ReadByte / 255
        End If

    End Sub

    Public Sub Save(ByVal VertexElements As VertexElement(), ByVal w As FileWriter)     '--> this should be deleted : force to create a new vertex when u change the vertexelements
        Me.m_VertexElements = VertexElements
        Me.Save(w)
    End Sub

    Public Sub Save(ByVal w As FileWriter)
        Me.m_Endianness = w.Endianness

        'Dim num_textcords As Integer = 0

        For Each VElement In Me.m_VertexElements
            Dim flag_Unknown_Usage As Boolean = False
            Dim flag_Unknown_Type As Boolean = False

            Select Case VElement.Usage
                Case DeclarationUsage.Position
                    Select Case VElement.DataType
                        Case D3DDECLTYPE.FLOAT3
                            Me.SavePosition_3f32(w)
                        Case D3DDECLTYPE.FLOAT16_4
                            Me.SavePosition_4f16(w)
                        Case Else
                            flag_Unknown_Type = True
                    End Select

                Case DeclarationUsage.BlendWeight
                    If VElement.DataType = D3DDECLTYPE.UBYTE4N Then '4u8n
                        Me.SaveWeight_4u8n(VElement.UsageIndex, w)
                    ElseIf VElement.DataType = D3DDECLTYPE.USHORT4N Then
                        Me.SaveWeight_4u16n(VElement.UsageIndex, w)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case DeclarationUsage.BlendIndices
                    Select Case VElement.DataType
                        Case D3DDECLTYPE.UBYTE4
                            Me.SaveIndices_4u8(VElement.UsageIndex, w)
                        Case D3DDECLTYPE.USHORT4
                            Me.SaveIndices_4u16(VElement.UsageIndex, w)
                        Case Else
                            flag_Unknown_Type = True
                    End Select

                Case DeclarationUsage.Normal
                    If VElement.DataType = D3DDECLTYPE.DEC3N Then '3s10n
                        Me.SaveNormal_3s10n(w)
                    ElseIf VElement.DataType = D3DDECLTYPE.FLOAT3 Then
                        Me.SaveNormal_3f32(w)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case DeclarationUsage.TextureCoordinate
                    Select Case VElement.DataType
                        Case D3DDECLTYPE.FLOAT2
                            Me.SaveTextureCoordinate_2f32(VElement.UsageIndex, w)
                        Case D3DDECLTYPE.FLOAT16_2
                            Me.SaveTextureCoordinate_2f16(VElement.UsageIndex, w)
                        Case D3DDECLTYPE.USHORT2N
                            Me.SaveTextureCoordinate_2u16n(VElement.UsageIndex, w)
                        Case D3DDECLTYPE.SHORT2N
                            Me.SaveTextureCoordinate_216n(VElement.UsageIndex, w)
                        Case D3DDECLTYPE.FLOAT3
                            Me.SaveTextureCoordinate_3f32(VElement.UsageIndex, w)
                        Case Else
                            flag_Unknown_Type = True
                    End Select

                Case DeclarationUsage.Tangent
                    If VElement.DataType = D3DDECLTYPE.DEC3N Then '3s10n
                        Me.SaveTangent_3s10n(w)
                    ElseIf VElement.DataType = D3DDECLTYPE.FLOAT3 Then
                        Me.SaveTangent_3f32(w)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case DeclarationUsage.BiNormal
                    If VElement.DataType = D3DDECLTYPE.DEC3N Then '3s10n
                        Me.SaveBiNormal_3s10n(w)
                    ElseIf VElement.DataType = D3DDECLTYPE.FLOAT3 Then
                        Me.SaveBiNormal_3f32(w)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case DeclarationUsage.Color
                    If VElement.DataType = D3DDECLTYPE.UBYTE4N Then '4u8n
                        Me.SaveColor_4u8n(w)
                    ElseIf VElement.DataType = D3DDECLTYPE.D3DCOLOR Then
                        Me.SaveColor_D3DC(w)
                    Else
                        flag_Unknown_Type = True
                    End If

                Case Else
                    flag_Unknown_Usage = True

            End Select

            If flag_Unknown_Usage Or flag_Unknown_Type Then
                MsgBox("At vertex saving: Unknown combination found: UsageType = " & VElement.Usage.ToString & ", DataType = " & VElement.DataType.ToString)
            End If
        Next VElement

    End Sub

    Private Sub SavePosition_3f32(ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(Me.Position.X)
            w.Write(Me.Position.Y)
            w.Write(Me.Position.Z)
        Else
            w.Write(Me.Position.X)
            w.Write(Me.Position.Y)
            w.Write(Me.Position.Z)
        End If
    End Sub

    Private Sub SavePosition_4f16(ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(CType(Me.Position.X, Half))
            w.Write(CType(Me.Position.Y, Half))
            w.Write(CType(Me.Position.Z, Half))
            w.Write(CType(Me.Position.W, Half))
        Else
            w.Write(CType(Me.Position.X, Half))
            w.Write(CType(Me.Position.Y, Half))
            w.Write(CType(Me.Position.Z, Half))
            w.Write(CType(Me.Position.W, Half))
        End If
    End Sub

    Private Sub SaveWeight_4u8n(ByVal UsageIndex As Integer, ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(CByte(Math.Round(Me.BlendWeights(UsageIndex).Weight_4 * 255, 0)))
            w.Write(CByte(Math.Round(Me.BlendWeights(UsageIndex).Weight_3 * 255, 0)))
            w.Write(CByte(Math.Round(Me.BlendWeights(UsageIndex).Weight_2 * 255, 0)))
            w.Write(CByte(Math.Round(Me.BlendWeights(UsageIndex).Weight_1 * 255, 0)))   'w.Write(CByte(255 - Math.Round(Me.BlendWeights(UsageIndex).Weight_4 * 255, 0) - Math.Round(Me.BlendWeights(UsageIndex).Weight_3 * 255, 0) - Math.Round(Me.BlendWeights(UsageIndex).Weight_2 * 255, 0)))
        Else
            w.Write(CByte(Math.Round(Me.BlendWeights(UsageIndex).Weight_1 * 255, 0)))
            w.Write(CByte(Math.Round(Me.BlendWeights(UsageIndex).Weight_2 * 255, 0)))
            w.Write(CByte(Math.Round(Me.BlendWeights(UsageIndex).Weight_3 * 255, 0)))
            w.Write(CByte(Math.Round(Me.BlendWeights(UsageIndex).Weight_4 * 255, 0)))   'w.Write(CByte(255 - Math.Round(Me.BlendWeights(UsageIndex).Weight_4 * 255, 0) - Math.Round(Me.BlendWeights(UsageIndex).Weight_3 * 255, 0) - Math.Round(Me.BlendWeights(UsageIndex).Weight_2 * 255, 0)))
        End If

        'If Math.Round(Me.BlendWeights(UsageIndex).Weight_4 * 255, 0) + Math.Round(Me.BlendWeights(UsageIndex).Weight_3 * 255, 0) + Math.Round(Me.BlendWeights(UsageIndex).Weight_2 * 255, 0) + Math.Round(Me.BlendWeights(UsageIndex).Weight_1 * 255, 0) <> 255 Then MsgBox(Math.Round(Me.BlendWeights(UsageIndex).Weight_4 * 255, 0) + Math.Round(Me.BlendWeights(UsageIndex).Weight_3 * 255, 0) + Math.Round(Me.BlendWeights(UsageIndex).Weight_2 * 255, 0) + Math.Round(Me.BlendWeights(UsageIndex).Weight_1 * 255, 0))
    End Sub

    Private Sub SaveWeight_4u16n(ByVal UsageIndex As Integer, ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(CUShort(Math.Round(Me.BlendWeights(UsageIndex).Weight_1 * 65535, 0)))   'NO endian swapping at FIFA07 !!!! bytes 
            w.Write(CUShort(Math.Round(Me.BlendWeights(UsageIndex).Weight_2 * 65535, 0)))
            w.Write(CUShort(Math.Round(Me.BlendWeights(UsageIndex).Weight_3 * 65535, 0)))
            w.Write(CUShort(Math.Round(Me.BlendWeights(UsageIndex).Weight_4 * 65535, 0)))
        Else
            w.Write(CUShort(Math.Round(Me.BlendWeights(UsageIndex).Weight_1 * 65535, 0)))
            w.Write(CUShort(Math.Round(Me.BlendWeights(UsageIndex).Weight_2 * 65535, 0)))
            w.Write(CUShort(Math.Round(Me.BlendWeights(UsageIndex).Weight_3 * 65535, 0)))
            w.Write(CUShort(Math.Round(Me.BlendWeights(UsageIndex).Weight_4 * 65535, 0)))
        End If

    End Sub

    Private Sub SaveIndices_4u8(ByVal UsageIndex As Integer, ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(CByte(Me.BlendIndices(UsageIndex).Index_4))
            w.Write(CByte(Me.BlendIndices(UsageIndex).Index_3))
            w.Write(CByte(Me.BlendIndices(UsageIndex).Index_2))
            w.Write(CByte(Me.BlendIndices(UsageIndex).Index_1))
        Else
            w.Write(CByte(Me.BlendIndices(UsageIndex).Index_1))
            w.Write(CByte(Me.BlendIndices(UsageIndex).Index_2))
            w.Write(CByte(Me.BlendIndices(UsageIndex).Index_3))
            w.Write(CByte(Me.BlendIndices(UsageIndex).Index_4))
        End If
    End Sub

    Private Sub SaveIndices_4u16(ByVal UsageIndex As Integer, ByVal w As FileWriter)    'UNknown format??
        If Me.m_Endianness = Endian.Big Then
            w.Write(Me.BlendIndices(UsageIndex).Index_4)
            w.Write(Me.BlendIndices(UsageIndex).Index_3)
            w.Write(Me.BlendIndices(UsageIndex).Index_2)
            w.Write(Me.BlendIndices(UsageIndex).Index_1)
        Else
            w.Write(Me.BlendIndices(UsageIndex).Index_1)
            w.Write(Me.BlendIndices(UsageIndex).Index_2)
            w.Write(Me.BlendIndices(UsageIndex).Index_3)
            w.Write(Me.BlendIndices(UsageIndex).Index_4)
        End If
    End Sub

    Private Sub SaveNormal_3s10n(ByVal w As FileWriter)    'DEC3N
        Dim DEC3N As Integer = FifaUtil.FloatsToDEC3N(Me.Normal.Normal_x, Me.Normal.Normal_y, Me.Normal.Normal_z)
        w.Write(DEC3N)

    End Sub

    Private Sub SaveNormal_3f32(ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(Me.Normal.Normal_x)
            w.Write(Me.Normal.Normal_y)
            w.Write(Me.Normal.Normal_z)
        Else
            w.Write(Me.Normal.Normal_x)
            w.Write(Me.Normal.Normal_y)
            w.Write(Me.Normal.Normal_z)
        End If
    End Sub

    Private Sub SaveTextureCoordinate_2f32(ByVal UsageIndex As Integer, ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(Me.TextureCoordinates(UsageIndex).U)
            w.Write(Me.TextureCoordinates(UsageIndex).V)
        Else
            w.Write(Me.TextureCoordinates(UsageIndex).U)
            w.Write(Me.TextureCoordinates(UsageIndex).V)
        End If
    End Sub

    Private Sub SaveTextureCoordinate_3f32(ByVal UsageIndex As Integer, ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(Me.TextureCoordinates(UsageIndex).U)
            w.Write(Me.TextureCoordinates(UsageIndex).V)
            w.Write(Me.TextureCoordinates(UsageIndex).Xtra_Value)
        Else
            w.Write(Me.TextureCoordinates(UsageIndex).U)
            w.Write(Me.TextureCoordinates(UsageIndex).V)
            w.Write(Me.TextureCoordinates(UsageIndex).Xtra_Value)
        End If
    End Sub

    Private Sub SaveTextureCoordinate_2f16(ByVal UsageIndex As Integer, ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(CType(Me.TextureCoordinates(UsageIndex).U, Half))
            w.Write(CType(Me.TextureCoordinates(UsageIndex).V, Half))
        Else
            w.Write(CType(Me.TextureCoordinates(UsageIndex).U, Half))
            w.Write(CType(Me.TextureCoordinates(UsageIndex).V, Half))
        End If
    End Sub

    Private Sub SaveTextureCoordinate_2u16n(ByVal UsageIndex As Integer, ByVal w As FileWriter)    'USHORT2N  swapped bytes possible (U,V <> V,U ??) ?
        If Me.m_Endianness = Endian.Big Then
            w.Write(CUShort(Math.Round(Me.TextureCoordinates(UsageIndex).U * 65535, 0)))   'NO endian swapping at FIFA07 !!!! bytes 
            w.Write(CUShort(Math.Round(Me.TextureCoordinates(UsageIndex).V * 65535, 0)))
        Else
            w.Write(CUShort(Math.Round(Me.TextureCoordinates(UsageIndex).U * 65535, 0)))
            w.Write(CUShort(Math.Round(Me.TextureCoordinates(UsageIndex).V * 65535, 0)))
        End If
    End Sub

    Private Sub SaveTextureCoordinate_216n(ByVal UsageIndex As Integer, ByVal w As FileWriter)    'SHORT2N  swapped bytes possible (U,V <> V,U ??) ?
        If Me.m_Endianness = Endian.Big Then
            w.Write(CShort(Math.Round(Me.TextureCoordinates(UsageIndex).U * 32767, 0)))    'NO endian swapping at FIFA07 !!!! bytes 
            w.Write(CShort(Math.Round(Me.TextureCoordinates(UsageIndex).V * 32767, 0)))
        Else
            w.Write(CShort(Math.Round(Me.TextureCoordinates(UsageIndex).U * 32767, 0)))
            w.Write(CShort(Math.Round(Me.TextureCoordinates(UsageIndex).V * 32767, 0)))
        End If
    End Sub

    Private Sub SaveTangent_3s10n(ByVal w As FileWriter)   'DEC3N
        Dim DEC3N As Integer = FifaUtil.FloatsToDEC3N(-Me.Tangent.Tangent_x, -Me.Tangent.Tangent_y, -Me.Tangent.Tangent_z)
        w.Write(DEC3N)
    End Sub

    Private Sub SaveTangent_3f32(ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(Me.Tangent.Tangent_x)
            w.Write(Me.Tangent.Tangent_y)
            w.Write(Me.Tangent.Tangent_z)
        Else
            w.Write(Me.Tangent.Tangent_x)
            w.Write(Me.Tangent.Tangent_y)
            w.Write(Me.Tangent.Tangent_z)
        End If
    End Sub

    Private Sub SaveBiNormal_3s10n(ByVal w As FileWriter)
        Dim DEC3N As Integer = FifaUtil.FloatsToDEC3N(Me.Binormal.Binormal_x, Me.Binormal.Binormal_y, Me.Binormal.Binormal_z)
        w.Write(DEC3N)
    End Sub

    Private Sub SaveBiNormal_3f32(ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(Me.Binormal.Binormal_x)
            w.Write(Me.Binormal.Binormal_y)
            w.Write(Me.Binormal.Binormal_z)
        Else
            w.Write(Me.Binormal.Binormal_x)
            w.Write(Me.Binormal.Binormal_y)
            w.Write(Me.Binormal.Binormal_z)
        End If
    End Sub

    Private Sub SaveColor_4u8n(ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(CByte(Math.Round(Me.Color.Value_A * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_B * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_G * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_R * 255)))
        Else
            w.Write(CByte(Math.Round(Me.Color.Value_R * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_G * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_B * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_A * 255)))
        End If
    End Sub

    Private Sub SaveColor_D3DC(ByVal w As FileWriter)
        If Me.m_Endianness = Endian.Big Then
            w.Write(CByte(Math.Round(Me.Color.Value_A * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_R * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_G * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_B * 255)))
        Else
            w.Write(CByte(Math.Round(Me.Color.Value_R * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_G * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_B * 255)))
            w.Write(CByte(Math.Round(Me.Color.Value_A * 255)))
        End If
    End Sub


    ' Properties
    'Public Shared Property FloatType As EFloatType
    'Get
    'Return Rx3Vertex.s_FloatType
    'End Get
    'Set(ByVal value As EFloatType)
    'Rx3Vertex.s_FloatType = value
    'End Set
    'End Property

    Public Property Position As Position
    Public Property TextureCoordinates As TextureCoordinate() '= New TextureCoordinate(0) {}
    Public Property BlendWeights As BlendWeights() 'Byte() '= New Byte(4 - 1) {}
    Public Property BlendIndices As BlendIndices() 'UShort() '= New UShort(4 - 1) {}
    Public Property Tangent As Tangent
    Public Property Binormal As Binormal
    Public Property Color As VertexColor    'array?
    Public Property Normal As Normal

    ' Fields
    Private m_Endianness As Endian = Endian.Little
    Private m_VertexElements As VertexElement()

End Class

Public Class BlendIndices
    Public Property Index_1 As UShort
    Public Property Index_2 As UShort
    Public Property Index_3 As UShort
    Public Property Index_4 As UShort

End Class

Public Class BlendWeights
    Public Property Weight_1 As Single
    Public Property Weight_2 As Single
    Public Property Weight_3 As Single
    Public Property Weight_4 As Single

End Class

Public Class TextureCoordinate
    Public Property U As Single
    Public Property V As Single
    Public Property Xtra_Value As Single = 0    'rarely found: Found at game "UEFA Champions League 2006-2007" > file "stadium_159_6_container_0.rx2"

End Class

Public Class Position
    Public Property X As Single
    Public Property Y As Single
    Public Property Z As Single
    Public Property W As Single = 1
End Class

Public Class Normal
    Public Property Normal_x As Single
    Public Property Normal_y As Single
    Public Property Normal_z As Single

End Class

Public Class Tangent
    Public Property Tangent_x As Single
    Public Property Tangent_y As Single
    Public Property Tangent_z As Single

End Class

Public Class Binormal
    Public Property Binormal_x As Single
    Public Property Binormal_y As Single
    Public Property Binormal_z As Single

End Class

Public Class VertexColor    'RGBA 
    Public Property Value_R As Single
    Public Property Value_G As Single
    Public Property Value_B As Single
    Public Property Value_A As Single

End Class
