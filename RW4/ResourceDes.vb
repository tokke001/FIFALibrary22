Namespace Rw
    Public Class ResourceDescriptor
        'rw::ResourceDescriptor
        'Inherits BaseResourceDescriptor
        'Public Sub New(r As FileReader)
        '    MyBase.New(r)
        'End Sub
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            For i = 0 To 5 - 1
                Me.BaseResourceDescriptors(i) = New BaseResourceDescriptor(r)
            Next i
        End Sub
        Public Sub Save(ByVal w As FileWriter)
            Dim m_length As UInteger = If(Me.BaseResourceDescriptors Is Nothing, 0, CUInt(Me.BaseResourceDescriptors.Length))
            For i = 0 To m_length - 1
                Me.BaseResourceDescriptors(i).Save(w)
            Next i
        End Sub

        'Public Property Rx3Offset  'also used for "ResourcesUsed"
        '    Get
        '        Return Me.BaseResourceDescriptors(0).Size
        '    End Get
        '    Set
        '        Me.BaseResourceDescriptors(0).Size = Value
        '    End Set
        'End Property

        Public Property BaseResourceDescriptors As BaseResourceDescriptor() = New BaseResourceDescriptor(5 - 1) {}
    End Class

    Public Class BaseResourceDescriptor
        'rw::BaseResourceDescriptor
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            Me.Size = r.ReadUInt32
            Me.Alignment = r.ReadUInt32
        End Sub
        Public Sub Save(ByVal w As FileWriter)
            w.Write(Me.Size)
            w.Write(Me.Alignment)
        End Sub
        Public Property Size As UInteger       '(can be 0)
        Public Property Alignment As UInteger  '(minimum possible value is 1)
        'For empty data, the descriptor is: data size = 0; data alignment = 1.
    End Class

    Public Class TargetResource
        'rw::TargetResource
        Public Sub New()

        End Sub

        Public Sub New(ByVal r As FileReader)
            Me.Load(r)
        End Sub

        Public Sub Load(ByVal r As FileReader)
            For i = 0 To 5 - 1
                Me.BaseResources(i) = r.ReadUInt32
            Next i
        End Sub
        Public Sub Save(ByVal w As FileWriter)
            Dim m_length As UInteger = If(Me.BaseResources Is Nothing, 0, CUInt(Me.BaseResources.Length))
            For i = 0 To m_length - 1
                w.Write(Me.BaseResources(i))
            Next i
        End Sub

        Public Property BaseResources As UInteger() = New UInteger(5 - 1) {}
    End Class
End Namespace
