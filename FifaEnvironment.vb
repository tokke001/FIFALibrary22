Imports System.Windows.Forms

Public Class FifaEnvironment
    ' Methods
    Public Shared Function Initialize() As Boolean
        FifaEnvironment.m_TempFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) & "\FL_temp"
        FifaEnvironment.m_ExportFolder = FifaEnvironment.m_TempFolder   'If(FifaEnvironment.m_UserOptions.m_AutoExportFolder, FifaEnvironment.m_TempFolder, FifaEnvironment.m_UserOptions.m_ExportFolder)
        If Not Directory.Exists(FifaEnvironment.m_TempFolder) Then
            Directory.CreateDirectory(FifaEnvironment.m_TempFolder)
        End If
        If Not Directory.Exists(FifaEnvironment.m_ExportFolder) Then
            Directory.CreateDirectory(FifaEnvironment.m_ExportFolder)
        End If

        FifaEnvironment.InitializeLaunchFolder()
        Return True
    End Function

    Private Shared Sub InitializeLaunchFolder()
        'Dim index As Integer = Environment.CommandLine.IndexOf(".exe")
        'Dim i As Integer = index
        'Do While (i >= 0)
        'If (Environment.CommandLine(i) = "\"c) Then
        'index = i
        'Exit Do
        'End If
        'i -= 1
        'Loop
        'If (index >= 0) Then
        'FifaEnvironment.m_LaunchDir = Environment.CommandLine.Substring(1, (index - 1))
        'End If
        'MsgBox(Environment.CommandLine.ToString)
        FifaEnvironment.m_LaunchDir = Application.StartupPath
    End Sub


    ' Properties
    Public Shared Property ExportFolder As String
        Get
            Return FifaEnvironment.m_ExportFolder
        End Get
        Set(value As String)
            FifaEnvironment.m_ExportFolder = value
        End Set
    End Property
    Public Shared ReadOnly Property TempFolder As String
        Get
            Return FifaEnvironment.m_TempFolder
        End Get
    End Property
    Public Shared ReadOnly Property LaunchDir As String
        Get
            Return FifaEnvironment.m_LaunchDir
        End Get
    End Property

    ' Fields
    'Private Shared m_RootDir As String
    'Private Shared m_GameDir As String
    Private Shared m_ExportFolder As String
    Private Shared m_TempFolder As String
    Private Shared m_LaunchDir As String

End Class

