Namespace Rw.Bxd
    Public Class RenderModel
        'bxd::tRenderModel
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.MODELRENDER_ARENAID
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)
            Dim BaseOffset As Long = r.BaseStream.Position

            Me.NumMeshes = r.ReadInt32 '"1" value
            Me.OffsetMeshes = r.ReadUInt32  '"8" value  : offset to array of bxd::tRenderMesh

            r.BaseStream.Position = BaseOffset + Me.OffsetMeshes
            Me.Meshes = New RenderMesh(Me.NumMeshes - 1) {}  '"0" value  'padding?
            For i = 0 To Me.Meshes.Length - 1
                Me.Meshes(i) = New RenderMesh(BaseOffset, Me.RwArena, r)
            Next

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.NumMeshes = Me.Meshes.Length
            Me.OffsetMeshes = 8

            w.Write(Me.NumMeshes)
            w.Write(Me.OffsetMeshes)

            For i = 0 To Me.NumMeshes - 1
                Me.Meshes(i).Save(BaseOffset, w)
            Next

        End Sub

        Public Property NumMeshes As Integer       '"1" value
        Public Property OffsetMeshes As UInteger       '"8" value  
        Public Property Meshes As RenderMesh()

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class RenderMesh
        'bxd::tRenderMesh
        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            Me.RwArena = RwArena
        End Sub

        Public Sub New(ByVal BaseOffset As Long, ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.Load(BaseOffset, r)
        End Sub

        Public Sub Load(ByVal BaseOffset As Long, ByVal r As FileReader)

            'Me.NumMeshes = r.ReadInt32 '"1" value
            'Me.OffsetMeshes = r.ReadUInt32  '"8" value  : offset to array of bxd::tRenderMesh

            Me.Flags = r.ReadUInt32  '"0" value  
            Me.BoneIndex = r.ReadInt32  '"0" value  
            Me.NumRenderObjects = r.ReadInt32

            Me.RenderObjects = New RenderObject(Me.NumRenderObjects - 1) {}
            For i = 0 To Me.NumRenderObjects - 1
                Me.RenderObjects(i) = New RenderObject(BaseOffset, Me.RwArena, r)
            Next

        End Sub

        Public Sub Save(ByVal BaseOffset As Long, ByVal w As FileWriter)
            Me.NumRenderObjects = Me.RenderObjects.Length

            w.Write(Me.Flags)
            w.Write(Me.BoneIndex)
            w.Write(Me.NumRenderObjects)

            '-- write with wrong offsets
            For i = 0 To Me.NumRenderObjects - 1
                w.Write(CInt(0))
            Next

            For i = 0 To Me.NumRenderObjects - 1
                Me.RenderObjects(i).Save(BaseOffset, w)
            Next i

            Dim EndOffset As Long = w.BaseStream.Position
            '-- write again with correct offsets
            For i = 0 To Me.NumRenderObjects - 1
                w.Write(Me.RenderObjects(i).Offset)
            Next
            '-- jump to end
            w.BaseStream.Position = EndOffset

        End Sub

        Private RwArena As Rw.Core.Arena.Arena
        Public Property Flags As UInteger       '"0" value  
        Public Property BoneIndex As Integer       '"0" value  
        Public Property NumRenderObjects As Integer
        Public Property RenderObjects As RenderObject()


    End Class

    Public Class RenderObject
        'bxd::tRenderObject
        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            Me.RwArena = RwArena
        End Sub
        'Public Sub New(ByVal Offset As Long, ByVal r As FileReader)
        'Me.m_Offset = Offset
        'Dim BaseOffset As Long = r.BaseStream.Position
        '   r.BaseStream.Position = Me.m_Offset
        'Me.Load(r)
        '   r.BaseStream.Position = BaseOffset
        'End Sub
        Public Sub New(ByVal BaseOffset As Long, ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            Me.RwArena = RwArena
            Me.Offset = r.ReadUInt32
            Dim m_NextOffset As Long = r.BaseStream.Position
            r.BaseStream.Position = BaseOffset + Me.Offset
            Me.Load(r)
            r.BaseStream.Position = m_NextOffset
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.BBox = New BBox(r)  'm_v3Min and m_v3Max

            Me.PFxRenderableSimple = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), EA.FxShader.FxRenderableSimple)    'index of SectIndex_HEF0004FxRenderableSimple
            Me.PFxMaterial = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), EA.FxShader.FxMaterial)            'index of SectIndex_HEF0005FxMaterial
            Me.PMeshCullInfo = r.ReadInt32 'CType(Me.RwArena.Sections.GetObject(r.ReadInt32), MeshCullInfo)          '--> always "00 80", followed with an index wich is refer to sectionmanifest.ArenaSectionSubreferences.records -->  "bxd::tMeshCullInfo" 
            'Console.Error.WriteLine("PMeshCullInfo = " & Me.PMeshCullInfo)
        End Sub

        Public Sub Save(ByVal BaseOffset As Long, ByVal w As FileWriter)
            Me.Offset = w.BaseStream.Position - BaseOffset

            Me.BBox.Save(w)

            w.Write(Me.RwArena.Sections.IndexOf(Me.PFxRenderableSimple))
            w.Write(Me.RwArena.Sections.IndexOf(Me.PFxMaterial))
            w.Write(Me.PMeshCullInfo) 'Me.RwArena.Sections.IndexOf(Me.PMeshCullInfo))

        End Sub

        Private RwArena As Rw.Core.Arena.Arena
        Public Property Offset As UInteger
        Public Property BBox As New BBox   '2xVector4 (min, max)
        Public Property PFxRenderableSimple As EA.FxShader.FxRenderableSimple        'section index of EA_FxShader_FxRenderableSimple ( &HEF0004)
        Public Property PFxMaterial As EA.FxShader.FxMaterial      'section index of EA_FxShader_FxMaterial ( &HEF0005)
        Public Property PMeshCullInfo As Integer 'MeshCullInfo       '--> always "00 80", followed with an index wich is refer to sectionmanifest.ArenaSectionSubreferences.records -->  "bxd::tMeshCullInfo" 

    End Class

End Namespace