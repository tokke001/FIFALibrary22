Imports Microsoft.DirectX

Module testing3
    Sub EditCrowdDat()
        'Create a new CrowdDat file 
        Dim m_Crowd As New CrowdDat.DatFile

        '1 - Load the File
        Dim m_CurrentFileName As String = "C:\Documents\test\data\sceneassets\crowdplacement\crowd_1_1.dat"
        Dim FileType As CrowdDat.CrwdFileType = CrowdDat.CrwdFileType.FIFA_14
        m_Crowd.Load(m_CurrentFileName, FileType)

        '- header info
        Dim Signature As String = m_Crowd.Signature     ' = "CRWD"
        Dim Version As CrowdDat.CrwdFileType = m_Crowd.Unknown(0)
        Dim unknown As Byte = m_Crowd.Unknown(1)
        Dim NumSeats As UInteger = m_Crowd.NumSeats

        '2 - loop trough the Crowd Data
        For i = 0 To NumSeats - 1
            Dim Vertices As Vector3 = m_Crowd.CrowdData(i).Verts
            Dim Rotation As Single = m_Crowd.CrowdData(i).ZRot
            Dim Color As Byte() = m_Crowd.CrowdData(i).Color

            If FileType = CrowdDat.CrwdFileType.FIFA_14 Then
                Dim m_CrowdStatus As CrowdDat.CrowdStatus = m_Crowd.CrowdData(i).Status
                Dim m_Unknown As CrowdDat.CrowdUnknown3 = m_Crowd.CrowdData(i).Unknown

                Dim NumAwayCrowd As Byte = m_CrowdStatus.NumAwayCrowd
                Dim CrowdSize As Byte = m_CrowdStatus.CrowdSize
                Dim Value_2 As Byte = m_CrowdStatus.Value_2
                Dim Value_4 As Byte = m_CrowdStatus.Value_4

            ElseIf FileType = CrowdDat.CrwdFileType.FIFA_WC14 Then
                Dim m_CrowdStatus As Byte() = m_Crowd.CrowdData(i).Status   '4 byte size
                Dim m_Unknown As CrowdDat.CrowdUnknown4 = m_Crowd.CrowdData(i).Unknown

            ElseIf FileType = CrowdDat.CrwdFileType.FIFA_15 Then
                Dim m_CrowdStatus As CrowdDat.CrowdStatus = m_Crowd.CrowdData(i).Status
                Dim m_Unknown As Byte() = m_Crowd.CrowdData(i).Unknown      '9 byte size

                Dim NumAwayCrowd As Byte = m_CrowdStatus.NumAwayCrowd
                Dim CrowdSize As Byte = m_CrowdStatus.CrowdSize
                Dim Value_2 As Byte = m_CrowdStatus.Value_2
                Dim Value_4 As Byte = m_CrowdStatus.Value_4

            End If
        Next

        '3 - Save the File 
        Dim MySaveLocation As String = "C:\Export\crowd_1_1.dat"
        Dim FileType_save As CrowdDat.CrwdFileType = CrowdDat.CrwdFileType.FIFA_14
        m_Crowd.Save(MySaveLocation, FileType_save)

    End Sub
End Module
