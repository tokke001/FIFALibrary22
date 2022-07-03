
Namespace Rx3
    Public MustInherit Class Rx3Object

        ''' The owner rx3file class. 
        'Protected Friend Rx3File As Rx3FileRx3Section

        ''' <summary>
        ''' Object metadata used when generating the file. Not all objects require this. </summary>
        Protected Friend SectionInfo As SectionHeader

        Public Sub New()

        End Sub
        'Public Sub New(ByVal r As FileReader) 'ByVal renderWare As RW4Section)
        '    Me.Load(r)
        'End Sub

        ''' <summary>
        ''' Returns the Object metadata used when generating the file. Not all objects require this. </summary>
        Public Overridable Function GetSectionInfo() As SectionHeader
            Return SectionInfo
        End Function

        'Public MustOverride Sub Load(ByVal r As FileReader)

        'Public MustOverride Sub Save(ByVal w As FileWriter)

        ''' <summary>
        ''' Returns the 32-bit code used to uniquely identify the type of object.
        ''' </summary>
        Public MustOverride Function GetTypeCode() As Rx3.SectionHash

        ''' <summary>
        ''' Returns the alignment used by the section data, if any.
        ''' </summary>
        Public Overridable Function GetAlignment() As Integer
            Return 16
        End Function

        ''' Returns the owner rx3file object.
        'Public Function GetRx3File() As Rx3FileRx3Section
        '    Return Rx3File
        'End Function
    End Class
End Namespace