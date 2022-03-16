
Public Class Rx3MorphIndexed

    Public Sub New()

    End Sub

    Public Sub New(ByVal r As FileReader)
        Me.Load(r)
    End Sub

    Public Function Load(ByVal r As FileReader) As Boolean



        Me.TotalSize = r.ReadUInt32
        Me.Unknown_1 = r.ReadUInt32
        Me.Unknown_2 = r.ReadUInt32
        Me.Unknown_3 = r.ReadUInt32

        Me.Data = r.ReadBytes(Me.TotalSize - 16)    'size - values 


        Return True
    End Function

    Public Function Save(ByVal w As FileWriter) As Boolean
        Dim BaseOffset As Long = w.BaseStream.Position


        w.Write(Me.TotalSize)
        w.Write(Me.Unknown_1)
        w.Write(Me.Unknown_2)
        w.Write(Me.Unknown_3)

        w.Write(Me.Data)

        'Padding   
        FifaUtil.WriteAllignment_16(w)

        'Get & Write totalsize
        Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, BaseOffset, w.BaseStream.Position, Me.m_SwapEndian)

        Return True
    End Function


    Public Property TotalSize As UInteger
    Public Property Unknown_1 As UInteger
    Public Property Unknown_2 As UInteger
    Public Property Unknown_3 As UInteger
    Public Property Data As Byte()   'rest of (unknown) data

End Class
