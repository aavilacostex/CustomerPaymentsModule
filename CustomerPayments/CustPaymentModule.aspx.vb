Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports ClosedXML.Excel
Imports CustomerPayments.DTO
Imports Newtonsoft.Json

Public Class CustPaymentModule
    Inherits System.Web.UI.Page

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing

    Private Shared eventLog1 As EventLog = New EventLog("Purchasing.Log", GetComputerName(), "Purchasing.App")
    Private Shared ReadOnly Log As log4net.ILog = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType)

    Dim objLog = New Logs()

#Region "Page Load"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim sel As Integer = -1
        Dim fullData As Boolean = False
        Dim exMessage As String = " "
        'Session("userid") = Nothing 'forcing session timeout
        Dim url As String = Nothing
        Try

            If Session("userid") Is Nothing Then
                url = String.Format("Login.aspx?data={0}", "Session Expired!")
                Response.Redirect(url, False)
            Else
                Dim welcomeMsg = ConfigurationManager.AppSettings("UserWelcome")
                lblUserLogged.Text = String.Format(welcomeMsg, Session("username").ToString().Trim(), Session("userid").ToString().Trim())
                hdWelcomeMess.Value = lblUserLogged.Text
            End If

            If Not IsPostBack Then

                Dim flag = GetAccessByUsers(sel, fullData)

                'condition to allow redirection when session expired
                'flag = If(String.IsNullOrEmpty(url), flag, True)
                'Session("userid") = "AAVILA"
                'Session("username") = "Alexia Avila"
                'flag = True
                If Not flag Then
                    If sel = 0 Then
                        Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User: " + usr, " User is not authorized to access to Customer Payment Module. Time: " + DateTime.Now.ToString())
                        Response.Redirect("http://svrwebapps.costex.com/BaseProject/default.aspx", False)
                    Else
                        Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, Nothing, "There is not an user detected tryng to access to Customer Payment Module. Time: " + DateTime.Now.ToString())
                        Response.Redirect("http://www.costex.com", False)
                    End If
                Else
                    Log.Info("Starting Customer Payments")

                    'If Session("userid") IsNot Nothing Then
                    '    Dim welcomeMsg = ConfigurationManager.AppSettings("UserWelcome")
                    '    lblUserLogged.Text = String.Format(welcomeMsg, Session("username").ToString().Trim(), Session("userid").ToString().Trim())
                    '    hdWelcomeMess.Value = lblUserLogged.Text
                    'Else
                    '    If String.IsNullOrEmpty(url) Then
                    '        FormsAuthentication.RedirectToLoginPage()
                    '    End If
                    'End If

                    Dim dsResult As DataSet = New DataSet()
                    fill_Page_Size(ddlPageSize)
                    Session("PageSize") = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("PageSize")), ConfigurationManager.AppSettings("PageSize"), "1000")
                    Session("PageAmounts") = If(Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("PageAmounts")), ConfigurationManager.AppSettings("PageAmounts"), "10")
                    Session("currentPage") = 1
                    Session("SortedField") = Nothing

                    cleanForm(filters)
                    loadData(Nothing, Nothing, False, True)
                    'GetCustPaymentsData(dsResult)
                    'loadData(dsResult)
                    'setDefaultValues(dsResult)
                    'SendMessage("Please select the filter criteria to search the payments for specific customer.", messageType.info)
                End If

            Else
                Dim controlName As String = Page.Request.Params("__EVENTTARGET")
                If LCase(controlName).Contains("updatepnl") Or LCase(controlName).Contains("lnkexpander") Or LCase(controlName).Contains("rd") Or True Then
                    Dim dsOut = DirectCast(Session("CustPaymentData"), DataSet)
                    loadData(dsOut, Nothing, False, True)
                    setDefaultValues(dsOut)
                End If
            End If
        Catch ex As Exception
            Log.Info("Raise an exception")
            writeComputerEventLog(ex.Message)

            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "An Exception occurs: " + ex.Message + " for the user: " + usr, " at time: " + DateTime.Now.ToString())
        End Try

    End Sub

#End Region

