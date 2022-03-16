Namespace Rx3
    Public Class TextureHeader
        ' Methods
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32

            Me.TextureType = r.ReadByte
            Me.TextureFormat = r.ReadByte
            Me.Flags_1_TextureEndian = r.ReadByte
            Me.Flags_2 = r.ReadByte

            Me.Width = r.ReadUInt16
            Me.Height = r.ReadUInt16

            Me.NumFaces = r.ReadUInt16
            Me.NumMipLevels = r.ReadUInt16

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            'Me.TotalSize = m_Rx3TextureHeader.TotalSize
            'Me.TextureType = m_Rx3TextureHeader.TextureType
            'Me.TextureFormat = m_Rx3TextureHeader.TextureFormat
            'Me.Flags_1_TextureEndian = m_Rx3TextureHeader.Flags_1_TextureEndian
            'Me.Flags_2 = m_Rx3TextureHeader.Flags_2
            'Me.Width = m_Rx3TextureHeader.Width
            'Me.Height = m_Rx3TextureHeader.Height
            'Me.NumFaces = m_Rx3TextureHeader.NumFaces
            'Me.NumLevels = m_Rx3TextureHeader.NumLevels

            w.Write(Me.TotalSize)
            w.Write(CByte(Me.TextureType))
            w.Write(CByte(Me.TextureFormat))
            w.Write(CByte(Me.Flags_1_TextureEndian))
            w.Write(CByte(Me.Flags_2))
            w.Write(Me.Width)
            w.Write(Me.Height)
            w.Write(Me.NumFaces)
            w.Write(Me.NumMipLevels)

        End Sub


        ' Properties
        Public Property TotalSize As UInteger
        Public Property TextureType As ETextureType
        Public Property TextureFormat As TextureFormat
        Public Property Width As UShort
        Public Property Height As UShort
        Public Property NumMipLevels As UShort
        Public Property Flags_1_TextureEndian As ETextureEndian   'Flag_endian:  Flag_endian = 1 for little endian, 3 for big endian (image RAW data)  
        Public Property Flags_2 As Byte   'Flag_unknown    :  always 0
        Public Property NumFaces As UShort   'faces count for cubemap or volume textures (layers), 1 for 2D texture

        '<Flags>
        Public Enum ETextureEndian As Byte
            TEXTURE_ENDIAN_LITTLE = 1
            TEXTURE_ENDIAN_BIG = 3
        End Enum
    End Class


End Namespace