
Namespace Rx3
    Public MustInherit Class Rx3Object

        ''' The owner rx3file class. 
        Protected Friend Rx3File As Rx3FileRx3Section

        ''' Object metadata used when generating the file. Not all objects require this. 
        Protected Friend SectionInfo As SectionHeader

        Public Sub New(ByVal Rx3File As Rx3FileRx3Section)
            Me.Rx3File = Rx3File
        End Sub
        'Public Sub New(ByVal r As FileReader) 'ByVal renderWare As RW4Section)
        '    Me.Load(r)
        'End Sub

        Public Overridable Function GetSectionInfo() As SectionHeader
            Return SectionInfo
        End Function

        'Public MustOverride Sub Load(ByVal r As FileReader)

        'Public MustOverride Sub Save(ByVal w As FileWriter)

        ''' Returns the 32-bit code used to uniquely identify the type of object.
        Public MustOverride Function GetTypeCode() As Rx3.SectionHash

        ''' Returns the alignment used by the section data, if any
        Public Overridable Function GetAlignment() As Integer
            Return 16
        End Function

        ''' Returns the owner rx3file object.
        Public Function GetRx3File() As Rx3FileRx3Section
            Return Rx3File
        End Function
    End Class
End Namespace