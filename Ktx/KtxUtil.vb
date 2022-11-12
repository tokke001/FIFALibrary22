Imports System.Drawing
Imports BCnEncoder.Shared

'https://www.khronos.org/registry/KTX/specs/1.0/ktxspec_v1.html

Namespace Ktx
    Public Class KtxUtil

        Public Shared Function GetBitmapFromKtx(FileKtx As BCnEncoder.Shared.KtxFile) As Bitmap
            Dim Output As RawImage '= New RawImage(FileKtx.Faces.Count)() {}

            Dim Width As UInteger = FileKtx.MipMaps(0).Faces(0).Width
            Dim Height As UInteger = FileKtx.MipMaps(0).Faces(0).Height
            Dim TextureFormat As ETextureFormat = FileKtx.Header.GlInternalFormat.ToETextureFormat
            Dim Size As UInteger = FileKtx.MipMaps(0).Faces(0).Data.Length
            Dim SwapEndian_DxtBlock As Boolean = False
            Output = New RawImage(Width, Height, TextureFormat, Size, SwapEndian_DxtBlock) 'With {}
            Output.RawData(SwapEndian_DxtBlock) = FileKtx.MipMaps(0).Faces(0).Data

            Return Output.Bitmap
        End Function

        Public Shared Function GetBitmapsFromKtx(FileKtx As BCnEncoder.Shared.KtxFile) As Bitmap()
            Dim Output As Bitmap() = New Bitmap(FileKtx.Header.NumberOfFaces - 1) {}

            For f = 0 To Output.Length - 1
                Dim Width As UInteger = FileKtx.MipMaps(0).Faces(f).Width
                Dim Height As UInteger = FileKtx.MipMaps(0).Faces(f).Height
                Dim TextureFormat As ETextureFormat = FileKtx.Header.GlInternalFormat.ToETextureFormat
                Dim Size As UInteger = FileKtx.MipMaps(0).Faces(f).Data.Length
                Dim SwapEndian_DxtBlock As Boolean = False
                Dim m_RawImage As New RawImage(Width, Height, TextureFormat, Size, SwapEndian_DxtBlock) 'With {}
                m_RawImage.RawData(SwapEndian_DxtBlock) = FileKtx.MipMaps(0).Faces(f).Data

                Output(f) = m_RawImage.Bitmap
            Next

            Return Output
        End Function

        Public Shared Function SetBitmapToKtx(ByVal FileKtx As KtxFile, ByVal Bitmap As Bitmap) As KtxFile   '2D - Keep Formats from Ktxfile
            Return SetBitmapsToKtx(New Bitmap() {Bitmap}, FileKtx.Header.GlInternalFormat.ToETextureFormat, FileKtx.Header.NumberOfMipmapLevels)
        End Function

        Public Shared Function SetBitmapToKtx(ByVal Bitmap As Bitmap, ByVal TextureFormat As ETextureFormat, ByVal NumMips As UShort) As KtxFile   '2D
            Return SetBitmapsToKtx(New Bitmap() {Bitmap}, TextureFormat, NumMips)
        End Function

        Public Shared Function SetBitmapsToKtx(ByVal FileKtx As KtxFile, ByVal Bitmaps As Bitmap()) As KtxFile   'cubic - Keep Formats from Ktxfile
            Return SetBitmapsToKtx(Bitmaps, FileKtx.Header.GlInternalFormat.ToETextureFormat, FileKtx.Header.NumberOfMipmapLevels)
        End Function

        Public Shared Function SetBitmapsToKtx(ByVal Bitmaps As Bitmap(), ByVal TextureFormat As ETextureFormat, ByVal NumMips As UShort) As KtxFile   'cubic
            Dim m_RawImages As RawImage()() = New RawImage(Bitmaps.Length - 1)() {} '

            For f = 0 To m_RawImages.Length - 1
                Dim Width As UInteger = Bitmaps(f).Width
                Dim Height As UInteger = Bitmaps(f).Height
                Dim SwapEndian_DxtBlock As Boolean = False
                Dim srcBitmap As Bitmap = Bitmaps(f)

                m_RawImages(f) = New RawImage(NumMips - 1) {}

                For i = 0 To m_RawImages(f).Length - 1
                    Dim size As Integer = GraphicUtil.GetTextureSize(Width, Height, TextureFormat)

                    m_RawImages(f)(i) = New RawImage(Width, Height, TextureFormat, size, SwapEndian_DxtBlock)
                    m_RawImages(f)(i).Bitmap = srcBitmap

                    srcBitmap = GraphicUtil.ReduceBitmap(srcBitmap)
                    Width = (Width \ 2)
                    Height = (Height \ 2)
                Next i
            Next f

            Dim m_KtxFile As New KtxFile
            m_KtxFile.FromRawImages(m_RawImages)
            Return m_KtxFile
        End Function
    End Class
End Namespace