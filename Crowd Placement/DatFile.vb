Imports Microsoft.DirectX

Namespace CrowdDat
    'CrowdPlacement *.dat files
    Public Class DatFile
        Public Function Load(ByVal FileName As String, ByVal FileType As CrwdFileType) As Boolean
            Dim f As New FileStream(FileName, FileMode.Open, FileAccess.ReadWrite)
            Dim r As New FileReader(f, Endian.Little)
            Dim flag As Boolean = Me.Load(FileType, r)
            f.Close()
            r.Close()
            Return flag
        End Function
        Public Function Load(ByVal fifaFile As FifaFile, ByVal FileType As CrwdFileType) As Boolean
            If fifaFile.IsCompressed Then
                fifaFile.Decompress()
            End If
            Dim r As FileReader = fifaFile.GetReader
            r.Endianness = Endian.Little
            Dim flag As Boolean = Me.Load(FileType, r)
            fifaFile.ReleaseReader(r)

            Return flag
        End Function
        Public Overridable Function Load(ByVal FileType As CrwdFileType, ByVal r As FileReader) As Boolean
            Me.Signature = New String(r.ReadChars(4))
            If Me.Signature <> "CRWD" Then
                Return False
            End If

            Me.Unknown = r.ReadBytes(2)
            Me.NumSeats = r.ReadUInt32

            Select Case Me.Unknown(0)
                'Case 3 'can be FIFAWC06 (wich is little different) or FIFA11-14
                    'FileType = CrwdFileType.FIFA_14
                Case 4
                    FileType = CrwdFileType.FIFA_WC14
                Case 5
                    FileType = CrwdFileType.FIFA_15
            End Select

            Me.CrowdData = New Crowd(Me.NumSeats - 1) {}
            For i = 0 To Me.NumSeats - 1
                Me.CrowdData(i) = New Crowd(FileType, r)
            Next

            Return True
        End Function

        Public Overridable Function Save(ByVal FileType As CrwdFileType, ByVal w As FileWriter) As Boolean
            Me.NumSeats = Me.CrowdData.Length

            w.Write(Me.Signature.ToCharArray)
            w.Write(Me.Unknown)
            w.Write(Me.NumSeats)

            Select Case Me.Unknown(0)
                'Case 3 'can be FIFAWC06 (wich is little different) or FIFA11-14
                    'FileType = CrwdFileType.FIFA_14
                Case 4
                    FileType = CrwdFileType.FIFA_WC14
                Case 5
                    FileType = CrwdFileType.FIFA_15
            End Select

            For i = 0 To Me.NumSeats - 1
                Me.CrowdData(i).Save(FileType, w)
            Next

            Return True
        End Function

        Public Function Save(ByVal FileName As String, ByVal FileType As CrwdFileType) As Boolean
            Dim output As New FileStream(FileName, FileMode.Create, FileAccess.ReadWrite)
            Dim w As New FileWriter(output, Endian.Little)
            Dim flag As Boolean = Me.Save(FileType, w)
            output.Close()
            w.Close()

            Return flag
        End Function

        'Public Function ToTxt() As String

        '    Return ""
        'End Function


        Public Property Signature As String = "CRWD"
        Public Property Unknown As Byte() = New Byte(2 - 1) {}  'version maybe ? --> "3 , 1" = FIFA 11-14 , "5 , 1" = FIFA 15 
        Public Property NumSeats As UInteger
        Public Property CrowdData As Crowd()

    End Class

    Public Class Crowd
        Public Sub New()
        End Sub

        Public Sub New(ByVal FileType As CrwdFileType, ByVal r As FileReader)
            Me.Load(FileType, r)
        End Sub

        Public Function Load(ByVal FileType As CrwdFileType, ByVal r As FileReader) As Boolean

            Me.Verts = r.ReadVector3
            Me.ZRot = r.ReadSingle      'F22: Orientation
            Me.Color = r.ReadBytes(3)   'F22: SeatColor

            If FileType = CrwdFileType.FIFA_14 Then
                Me.Status = New CrowdStatus(FileType, r)
                Me.Unknown = New CrowdUnknown3(r)
            ElseIf FileType = CrwdFileType.FIFA_WC14 Then
                Me.Status = r.ReadBytes(4)  'New CrowdStatus(FileType, r) --> unknown values
                Me.Unknown = New CrowdUnknown4(r)
            ElseIf FileType = CrwdFileType.FIFA_15 Then
                Me.Status = New CrowdStatus(FileType, r)
                Me.Unknown = r.ReadBytes(9)
            End If

            Return True
        End Function

        Public Function Save(ByVal FileType As CrwdFileType, ByVal w As FileWriter) As Boolean
            'Dim Length_unknown As Integer

            'If FileType = CrwdFileType.FIFA_14 Then
            '    Length_unknown = 9
            'Else
            '    Length_unknown = 21
            'End If

            w.Write(Me.Verts)
            w.Write(Me.ZRot)
            w.Write(Me.Color)

            If Me.Status.GetType = GetType(Byte()) Then
                w.Write(CType(Me.Status, Byte()))
            ElseIf Me.Status.GetType = GetType(CrowdStatus) Then
                CType(Me.Status, CrowdStatus).Save(FileType, w)
            End If

            If Me.Unknown.GetType = GetType(Byte()) Then
                w.Write(CType(Me.Unknown, Byte()))
            ElseIf Me.Unknown.GetType = GetType(CrowdUnknown3) Then
                CType(Me.Unknown, CrowdUnknown3).Save(w)
            ElseIf Me.Unknown.GetType = GetType(CrowdUnknown4) Then
                CType(Me.Unknown, CrowdUnknown4).Save(w)
            End If

            Return True
        End Function

        Public Property Verts As Vector3
        Public Property ZRot As Single
        Public Property Color As Byte() = New Byte(3 - 1) {}
        Public Property Status As Object
        Public Property Unknown As Object   'FIFA 11-14, i think: 1 byte, 4 floats, 2 bytes

    End Class

    Public Class CrowdStatus
        Public Sub New()
        End Sub

        Public Sub New(ByVal FileType As CrwdFileType, ByVal r As FileReader)
            Me.Load(FileType, r)
        End Sub

        Public Sub Load(ByVal FileType As CrwdFileType, ByVal r As FileReader)
            If FileType = CrwdFileType.FIFA_14 Then
                Me.Load3(r)
            Else    'if FileType = CrwdFileType.FIFA_15
                Me.Load5(r)
            End If

            'Status(0) = 0     'number of awaycrowd: 0 = all home, 255 = all away
            'Status(1) = 255       '
            'Status(2) = 255     'crowd size
            'Status(3) = 255

            'Select Case Status(0)
            '    'Case 0 To 255 '83, 93, 97, 99, 106, 108, 111, 113, 114, 119, 120, 121, 122, 123, 124, 125, 128, 131,243, ...    'F14
            '    'Case 0 To 255 '239, 240, 241    'F11
            '    'Case 0 To 255 '0, 6, 22, 33, 200, 43, 1, 14, 27  'wc14
            '    Case 0 To 255 '0, 2, 14, 23, 33, 45, 54, 66, 128, 191, 255    'F15
            '    Case Else
            'End Select

            'Select Case Status(1)
            '    'Case 0, 1, 2, 3, 4, 5     'F14
            '    'Case 0, 2, 255            'F11, FO3_old
            '    'Case 0, 1, 2, 255          'WC10   --> 1: crowd_213_1, crowd_214_1
            '    'Case 0, 255                'UEFA euro08
            '    'Case 0, 2, 255            'F12
            '    'Case 0, 255                'F13
            '    'Case 0 To 255 '223, 228, 233, 237, 242, 244, 246, 248, 249, 251, 253, 255  'wc14
            '    Case 0 To 255 '0, 75, 126, 127, 128, 163, 255    'F15
            '    Case Else
            'End Select

            'Select Case Status(2)
            '    'Case 0 To 255 '30,167, 170, 173, 177, 178, 181, 183, 186, 189, 191, 193, 197, 199, 200, 206, 208, 211, 217, 218, 219, 222, 224, 226, 227, 228, 230, 232, 233, 235, 238, 240, 241, 243, 244, 247, 248, 249, 250, 251, 253, ...    'F14
            '    Case 0 To 255 '237, 238, 239, 240, 241
            '        'Case 0 To 255 '0, 28, 31, 33, 35, 38, 79, 80, 84, 91, 93, 96, 98, 100, 105   'wc14
            '        'Case 1, 2, 3, 4, 5  'F15
            '        'Case 1, 2, 3, 4     'F22 switch
            '    Case Else
            'End Select

            'Select Case Status(3)
            '    'Case 127    'F14
            '    'Case 127   'F11
            '    'Case 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 127, 255 'WC10 --> 255: crowd_213_1,crowd_214_1 , 0 : crowd_214_1
            '    'Case 0 To 255 '10, 11, 13, 14, 16, 17, 18, 20, 21, 24, 26, 105, 223, 225, 226, 228, 230, 232   'wc14
            '    Case 0 To 255 '98, 109, 114, 119, 124, 131, 255    'F15
            '    Case Else
            'End Select
        End Sub

        Private Sub Load3(ByVal r As FileReader)
            Me.NumAwayCrowd = r.ReadByte
            Me.Value_2 = r.ReadByte
            Me.CrowdSize = r.ReadByte
            Me.Value_4 = r.ReadByte
        End Sub

        Private Sub Load4(ByVal r As FileReader)
            Me.NumAwayCrowd = r.ReadByte
            Me.Value_2 = r.ReadByte     'maybe, not sure ?
            r.ReadByte()
            r.ReadByte()
            Me.Value_4 = r.ReadByte     'maybe, not sure ?
            Me.CrowdSize = r.ReadByte
        End Sub

        Private Sub Load5(ByVal r As FileReader)
            Me.NumAwayCrowd = r.ReadByte
            Me.Value_4 = r.ReadByte
            Me.Value_2 = r.ReadByte
            Me.CrowdSize = r.ReadByte
        End Sub

        Public Sub Save(ByVal FileType As CrwdFileType, ByVal w As FileWriter)
            If FileType = CrwdFileType.FIFA_14 Then
                Me.Save3(w)
            Else
                Me.Save5(w)
            End If
        End Sub

        Private Sub Save3(ByVal w As FileWriter)
            w.Write(Me.NumAwayCrowd)
            w.Write(Me.Value_2)
            w.Write(Me.CrowdSize)
            w.Write(Me.Value_4)
        End Sub

        Private Sub Save5(ByVal w As FileWriter)
            w.Write(Me.NumAwayCrowd)
            w.Write(Me.Value_4)
            w.Write(Me.Value_2)
            w.Write(Me.CrowdSize)
        End Sub

        Public Property NumAwayCrowd As Byte    'number of awaycrowd: 0 = all home, 255 = all away
        Public Property Value_2 As Byte         'F14: 0, 1, 2, 3, 4, 5 | F11: 0, 2, 255
        Public Property CrowdSize As Byte       'Size of crowd: 0 = near no crowd, 255 = full packed
        Public Property Value_4 As Byte         'F11/14: always 127

    End Class

    Public Class CrowdUnknown3
        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Function Load(ByVal r As FileReader) As Boolean

            Me.Value_1 = r.ReadByte
            Me.Value_2 = New Single(4 - 1) {}
            For i = 0 To 4 - 1
                Me.Value_2(i) = r.ReadSingle
                'If Me.Value_2(i) < 0 Or Me.Value_2(i) > 1 Then
                '    MsgBox("test")
                'End If
            Next

            Me.Value_3 = r.ReadBytes(2)

            'If Value_1 <> 51 Then
            '    MsgBox("test")
            'End If


            'If Value_3(0) <> 0 Or Value_3(1) <> 0 Then
            '    MsgBox("test")
            'End If
            Return True
        End Function

        Public Function Save(ByVal w As FileWriter) As Boolean

            w.Write(Me.Value_1)
            For i = 0 To Me.Value_2.Length - 1
                w.Write(Me.Value_2(i))
            Next i
            w.Write(Me.Value_3)

            Return True
        End Function

        Public Property Value_1 As Byte                             'always 51
        Public Property Value_2 As Single() = New Single(4 - 1) {}  '4 float Values: ussually between 0 - 1, but may also be negative or 2  --> tangents, normals, scaling, ... ??
        Public Property Value_3 As Byte() = New Byte(2 - 1) {}      'always 0 , 0 --> padding maybe, or unused ?

    End Class

    Public Class CrowdUnknown4
        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Function Load(ByVal r As FileReader) As Boolean

            Me.Value_1 = r.ReadBytes(2)
            Me.Value_2 = New Single(4 - 1) {}
            For i = 0 To 4 - 1
                Me.Value_2(i) = r.ReadSingle
                'If Me.Value_2(i) < 0 Or Me.Value_2(i) > 1 Then
                '    MsgBox("test")
                'End If
            Next


            'Select Case Value_1(0)
            '    Case 0 To 255 '0, 151, 255, 128, 135, 248, 14, 119, 163, 80, 179, 11, 73, 13, 8, 100, 235, 42, 209
            '    Case Else
            'End Select

            'Select Case Value_1(1)
            '    Case 0 To 255 '14, 255, 35, 56, 240, 70, 84, 106
            '    Case Else
            'End Select

            'Select Case Value_2(0)
            '    Case 0
            '    Case Else
            'End Select
            'Select Case Value_2(1)
            '    Case 0
            '    Case Else
            'End Select
            'Select Case Value_2(2)
            '    Case 0
            '    Case Else
            'End Select
            'Select Case Value_2(3)
            '    Case 1
            '    Case Else
            'End Select

            Return True
        End Function

        Public Function Save(ByVal w As FileWriter) As Boolean

            w.Write(Me.Value_1)
            For i = 0 To Me.Value_2.Length - 1
                w.Write(Me.Value_2(i))
            Next i

            Return True
        End Function

        Public Property Value_1 As Byte() = New Byte(2 - 1) {}
        Public Property Value_2 As Single() = New Single(4 - 1) {}

    End Class
    Public Enum CrwdFileType As Byte
        FIFA_WC06
        FIFA_14 = 3     'crowd.dat, compatible with FIFA 11
        FIFA_WC14 = 4
        FIFA_15 = 5
    End Enum
End Namespace