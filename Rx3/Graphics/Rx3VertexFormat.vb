Imports Microsoft.DirectX.Direct3D
Imports D3DDECLTYPE = FIFALibrary22.Rw.D3D.D3DDECLTYPE
Namespace Rx3
    Public Class VertexFormat
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.VERTEX_FORMAT
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Dim StartPosition As Integer = r.BaseStream.Position

            Me.TotalSize = r.ReadUInt32
            Dim m_VertexFormatSize As UInteger = r.ReadUInt32
            Me.Padding(0) = r.ReadUInt32
            Me.Padding(1) = r.ReadUInt32

            Dim m_StrVertexFormat As String() = Split(CStr(r.ReadChars(m_VertexFormatSize - 1)), " ") 'dont read last char (null-terminator)

            'Create elements list
            ReDim Me.Elements(m_StrVertexFormat.Length - 1)
            For i = 0 To m_StrVertexFormat.Length - 1
                Me.Elements(i) = New Element(m_StrVertexFormat(i))
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.TotalSize)
            w.Write(Me.VertexFormatSize)
            w.Write(Me.Padding(0))
            w.Write(Me.Padding(1))

            For i = 0 To Me.Elements.Length - 1

                Me.Elements(i).Save(w)

                If i <> Me.Elements.Length - 1 Then     'write space splitter
                    w.Write(" "c)
                End If

            Next i
            w.Write(CByte(0))                               'write null terminator 

            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub

        Private Function GetVertexFormatSize(ByVal m_Elements As Element())
            Dim Size As Long

            For i = 0 To m_Elements.Length - 1
                Size += m_Elements(i).VertexFormatString.Length   'string size
                Size += 1                           'space splitter (or null terminator at total end) size 
            Next i

            Return Size
        End Function

        Private m_TotalSize As UInteger

        ''' <summary>
        ''' Total section size (ReadOnly). </summary>
        Public Property TotalSize As UInteger
            Get
                Return m_TotalSize
            End Get
            Private Set
                m_TotalSize = Value
            End Set
        End Property
        ''' <summary>
        ''' Size of the VertexFormat strings (ReadOnly). </summary>
        Public ReadOnly Property VertexFormatSize As UInteger
            Get
                Return GetVertexFormatSize(Me.Elements)
            End Get
        End Property
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Padding As UInteger() = New UInteger(2 - 1) {}   '{ 0, 0 }, maybe padding
        ''' <summary>
        ''' Gets/Sets the VertexFormat Elements. </summary>
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

            Public Sub New(ByVal m_VertexElement As VertexElement, Optional Unk0 As Byte? = Nothing, Optional Unk1 As Byte? = Nothing)
                MyBase.Offset = m_VertexElement.Offset
                MyBase.DataType = m_VertexElement.DataType
                MyBase.Usage = m_VertexElement.Usage
                MyBase.UsageIndex = m_VertexElement.UsageIndex
                Me.Unk0 = Unk0
                Me.Unk1 = Unk1
            End Sub

            Public Sub New(ByVal r As FileReader, ByVal StringSize As UInteger)
                Me.Load(CStr(r.ReadChars(StringSize)))
            End Sub

            Public Sub New(ByVal StrVertexElement As String)
                Me.Load(StrVertexElement)
            End Sub

            Public Sub Load(ByVal VertexFormatString As String)
                Me.VertexFormatString = VertexFormatString
            End Sub

            Public Sub Save(ByVal w As FileWriter)
                w.Write(Me.VertexFormatString.ToCharArray)
            End Sub

            Private Sub FromVertexFormatString(ByVal VertexFormatString As String)
                '%c%d:%s - usage, usageIndex : dataType
                '%c%d:%X:%s - usage, usageIndex : offset : dataType
                '%c%X:%02X:%02X:%s - usage, usageIndex : offset : unk0 : dataType
                '%c%X:%02X:%02X:%04X:%s - usage, usageIndex : offset : unk0 : unk1 : dataType
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

                        Case 2 And StrElements.Length >= 4
                            Me.Unk0 = CInt("&H" & StrElements(i))

                        Case 3 And StrElements.Length >= 5
                            Me.Unk1 = CInt("&H" & StrElements(i))

                        Case StrElements.Length - 1     'last index
                            Me.DataType = GetDeclarationType(StrElements(i))

                    End Select

                Next i
            End Sub

            Private Function ToVertexFormatString() As String
                '-- VertexFormatString is used as base string: format + uknown values (unk0, unk1) are used, others values (usage, usageIndex, offset, dataType) are taken from local (Me.) !! ---
                '%c%X:%02X:%02X:%04X:%s - usage, usageIndex, offset, unk0, unk1, dataType
                '%c%X:%02X:%02X:%s - usage, usageIndex, offset, unk0, dataType
                '%c%d:%X:%s - usage, usageIndex, offset, dataType
                '%c%d:%s - usage, usageIndex, dataType
                Dim StrElements As New List(Of String) '= Split(VertexFormatString, ":")

                StrElements.Add(GetUsageString(Me.Usage) & CStr(Me.UsageIndex))

                If Me.Offset IsNot Nothing Then
                    Dim StrOffset As String = Hex(CByte(Me.Offset))
                    If StrOffset.Length = 1 Then
                        StrOffset = "0" & StrOffset
                    End If
                    StrElements.Add(StrOffset)

                    If Me.Unk0 IsNot Nothing Then
                        Dim StrUnk0 As String = Hex(CByte(Me.Unk0))
                        If StrUnk0.Length = 1 Then
                            StrUnk0 = "0" & StrUnk0
                        End If
                        StrElements.Add(StrUnk0)

                        If Me.Unk1 IsNot Nothing Then
                            Dim StrUnk1 As String = Hex(CByte(Me.Unk1))
                            If StrUnk1.Length <> 4 Then
                                Dim StrPrefix As String = ""
                                For i = 1 To 4 - StrUnk1.Length
                                    StrPrefix &= "0"
                                Next
                                StrUnk1 = StrPrefix & StrUnk1
                            End If
                            StrElements.Add(StrUnk1)
                        End If
                    End If
                End If

                StrElements.Add(GetDeclarationTypeString(Me.DataType))

                Return Join(StrElements.ToArray, ":")
            End Function

            Private Function GetUsage(ByVal StrUsage As String) As DeclarationUsage

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
            Private Function GetUsageString(ByVal Usage As DeclarationUsage) As String

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
            Private Function GetDeclarationType(ByVal StrType As String) As D3DDECLTYPE

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
            Private Function GetDeclarationTypeString(ByVal Type As D3DDECLTYPE) As String

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
            Public Property Unk0 As Byte? = Nothing     'ussually 0     (DeclarationMethod https://docs.microsoft.com/en-us/previous-versions/ms859059(v=msdn.10) / Stream https://docs.microsoft.com/en-us/previous-versions/ms847453(v=msdn.10) ) ?
            Public Property Unk1 As Byte? = Nothing     'ussually 1     (DeclarationMethod https://docs.microsoft.com/en-us/previous-versions/ms859059(v=msdn.10) / Stream https://docs.microsoft.com/en-us/previous-versions/ms847453(v=msdn.10) ) ?

            Public Property VertexFormatString As String
                Get
                    Return Me.ToVertexFormatString()
                End Get
                Set
                    Me.FromVertexFormatString(Value)
                End Set
            End Property
        End Class
    End Class


End Namespace