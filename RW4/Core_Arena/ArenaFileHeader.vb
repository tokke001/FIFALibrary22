Namespace Rw.Core.Arena
    Public Class ArenaFileHeader
        'class name = rw::core::arena::ArenaFileHeader
        Public Sub New(ByVal Endianness As Endian)
            If Endianness = Endian.Big Then
                Me.IsBigEndian = True
            Else
                Me.IsBigEndian = False
            End If

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.MagicNumber = New ArenaFileHeaderMagicNumber(r)

            Me.IsBigEndian = r.ReadBoolean          'endianness (0 - little-endian, 1 - big-endian)
            r.Endianness = GetFileEndianness(Me.IsBigEndian)

            Me.PointerSizeInBits = r.ReadByte    'number of bits for pointer type (normally 32)
            Me.PointerAlignment = r.ReadByte    'pointer alignment, in bytes (normally 4)
            Me.Unused = r.ReadByte             'unused, always 0

            Me.MajorVersion = r.ReadBytes(4)  'major version (RW) - normally "454" (i.e. RenderWare 4.5.4)
            Me.MinorVersion = r.ReadBytes(4)  'minor version (RW) - normally "000"

            Me.BuildNo = r.ReadUInt32     'build number (RW) - normally 0

        End Sub

        Private Function GetFileEndianness(ByVal m_IsBigEndian As Boolean) As Endian
            If m_IsBigEndian Then
                Return Endian.Big
            End If

            Return Endian.Little
        End Function
        'Private Function Read4ByteString(ByVal r As FileReader) As String
        'Dim num As Byte
        'Dim chArray As Char() = New Char(3) {}

        'For i = 0 To 3 - 1
        '       num = r.ReadByte
        '      chArray(i) = Convert.ToChar(num)
        'Next i

        '   r.ReadByte()  'null terminator
        'Return New String(chArray, 0, 3)
        'End Function

        Public Sub Save(ByVal w As FileWriter)
            w.Endianness = GetFileEndianness(Me.IsBigEndian)

            Me.MagicNumber.Save(w)

            w.Write(Me.IsBigEndian)
            w.Write(Me.PointerSizeInBits)
            w.Write(Me.PointerAlignment)
            w.Write(Me.Unused)

            w.Write(Me.MajorVersion)
            w.Write(Me.MinorVersion)

            w.Write(Me.BuildNo)

        End Sub

        Public Property MagicNumber As ArenaFileHeaderMagicNumber
        Public Property IsBigEndian As Boolean = True 'RWEndianness
        Public Property PointerSizeInBits As Byte
        Public Property PointerAlignment As Byte
        Public Property Unused As Byte
        Public Property MajorVersion As Byte() = New Byte(4) {}
        Public Property MinorVersion As Byte() = New Byte(4) {}
        Public Property BuildNo As UInteger

    End Class

    Public Class ArenaFileHeaderMagicNumber
        'rw::core::arena::ArenaFileHeaderMagicNumber
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Prefix = r.ReadBytes(4)      'RW4 magick (\x89,'R','W','4')
            Me.Body = r.ReadBytes(4)        'platform magick ('x','b','2',\x00 for Xbox 360; 'w','3','2',\x00 for PC)
            Me.Suffix = r.ReadBytes(4)      'unknown magick (always \xD,\xA,\x1A,\xA)

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Prefix)
            w.Write(Me.Body)
            w.Write(Me.Suffix)

        End Sub

        Public Property Prefix As Byte() = New Byte(4) {}
        Public Property Body As Byte() = New Byte(4) {}
        Public Property Suffix As Byte() = New Byte(4) {}

    End Class

End Namespace