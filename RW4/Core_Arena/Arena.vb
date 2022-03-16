'https://github.com/emd4600/SporeModder-FX/tree/d5353aa43fca7c6cc0d8ac78c550e69547096e98/src/sporemodder/file/rw4
'https://emd4600.github.io/Spore-ModAPI/
Imports FIFALibrary22.Rw.Bxd
Imports FIFALibrary22.Rw.Collision
Imports FIFALibrary22.Rw.EA
Imports FIFALibrary22.Rw.EA.FxShader
Imports FIFALibrary22.Rw.Graphics
Imports FIFALibrary22.Rw.OldAnimation
Imports Microsoft.DirectX.Direct3D

Namespace Rw.Core.Arena
    Public Class Arena
        'rw::core::arena::Arena
        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        'Private ReadOnly DictEntries As List(Of ArenaDictEntry) = New List(Of ArenaDictEntry)()

        Public Sub Load(ByVal r As FileReader)

            Me.FileHeader = New ArenaFileHeader(r)

            Me.LoadGeneralStructure(r)
            'Me.RW4GeneralStructure = New RW4GeneralStructure(r)

            r.BaseStream.Position = Me.OffsetDict
            For i = 0 To Me.NumEntries - 1
                Dim sectionInfo As New ArenaDictEntry(r)
                Me.ArenaDictEntries.Add(sectionInfo)

                'If sectionInfo.TypeId = Rw.SectionTypeCode.RWOBJECTTYPE_BUFFER Then
                '    sectionInfo.Offset += Me.ResourceDescriptor.BaseResourceDescriptors(0).Size
                'End If
            Next i

            r.BaseStream.Position = Me.OffsetSectManifest
            Me.SectionManifest = New ArenaSectionManifest(Me, r)

            Me.Sections = New RWSections(Me, r)

        End Sub

        'Public Sub Load_old(ByVal r As FileReader)

        '    Me.FileHeader = New ArenaFileHeader(r)

        '    LoadGeneralStructure(r)
        '    'Me.RW4GeneralStructure = New RW4GeneralStructure(r)

        '    r.BaseStream.Position = Me.OffsetDict
        '    Me.DictEntries = New ArenaDictEntry(Me.NumEntries - 1) {}
        '    For i = 0 To Me.NumEntries - 1
        '        Me.DictEntries(i) = New ArenaDictEntry(r) '
        '    Next

        '    r.BaseStream.Position = Me.OffsetSectManifest
        '    Me.SectionManifest = New ArenaSectionManifest(r)


        '    Dim num1 As Integer = 0
        '    Dim num2 As Integer = 0
        '    Dim num3 As Integer = 0
        '    Dim num4 As Integer = 0
        '    Dim num5 As Integer = 0
        '    Dim num6 As Integer = 0
        '    Dim num7 As Integer = 0
        '    Dim num8 As Integer = 0
        '    Dim num9 As Integer = 0
        '    Dim num10 As Integer = 0
        '    Dim num11 As Integer = 0
        '    Dim num12 As Integer = 0
        '    Dim num13 As Integer = 0
        '    Dim num14 As Integer = 0
        '    Dim num15 As Integer = 0
        '    Dim num16 As Integer = 0
        '    Dim num17 As Integer = 0
        '    Dim num18 As Integer = 0
        '    Dim num19 As Integer = 0
        '    Dim num20 As Integer = 0
        '    Dim num21 As Integer = 0
        '    Dim num22 As Integer = 0
        '    Dim num23 As Integer = 0
        '    Dim num24 As Integer = 0
        '    Dim num25 As Integer = 0
        '    Dim num26 As Integer = 0
        '    For j = 0 To Me.NumEntries - 1
        '        'If Me.RW4SectionInfos(j).Offset = 0 Then   'if offset = 0 --> dont exist 
        '        'Continue For
        '        'Else
        '        If Not (Me.DictEntries(j).TypeId = 0 Or Me.DictEntries(j).TypeId = SectionTypeCode.RWOBJECTTYPE_BUFFER) Then
        '            r.BaseStream.Position = Me.DictEntries(j).Offset
        '        End If
        '        'End If

        '        Select Case Me.DictEntries(j).TypeId

        '            Case SectionTypeCode.RWOBJECTTYPE_BUFFER    'texture / vertex / index buffer       'present at end of file / Rx3 sections
        '                Continue For

        '            Case 0  'FIFA 07 "stadium_141_3_container_0.rx2"
        '                Continue For

        '            Case SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR
        '                ReDim Preserve Me.Sections.VertexDescriptor(num5)
        '                Me.Sections.VertexDescriptor(num5) = New VertexDescriptor(r)
        '                num5 += 1

        '            Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
        '                ReDim Preserve Me.Sections.VertexBuffer(num1)
        '                Me.Sections.VertexBuffer(num1) = New VertexBuffer(r)
        '                num1 += 1

        '            Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
        '                ReDim Preserve Me.Sections.IndexBuffer(num2)
        '                Me.Sections.IndexBuffer(num2) = New IndexBuffer(r)
        '                num2 += 1

        '            Case SectionTypeCode.RWGOBJECTTYPE_MESH
        '                ReDim Preserve Me.Sections.RW4Meshes(num3)
        '                Me.Sections.RW4Meshes(num3) = New EmbeddedMesh(r)
        '                num3 += 1

        '            Case SectionTypeCode.EA_FxShader_FxRenderableSimple
        '                ReDim Preserve Me.Sections.FxRenderableSimple(num4)
        '                Me.Sections.FxRenderableSimple(num4) = New FxRenderableSimple(r)
        '                num4 += 1

        '            Case SectionTypeCode.OBJECTTYPE_SKELETON
        '                ReDim Preserve Me.Sections.RW4Skeletons(num7)
        '                Me.Sections.RW4Skeletons(num7) = New OldAnimation.Skeleton(r)
        '                num7 += 1

        '            Case SectionTypeCode.OBJECTTYPE_ANIMATIONSKIN    'BONE_MATRICES
        '                ReDim Preserve Me.Sections.AnimationSkin(num6)
        '                Me.Sections.AnimationSkin(num6) = New AnimationSkin(r)
        '                num6 += 1

        '            Case SectionTypeCode.EA_HOTSPOT
        '                Me.Sections.RW4HotSpot = New HotSpot(r)

        '            Case SectionTypeCode.RWGOBJECTTYPE_RASTER
        '                ReDim Preserve Me.Sections.Raster(num8)
        '                Me.Sections.Raster(num8) = New Raster(r)
        '                num8 += 1

        '            Case SectionTypeCode.EA_ArenaDictionary
        '                Me.Sections.ArenaDictionary = New ArenaDictionary(r)

        '            Case SectionTypeCode.OBJECTTYPE_SKINMATRIXBUFFER
        '                ReDim Preserve Me.Sections.RW4SkinMatrixBuffers(num9)
        '                Me.Sections.RW4SkinMatrixBuffers(num9) = New SkinMatrixBuffer(r)
        '                num9 += 1

        '            Case SectionTypeCode.EA_FxShader_ParameterBlockDescriptor  '2
        '                ReDim Preserve Me.RW4Shader_ParameterBlockDescriptors(num10)
        '                Me.RW4Shader_ParameterBlockDescriptors(num10) = New ParameterBlockDescriptor(r)  'ALWAYS little endian (set in section) !
        '                num10 += 1

        '            Case SectionTypeCode.EA_FxShader_ParameterBlock     '2
        '                ReDim Preserve Me.RW4Shader_ParameterBlocks(num11)
        '                Me.RW4Shader_ParameterBlocks(num11) = New ParameterBlock(r, Me.RW4Shader_ParameterBlockDescriptors(num11))
        '                num11 += 1

        '            Case SectionTypeCode.EA_FxShader_FxMaterial           '2
        '                ReDim Preserve Me.Sections.RW4Shader_FxMaterials(num12)
        '                Me.Sections.RW4Shader_FxMaterials(num12) = New FxMaterial(r)
        '                num12 += 1

        '            Case SectionTypeCode.RWCOBJECTTYPE_VOLUME      '2
        '                ReDim Preserve Me.RW4Volumes(num13)
        '                Me.RW4Volumes(num13) = New Volume(r)
        '                num13 += 1

        '            Case SectionTypeCode.RWCOBJECTTYPE_SIMPLEMAPPEDARRAY   '2
        '                ReDim Preserve Me.RW4SimpleMappedArrays(num14)
        '                Me.RW4SimpleMappedArrays(num14) = New SimpleMappedArray(r)
        '                num14 += 1

        '            Case SectionTypeCode.MODELINSTANCE_ARENAID '2
        '                ReDim Preserve Me.RW4ModelInstances(num15)
        '                Me.RW4ModelInstances(num15) = New Instance(r, Me.DictEntries(j).Size)
        '                num15 += 1

        '            Case SectionTypeCode.MODELSKELETON_ARENAID '2
        '                ReDim Preserve Me.RW4ModelSkeletons(num16)
        '                Me.RW4ModelSkeletons(num16) = New Bxd.Skeleton(r)
        '                num16 += 1

        '            Case SectionTypeCode.MODELSKELETONPOSE_ARENAID '2
        '                ReDim Preserve Me.RW4ModelSkeletonPoses(num17)
        '                Me.RW4ModelSkeletonPoses(num17) = New Skeletonpose(r)
        '                num17 += 1

        '            Case SectionTypeCode.MODELRENDER_ARENAID        '2
        '                ReDim Preserve Me.RW4ModelRenders(num18)
        '                Me.RW4ModelRenders(num18) = New RenderModel(r)
        '                num18 += 1

        '            Case SectionTypeCode.MODELCOLLISION_ARENAID   '2
        '                ReDim Preserve Me.RW4ModelCollisions(num19)
        '                Me.RW4ModelCollisions(num19) = New CollisionModel(r)
        '                num19 += 1

        '            Case SectionTypeCode.SPLINE_ARENAID '2
        '                ReDim Preserve Me.RW4Splines(num20)
        '                Me.RW4Splines(num20) = New Spline(r)
        '                num20 += 1

        '            Case SectionTypeCode.SCENELAYER_ARENAID '2
        '                ReDim Preserve Me.RW4SceneLayers(num21)
        '                Me.RW4SceneLayers(num21) = New SceneLayer(r)
        '                num21 += 1

        '            Case SectionTypeCode.LOCATION_ARENAID '2
        '                ReDim Preserve Me.RW4Locations(num22)
        '                Me.RW4Locations(num22) = New Location(r)
        '                num22 += 1

        '            Case SectionTypeCode.CULLINFO_ARENAID   '1
        '                'ReDim Preserve Me.RW4CullInfos(num23)
        '                Me.RW4CullInfo = New CullInfo(r)
        '                num23 += 1
        '                If num23 > 1 Then MsgBox("Error - Found multiple sections at loading: CullInfo_ArenaId")

        '            Case SectionTypeCode.CAMERA_ARENAID   'euro08 "stadium_167_3_container_0.rx2" "stadium_167_4_container_0.rx2" '1
        '                ReDim Preserve Me.RW4Cameras(num24)
        '                Me.RW4Cameras(num24) = New Camera(r)
        '                num24 += 1

        '            Case SectionTypeCode.CHANNELCURVE_ARENAID    ' 'WC2010 "stadium_213_1_container_0.rx3"  'FIFA09 "festadium_188_4_container_0.rx2" "stadium_29_1_container_0.rx2" "stadium_29_4_container_0.rx2"  'array
        '                ReDim Preserve Me.RW4ChannelCurves(num25)
        '                Me.RW4ChannelCurves(num25) = New ChannelCurve(r)
        '                num25 += 1

        '            Case SectionTypeCode.ANIMSEQ_ARENAID   'array      'WC2010 "stadium_213_1_container_0.rx3" 'FIFA09 "festadium_188_4_container_0.rx2" (1) "stadium_29_1_container_0.rx2" (1) "stadium_29_4_container_0.rx2" (1) "stadium_175_1_container_0.rx2"  (array)  'usually exists with CHANNELCURVE_ARENAID ? -> may have links ! ?
        '                ReDim Preserve Me.RW4AnimSeqs(num26)
        '                Me.RW4AnimSeqs(num26) = New AnimSeq(r)
        '                num26 += 1
        '                'If num26 > 1 Then MsgBox("Error - Found multiple sections at loading: AnimSeq_ArenaId")

        '            Case Else
        '                MsgBox("Error at loading Rx3File: Unknown RW4 section found - " & Me.DictEntries(j).TypeId.ToString)
        '                Continue For

        '        End Select
        '    Next j

        '    'Me.RW4SectionManifest.SubReferences.LoadReferences(r)

        'End Sub

        Private Sub LoadGeneralStructure(ByVal r As FileReader)
            Me.AssetId = r.ReadUInt32               'id
            Me.NumEntries = r.ReadUInt32            'Number of Sections
            Me.NumUsed = r.ReadUInt32               'Number of Sections Used
            Me.Alignment = r.ReadUInt32
            Me.Virt = r.ReadUInt32
            Me.OffsetDict = r.ReadUInt32            'dump name: dictStart --> pointer to rw::core::arena::ArenaDictEntry
            Me.OffsetSectManifest = r.ReadUInt32    'dump name: sections
            Me.Base = r.ReadUInt32                  '32 bit pointer to void
            Me.UnfixContext = r.ReadUInt32          '--> pointer to 'rw::core::arena::SizeAndAlignment'
            Me.FixContext = r.ReadUInt32            ' --> pointer to 'rw::core::arena::ArenaIterator'

            'me.m_resourceDescriptor --> rw::ResourceDescriptor (Array of 5, total length 40)
            Me.ResourceDescriptor = New ResourceDescriptor(r)
            'For i = 0 To 5 - 1
            '    Me.ResourceDescriptor(i) = New ResourceDescriptor(r)
            'Next i
            'me.m_resourcesUsed --> rw::ResourceDescriptor (Array of 5, total length 40)
            Me.ResourcesUsed = New ResourceDescriptor(r)
            'For j = 0 To 5 - 1
            '    Me.ResourcesUsed(j) = New ResourceDescriptor(r)
            'Next j
            'me.m_resource  --> rw::TargetResource (pointers maybe, but usually 0) (Array of 5, total length 20)
            Me.Resource = New TargetResource(r)
            'For j = 0 To 5 - 1
            '    Me.Resource(j) = r.ReadUInt32
            'Next j

            Me.ArenaGroup = r.ReadUInt32            '--> pointer to 'rw::core::arena::ArenaGroup'

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            '1 - Save Header
            Me.FileHeader.Save(w)

            '2 - Save General Structure
            Dim OffsetGeneralStructure As Long = w.BaseStream.Position
            w.Write(New Byte(144) {}) 'Me.SaveGeneralStructure(w)

            ' - Section Manifest > SubReferences : clear first
            'Me.SectionManifest.SubReferences.Records.Clear()

            ' - Section Manifest > Types : Create the list with all the type codes
            Me.SectionManifest.Types.CreateList(Me.Sections.GetObjects)

            '3 - Save Section Manifest
            Me.OffsetSectManifest = w.BaseStream.Position
            Me.SectionManifest.Save(w)

            '4 - Save Sections
            Me.Sections.Save(w)

            ' - padding after last section
            'If Me.RW4GeneralStructure.Resources_3_Used(0).DataSize <> 0 Then
            'FifaUtil.WriteAllignment(w, Me.RW4GeneralStructure.Resources_3_Used(0).DataAlignment)
            'Else
            FifaUtil.WriteAlignment(w, 4)
            'End If

            '5 - Save Section infos
            Me.OffsetDict = w.BaseStream.Position
            Me.Sections.WriteSectionInfos(w)

            '6 - Save SubReferences (at end of RW4 section)
            Me.SectionManifest.SubReferences.SaveRecords(w)

            '7 - Add padding + get Offset rx3b/rx2 section
            FifaUtil.WriteAlignment(w, Me.ResourceDescriptor.BaseResourceDescriptors(0).Alignment)
            If Me.ResourcesUsed.BaseResourceDescriptors(2).Size <> 0 Then 'And Me.RW4GeneralStructure.Resources_3_Used(2).DataAlignment = 4096 Then
                w.Write(CByte(0))
                FifaUtil.WriteAlignment(w, Me.ResourcesUsed.BaseResourceDescriptors(2).Alignment)
            End If
            Me.ResourceDescriptor.BaseResourceDescriptors(0).Size = w.BaseStream.Position

            '8 - Save GeneralStructure & SectionManifest  (now with correct offsets/sizes)
            w.BaseStream.Position = OffsetGeneralStructure
            SaveGeneralStructure(w)
            w.BaseStream.Position = Me.OffsetSectManifest
            Me.SectionManifest.Save(w)

            '9 - Go to end of RW4 section (to continue to rx3b/rx2 section)
            w.BaseStream.Position = Me.ResourceDescriptor.BaseResourceDescriptors(0).Size

        End Sub

        'Public Sub Save_OLD(ByVal w As FileWriter)
        '    'Dim BlnHasBetaRW4Sections As Boolean = False  'HasBetaRW4Sections(Me.RW4SectionInfos) --> all known sections added !

        '    '1 - Save Header
        '    Me.FileHeader.Save(w)

        '    '2 - Save General Structure
        '    Dim OffsetRW4GeneralStructure As Long = w.BaseStream.Position
        '    SaveGeneralStructure(w)
        '    'Me.RW4GeneralStructure.Save(w)

        '    '3 - Save Section Manifest
        '    Me.OffsetSectManifest = w.BaseStream.Position
        '    Me.SectionManifest.Save(w)


        '    Dim num1_VB As Integer = 0
        '    Dim num2_IB As Integer = 0
        '    Dim num3 As Integer = 0
        '    Dim num4 As Integer = 0
        '    Dim num5 As Integer = 0
        '    Dim num6 As Integer = 0
        '    Dim num7 As Integer = 0
        '    Dim num8_RAS As Integer = 0
        '    Dim num9 As Integer = 0
        '    Dim num10 As Integer = 0
        '    Dim num11 As Integer = 0
        '    Dim num12 As Integer = 0
        '    Dim num13 As Integer = 0
        '    Dim num14 As Integer = 0
        '    Dim num15 As Integer = 0
        '    Dim num16 As Integer = 0
        '    Dim num17 As Integer = 0
        '    Dim num18 As Integer = 0
        '    Dim num19 As Integer = 0
        '    Dim num20 As Integer = 0
        '    Dim num21 As Integer = 0
        '    Dim num22 As Integer = 0
        '    Dim num23 As Integer = 0
        '    Dim num24 As Integer = 0
        '    Dim num25 As Integer = 0
        '    Dim num26 As Integer = 0
        '    Dim OffsetBuffers As UInteger = 0

        '    '4 - Save Sections
        '    For j = 0 To Me.ArenaDictEntries.Length - 1

        '        '4.1 - Add padding + Save Offset Section (to Section info)
        '        If Me.ArenaDictEntries(j).TypeId = 0 Then
        '            Continue For
        '        ElseIf Me.ArenaDictEntries(j).TypeId = SectionTypeCode.RWOBJECTTYPE_BUFFER Then
        '            Me.ArenaDictEntries(j).Offset = OffsetBuffers
        '        Else
        '            '4.1.1 - Add padding (so alignment is oke)   'put alignment of previous section before current starts: because current "Alignment" value is used before this section
        '            FifaUtil.WriteAlignment(w, Me.ArenaDictEntries(j).Alignment)
        '            '4.1.2 - Save Offset Section (to Section info)
        '            Me.ArenaDictEntries(j).Offset = w.BaseStream.Position
        '        End If

        '        '4.2 - Save Section
        '        Select Case Me.ArenaDictEntries(j).TypeId

        '            Case SectionTypeCode.RWOBJECTTYPE_BUFFER    'texture / vertex / index buffer       'present at end of file / Rx3 sections
        '                'Continue For

        '            Case 0  'FIFA 07 "stadium_141_3_container_0.rx2"
        '                Continue For

        '            Case SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR
        '                Me.Sections.VertexDescriptor(num5).Save(w)
        '                num5 += 1

        '            Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
        '                Me.Sections.VertexBuffer(num1_VB).Save(w)
        '                num1_VB += 1

        '            Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
        '                Me.Sections.IndexBuffer(num2_IB).Save(w)
        '                num2_IB += 1

        '            Case SectionTypeCode.RWGOBJECTTYPE_MESH
        '                Me.Sections.RW4Meshes(num3).Save(w)
        '                num3 += 1

        '            Case SectionTypeCode.EA_FxShader_FxRenderableSimple
        '                Me.Sections.FxRenderableSimple(num4).Save(w)
        '                num4 += 1

        '            Case SectionTypeCode.OBJECTTYPE_SKELETON
        '                Me.Sections.RW4Skeletons(num7).Save(w)
        '                num7 += 1

        '            Case SectionTypeCode.OBJECTTYPE_ANIMATIONSKIN    'BONE_MATRICES
        '                Me.Sections.AnimationSkin(num6).Save(w)
        '                num6 += 1

        '            Case SectionTypeCode.EA_HOTSPOT
        '                Me.Sections.RW4HotSpot.Save(w)

        '            Case SectionTypeCode.RWGOBJECTTYPE_RASTER
        '                Me.Sections.Raster(num8_RAS).Save(w)
        '                num8_RAS += 1

        '            Case SectionTypeCode.EA_ArenaDictionary
        '                Me.Sections.ArenaDictionary.Save(w)

        '            Case SectionTypeCode.OBJECTTYPE_SKINMATRIXBUFFER
        '                Me.Sections.RW4SkinMatrixBuffers(num9).Save(w)
        '                num9 += 1

        '            Case SectionTypeCode.EA_FxShader_ParameterBlockDescriptor  '2
        '                Me.RW4Shader_ParameterBlockDescriptors(num10).Save(w)      'ALWAYS little endian (set in section) !
        '                num10 += 1

        '            Case SectionTypeCode.EA_FxShader_ParameterBlock     '2
        '                Me.RW4Shader_ParameterBlocks(num11).Save(w) ', Me.RW4Shader_ParameterBlockDescriptors(num11))
        '                num11 += 1

        '            Case SectionTypeCode.EA_FxShader_FxMaterial           '2
        '                Me.Sections.RW4Shader_FxMaterials(num12).Save(w)
        '                num12 += 1

        '            Case SectionTypeCode.RWCOBJECTTYPE_VOLUME      '2
        '                Me.RW4Volumes(num13).Save(w)
        '                num13 += 1

        '            Case SectionTypeCode.RWCOBJECTTYPE_SIMPLEMAPPEDARRAY   '2
        '                Me.RW4SimpleMappedArrays(num14).Save(w)
        '                num14 += 1

        '            Case SectionTypeCode.MODELINSTANCE_ARENAID '2
        '                Me.RW4ModelInstances(num15).Save(w)
        '                num15 += 1

        '            Case SectionTypeCode.MODELSKELETON_ARENAID '2
        '                Me.RW4ModelSkeletons(num16).Save(w)
        '                num16 += 1

        '            Case SectionTypeCode.MODELSKELETONPOSE_ARENAID '2
        '                Me.RW4ModelSkeletonPoses(num17).Save(w)
        '                num17 += 1

        '            Case SectionTypeCode.MODELRENDER_ARENAID        '2
        '                Me.RW4ModelRenders(num18).Save(w)
        '                num18 += 1

        '            Case SectionTypeCode.MODELCOLLISION_ARENAID   '2
        '                Me.RW4ModelCollisions(num19).Save(w)
        '                num19 += 1

        '            Case SectionTypeCode.SPLINE_ARENAID '2
        '                Me.RW4Splines(num20).Save(w)
        '                num20 += 1

        '            Case SectionTypeCode.SCENELAYER_ARENAID '2
        '                Me.RW4SceneLayers(num21).Save(w)
        '                num21 += 1

        '            Case SectionTypeCode.LOCATION_ARENAID '2
        '                Me.RW4Locations(num22).Save(w)
        '                num22 += 1

        '            Case SectionTypeCode.CULLINFO_ARENAID   '1
        '                Me.RW4CullInfo.Save(w)
        '                'num23 += 1

        '            Case SectionTypeCode.CAMERA_ARENAID
        '                Me.RW4Cameras(num24).Save(w)
        '                num24 += 1

        '            Case SectionTypeCode.CHANNELCURVE_ARENAID
        '                Me.RW4ChannelCurves(num25).Save(w)
        '                num25 += 1

        '            Case SectionTypeCode.ANIMSEQ_ARENAID
        '                Me.RW4AnimSeqs(num26).Save(w)
        '                num26 += 1

        '            Case Else
        '                MsgBox("Error at saving Rx3File: Unknown RW4 section found - " & Me.ArenaDictEntries(j).TypeId.ToString)
        '                Continue For

        '        End Select

        '        '4.3 - Save Size Section (to Section info)
        '        If Me.ArenaDictEntries(j).TypeId = SectionTypeCode.RWOBJECTTYPE_BUFFER Then
        '            If j + 1 <= Me.ArenaDictEntries.Length Then
        '                Dim IdType As UInteger = 0
        '                Select Case Me.ArenaDictEntries(j + 1).TypeId
        '                    Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
        '                        IdType = num1_VB
        '                    Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
        '                        IdType = num2_IB
        '                    Case SectionTypeCode.RWGOBJECTTYPE_RASTER
        '                        IdType = num8_RAS
        '                End Select
        '                Me.ArenaDictEntries(j).Size = GetSizeRWBuffer(Me.ArenaDictEntries(j).Alignment, Me.ArenaDictEntries(j + 1).TypeId, IdType)
        '                OffsetBuffers += Me.ArenaDictEntries(j).Size
        '            End If
        '        Else
        '            Me.ArenaDictEntries(j).Size = w.BaseStream.Position - Me.ArenaDictEntries(j).Offset
        '        End If

        '    Next j

        '    ' - padding after last section
        '    'If Me.RW4GeneralStructure.Resources_3_Used(0).DataSize <> 0 Then
        '    'FifaUtil.WriteAllignment(w, Me.RW4GeneralStructure.Resources_3_Used(0).DataAlignment)
        '    'Else
        '    FifaUtil.WriteAlignment(w, 4)
        '    'End If

        '    '5 - Save Section infos
        '    Me.OffsetDict = w.BaseStream.Position
        '    For i = 0 To Me.ArenaDictEntries.Length - 1
        '        Me.ArenaDictEntries(i).Save(w)
        '    Next

        '    '6 - Save SubReferences (at end of RW4 section)
        '    CType(Me.SectionManifest.Sections(Me.SectionManifest.IndexOf(GetType(ArenaSectionSubreferences))), ArenaSectionSubreferences).SaveRecords(w)

        '    '7 - Add padding + get Offset rx3b/rx2 section
        '    FifaUtil.WriteAlignment(w, Me.ResourceDescriptor.BaseResourceDescriptors(0).Alignment)
        '    If Me.ResourcesUsed.BaseResourceDescriptors(2).Size <> 0 Then 'And Me.RW4GeneralStructure.Resources_3_Used(2).DataAlignment = 4096 Then
        '        w.Write(CByte(0))
        '        FifaUtil.WriteAlignment(w, Me.ResourcesUsed.BaseResourceDescriptors(2).Alignment)
        '    End If
        '    Me.ResourceDescriptor.BaseResourceDescriptors(0).Size = w.BaseStream.Position

        '    '8 - Save GeneralStructure & SectionManifest  (now with correct offsets/sizes)
        '    Me.NumEntries = Me.ArenaDictEntries.Length
        '    Me.NumUsed = Me.ArenaDictEntries.Length
        '    w.BaseStream.Position = OffsetRW4GeneralStructure
        '    SaveGeneralStructure(w)
        '    w.BaseStream.Position = Me.OffsetSectManifest
        '    Me.SectionManifest.Save(w)

        '    '9 - Go to end of RW4 section (to continue to rx3b/rx2 section)
        '    w.BaseStream.Position = Me.ResourceDescriptor.BaseResourceDescriptors(0).Size

        'End Sub

        Private Sub SaveGeneralStructure(ByVal w As FileWriter)
            Me.NumEntries = Me.ArenaDictEntries.Count
            Me.NumUsed = Me.ArenaDictEntries.Count
            'Me.OffsetDict = 'set in RWFile 
            'Me.OffsetSectManifest = 'set in RWFile 

            w.Write(Me.AssetId)
            w.Write(Me.NumEntries)
            w.Write(Me.NumUsed)
            w.Write(Me.Alignment)
            w.Write(Me.Virt)
            w.Write(Me.OffsetDict)
            w.Write(Me.OffsetSectManifest)
            w.Write(Me.Base)
            w.Write(Me.UnfixContext)
            w.Write(Me.FixContext)

            Me.ResourceDescriptor.Save(w)
            'For i = 0 To 5 - 1
            '    w.Write(Me.ResourceDescriptor(i).Size)
            '    w.Write(Me.ResourceDescriptor(i).Alignment)
            'Next i

            Me.ResourcesUsed.Save(w)
            'For j = 0 To 5 - 1
            '    w.Write(Me.ResourcesUsed(j).Size)
            '    w.Write(Me.ResourcesUsed(j).Alignment)
            'Next j

            Me.Resource.Save(w)
            'For i = 0 To 5 - 1
            '    w.Write(Me.Resource(i))
            'Next i

            w.Write(Me.ArenaGroup)

        End Sub

        'Private Function HasBetaRW4Sections(ByVal RW4SectionInfos As ArenaDictEntry()) As Boolean
        '    For j = 0 To RW4SectionInfos.Length - 1
        '        Select Case RW4SectionInfos(j).TypeId
        '            Case SectionTypeCode.RWOBJECTTYPE_BUFFER
        '                Continue For
        '            Case SectionTypeCode.RWGOBJECTTYPE_VERTEXDESCRIPTOR
        '                Continue For
        '            Case SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
        '                Continue For
        '            Case SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
        '                Continue For
        '            Case SectionTypeCode.RWGOBJECTTYPE_MESH
        '                Continue For
        '            Case SectionTypeCode.EA_FxShader_FxRenderableSimple
        '                Continue For
        '            Case SectionTypeCode.OBJECTTYPE_SKELETON
        '                Continue For
        '            Case SectionTypeCode.OBJECTTYPE_ANIMATIONSKIN
        '                Continue For
        '            Case SectionTypeCode.EA_HOTSPOT
        '                Continue For
        '            Case SectionTypeCode.RWGOBJECTTYPE_RASTER
        '                Continue For
        '            Case SectionTypeCode.EA_ArenaDictionary
        '                Continue For
        '            Case SectionTypeCode.OBJECTTYPE_SKINMATRIXBUFFER
        '                Continue For
        '            Case SectionTypeCode.EA_FxShader_ParameterBlockDescriptor
        '                Continue For
        '            Case SectionTypeCode.EA_FxShader_ParameterBlock
        '                Continue For
        '            Case SectionTypeCode.EA_FxShader_FxMaterial
        '                Continue For
        '            Case SectionTypeCode.SCENELAYER_ARENAID
        '                Continue For
        '            Case SectionTypeCode.MODELINSTANCE_ARENAID
        '                Continue For
        '            Case SectionTypeCode.MODELSKELETON_ARENAID
        '                Continue For
        '            Case SectionTypeCode.MODELSKELETONPOSE_ARENAID
        '                Continue For
        '            Case SectionTypeCode.MODELRENDER_ARENAID
        '                Continue For
        '            Case SectionTypeCode.MODELCOLLISION_ARENAID
        '                Continue For
        '            Case SectionTypeCode.RWCOBJECTTYPE_VOLUME
        '                Continue For
        '            Case SectionTypeCode.RWCOBJECTTYPE_SIMPLEMAPPEDARRAY
        '                Continue For
        '            Case SectionTypeCode.SPLINE_ARENAID
        '                Continue For
        '            Case SectionTypeCode.LOCATION_ARENAID
        '                Continue For
        '            Case SectionTypeCode.CULLINFO_ARENAID
        '                Continue For
        '            Case SectionTypeCode.CAMERA_ARENAID
        '                Continue For
        '            Case SectionTypeCode.CHANNELCURVE_ARENAID
        '                Continue For
        '            Case SectionTypeCode.ANIMSEQ_ARENAID
        '                Continue For
        '            Case Else
        '                Return True
        '        End Select
        '    Next j
        '    Return False
        'End Function



        'Public Overridable Sub addReference(ByVal [object] As RWObject, ByVal offset As Integer)
        '    header_Conflict.sectionManifest.subReferences.references.add(New SubReference([object], offset))
        'End Sub

        'Public Overridable Function GetName(ByVal m_Object As RWObject) As String
        '    Return m_Object.GetTypeCode.ToString 'm_Object.GetType().Name + AscW("-"c) + sectionInfos.IndexOf(m_Object.sectionInfo)
        'End Function

        Public Function WriteDictionary()   '--> found at dump

        End Function

        Public Property PrimitiveTypes(ByVal MeshIndex As UInteger) As Microsoft.DirectX.Direct3D.PrimitiveType
            Get
                If Me.Sections.FxRenderableSimples IsNot Nothing AndAlso Me.Sections.FxRenderableSimples(MeshIndex) IsNot Nothing Then
                    Return Me.Sections.FxRenderableSimples(MeshIndex).PrimitiveType
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.FxRenderableSimples IsNot Nothing AndAlso Me.Sections.FxRenderableSimples(MeshIndex) IsNot Nothing Then
                    Me.Sections.FxRenderableSimples(MeshIndex).PrimitiveType = Value
                End If
            End Set
        End Property
        Public Property PrimitiveTypes As List(Of Microsoft.DirectX.Direct3D.PrimitiveType)
            Get
                If Me.Sections.FxRenderableSimples IsNot Nothing Then
                    Dim Result As List(Of Microsoft.DirectX.Direct3D.PrimitiveType) = New List(Of Microsoft.DirectX.Direct3D.PrimitiveType)
                    For i = 0 To Me.Sections.FxRenderableSimples.Count - 1
                        Result.Add(Me.Sections.FxRenderableSimples(i).PrimitiveType)
                    Next

                    Return Result
                End If

                Return Nothing
            End Get
            Set
                If Me.Sections.FxRenderableSimples IsNot Nothing AndAlso Me.Sections.FxRenderableSimples.Count = Value.Count Then
                    For i = 0 To Me.Sections.FxRenderableSimples.Count - 1
                        Me.Sections.FxRenderableSimples(i).PrimitiveType = Value(i)
                    Next i
                End If
            End Set
        End Property


        Public Property FileHeader As ArenaFileHeader
        Friend ReadOnly ArenaDictEntries As List(Of ArenaDictEntry) = New List(Of ArenaDictEntry)
        Public Property SectionManifest As ArenaSectionManifest
        Public Property Sections As RWSections

        Public Property AssetId As UInteger     'Id
        Public Property NumEntries As UInteger
        Public Property NumUsed As UInteger
        Public Property Alignment As UInteger  ' (in bytes)
        Public Property Virt As UInteger  '0 - offset to file start?
        Public Property OffsetDict As UInteger
        Public Property OffsetSectManifest As UInteger
        Public Property Base As UInteger  ' (0)
        Public Property UnfixContext As UInteger  ' (0)
        Public Property FixContext As UInteger  ' (0)
        Public Property ResourceDescriptor As ResourceDescriptor '() = New ResourceDescriptor(5 - 1) {}
        Public Property ResourcesUsed As ResourceDescriptor '() = New ResourceDescriptor(5 - 1) {}
        Public Property Resource As TargetResource 'UInteger() = New UInteger(5 - 1) {}
        Public Property ArenaGroup As UInteger  ' (0)

    End Class
End Namespace