#Region "Main Load of Data"

    Private Function getSelectedExtraFilter() As List(Of String)
        Dim prepQuery As List(Of String) = New List(Of String)()
        Dim prepValue As String = Nothing
        Try
            If Not String.IsNullOrEmpty(txtReceiptNo.Text.Trim()) And Not String.IsNullOrEmpty(txtInvoiceNo.Text.Trim()) Then
                Dim prepValue1 As String = Nothing

                prepValue = " a.crgcka = "
                prepQuery.Add(prepValue)
                prepQuery.Add(txtReceiptNo.Text.Trim())

                prepValue1 = " a.crgrf# = "
                prepQuery.Add(prepValue1)
                prepQuery.Add(txtInvoiceNo.Text.Trim())
            ElseIf Not String.IsNullOrEmpty(txtReceiptNo.Text.Trim()) Then
                prepValue = " a.crgcka = "
                prepQuery.Add(prepValue)
                prepQuery.Add(txtReceiptNo.Text.Trim())
            Else
                prepValue = " a.crgrf# = "
                prepQuery.Add(prepValue)
                prepQuery.Add(txtInvoiceNo.Text.Trim())
            End If
            Return prepQuery
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Sub EnumDrwCollectToDt(dt As DataTable, erc As EnumerableRowCollection(Of DataRow), ByRef dtOut As DataTable)
        Dim exMessage As String = Nothing
        Try
            Dim dtResult As DataTable = dt.Clone()
            For Each dw As DataRow In erc
                dtResult.ImportRow(dw)
            Next
            If dtResult IsNot Nothing Then
                If dtResult.Rows.Count > 0 Then
                    dtOut = dtResult

                    'fillObj(dtResult)

                    'loadData(Nothing, dtResult)
                End If
            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Sub AddFiltersToPrincipal(dsLoad As DataSet, ByRef proc As Boolean, Optional ByRef dtOut As DataTable = Nothing)
        dtOut = New DataTable()
        Dim exMessage As String = Nothing
        Try
            Dim receipt = txtReceiptNo.Text.Trim()
            Dim invoice = txtInvoiceNo.Text.Trim()

            If dsLoad IsNot Nothing Then
                If dsLoad.Tables(0).Rows.Count > 0 Then
                    If Not String.IsNullOrEmpty(receipt) Then
                        Dim lstReceipt = dsLoad.Tables(0).AsEnumerable().Where(Function(item) item.Item("receipt").ToString().Trim().Equals(receipt, StringComparison.InvariantCultureIgnoreCase))
                        If lstReceipt.Count > 0 Then
                            If Not String.IsNullOrEmpty(invoice) Then
                                Dim lstInvoice = lstReceipt.Where(Function(item) item.Item("invoice").ToString().Trim().Equals(invoice, StringComparison.InvariantCultureIgnoreCase))
                                If lstInvoice.Count > 0 Then
                                    EnumDrwCollectToDt(dsLoad.Tables(0), lstInvoice, dtOut)
                                    proc = True
                                Else
                                    'no invoice data in the collection
                                End If
                            Else
                                EnumDrwCollectToDt(dsLoad.Tables(0), lstReceipt, dtOut)
                                proc = True
                            End If
                        Else
                            'no receipt data in the collection
                            If Not String.IsNullOrEmpty(invoice) Then
                                Dim lstInvoice = dsLoad.Tables(0).AsEnumerable().Where(Function(item) item.Item("invoice").ToString().Trim().Equals(invoice, StringComparison.InvariantCultureIgnoreCase))
                                If lstInvoice.Count > 0 Then
                                    EnumDrwCollectToDt(dsLoad.Tables(0), lstInvoice, dtOut)
                                    proc = True
                                Else
                                    'no receipt or invoice data in the collection
                                End If
                            Else
                                'no receipt data in the collection
                            End If
                        End If
                    Else
                        'not searching by receipt
                        If Not String.IsNullOrEmpty(invoice) Then
                            Dim lstInvoice = dsLoad.Tables(0).AsEnumerable().Where(Function(item) item.Item("invoice").ToString().Trim().Equals(invoice, StringComparison.InvariantCultureIgnoreCase))
                            If lstInvoice.Count > 0 Then
                                EnumDrwCollectToDt(dsLoad.Tables(0), lstInvoice, dtOut)
                                proc = True
                            Else
                                'no invoice data in the collection
                            End If
                        Else
                            'no searching by receipt or invoice 
                        End If
                    End If
                Else
                    'error in data load
                End If
            Else
                'data is nothing
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Function GetCustPaymentsDataPartial(Optional ByRef dsResult As DataSet = Nothing, Optional flag As Boolean = False, Optional dec As Boolean = False, Optional ByRef objCostumer As Costumer = Nothing) As Integer
        dsResult = New DataSet()
        Dim dsExtra = New DataSet()
        Dim result As Integer = -1
        Dim messageOut As String = Nothing
        Dim lstDates = New List(Of String)()
        Dim proc As Boolean = False
        Dim dtOut As DataTable = New DataTable()
        Dim exMessage As String = Nothing
        objCostumer = New Costumer()
        Try
            Dim cliNo = txtvendor.Text.Trim()
            Dim startDate = txtDate.Text
            Dim endDate = txtDateTo.Text

            lstDates.Add(startDate)
            lstDates.Add(endDate)

            Using objBL As CustomerPayments.BL.CustPayment = New CustomerPayments.BL.CustPayment()

                If Not dec Then
                    result = objBL.GetCustPaymentDataByCliNo(cliNo, lstDates, dsResult, messageOut)
                Else
                    Dim lstSelection = getSelectedExtraFilter()
                    result = objBL.GetCustPaymentDataBySelection(dsResult, lstSelection, messageOut)
                End If

                If result > 0 Then
                    If dsResult IsNot Nothing Then
                        If dsResult.Tables(0).Rows.Count > 0 Then
                            Session("CustPaymentData") = dsResult
                            If flag Then
                                AddFiltersToPrincipal(dsResult, proc, dtOut)
                                If dtOut IsNot Nothing Then
                                    If dtOut.Rows.Count > 0 Then
                                        Dim dsNew = New DataSet()
                                        dsNew.Tables.Add(dtOut)
                                        dsResult = dsNew
                                        Session("CustPaymentData") = dsResult
                                    End If
                                End If
                            End If
                            'If Not proc Then

                            objCostumer = fillObj(dsResult.Tables(0))
                            'Dim dtt = ObjectToDataTable(objGet)
                            'create a datatable from this object
                            'loadData(dsResult)
                            'End If

                            Return result
                            'Session("CustPaymentData") = dsResult
                            'Session("CustPaymentBck") = dsResult
                        End If
                    End If
                Else
                    'loadData(Nothing)
                    Return result
                End If
            End Using

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
            Return result
        End Try
    End Function

    Public Function GetCustPaymentsData(Optional ByRef dsResult As DataSet = Nothing, Optional dsLoad As DataSet = Nothing, Optional userSql As String = Nothing) As Integer
        Dim exMessage As String = Nothing
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim messageOut As String = Nothing
        Dim lstDates = New List(Of String)()

        Try
            Dim cliNo = txtvendor.Text.Trim()
            Dim startDate = txtDate.Text
            Dim endDate = txtDateTo.Text
            Dim dtOut As DateTime = New DateTime()
            Dim dtOut1 As DateTime = New DateTime()

            Dim Culture = CultureInfo.CreateSpecificCulture("en-US")
            Dim styles = DateTimeStyles.AssumeLocal

            Dim dtValue = startDate.Split(" ")(0).Trim()
            Dim dtValue1 = endDate.Split(" ")(0).Trim()
            Dim dt2 = DateTime.TryParseExact(dtValue, "MM/dd/yyyy", Culture, styles, dtOut)
            Dim dt3 = DateTime.TryParseExact(dtValue1, "MM/dd/yyyy", Culture, styles, dtOut1)
            Dim strStartDate = dtOut.ToString().Split(" ")(0).Trim()
            Dim strEndDate = dtOut1.ToString().Split(" ")(0).Trim()
            'Dim curDate = Now.ToString()
            'Dim curDtValue = curDate.Split(" ")(0).Trim()

            lstDates.Add(strStartDate)
            lstDates.Add(strEndDate)
            Using objBL As CustomerPayments.BL.CustPayment = New CustomerPayments.BL.CustPayment()
                result = objBL.GetCustPaymentDataByCliNo(cliNo, lstDates, dsResult, messageOut)
                If result > 0 Then
                    If dsResult IsNot Nothing Then
                        If dsResult.Tables(0).Rows.Count > 0 Then
                            Session("CustPaymentData") = dsResult
                            Session("CustPaymentBck") = dsResult
                        End If
                    End If
                End If
            End Using
            Return result
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
            Return result
        End Try
    End Function

#End Region

#Region "GridView"

    Protected Sub grvCustPayment_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grvCustPayment.PageIndexChanging
        Dim exMessage As String = " "
        Dim dsSetDataSource = New DataSet()
        Try
            grvCustPayment.PageIndex = e.NewPageIndex
            'Dim ds = DirectCast(Session("CustPaymentData"), DataSet)
            'GetCustPaymentsDataPartial(dsSetDataSource, True)
            dsSetDataSource = DirectCast(Session("CustPaymentData"), DataSet)
            If dsSetDataSource IsNot Nothing Then
                loadData(dsSetDataSource)
                setDefaultValues(dsSetDataSource)
            Else
                btnSearchPayments_Click(Nothing, Nothing)
            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub grvCustPayment_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles grvCustPayment.RowCommand
        Dim exMessage As String = Nothing
        Try
            If e.CommandName = "ShowInvoice" Then
                Dim row As GridViewRow = DirectCast(DirectCast((e.CommandSource), LinkButton).Parent.Parent, GridViewRow)
                Dim dataReceipt = row.Cells(1).Text.Trim()
                Dim dataInvoice = row.Cells(2)
                Dim myLabelInv As Label = DirectCast(dataInvoice.FindControl("txtInvoiceNo"), Label)
                txtInvoiceNo.Text = Trim(myLabelInv.Text)
                txtReceiptNo.Text = dataReceipt
            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub grvCustPayment_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grvCustPayment.RowDataBound
        Dim exMessage As String = Nothing
        Dim lstValues = New List(Of String)()

        Try
            If e.Row.RowType = DataControlRowType.DataRow Then

                'Dim sessionFlag = If(Session("SumarizeFlag") IsNot Nothing, DirectCast(Session("SumarizeFlag"), Boolean), False)
                'If Not sessionFlag Then
                '    Dim dataInvoice = e.Row.Cells(2)
                '    Dim myLabelInv As Label = DirectCast(dataInvoice.FindControl("txtInvoiceNo"), Label)
                '    Dim invoice = myLabelInv.Text.Trim().ToUpper()
                '    Dim balance = e.Row.Cells(7).Text.ToString()
                '    Dim payment = e.Row.Cells(5).Text.ToString()

                '    Dim strResult = CheckIfInvoiceIsDuplicated(invoice, balance, payment)
                '    If Not String.IsNullOrEmpty(strResult) Then
                '        e.Row.Cells(7).Text = strResult
                '    End If
                'End If

            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub grvCustPayment_Sorting(sender As Object, e As GridViewSortEventArgs) Handles grvCustPayment.Sorting
        Dim dtw As DataView = Nothing
        Dim newDt As DataTable = New DataTable()
        Dim exMessage As String = Nothing
        Dim direction As String = Nothing
        Try
            Dim dt As DataTable = DirectCast(grvCustPayment.DataSource, DataTable)
            Session("SortedField") = e.SortExpression
            If dt IsNot Nothing Then
                dtw = New DataView(dt)
                direction = DirectCast(Session("sortDirection"), String)
                dtw.Sort = e.SortExpression + " " + SetSortDirection(direction)

                newDt = dtw.ToTable()
                Dim ds As DataSet = New DataSet()
                ds.Tables.Add(newDt)
                'Session("CustPaymentData") = ds

                loadData(ds)
                'grvCustPayment.DataSource = ds
                'grvCustPayment.DataBind()

            Else
                Dim ds As DataSet = New DataSet()
                GetCustPaymentsDataPartial(ds, True)
                'ds = DirectCast(Session("CustPaymentData"), DataSet)
                If ds IsNot Nothing Then

                    dtw = New DataView(ds.Tables(0))
                    direction = DirectCast(Session("sortDirection"), String)
                    dtw.Sort = e.SortExpression + " " + SetSortDirection(direction)

                    newDt = dtw.ToTable()
                    ds.Tables.RemoveAt(0)
                    ds.Tables.Add(newDt)
                    'Session("CustPaymentData") = ds
                    loadData(ds)
                    'grvCustPayment.DataSource = ds
                    'grvCustPayment.DataBind()

                End If

            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub grvCustPayment_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Dim exMessage As String = Nothing
        Try

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub

