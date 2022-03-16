
Public Class FileReader
    Inherits BinaryReader
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
        Dim num As Integer = CInt((Me.BaseStream.Position And 3))
        If (num <> 0) Then
            Dim baseStream As Stream = Me.BaseStream
            baseStream.Position = (baseStream.Position + (4 - num))
        End If
        Me.m_CurrentBitPosition = 0
    End Sub

    Public Sub AlignToByte()
        If (Me.m_CurrentBitPosition <> 0) Then
            Me.m_CurrentBitPosition = 0
            Me.m_CurrentByte = 0
        End If
    End Sub

    '------ for DB s --------------
    'Public Function PopInteger(ByVal fieldDescriptor As FieldDescriptor) As Integer
    'If (Me.m_Endianness = Endian.Little) Then
    'Return Me.PopIntegerPc(fieldDescriptor)
    'End If
    'Return Me.PopIntegerXbox(fieldDescriptor)
    'End Function

    'Private Function PopIntegerPc(ByVal fieldDescriptor As FieldDescriptor) As Integer
    'Dim num As Integer = 0
    'Dim depth As Integer = fieldDescriptor.Depth
    'Dim num3 As Integer = 0
    'If (Me.m_CurrentBitPosition <> 0) Then
    '       num3 = (8 - Me.m_CurrentBitPosition)
    '      num = (Me.m_CurrentByte >> Me.m_CurrentBitPosition)
    'End If
    'Do While (num3 < depth)
    'Me.m_CurrentByte = Me.ReadByte
    '       num = (num + (Me.m_CurrentByte << num3))
    '      num3 = (num3 + 8)
    'Loop
    'Me.m_CurrentBitPosition = (((depth + 8) - num3) And 7)
    'Dim num4 As Integer = CInt(((CLng(1) << depth) - 1))
    '   num = (num And num4)
    'Return (num + fieldDescriptor.RangeLow)
    'End Function

    'Private Function PopIntegerXbox(ByVal fieldDescriptor As FieldDescriptor) As Integer
    'Dim num As Integer = Me.ReadByte
    'Dim num2 As Integer = (fieldDescriptor.BitOffset Mod 8)
    'Dim num3 As Integer = 0
    'Dim i As Integer = (fieldDescriptor.Depth - 1)
    'Do While (i >= 0)
    'Dim num5 As Integer = If(((num And (CInt(&H80) >> num2)) = 0), 0, 1)
    '       num3 = (num3 + (num5 << i))
    '       num2 += 1
    'If (num2 = 8) Then
    '           num = Me.ReadByte
    '          num2 = 0
    'End If
    '       i -= 1
    'Loop
    'Return (num3 + fieldDescriptor.RangeLow)
    'End Function
    '-----------------------

    Public Overrides Function ReadDouble() As Double
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(8)
            Array.Reverse(m_array)
            Return BitConverter.ToDouble(m_array, 0)
        End If
        Return MyBase.ReadDouble
    End Function

    Public Overrides Function ReadInt16() As Short
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(2)
            Array.Reverse(m_array)
            Return BitConverter.ToInt16(m_array, 0)
        End If
        Return MyBase.ReadInt16
    End Function

    Public Overrides Function ReadInt32() As Integer
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(4)
            Array.Reverse(m_array)
            Return BitConverter.ToInt32(m_array, 0)
        End If
        Return MyBase.ReadInt32
    End Function

    Public Overrides Function ReadInt64() As Long
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(8)
            Array.Reverse(m_array)
            Return BitConverter.ToInt64(m_array, 0)
        End If
        Return MyBase.ReadInt64
    End Function

    Public Overrides Function ReadSingle() As Single
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(4)
            Array.Reverse(m_array)
            Return BitConverter.ToSingle(m_array, 0)
        End If
        Return MyBase.ReadSingle
    End Function

    Public Overrides Function ReadUInt16() As UInt16
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(2)
            Array.Reverse(m_array)
            Return BitConverter.ToUInt16(m_array, 0)
        End If
        Return MyBase.ReadUInt16
    End Function

    Public Overrides Function ReadUInt32() As UInt32
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(4)
            Array.Reverse(m_array)
            Return BitConverter.ToUInt32(m_array, 0)
        End If
        Return MyBase.ReadUInt32
    End Function

    Public Overrides Function ReadUInt64() As UInt64
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(8)
            Array.Reverse(m_array)
            Return BitConverter.ToUInt64(m_array, 0)
        End If
        Return MyBase.ReadUInt64
    End Function

    Public Function Read16BitEncodedSingle() As Single     'ReadFloat16
        If (Me.m_Endianness = Endian.Big) Then
            Dim m_array As Byte() = MyBase.ReadBytes(2)
            Array.Reverse(m_array)
            Return FifaUtil.ConvertToFloat(BitConverter.ToInt16(m_array, 0))
        End If
        Return FifaUtil.ConvertToFloat(MyBase.ReadInt16)
    End Function
    Public Function ReadUIntegers(count As Integer) As UInt32()
        Dim buffer As UInt32() = New UInt32(count - 1) {}
        For i = 0 To count - 1
            buffer(i) = Me.ReadUInt32()
        Next
        Return buffer
    End Function
    Public Function ReadIntegers(count As Integer) As Int32()
        Dim buffer As Int32() = New Int32(count - 1) {}
        For i = 0 To count - 1
            buffer(i) = Me.ReadInt32()
        Next
        Return buffer
    End Function

    Public Function ReadVector2() As Microsoft.DirectX.Vector2
        Return New Microsoft.DirectX.Vector2(Me.ReadSingle, Me.ReadSingle)
    End Function
    Public Function ReadVector3() As Microsoft.DirectX.Vector3
        Return New Microsoft.DirectX.Vector3(Me.ReadSingle, Me.ReadSingle, Me.ReadSingle)
    End Function
    Public Function ReadVector4() As Microsoft.DirectX.Vector4
        Return New Microsoft.DirectX.Vector4(Me.ReadSingle, Me.ReadSingle, Me.ReadSingle, Me.ReadSingle)
    End Function

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


Public Enum Endian
    ' Fields
    Little = 0
    Big = 1
End Enum
