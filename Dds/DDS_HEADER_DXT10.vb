Namespace Dds
    Public Class DDS_HEADER_DXT10
        Public Sub New()

        End Sub
        Public Sub New(ByVal r As FileReader)
            Me.dxgiFormat = r.ReadInt32
            Me.resourceDimension = r.ReadInt32
            Me.miscFlag = r.ReadInt32
            Me.arraySize = r.ReadInt32
            Me.miscFlags2 = r.ReadInt32
        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(CUInt(Me.dxgiFormat))
            w.Write(CUInt(Me.resourceDimension))
            w.Write(CUInt(Me.miscFlag))
            w.Write(Me.arraySize)
            w.Write(CUInt(Me.miscFlags2))

        End Sub
        Public Sub PrintValues()
            PrintValues(0)
        End Sub

        Public Sub PrintValues(ByVal nSpace As Integer)
            Dim sSpace As String = ""
            For i As Integer = 0 To nSpace - 1
                sSpace = sSpace & "	"
            Next i
            Console.WriteLine(sSpace & "DX10 Header: ")
            Console.WriteLine(sSpace & "	dxgiFormat: " & dxgiFormat.ToString) '& " (" & dxgiFormat.ToString & ")")
            Console.WriteLine(sSpace & "	resourceDimension: " & resourceDimension.ToString) '& " (" & resourceDimension.ToString & ")")
            Console.WriteLine(sSpace & "	miscFlag: " & miscFlag)
            Console.WriteLine(sSpace & "	arraySize: " & arraySize)
            Console.WriteLine(sSpace & "	reserved: " & miscFlags2)
        End Sub


        ' Fields
        Public Property dxgiFormat As DXGI_FORMAT                       'The surface pixel format
        Public Property resourceDimension As D3D10_RESOURCE_DIMENSION   'Identifies the type of resource.
        Public Property miscFlag As D3D10_RESOURCE_MISC                 'Identifies other, less common options for resources. 
        Public Property arraySize As UInt32                             'The number of elements in the array.
        Public Property miscFlags2 As D3D10_ALPHA_MODE                            'Contains additional metadata (formerly was reserved). 

        <Flags>
        Public Enum D3D10_RESOURCE_MISC As UInt32
            DDS_RESOURCE_MISC_TEXTURECUBE = &H4     'Indicates a 2D texture is a cube-map texture.
        End Enum

        <Flags>
        Public Enum D3D10_ALPHA_MODE As UInt32
            DDS_ALPHA_MODE_UNKNOWN = &H0        'Alpha channel content Is unknown. This Is the value For legacy files, which typically Is assumed To be 'straight' alpha. 	
            DDS_ALPHA_MODE_STRAIGHT = &H1       'Any alpha channel content Is presumed To use straight alpha. 	
            DDS_ALPHA_MODE_PREMULTIPLIED = &H2  'Any alpha channel content Is Using premultiplied alpha. The only legacy file formats that indicate this information are 'DX2' and 'DX4'. 	
            DDS_ALPHA_MODE_OPAQUE = &H3         'Any alpha channel content Is all Set To fully opaque. 	
            DDS_ALPHA_MODE_CUSTOM = &H4         'Any alpha channel content Is being used As a 4th channel And Is Not intended To represent transparency (straight Or premultiplied). 	
        End Enum
    End Class

End Namespace