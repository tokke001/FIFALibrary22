Imports BCnEncoder.Shared

Namespace Dds
    Public Module DdsExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Sub FromRawImages(ByVal m_DdsFile As DdsFile, ByVal RawImages As RawImage()(), ByVal preferDx10Header As Boolean)
            If Not DdsUtil.IsSupportedFormat(RawImages(0)(0).TextureFormat) Then
                Throw New ArgumentException($"Unsupported format {RawImages(0)(0).TextureFormat} in textureData.", RawImages(0)(0).TextureFormat.ToString)
            End If

            '-- (header, dx10Header) = DdsHeader.InitializeFor(textureData.Width, textureData.Height, textureData.Format, preferDxt10Header)
            Dim DxgiFormat As DXGI_FORMAT = RawImages(0)(0).TextureFormat.ToDxgiFormat
            If DxgiFormat = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN Then
                Throw New ArgumentException("The requested format is not supported!", RawImages(0)(0).TextureFormat.ToString)
            End If
            m_DdsFile.Header = DdsHeader.InitializeDx10Header(RawImages(0)(0).Width, RawImages(0)(0).Height, DxgiFormat, preferDx10Header, m_DdsFile.Dx10Header)
            'If Format() = CompressionFormat.BC1WithAlpha Then
            '    Header.ddsPixelFormat.dwFlags = Header.ddsPixelFormat.dwFlags Or PixelFormatFlags.DDPF_ALPHAPIXELS
            'End If


            If RawImages.HasMipLevels Then ' MipMaps
                m_DdsFile.Header.dwCaps = m_DdsFile.Header.dwCaps Or HeaderCaps.DDSCAPS_MIPMAP Or HeaderCaps.DDSCAPS_COMPLEX
                m_DdsFile.Header.dwMipMapCount = CUInt(Math.Truncate(RawImages(0).Count))
            End If
            If RawImages.IsCubeMap Then ' CubeMap
                m_DdsFile.Header.dwCaps = m_DdsFile.Header.dwCaps Or HeaderCaps.DDSCAPS_COMPLEX
                m_DdsFile.Header.dwCaps2 = m_DdsFile.Header.dwCaps2 Or HeaderCaps2.DDSCAPS2_CUBEMAP Or
                                                            HeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEX Or
                                                            HeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEX Or
                                                            HeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEY Or
                                                            HeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEY Or
                                                            HeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEZ Or
                                                            HeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEZ
            End If

            If (m_DdsFile.Header.dwFlags And HeaderFlags.DDSD_LINEARSIZE) <> 0 Then
                m_DdsFile.Header.dwPitchOrLinearSize = RawImages.TotalSize
            End If

            Dim uWidth = CUInt(RawImages(0)(0).Width)
            Dim uHeight = CUInt(RawImages(0)(0).Height)

            m_DdsFile.Faces.Clear()

            For Each face In RawImages
                Dim ddsFace As New DdsFace(uWidth, uHeight, CUInt(face(0).RawData(False).Length), RawImages(0).Length)
                Dim size As Integer = GraphicUtil.GetTextureSize(RawImages(0)(0).Width, RawImages(0)(0).Height, RawImages(0)(0).TextureFormat) '((((Me.dwWidth / 4) * Me.dwHeight) / 4) * &H10)
                Dim m_Width As UInteger = CUInt(RawImages(0)(0).Width)
                Dim m_Height As UInteger = CUInt(RawImages(0)(0).Height)
                For m = 0 To face.Count - 1
                    Dim m_array As Byte() = face(m).RawData(False)
                    If size <> m_array.Length Then
                        ReDim m_array(size - 1)
                    End If
                    ddsFace.MipMaps(m) = New DdsMipMap(m_array, m_Width, m_Height)
                    m_Width \= 2
                    m_Height \= 2
                    size = GraphicUtil.GetTextureSize(m_Width, m_Height, RawImages(0)(0).TextureFormat)  '(size / 4)

                Next m
                m_DdsFile.Faces.Add(ddsFace)
            Next face
        End Sub

        <System.Runtime.CompilerServices.Extension>
        Public Function ToRawImages(ByVal m_DdsFile As DdsFile) As RawImage()()
            Dim data As RawImage()() = New RawImage(m_DdsFile.Faces.Count - 1)() {} 'BCnTextureData(compressionFormat, CInt(Math.Truncate(header.dwWidth)), CInt(Math.Truncate(header.dwHeight)), CInt(Math.Truncate(header.dwMipMapCount)), isCubeMap)
            Dim TextureFormat As ETextureFormat = If(m_DdsFile.Header.ddsPixelFormat.IsDx10Format, m_DdsFile.Dx10Header.dxgiFormat.ToETextureFormat, m_DdsFile.Header.ddsPixelFormat.DxgiFormat.ToETextureFormat)

            For f = 0 To m_DdsFile.Faces.Count - 1
                data(f) = New RawImage(m_DdsFile.Faces(f).MipMaps.Length - 1) {}
                For m = 0 To data(f).Count - 1
                    'If m_DdsFile.Faces(f).MipMaps(m).SizeInBytes <> data(f)(m).SizeInBytes Then
                    '    Throw New TextureFormatException("DdsFile mipmap size different from expected!")
                    'End If

                    Dim Width As UInteger = m_DdsFile.Faces(f).MipMaps(m).Width
                    Dim Height As UInteger = m_DdsFile.Faces(f).MipMaps(m).Height
                    Dim Size As UInteger = m_DdsFile.Faces(f).MipMaps(m).Data.Length
                    Dim SwapEndian_DxtBlock As Boolean = False
                    data(f)(m) = New RawImage(Width, Height, TextureFormat, Size, SwapEndian_DxtBlock) 'With {}
                    data(f)(m).RawData(SwapEndian_DxtBlock) = m_DdsFile.Faces(f).MipMaps(m).Data
                Next m
            Next f

            Return data
        End Function

    End Module

    Public Module RawImagesExtensions
        <System.Runtime.CompilerServices.Extension>
        Public Function HasMipLevels(ByVal RawImages As RawImage()()) As Boolean
            Return (RawImages.Count > 0) AndAlso (RawImages(0).Count > 1)
        End Function

        <System.Runtime.CompilerServices.Extension>
        Public Function IsCubeMap(ByVal RawImages As RawImage()()) As Boolean
            Return RawImages.Count > 1
        End Function

        <System.Runtime.CompilerServices.Extension>
        Public Function TotalSize(ByVal RawImages As RawImage()()) As Long
            Return RawImages.Sum(Function(f) f.Sum(Function(m) m.RawData(False).Length))
        End Function

    End Module
End Namespace