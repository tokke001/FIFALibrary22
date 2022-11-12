Imports System.Globalization
Imports System.Windows.Forms

'http://www.soccergaming.com/index.php?threads/rx3-file-format-research-thread.6467750/
'http://www.soccergaming.com/index.php?threads/fifa-11-pc-file-formats-resarch-renderware-4-5-assets.6468020/

Partial Public Class RxFile

    Public Function CreateXFiles(ByVal baseFileName As String) As Boolean
        If (Me.NumMeshes = 0) Then
            Return False
        End If

        For i = 0 To Me.NumMeshes - 1
            Dim modeld As New Model3D

            modeld.Initialize(Me.Rx3Section.Sections.IndexBuffers(i).IndexData, Me.Rx3Section.Sections.VertexBuffers(i).VertexData, Me.PrimitiveTypes(i))

            Application.CurrentCulture = New CultureInfo("en-us")
            Dim stream As New FileStream((baseFileName & i.ToString & ".X"), FileMode.Create, FileAccess.Write)
            Dim w As New StreamWriter(stream)
            modeld.SaveXFile(w)
            w.Close()
            stream.Close()
        Next i
        Return True
    End Function


End Class