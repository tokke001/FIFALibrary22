Namespace Rx3
    Public Class Rx3FileHeader
        ' Methods

        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Signature = New String(r.ReadChars(3))
            Me.Endianness = New String(r.ReadChars(1))
            r.Endianness = GetFileEndianness(Me.Endianness)

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
            w.Endianness = GetFileEndianness(Me.Endianness)

            w.Write(Me.Signature.ToCharArray)
            w.Write(Me.Endianness.ToCharArray)

            w.Write(Me.Version)
            w.Write(Me.Size)
            w.Write(Me.NumSections)

        End Sub


        ' Properties
        Public Property Size As UInteger
        Public Property NumSections As UInteger
        Public Property Signature As String
        Public Property Endianness As String
        Public Property Version As UInteger

    End Class
End Namespace