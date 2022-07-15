Imports System.Drawing
Imports Microsoft.DirectX

Module testing2
    Sub EditModels()

        'Create a new Rx file 
        Dim FileRx As New RxFile

        'Load the File (this can be any rx2, rx3Hybrid (FIFA 11), rx3 file)
        Dim MyLocation As String = "C:\Documents\test\data\sceneassets\ball\ball_1_textures.rx3"
        FileRx.Load(MyLocation)

        '1 - we do easy edits
        For i = 0 To FileRx.NumTextures - 1   'for each texture
            'Get the name of the texture
            Dim TextName As String = FileRx.TextureName(i)

            'Get bitmap, and export as png
            Dim m_bitmap As Bitmap = FileRx.Bitmaps(i)
            m_bitmap.Save("C:\Export\ball_1_textures_" & TextName & ".png", Imaging.ImageFormat.Png)

            'Get dds texture, and export
            Dim m_ddsfile As BCnEncoder.Shared.DdsFile = FileRx.DdsTextures(i)
            Dim fileName As String = "C:\Export\ball_1_textures_" & TextName & ".dds"
            Dim output As New FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)
            m_ddsfile.Write(output)
            output.Close()
        Next

        '2 - Get the full nametable (with names and types)
        Dim m_NameTable As List(Of NameTable) = FileRx.NameTable

        '3 - we do advanced edits: accesing the rx3/rw4 sections
        If FileRx.Rw4Section IsNot Nothing Then     'Rw4-Section exists at: rx2, rx3Hybrid (FIFA 11) files
            'get the RW header
            Dim Rw4Header As Rw.Core.Arena.ArenaFileHeader = FileRx.Rw4Section.FileHeader
            'get the RW SectionManifest
            Dim ArenaSectionManifest As Rw.Core.Arena.ArenaSectionManifest = FileRx.Rw4Section.SectionManifest
            '...
            '- Access the RW sections -
            '-- 3.1 loop trough all sections
            For i = 0 To FileRx.Rw4Section.Sections.NumObjects - 1
                Dim m_rwObject As Rw.RwObject = FileRx.Rw4Section.Sections.GetObject(i)
            Next
            '-- 3.2 Get a specific section
            ' get the first raster section (contains all texture infos)
            Dim m_Raster As Rw.Graphics.Raster = FileRx.Rw4Section.Sections.Rasters(0)

        End If

        If FileRx.Rx3Section IsNot Nothing Then     'Rx3-Section exists at: rx3Hybrid (FIFA 11), rx3 (FIFA 12 or later) files
            'get the rx3 header
            Dim Rx3Header As Rx3.Rx3FileHeader = FileRx.Rx3Section.Rx3Header
            '- Access the Rx3 sections -
            '-- 3.1 loop trough all sections
            For i = 0 To FileRx.Rx3Section.Sections.NumObjects - 1
                Dim m_rx3Object As Rx3.Rx3Object = FileRx.Rx3Section.Sections.GetObject(i)
            Next
            '-- 3.2 Get a specific section
            ' get the first (texture 0) Texture section
            Dim m_Texture As Rx3.Texture = FileRx.Rx3Section.Sections.Textures(0)

        End If

        '4 - Save the File 
        Dim MySaveLocation As String = "C:\Export\ball_2.rx3"
        FileRx.Save(MySaveLocation)

    End Sub
End Module
