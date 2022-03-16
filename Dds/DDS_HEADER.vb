Namespace Dds
    Public Class DDS_HEADER
        Public Sub New()

        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.dwSize = r.ReadInt32
            Me.dwFlags = r.ReadInt32
            Me.dwHeight = r.ReadInt32
            Me.dwWidth = r.ReadInt32
            Me.dwPitchOrLinearSize = r.ReadInt32
            Me.dwDepth = r.ReadInt32
            Me.dwMipMapCount = r.ReadInt32
            If Me.dwMipMapCount <= 0 Then 'Minimum one image inside (Also when the mapmap flag is not set)
                Me.dwMipMapCount = 1
            End If

            For i As Integer = 0 To 11 - 1
                Me.dwReserved1(i) = r.ReadInt32
            Next i
            Me.ddspf = New DDS_PIXELFORMAT(r)

            Me.dwCaps = r.ReadInt32
            Me.dwCaps2 = r.ReadInt32
            Me.dwCaps3 = r.ReadInt32
            Me.dwCaps4 = r.ReadInt32
            Me.dwReserved2 = r.ReadInt32
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.dwSize)
            w.Write(CInt(Me.dwFlags))
            w.Write(Me.dwHeight)
            w.Write(Me.dwWidth)
            w.Write(Me.dwPitchOrLinearSize)
            w.Write(Me.dwDepth)
            w.Write(Me.dwMipMapCount)
            For i As Integer = 0 To 11 - 1
                w.Write(Me.dwReserved1(i))
            Next i

            Me.ddspf.Save(w)

            w.Write(CInt(Me.dwCaps))
            w.Write(CInt(Me.dwCaps2))
            w.Write(Me.dwCaps3)
            w.Write(Me.dwCaps4)
            w.Write(Me.dwReserved2)
        End Sub

        Public Sub PrintValues()
            Console.WriteLine("DDSHeader:")
            Console.WriteLine("	dwsize: " & dwSize)
            Console.Write("	flags: " & dwFlags)
            If dwFlags.HasFlag(DdsHeaderFlags.DDSD_CAPS) Then
                Console.Write(" (CAPS)")
            End If
            If dwFlags.HasFlag(DdsHeaderFlags.DDSD_HEIGHT) Then
                Console.Write(" (HEIGHT)")
            End If
            If dwFlags.HasFlag(DdsHeaderFlags.DDSD_WIDTH) Then
                Console.Write(" (WIDTH)")
            End If
            If dwFlags.HasFlag(DdsHeaderFlags.DDSD_PITCH) Then
                Console.Write(" (PITCH)")
            End If
            If dwFlags.HasFlag(DdsHeaderFlags.DDSD_PIXELFORMAT) Then
                Console.Write(" (PIXELFORMAT)")
            End If
            If dwFlags.HasFlag(DdsHeaderFlags.DDSD_MIPMAPCOUNT) Then
                Console.Write(" (MIPMAPCOUNT)")
            End If
            If dwFlags.HasFlag(DdsHeaderFlags.DDSD_LINEARSIZE) Then
                Console.Write(" (LINEARdwsize)")
            End If
            If dwFlags.HasFlag(DdsHeaderFlags.DDSD_DEPTH) Then
                Console.Write(" (DEPTH)")
            End If
            Console.Write(vbLf)
            Console.WriteLine("	height: " & dwHeight)
            Console.WriteLine("	width: " & dwWidth)
            Console.WriteLine("	lineardwsize: " & dwPitchOrLinearSize)
            Console.WriteLine("	depth: " & dwDepth)
            Console.WriteLine("	mipMapCount: " & dwMipMapCount)
            'dwddsPixelFormat.printValues(1)
            Console.Write("	caps: " & dwCaps)
            If dwCaps.HasFlag(DdsHeaderCaps.DDSCAPS_COMPLEX) Then
                Console.Write(" (CAPS_COMPLEX)")
            End If
            If dwCaps.HasFlag(DdsHeaderCaps.DDSCAPS_MIPMAP) Then
                Console.Write(" (CAPS_MIPMAP)")
            End If
            If dwCaps.HasFlag(DdsHeaderCaps.DDSCAPS_TEXTURE) Then
                Console.Write(" (CAPS_TEXTURE)")
            End If
            Console.Write(vbLf)
            Console.Write("	caps2: " & dwCaps2)
            If dwCaps2.HasFlag(DdsHeaderCaps2.DDSCAPS2_CUBEMAP) Then
                Console.Write(" (CAPS2_CUBEMAP)")
            End If
            If dwCaps2.HasFlag(DdsHeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEX) Then
                Console.Write(" (CAPS2_CUBEMAP_POSITIVEX)")
            End If
            If dwCaps2.HasFlag(DdsHeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEX) Then
                Console.Write(" (CAPS2_CUBEMAP_NEGATIVEX)")
            End If
            If dwCaps2.HasFlag(DdsHeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEY) Then
                Console.Write(" (CAPS2_CUBEMAP_POSITIVEY)")
            End If
            If dwCaps2.HasFlag(DdsHeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEY) Then
                Console.Write(" (CAPS2_CUBEMAP_NEGATIVEY)")
            End If
            If dwCaps2.HasFlag(DdsHeaderCaps2.DDSCAPS2_CUBEMAP_POSITIVEZ) Then
                Console.Write(" (CAPS2_CUBEMAP_POSITIVEZ)")
            End If
            If dwCaps2.HasFlag(DdsHeaderCaps2.DDSCAPS2_CUBEMAP_NEGATIVEZ) Then
                Console.Write(" (CAPS2_CUBEMAP_NEGATIVEZ)")
            End If
            If dwCaps2.HasFlag(DdsHeaderCaps2.DDSCAPS2_VOLUME) Then
                Console.Write(" (CAPS2_VOLUME)")
            End If
            Console.Write(vbLf)
            Console.WriteLine("	caps3: " & dwCaps3)
            Console.WriteLine("	caps4: " & dwCaps4)
            'If dwddsHeaderDX10 IsNot Nothing Then
            'dwddsHeaderDX10.printValues(1)
            'End If
        End Sub


        ' Fields
        Public Property dwSize As Integer = 124     'Size of structure. This member must be set to 124.
        Public Property dwFlags As DdsHeaderFlags          'Flags to indicate which members contain valid data.
        Public Property dwHeight As Integer
        Public Property dwWidth As Integer
        Public Property dwPitchOrLinearSize As Integer  'The pitch or number of bytes per scan line in an uncompressed texture; the total number of bytes in the top level texture for a compressed texture. 
        Public Property dwDepth As Integer = 0          'Depth of a volume texture (in pixels), otherwise unused.
        Public Property dwMipMapCount As Integer = 0    'Number of mipmap levels, otherwise unused.
        Public Property dwReserved1 As Integer() = New Integer(11 - 1) {}   'Unused.
        Public Property ddspf As DDS_PIXELFORMAT = New DDS_PIXELFORMAT 'The pixel format
        Public Property dwCaps As DdsHeaderCaps         'Specifies the complexity of the surfaces stored.
        Public Property dwCaps2 As DdsHeaderCaps2       'Additional detail about the surfaces stored.
        Public Property dwCaps3 As Integer = 0          'Unused.
        Public Property dwCaps4 As Integer = 0          'Unused.
        Public Property dwReserved2 As Integer = 0      'Unused.

        <Flags>
        Public Enum DdsHeaderFlags As Integer
            DDSD_CAPS = &H1
            DDSD_HEIGHT = &H2
            DDSD_WIDTH = &H4
            DDSD_PITCH = &H8
            DDSD_PIXELFORMAT = &H1000
            DDSD_MIPMAPCOUNT = &H20000
            DDSD_LINEARSIZE = &H80000
            DDSD_DEPTH = &H800000
        End Enum

        <Flags>
        Public Enum DdsHeaderCaps As Integer
            DDSCAPS_COMPLEX = &H8       'Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
            DDSCAPS_MIPMAP = &H400000   'Optional; should be used for a mipmap.
            DDSCAPS_TEXTURE = &H1000    'Required
        End Enum

        <Flags>
        Public Enum DdsHeaderCaps2 As Integer
            DDSCAPS2_CUBEMAP = &H200            'Required For a cube map. 	
            DDSCAPS2_CUBEMAP_POSITIVEX = &H400  'Required When these surfaces are stored In a cube map. 	
            DDSCAPS2_CUBEMAP_NEGATIVEX = &H800  'Required When these surfaces are stored In a cube map. 	
            DDSCAPS2_CUBEMAP_POSITIVEY = &H1000 'Required When these surfaces are stored In a cube map. 	
            DDSCAPS2_CUBEMAP_NEGATIVEY = &H2000 'Required When these surfaces are stored In a cube map. 	
            DDSCAPS2_CUBEMAP_POSITIVEZ = &H4000 'Required When these surfaces are stored In a cube map. 	
            DDSCAPS2_CUBEMAP_NEGATIVEZ = &H8000 'Required When these surfaces are stored In a cube map. 	
            DDSCAPS2_VOLUME = &H200000          'Required For a volume texture. 	
        End Enum
    End Class
End Namespace