#End Region

#Region "DropDownList"

    Protected Sub fill_Page_Size(dwlControl As DropDownList)

        Dim ListItem As ListItem = New ListItem()
        'dwlControl.Items.Add(New ListItem("Select a Projet Status", "-1"))
        dwlControl.Items.Add(New WebControls.ListItem("10", "10"))
        dwlControl.Items.Add(New WebControls.ListItem("25", "25"))
        dwlControl.Items.Add(New WebControls.ListItem("50", "50"))
        dwlControl.Items.Add(New WebControls.ListItem("100", "100"))
        dwlControl.Items.Add(New WebControls.ListItem("All", "All"))

    End Sub

    Protected Sub ddlPageSize_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim intValue As Integer
        Dim ds = New DataSet()
        Dim exMessage As String = Nothing
        Try
            If Integer.TryParse(ddlPageSize.SelectedValue, intValue) Then
                grvCustPayment.AllowPaging = True
                grvCustPayment.PageSize = If(CInt(ddlPageSize.SelectedValue) > 10, CInt(ddlPageSize.SelectedValue), 10)
                Session("GrvPageSize") = grvCustPayment.PageSize

                Dim CurrentPage = (DirectCast(Session("currentPage"), Integer))
                Session("PageAmounts") = (grvCustPayment.PageSize * CurrentPage).ToString()

                '    'Dim ItemConttt = (DirectCast(Session("ItemCounts"), Integer))

                '    'Session("PageAmountsDdl") = grvWishList.PageSize

                '    Dim dsLoad = DirectCast(Session("WishListData"), DataSet)
                Dim rsResult = GetCustPaymentsDataPartial(ds, True)
                If ds IsNot Nothing Then
                    If ds.Tables(0).Rows.Count > 0 Then
                        loadData(ds)
                    End If
                Else
                    loadData(Nothing)
                End If

            Else
                '    loadData(Nothing)
            End If
            'updatePagerSettings(grvWishList)
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try

    End Sub

#End Region

#Region "TextBoxes"

    Protected Sub txtInvoiceNo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtInvoiceNo.TextChanged
        Dim exMessage As String = Nothing
        Try
            btnSearchPayments.Enabled = True
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub txtReceiptNo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtReceiptNo.TextChanged
        Dim exMessage As String = Nothing
        Try
            btnSearchPayments.Enabled = True
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

#End Region

