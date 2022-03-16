Namespace Rx2
    Public Class TextureLevel
        Inherits RawImage
        ' Methods
        'Public Sub New()
        'MyBase.New
        'End Sub
        'Public Sub New(ByVal RWRaster As RWGObjectType_Raster, ByVal Size As UInteger, ByVal SwapEndian_DxtBlock As Boolean)
        Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal TextureFormat As Rw.SurfaceFormat, ByVal SwapEndian_DxtBlock As Boolean)
            MyBase.New(width, height, GraphicUtil.GetEFromRWTextureFormat(TextureFormat), 0, SwapEndian_DxtBlock, True) 'untile needed
            'Me.m_SwapEndian = SwapEndian
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.CalcPitchLinesSize()

            MyBase.Load(r)

        End Sub

        Public Sub Save(ByVal SwapEndian_DxtBlock As Boolean, ByVal w As FileWriter)

            Me.CalcPitchLinesSize()

            MyBase.Save(SwapEndian_DxtBlock, w)

        End Sub

        Public Sub CalcPitchLinesSize() '(ByVal width As Integer, ByVal height As Integer, ByVal TextureFormat As ETextureFormat)
            'https://docs.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide
            'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/page-2#post-6600051

            Me.Pitch = GraphicUtil.GetTexturePitch(MyBase.Width, MyBase.TextureFormat)
            MyBase.Size = GraphicUtil.GetTextureSize(MyBase.Width, MyBase.Height, MyBase.TextureFormat)
            Me.Lines = MyBase.Size \ Me.Pitch
        End Sub

        ' Fields
        Public Property Pitch As UInteger
        Public Property Lines As UInteger

    End Class

End Namespace