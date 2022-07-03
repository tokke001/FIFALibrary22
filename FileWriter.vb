
Imports BCnEncoder.Shared

Public Class FileWriter
    Inherits BinaryWriter
    ' Methods
    Public Sub New(ByVal stream As Stream)
        MyBase.New(stream)
        Me.m_Endianness = Endian.Little
    End Sub
    Public Sub New(ByVal stream As Stream, ByVal Endianness As Endian)
        MyBase.New(stream)
        Me.m_Endianness = Endianness
    End Sub

    Public Sub Align(ByVal position As Long)
        Me.BaseStream.Position = position
        Me.m_CurrentBitPosition = 0
        Me.m_CurrentByte = 0
    End Sub

    Public Sub AlignTo32Bit()
        If (Me.m_CurrentBitPosition <> 0) Then
            Me.Write(Me.m_CurrentByte)
        End If
        Dim num As Integer = CInt((Me.BaseStream.Position And 3))
        If (num <> 0) Then
            Do While (num < 4)
                Me.Write(CByte(0))
                num += 1
            Loop
        End If
    End Sub

    Public Sub AlignToByte()
        If (Me.m_CurrentBitPosition <> 0) Then
            Me.Write(Me.m_CurrentByte)
        End If
    End Sub

    '------ for DB s --------------
    'Public Sub PushInteger(ByVal value As Integer, ByVal fieldDescriptor As FieldDescriptor)
    'If (Me.m_Endianness = Endian.Little) Then
    'Me.PushIntegerPc(value, fieldDescriptor)
    'Else
    'Me.PushIntegerXbox(value, fieldDescriptor)
    'End If
    'End Sub

    'Private Sub PushIntegerPc(ByVal value As Integer, ByVal fieldDescriptor As FieldDescriptor)
    'Dim depth As Integer = fieldDescriptor.Depth
    'Dim flag As Boolean = False
    'Dim num2 As Integer = (value - fieldDescriptor.RangeLow)
    'Do
    'If ((Me.m_CurrentBitPosition + depth) > 8) Then
    'Dim num3 As Integer = (8 - Me.m_CurrentBitPosition)
    'Dim num4 As Integer = ((CInt(1) << num3) - 1)
    'Dim num5 As Integer = (num2 And num4)
    'Me.m_CurrentByte = (Me.m_CurrentByte + (num5 << Me.m_CurrentBitPosition))
    'Me.Write(CByte(Me.m_CurrentByte))
    '           num2 = (num2 >> num3)
    'Me.m_CurrentByte = 0
    'Me.m_CurrentBitPosition = 0
    '           depth = (depth - num3)
    'ElseIf ((Me.m_CurrentBitPosition + depth) < 8) Then
    'Me.m_CurrentByte = (Me.m_CurrentByte + (num2 << Me.m_CurrentBitPosition))
    'Me.m_CurrentBitPosition = (Me.m_CurrentBitPosition + depth)
    '           flag = True
    'Else
    'Me.m_CurrentByte = (Me.m_CurrentByte + (num2 << Me.m_CurrentBitPosition))
    'Me.Write(CByte(Me.m_CurrentByte))
    'Me.m_CurrentByte = 0
    'Me.m_CurrentBitPosition = 0
    '           flag = True
    'End If
    'Loop While Not flag
    'End Sub

    'Private Sub PushIntegerXbox(ByVal value As Integer, ByVal fieldDescriptor As FieldDescriptor)
    'Dim reader As New FileReader(Me.BaseStream)
    'Dim num As Integer = 0
    'If (reader.BaseStream.Position < reader.BaseStream.Length) Then
    '       num = reader.ReadByte
    'Dim baseStream As Stream = reader.BaseStream
    '       baseStream.Position = (baseStream.Position - 1)
    'End If
    'Dim num2 As Integer = (fieldDescriptor.BitOffset Mod 8)
    'Dim num3 As Integer = (value - fieldDescriptor.RangeLow)
    'Dim i As Integer = (fieldDescriptor.Depth - 1)
    'Do While (i >= 0)
    'Dim num6 As Integer = (CInt(&H80) >> num2)
    'If ((num3 And (CInt(1) << i)) = 0) Then
    '           num = (num And Not num6)
    'Else
    '           num = (num Or num6)
    'End If
    '       num2 += 1
    'If (num2 = 8) Then
    'Dim num7 As Byte = CByte(num)
    'Me.Write(num7)
    '           num2 = 0
    '          num = 0
    'If (reader.BaseStream.Position < reader.BaseStream.Length) Then
    '               num = reader.ReadByte
    'Dim baseStream As Stream = reader.BaseStream
    '               baseStream.Position = (baseStream.Position - 1)
    'End If
    'End If
    '       i -= 1
    'Loop
    'If (num2 <> 0) Then
    'Dim num8 As Byte = CByte(num)
    'Me.Write(num8)
    'End If
    'End Sub
    '-----------------------
    Public Overrides Sub Write(ByVal value As Double)
        If (Me.m_Endianness = Endian.Big) Then
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Array.Reverse(bytes)
            MyBase.Write(bytes)
        Else
            MyBase.Write(value)
        End If
    End Sub

    Public Overrides Sub Write(ByVal value As Short)
        If (Me.m_Endianness = Endian.Big) Then
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Array.Reverse(bytes)
            MyBase.Write(bytes)
        Else
            MyBase.Write(value)
        End If
    End Sub

    Public Overrides Sub Write(ByVal value As Integer)
        If (Me.m_Endianness = Endian.Big) Then
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Array.Reverse(bytes)
            MyBase.Write(bytes)
        Else
            MyBase.Write(value)
        End If
    End Sub

    Public Overrides Sub Write(ByVal value As Long)
        If (Me.m_Endianness = Endian.Big) Then
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Array.Reverse(bytes)
            MyBase.Write(bytes)
        Else
            MyBase.Write(value)
        End If
    End Sub

    Public Overrides Sub Write(ByVal value As Single)
        If (Me.m_Endianness = Endian.Big) Then
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Array.Reverse(bytes)
            MyBase.Write(bytes)
        Else
            MyBase.Write(value)
        End If
    End Sub

    Public Overrides Sub Write(ByVal value As UInt16)
        If (Me.m_Endianness = Endian.Big) Then
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Array.Reverse(bytes)
            MyBase.Write(bytes)
        Else
            MyBase.Write(value)
        End If
    End Sub

    Public Overrides Sub Write(ByVal value As UInt32)
        If (Me.m_Endianness = Endian.Big) Then
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Array.Reverse(bytes)
            MyBase.Write(bytes)
        Else
            MyBase.Write(value)
        End If
    End Sub

    Public Overrides Sub Write(ByVal value As UInt64)
        If (Me.m_Endianness = Endian.Big) Then
            Dim bytes As Byte() = BitConverter.GetBytes(value)
            Array.Reverse(bytes)
            MyBase.Write(bytes)
        Else
            MyBase.Write(value)
        End If
    End Sub

    Public Overloads Sub Write(ByVal value As Half)
        Dim bytes As Byte() = Half.GetBytes(value)
        If (Me.m_Endianness = Endian.Big) Then
            Array.Reverse(bytes)
        End If
        MyBase.Write(bytes)
    End Sub

    Public Overloads Sub Write(ByVal buffer As Integer())
        If (buffer Is Nothing) Then
            Throw New ArgumentNullException("buffer")
        End If
        For i = 0 To buffer.Length - 1
            Me.Write(buffer(i))
        Next
    End Sub
    Public Overloads Sub Write(ByVal buffer As UInteger())
        If (buffer Is Nothing) Then
            Throw New ArgumentNullException("buffer")
        End If
        For i = 0 To buffer.Length - 1
            Me.Write(buffer(i))
        Next
    End Sub
    Public Overloads Sub Write(ByVal value As Microsoft.DirectX.Vector2)
        Me.Write(value.X)
        Me.Write(value.Y)
    End Sub
    Public Overloads Sub Write(ByVal value As Microsoft.DirectX.Vector3)
        Me.Write(value.X)
        Me.Write(value.Y)
        Me.Write(value.Z)
    End Sub
    Public Overloads Sub Write(ByVal value As Microsoft.DirectX.Vector4)
        Me.Write(value.X)
        Me.Write(value.Y)
        Me.Write(value.Z)
        Me.Write(value.W)
    End Sub
    Public Sub WritePendingByte()
        If (Me.m_CurrentBitPosition <> 0) Then
            Me.Write(CByte(Me.m_CurrentByte))
            Me.m_CurrentBitPosition = 0
            Me.m_CurrentByte = 0
        End If
    End Sub


    ' Properties
    Public Property Endianness As Endian
        Get
            Return Me.m_Endianness
        End Get
        Set(ByVal value As Endian)
            Me.m_Endianness = value
        End Set
    End Property


    ' Fields
    Private m_CurrentByte As Integer
    Private m_CurrentBitPosition As Integer
    Private m_Endianness As Endian
End Class
