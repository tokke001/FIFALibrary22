Imports System.Drawing
Imports BCnEncoder.Shared
Imports FIFALibrary22.Dds
Imports FIFALibrary22.Ktx

Namespace Rx3
    Public Class Texture
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.TEXTURE
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Function GetBitmap() As Bitmap
            Return Me.TextureFaces(0).TextureLevels(0).Bitmap
        End Function

        Public Function GetBitmap(ByVal TextureFace As Integer, ByVal TextureLevel As Integer) As Bitmap
            Return If((TextureLevel < Me.TextureFaces(TextureFace).TextureLevels.Length), Me.TextureFaces(TextureFace).TextureLevels(TextureLevel).Bitmap, Nothing)
        End Function

        Public Function GetDds() As DdsFile
            Dim m_RawImages = New RawImage(Me.TextureFaces.Length - 1)() {}
            For f = 0 To m_RawImages.Length - 1
                m_RawImages(f) = Me.TextureFaces(f).TextureLevels
            Next
            Dim m_DdsFile As New DdsFile
            Dim PreferDx10Header As Boolean = False
            m_DdsFile.FromRawImages(m_RawImages, PreferDx10Header)
            Return m_DdsFile
        End Function

        Public Function GetKtx() As KtxFile
            Dim m_RawImages = New RawImage(Me.TextureFaces.Length - 1)() {}
            For f = 0 To m_RawImages.Length - 1
                m_RawImages(f) = Me.TextureFaces(f).TextureLevels
            Next
            Return KtxUtil.GetKtx(m_RawImages) ', Me.Header.TextureType, GraphicUtil.GetEFromRx3TextureFormat(Me.Header.TextureFormat), Me.Header.Width, Me.Header.Height)
        End Function

        Public Sub Load(ByVal r As FileReader)
            '1 - Load textureheader
            Me.Header = New TextureHeader(r)

            '2 - Load rest
            Me.TextureFaces = New TextureFace(Me.Header.NumFaces - 1) {}
            For f = 0 To Me.Header.NumFaces - 1
                Me.TextureFaces(f) = New TextureFace()
                Dim width As Integer = Me.Header.Width    'each face same height/width from header -> so put here
                Dim height As Integer = Me.Header.Height  'each face same height/width from header -> so put here
                Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.DataFormat)

                Me.TextureFaces(f).TextureLevels = New TextureLevel(Me.Header.NumMipLevels - 1) {}
                For i = 0 To Me.Header.NumMipLevels - 1
                    Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, Me.Header.TextureFormat, SwapEndian_DxtBlock)
                    Me.TextureFaces(f).TextureLevels(i).Load(r)
                    width = (width \ 2)
                    height = (height \ 2)
                Next i
            Next f
        End Sub

        Private Function GetEndianDxtBlock(ByVal m_Flags As Rx3.TextureHeader.EDataFormat) As Boolean
            If m_Flags.HasFlag(Rx3.TextureHeader.EDataFormat.eBigEndian) Then
                Return True
            End If

            Return False
        End Function

        Public Sub Save(ByVal w As FileWriter)
            Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.DataFormat)

            Me.Header.NumFaces = Me.TextureFaces.Length
            Me.Header.NumMipLevels = Me.TextureFaces(0).TextureLevels.Length

            'Me.Rx3TextureHeader.Height = 
            'Me.Rx3TextureHeader.Width = 
            'Me.Rx3TextureHeader.TextureFormat = 

            Me.Header.Save(w)

            For f = 0 To Me.Header.NumFaces - 1
                Me.Header.NumMipLevels = Me.TextureFaces(f).TextureLevels.Length

                For i = 0 To Me.Header.NumMipLevels - 1
                    Me.TextureFaces(f).TextureLevels(i).Save(SwapEndian_DxtBlock, w)
                Next i
            Next f

            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.Header.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub

        Public Function SetBitmap(ByVal Bitmap As Bitmap) As Boolean
            Dim TextureFormat As ETextureFormat = Me.Header.TextureFormat.ToETextureFormat
            Dim NumLevels As UShort = Me.Header.NumMipLevels

            Me.SetBitmap(Bitmap, TextureFormat, NumLevels)

            Return True
        End Function

        Public Function SetBitmap(ByVal Bitmap As Bitmap, ByVal TextureFormat As TextureFormat, ByVal NumLevels As UShort) As Boolean
            Dim FaceIndex As Integer = 0
            Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.DataFormat)

            If Me.Header.TextureType <> ETextureType.TEXTURE_2D Then
                Return False
            End If

            If (Bitmap Is Nothing) Then
                Return False
            End If

            '1 - Generate Texture-Header
            Me.Header.Height = Bitmap.Height
            Me.Header.Width = Bitmap.Width
            Me.Header.TextureFormat = TextureFormat
            Me.Header.NumMipLevels = NumLevels

            '2 - Set main Bitmap (level 0)
            Me.TextureFaces(FaceIndex).TextureLevels(0) = New TextureLevel(Bitmap.Width, Bitmap.Height, TextureFormat, SwapEndian_DxtBlock)
            Me.TextureFaces(FaceIndex).TextureLevels(0).CalcPitchLinesSize()
            Me.TextureFaces(FaceIndex).TextureLevels(0).Bitmap = Bitmap
            '3 - Generate Mipmaps (from main)
            Me.GenerateMipmaps(NumLevels)

            Return True
        End Function

        Public Function SetDds(ByVal DdsFile As DdsFile, Optional KeepRx3TextureFormat As Boolean = False) As Boolean
            Return SetRawImages(DdsFile.ToRawImages, KeepRx3TextureFormat)
        End Function

        Public Function SetKtx(ByVal KtxFile As KtxFile, Optional KeepRx3TextureFormat As Boolean = False) As Boolean
            Dim m_RawImages As RawImage()() = KtxUtil.GetRawImages(KtxFile)
            Return SetRawImages(m_RawImages, KeepRx3TextureFormat)
        End Function

        Private Function SetRawImages(m_RawImages As RawImage()(), KeepRx3TextureFormat As Boolean) As Boolean
            Me.Header.DataFormat = Rx3.TextureHeader.EDataFormat.eLinear     '1
            Me.Header.Width = m_RawImages(0)(0).Width
            Me.Header.Height = m_RawImages(0)(0).Height
            If KeepRx3TextureFormat = False Then
                Me.Header.TextureFormat = m_RawImages(0)(0).TextureFormat.ToRx3TextureFormat 'DdsUtil.ToETextureFormat(DdsFile.Header.ddsPixelFormat.DxgiFormat) 
            End If

            Me.Header.NumFaces = m_RawImages.Length
            Select Case Me.Header.NumFaces
                Case 6
                    Me.Header.TextureType = ETextureType.TEXTURE_CUBEMAP
                Case Else
                    Me.Header.TextureType = ETextureType.TEXTURE_2D
            End Select
            Me.Header.NumMipLevels = m_RawImages(0).Length

            Me.TextureFaces = New TextureFace(Me.Header.NumFaces - 1) {}
            For f = 0 To Me.Header.NumFaces - 1
                Me.TextureFaces(f) = New TextureFace()
                Dim width As Integer = Me.Header.Width
                Dim height As Integer = Me.Header.Height
                Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.DataFormat)
                'Dim srcBitmap As Bitmap = Me.TextureFaces(f).TextureLevels(0).Bitmap

                Me.TextureFaces(f).TextureLevels = New TextureLevel(Me.Header.NumMipLevels - 1) {}
                For i = 0 To Me.Header.NumMipLevels - 1

                    Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, Me.Header.TextureFormat, SwapEndian_DxtBlock)
                    If m_RawImages(0)(0).TextureFormat <> ETextureFormat.BC7 AndAlso Me.Header.TextureFormat = m_RawImages(0)(0).TextureFormat.ToRx3TextureFormat Then
                        Me.TextureFaces(f).TextureLevels(i).RawData(SwapEndian_DxtBlock) = m_RawImages(f)(i).RawData(SwapEndian_DxtBlock)
                    Else
                        Dim m_Bitmap As Bitmap = m_RawImages(f)(0).Bitmap 'DdsFile.GetBitmap
                        Me.SetBitmap(m_Bitmap)
                    End If

                    Me.TextureFaces(f).TextureLevels(i).CalcPitchLinesSize()

                    width = (width \ 2)
                    height = (height \ 2)
                Next i
            Next f

            Return True
        End Function

        Public Sub SetTextureEndian(IsBigEndian As Boolean)

            If Me.Header.DataFormat.HasFlag(TextureHeader.EDataFormat.eBigEndian) <> IsBigEndian Then
                For f = 0 To Me.Header.NumFaces - 1
                    For i = 0 To Me.Header.NumMipLevels - 1
                        Me.TextureFaces(f).TextureLevels(i).SetEndianFormat(IsBigEndian)
                        'Me.TextureFaces(f).TextureLevels(i) = New Rx3TextureLevelHeader(Me.Rx3TextureHeader.Width, Me.Rx3TextureHeader.Height, Me.Rx3TextureHeader.TextureFormat, Me.m_SwapEndian, SwapEndian_DxtBlock)
                    Next
                Next

                Me.Header.DataFormat &= TextureHeader.EDataFormat.eBigEndian
            End If
        End Sub

        Public Function GenerateMipmaps(ByVal NumLevels As UShort) As Boolean  'generate mipmaps
            Me.Header.NumMipLevels = NumLevels

            For f = 0 To Me.Header.NumFaces - 1
                'Me.TextureFaces(f) = New Rx3TextureFace()
                Dim width As Integer = Me.Header.Width
                Dim height As Integer = Me.Header.Height
                Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.DataFormat)
                Dim srcBitmap As Bitmap = Me.TextureFaces(f).TextureLevels(0).Bitmap

                ReDim Preserve Me.TextureFaces(f).TextureLevels(Me.Header.NumMipLevels - 1)
                For i = 0 To Me.Header.NumMipLevels - 1

                    If i <> 0 Then
                        Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, Me.Header.TextureFormat, SwapEndian_DxtBlock)
                        Me.TextureFaces(f).TextureLevels(i).CalcPitchLinesSize()

                        srcBitmap = GraphicUtil.ReduceBitmap(srcBitmap)
                        Me.TextureFaces(f).TextureLevels(i).Bitmap = srcBitmap
                    End If

                    width = (width \ 2)
                    height = (height \ 2)
                Next i
            Next f

            Return True
        End Function

        ' Properties
        Public Property Header As TextureHeader
        Public Property TextureFaces As TextureFace()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class TextureFace
        Public Property TextureLevels As TextureLevel()    'mipmaps

    End Class


End Namespace