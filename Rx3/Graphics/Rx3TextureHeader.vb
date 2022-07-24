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
            Me.DataFormat = r.ReadByte
            Me.Pad = r.ReadByte

            Me.Width = r.ReadUInt16
            Me.Height = r.ReadUInt16

            Me.NumFaces = r.ReadUInt16
            Me.NumMipLevels = r.ReadUInt16

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.TotalSize)
            w.Write(CByte(Me.TextureType))
            w.Write(CByte(Me.TextureFormat))
            w.Write(CByte(Me.DataFormat))
            w.Write(CByte(Me.Pad))
            w.Write(Me.Width)
            w.Write(Me.Height)
            w.Write(Me.NumFaces)
            w.Write(Me.NumMipLevels)

        End Sub


        ' Properties
        Private m_TotalSize As UInteger

        ''' <summary>
        ''' Total section size (ReadOnly). </summary>
        Public Property TotalSize As UInteger
            Get
                Return m_TotalSize
            End Get
            Friend Set
                m_TotalSize = Value
            End Set
        End Property
        Public Property TextureType As ETextureType
        Public Property TextureFormat As TextureFormat
        Public Property DataFormat As EDataFormat
        Public Property Pad As Byte   '  always 0
        Public Property Width As UShort
        Public Property Height As UShort
        Public Property NumFaces As UShort   'faces count for cubemap or volume textures (layers), 1 for 2D texture
        Public Property NumMipLevels As UShort


        <Flags>
        Public Enum EDataFormat
            eLinear = &H1
            eBigEndian = &H2
            eTiledXenon = &H4
            eSwizzledPS3 = &H8
            eRefpacked = &H80
        End Enum


    End Class


End Namespace