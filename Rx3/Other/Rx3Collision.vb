Namespace Rx3
    Public Class Collision
        Inherits Rx3Object
        Public Const TYPE_CODE As Rx3.SectionHash = Rx3.SectionHash.COLLISION
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
            Dim m_NumSpaces As UInteger = r.ReadUInt32     '1 - NumSpaces?
            Me.Pad = r.ReadBytes(8)

            Me.CollisionName = FifaUtil.ReadNullTerminatedString(r)     'might be part of Spaces, a CollisionName for each Space ?? : no idea because NumSpaces is always 1!! 
            Me.Spaces = New CollisionSpace(m_NumSpaces - 1) {}
            For i = 0 To Me.Spaces.Length - 1
                Me.Spaces(i) = New CollisionSpace(r)
            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.TotalSize)
            w.Write(Me.NumSpaces)
            w.Write(Me.Pad)

            FifaUtil.WriteNullTerminatedString(w, Me.CollisionName)
            For i = 0 To Me.NumSpaces - 1
                Me.Spaces(i).Save(w)
            Next

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
        ''' Returns the number of Spaces (ReadOnly). </summary>
        Public ReadOnly Property NumSpaces As UInteger   'always 1?
            Get
                Return If(Spaces?.Count, 0)
            End Get
        End Property
        ''' <summary>
        ''' Empty 0-values. </summary>
        Public Property Pad As Byte() = New Byte(8 - 1) {}    'always 0, padding

        ''' <summary>
        ''' The Collision name. </summary>
        Public Property CollisionName As String

        Public Property Spaces As CollisionSpace()

        Public Overrides Function GetTypeCode() As Rx3.SectionHash
            Return TYPE_CODE
        End Function

        Public Overrides Function GetAlignment() As Integer
            Return ALIGNMENT
        End Function
    End Class

    Public Class CollisionSpace

        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Dim m_NumShapes = r.ReadUInt32

            Me.Shapes = New CollisionShape(m_NumShapes - 1) {}
            For i = 0 To Me.Shapes.Length - 1
                Me.Shapes(i) = New CollisionShape(r)
            Next

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.NumShapes)

            For i = 0 To Me.NumShapes - 1
                Me.Shapes(i).Save(w)
            Next

        End Sub

        ''' <summary>
        ''' Returns the number of Shapes (ReadOnly). </summary>
        Public ReadOnly Property NumShapes As UInteger   'always 1? , NumShapes maybe (similar to rw modelCollision)?? 
            Get
                Return If(Shapes?.Count, 0)
            End Get
        End Property

        Public Property Shapes As CollisionShape()

    End Class

    Public Class CollisionShape

        Public Sub New()
        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Unknown_1 = r.ReadUInt32     'always 0 - BoneIndex/flags ?
            Me.Unknown_2 = r.ReadUInt32     'always 3 - number of arrays in collisions / Rw.Bxd.CollisionShape.eCS_Volume / Rw.Collision.VolumeType.VOLUMETYPETRIANGLE ?
            Dim m_NumCollisions As UInteger = r.ReadUInt32             'num volumes
            Me.NumTagBits = r.ReadByte      'Caching (memory) --> calculated from NumCollisions (see function CalcNumTagBits)
            Me.Unknown_3 = r.ReadBytes(3)   'always 0 0 0    padding maybe ?

            Me.Collisions = New Matrix4x4Affine(m_NumCollisions - 1) {}
            For i = 0 To m_NumCollisions - 1
                Me.Collisions(i) = New Matrix4x4Affine(r) 'last vector 4 is empty: Matrix4x4Affine ?
            Next i

        End Sub

        Public Sub Save(ByVal w As FileWriter)

            w.Write(Me.Unknown_1)
            w.Write(Me.Unknown_2)
            w.Write(Me.NumCollisions)
            w.Write(Me.NumTagBits)
            w.Write(Me.Unknown_3)

            For i = 0 To Me.NumCollisions - 1
                Me.Collisions(i).Save(w)
            Next i

        End Sub

        Public Function CalcNumTagBits(ByVal NumCollisions As UInteger) As Integer
            Dim NumTagBits As Integer = 0
            Do
                If 2 ^ NumTagBits > NumCollisions Then
                    Return NumTagBits
                Else
                    NumTagBits += 1
                End If
            Loop

        End Function
        ''' <summary>
        ''' Unknown value, alway value 0. </summary>
        Public Property Unknown_1 As UInteger = 0  'always 0?
        ''' <summary>
        ''' Unknown value, alway value 3. </summary>
        Public Property Unknown_2 As UInteger = 3  'always 3?  'number of arrays in an array?
        ''' <summary>
        ''' Returns the number of Collisions (ReadOnly). </summary>
        Public ReadOnly Property NumCollisions As UInteger
            Get
                Return If(Collisions?.Count, 0)
            End Get
        End Property
        ''' <summary>
        ''' Number of bits in tag field of cache block. Use function CalcNumTagBits to calculate this. </summary>
        Public Property NumTagBits As Byte    ' similar too rw.collision.SimpleMappedArray.Aggregate
        ''' <summary>
        ''' Unknown values, alway value {0,0,0} . </summary>
        Public Property Unknown_3 As Byte() = New Byte(3 - 1) {}    ' always 0 0 0 : paddingg ?

        ''' <summary>
        ''' Collisions: an array of 4 (each is represented with a Vector3 struct, and an unused value ) . </summary>
        Public Property Collisions As Matrix4x4Affine() 'New List(Of List(Of Vector4))

    End Class

End Namespace