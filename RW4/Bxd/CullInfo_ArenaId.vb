Namespace Rw.Bxd
    Public Class CullInfo
        'bxd::tCullInfo
        Inherits RwObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.CULLINFO_ARENAID
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position

            Me.NumBins = r.ReadUInt32
            Me.NumMeshes = r.ReadUInt32
            Me.OffsetV3FatBBoxMins = r.ReadUInt32
            Me.OffsetV3FatBBoxMaxes = r.ReadUInt32

            Me.OffsetMeshCullInfos = r.ReadUInt32
            Me.Pad = r.ReadUIntegers(3)


            r.BaseStream.Position = BaseOffset + Me.OffsetV3FatBBoxMins
            Me.V3FatBBoxMins = New Microsoft.DirectX.Vector4(Me.NumBins - 1) {}
            For i = 0 To Me.V3FatBBoxMins.Length - 1
                Me.V3FatBBoxMins(i) = r.ReadVector4
            Next

            r.BaseStream.Position = BaseOffset + Me.OffsetV3FatBBoxMaxes
            Me.V3FatBBoxMaxes = New Microsoft.DirectX.Vector4(Me.NumBins - 1) {}
            For i = 0 To Me.V3FatBBoxMaxes.Length - 1
                Me.V3FatBBoxMaxes(i) = r.ReadVector4
            Next

            r.BaseStream.Position = BaseOffset + Me.OffsetMeshCullInfos
            Me.MeshCullInfos = New MeshCullInfo(Me.NumMeshes - 1) {}
            For i = 0 To Me.MeshCullInfos.Length - 1
                Me.MeshCullInfos(i) = New MeshCullInfo(BaseOffset, Me.NumBins, r)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Me.NumBins = Me.V3FatBBoxMins.Length
            Me.NumMeshes = Me.MeshCullInfos.Length
            Me.OffsetV3FatBBoxMins = 32
            Me.OffsetV3FatBBoxMaxes = Me.OffsetV3FatBBoxMins + (Me.NumBins * 16) '176
            Me.OffsetMeshCullInfos = Me.OffsetV3FatBBoxMaxes + (Me.NumBins * 16) '320


            w.Write(Me.NumBins)
            w.Write(Me.NumMeshes)
            w.Write(Me.OffsetV3FatBBoxMins)
            w.Write(Me.OffsetV3FatBBoxMaxes)

            w.Write(Me.OffsetMeshCullInfos)
            w.Write(Me.Pad)

            For i = 0 To Me.NumBins - 1
                w.Write(Me.V3FatBBoxMins(i))
            Next

            For i = 0 To Me.NumBins - 1
                w.Write(Me.V3FatBBoxMaxes(i))
            Next

            For i = 0 To Me.NumMeshes - 1
                Me.MeshCullInfos(i).Offset = Me.OffsetMeshCullInfos + (Me.NumMeshes * 4) + (i * 400)
                w.Write(Me.MeshCullInfos(i).Offset)
            Next

            For i = 0 To Me.NumMeshes - 1
                Me.MeshCullInfos(i).OffsetCullingBins = Me.MeshCullInfos(i).Offset + 4
                w.Write(Me.MeshCullInfos(i).OffsetCullingBins)

                For j = 0 To Me.NumBins - 1
                    Me.MeshCullInfos(i).CullingBins(j).Offset = Me.MeshCullInfos(i).OffsetCullingBins + (Me.NumBins * 4) + (j * 40)
                    w.Write(Me.MeshCullInfos(i).CullingBins(j).Offset)
                Next

                For j = 0 To Me.NumBins - 1
                    For h = 0 To Me.MeshCullInfos(i).CullingBins(j).Elements.Length - 1
                        w.Write(Me.MeshCullInfos(i).CullingBins(j).Elements(h))
                    Next
                Next
            Next

        End Sub

        Public Property NumBins As UInteger       'always "9", number of Float4 values at Floats1 and Floats2, and/or number of CullInfoData?
        Public Property NumMeshes As UInteger
        Public Property OffsetV3FatBBoxMins As UInteger
        Public Property OffsetV3FatBBoxMaxes As UInteger

        Public Property OffsetMeshCullInfos As UInteger
        Public Property Pad As UInteger() = New UInteger(3 - 1) {}    'always "0", "0", "0"    padding?

        Public Property V3FatBBoxMins As Microsoft.DirectX.Vector4()             'Float4 values, array of size NumBins (ussually 9)
        Public Property V3FatBBoxMaxes As Microsoft.DirectX.Vector4()              'Float4 values, array of size NumBins (ussually 9)

        Public Property MeshCullInfos As MeshCullInfo()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class MeshCullInfo
        'bxd::tMeshCullInfo
        Public Sub New()

        End Sub

        Public Sub New(ByVal BaseOffset As Long, ByVal NumBins As UInteger, ByVal r As FileReader)
            Me.Offset = r.ReadUInt32
            Dim m_NextOffset As Long = r.BaseStream.Position
            r.BaseStream.Position = BaseOffset + Me.Offset
            Me.Load(BaseOffset, NumBins, r)
            r.BaseStream.Position = m_NextOffset
        End Sub

        Public Sub Load(ByVal BaseOffset As Long, ByVal NumBins As UInteger, ByVal r As FileReader)
            Me.OffsetCullingBins = r.ReadUInt32
            r.BaseStream.Position = BaseOffset + Me.OffsetCullingBins

            Me.CullingBins = New CullingBin(NumBins - 1) {}
            For j = 0 To Me.CullingBins.Length - 1
                Me.CullingBins(j) = New CullingBin(BaseOffset, r)
            Next
        End Sub

        'Public Sub Save(ByVal w As FileWriter)

        'End Sub
        Public Property Offset As UInteger
        Public Property OffsetCullingBins As UInteger
        Public Property CullingBins As CullingBin() ' = New CullInfoData(9 - 1) {}

    End Class

    Public Class CullingBin
        'bxd::tCullingBin
        Public Sub New()

        End Sub

        Public Sub New(ByVal BaseOffset As Long, ByVal r As FileReader)
            Me.Offset = r.ReadUInt32
            Dim m_NextOffset As Long = r.BaseStream.Position
            r.BaseStream.Position = BaseOffset + Me.Offset
            Me.Load(r)
            r.BaseStream.Position = m_NextOffset
        End Sub

        Public Sub Load(ByVal r As FileReader)
            'm_iStartElement    'T_INT4 ???
            'm_iNumElements     'array of T_INT4 ???
            Me.Elements = r.ReadIntegers(10)
        End Sub

        'Public Sub Save(ByVal w As FileWriter)

        'End Sub
        Public Property Offset As Integer
        Public Property Elements As Integer() = New Integer(10 - 1) {}   '10 integer values (indexes ?), "0" and "-1" value possible (default/unused value ??) 

    End Class
End Namespace