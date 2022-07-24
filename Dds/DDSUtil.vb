Imports System.Drawing
Imports BCnEncoder.Shared

Namespace Dds

    Public Class DdsUtil

        Public Shared Function IsSupportedFormat(ByVal Format As ETextureFormat) As Boolean
            Return Format.ToDxgiFormat() <> DXGI_FORMAT.DXGI_FORMAT_UNKNOWN
        End Function



        'Public Shared Function GetDds(ByVal RawImages As RawImage()()) As DdsFile ', ByVal TextureType As ETextureType, ByVal TextureFormat As ETextureFormat, ByVal m_Width As UInteger, ByVal m_Height As UInteger) As DdsFile
        '    Dim PreferDxt10Header As Boolean = False

        '    Dim output As DdsFile

        '    Dim DxgiFormat As DXGI_FORMAT = RawImages(0)(0).TextureFormat.ToDxgiFormat
        '    Dim dxt10Header As DdsHeaderDx10 = Nothing
        '    Dim Header As DdsHeader = DdsHeader.InitializeDx10Header(RawImages(0)(0).Width, RawImages(0)(0).Height, DxgiFormat, PreferDxt10Header, dxt10Header)

        '    output = New DdsFile(Header, dxt10Header)

        '    Dim NumFaces As UInteger = RawImages.Length
        '    Dim NumMipMaps As UInteger = RawImages(0).Length
        '    Dim TotalSize As UInteger = 0

        '    For f = 0 To NumFaces - 1
        '        output.Faces.Add(New DdsFace(RawImages(f)(0).Width, RawImages(f)(0).Height, RawImages(f)(0).RawData(False).Length, CInt(NumMipMaps)))
        '        For mip = 0 To RawImages(f).Length - 1
        '            Dim encoded As Byte() = RawImages(f)(mip).RawData(False)
        '            output.Faces(f).MipMaps(mip) = New DdsMipMap(encoded, RawImages(f)(mip).Width, RawImages(f)(mip).Height)
        '            TotalSize += encoded.Length
        '        Next

        '    Next

        '    output.Header.dwDepth = NumFaces
        '    output.Header.dwMipMapCount = NumMipMaps
        '    'If numMipMaps > 1 Then
        '    '    output.Header.dwCaps = output.Header.dwCaps Or HeaderCaps.DDSCAPS_COMPLEX Or HeaderCaps.DDSCAPS_MIPMAP
        '    'End If

        '    If NumFaces > 1 Or NumMipMaps > 1 Then  'Or (TextureType = ETextureType.TEXTURE_CUBEMAP Or TextureType = ETextureType.TEXTURE_VOLUME)
        '        output.Header.dwCaps = output.Header.dwCaps Or HeaderCaps.DDSCAPS_COMPLEX
        '        If NumMipMaps > 1 Then
        '            output.Header.dwCaps = output.Header.dwCaps Or HeaderCaps.DDSCAPS_MIPMAP
        '        End If
        '    End If

        '    If NumFaces > 1 Then
        '        If NumFaces = 6 Then
        '            output.Header.dwCaps2 = HeaderCaps2.DDSCAPS2_CUBEMAP
        '            output.Header.dwCaps2 = output.Header.dwCaps2 Or HeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEX Or HeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEY Or HeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEZ Or HeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEX Or HeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEY Or HeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEZ

        '        Else
        '            output.Header.dwCaps2 = HeaderCaps2.DDSCAPS2_VOLUME
        '        End If
        '    End If

        '    If (output.Header.dwFlags And HeaderFlags.DDSD_LINEARSIZE) <> 0 Then
        '        output.Header.dwPitchOrLinearSize = TotalSize 'CUInt(Math.Truncate(textureData.TotalSize))
        '    End If


        '    Return output
        'End Function

        'Public Shared Function GetRawImages(FileDds As BCnEncoder.Shared.DdsFile) As RawImage()()
        '    Dim Output As RawImage()() = New RawImage(FileDds.Faces.Count - 1)() {}
        '    Dim TextureFormat As ETextureFormat = If(FileDds.Header.ddsPixelFormat.IsDxt10Format, FileDds.Dx10Header.dxgiFormat.ToETextureFormat, FileDds.Header.ddsPixelFormat.DxgiFormat.ToETextureFormat)

        '    For f = 0 To Output.Length - 1
        '        Output(f) = New RawImage(FileDds.Faces(f).MipMaps.Length - 1) {}
        '        For mip = 0 To Output(f).Length - 1
        '            Dim Width As UInteger = FileDds.Faces(f).MipMaps(mip).Width
        '            Dim Height As UInteger = FileDds.Faces(f).MipMaps(mip).Height
        '            Dim Size As UInteger = FileDds.Faces(f).MipMaps(mip).Data.Length
        '            Dim SwapEndian_DxtBlock As Boolean = False
        '            Output(f)(mip) = New RawImage(Width, Height, TextureFormat, Size, SwapEndian_DxtBlock) 'With {}
        '            Output(f)(mip).RawData(SwapEndian_DxtBlock) = FileDds.Faces(f).MipMaps(mip).Data
        '        Next
        '    Next

        '    Return Output
        'End Function

        Public Shared Function GetBitmapFromDds(FileDds As BCnEncoder.Shared.DdsFile) As Bitmap
            Dim Output As RawImage '= New RawImage(FileDds.Faces.Count)() {}

            Dim Width As UInteger = FileDds.Faces(0).MipMaps(0).Width
            Dim Height As UInteger = FileDds.Faces(0).MipMaps(0).Height
            Dim TextureFormat As ETextureFormat = If(FileDds.Header.ddsPixelFormat.IsDx10Format, FileDds.Dx10Header.dxgiFormat.ToETextureFormat, FileDds.Header.ddsPixelFormat.DxgiFormat.ToETextureFormat)
            Dim Size As UInteger = FileDds.Faces(0).MipMaps(0).Data.Length
            Dim SwapEndian_DxtBlock As Boolean = False
            Output = New RawImage(Width, Height, TextureFormat, Size, SwapEndian_DxtBlock) 'With {}
            Output.RawData(SwapEndian_DxtBlock) = FileDds.Faces(0).MipMaps(0).Data

            Return Output.Bitmap
        End Function

        Public Shared Function GetBitmapsFromDds(FileDds As BCnEncoder.Shared.DdsFile) As Bitmap()
            Dim Output As Bitmap() = New Bitmap(FileDds.Faces.Count - 1) {}

            For f = 0 To Output.Length - 1
                Dim Width As UInteger = FileDds.Faces(f).MipMaps(0).Width
                Dim Height As UInteger = FileDds.Faces(f).MipMaps(0).Height
                Dim TextureFormat As ETextureFormat = If(FileDds.Header.ddsPixelFormat.IsDx10Format, FileDds.Dx10Header.dxgiFormat.ToETextureFormat, FileDds.Header.ddsPixelFormat.DxgiFormat.ToETextureFormat)
                Dim Size As UInteger = FileDds.Faces(f).MipMaps(0).Data.Length
                Dim SwapEndian_DxtBlock As Boolean = False
                Dim m_RawImage As New RawImage(Width, Height, TextureFormat, Size, SwapEndian_DxtBlock) 'With {}
                m_RawImage.RawData(SwapEndian_DxtBlock) = FileDds.Faces(f).MipMaps(0).Data

                Output(f) = m_RawImage.Bitmap
            Next

            Return Output
        End Function

        Public Shared Function SetBitmapToDds(ByVal FileDds As DdsFile, ByVal Bitmap As Bitmap) As DdsFile   '2D - Keep Formats from Ddsfile
            Return SetBitmapsToDds(New Bitmap() {Bitmap}, If(FileDds.Header.ddsPixelFormat.IsDx10Format, FileDds.Dx10Header.dxgiFormat.ToETextureFormat, FileDds.Header.ddsPixelFormat.DxgiFormat.ToETextureFormat), FileDds.Faces(0).MipMaps.Length)
        End Function

        Public Shared Function SetBitmapToDds(ByVal Bitmap As Bitmap, ByVal TextureFormat As ETextureFormat, ByVal NumMips As UShort) As DdsFile   '2D
            Return SetBitmapsToDds(New Bitmap() {Bitmap}, TextureFormat, NumMips)
        End Function

        Public Shared Function SetBitmapsToDds(ByVal FileDds As DdsFile, ByVal Bitmaps As Bitmap()) As DdsFile   'cubic - Keep Formats from Ddsfile
            Return SetBitmapsToDds(Bitmaps, If(FileDds.Header.ddsPixelFormat.IsDx10Format, FileDds.Dx10Header.dxgiFormat.ToETextureFormat, FileDds.Header.ddsPixelFormat.DxgiFormat.ToETextureFormat), FileDds.Faces(0).MipMaps.Length)
        End Function

        Public Shared Function SetBitmapsToDds(ByVal Bitmaps As Bitmap(), ByVal TextureFormat As ETextureFormat, ByVal NumMips As UShort) As DdsFile   'cubic
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

            Dim m_DdsFile As New DdsFile
            Dim PreferDx10Header As Boolean = False
            m_DdsFile.FromRawImages(m_RawImages, PreferDx10Header)
            Return m_DdsFile
        End Function

    End Class
End Namespace