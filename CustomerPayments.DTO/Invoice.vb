Public Class Invoice

    Sub MySub()
        _selfObject = New Invoice()
    End Sub

    Private _invoiceId As String
    Public Property InvoiceId() As String
        Get
            Return _invoiceId
        End Get
        Set(ByVal value As String)
            _invoiceId = value
        End Set
    End Property

    Private _invoiceAmt As String
    Public Property InvoiceAmt() As String
        Get
            Return _invoiceAmt
        End Get
        Set(ByVal value As String)
            _invoiceAmt = value
        End Set
    End Property

    Private _invoicePaid As String
    Public Property InvoicePaid() As String
        Get
            Return _invoicePaid
        End Get
        Set(ByVal value As String)
            _invoicePaid = value
        End Set
    End Property

    Private _invoiceBalance As String
    Public Property InvoiceBalance() As String
        Get
            Return _invoiceBalance
        End Get
        Set(ByVal value As String)
            _invoiceBalance = value
        End Set
    End Property

    Private _invoiceDate As String
    Public Property InvoiceDate() As String
        Get
            Return _invoiceDate
        End Get
        Set(ByVal value As String)
            _invoiceDate = value
        End Set
    End Property

    Private _invoicePaidDate As String
    Public Property InvoicePaidDate() As String
        Get
            Return _invoicePaidDate
        End Get
        Set(ByVal value As String)
            _invoicePaidDate = value
        End Set
    End Property

    Private _invoiceTempBalance As String
    Public Property TempBalance() As String
        Get
            Return _invoiceTempBalance
        End Get
        Set(ByVal value As String)
            _invoiceTempBalance = value
        End Set
    End Property

    Private _selfObject As Invoice
    Public Property SelfObj() As Invoice
        Get
            Return _selfObject
        End Get
        Set(ByVal value As Invoice)
            _selfObject = value
        End Set
    End Property

End Class