#Region "Buttons"

    Protected Sub btnSearchPayments_Click(sender As Object, e As EventArgs) Handles btnPayments1.Click
        Dim exMessage As String = Nothing
        Dim rsReturn As Integer = -1
        Dim dsResult As DataSet = New DataSet()
        Dim objCostumer As Costumer = New Costumer()
        Try

            Session("SortedField") = Nothing
            Dim vendorNo = txtvendor.Text.Trim()
            Dim dtValueStart = txtDate.Text.Trim()
            Dim dtValueEnd = txtDateTo.Text.Trim()

            If Not String.IsNullOrEmpty(vendorNo) And Not String.IsNullOrEmpty(dtValueStart) And Not String.IsNullOrEmpty(dtValueEnd) Then

                If String.IsNullOrEmpty(txtReceiptNo.Text.Trim()) And String.IsNullOrEmpty(txtInvoiceNo.Text.Trim()) Then
                    rsReturn = GetCustPaymentsData(dsResult)
                    If rsReturn > 0 Then

                        Dim objGet = fillObj(dsResult.Tables(0))

                        Dim dt As DataTable = New DataTable()
                        dt = dsResult.Tables(0).Clone()
                        Dim dss = GenerateDataSetFromObject(objGet, dt)

                        loadData(dss)
                        setDefaultValues(dss)
                        Session("CustPaymentData") = dss
                        'btnSearchPayments.Enabled = False
                    Else
                        Dim message = "There is no data for the current selection Filters."
                        SendMessage(message, messageType.warning)
                    End If
                Else
                    rsReturn = GetCustPaymentsDataPartial(dsResult, True, False, objCostumer)
                    If rsReturn > 0 Then

                        Dim dt As DataTable = New DataTable()
                        dt = dsResult.Tables(0).Clone()
                        Dim dss = GenerateDataSetFromObject(objCostumer, dt)

                        loadData(dss)
                        setDefaultValues(dss)
                        Session("CustPaymentData") = dss
                        'btnSearchPayments.Enabled = False
                    Else
                        Dim message = "Please check the value for Recepit Number or Invoice Number. It has a not valid number."
                        SendMessage(message, messageType.warning)
                    End If
                End If

            Else
                If Not String.IsNullOrEmpty(txtReceiptNo.Text.Trim()) Or Not String.IsNullOrEmpty(txtInvoiceNo.Text.Trim()) Then
                    rsReturn = GetCustPaymentsDataPartial(dsResult, False, True, objCostumer)
                    If rsReturn > 0 Then

                        Dim dt As DataTable = New DataTable()
                        dt = dsResult.Tables(0).Clone()
                        Dim dss = GenerateDataSetFromObject(objCostumer, dt)

                        loadData(dss)
                        setDefaultValues(dss)
                        Session("CustPaymentData") = dss
                        'btnSearchPayments.Enabled = False
                    Else
                        Dim message = "Please check the value for Recepit Number or Invoice Number. It has a not valid number."
                        SendMessage(message, messageType.warning)
                    End If
                Else
                    loadData(Nothing)
                    SendMessage("Please select the Customer Number and Start Date in order to get the payments.", messageType.warning)
                End If
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub lnkReceiptNo_Click()
        Dim dsData = New DataSet()
        Dim rsResult As Integer = -1
        Dim exMessage As String = Nothing
        Dim objCostumer As Costumer = New Costumer()
        Try
            Dim receipt = txtReceiptNo.Text.Trim()
            Session("SortedField") = Nothing

            rsResult = GetCustPaymentsDataPartial(dsData, True, False, objCostumer)

            Dim dt As DataTable = New DataTable()
            dt = dsData.Tables(0).Clone()
            Dim dss = GenerateDataSetFromObject(objCostumer, dt)

            If dss IsNot Nothing Then
                If dss.Tables(0).Rows.Count > 0 Then
                    Dim lstReceipt = dss.Tables(0).AsEnumerable().Where(Function(item) item.Item("receipt").ToString().Trim().Equals(receipt, StringComparison.InvariantCultureIgnoreCase))
                    If lstReceipt.Count > 0 Then

                        'method to import from enumerabledataroe collectionto datatable
                        Dim dtResult As DataTable = dss.Tables(0).Clone()
                        For Each dw As DataRow In lstReceipt
                            dtResult.ImportRow(dw)
                        Next
                        If dtResult IsNot Nothing Then
                            If dtResult.Rows.Count > 0 Then
                                loadData(Nothing, dtResult)
                            End If
                        End If
                    End If
                End If
            Else
                Dim message = "The Receipt Number is not valid."
                SendMessage(message, messageType.warning)
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub lnkInvoiceNo_Click()
        Dim dsData = New DataSet()
        Dim rsResult As Integer = -1
        Dim exMessage As String = Nothing
        Dim objCostumer As Costumer = New Costumer()
        Try
            Dim invoice = txtInvoiceNo.Text.Trim()
            Session("SortedField") = Nothing

            rsResult = GetCustPaymentsDataPartial(dsData, True, False, objCostumer)

            Dim dt As DataTable = New DataTable()
            dt = dsData.Tables(0).Clone()
            Dim dss = GenerateDataSetFromObject(objCostumer, dt)

            If dss IsNot Nothing Then
                If dss.Tables(0).Rows.Count > 0 Then
                    Dim lstInvoice = dss.Tables(0).AsEnumerable().Where(Function(item) item.Item("invoice").ToString().Trim().Equals(invoice, StringComparison.InvariantCultureIgnoreCase))
                    If lstInvoice.Count > 0 Then

                        'method to import from enumerabledataroe collectionto datatable
                        Dim dtResult As DataTable = dss.Tables(0).Clone()
                        For Each dw As DataRow In lstInvoice
                            dtResult.ImportRow(dw)
                        Next
                        If dtResult IsNot Nothing Then
                            If dtResult.Rows.Count > 0 Then
                                loadData(Nothing, dtResult)
                            End If
                        End If
                    End If
                End If
            Else
                Dim message = "The Invoice Number is not valid."
                SendMessage(message, messageType.warning)
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        Dim exMessage As String = Nothing
        Dim dsData As DataSet = New DataSet()
        Dim result As Integer = -1
        Try
            cleanForm(filters)
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub lnkSearchVendorNo_Click(sender As Object, e As EventArgs) Handles lnkSearchVendorNo.Click
        Dim exMessage As String = Nothing
        Dim dsData As DataSet = New DataSet()
        Dim result As Integer = -1
        Try
            Session("SortedField") = Nothing
            If Not String.IsNullOrEmpty(txtvendor.Text) Then
                Using objBL As CustomerPayments.BL.CustPayment = New CustomerPayments.BL.CustPayment()
                    result = objBL.GetClientDataByNo(txtvendor.Text, dsData)
                    If result > 0 Then
                        If dsData IsNot Nothing Then
                            If dsData.Tables(0).Rows.Count > 0 Then
                                lblVndDesc.Text = dsData.Tables(0).Rows(0).ItemArray(1).ToString()
                                btnSearchPayments.Enabled = True
                            End If
                        End If
                    Else
                        btnSearchPayments.Enabled = False
                        Dim message = "The client number is not valid"
                        SendMessage(message, messageType.warning)
                    End If

                End Using

                loadData(Nothing)

            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Protected Sub btnExcel_Click(sender As Object, e As EventArgs) Handles btnGenerateExcel.Click
        Dim exMessage As String = Nothing
        Dim fileExtension As String = ""
        Dim fileName As String = ""
        Dim rsReturn As Integer = -1
        Dim dsResult = New DataSet()
        Try
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Start Open excel for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
            'Dim dsResult = DirectCast(Session("LostSaleData"), DataSet)
            rsReturn = GetCustPaymentsDataPartial(dsResult, True)
            If dsResult IsNot Nothing Then
                If dsResult.Tables(0).Rows.Count > 0 Then

                    Dim objGet = fillObj(dsResult.Tables(0))

                    Dim dt As DataTable = New DataTable()
                    dt = dsResult.Tables(0).Clone()
                    Dim dss = GenerateDataSetFromObject(objGet, dt)

                    Dim pathToProcess = ConfigurationManager.AppSettings("urlTemplateToProcess")
                    'Dim updUserPath = userPath + "\WishList-Template\"
                    Dim folderPath = pathToProcess
                    Dim methodMessage = If(Not String.IsNullOrEmpty(folderPath), "The template document will be downloaded to your documents folder", "There is not a path defined for this document. Call an administrator!!")

                    'Dim userPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    'Dim folderPath As String = userPath & "\Client-Payments\"

                    writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Check if path exists", "Occurs at time: " + DateTime.Now.ToString())
                    If Not Directory.Exists(folderPath) Then
                        Directory.CreateDirectory(folderPath)
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Path created", "Occurs at time: " + DateTime.Now.ToString())
                    Else
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Check process" + pathToProcess + ".", "Occurs at time: " + DateTime.Now.ToString())
                        Dim files = Directory.GetFiles(pathToProcess)
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Check process ok", "Occurs at time: " + DateTime.Now.ToString())
                        Dim fi = Nothing
                        If files.Length = 1 Then
                            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "file lenght = 1", "Occurs at time: " + DateTime.Now.ToString())
                            For Each item In files
                                fi = item
                                Dim isOpened = IsFileinUse(New FileInfo(fi))
                                If Not isOpened Then
                                    File.Delete(item)
                                    writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "File deleted", "Occurs at time: " + DateTime.Now.ToString())
                                Else
                                    SendMessage("Please close the file " & fi & " in order to proceed!", messageType.info)
                                    Exit Sub
                                End If
                            Next
                        Else
                            'SendMessage("Please close the file " & fi & " in order to proceed!", messageType.info)
                            'Exit Sub
                        End If
                    End If

                    Using objBL As CustomerPayments.BL.CustPayment = New CustomerPayments.BL.CustPayment()
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Determine Office version", "Occurs at time: " + DateTime.Now.ToString())
                        fileExtension = objBL.Determine_OfficeVersion()
                        If String.IsNullOrEmpty(fileExtension) Then
                            fileExtension = "xlsx"
                            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "exit method" + pathToProcess + ".", "Occurs at time: " + DateTime.Now.ToString())
                            'Exit Sub
                        End If

                        Dim title As String
                        title = "Client.Payments.generated.on"
                        fileName = objBL.adjustDatetimeFormat(title, fileExtension)

                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Extension and name ok", "Occurs at time: " + DateTime.Now.ToString())

                    End Using

                    Dim fullPath = folderPath + fileName

                    Using wb As New XLWorkbook()
                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "start save doc", "Occurs at time: " + DateTime.Now.ToString())

                        Try
                            wb.Worksheets.Add(dss.Tables(0), "ClientPayments")

                            wb.SaveAs(fullPath)
                        Catch exx As Exception
                            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Error: " + exx.Message + "" + exx.ToString() + ". ", "Occurs at time: " + DateTime.Now.ToString())
                        End Try


                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "end save doc in path " + fullPath + "", "Occurs at time: " + DateTime.Now.ToString())
                    End Using

                    If File.Exists(fullPath) Then

                        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Exist fullpath", "Occurs at time: " + DateTime.Now.ToString())

                        Dim myFile As FileInfo = New FileInfo(fullPath)
                        If myFile.Exists Then
                            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "Exist file", "Occurs at time: " + DateTime.Now.ToString())
                            Try
                                'Process.Start("explorer.exe", fullPath)
                                writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "try to open file", "Occurs at time: " + DateTime.Now.ToString())
                                Session("PathToDownload") = fullPath
                                Response.Redirect("Download.ashx", True)
                                'Process.Start("explorer.EXE", filePath1)
                            Catch Win32Exception As Win32Exception
                                writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + Win32Exception.Message + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
                                Shell("explorer " & fullPath, AppWinStyle.NormalFocus)
                            Catch ex As Exception
                                writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + ex.Message + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
                            End Try
                        End If
                    End If
                    loadData(dss)
                End If
            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    'Protected Sub lnkSearchVendorNo_Click(sender As Object, e As EventArgs) Handles lnkSearchVendorNo.Click
    '    Dim exMessage As String = Nothing
    '    Dim vndName As String = Nothing
    '    Try
    '        If Not String.IsNullOrEmpty(txtvendor.Text) Then
    '            Using objBL As CustomerPayments.BL.CustPayment = New CustomerPayments.BL.CustPayment()
    '                Dim dsData = objBL.getVendorTypeByVendorNum(txtvendor.Text)
    '                If dsData IsNot Nothing Then
    '                    If dsData.Tables(0).Rows.Count > 0 Then
    '                        Dim validVnd = objBL.isVendorAccepted(txtvendor.Text)
    '                        If validVnd Then
    '                            lblVndDesc.Text = dsData.Tables(0).Rows(0).ItemArray(1).ToString()
    '                        End If
    '                    End If
    '                End If
    '            End Using
    '        Else
    '            'must be have a value
    '        End If

    '    Catch ex As Exception
    '        exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
    '        writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
    '    End Try
    'End Sub

    Public Sub cleanForm(parent As Control)
        Try
            ClearInputCustom(parent)
            loadData(Nothing)
        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message + ex.ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

