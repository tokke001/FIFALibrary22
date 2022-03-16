Namespace Dds
	Public Class DDS_PIXELFORMAT
		Public Sub New()

		End Sub
		Public Sub New(ByVal r As FileReader)
			Me.dwSize = r.ReadInt32
			Me.dwFlags = r.ReadInt32
			Me.dwFourCC = New String(r.ReadChars(4))
			Me.dwRGBBitCount = r.ReadInt32
			Me.dwRBitMask = r.ReadInt32
			Me.dwGBitMask = r.ReadInt32
			Me.dwBBitMask = r.ReadInt32
			Me.dwABitMask = r.ReadInt32
		End Sub
		Public Sub Save(ByVal w As FileWriter)
			w.Write(Me.dwSize)
			w.Write(CInt(Me.dwFlags))
			If Me.dwFourCC.Length = 4 Then
				w.Write(Me.dwFourCC.ToCharArray)
			Else
				w.Write(CInt(&H0))
			End If
			w.Write(Me.dwRGBBitCount)
			w.Write(Me.dwRBitMask)
			w.Write(Me.dwGBitMask)
			w.Write(Me.dwBBitMask)
			w.Write(Me.dwABitMask)
		End Sub

		Public Overridable ReadOnly Property IsAlphaPixels As Boolean
			Get
				Return dwFlags.HasFlag(DdsPixelformatFlags.DDPF_ALPHAPIXELS)
			End Get
		End Property
		Public Overridable ReadOnly Property IsAlpha As Boolean
			Get
				Return dwFlags.HasFlag(DdsPixelformatFlags.DDPF_ALPHA)
			End Get
		End Property
		Public Overridable ReadOnly Property IsFourCC As Boolean
			Get
				Return dwFlags.HasFlag(DdsPixelformatFlags.DDPF_FOURCC)
			End Get
		End Property
		Public Overridable ReadOnly Property IsRGB As Boolean
			Get
				Return dwFlags.HasFlag(DdsPixelformatFlags.DDPF_RGB)
			End Get
		End Property
		Public Overridable ReadOnly Property IsYUV As Boolean
			Get
				Return dwFlags.HasFlag(DdsPixelformatFlags.DDPF_YUV)
			End Get
		End Property
		Public Overridable ReadOnly Property IsQWVU As Boolean
			Get
				Return dwFlags.HasFlag(DdsPixelformatFlags.DDPF_QWVU)
			End Get
		End Property
		Public Overridable ReadOnly Property IsLuminance As Boolean
			Get
				Return dwFlags.HasFlag(DdsPixelformatFlags.DDPF_LUMINANCE)
			End Get
		End Property
		Public Overridable ReadOnly Property IsNormal As Boolean
			Get
				Return dwFlags.HasFlag(DdsPixelformatFlags.DDPF_NORMAL)
			End Get
		End Property

		Public Sub PrintValues()
			PrintValues(0)
		End Sub

		Public Sub PrintValues(ByVal nSpace As Integer)
			Dim sSpace As String = ""
			For i As Integer = 0 To nSpace - 1
				sSpace = sSpace & "	"
			Next i
			Console.WriteLine(sSpace & "PixelFormat: ")

			Console.WriteLine(sSpace & "	size: " & dwSize)
			Console.Write(sSpace & "	flags: " & dwFlags)
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_ALPHAPIXELS) Then
				Console.Write(" (ALPHAPIXELS)")
			End If
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_ALPHA) Then
				Console.Write(" (ALPHA)")
			End If
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_FOURCC) Then
				Console.Write(" (FOURCC)")
			End If
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_RGB) Then
				Console.Write(" (RGB)")
			End If
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_YUV) Then
				Console.Write(" (YUV)")
			End If
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_LUMINANCE) Then
				Console.Write(" (LUMINANCE)")
			End If
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_PALETTEINDEXED8) Then
				Console.Write(" (PALETTEINDEXED8)")
			End If
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_QWVU) Then
				Console.Write(" (QWVU)")
			End If
			If dwFlags.HasFlag(DdsPixelformatFlags.DDPF_NORMAL) Then
				Console.Write(" (NORMAL)")
			End If
			Console.Write(vbLf)
			Dim sFourCC As String = ""
			sFourCC &= ChrW(dwFourCC And &HFF)
			sFourCC &= ChrW((dwFourCC >> 8) And &HFF)
			sFourCC &= ChrW((dwFourCC >> 16) And &HFF)
			sFourCC &= ChrW((dwFourCC >> 24) And &HFF)
			Console.WriteLine(sSpace & "	fourCC: " & dwFourCC) ' & " (" & getFormat().getName() & " - " & sFourCC & ")")
			Console.WriteLine(sSpace & "	rgbBitCount: " & dwRGBBitCount)
			Console.WriteLine(sSpace & "	dwRBitMask: " & dwRBitMask.ToString("x") & " int(" & dwRBitMask & ")") ' fixed(" & dwRBitMaskFixed.ToString("x") & ") shift(" & rShift & ") bits(" & rBits & ")")
			Console.WriteLine(sSpace & "	dwGBitMask: " & dwGBitMask.ToString("x") & " int(" & dwGBitMask & ")") ' fixed(" & dwGBitMaskFixed.ToString("x") & ") shift(" & gShift & ") bits(" & gBits & ")")
			Console.WriteLine(sSpace & "	dwBBitMask: " & dwBBitMask.ToString("x") & " int(" & dwBBitMask & ")") ' fixed(" & dwBBitMaskFixed.ToString("x") & ") shift(" & bShift & ") bits(" & bBits & ")")
			Console.WriteLine(sSpace & "	dwABitMask: " & dwABitMask.ToString("x") & " int(" & dwABitMask & ")") ' fixed(" & dwABitMaskFixed.ToString("x") & ") shift(" & aShift & ") bits(" & aBits & ")")
			'Console.WriteLine(sSpace & "	Format: " & getFormat().getName())
		End Sub


		' Fields
		Public Property dwSize As Integer = 32          'Structure size; set to 32 (bytes).
		Public Property dwFlags As DdsPixelformatFlags  'Values which indicate what type of data is in the surface.
		Public Property dwFourCC As String              'Four-character codes for specifying compressed or custom formats: DXT1, DXT2, DXT3, DXT4, DXT5, .... A FourCC of DX10 indicates the prescense of the DDS_HEADER_DXT10 extended header
		Public Property dwRGBBitCount As Integer        'Number of bits in an RGB (possibly including alpha) format. Valid when dwFlags includes DDPF_RGB, DDPF_LUMINANCE, or DDPF_YUV.
		Public Property dwRBitMask As Integer
		Public Property dwGBitMask As Integer
		Public Property dwBBitMask As Integer
		Public Property dwABitMask As Integer

		<Flags>
		Public Enum DdsPixelformatFlags As Integer
			DDPF_ALPHAPIXELS = &H1  'Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
			DDPF_ALPHA = &H2        'Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
			DDPF_FOURCC = &H4       'Texture contains compressed RGB data; dwFourCC contains valid data.
			DDPF_PALETTEINDEXED8 = &H20
			DDPF_QWVU = &H80000
			DDPF_RGB = &H40         'Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks (dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
			DDPF_YUV = &H200        'Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)
			DDPF_LUMINANCE = &H20000 'Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
			DDPF_NORMAL = &H80000000
		End Enum
	End Class

End Namespace