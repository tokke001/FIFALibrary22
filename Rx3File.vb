
'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/

Public Class Rx3File
    Inherits Rx3.Rx3File

    Public Sub New()
        MyBase.New
    End Sub

    Public Sub New(ByVal Endianness As Endian)
        MyBase.New(Endianness)
    End Sub

    'Public Sub New(ByVal Endianness As Endian, Rw4Section As Arena)
    '    Me.Rx3Header = New Rx3FileHeader(Endianness)
    '    Me.Rw4Section = Rw4Section
    'End Sub

    'Public Sub New(Rw4Section As Arena, ByVal r As FileReader)
    '    Me.Rw4Section = Rw4Section
    '    Me.Load(r)
    'End Sub

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
        Me.FileSize = r.BaseStream.Length

        Dim Str As Integer = r.ReadInt32
        r.BaseStream.Position = r.BaseStream.Position - 4
        If (Str = &H6C335852) Or (Str = &H62335852) Or (Str = &H5258336C) Or (Str = &H52583362) Then
            MyBase.Load(r)  ' = New Rx3FileRx3Section(Me.RW4Section, r)
        Else
            Return False
        End If

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

    Public Overridable Overloads Function Save(ByVal w As FileWriter) As Boolean

        '1 - Save Rx3 section
        MyBase.Save(w)

        Return True
    End Function

    Public Overloads Function Save(ByVal FileName As String) As Boolean
        Dim output As New FileStream(FileName, FileMode.Create, FileAccess.ReadWrite)
        Dim w As New FileWriter(output, Endian.Little)
        Dim flag As Boolean = Me.Save(w)
        output.Close()
        w.Close()

        Return flag
    End Function

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


    Public Property FileSize As Long

End Class