#End Region

#Region "Generics"

    Protected Sub lnkLogout_Click() Handles lnkLogout.Click
        Try
            FormsAuthentication.SignOut()
            Session.Abandon()
            coockieWork()
            Session("UserLoginData") = Nothing
            FormsAuthentication.RedirectToLoginPage()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub coockieWork()
        Try

            Dim cookie1 As HttpCookie = New HttpCookie(FormsAuthentication.FormsCookieName, "")
            cookie1.HttpOnly = True
            cookie1.Expires = DateTime.Now.AddYears(-1)
            Response.Cookies.Add(cookie1)

        Catch ex As Exception

        End Try
    End Sub

    Public Function GenerateDataSetFromObject(obj As Costumer, dt As DataTable) As DataSet
        Dim dsReturn As DataSet = New DataSet()
        Try
            Dim dtTemp = dt.Copy()
            Dim customerNo As String = obj.CUSTNO.Trim()

            For Each objR As Receipt In obj.LstReceipt

                For Each objIn As Invoice In objR.LstInvoice

                    Dim dr = dtTemp.NewRow()
                    dr("CUSTNO") = customerNo
                    dr("receipt") = objR.ReceiptId.Trim()
                    dr("invoice") = objIn.InvoiceId.Trim()
                    dr("invdt") = objIn.InvoiceDate.Trim()
                    dr("invamt") = objIn.InvoiceAmt.Trim()
                    dr("paydt") = objIn.InvoicePaidDate.Trim()
                    dr("paid") = objIn.InvoicePaid.Trim()
                    dr("balance") = objIn.InvoiceBalance.Trim()

                    dtTemp.Rows.Add(dr)

                Next

                dtTemp.AcceptChanges()

            Next

            dsReturn.Tables.Add(dtTemp)
            Return dsReturn

        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Sub ProcessDataBalance(ds As DataSet)
        Dim invDs As String = Nothing
        Dim lstDup As List(Of String) = New List(Of String)()
        Dim dsTemp As DataSet = New DataSet()
        Dim dtTemp As DataTable = New DataTable()
        Try
            If ds IsNot Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then
                    GetMoreThanOneInvoiceByReceipt(ds, lstDup)
                    If lstDup.Count > 0 Then
                        dtTemp = ds.Tables(0).Clone()

                        For Each dw As DataRow In ds.Tables(0).Rows
                            invDs = dw.Item("invoice").ToString().Trim()
                            If lstDup.Contains(invDs) Then

                                dtTemp.ImportRow(dw)
                            End If
                        Next
                    End If


                    'separate process
                    Dim firstB As String = Nothing
                    Dim newBalance As String = Nothing
                    For Each diw As DataRow In dtTemp.Rows
                        Dim i = dtTemp.Rows.IndexOf(diw)
                        If i = 0 Then
                            firstB = diw.Item("balance").ToString().Trim()
                        Else
                            Dim numFirstB = Decimal.Parse(firstB)
                            Dim numPaid = Decimal.Parse(diw.Item("paid").ToString().Trim())
                            diw.Item("balance") = (numFirstB - numPaid).ToString()
                            firstB = diw.Item("balance").ToString().Trim()
                        End If
                    Next


                    For Each dw2 As DataRow In ds.Tables(0).Rows

                    Next

                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Function CheckIfInvoiceIsDuplicated(invoice As String, balance As String, payment As String) As String
        Dim lstDuplicates As List(Of String) = New List(Of String)()
        Dim dctInvoiceBalance As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim strResult As String = Nothing
        Try
            lstDuplicates = If(Session("DuplicatesInv") IsNot Nothing, DirectCast(Session("DuplicatesInv"), List(Of String)), Nothing)
            If lstDuplicates Is Nothing Then
                Session("DctDuplicateInv") = Nothing
                Return strResult
            Else
                If Not lstDuplicates.Contains(UCase(invoice.Trim())) Then
                    Session("DctDuplicateInv") = Nothing
                    Return strResult
                Else
                    If Session("DctDuplicateInv") Is Nothing Then
                        dctInvoiceBalance.Add(invoice, balance)
                        Session("DctDuplicateInv") = dctInvoiceBalance
                        Return strResult
                    Else
                        Dim dctTemp = DirectCast(Session("DctDuplicateInv"), Dictionary(Of String, String))
                        If dctTemp.ContainsKey(invoice) Then
                            Dim paymentAmount = Decimal.Parse(payment)

                            Dim strAmount As String = Nothing
                            dctTemp.TryGetValue(invoice, strAmount)
                            If Not String.IsNullOrEmpty(strAmount) Then
                                Dim balanceAmount = Decimal.Parse(strAmount)

                                Dim newCalcBalance = balanceAmount - paymentAmount
                                Dim strNewBalance As String = newCalcBalance.ToString()
                                Return strNewBalance
                            Else
                                Session("DctDuplicateInv") = Nothing
                                Return strResult
                            End If
                        Else
                            Session("DctDuplicateInv") = Nothing
                            Return strResult
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
            Return strResult
        End Try
    End Function

    Public Sub GetMoreThanOneInvoiceByReceipt(ds As DataSet, Optional ByRef lstDuplicates As List(Of String) = Nothing)
        Try
            Dim duplicates = ds.Tables(0).AsEnumerable().Select(Function(dr) dr.Item("invoice").ToString()).GroupBy(Function(x) x).Where(Function(g) g.Count() > 1).
                            Select(Function(c) c.Key.Trim()).ToList()

            If duplicates.Count > 0 Then
                Session("DuplicatesInv") = duplicates
                If lstDuplicates IsNot Nothing Then
                    lstDuplicates = duplicates
                End If
            End If

        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, ex.Message, ex.ToString)
        End Try
    End Sub

    Public Sub writeComputerEventLog(Optional strMessage As String = Nothing)
        Dim exMessage As String = Nothing
        Try

            If Not EventLog.SourceExists("CustomerPayments.App") Then
                EventLog.CreateEventSource("CustomerPayments.App", "CustomerPayments.Log")
            End If
            'EventLog.CreateEventSource("CTPSystem-Net", "CTPSystem-Log")

            Dim lgSource = "CustomerPayments.App"
            Dim lgName = "CustomerPayments.Log"
            Dim msg = If(String.IsNullOrEmpty(strMessage), "Info: Session started for: " & Environment.UserName, strMessage)

            eventLog1 = New EventLog(lgName, Environment.MachineName, lgSource)
            eventLog1.WriteEntry(msg, EventLogEntryType.Information)

            Log.Info("Info Message: Adding info to console log.")

        Catch ex As Exception
            Log.Error("Error trying to put info un console log: " + ex.Message + ".")
        End Try
    End Sub

    Public Function GetAccessByUsers(ByRef sel As Integer, ByRef fullData As Boolean) As Boolean
        Dim optionSelection As String = Nothing
        Dim user As String = Nothing
        Dim flag As Boolean = False
        Dim authUser As String = Nothing
        Dim lstPurcUsers As List(Of String) = New List(Of String)()
        Dim exMessage As String = Nothing
        Try
            Dim validUsers = ConfigurationManager.AppSettings("validUsersForWeb")

            'Dim args As String() = Environment.GetCommandLineArgs()
            'Dim argumentsJoined = String.Join(".", args)

            'Dim arrayArgs As String() = argumentsJoined.Split(".")
            'optionSelection = UCase(arrayArgs(3).ToString().Replace(",", ""))
            'user = UCase(arrayArgs(2).ToString().Replace(",", ""))

            user = If(Session("userid") IsNot Nothing, UCase(Session("userid").ToString().Trim()), "NA")
            If Not user.Equals("NA") Then

                Dim ls = validUsers.Split(",")

                For Each item As String In ls
                    lstPurcUsers.Add(item)
                Next

                If lstPurcUsers.Count > 0 Then

                    authUser = lstPurcUsers.AsEnumerable().Where(Function(val) UCase(val).Trim().Contains(user)).First()

                    If Not String.IsNullOrEmpty(authUser) Then

                        fullData = If(LCase(validUsers.Trim()).Contains(LCase(authUser.Trim())), True, False)

                        If Not fullData Then
                            sel = 0
                            flag = False
                        Else
                            flag = True
                        End If

                        'fullData = False 'test remove
                        'Session("userid") = user
                        'full query -- >
                        'flag = True
                        Return flag
                    Else
                        'test
                        'Session("userid") = ConfigurationManager.AppSettings("authorizeTestUser")
                        'test

                        'not authorized user
                        sel = 0
                        Return False
                    End If

                End If
            Else
                sel = 1
                Return False
                'Response.Redirect("http://svrwebapps.costex.com/PurchasingApp/", True)
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
            Return flag
        End Try
    End Function

    Private Shared Function IsFileLocked(exception As Exception) As Boolean
        Dim errorCode As Integer = Marshal.GetHRForException(exception) And ((1 << 16) - 1)
        Return errorCode = 32 OrElse errorCode = 33
    End Function

    Public Function IsFileinUse(file As FileInfo) As Boolean
        Dim exMessage As String = Nothing
        Dim opened As Boolean = False
        Dim myStream As FileStream = Nothing
        Try
            myStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
        Catch ex As Exception

            If TypeOf ex Is IOException AndAlso IsFileLocked(ex) Then
                IO.File.Delete(file.Name)
                opened = False
            Else
                opened = True
            End If
        Finally
            If myStream IsNot Nothing Then
                myStream.Close()
            End If
        End Try
        Return opened
    End Function

    Private Sub loadData(Optional ds As DataSet = Nothing, Optional dt As DataTable = Nothing, Optional flag As Boolean = False, Optional fromPL As Boolean = False)
        Dim exMessage As String = Nothing
        Dim methodMessage As String = Nothing
        Dim lstDup As List(Of String) = New List(Of String)()
        Try
            If Session("GrvPageSize") IsNot Nothing Then
                grvCustPayment.PageSize = DirectCast(Session("GrvPageSize"), Integer)
            End If

            If ds IsNot Nothing Then
                If ds.Tables(0).Rows.Count > 0 Then

                    'grvWishList.PageSize = DirectCast(Session("PageAmountsDdl"), Integer)
                    'lblCountItems.Text = ds.Tables(0).Rows.Count.ToString()
                    Session("ItemCounts") = CInt(ds.Tables(0).Rows.Count.ToString())

                    'If Not fromPL Then
                    '    GetMoreThanOneInvoiceByReceipt(ds, lstDup)

                    '    For Each dw As DataRow In ds.Tables(0).Rows

                    '        If lstDup.Contains(dw.Item("invoice").ToString().Trim()) Then
                    '            Dim invoice = dw.Item("invoice").ToString().Trim()
                    '            Dim balance = dw.Item("balance").ToString().Trim()
                    '            Dim payment = dw.Item("paid").ToString().Trim()
                    '            Dim newBalance = CheckIfInvoiceIsDuplicated(invoice, balance, payment)

                    '            dw.Item("balance") = If(Not String.IsNullOrEmpty(newBalance), newBalance, dw.Item("balance").ToString())

                    '        End If

                    '    Next

                    'End If



                    'Session("SumarizeFlag") = fromPL
                    grvCustPayment.DataSource = ds.Tables(0)
                    grvCustPayment.DataBind()

                    'Session("CustPaymentData") = ds
                Else
                    grvCustPayment.DataSource = Nothing
                    grvCustPayment.DataBind()

                    If flag Then
                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)
                    End If

                End If

                Exit Sub
            Else
                If dt IsNot Nothing Then
                    If dt.Rows.Count > 0 Then

                        'grvWishList.PageSize = DirectCast(Session("PageAmountsDdl"), Integer)
                        'lblCountItems.Text = dt.Rows.Count.ToString()
                        Session("ItemCounts") = CInt(dt.Rows.Count.ToString())

                        'If Not fromPL Then
                        '    GetMoreThanOneInvoiceByReceipt(ds, lstDup)

                        '    For Each dw As DataRow In ds.Tables(0).Rows

                        '        If lstDup.Contains(dw.Item("invoice").ToString().Trim()) Then
                        '            Dim invoice = dw.Item("invoice").ToString().Trim()
                        '            Dim balance = dw.Item("balance").ToString().Trim()
                        '            Dim payment = dw.Item("paid").ToString().Trim()
                        '            Dim newBalance = CheckIfInvoiceIsDuplicated(invoice, balance, payment)

                        '            dw.Item("balance") = If(Not String.IsNullOrEmpty(newBalance), newBalance, dw.Item("balance").ToString())

                        '        End If

                        '    Next

                        'End If


                        'Session("SumarizeFlag") = fromPL
                        grvCustPayment.DataSource = dt
                        grvCustPayment.DataBind()

                        Dim dtt = New DataTable()
                        dtt = dt.Copy()
                        Dim dss = New DataSet()
                        dss.Tables.Add(dtt)

                        'Session("CustPaymentData") = dss
                    Else
                        grvCustPayment.DataSource = Nothing
                        grvCustPayment.DataBind()

                        If flag Then
                            methodMessage = "There is not results with the selected criteria."
                            SendMessage(methodMessage, messageType.warning)
                        End If

                    End If
                Else
                    grvCustPayment.DataSource = Nothing
                    grvCustPayment.DataBind()

                    If flag Then
                        methodMessage = "There is not results with the selected criteria."
                        SendMessage(methodMessage, messageType.warning)
                    End If
                End If

                Exit Sub
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Public Sub setDefaultValues(Optional ds As DataSet = Nothing)
        Dim exMessage As String = Nothing
        Dim myItem As EnumerableRowCollection(Of DataRow) = Nothing
        Dim dt As DataTable = New DataTable()
        Try
            If Session("SortedField") IsNot Nothing Then
                Dim sorted = DirectCast(Session("SortedField"), String)
                Dim direction = DirectCast(Session("sortDirection"), String)
                'Dim dir = SetSortDirection(direction)

                If direction.Equals("0") Then
                    'asc
                    'myItem = ds.Tables(0).AsEnumerable().OrderBy(Function(data) data.Item(sorted)).ThenBy(Function(dat) dat.Item(sorted))
                    myItem = ds.Tables(0).AsEnumerable().OrderBy(Function(data) data.Item(sorted))
                    EnumDrwCollectToDt(ds.Tables(0), myItem, dt)
                Else
                    'desc
                    'myItem = ds.Tables(0).AsEnumerable().OrderBy(Function(data) data.Item(sorted)).ThenByDescending(Function(dat) dat.Item(sorted))
                    myItem = ds.Tables(0).AsEnumerable().OrderByDescending(Function(data) data.Item(sorted))
                    EnumDrwCollectToDt(ds.Tables(0), myItem, dt)

                End If
            End If
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
        End Try
    End Sub

    Structure messageType
        Const success = "success"
        Const warning = "warning"
        Const info = "info"
        Const [Error] = "Error"
    End Structure

    Public Sub SendMessage(methodMessage As String, detailInfo As String)
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & " ')", True)
    End Sub

    Protected Function SetSortDirection(sortDirection As String) As String
        Dim _sortDirection As String = Nothing
        If sortDirection = "0" Then
            _sortDirection = "DESC"
        Else
            _sortDirection = "ASC"
        End If
        Session("sortDirection") = If(_sortDirection = "DESC", "1", "0")
        Return _sortDirection
    End Function

    Public Shared Function GetComputerName() As String
        Dim exMessage As String = Nothing
        Try
            Dim ComputerName As String
            ComputerName = Environment.MachineName
            Return ComputerName
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            'writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + ".", "Occurs at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try
    End Function

    Public Function dateComparission(dtStart As String, dtEnd As String) As Boolean
        Dim Culture = CultureInfo.CreateSpecificCulture("en-US")
        Dim styles = DateTimeStyles.AssumeLocal
        Dim dtOut As DateTime = New DateTime()
        Dim dtOut1 As DateTime = New DateTime()
        Dim result As Boolean = False
        Dim exMessage As String = Nothing
        Try
            Dim dtValue = dtStart.Split(" ")(0).Trim()
            Dim dtValue1 = dtEnd.Split(" ")(0).Trim()
            Dim dt2 = DateTime.TryParseExact(dtValue, "MM/dd/yyyy", Culture, styles, dtOut)
            Dim dt3 = DateTime.TryParseExact(dtValue1, "MM/dd/yyyy", Culture, styles, dtOut1)

            If dtOut > dtOut1 Or dtOut1 < dtOut Then
                Return result
            Else
                result = True
                Return result
            End If

        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + " for user " + Session("userid").ToString(), "Occurs at time: " + DateTime.Now.ToString())
            Return False
        End Try
    End Function

    Public Shared Function ListToDataTable(ByVal _List As Object) As DataTable

        Dim dt As New DataTable

        Dim obj As Object = _List(0)
        dt = ObjectToDataTable(obj)
        Dim dr As DataRow = dt.NewRow

        For Each obj In _List

            dr = dt.NewRow

            For Each p As PropertyInfo In obj.GetType.GetProperties

                'If p.Name = "WLIST" Then
                '    If p.GetValue(obj, p.GetIndexParameters) > 0 Then
                '        Dim pepe = p.GetValue(obj, p.GetIndexParameters)
                '    End If
                'End If
                dr.Item(p.Name) = p.GetValue(obj, p.GetIndexParameters)


            Next

            dt.Rows.Add(dr)

        Next

        Return dt

    End Function

    'Public Shared Function ObjectToDataTableDeep() As DataTable

    '    Return 0
    'End Function


    '    Public Static DataTable CreateDataTable<T>(IEnumerable<T> list)
    '{
    '    Type type = TypeOf(T);
    '    var properties = Type.GetProperties();      

    '    DataTable dataTable = New DataTable();
    '    dataTable.TableName = TypeOf(T).FullName;
    '    foreach (PropertyInfo info in properties)
    '    {
    '        dataTable.Columns.Add(New DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
    '    }

    '    foreach (T entity in list)
    '    {
    '        Object[] values = New Object[properties.Length];
    '        For (int i = 0; i < properties.Length; i++)
    '        {
    '            values[i] = properties[i].GetValue(entity);
    '        }

    '        dataTable.Rows.Add(values);
    '    }

    '    Return dataTable;
    '}

    Public Shared Function ObjectToDataTable(ByVal o As Object) As DataTable
        Dim exMessage As String = Nothing
        Try
            Dim dt As New DataTable
            Dim properties As List(Of PropertyInfo) = o.GetType.GetProperties.ToList()

            For Each prop As PropertyInfo In properties
                dt.Columns.Add(prop.Name, prop.PropertyType)
            Next

            dt.TableName = o.GetType.Name
            Return dt
        Catch ex As Exception
            Log.Info(strLogCadenaCabecera + ".." + ex.Message)
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            'writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "Error Ocurred: " + exMessage + ".", "Occurs at time: " + DateTime.Now.ToString())
            Return Nothing
        End Try

    End Function

    Public Sub ClearInputCustom(parent As Control)

        Try
            For Each ctl As Control In parent.Controls

                If (ctl.Controls.Count > 0) Then
                    ClearInputCustom(ctl)
                Else
                    If TypeOf ctl Is TextBox Then
                        DirectCast(ctl, TextBox).Text = String.Empty
                    End If
                    'If TypeOf ctl Is Label Then
                    '    DirectCast(ctl, Label).Text = String.Empty
                    'End If
                    If TypeOf ctl Is DropDownList Then
                        If (DirectCast(ctl, DropDownList).Enabled Or Not (DirectCast(ctl, DropDownList)).Enabled) Then
                            DirectCast(ctl, DropDownList).ClearSelection()
                        End If
                    End If
                    If TypeOf ctl Is ListBox Then
                        If (DirectCast(ctl, ListBox).Enabled Or Not (DirectCast(ctl, ListBox)).Enabled) Then
                            DirectCast(ctl, ListBox).ClearSelection()
                            DirectCast(ctl, ListBox).Items.Clear()
                        End If
                    End If
                    If TypeOf ctl Is CheckBox Then
                        DirectCast(ctl, CheckBox).Checked = False
                    End If
                End If

            Next
        Catch ex As Exception
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Information, "User Logged In Wish List: " + Session("userid").ToString(), "Login at time: " + DateTime.Now.ToString())
        End Try
    End Sub

