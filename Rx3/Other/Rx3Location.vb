Imports Microsoft.DirectX

Namespace Rx3
    Public Class Location
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.LOCATION
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Me.Position = r.ReadVector3
            Me.Rotation = r.ReadVector3
            Me.Unknown = r.ReadUInt32

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.TotalSize)
            w.Write(Me.Position)

            w.Write(Me.Rotation)
            w.Write(Me.Unknown)

            'Padding
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            'Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub


        'Private m_TotalSize As UInteger = 32

        ''' <summary>
        ''' Total section size. </summary>
        Public Property TotalSize As UInteger = 32  'at Rx3b files (FIFA 12/13) this isnt the section size: is unknown large value !
        '    Get
        '        Return m_TotalSize
        '    End Get
        '    Private Set
        '        m_TotalSize = Value
        '    End Set
        'End Property
        ''' <summary>
        ''' Gets/Sets the Position X/Y/Z values. </summary>
        Public Property Position As Vector3
        ''' <summary>
        ''' Gets/Sets the Rotation X/Y/Z values. </summary>
        Public Property Rotation As Vector3
        Public Property Unknown As UInteger         'always 0? maybe padding

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace