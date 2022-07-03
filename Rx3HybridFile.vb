Imports FIFALibrary22.Rw.Core.Arena

'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/

Partial Public Class Rx3HybridFile
    Inherits RxFile
    Public Sub New()
        MyBase.New
    End Sub

    Sub New(Rw4Section As Arena, Rx3Section As Rx3.Rx3File)
        MyBase.New(Rw4Section, Rx3Section)
    End Sub

    Public Overloads Function Load(ByVal FifaFile As FifaFile) As Boolean
        If FifaFile.IsCompressed Then
            FifaFile.Decompress()
        End If
        Dim r As FileReader = FifaFile.GetReader
        Dim flag As Boolean = Me.Load(r)
        FifaFile.ReleaseReader(r)

        Return flag
    End Function

    Public Overridable Overloads Function Load(ByVal r As FileReader) As Boolean
        MyBase.Load(r)

        Return True
    End Function

    Public Overloads Function Load(ByVal FileName As String) As Boolean
        Dim f As New FileStream(FileName, FileMode.Open, FileAccess.ReadWrite)
        Dim r As New FileReader(f, Endian.Little)
        Dim flag As Boolean = Me.Load(r)
        f.Close()
        r.Close()
        Return flag
    End Function

    Public Function ToRx2File(Optional enabletexts As Boolean = True) As Rx2File
        '1 - create file and set Rw4  section
        Dim FileOut_Rx2 As New Rx2File With {
            .Alignment = Me.Rw4Section.Alignment,
            .ArenaGroup = Me.Rw4Section.ArenaGroup,
            .AssetId = Me.Rw4Section.AssetId,
            .Base = Me.Rw4Section.Base,
            .FileHeader = Me.Rw4Section.FileHeader,
            .FixContext = Me.Rw4Section.FixContext,
            .NumEntries = Me.Rw4Section.NumEntries,
            .NumUsed = Me.Rw4Section.NumUsed,
            .Resource = Me.Rw4Section.Resource,
            .OffsetDict = Me.Rw4Section.OffsetDict,
            .ResourceDescriptor = Me.Rw4Section.ResourceDescriptor,
            .ResourcesUsed = Me.Rw4Section.ResourcesUsed,
            .SectionManifest = Me.Rw4Section.SectionManifest,
            .Sections = Me.Rw4Section.Sections,
            .OffsetSectManifest = Me.Rw4Section.OffsetSectManifest,
            .UnfixContext = Me.Rw4Section.UnfixContext,
            .Virt = Me.Rw4Section.Virt}


        '2 - Set buffers (from rx3 sections)
        '2.1 - texture
        If Me.NumTextures > 0 And enabletexts Then
            Dim SwapEndian_DxtBlock As Boolean = True   'rx2 : always big endian textures
            Dim Tiled360 As Boolean = True              'rx2 : always tiled textures

            For i = 0 To Me.Rx3Section.Sections.Textures.Count - 1
                Dim RawLists As New List(Of List(Of RawImage))

                For f = 0 To Me.Rx3Section.Sections.Textures(i).TextureFaces.Length - 1
                    RawLists.Add(New List(Of RawImage))
                    For m = 0 To Me.Rx3Section.Sections.Textures(i).TextureFaces(f).TextureLevels.Length - 1
                        RawLists(f).Add(Me.Rx3Section.Sections.Textures(i).TextureFaces(f).TextureLevels(m))
                        RawLists(f)(m).SetTiling360Format(Tiled360)         '-> first tiling, then swapping ?
                        RawLists(f)(m).SetEndianFormat(SwapEndian_DxtBlock) '-> first tiling, then swapping ?
                    Next
                Next

                FileOut_Rx2.Sections.Rasters(i).SetRawImageData(RawLists)
            Next
        End If

        '2.2 - IndexBuffer & VertexBuffer
        If Me.Rx3Section.Sections.VertexBuffers IsNot Nothing And Me.Rx3Section.Sections.IndexBuffers IsNot Nothing Then
            If Me.Rw4Section.Sections.EmbeddedMeshes.Count = Me.Rx3Section.Sections.VertexBuffers.Count Then
                For i = 0 To Me.Rw4Section.Sections.EmbeddedMeshes.Count - 1
                    'vertex buffers
                    FileOut_Rx2.Sections.EmbeddedMeshes(i).PVertexBuffers(0).SetVertexData(Me.Rx3Section.Sections.VertexBuffers(i).VertexData)
                    'index buffers
                    FileOut_Rx2.Sections.EmbeddedMeshes(i).PIndexBuffer.SetIndexData(Me.Rx3Section.Sections.IndexBuffers(i).IndexData)
                Next
            End If
        End If

        Return FileOut_Rx2
    End Function

    Public Function ToRx3KitFile(ByVal Rx3FileEndianness As Endian, Optional TextureEndianness As Rx3.TextureHeader.ETextureEndian = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_LITTLE) As Rx3File
        If Me.NumTextures > 0 And Me.Rw4Section.Sections.HotSpot IsNot Nothing Then
            Dim FileOut_Rx3 As New Rx3File(Rx3FileEndianness)

            ' - Rx3TextureBatch
            FileOut_Rx3.Sections.AddObject(New Rx3.TextureBatch)

            ' - Textures
            For i = 0 To Me.Rx3Section.Sections.Textures.Count - 1
                Dim m_Rx3Texture As Rx3.Texture = Me.Rx3Section.Sections.Textures(i)
                m_Rx3Texture.SetTextureEndian(TextureEndianness)

                FileOut_Rx3.Sections.AddObject(m_Rx3Texture)
            Next

            ' - NameTable
            Dim m_NameTable As New Rx3.NameTable
            m_NameTable.Names = New Rx3.NameTableEntry(Me.NumTextures - 1) {} '{New Rx3.NameTableEntry With .}
            For i = 0 To m_NameTable.Names.Length - 1
                m_NameTable.Names(i) = New Rx3.NameTableEntry With {
                    .m_Type = Rx3.SectionHash.TEXTURE,
                    .Name = Me.TextureName(i)}
            Next
            FileOut_Rx3.Sections.AddObject(m_NameTable)

            ' - Hotspots
            FileOut_Rx3.Sections.AddObject(Me.Rw4Section.Sections.HotSpot.ToRx3Hotspot)


        End If


        Return Nothing
    End Function

End Class