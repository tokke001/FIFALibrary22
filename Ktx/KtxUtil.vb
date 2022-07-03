Imports BCnEncoder.Shared

'https://www.khronos.org/registry/KTX/specs/1.0/ktxspec_v1.html

Namespace Ktx
    Public Class KtxUtil

        Public Shared Function GetKtx(ByVal RawImages As RawImage()()) As KtxFile ', ByVal TextureType As ETextureType, ByVal TextureFormat As ETextureFormat, ByVal m_Width As UInteger, ByVal m_Height As UInteger) As DdsFile
            Dim output As KtxFile

            Dim GlFormat As (GlFormat As GLFormat, InternalFormat As GlInternalFormat, GlType As GLType) = RawImages(0)(0).TextureFormat.ToGlFormat

            If RawImages(0)(0).TextureFormat.IsCompressedFormat Then 'GraphicUtil.IsCompressedFormat(RawImages(0)(0).TextureFormat) Then
                'compressedEncoder = GetEncoder(OutputOptions.format)
                'If compressedEncoder Is Nothing Then
                '    Throw New NotSupportedException($"This format is not supported: {OutputOptions.format}")
                'End If

                output = New KtxFile(KtxHeader.InitializeCompressed(RawImages(0)(0).Width, RawImages(0)(0).Height, GlFormat.InternalFormat, GlFormat.GlFormat))
            Else

                output = New KtxFile(KtxHeader.InitializeUncompressed(RawImages(0)(0).Width,
                                                                      RawImages(0)(0).Height,
                                                                      GlFormat.GlType,
                                                                      GlFormat.GlFormat,
                                                                      GlFormat.GlType.GetTypeSize,
                                                                      GlFormat.InternalFormat,
                                                                      GlFormat.GlFormat))
            End If

            Dim NumFaces As UInteger = RawImages.Length
            Dim NumMipMaps As UInteger = RawImages(0).Length

            For i As UInteger = 0 To NumMipMaps - 1
                output.MipMaps.Add(New KtxMipmap(0, 0, 0, NumFaces))
            Next i

            For f As Integer = 0 To NumFaces - 1

                'Dim mipChain = MipMapper.GenerateMipChain(faces(f), NumMipMaps)

                For i As Integer = 0 To NumMipMaps - 1
                    Dim encoded As Byte() = RawImages(f)(i).RawData(False)
                    'If OutputOptions.format.IsCompressedFormat() Then
                    '    Dim blocksWidth As Integer
                    '    Dim blocksHeight As Integer
                    '    Dim blocks = ImageToBlocks.ImageTo4X4(mipChain(i).Frames(0), blocksWidth, blocksHeight)
                    '    encoded = compressedEncoder.Encode(blocks, blocksWidth, blocksHeight, OutputOptions.quality, Not Debugger.IsAttached AndAlso Options.multiThreaded)
                    'Else
                    '    Dim mipPixels As Object
                    '    If Not mipChain(i).TryGetSinglePixelSpan(mipPixels) Then
                    '        Throw New Exception("Cannot get pixel span.")
                    '    End If
                    '    encoded = uncompressedEncoder.Encode(mipPixels)
                    'End If

                    If f = 0 Then
                        output.MipMaps(i) = New KtxMipmap(CUInt(encoded.Length), RawImages(f)(0).Width, RawImages(f)(0).Height, NumFaces)
                    End If

                    output.MipMaps(i).Faces(f) = New KtxMipFace(encoded, RawImages(f)(0).Width, RawImages(f)(0).Height)
                Next i

                'For Each image In mipChain
                '    image.Dispose()
                'Next image
            Next f

            output.Header.NumberOfFaces = NumFaces
            output.Header.NumberOfMipmapLevels = NumMipMaps

            Return output
        End Function

        Public Shared Function GetRawImages(FileKtx As BCnEncoder.Shared.KtxFile) As RawImage()()
            Dim Output As RawImage()() = New RawImage(FileKtx.Header.NumberOfFaces - 1)() {}
            Dim TextureFormat As ETextureFormat = FileKtx.Header.GlInternalFormat.ToETextureFormat

            For f = 0 To Output.Length - 1
                Output(f) = New RawImage(FileKtx.Header.NumberOfMipmapLevels - 1) {}
                For mip = 0 To Output(f).Length - 1
                    Dim Width As UInteger = FileKtx.MipMaps(mip).Faces(f).Width
                    Dim Height As UInteger = FileKtx.MipMaps(mip).Faces(f).Height
                    Dim Size As UInteger = FileKtx.MipMaps(mip).Faces(f).Data.Length
                    Dim SwapEndian_DxtBlock As Boolean = False
                    Output(f)(mip) = New RawImage(Width, Height, TextureFormat, Size, SwapEndian_DxtBlock) 'With {}
                    Output(f)(mip).RawData(SwapEndian_DxtBlock) = FileKtx.MipMaps(mip).Faces(f).Data
                Next
            Next

            Return Output
        End Function

    End Class
End Namespace