Namespace Rx3
    Public Class Rx3FileHeader
        ' Methods

        Public Sub New(byval Endianness As Endian)
            Me.Endianness = Endianness
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Signature = New String(r.ReadChars(3))
            Me.Endianness = GetFileEndianness(New String(r.ReadChars(1)))
            r.Endianness = Me.Endianness

            Me.Version = r.ReadUInt32                       'file version? (4)
            Me.Size = r.ReadUInt32
            Me.NumSections = r.ReadUInt32

        End Sub

        Private Function GetFileEndianness(ByVal m_Endianness As String) As Endian
            If m_Endianness = "b" Then
                Return Endian.Big
            End If

            Return Endian.Little
        End Function

        Public Sub Save(ByVal w As FileWriter)
            w.Endianness = Me.Endianness 'GetFileEndianness(Me.Endianness)
            Dim StrEndian As String = If(Me.Endianness = Endian.Big, "b", "l")

            w.Write(Me.Signature.ToCharArray)
            w.Write(StrEndian.ToCharArray)

            w.Write(Me.Version)
            w.Write(Me.Size)
            w.Write(Me.NumSections)

        End Sub


        Private _Size As UInteger
        Private _NumSections As UInteger
        Private _Signature As String = "RX3"
        ' Properties
        ''' <summary>
        ''' Total size of Rx3-File or Rx3-Section. (ReadOnly). </summary>
        Public Property Size As UInteger
            Get
                Return _Size
            End Get
            Friend Set
                _Size = Value
            End Set
        End Property
        ''' <summary>
        ''' Number of sections (ReadOnly). </summary>
        Public Property NumSections As UInteger
            Get
                Return _NumSections
            End Get
            Friend Set
                _NumSections = Value
            End Set
        End Property
        ''' <summary>
        ''' Signature of the Rx3-File or Rx3-Section (ReadOnly). </summary>
        Public Property Signature As String
            Get
                Return _Signature
            End Get
            Private Set
                _Signature = Value
            End Set
        End Property
        ''' <summary>
        ''' Endianness of the Rx3-File or Rx3-Section. </summary>
        Public Property Endianness As Endian = Endian.Little 'String = "l"
        ''' <summary>
        ''' Version of the Rx3-File or Rx3-Section (Default 4). </summary>
        Public Property Version As UInteger = 4

    End Class
End Namespace