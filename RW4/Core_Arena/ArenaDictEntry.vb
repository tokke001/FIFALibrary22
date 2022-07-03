
Namespace Rw.Core.Arena
    Public Class ArenaDictEntry     'RW4SectionInfo
        'rw::core::arena::ArenaDictEntry
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.Offset = r.ReadUInt32
            Me.Reloc = r.ReadUInt32     'pointer (offset) to ?
            Me.Size = r.ReadUInt32
            Me.Alignment = r.ReadUInt32
            Me.TypeIndex = r.ReadUInt32
            Me.TypeId = r.ReadUInt32

            'Select Case TypeId '--> debugger
            '    Case AnimationSkin.TYPE_CODE
            '    Case AnimSeq.TYPE_CODE
            '    Case Bxd.Skeleton.TYPE_CODE
            '    Case Camera.TYPE_CODE
            '    Case ChannelCurve.TYPE_CODE
            '    Case CollisionModel.TYPE_CODE
            '    Case CullInfo.TYPE_CODE
            '    Case EmbeddedMesh.TYPE_CODE
            '    Case FxMaterial.TYPE_CODE
            '    Case FxRenderableSimple.TYPE_CODE
            '    Case HotSpot.TYPE_CODE
            '    Case IndexBuffer.TYPE_CODE
            '    Case Instance.TYPE_CODE
            '    Case Location.TYPE_CODE
            '    Case OldAnimation.Skeleton.TYPE_CODE
            '    Case ParameterBlock.TYPE_CODE
            '    Case ParameterBlockDescriptor.TYPE_CODE
            '    Case Raster.TYPE_CODE
            '    Case RenderModel.TYPE_CODE
            '    Case SceneLayer.TYPE_CODE
            '    Case SimpleMappedArray.TYPE_CODE
            '    Case Skeletonpose.TYPE_CODE
            '    Case SkinMatrixBuffer.TYPE_CODE
            '    Case Spline.TYPE_CODE
            '    Case VertexBuffer.TYPE_CODE
            '    Case Volume.TYPE_CODE
            '    Case VertexDescriptor.TYPE_CODE
            '    Case ArenaDictionary.TYPE_CODE
            '    Case FIFALibrary22.Rw.SectionTypeCode.RWOBJECTTYPE_BUFFER
            '        Select Case Me.Alignment
            '            Case 4096   'raster
            '            Case 4      'vertex
            '            Case Else

            '        End Select
            '    Case Else
            'End Select
        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.Offset)
            w.Write(Me.Reloc)
            w.Write(Me.Size)
            w.Write(Me.Alignment)
            w.Write(Me.TypeIndex)
            w.Write(CUInt(Me.TypeId))

        End Sub

        Public Property Offset As UInteger
        Public Property Reloc As UInteger = 0       'always 0
        Public Property Size As UInteger
        Public Property Alignment As UInteger
        Public Property TypeIndex As UInteger       'index of type at sectionManifest.ArenaSectionTypes.TypeCodes()
        Public Property TypeId As SectionTypeCode

    End Class
End Namespace
