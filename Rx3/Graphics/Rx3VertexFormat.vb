Imports Microsoft.DirectX.Direct3D
Imports D3DDECLTYPE = FIFALibrary22.Rw.D3D.D3DDECLTYPE
Namespace Rx3
    Public Class VertexFormat
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.VERTEX_FORMAT
        Public Const ALIGNMENT As Integer = 16

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            MyBase.New(Rx3File)
        End Sub

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section, ByVal r As FileReader)
            MyBase.New(Rx3File)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Dim StartPosition As Integer = r.BaseStream.Position

            Me.TotalSize = r.ReadUInt32
            Me.VertexFormatSize = r.ReadUInt32
            Me.Padding(0) = r.ReadUInt32
            Me.Padding(1) = r.ReadUInt32

            Me.StrVertexFormat = Split(CStr(r.ReadChars(Me.VertexFormatSize - 1)), " ") 'dont read last char (null-terminator)


            'Create elements list
            ReDim Me.Elements(Me.StrVertexFormat.Length - 1)
            For i = 0 To Me.StrVertexFormat.Length - 1
                Me.Elements(i) = New Element(Me.StrVertexFormat(i))
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            Dim BaseOffset As Long = w.BaseStream.Position
            Me.VertexFormatSize = GetVertexFormatSize(Me.StrVertexFormat)

            'ReDim Me.VertexFormat(Me.Elements.Length - 1)  'disabled, to prevent errors with different formats that exists
            'For i = 0 To Me.Elements.Length - 1
            'Me.VertexFormat(i) = Me.Elements(i).ToVertexFormatString(Me.VertexFormat(i))    'function to get string formats from elements lists
            'Next i

            w.Write(Me.TotalSize)
            w.Write(Me.VertexFormatSize)
            w.Write(Me.Padding(0))
            w.Write(Me.Padding(1))

            For i = 0 To Me.StrVertexFormat.Length - 1

                w.Write(Me.StrVertexFormat(i).ToCharArray)

                If i <> Me.StrVertexFormat.Length - 1 Then     'write space splitter
                    w.Write(" "c)
                End If

            Next i
            w.Write(CByte(0))                               'write null terminator 


            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position)

        End Sub

        Private Function GetVertexFormatSize(ByVal m_VertexFormat As String())

            Dim Size As Long

            For i = 0 To m_VertexFormat.Length - 1

                Size += m_VertexFormat(i).Length   'string size
                Size += 1                           'space splitter (or null terminator at total end) size 

            Next i

            Return Size

        End Function


        Public Property TotalSize As UInteger
        Public Property VertexFormatSize As UInteger
        Public Property Padding As UInteger() = New UInteger(2 - 1) {}   '{ 0, 0 }, maybe padding
        Public Property StrVertexFormat As String()
        Public Property Elements As Element()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function

        Public Class Element
            Inherits VertexElement
            Public Sub New()

            End Sub

            Public Sub New(ByVal StrVertexElement As String)
                'Me.m_SwapEndian = swapEndian
                Me.Load(StrVertexElement)
            End Sub

            Public Sub Load(ByVal VertexFormatString As String)
                '%c%X:%02X:%02X:%04X:%s - usage, usageIndex, offset, unk0, unk1, dataType
                '%c%X:%02X:%02X:%s - usage, usageIndex, offset, unk0, dataType
                '%c%d:%X:%s - usage, usageIndex, offset, dataType
                '%c%d:%s - usage, usageIndex, dataType
                Dim StrElements As String() = Split(VertexFormatString, ":")

                For i = 0 To StrElements.Length - 1
                    Select Case i
                        Case 0
                            Me.Usage = GetUsage(Left(StrElements(i), 1))
                            Me.UsageIndex = CInt(Mid(StrElements(i), 2, StrElements(i).Length - 1))

                        Case 1 And Not StrElements.Length - 1   'index 1, and not last index
                            If StrElements(i) <> "" Then
                                Me.Offset = CInt("&H" & StrElements(i))
                            End If

                        Case StrElements.Length - 1     'last index
                            Me.DataType = GetDeclarationType(StrElements(i))

                    End Select



                Next i

            End Sub

            'Public sub Save(ByVal w As FileWriter)
            'End sub

            Public Function ToVertexFormatString(ByVal VertexFormatString As String) As String
                '-- VertexFormatString is used as base string: format + uknown values (unk0, unk1) are used, others values (usage, usageIndex, offset, dataType) are taken from local (Me.) !! ---
                '%c%X:%02X:%02X:%04X:%s - usage, usageIndex, offset, unk0, unk1, dataType
                '%c%X:%02X:%02X:%s - usage, usageIndex, offset, unk0, dataType
                '%c%d:%X:%s - usage, usageIndex, offset, dataType
                '%c%d:%s - usage, usageIndex, dataType
                Dim StrElements As String() = Split(VertexFormatString, ":")

                For i = 0 To StrElements.Length - 1
                    Select Case i
                        Case 0
                            StrElements(i) = GetUsageString(Me.Usage) & CStr(Me.UsageIndex)
                        Case 1 And Not StrElements.Length - 1   'index 1, and not last index
                            If StrElements(i) <> "" Then
                                StrElements(i) = Hex(CByte(Me.Offset))
                                If StrElements(i).Length = 1 Then
                                    StrElements(i) = "0" & StrElements(i)
                                End If
                            End If

                        Case StrElements.Length - 1     'last index
                            StrElements(i) = GetDeclarationTypeString(Me.DataType)

                    End Select

                Next i

                Return Join(StrElements, ":")
            End Function

            Public Function GetUsage(ByVal StrUsage As String) As DeclarationUsage

                Select Case StrUsage
                    Case "p"
                        Return DeclarationUsage.Position
                    Case "n"
                        Return DeclarationUsage.Normal
                    Case "g"
                        Return DeclarationUsage.Tangent
                    Case "t"
                        Return DeclarationUsage.TextureCoordinate
                    Case "i"
                        Return DeclarationUsage.BlendIndices
                    Case "w"
                        Return DeclarationUsage.BlendWeight
                    Case "b"
                        Return DeclarationUsage.BiNormal
                    Case "c"
                        Return DeclarationUsage.Color
                    Case Else
                        MsgBox("Unknown DeclarationUsage found: " & StrUsage)
                        Return Nothing

                End Select

            End Function
            Public Function GetUsageString(ByVal Usage As DeclarationUsage) As String

                Select Case Usage
                    Case DeclarationUsage.Position
                        Return "p"
                    Case DeclarationUsage.Normal
                        Return "n"
                    Case DeclarationUsage.Tangent
                        Return "g"
                    Case DeclarationUsage.TextureCoordinate
                        Return "t"
                    Case DeclarationUsage.BlendIndices
                        Return "i"
                    Case DeclarationUsage.BlendWeight
                        Return "w"
                    Case DeclarationUsage.BiNormal
                        Return "b"
                    Case DeclarationUsage.Color
                        Return "c"
                    Case Else
                        MsgBox("Unknown DeclarationUsage found: " & Usage.ToString)
                        Return Nothing

                End Select

            End Function
            Public Function GetDeclarationType(ByVal StrType As String) As D3DDECLTYPE

                Select Case StrType
                    Case "3f32"
                        Return D3DDECLTYPE.FLOAT3
                    Case "4f16"
                        Return D3DDECLTYPE.FLOAT16_4
                    Case "3s10n"
                        Return D3DDECLTYPE.DEC3N
                    Case "2f16"
                        Return D3DDECLTYPE.FLOAT16_2
                    Case "2f32"
                        Return D3DDECLTYPE.FLOAT2
                    Case "4u8"
                        Return D3DDECLTYPE.UBYTE4
                    Case "4u8n"
                        Return D3DDECLTYPE.UBYTE4N
                    Case "4u16"
                        Return D3DDECLTYPE.USHORT4
                    Case "2u16n"   'experimental !!    -> 'found at FIFA07 hairmodels (texcoordinates) (RW4), not at newer fifas
                        Return D3DDECLTYPE.USHORT2N
                    Case "4u16n"   'experimental !!    -> 'found at FIFA07 hairmodels (blendweights) (RW4), not at newer fifas
                        Return D3DDECLTYPE.USHORT4N
                    Case "4u8rgba8"  'found at FIFA07 (RW4), not at newer fifas
                        Return D3DDECLTYPE.D3DCOLOR
                    Case Else
                        MsgBox("Unknown DeclarationType found: " & StrType)
                        Return Nothing
                End Select

            End Function
            Public Function GetDeclarationTypeString(ByVal Type As D3DDECLTYPE) As String

                Select Case Type
                    Case D3DDECLTYPE.FLOAT3
                        Return "3f32"
                    Case D3DDECLTYPE.FLOAT16_4
                        Return "4f16"
                    Case D3DDECLTYPE.DEC3N
                        Return "3s10n"
                    Case D3DDECLTYPE.FLOAT16_2
                        Return "2f16"
                    Case D3DDECLTYPE.FLOAT2
                        Return "2f32"
                    Case D3DDECLTYPE.UBYTE4
                        Return "4u8"
                    Case D3DDECLTYPE.UBYTE4N
                        Return "4u8n"
                    Case D3DDECLTYPE.USHORT4
                        Return "4u16"
                    Case D3DDECLTYPE.USHORT2N   'experimental !!    -> 'found at FIFA07 hairmodels (texcoordinates) (RW4), not at newer fifas
                        Return "2u16n"
                    Case D3DDECLTYPE.USHORT4N   'experimental !!    -> 'found at FIFA07 hairmodels (blendweights) (RW4), not at newer fifas
                        Return "4u16n"
                    Case D3DDECLTYPE.D3DCOLOR  'found at FIFA07 (RW4), not at newer fifas
                        Return "4u8rgba8"
                    Case Else
                        MsgBox("Unknown DeclarationType found: " & Type.ToString)
                        Return Nothing
                End Select

            End Function

            'Public Property Usage As DeclarationUsage
            'Public Property UsageIndex As UInteger
            'Public Property DataType As D3DDECLTYPE
            'Public Property Offset As UInteger

        End Class
    End Class


End Namespace