Namespace Rx3
    Public Class SectionHeader
        ' Methods
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        'Public Function Is3dDirectory() As Boolean
        'Return (Me.Signature = &H22B2BE36)
        'End Function

        'Public Function IsImageDirectory() As Boolean
        'Return (Me.Signature = &H6BD085DC)
        'End Function

        'Public Function IsIndexStream() As Boolean
        'Return (Me.Signature = &H5878F4)
        'End Function

        'Public Function IsTexture() As Boolean
        'Return (Me.Signature = &H700B60DA)
        'End Function

        'Public Function IsVertexVector() As Boolean
        'Return (Me.Signature = &H587AA1)
        'End Function

        Public Sub Load(ByVal r As FileReader)

            Me.Signature = r.ReadUInt32
            Me.Offset = r.ReadUInt32
            Me.Size = r.ReadUInt32
            Me.Unknown = r.ReadUInt32  'maybe: padding

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.Signature)
            w.Write(Me.Offset)
            w.Write(Me.Size)
            w.Write(Me.Unknown)

        End Sub


        Private _Offset As UInteger
        Private _Size As UInteger
        ' Properties
        ''' <summary>
        ''' Signature of the Section. </summary>
        Public Property Signature As SectionHash
        ''' <summary>
        ''' File-offset to the section. (ReadOnly). </summary>
        Public Property Offset As UInteger
            Get
                Return _Offset
            End Get
            Friend Set
                _Offset = Value
            End Set
        End Property
        ''' <summary>
        ''' Size of of the section. (ReadOnly). </summary>
        Public Property Size As UInteger
            Get
                Return _Size
            End Get
            Friend Set
                _Size = Value
            End Set
        End Property
        ''' <summary>
        ''' Empty 0-value. </summary>
        Public Property Unknown As UInteger = 0

    End Class

End Namespace