#End Region

#Region "Fill Objects"

    Private Function getObjInvFromDw(dw As DataRow) As Invoice
        Dim objInvoice As Invoice = New Invoice()

        Try
            Dim duplicates = If(Session("DuplicatesInv") IsNot Nothing, DirectCast(Session("DuplicatesInv"), List(Of String)), Nothing)
            If dw IsNot Nothing Then

                'If dw.Item("invoice").ToString().Trim().Contains("F72298") Then
                '    Dim pppp = "a"
                'End If

                objInvoice.InvoiceId = dw.Item("invoice").ToString().Trim()
                objInvoice.InvoiceDate = dw.Item("invdt").ToString().Trim()
                objInvoice.InvoiceAmt = dw.Item("invamt").ToString().Trim()
                objInvoice.InvoicePaidDate = dw.Item("paydt").ToString().Trim()
                objInvoice.InvoicePaid = dw.Item("paid").ToString().Trim()

                If duplicates IsNot Nothing Then
                    If duplicates.Contains(dw.Item("invoice").ToString().Trim()) Then
                        If Session("Dup") IsNot Nothing Then
                            If Session("Dup").ToString() <> "0.00" Then
                                objInvoice.InvoiceBalance = (Decimal.Parse(Session("Dup").ToString()) - Decimal.Parse(dw.Item("paid").ToString().Trim())).ToString()
                                objInvoice.TempBalance = objInvoice.InvoiceBalance
                                Session("Dup") = objInvoice.TempBalance
                            Else
                                Session("Dup") = dw.Item("balance").ToString().Trim()
                                objInvoice.InvoiceBalance = dw.Item("balance").ToString().Trim()
                                objInvoice.TempBalance = If(String.IsNullOrEmpty(dw.Item("invamt").ToString().Trim()) Or String.IsNullOrEmpty(dw.Item("paid").ToString().Trim()), 0,
                                    (Decimal.Parse(dw.Item("invamt").ToString().Trim()) - Decimal.Parse(dw.Item("paid").ToString().Trim())).ToString())
                            End If
                        Else
                            Session("Dup") = dw.Item("balance").ToString().Trim()
                            objInvoice.InvoiceBalance = dw.Item("balance").ToString().Trim()
                            objInvoice.TempBalance = If(String.IsNullOrEmpty(dw.Item("invamt").ToString().Trim()) Or String.IsNullOrEmpty(dw.Item("paid").ToString().Trim()), 0,
                                (Decimal.Parse(dw.Item("invamt").ToString().Trim()) - Decimal.Parse(dw.Item("paid").ToString().Trim())).ToString())
                        End If
                    Else
                        objInvoice.InvoiceBalance = dw.Item("balance").ToString().Trim()
                        objInvoice.TempBalance = If(String.IsNullOrEmpty(dw.Item("invamt").ToString().Trim()) Or String.IsNullOrEmpty(dw.Item("paid").ToString().Trim()), 0,
                            (Decimal.Parse(dw.Item("invamt").ToString().Trim()) - Decimal.Parse(dw.Item("paid").ToString().Trim())).ToString())
                        Session("Dup") = Nothing
                    End If
                Else
                    objInvoice.InvoiceBalance = dw.Item("balance").ToString().Trim()
                    objInvoice.TempBalance = If(String.IsNullOrEmpty(dw.Item("invamt").ToString().Trim()) Or String.IsNullOrEmpty(dw.Item("paid").ToString().Trim()), 0,
                        (Decimal.Parse(dw.Item("invamt").ToString().Trim()) - Decimal.Parse(dw.Item("paid").ToString().Trim())).ToString())
                    Session("Dup") = Nothing
                End If

                Return objInvoice
            End If
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Private Function addEmptyRowToEnd(dt As DataTable) As DataTable
        Dim dtTemp As DataTable = New DataTable()
        Try
            dtTemp = dt.Copy()
            Dim dr = dtTemp.NewRow()
            dr("CUSTNO") = dt.Rows(0).Item("CUSTNo").ToString().Trim()
            dr("receipt") = "0"
            dr("invoice") = "0"
            dr("invdt") = DateTime.Now.ToString()
            dr("invamt") = "0"
            dr("paydt") = DateTime.Now.ToString()
            dr("paid") = "0"
            dr("balance") = "0"

            dtTemp.Rows.Add(dr)
            Return dtTemp
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function fillObj(dt As DataTable) As Costumer
        Dim exMessage As String = Nothing
        Dim objCostumer = Nothing
        Dim lstCostumer = New List(Of Costumer)()
        Session("Dup") = Nothing
        Dim lstDup = New List(Of String)()

        Try

#Region "Get Duplicate Invoices"

            Dim ds = New DataSet()
            Dim dt1 = dt.Copy()
            ds.Tables.Add(dt1)
            GetMoreThanOneInvoiceByReceipt(ds, lstDup)

#End Region

#Region "Obj Declaration"

            Dim objCostNo = New Costumer()
            Dim objReceipt = New Receipt()
            Dim objInvoice = New Invoice()
            Dim lstReceipt = New List(Of Receipt)()
            Dim lstInvoice = New List(Of Invoice)()

            objReceipt.LstInvoice = lstInvoice
            objCostNo.LstReceipt = lstReceipt

#End Region
            Dim currentReceipt As String = Nothing
            Dim currentInvoice As String = Nothing
            Dim currentCostumer As String = dt.Rows(0).Item("CUSTNO").ToString().Trim()

            Dim dtFull = addEmptyRowToEnd(dt)

            currentReceipt = dtFull.Rows(0).Item("receipt").ToString().Trim()
            For Each dw As DataRow In dtFull.Rows

                'If lstDup IsNot Nothing Then
                '    If lstDup.Contains(dw.Item("invoice").ToString().Trim()) Then

                If dw.Item("receipt").ToString().Trim().Equals(currentReceipt) Then
                    objReceipt.ReceiptId = currentReceipt
                    Dim objProc = getObjInvFromDw(dw)

                    'If objProc.InvoiceId.Contains("F72298") Then
                    '    Dim pp = objProc.InvoiceId
                    'End If

                    objReceipt.LstInvoice.Add(objProc)
                Else
                    'objReceipt.LstInvoice = lstInvoice
                    objReceipt.InvoiceQty = If(objReceipt.LstInvoice IsNot Nothing, objReceipt.LstInvoice.Count.ToString(), 0)
                    objCostNo.LstReceipt.Add(objReceipt)

                    'Session("Dup") = If(objCostNo.LstReceipt.AsEnumerable().Any(Function(ss) ss.ObjInvoice.InvoiceId = objReceipt.LstInvoice(CInt(objReceipt.InvoiceQty) - 1).InvoiceId And
                    '                                                      objReceipt.LstInvoice(CInt(objReceipt.InvoiceQty) - 1).InvoiceBalance <> 0),
                    '                                                        objReceipt.LstInvoice(CInt(objReceipt.InvoiceQty) - 1).InvoiceBalance, "0.00")

                    Session("Dup") = If(objCostNo.LstReceipt.AsEnumerable().Any(Function(ss) ss.LstInvoice.AsEnumerable().Any(Function(dd) dd.InvoiceId = objReceipt.LstInvoice(CInt(objReceipt.InvoiceQty) - 1).InvoiceId) And
                                                                                objReceipt.LstInvoice(CInt(objReceipt.InvoiceQty) - 1).InvoiceBalance <> 0), objReceipt.LstInvoice(CInt(objReceipt.InvoiceQty) - 1).InvoiceBalance, "0.00")

                    objCostNo.CUSTNO = If(String.IsNullOrEmpty(txtvendor.Text.Trim()), currentCostumer, txtvendor.Text.Trim())
                    currentReceipt = dw.Item("receipt").ToString().Trim()
                    objReceipt = New Receipt()
                    objReceipt.ReceiptId = currentReceipt
                    objReceipt.LstInvoice = New List(Of Invoice)()

                    Dim objProc = getObjInvFromDw(dw)
                    objReceipt.LstInvoice.Add(objProc)
                    'Session("Dup") = Nothing
                End If

                '    End If
                'End If
            Next

            Dim tempp = JsonConvert.SerializeObject(objCostNo)

            Return objCostNo



            'Try
            '    lstCostumer = JsonConvert.DeserializeObject(tempp)
            'Catch ex As Exception
            '    Dim exxx = ex.Message
            'End Try

            'Try
            '    Dim lstCostumer1 = JsonConvert.DeserializeObject(Of Costumer)(tempp)
            'Catch ex As Exception
            '    Dim essd = ex.Message
            'End Try


            'Dim items As IList(Of Costumer) = dt.AsEnumerable() _
            '    .Select(Function(row) New Costumer() With {
            '    .
        Catch ex As Exception

        End Try

    End Function

