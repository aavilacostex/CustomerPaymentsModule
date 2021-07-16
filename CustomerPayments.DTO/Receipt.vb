Public Class Receipt

    Sub MySub()
        _selfObj = New Receipt()
        _objInvoice = New Invoice()
    End Sub

    Private _receiptId As String
    Public Property ReceiptId() As String
        Get
            Return _receiptId
        End Get
        Set(ByVal value As String)
            _receiptId = value
        End Set
    End Property

    Private _invoiceQty As String
    Public Property InvoiceQty() As String
        Get
            Return _invoiceQty
        End Get
        Set(ByVal value As String)
            _invoiceQty = value
        End Set
    End Property

    Private _objInvoice As Invoice
    Public Property ObjInvoice() As Invoice
        Get
            Return _objInvoice
        End Get
        Set(ByVal value As Invoice)
            _objInvoice = value
        End Set
    End Property

    Private _lstInvoice As List(Of Invoice)
    Public Property LstInvoice() As List(Of Invoice)
        Get
            Return _lstInvoice
        End Get
        Set(ByVal value As List(Of Invoice))
            _lstInvoice = value
        End Set
    End Property

    Private _selfObj As Receipt
    Public Property SelfObj() As Receipt
        Get
            Return _selfObj
        End Get
        Set(ByVal value As Receipt)
            _selfObj = value
        End Set
    End Property

End Class
