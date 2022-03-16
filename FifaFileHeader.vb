
Public Class FifaFileHeader
    ' Methods
    Public Sub New()
        Me.m_BigFile = Nothing
        Me.m_StartPosition = 0
        Me.m_Name = Nothing
        Me.m_Size = 0
    End Sub

    Public Sub New(ByVal bigFile As FifaBigFile)
        Me.m_BigFile = bigFile
    End Sub

    Public Function Load(ByVal r As FileReader) As Boolean
        Me.m_StartPosition = FifaUtil.SwapEndian(r.ReadUInt32)
        Me.m_Size = FifaUtil.SwapEndian(r.ReadInt32)
        Me.m_Name = FifaUtil.ReadNullTerminatedString(r)
        Return True
    End Function

    Public Function Save(ByVal w As FileWriter) As Boolean
        w.Write(FifaUtil.SwapEndian(Me.m_StartPosition))
        w.Write(FifaUtil.SwapEndian(Me.m_Size))
        FifaUtil.WriteNullTerminatedString(w, Me.m_Name)
        Return True
    End Function


    ' Properties
    Public ReadOnly Property BigFile As FifaBigFile
        Get
            Return Me.m_BigFile
        End Get
    End Property

    Public Property StartPosition As UInt32
        Get
            Return Me.m_StartPosition
        End Get
        Set(ByVal value As UInt32)
            Me.m_StartPosition = value
        End Set
    End Property

    Public Property Size As Integer
        Get
            Return Me.m_Size
        End Get
        Set(ByVal value As Integer)
            Me.m_Size = value
        End Set
    End Property

    Public Property Name As String
        Get
            Return Me.m_Name
        End Get
        Set(ByVal value As String)
            Me.m_Name = value
        End Set
    End Property


    ' Fields
    Private m_BigFile As FifaBigFile
    Private m_StartPosition As UInt32
    Private m_Size As Integer
    Private m_Name As String
End Class