#End Region

#Region "Logs"

    Public Sub writeLog(strLogCadenaCabecera As String, strLevel As Logs.ErrorTypeEnum, strMessage As String, strDetails As String)
        strLogCadena = strLogCadenaCabecera + " " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
        Dim userid = If(DirectCast(Session("userid"), String) IsNot Nothing, DirectCast(Session("userid"), String), "N/A")
        objLog.WriteLog(strLevel, "CTPSystem" & strLevel, strLogCadena, userid, strMessage, strDetails)
    End Sub

#End Region

#Region "Not in use Now"

    'Protected Sub txtDate_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtDate.TextChanged
    '    Try
    '        'btnSearchPayments.Enabled = True

    '        Dim dtOut As DateTime = New DateTime()
    '        Dim Culture = CultureInfo.CreateSpecificCulture("en-US")
    '        Dim styles = DateTimeStyles.AssumeLocal

    '        Dim startDate = txtDate.Text
    '        Dim dtValue = startDate.Split(" ")(0).Trim()
    '        Dim dt2 = DateTime.TryParseExact(dtValue, "MM/dd/yyyy", Culture, styles, dtOut)
    '        Dim strStartDate = dtOut.ToString().Split(" ")(0).Trim()

    '        If Not String.IsNullOrEmpty(txtDateTo.Text) Then
    '            If Not dateComparission(txtDate.Text, txtDateTo.Text) Then
    '                SendMessage("The Start Date must be smaller than the End Date", messageType.warning)
    '            End If
    '        Else
    '            txtDateTo.Text = strStartDate
    '        End If

    '        'txtDateTo_TextChanged(Nothing, Nothing)

    '    Catch ex As Exception

    '    End Try
    'End Sub

    'Protected Sub txtDateTo_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtDateTo.TextChanged
    '    Try
    '        If Not String.IsNullOrEmpty(hdEndData.Value) Then
    '            txtDateTo.Text = hdEndData.Value
    '        End If
    '        If Not dateComparission(txtDate.Text, txtDateTo.Text) Then
    '            SendMessage("The Start Date must be smaller than the End Date", messageType.warning)
    '        End If

    '    Catch ex As Exception

    '    End Try
    'End Sub

#End Region

End Class