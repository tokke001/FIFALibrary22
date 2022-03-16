Namespace Rx3
    Public Class TextureLevel
        Inherits RawImage
        ' Methods
        'Public Sub New()
        'MyBase.New
        'End Sub
        Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal dxtType As TextureFormat, ByVal SwapEndian_DxtBlock As Boolean)
            MyBase.New(width, height, GraphicUtil.GetEFromRx3TextureFormat(dxtType), 0, SwapEndian_DxtBlock)
        End Sub

        Public Overloads Sub Load(ByVal r As FileReader)

            Me.Pitch = r.ReadUInt32
            Me.Lines = r.ReadUInt32
            MyBase.Size = r.ReadUInt32      'size of pixels data (pitch * lines)
            Me.Padding = r.ReadUInt32
            MyBase.Load(r)

        End Sub

        Public Overloads Sub Save(ByVal SwapEndian_DxtBlock As Boolean, ByVal w As FileWriter)
            If MyBase.NeedToSaveRawData Then
                Me.CalcPitchLinesSize()
            End If

            w.Write(Me.Pitch)
            w.Write(Me.Lines)
            w.Write(MyBase.Size)
            w.Write(Me.Padding)
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
        Public Property Padding As UInteger

    End Class

End Namespace