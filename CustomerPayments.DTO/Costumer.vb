Public Class Costumer

    Sub MySub()
        _selfObject = New Costumer()
        _objReceipt = New Receipt()
    End Sub

    Private _costumerId As String
    Public Property CUSTNO() As String
        Get
            Return _costumerId
        End Get
        Set(ByVal value As String)
            _costumerId = value
        End Set
    End Property

    Private _objReceipt As Receipt
    Public Property ObjReceipt() As Receipt
        Get
            Return _objReceipt
        End Get
        Set(ByVal value As Receipt)
            _objReceipt = value
        End Set
    End Property

    Private _lstReceipt As List(Of Receipt)
    Public Property LstReceipt() As List(Of Receipt)
        Get
            Return _lstReceipt
        End Get
        Set(ByVal value As List(Of Receipt))
            _lstReceipt = value
        End Set
    End Property

    Private _selfObject As Costumer
    Public Property SelfObj() As Costumer
        Get
            Return _selfObject
        End Get
        Set(ByVal value As Costumer)
            _selfObject = value
        End Set
    End Property

End Class
