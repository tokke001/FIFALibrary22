Imports System.Drawing
Imports FIFALibrary22.Rw.Core.Arena
Imports FIFALibrary22.Rx3
Imports Microsoft.DirectX.Direct3D
'Imports FIFALibrary22.Rw.Core.Arena

'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/
Namespace Rx2
    Public Class Rx2File

        Public Function ConvertToRx3Fifa11() As Rx3File
            Dim FileOut_Rx3 As Rx3File = New Rx3File

            '0 - Check for unsupported formats (texture types, ...)
            CheckFormatsFor11()

            FileOut_Rx3.RW4Section = Me.RW4Section

            '1 - create RX3  header
            FileOut_Rx3.Rx3Section.Rx3Header = New Rx3FileHeader With {
                    .Signature = "RX3",
                    .Endianness = "b",
                    .Version = 4,
                    .Size = 0,          'wrong rx3 section size, but fixed at saving
                    .NumSections = 0    'fixed later
                    }

            '2 - create Rx3 section (from rx2 buffers)
            '2.1 - Rx3texture
            Dim enabletexts As Boolean = True
            If Me.Rx2TextureBuffers IsNot Nothing And enabletexts Then
                FileOut_Rx3.Rx3Section.Sections.Textures = New Rx3.Texture(Me.Rx2TextureBuffers.Length - 1) {}
                For i = 0 To FileOut_Rx3.Rx3Section.Sections.Textures.Count - 1
                    FileOut_Rx3.Rx3Section.Sections.Textures(i) = New Rx3.Texture(FileOut_Rx3.Rx3Section)
                    FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader = New Rx3.TextureHeader With {
                            .TextureType = Me.Rx2TextureBuffers(i).TextureType,' ETextureType.TEXTURE_2D,
                            .TextureFormat = GraphicUtil.GetRx3FromRWTextureFormat(Me.RW4Section.Sections.Rasters(i).D3d.Format.TextureFormat),
                            .Height = Me.RW4Section.Sections.Rasters(i).D3d.Format.Height,
                            .Width = Me.RW4Section.Sections.Rasters(i).D3d.Format.Width,
                            .NumFaces = Me.Rx2TextureBuffers(i).NumFaces,' 1,
                            .NumMipLevels = Me.RW4Section.Sections.Rasters(i).NumMipLevels,
                            .TotalSize = 0
                            }
                    FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader.Flags_1_TextureEndian = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_LITTLE    '1 for little endian, 3 for big endian (image RAW data)
                    FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader.Flags_2 = 0    'always 0
                    FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces = New Rx3.TextureFace(FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader.NumFaces - 1) {}
                    Dim width As Integer = FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader.Width
                    Dim height As Integer = FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader.Height
                    Dim SwapEndian_DxtBlock As Boolean = False
                    For j = 0 To FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces.Length - 1
                        FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces(j) = New Rx3.TextureFace
                        FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces(j).TextureLevels = New Rx3.TextureLevel(FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader.NumMipLevels - 1) {}
                        For k = 0 To FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces(j).TextureLevels.Length - 1
                            FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces(j).TextureLevels(k) = New Rx3.TextureLevel(width, height, FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader.TextureFormat, SwapEndian_DxtBlock)
                            FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces(j).TextureLevels(k).CalcPitchLinesSize()
                            FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces(j).TextureLevels(k).SetRawData(Me.Rx2TextureBuffers(i).TextureFaces(j).TextureLevels(k).GetRawData(SwapEndian_DxtBlock), SwapEndian_DxtBlock)

                            width = (width \ 2)
                            height = (height \ 2)
                        Next k
                        'FileOut_Rx3.Rx3Section.Sections.Textures(i).TextureFaces(j).TextureLevels(0).SetRawData(Me.Rx2TextureBuffers(i).TextureFaces(j).TextureLevels(0).GetRawData(SwapEndian_DxtBlock), SwapEndian_DxtBlock)
                        'FileOut_Rx3.Rx3Section.Sections.Textures(i).GenerateMipmaps(FileOut_Rx3.Rx3Section.Sections.Textures(i).Rx3TextureHeader.NumLevels)
                    Next j
                Next i
            End If
            '2.2 - Rx3IndexBuffer
            If Me.Rx2IndexBuffers IsNot Nothing Then
                FileOut_Rx3.Rx3Section.Sections.IndexBuffers = New Rx3.IndexBuffer(Me.Rx2IndexBuffers.Length - 1) {}
                For i = 0 To FileOut_Rx3.Rx3Section.Sections.IndexBuffers.Count - 1
                    FileOut_Rx3.Rx3Section.Sections.IndexBuffers(i) = New Rx3.IndexBuffer With {
                            .IndexData = Me.Rx2IndexBuffers(i).IndexData,
                            .Rx3IndexBufferHeader = New Rx3.IndexBufferHeader With {
                                .TotalSize = 0,
                                .IndexStride = Me.RW4Section.Sections.IndexBuffers(i).IndexStride,
                                .NumIndices = Me.RW4Section.Sections.IndexBuffers(i).NumIndices,
                                .Padding = New Byte(7 - 1) {0, 0, 0, 0, 0, 0, 0}
                            }
                        }
                Next
            End If
            '2.3 - Rx3BoneRemap --> not present at rx2, can be generated by code, based on indice data in Rx3VertexBuffer
            If Me.RW4Section.Sections.AnimationSkins IsNot Nothing Then
                FileOut_Rx3.Rx3Section.Sections.BoneRemaps = New Rx3.BoneRemap(Me.Rx2VertexBuffers.Length - 1) {}
                For i = 0 To FileOut_Rx3.Rx3Section.Sections.BoneRemaps.Count - 1
                    FileOut_Rx3.Rx3Section.Sections.BoneRemaps(i) = New Rx3.BoneRemap With {
                            .TotalSize = 0,
                            .NumUsedBones = 0,
                            .UsedBones = New Byte(256 - 1) {},              'need be generated by code, based on indice data in Rx3VertexBuffer
                            .UsedBonesPositions = New Byte(256 - 1) {},     'need be generated by code, based on indice data in Rx3VertexBuffer
                            .Padding = New Byte(11 - 1) {}
                            }
                Next i
            End If

            '2.4 - Rx3VertexBuffer
            If Me.Rx2VertexBuffers IsNot Nothing Then
                FileOut_Rx3.Rx3Section.Sections.VertexBuffers = New Rx3.VertexBuffer(Me.Rx2VertexBuffers.Length - 1) {}
                For i = 0 To FileOut_Rx3.Rx3Section.Sections.VertexBuffers.Count - 1
                    FileOut_Rx3.Rx3Section.Sections.VertexBuffers(i) = New Rx3.VertexBuffer With {
                            .TotalSize = 0,
                            .NumVertices = Me.RW4Section.Sections.VertexBuffers(i).NumVertices,
                            .VertexStride = Me.RW4Section.Sections.VertexBuffers(i).VertexStride,
                            .VertexData = Me.Rx2VertexBuffers(i).VertexData,
                            .VertexEndianness = Rx3.VertexBuffer.EVertexEndian.Big_Endian,
                            .Padding = New Byte(3 - 1) {0, 0, 0}
                        }
                Next
            End If

            '3 - create Rx3SectionHeaders
            FileOut_Rx3.GenerateRx3SectionHeaders()

            Return FileOut_Rx3
        End Function

        Private Sub CheckFormatsFor11()
            If Me.RW4Section IsNot Nothing Then
                If Me.RW4Section.Sections.Rasters IsNot Nothing Then
                    For i = 0 To Me.RW4Section.Sections.Rasters.Count - 1
                        If Me.RW4Section.Sections.Rasters(i).D3d.Format.TextureFormat = Rw.SurfaceFormat.FMT_32_32_32_32_FLOAT Then
                            Dim m_Bitmap As Bitmap = Me.Rx2TextureBuffers(i).GetBitmap() 'Rx3File_in.Rx3Section.Sections.Textures(i))
                            Dim NewTextureFormat As Rw.SurfaceFormat = Rw.SurfaceFormat.FMT_DXT4_5 ' RWTextureFormat.A8R8G8B8

                            Me.Rx2TextureBuffers(i).SetTextureTiling(False)
                            Me.Rx2TextureBuffers(i).SetTextureEndian(False)
                            Me.Rx2TextureBuffers(i).SetBitmap(m_Bitmap, NewTextureFormat, Me.RW4Section.Sections.Rasters(i).NumMipLevels)
                            Me.RW4Section.Sections.Rasters(i).D3d.Format.TextureFormat = NewTextureFormat
                        End If
                    Next
                End If
                If Me.RW4Section.Sections.VertexDescriptors IsNot Nothing Then
                    For i = 0 To Me.RW4Section.Sections.VertexDescriptors.Count - 1
                        Dim FoundTexError1 As Integer = 0
                        For j = 0 To Me.RW4Section.Sections.VertexDescriptors(i).Elements.Length - 1
                            If FoundTexError1 > 0 Then
                                Me.RW4Section.Sections.VertexDescriptors(i).Elements(j).Offset -= (4 * FoundTexError1)
                            End If
                            If Me.RW4Section.Sections.VertexDescriptors(i).Elements(j).Usage = DeclarationUsage.TextureCoordinate And Me.RW4Section.Sections.VertexDescriptors(i).Elements(j).DataType = Rw.D3D.D3DDECLTYPE.FLOAT3 Then
                                Me.RW4Section.Sections.VertexDescriptors(i).Elements(j).DataType = Rw.D3D.D3DDECLTYPE.FLOAT2
                                'Me.RW4Section.Sections.VertexDescriptor(i).Elements(j).DataTypeCode = 
                                FoundTexError1 += 1
                                Continue For
                            ElseIf Me.RW4Section.Sections.VertexDescriptors(i).Elements(j).Usage = DeclarationUsage.Normal Or Me.RW4Section.Sections.VertexDescriptors(i).Elements(j).Usage = DeclarationUsage.BiNormal Or Me.RW4Section.Sections.VertexDescriptors(i).Elements(j).Usage = DeclarationUsage.Tangent Then
                                'MsgBox(Me.RW4Section.Sections.VertexDescriptor(i).Elements(j).Usage)
                            End If

                        Next
                        If FoundTexError1 > 0 Then
                            Me.RW4Section.Sections.VertexDescriptors(i).VertexStride -= (4 * FoundTexError1)
                            Me.RW4Section.Sections.VertexBuffers(i).VertexStride = Me.RW4Section.Sections.VertexDescriptors(i).VertexStride
                        End If
                    Next
                End If
            End If
        End Sub

        'Public Function CreateXFiles(ByVal baseFileName As String) As Boolean
        'If (Me.Rx2IndexBuffers.Length = 0) Then
        'Return False
        'End If
        'Dim i As Integer
        'For i = 0 To Me.Rx2IndexBuffers.Length - 1
        'Dim modeld As New Model3D
        '       modeld.Initialize(Me.Rx2IndexBuffers(i), Me.Rx2VertexBuffers(i))
        '       Application.CurrentCulture = New CultureInfo("en-us")
        'Dim stream As New FileStream((baseFileName & i.ToString & ".X"), FileMode.Create, FileAccess.Write)
        'Dim w As New StreamWriter(stream)
        '       modeld.SaveXFile(w)
        '       w.Close()
        '      stream.Close()
        'Next i
        'Return True
        'End Function


        Public Function GetBitmaps() As Bitmap()
            Dim bitmapArray As Bitmap() = New Bitmap(Me.Rx2TextureBuffers.Length - 1) {}
            Dim i As Integer
            For i = 0 To Me.Rx2TextureBuffers.Length - 1
                bitmapArray(i) = Me.Rx2TextureBuffers(i).GetBitmap
            Next i
            Return bitmapArray
        End Function

        Public Function GetPrimitiveType(ByVal MeshIndex As UInteger) As PrimitiveType
            If Me.RW4Section IsNot Nothing AndAlso Me.RW4Section.Sections.FxRenderableSimples IsNot Nothing Then
                Return Me.RW4Section.Sections.FxRenderableSimples(MeshIndex).PrimitiveType
            Else
                Return Nothing
            End If
        End Function

        Public Function Load(ByVal fifaFile As FifaFile) As Boolean
            If fifaFile.IsCompressed Then
                fifaFile.Decompress()
            End If
            Dim r As FileReader = fifaFile.GetReader
            Dim flag As Boolean = Me.Load(r)
            fifaFile.ReleaseReader(r)

            Return flag
        End Function

        Public Overridable Function Load(ByVal r As FileReader) As Boolean
            Dim m_ListVertexElements As New List(Of VertexElement()) '= Nothing
            'Dim SwapEndian_DxtBlock As Boolean = True

            Dim str As New String(r.ReadChars(4))
            r.BaseStream.Position = (r.BaseStream.Position - 4)
            If (str.Contains("RW4")) Then   'rx2 file
                Me.RW4Section = New Arena(r)
                m_ListVertexElements = Me.GetVertexElementList11()
                r.BaseStream.Position = Me.RW4Section.ResourceDescriptor.BaseResourceDescriptors(0).Size
            Else
                Return False
            End If

            'Me.m_SwapEndian = Me.RW4Section.FileHeader.IsBigEndian

            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim num3 As Integer = 0

            For j = 0 To Me.RW4Section.NumEntries - 1

                If Me.RW4Section.ArenaDictEntries(j).TypeId = Rw.SectionTypeCode.RWOBJECTTYPE_BUFFER Then
                    r.BaseStream.Position = Me.RW4Section.ResourceDescriptor.BaseResourceDescriptors(0).Size + Me.RW4Section.ArenaDictEntries(j).Offset

                    Select Case Me.RW4Section.ArenaDictEntries(j + 1).TypeId     'in RW4SectionInfos-list : buffer always located before infos

                        Case Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
                            ReDim Preserve Me.Rx2VertexBuffers(num)
                            Me.Rx2VertexBuffers(num) = New VertexBuffer(m_ListVertexElements(num), Me.RW4Section.Sections.VertexBuffers(num).NumVertices, r)
                            num += 1

                        Case Rw.SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
                            ReDim Preserve Me.Rx2IndexBuffers(num2)
                            Me.Rx2IndexBuffers(num2) = New IndexBuffer(Me.RW4Section.Sections.IndexBuffers(num2), r)
                            num2 += 1

                        Case Rw.SectionTypeCode.RWGOBJECTTYPE_RASTER
                            ReDim Preserve Me.Rx2TextureBuffers(num3)
                            Me.Rx2TextureBuffers(num3) = New Texture(Me.RW4Section.Sections.Rasters(num3), Me.RW4Section.ArenaDictEntries(j).Size, True, r) 'GetTextureSize(Me.RW4Section.Sections.Raster(num3).Width, Me.RW4Section.Sections.Raster(num3).Height, ConvertTexformatRW4toRx3(Me.RW4Section.Sections.Raster(num3).TextureFormat)), True)   'rx2 textures always big endian
                            'Me.Rx2TextureBuffers(num3).Load(r)
                            num3 += 1

                        Case Else
                            MsgBox("Error at loading Rx2File: Unknown rx2 buffer found - " & CStr(Me.RW4Section.ArenaDictEntries(j + 1).TypeId))
                            Continue For

                    End Select
                End If
            Next j

            Return True
        End Function

        Private Function GetVertexElementList11() As List(Of VertexElement())
            Dim m_ListVertexElements As New List(Of VertexElement()) '= Nothing
            Dim m_VertexElements As VertexElement() '{}'= Nothing

            If Me.RW4Section.Sections.VertexBuffers Is Nothing Then
                Exit Function
            End If

            For v = 0 To Me.RW4Section.Sections.VertexBuffers.Count - 1
                'm_VertexElements = New VertexElement(Me.RW4Section.VertexDescription(v).nElements - 1) {}

                If Me.RW4Section.Sections.VertexDescriptors.Count = 1 Then
                    For u = 0 To Me.RW4Section.Sections.VertexDescriptors(0).NumElements - 1
                        ReDim Preserve m_VertexElements(Me.RW4Section.Sections.VertexDescriptors(0).NumElements - 1)
                        m_VertexElements(u) = New VertexElement With {
                                    .DataType = Me.RW4Section.Sections.VertexDescriptors(0).Elements(u).DataType,
                                    .Usage = Me.RW4Section.Sections.VertexDescriptors(0).Elements(u).Usage,
                                    .Offset = Me.RW4Section.Sections.VertexDescriptors(0).Elements(u).Offset,
                                    .UsageIndex = Me.RW4Section.Sections.VertexDescriptors(0).Elements(u).UsageIndex
                                }
                    Next u
                Else
                    For u = 0 To Me.RW4Section.Sections.VertexDescriptors(v).NumElements - 1
                        ReDim Preserve m_VertexElements(Me.RW4Section.Sections.VertexDescriptors(v).NumElements - 1)
                        m_VertexElements(u) = New VertexElement With {
                                        .DataType = Me.RW4Section.Sections.VertexDescriptors(v).Elements(u).DataType,
                                        .Usage = Me.RW4Section.Sections.VertexDescriptors(v).Elements(u).Usage,
                                        .Offset = Me.RW4Section.Sections.VertexDescriptors(v).Elements(u).Offset,
                                        .UsageIndex = Me.RW4Section.Sections.VertexDescriptors(v).Elements(u).UsageIndex
                                    }
                    Next u
                End If




                m_ListVertexElements.Add(m_VertexElements)
            Next v

            Return m_ListVertexElements
        End Function


        Public Function Load(ByVal fileName As String) As Boolean
            Dim f As New FileStream(fileName, FileMode.Open, FileAccess.ReadWrite)
            Dim r As New FileReader(f, Endian.Big)
            Dim flag As Boolean = Me.Load(r)
            f.Close()
            r.Close()
            Return flag
        End Function

        Public Function ReplaceBitmaps(ByVal bitmaps As Bitmap()) As Boolean
            Dim flag As Boolean = True
            Dim num As Integer = If((bitmaps.Length < Me.Rx2TextureBuffers.Length), bitmaps.Length, Me.Rx2TextureBuffers.Length)
            Dim i As Integer
            For i = 0 To num - 1
                If (Not bitmaps(i) Is Nothing) Then
                    'flag = (flag And Me.Rx2TextureBuffers(i).SetBitmap(bitmaps(i)))
                End If
            Next i
            Return flag
        End Function

        Public Overridable Function Save(ByVal w As FileWriter) As Boolean

            '1 - RW4 Section (if present)
            '1.1 - Save RW4 section
            Me.RW4Section.Save(w)

            Dim num As Integer = 0
            Dim num2 As Integer = 0
            Dim num3 As Integer = 0
            Dim num4 As Integer = 0
            Dim num5 As Integer = 0
            Dim num6 As Integer = 0
            Dim num7 As Integer = 0
            Dim num8 As Integer = 0
            Dim num9 As Integer = 0
            Dim m_ListVertexElements As New List(Of VertexElement()) '= Nothing

            '4 - Get VertexElements List
            m_ListVertexElements = Me.GetVertexElementList11()


            '5 - Save buffers

            Return True
        End Function

        Public Function Save(ByVal fileName As String) As Boolean
            Dim output As New FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)
            Dim w As New FileWriter(output, Endian.Big)
            Dim flag As Boolean = Me.Save(w)
            output.Close()
            w.Close()

            Return flag
        End Function


        ' Properties
        Public Property RW4Section As Arena
        Public Property Rx2TextureBuffers As Texture()
        Public Property Rx2IndexBuffers As IndexBuffer()
        Public Property Rx2VertexBuffers As VertexBuffer()

        'Private m_IsFifa11 As Boolean
        'Private m_FileSize As Long


    End Class

End Namespace