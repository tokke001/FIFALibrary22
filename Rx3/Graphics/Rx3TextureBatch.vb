Namespace Rx3
    Public Class TextureBatch
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.TEXTURE_BATCH
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.NumTextures = r.ReadUInt32
            Me.Pad = r.ReadBytes(12)

            Me.TextureHeaders = New TextureHeader(Me.NumTextures - 1) {}
            For i = 0 To Me.NumTextures - 1
                Me.TextureHeaders(i) = New TextureHeader(r)
            Next i

        End Sub

        Public Sub Save(ByVal m_Rx3Textures As List(Of Texture), ByVal w As FileWriter)
            Me.NumTextures = m_Rx3Textures.Count

            w.Write(Me.NumTextures)
            w.Write(Me.Pad)

            Me.TextureHeaders = New TextureHeader(Me.NumTextures - 1) {}
            For i = 0 To Me.NumTextures - 1
                Me.TextureHeaders(i) = m_Rx3Textures(i).Header    'As New Rx3TextureHeader
                Me.TextureHeaders(i).Save(w)
            Next i

            FifaUtil.WriteAlignment(w, ALIGNMENT)
        End Sub


        ' Properties
        Public Property NumTextures As UInteger
        Public Property TextureHeaders As TextureHeader()
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Pad As Byte() = New Byte(12 - 1) {}

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace