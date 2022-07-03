
Public Class VertexElement
    ''' <summary>
    ''' Retrieves or sets the offset (if any) from the beginning of the stream to the beginning of the vertex data. </summary>
    Public Property Offset As UShort? = Nothing
    ''' <summary>
    ''' Retrieves or sets the data type that defines the data size for the vertex element. </summary>
    Public Property DataType As Rw.D3D.D3DDECLTYPE
    ''' <summary>
    ''' Retrieves or sets one or more usage values that define the intended use of the vertex data. </summary>
    Public Property Usage As Microsoft.DirectX.Direct3D.DeclarationUsage
    ''' <summary>
    ''' Modifies the usage data to allow the user to specify multiple usage types. </summary>
    Public Property UsageIndex As Byte

End Class
