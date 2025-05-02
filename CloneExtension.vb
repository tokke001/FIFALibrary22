Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Public Module CloneExtension
    '--> Deep cloning objects -> https://stackoverflow.com/questions/78536/deep-cloning-objects
    ''' <summary>
    ''' Creates a New object that Is a copy of the current instance. </summary>
    ''' <Returns>
    ''' A New object that Is a copy of this instance.  </Returns>
    <System.Runtime.CompilerServices.Extension>
    Function Clone(Of T)(ByVal source As T) As T
        If Not GetType(T).IsSerializable Then
            Throw New ArgumentException("The type must be serializable.", NameOf(source))
        End If

        ' Don't serialize a null object, simply return the default for that object
        If source Is Nothing Then
            Return Nothing
        End If

        Using stream = New MemoryStream()
            Dim formatter As IFormatter = New BinaryFormatter()
            formatter.Serialize(stream, source)
            stream.Seek(0, SeekOrigin.Begin)
            Return CType(formatter.Deserialize(stream), T)
        End Using
    End Function
End Module
