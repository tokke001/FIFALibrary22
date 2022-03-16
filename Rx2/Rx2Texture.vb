Imports System.Drawing
Imports FIFALibrary22.Rw.Graphics

Namespace Rx2
    Public Class Texture
        ' Methods
        Public Sub New()

        End Sub
        Public Sub New(ByVal RWRaster As Raster, ByVal Size As UInteger, ByVal SwapEndian_DxtBlock As Boolean, ByVal r As FileReader)
            'Public Sub New(ByVal SwapEndian As Boolean, ByVal r As FileReader)
            'Me.m_SwapEndian = SwapEndian
            Me.m_RWRaster = RWRaster
            Me.TextureType = ETextureType.TEXTURE_2D    'unknown value at RWRaster: always 1 ...
            Me.NumFaces = 1     'unknown value at RWRaster: always 1 ...
            Me.m_SwapEndian_DxtBlock = SwapEndian_DxtBlock

            Me.Load(r)
        End Sub

        Public Function GetBitmap() As Bitmap
            Return Me.TextureFaces(0).TextureLevels(0).Bitmap
        End Function

        Public Function GetBitmap(ByVal TextureFace As Integer, ByVal TextureLevel As Integer) As Bitmap
            Return If((TextureLevel < Me.TextureFaces(TextureFace).TextureLevels.Length), Me.TextureFaces(TextureFace).TextureLevels(TextureLevel).Bitmap, Nothing)
        End Function

        Public Function GetDds() As DdsFile
            Dim m_RawImages = New RawImage(Me.TextureFaces.Length - 1)() {}
            For f = 0 To m_RawImages.Length - 1
                m_RawImages(f) = Me.TextureFaces(f).TextureLevels
            Next
            Return DdsUtil.GetDds(m_RawImages, TextureType, GraphicUtil.GetEFromRWTextureFormat(Me.m_RWRaster.D3d.Format.TextureFormat), Me.m_RWRaster.D3d.Format.Width, Me.m_RWRaster.D3d.Format.Height)
        End Function

        Public Sub Load(ByVal r As FileReader)
            Dim Fix_CreateMipmaps As Boolean = False     'fix: errors at function "ConvertToLinearTexture" with out of array

            Me.TextureFaces = New TextureFace(Me.NumFaces - 1) {}

            For f = 0 To Me.NumFaces - 1
                Me.TextureFaces(f) = New TextureFace()
                Dim width As Integer = Me.m_RWRaster.D3d.Format.Width    'each face same height/width from header -> so put here
                Dim height As Integer = Me.m_RWRaster.D3d.Format.Height  'each face same height/width from header -> so put here
                Dim SwapEndian_DxtBlock As Boolean = m_SwapEndian_DxtBlock

                Me.TextureFaces(f).TextureLevels = New TextureLevel(Me.m_RWRaster.NumMipLevels - 1) {}
                For i = 0 To Me.m_RWRaster.NumMipLevels - 1
                    Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, Me.m_RWRaster.D3d.Format.TextureFormat, SwapEndian_DxtBlock)

                    If i = 0 Or Fix_CreateMipmaps = False Then
                        Me.TextureFaces(f).TextureLevels(i).Load(r)
                    End If

                    width = (width \ 2)
                    height = (height \ 2)
                Next i

                If Fix_CreateMipmaps Then
                    Me.GenerateMipmaps(Me.m_RWRaster.NumMipLevels, Me.m_RWRaster.D3d.Format.TextureFormat)
                End If
            Next f

        End Sub

        Public Sub Save(ByVal SwapEndian_DxtBlock As Boolean, ByVal w As FileWriter)
            Me.m_SwapEndian_DxtBlock = SwapEndian_DxtBlock

            For f = 0 To Me.TextureFaces.Length - 1
                For i = 0 To Me.TextureFaces(f).TextureLevels.Length - 1
                    Me.TextureFaces(f).TextureLevels(i).Save(SwapEndian_DxtBlock, w)
                Next i
            Next f

        End Sub

        Public Function SetBitmap(ByVal bitmap As Bitmap) As Boolean
            Dim TextureFormat As ETextureFormat = GraphicUtil.GetEFromRWTextureFormat(Me.m_RWRaster.D3d.Format.TextureFormat)
            Dim NumLevels As UShort = Me.m_RWRaster.NumMipLevels

            Me.SetBitmap(bitmap, TextureFormat, NumLevels)

            Return True
        End Function

        Public Function SetBitmap(ByVal bitmap As Bitmap, ByVal TextureFormat As Rw.SurfaceFormat, ByVal NumLevels As UShort) As Boolean
            Dim FaceIndex As Integer = 0
            Dim SwapEndian_DxtBlock As Boolean = m_SwapEndian_DxtBlock

            'If Me.m_RWRaster.TextureType <> ETextureType.TEXTURE_2D Then
            'Return False
            'End If

            If (bitmap Is Nothing) Then
                Return False
            End If

            '1 - Generate Texture-Header    'not needed at RW4, RWraster is external section !
            Me.m_RWRaster.D3d.Format.Height = bitmap.Height
            Me.m_RWRaster.D3d.Format.Width = bitmap.Width
            Me.m_RWRaster.D3d.Format.TextureFormat = TextureFormat
            Me.m_RWRaster.NumMipLevels = NumLevels

            '2 - Set main Bitmap (level 0)
            Me.TextureFaces(FaceIndex).TextureLevels(0) = New TextureLevel(bitmap.Width, bitmap.Height, TextureFormat, SwapEndian_DxtBlock)
            Me.TextureFaces(FaceIndex).TextureLevels(0).CalcPitchLinesSize()
            Me.TextureFaces(FaceIndex).TextureLevels(0).Bitmap = bitmap
            '3 - Generate Mipmaps (from main)
            Me.GenerateMipmaps(NumLevels, TextureFormat)

            Return True
        End Function
        Public Function SetDds(ByVal DdsFile As DDSFile, Optional KeepRWTextureFormat As Boolean = False) As Boolean

        End Function

        Public Sub SetTextureEndian(SwapEndian_DxtBlock As Boolean)
            m_SwapEndian_DxtBlock = SwapEndian_DxtBlock

            For f = 0 To Me.TextureFaces.Length - 1
                For i = 0 To Me.TextureFaces(f).TextureLevels.Length - 1
                    Me.TextureFaces(f).TextureLevels(i).SetEndianFormat(SwapEndian_DxtBlock)
                    'Me.TextureFaces(f).TextureLevels(i) = New Rx3TextureLevelHeader(Me.Rx3TextureHeader.Width, Me.Rx3TextureHeader.Height, Me.Rx3TextureHeader.TextureFormat, Me.m_SwapEndian, SwapEndian_DxtBlock)
                Next
            Next

        End Sub

        Public Sub SetTextureTiling(Tiled360 As Boolean)
            'm_Tiled360 = Tiled360

            For f = 0 To Me.TextureFaces.Length - 1
                For i = 0 To Me.TextureFaces(f).TextureLevels.Length - 1
                    Me.TextureFaces(f).TextureLevels(i).SetTiling360Format(Tiled360)
                    'Me.TextureFaces(f).TextureLevels(i) = New Rx3TextureLevelHeader(Me.Rx3TextureHeader.Width, Me.Rx3TextureHeader.Height, Me.Rx3TextureHeader.TextureFormat, Me.m_SwapEndian, SwapEndian_DxtBlock)
                Next
            Next

        End Sub
        'Public Function GenerateMipmaps(ByVal NumLevels As UShort) As Boolean  'generate mipmaps

        'End Function

        Public Function GenerateMipmaps(ByVal NumLevels As UShort, ByVal TextureFormat As Rw.SurfaceFormat) As Boolean  'generate mipmaps
            'Me.Rx3TextureHeader.NumLevels = NumLevels

            For f = 0 To Me.NumFaces - 1
                'Me.TextureFaces(f) = New Rx3TextureFace()
                Dim srcBitmap As Bitmap = Me.TextureFaces(f).TextureLevels(0).Bitmap
                Dim width As Integer = srcBitmap.Width
                Dim height As Integer = srcBitmap.Height
                Dim SwapEndian_DxtBlock As Boolean = m_SwapEndian_DxtBlock

                ReDim Preserve Me.TextureFaces(f).TextureLevels(NumLevels - 1)
                For i = 0 To NumLevels - 1

                    If i <> 0 Then
                        Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, TextureFormat, SwapEndian_DxtBlock)
                        Me.TextureFaces(f).TextureLevels(i).CalcPitchLinesSize()

                        srcBitmap = GraphicUtil.ReduceBitmap(srcBitmap)
                        Me.TextureFaces(f).TextureLevels(i).Bitmap = srcBitmap
                    End If

                    width = (width \ 2)
                    height = (height \ 2)
                Next i
            Next f

            Return True
        End Function

        ' Fields
        Public Property m_RWRaster As Raster
        Public Property TextureType As ETextureType '= ETextureType.TEXTURE_2D
        Public Property TextureFaces As TextureFace()
        Public Property m_SwapEndian_DxtBlock As Boolean

        Public Property NumFaces As UShort '= 1
        'Private NumLevels As UShort

        'Private m_SwapEndian As Boolean
    End Class

    Public Class TextureFace
        Public Property TextureLevels As TextureLevel()    'mipmaps

    End Class
End Namespace