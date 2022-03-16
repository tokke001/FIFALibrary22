Namespace Rw.EA.FxShader
    Public Class FxRenderableSimple
        'EA::FxShader::FxRenderableSimple
        Inherits RWObject
        Public Const TYPE_CODE As Rw.SectionTypeCode = SectionTypeCode.EA_FxShader_FxRenderableSimple
        Public Const ALIGNMENT As Integer = 4

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            MyBase.New(RwArena)
        End Sub

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena, ByVal r As FileReader)
            MyBase.New(RwArena)
            Me.Load(r)
        End Sub

        Public Overrides Sub Load(ByVal r As FileReader)

            Me.PMesh = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), Rw.Graphics.EmbeddedMesh)              'reference to sectionindex &H20009 (MESH)
            Me.PVertexDescriptor = CType(Me.RwArena.Sections.GetObject(r.ReadInt32), Rw.Graphics.VertexDescriptor) 'reference to sectionindex &H20004 (VERTEX_DESCRIPTOR)
            Me.PrimitiveType = r.ReadUInt32
            Me.MaterialName = FifaUtil.Read64SizedString(r)     'dump: always 64 length --> ex. "ball_" "head_" "eyes_"
            Me.EffectName = FifaUtil.Read64SizedString(r)       'dump: always 64 length --> filename which found in shaders folder (inside big) (env_simplelightmap.FX) --> usually "default"

        End Sub

        Public Overrides Sub Save(ByVal w As FileWriter)

            w.Write(Me.RwArena.Sections.IndexOf(Me.PMesh))
            w.Write(Me.RwArena.Sections.IndexOf(Me.PVertexDescriptor))
            w.Write(CUInt(Me.PrimitiveType))
            FifaUtil.Write64SizedString(w, Me.MaterialName)
            FifaUtil.Write64SizedString(w, Me.EffectName)

        End Sub

        Public Property PMesh As Rw.Graphics.EmbeddedMesh
        Public Property PVertexDescriptor As Rw.Graphics.VertexDescriptor
        Public Property MaterialName As String
        Public Property EffectName As String
        Public Property PrimitiveType As Microsoft.DirectX.Direct3D.PrimitiveType

        Public Overrides Function GetTypeCode() As Rw.SectionTypeCode
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class
End Namespace

'Namespace Rw.Graphics
'    Public Enum PrimitiveType  'rw::graphics::PrimitiveType --> not sure this is same as FIFA ??
'        PRIMITIVETYPE_NA = 0
'        PRIMITIVETYPE_POINTS = 1
'        PRIMITIVETYPE_LINES = 2
'        PRIMITIVETYPE_LINESTRIP = 3
'        PRIMITIVETYPE_TRIANGLES = 4
'        PRIMITIVETYPE_TRIANGLEFAN = 5
'        PRIMITIVETYPE_TRIANGLESTRIPS = 6
'        PRIMITIVETYPE_RECTS = 8
'        PRIMITIVETYPE_QUADS = 13
'        PRIMITIVETYPE_LINEPATCH = 16
'        PRIMITIVETYPE_TRIPATCH = 17
'        PRIMITIVETYPE_QUADPATCH = 18
'        PRIMITIVETYPE_MAX = 19
'    End Enum
'End Namespace