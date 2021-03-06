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
            Return DdsUtil.GetDds(m_RawImages) ', Me.Header.TextureType, GraphicUtil.GetEFromRx3TextureFormat(Me.Header.TextureFormat), Me.Header.Width, Me.Header.Height)
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
                Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.Flags_1_TextureEndian)

                Me.TextureFaces(f).TextureLevels = New TextureLevel(Me.Header.NumMipLevels - 1) {}
                For i = 0 To Me.Header.NumMipLevels - 1
                    Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, Me.Header.TextureFormat, SwapEndian_DxtBlock)
                    Me.TextureFaces(f).TextureLevels(i).Load(r)
                    width = (width \ 2)
                    height = (height \ 2)
                Next i
            Next f
        End Sub

        Private Function GetEndianDxtBlock(ByVal m_Flags As Rx3.TextureHeader.ETextureEndian) As Boolean
            '! dont use m_Flags.HasFlag() , because the exact values are unknown: 1 = unknown (always present), 2 = BIG_ENDIAN (0 = LITTLE_ENDIAN)
            If m_Flags = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_LITTLE Then
                Return False
            ElseIf m_Flags = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_BIG Then
                Return True
            Else
                Return False
            End If

        End Function

        Public Sub Save(ByVal w As FileWriter)
            Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.Flags_1_TextureEndian)

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

        Public Function SetBitmap(ByVal bitmap As Bitmap) As Boolean
            Dim TextureFormat As ETextureFormat = Me.Header.TextureFormat.ToETextureFormat
            Dim NumLevels As UShort = Me.Header.NumMipLevels

            Me.SetBitmap(bitmap, TextureFormat, NumLevels)

            Return True
        End Function

        Public Function SetBitmap(ByVal bitmap As Bitmap, ByVal TextureFormat As TextureFormat, ByVal NumLevels As UShort) As Boolean
            Dim FaceIndex As Integer = 0
            Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.Flags_1_TextureEndian)

            If Me.Header.TextureType <> ETextureType.TEXTURE_2D Then
                Return False
            End If

            If (bitmap Is Nothing) Then
                Return False
            End If

            '1 - Generate Texture-Header
            Me.Header.Height = bitmap.Height
            Me.Header.Width = bitmap.Width
            Me.Header.TextureFormat = TextureFormat
            Me.Header.NumMipLevels = NumLevels

            '2 - Set main Bitmap (level 0)
            Me.TextureFaces(FaceIndex).TextureLevels(0) = New TextureLevel(bitmap.Width, bitmap.Height, TextureFormat, SwapEndian_DxtBlock)
            Me.TextureFaces(FaceIndex).TextureLevels(0).CalcPitchLinesSize()
            Me.TextureFaces(FaceIndex).TextureLevels(0).Bitmap = bitmap
            '3 - Generate Mipmaps (from main)
            Me.GenerateMipmaps(NumLevels)

            Return True
        End Function

        Public Function SetDds(ByVal DdsFile As DdsFile, Optional KeepRx3TextureFormat As Boolean = False) As Boolean
            Dim m_RawImages As RawImage()() = DdsUtil.GetRawImages(DdsFile)
            Return SetRawImages(KeepRx3TextureFormat, m_RawImages)
        End Function

        Public Function SetKtx(ByVal KtxFile As KtxFile, Optional KeepRx3TextureFormat As Boolean = False) As Boolean
            Dim m_RawImages As RawImage()() = KtxUtil.GetRawImages(KtxFile)
            Return SetRawImages(KeepRx3TextureFormat, m_RawImages)
        End Function

        Private Function SetRawImages(KeepRx3TextureFormat As Boolean, m_RawImages As RawImage()()) As Boolean
            Me.Header.Flags_1_TextureEndian = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_LITTLE     '1
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
                Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.Flags_1_TextureEndian)
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

        'Public Function SetDds_OLD(ByVal DdsFile As DdsFile, Optional KeepRx3TextureFormat As Boolean = False) As Boolean

        '    Me.Header.Flags_1_TextureEndian = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_LITTLE     '1
        '    Me.Header.Height = DdsFile.DdsHeader.dwHeight
        '    Me.Header.Width = DdsFile.DdsHeader.dwWidth
        '    If KeepRx3TextureFormat = False Then
        '        Me.Header.TextureFormat = GraphicUtil.GetRx3FromETextureFormat(DdsFile.TextureFormat)
        '    End If

        '    If DdsFile.DdsHeader.dwMipMapCount = 0 Then
        '        Me.Header.NumMipLevels = 1
        '    Else
        '        Me.Header.NumMipLevels = DdsFile.DdsHeader.dwMipMapCount
        '    End If
        '    ' Me.Rx3TextureHeader.NumFaces =
        '    'Me.Rx3TextureHeader.TextureType =


        '    For f = 0 To Me.Header.NumFaces - 1
        '        'Me.TextureFaces(f) = New Rx3TextureFace()
        '        Dim width As Integer = Me.Header.Width
        '        Dim height As Integer = Me.Header.Height
        '        Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.Flags_1_TextureEndian)
        '        'Dim srcBitmap As Bitmap = Me.TextureFaces(f).TextureLevels(0).Bitmap

        '        Me.TextureFaces(f).TextureLevels = New TextureLevel(Me.Header.NumMipLevels - 1) {}
        '        For i = 0 To Me.Header.NumMipLevels - 1

        '            Me.TextureFaces(f).TextureLevels(i) = New TextureLevel(width, height, Me.Header.TextureFormat, SwapEndian_DxtBlock)
        '            If DdsFile.TextureFormat <> ETextureFormat.BC7 AndAlso Me.Header.TextureFormat = GraphicUtil.GetRx3FromETextureFormat(DdsFile.TextureFormat) Then
        '                Me.TextureFaces(f).TextureLevels(i).RawData(SwapEndian_DxtBlock) = DdsFile.RawImages(f)(i).GetRawData(SwapEndian_DxtBlock)
        '            Else
        '                Dim m_Bitmap As Bitmap = DdsFile.GetBitmap
        '                Me.SetBitmap(m_Bitmap)
        '            End If

        '            Me.TextureFaces(f).TextureLevels(i).CalcPitchLinesSize()

        '            width = (width \ 2)
        '            height = (height \ 2)
        '        Next i
        '    Next f
        'End Function

        Public Sub SetTextureEndian(EndianFlag As Rx3.TextureHeader.ETextureEndian)

            If (Me.Header.Flags_1_TextureEndian <> EndianFlag) And (EndianFlag = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_LITTLE Or EndianFlag = Rx3.TextureHeader.ETextureEndian.TEXTURE_ENDIAN_BIG) Then
                Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(EndianFlag)
                For f = 0 To Me.Header.NumFaces - 1
                    For i = 0 To Me.Header.NumMipLevels - 1
                        Me.TextureFaces(f).TextureLevels(i).SetEndianFormat(SwapEndian_DxtBlock)
                        'Me.TextureFaces(f).TextureLevels(i) = New Rx3TextureLevelHeader(Me.Rx3TextureHeader.Width, Me.Rx3TextureHeader.Height, Me.Rx3TextureHeader.TextureFormat, Me.m_SwapEndian, SwapEndian_DxtBlock)
                    Next
                Next

                Me.Header.Flags_1_TextureEndian = EndianFlag
            End If
        End Sub

        Public Function GenerateMipmaps(ByVal NumLevels As UShort) As Boolean  'generate mipmaps
            Me.Header.NumMipLevels = NumLevels

            For f = 0 To Me.Header.NumFaces - 1
                'Me.TextureFaces(f) = New Rx3TextureFace()
                Dim width As Integer = Me.Header.Width
                Dim height As Integer = Me.Header.Height
                Dim SwapEndian_DxtBlock As Boolean = GetEndianDxtBlock(Me.Header.Flags_1_TextureEndian)
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