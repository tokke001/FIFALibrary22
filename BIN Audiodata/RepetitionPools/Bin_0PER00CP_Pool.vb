Namespace AudioBin.RepetitionPools
    Public Class Pool
        Public Sub New()
        End Sub
        Public Sub New(ByVal r As BinaryReader)
            Me.Id = r.ReadInt32
            Me.OffsetPool = r.ReadInt32

            Dim m_basestream As UInteger = r.BaseStream.Position
            r.BaseStream.Position = Me.OffsetPool

            Me.PoolType = GetPoolType(r)

            Select Case Me.PoolType
                Case EPoolType.TimedRepetitionPool
                    Me.Id = r.ReadInt32
                    r.ReadInt32()    '0 value
                    Me.PoolSize = r.ReadInt32
                    r.ReadInt32()    '0 value
                    Me.RepeatTime = r.ReadSingle
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    Me.Name = FifaUtil.ReadNullTerminatedString(r)

                Case EPoolType.UseOnceRepetitionPool
                    Me.Id = r.ReadInt32
                    Me.PoolSize = r.ReadInt32
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    Me.Name = FifaUtil.ReadNullTerminatedString(r)

                Case EPoolType.ShuffleRepetitionPool
                    Me.Id = r.ReadInt32
                    Me.PoolSize = r.ReadInt32
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    r.ReadInt32()    '0 value
                    Me.Name = FifaUtil.ReadNullTerminatedString(r)
            End Select

            r.BaseStream.Position = m_basestream

        End Sub

        Public Function GetPoolType(ByVal r As BinaryReader) As EPoolType
            Dim StrType As String = New String(r.ReadChars(4))

            Select Case StrType
                Case "0MIT"
                    Return EPoolType.TimedRepetitionPool
                Case "0ESU"
                    Return EPoolType.UseOnceRepetitionPool
                Case "0FHS"
                    Return EPoolType.ShuffleRepetitionPool
                Case Else
                    MsgBox("Unkown PoolType found: " & StrType)
                    Return Nothing
            End Select

        End Function

        'Public Sub Save(ByVal w As BinaryWriter)

        'End Sub
        Public Property Id As Integer
        Public Property OffsetPool As Integer
        Public Property PoolType As EPoolType
        Public Property PoolSize As Integer
        Public Property RepeatTime As Single
        Public Property Name As String


        Public Enum EPoolType As Byte
            TimedRepetitionPool
            UseOnceRepetitionPool
            ShuffleRepetitionPool
        End Enum

    End Class
End Namespace