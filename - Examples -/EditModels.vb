Imports Microsoft.DirectX

Module testing
    Sub EditModels()

        'Create a new Rx file 
        Dim FileRx As New RxFile

        'Load the File (this can be any rx2, rx3Hybrid (FIFA 11), rx3 file)
        Dim MyLocation As String = "C:\Documents\test\data\sceneassets\ball\ball_1.rx3"
        FileRx.Load(MyLocation)

        '1 - we do easy edits
        For m = 0 To FileRx.NumMeshes - 1   'for each mesh

            For Each Vertex In FileRx.VertexStreams(m)  'loop trough the vertices

                Dim Position_x As Single = Vertex.Position.X
                Dim Position_y As Single = Vertex.Position.Y
                Dim Position_z As Single = Vertex.Position.Z

                Dim Text_U As Single = Vertex.TextureCoordinates(0).U
                Dim Text_V As Single = Vertex.TextureCoordinates(0).V

                Dim Normal_x As Single = Vertex.Normal.Normal_x
                Dim Normal_y As Single = Vertex.Normal.Normal_y
                Dim Normal_z As Single = Vertex.Normal.Normal_z

            Next

            For Each Index In FileRx.IndexStreams(m)    'loop trough the indices
                Dim m_Index As UInteger = Index
            Next

            'Get the Bone-Matrices
            Dim BoneMatrices As List(Of BonePose) = FileRx.BoneMatrices(m)

            'Get the vertex formats
            Dim VertexFormat As VertexElement() = FileRx.VertexFormats(m)

            'Get the PrimitiveType
            Dim m_PrimitiveType As Direct3D.PrimitiveType = FileRx.PrimitiveTypes(m)

            'Get the name of the Mesh
            Dim MeshName As String = FileRx.MeshName(m)
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
            ' get the first VertexDescriptor section
            Dim VertexDescriptor As Rw.Graphics.VertexDescriptor = FileRx.Rw4Section.Sections.VertexDescriptors(0)
            ' get the first Skeleton section
            Dim Skeleton As Rw.OldAnimation.Skeleton = FileRx.Rw4Section.Sections.Skeletons(0)

        End If

        If FileRx.Rx3Section IsNot Nothing Then     'Rx3-Section exists at: rx3Hybrid (FIFA 11), rx3 (FIFA 12 or later) files
            'get the rx3 header
            Dim Rx3Header As Rx3.Rx3FileHeader = FileRx.Rx3Section.Rx3Header
            '...
            '- Access the Rx3 sections -
            '-- 3.1 loop trough all sections
            For i = 0 To FileRx.Rx3Section.Sections.NumObjects - 1
                Dim m_rx3Object As Rx3.Rx3Object = FileRx.Rx3Section.Sections.GetObject(i)
            Next
            '-- 3.2 Get a specific section
            ' get the first (mesh 0) vertexbuffer section
            Dim vertexbuffer As Rx3.VertexBuffer = FileRx.Rx3Section.Sections.VertexBuffers(0)
            ' get the first (mesh 0) indexbuffer section
            Dim indexbuffer As Rx3.IndexBuffer = FileRx.Rx3Section.Sections.IndexBuffers(0)

        End If

        '4 - Save the File 
        Dim MySaveLocation As String = "C:\Export\ball_2.rx3"
        FileRx.Save(MySaveLocation)

    End Sub
End Module
