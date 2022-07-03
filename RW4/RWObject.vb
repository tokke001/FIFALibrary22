Imports FIFALibrary22.Rw.Core.Arena
Namespace Rw
    Public MustInherit Class RwObject
        'rw::graphics::RwgObjectType

        ''' <summary>
        ''' The owner Arena class. </summary>
        Protected Friend RwArena As Rw.Core.Arena.Arena

        ''' <summary>
        ''' Object metadata used when generating the file. Not all objects require this. </summary>
        Protected Friend SectionInfo As ArenaDictEntry

        Public Sub New(ByVal RwArena As Rw.Core.Arena.Arena)
            Me.RwArena = RwArena
        End Sub
        'Public Sub New(ByVal r As FileReader) 'ByVal renderWare As RW4Section)
        '    Me.Load(r)
        'End Sub

        ''' <summary>
        ''' Returns the Object metadata used when generating the file. Not all objects require this. </summary>
        Public Overridable Function GetSectionInfo() As ArenaDictEntry
            Return SectionInfo
        End Function

        ''' <summary>
        ''' Reads the data of the object from the given stream. The stream file pointer is at the start offset of the object. </summary>
        ''' <param name="r"> </param>
        Public MustOverride Sub Load(ByVal r As FileReader)

        ''' <summary>
        ''' Writes the data of the object to the given stream. The stream file pointer is at the start offset of the object. </summary>
        ''' <param name="w"> </param>
        Public MustOverride Sub Save(ByVal w As FileWriter)

        ''' <summary>
        ''' Returns the 32-bit code used to uniquely identify the type of object.
        ''' </summary>
        Public MustOverride Function GetTypeCode() As Rw.SectionTypeCode

        ''' <summary>
        ''' Returns the alignment used by the section data, if any. Padding bytes will be added before the object data so that
        ''' the start offset can be divided by the alignment.
        ''' </summary>
        Public Overridable Function GetAlignment() As Integer
            Return 0
        End Function

        ''' <summary>
        ''' Returns the owner Arena object.
        ''' </summary>
        Public Function GetRwArena() As Rw.Core.Arena.Arena
            Return RwArena
        End Function
    End Class
End Namespace