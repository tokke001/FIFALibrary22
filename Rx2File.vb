Imports System.Drawing
Imports FIFALibrary22.Rw
Imports FIFALibrary22.Rx3
Imports Microsoft.DirectX.Direct3D
'Imports FIFALibrary22.Rw.Core.Arena

'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/

Public Class Rx2File
    Inherits Rw.Core.Arena.Arena

    Public Sub New()
        MyBase.New
    End Sub

    Public Sub New(ByVal Endianness As Endian)
        MyBase.New(Endianness)
    End Sub

    Public Overloads Function Load(ByVal fifaFile As FifaFile) As Boolean
        If fifaFile.IsCompressed Then
            fifaFile.Decompress()
        End If
        Dim r As FileReader = fifaFile.GetReader
        Dim flag As Boolean = Me.Load(r)
        fifaFile.ReleaseReader(r)

        Return flag
    End Function

    Public Overridable Overloads Function Load(ByVal r As FileReader) As Boolean
        Me.FileSize = r.BaseStream.Length

        Dim str As New String(r.ReadChars(4))
        r.BaseStream.Position = (r.BaseStream.Position - 4)
        If (str.Contains("RW4")) Then   'rx2 file
            MyBase.Load(r) '= New Arena(r)

        Else
            Return False
        End If

        'Me.m_SwapEndian = Me.FileHeader.IsBigEndian

        'Dim num As Integer = 0
        'Dim num2 As Integer = 0
        'Dim num3 As Integer = 0

        'For j = 0 To Me.NumEntries - 1

        '    If Me.ArenaDictEntries(j).TypeId = Rw.SectionTypeCode.RWOBJECTTYPE_BUFFER Then
        '        r.BaseStream.Position = Me.ResourceDescriptor.BaseResourceDescriptors(0).Size + Me.ArenaDictEntries(j).Offset

        '        Select Case Me.ArenaDictEntries(j + 1).TypeId     'in RW4SectionInfos-list : buffer always located before infos

        '            Case Rw.SectionTypeCode.RWGOBJECTTYPE_VERTEXBUFFER
        '                'ReDim Preserve Me.Rx2VertexBuffers(num)
        '                Me.Rx2VertexBuffers.Add(New VertexBuffer(MyBase.VertexFormats(num), Me.Sections.VertexBuffers(num).NumVertices, r))
        '                num += 1

        '            Case Rw.SectionTypeCode.RWGOBJECTTYPE_INDEXBUFFER
        '                'ReDim Preserve Me.Rx2IndexBuffers(num2)
        '                Me.Rx2IndexBuffers.Add(New IndexBuffer(Me.Sections.IndexBuffers(num2), r))
        '                num2 += 1

        '            Case Rw.SectionTypeCode.RWGOBJECTTYPE_RASTER
        '                'ReDim Preserve Me.Rx2TextureBuffers(num3)
        '                Me.Rx2TextureBuffers.Add(New Texture(Me.Sections.Rasters(num3), Me.ArenaDictEntries(j).Size, True, r)) 'GetTextureSize(Me.Sections.Raster(num3).Width, Me.Sections.Raster(num3).Height, ConvertTexformatRW4toRx3(Me.Sections.Raster(num3).TextureFormat)), True)   'rx2 textures always big endian
        '                'Me.Rx2TextureBuffers(num3).Load(r)
        '                num3 += 1

        '            Case Else
        '                MsgBox("Error at loading Rx2File: Unknown rx2 buffer found - " & CStr(Me.ArenaDictEntries(j + 1).TypeId))
        '                Continue For

        '        End Select
        '    End If
        'Next j

        Return True
    End Function

    Public Overloads Function Load(ByVal fileName As String) As Boolean
        Dim f As New FileStream(fileName, FileMode.Open, FileAccess.ReadWrite)
        Dim r As New FileReader(f, Endian.Big)
        Dim flag As Boolean = Me.Load(r)
        f.Close()
        r.Close()
        Return flag
    End Function

    Public Overridable Overloads Function Save(ByVal w As FileWriter) As Boolean

        '1 - RW4 Section (if present)
        '1.1 - Save RW4 section
        MyBase.Save(w)

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
        ' m_ListVertexElements = Me.GetVertexElementList11()


        '5 - Save buffers

        Return True
    End Function

    Public Overloads Function Save(ByVal fileName As String) As Boolean
        Dim output As New FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)
        Dim w As New FileWriter(output, Endian.Big)
        Dim flag As Boolean = Me.Save(w)
        output.Close()
        w.Close()

        Return flag
    End Function

    Public Function ToRx3HybridFile(Optional enabletexts As Boolean = True) As Rx3HybridFile
        Dim FileOut_Rx3 As New Rx3HybridFile

        '0 - Check for unsupported formats (texture types, ...)
        CheckFormatsFor11()

        FileOut_Rx3.Rw4Section = Me

        '1 - create RX3  section
        FileOut_Rx3.Rx3Section = New Rx3.Rx3File(Endian.Big)

        '2 - create Rx3 section (from rx2 buffers)
        '2.1 - Rx3TextureBatch
        If Me.NumTextures > 0 And enabletexts Then
            FileOut_Rx3.Rx3Section.Sections.AddObject(New Rx3.TextureBatch)
        End If
        '2.2 - Rx3IndexBufferBatch
        If Me.Sections.IndexBuffers.Count <> 0 Then
            FileOut_Rx3.Rx3Section.Sections.AddObject(New Rx3.IndexBufferBatch)
        End If
        '2.3 - Rx3texture
        If Me.NumTextures > 0 And enabletexts Then
            FileOut_Rx3.Rx3Section.Sections.Textures = New List(Of Rx3.Texture)  'Rx3.Texture(Me.Rx2TextureBuffers.Length - 1) {}
            For i = 0 To Me.Sections.Rasters.Count - 1
                Dim m_texture As New Rx3.Texture
                m_texture.Header = New Rx3.TextureHeader With {
                        .TextureType = Me.Sections.Rasters(i).D3d.Format.Dimension,' ETextureType.TEXTURE_2D,
                        .TextureFormat = Me.Sections.Rasters(i).D3d.Format.TextureFormat.ToRx3TextureFormat,
                        .Height = Me.Sections.Rasters(i).D3d.Format.Height,
                        .Width = Me.Sections.Rasters(i).D3d.Format.Width,
                        .NumFaces = Me.Sections.Rasters(i).D3d.Format.Depth,' 1,
                        .NumMipLevels = Me.Sections.Rasters(i).NumMipLevels,
                        .TotalSize = 0
                        }
                m_texture.Header.DataFormat = Rx3.TextureHeader.EDataFormat.eLinear    ' little endian (image RAW data)
                m_texture.Header.Pad = 0    'always 0
                m_texture.TextureFaces = New Rx3.TextureFace(m_texture.Header.NumFaces - 1) {}
                Dim width As Integer = m_texture.Header.Width
                Dim height As Integer = m_texture.Header.Height
                Dim SwapEndian_DxtBlock As Boolean = False
                For j = 0 To m_texture.TextureFaces.Length - 1
                    m_texture.TextureFaces(j) = New Rx3.TextureFace
                    m_texture.TextureFaces(j).TextureLevels = New Rx3.TextureLevel(m_texture.Header.NumMipLevels - 1) {}
                    For k = 0 To m_texture.TextureFaces(j).TextureLevels.Length - 1
                        m_texture.TextureFaces(j).TextureLevels(k) = New Rx3.TextureLevel(width, height, m_texture.Header.TextureFormat, SwapEndian_DxtBlock)
                        m_texture.TextureFaces(j).TextureLevels(k).CalcPitchLinesSize()
                        m_texture.TextureFaces(j).TextureLevels(k).RawData(SwapEndian_DxtBlock) = Me.Sections.Rasters(i).GetRawImageData(j)(k).RawData(SwapEndian_DxtBlock)

                        width = (width \ 2)
                        height = (height \ 2)
                    Next k
                    'm_texture.TextureFaces(j).TextureLevels(0).SetRawData(Me.Rx2TextureBuffers(i).TextureFaces(j).TextureLevels(0).GetRawData(SwapEndian_DxtBlock), SwapEndian_DxtBlock)
                    'm_texture.GenerateMipmaps(m_texture.Rx3TextureHeader.NumLevels)
                Next j
                FileOut_Rx3.Rx3Section.Sections.Textures.Add(m_texture)
            Next i
        End If
        '2.4 - Rx3IndexBuffer
        If Me.Sections.IndexBuffers.Count > 0 Then
            FileOut_Rx3.Rx3Section.Sections.IndexBuffers = New List(Of Rx3.IndexBuffer) 'New Rx3.IndexBuffer(Me.Rx2IndexBuffers.Count - 1) {}
            For i = 0 To Me.Sections.IndexBuffers.Count - 1
                FileOut_Rx3.Rx3Section.Sections.IndexBuffers.Add(New Rx3.IndexBuffer With {
                        .IndexData = Me.Sections.IndexBuffers(i).GetIndexData
                    })
            Next
        End If
        '2.5 - Rx3BoneRemap --> not present at rx2, can be generated by code, based on indice data in Rx3VertexBuffer
        If Me.Sections.AnimationSkins IsNot Nothing Then
            FileOut_Rx3.Rx3Section.Sections.BoneRemaps = New List(Of BoneRemap) 'New Rx3.BoneRemap(Me.Rx2VertexBuffers.Count - 1) {}
            For i = 0 To Me.Sections.VertexBuffers.Count - 1
                FileOut_Rx3.Rx3Section.Sections.BoneRemaps.Add(New Rx3.BoneRemap With {
                        .ReservedSize = 256
                        })
            Next i
        End If

        '2.6 - Rx3VertexBuffer
        If Me.Sections.VertexBuffers.Count > 0 Then
            FileOut_Rx3.Rx3Section.Sections.VertexBuffers = New List(Of Rx3.VertexBuffer) 'New Rx3.VertexBuffer(Me.Rx2VertexBuffers.Length - 1) {}
            For i = 0 To Me.Sections.VertexBuffers.Count - 1
                FileOut_Rx3.Rx3Section.Sections.VertexBuffers.Add(New Rx3.VertexBuffer With {
                        .VertexStride = Me.Sections.VertexBuffers(i).VertexStride,
                        .VertexData = Me.Sections.VertexBuffers(i).GetVertexData,
                        .VertexEndianness = Rx3.VertexBuffer.EVertexEndian.Big_Endian,
                        .Pad = New Byte(3 - 1) {0, 0, 0}
                    })
            Next
        End If

        Return FileOut_Rx3
    End Function

    Private Sub CheckFormatsFor11()
        'If Me.RW4Section IsNot Nothing Then
        If Me.Sections.Rasters IsNot Nothing Then
            For i = 0 To Me.Sections.Rasters.Count - 1
                If Me.Sections.Rasters(i).D3d.Format.TextureFormat = Rw.SurfaceFormat.FMT_32_32_32_32_FLOAT Then
                    Dim m_Bitmap As Bitmap = Me.Bitmaps(i) 'Rx3File_in.Rx3Section.Sections.Textures(i))
                    Dim NewTextureFormat As Rw.SurfaceFormat = Rw.SurfaceFormat.FMT_DXT4_5 ' RWTextureFormat.A8R8G8B8

                    Me.Sections.Rasters(i).SetTextureTiling(False)
                    Me.Sections.Rasters(i).SetTextureEndian(False)
                    Me.Sections.Rasters(i).SetBitmap(m_Bitmap, NewTextureFormat, Me.Sections.Rasters(i).NumMipLevels)
                    Me.Sections.Rasters(i).D3d.Format.TextureFormat = NewTextureFormat
                End If
            Next
        End If
        If Me.Sections.VertexDescriptors IsNot Nothing Then
            For i = 0 To Me.Sections.VertexDescriptors.Count - 1
                Dim FoundTexError1 As Integer = 0
                For j = 0 To Me.Sections.VertexDescriptors(i).Elements.Length - 1
                    If FoundTexError1 > 0 Then
                        Me.Sections.VertexDescriptors(i).Elements(j).Offset -= (4 * FoundTexError1)
                    End If
                    If Me.Sections.VertexDescriptors(i).Elements(j).Usage = DeclarationUsage.TextureCoordinate And Me.Sections.VertexDescriptors(i).Elements(j).DataType = Rw.D3D.D3DDECLTYPE.FLOAT3 Then
                        Me.Sections.VertexDescriptors(i).Elements(j).DataType = Rw.D3D.D3DDECLTYPE.FLOAT2
                        'Me.Sections.VertexDescriptor(i).Elements(j).DataTypeCode = 
                        FoundTexError1 += 1
                        Continue For
                    ElseIf Me.Sections.VertexDescriptors(i).Elements(j).Usage = DeclarationUsage.Normal Or Me.Sections.VertexDescriptors(i).Elements(j).Usage = DeclarationUsage.BiNormal Or Me.Sections.VertexDescriptors(i).Elements(j).Usage = DeclarationUsage.Tangent Then
                        'MsgBox(Me.Sections.VertexDescriptor(i).Elements(j).Usage)
                    End If

                Next
                If FoundTexError1 > 0 Then
                    Me.Sections.VertexBuffers(i).VertexStride = Me.Sections.VertexDescriptors(i).VertexStride
                End If
            Next
        End If
        'End If
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

    'Private Function GetVertexElementList11() As List(Of VertexElement())
    '    Dim m_ListVertexElements As New List(Of VertexElement()) '= Nothing
    '    Dim m_VertexElements As VertexElement() '{}'= Nothing

    '    If Me.Sections.VertexBuffers Is Nothing Then
    '        Exit Function
    '    End If

    '    For v = 0 To Me.Sections.VertexBuffers.Count - 1
    '        'm_VertexElements = New VertexElement(Me.VertexDescription(v).nElements - 1) {}

    '        If Me.Sections.VertexDescriptors.Count = 1 Then
    '            For u = 0 To Me.Sections.VertexDescriptors(0).NumElements - 1
    '                ReDim Preserve m_VertexElements(Me.Sections.VertexDescriptors(0).NumElements - 1)
    '                m_VertexElements(u) = New VertexElement With {
    '                            .DataType = Me.Sections.VertexDescriptors(0).Elements(u).DataType,
    '                            .Usage = Me.Sections.VertexDescriptors(0).Elements(u).Usage,
    '                            .Offset = Me.Sections.VertexDescriptors(0).Elements(u).Offset,
    '                            .UsageIndex = Me.Sections.VertexDescriptors(0).Elements(u).UsageIndex
    '                        }
    '            Next u
    '        Else
    '            For u = 0 To Me.Sections.VertexDescriptors(v).NumElements - 1
    '                ReDim Preserve m_VertexElements(Me.Sections.VertexDescriptors(v).NumElements - 1)
    '                m_VertexElements(u) = New VertexElement With {
    '                                .DataType = Me.Sections.VertexDescriptors(v).Elements(u).DataType,
    '                                .Usage = Me.Sections.VertexDescriptors(v).Elements(u).Usage,
    '                                .Offset = Me.Sections.VertexDescriptors(v).Elements(u).Offset,
    '                                .UsageIndex = Me.Sections.VertexDescriptors(v).Elements(u).UsageIndex
    '                            }
    '            Next u
    '        End If




    '        m_ListVertexElements.Add(m_VertexElements)
    '    Next v

    '    Return m_ListVertexElements
    'End Function

    'Public Property Bitmaps As List(Of Bitmap)
    'Public Property DdsTextures As List(Of DdsFile)
    'Public Property VertexStreams As List(Of List(Of Vertex))
    'Public Property IndexStreams As List(Of List(Of UInteger))
    'Public Property PrimitiveTypes(ByVal MeshIndex As UInteger) As PrimitiveType
    'Public Property PrimitiveTypes As List(Of PrimitiveType)
    'Public ReadOnly Property VertexFormats(ByVal MeshIndex As UInteger) As VertexElement()
    'Public ReadOnly Property VertexFormats As List(Of VertexElement())
    'Public ReadOnly Property NameTable As List(Of FIFALibrary22.NameTable)
    'Public Property MeshName(ByVal MeshIndex As UInteger) As String
    'Public Property TextureName(ByVal TextureIndex As UInteger) As String
    'Public ReadOnly Property NumMeshes As UInteger
    'Public ReadOnly Property NumTextures As UInteger

    ' Properties
    'Public Property RW4Section As Arena
    'Public Property Rx2TextureBuffers As New List(Of Texture)
    'Public Property Rx2IndexBuffers As New List(Of IndexBuffer)
    'Public Property Rx2VertexBuffers As New List(Of VertexBuffer)

    'Private m_IsFifa11 As Boolean
    Public Property FileSize As Long


End Class
