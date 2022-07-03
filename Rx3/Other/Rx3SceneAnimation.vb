Namespace Rx3
    Public Class SceneAnimation
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.SCENE_ANIMATION
        Public Const ALIGNMENT As Integer = 16

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ByVal r As FileReader)
            MyBase.New
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)

            Me.TotalSize = r.ReadUInt32
            Me.NumLocations = r.ReadUInt32
            Me.Unknown_1 = r.ReadUInt32
            Me.Unknown_2 = r.ReadUInt32
            Me.String_1 = FifaUtil.ReadNullTerminatedString(r)
            Me.Data = r.ReadBytes(Me.TotalSize - 16 - (String_1.Length - 1))    'size - values - (string.size - nullterminator)

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.TotalSize)
            w.Write(Me.NumLocations)
            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            FifaUtil.WriteNullTerminatedString(w, Me.String_1)
            w.Write(Me.Data)

            'Padding   
            FifaUtil.WriteAlignment(w, ALIGNMENT)

            'Get & Write totalsize
            Me.TotalSize = FifaUtil.WriteSectionTotalSize(w, MyBase.SectionInfo.Offset)

        End Sub


        Private m_TotalSize As UInteger

        ''' <summary>
        ''' Total section size (ReadOnly). </summary>
        Public Property TotalSize As UInteger
            Get
                Return m_TotalSize
            End Get
            Private Set
                m_TotalSize = Value
            End Set
        End Property
        ''' <summary>
        ''' Returns the number of Locations. </summary>
        Public Property NumLocations As UInteger
        '    Get
        '        Return If(?.Count, 0)
        '    End Get
        'End Property

        Public Property Unknown_1 As UInteger   'usually 1
        Public Property Unknown_2 As UInteger   'usually 0
        Public Property String_1 As String   'ex. "tree"
        Public Property Data As Byte()   'rest of (unknown) data

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

End Namespace