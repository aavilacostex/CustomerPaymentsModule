Imports System.Globalization
Imports CustomerPayments.DTO
Imports CustomerPayments.UTIL

Public Class CustPayment : Implements IDisposable

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing

    Shared ReadOnly objLog = New Logs()

    Public Function GetNewCustPayments(clientNo As String, lstDates As List(Of String), ByRef dsResult As DataSet, ByRef Optional messageOut As String = Nothing) As Integer
        Dim result As Integer = -1
        'messageOut = Nothing
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim query = "with y as ( select a.crgrf# invoice, (select ctpinv.cvtdcdtf(ohdate,'MDY') from qs36f.hordhd2 where ohrcd=1 and  ohcu#=a.CRGCU# 
                    and ohinno=a.CRGRF# fetch first 1 rows only) invdt, (select sum(OhSl$+OhTx$+ohgl$+ohsp$+ohcod$) from qs36f.hordhd2 where  ohrcd=1 and ohcu#=a.CRGCU# 
                    and ohinno=a.CRGRF#  group by ohcu#/*,ohinno*/ ) invamt,(a.CRGPD$+a.CRGDS$+a.CRGAJ$) paid, (select ctpinv.cvtdcdtf(CHDBCD,'YMD') from qs36f.crbhd01 where chdbc#=a.crgbc#) paydt 
                    from qs36f.cshrg01 a where a.CRGBC# IN (SELECT CHDBC# FROM qs36f.crbhd01 WHERE CHDBC#=a.CRGBC# AND CHDSTS='C') 
                    and a.crgcu#={0})                                                  
                    select paydt,invdt,invoice,invamt, paid from y where invdt between '{1}' and '{2}' /*and INVOICE = 'H98970'*/   order by 3  "

        Dim resultQuery = String.Format(query, clientNo, lstDates(0), lstDates(1))

        Try

            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            result = objDatos.GetDataFromDatabase(resultQuery, dsOut, dt, messageOut)
            dsResult = dsOut
            Return result

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetCustPaymentDataByCliNo(clientNo As String, lstDates As List(Of String), ByRef dsResult As DataSet, ByRef Optional messageOut As String = Nothing) As Integer
        Dim result As Integer = -1
        'messageOut = Nothing
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim query = "with y as ( select a.CRGCU# custno,a.crgcka receipt,a.crgrf# invoice, (select ctpinv.cvtdcdtf(ohdate,'MDY') from qs36f.hordhd2 where ohrcd=1 and       
                    ohcu#=a.CRGCU# and ohinno=a.CRGRF# fetch first 1 rows only) invdt, (select sum(OhSl$+OhTx$+ohgl$+ohsp$+ohcod$) from qs36f.hordhd2 where      
                    ohrcd=1 and ohcu#=a.CRGCU# and ohinno=a.CRGRF# group by ohcu#,ohinno) invamt, (a.CRGPD$+a.CRGDS$+a.CRGAJ$) paid,                                  
                    (select ctpinv.cvtdcdtf(CHDBCD,'YMD') from qs36f.crbhd01 where chdbc#=a.crgbc#) paydt from qs36f.cshrg01 a                                                
                    where a.CRGBC# IN (SELECT CHDBC# FROM qs36f.crbhd01 WHERE CHDBC#=a.CRGBC# AND CHDSTS='C') and a.crgcu#={0})                                                  
                    select custno,receipt,invoice,invdt,invamt,paydt, paid,invamt-paid balance from y where invdt between '{1}' and '{2}'            
                    order by invoice "

        Dim resultQuery = String.Format(query, clientNo, lstDates(0), lstDates(1))

        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            result = objDatos.GetDataFromDatabase(resultQuery, dsOut, dt, messageOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetAllCustPaymentData(ByRef dsResult As DataSet, Optional lstDates As List(Of String) = Nothing, ByRef Optional messageOut As String = Nothing) As Integer
        Dim result As Integer = -1
        'messageOut = Nothing
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim query = "with y as ( select a.CRGCU# custno,a.crgcka receipt,a.crgrf# invoice, (select ctpinv.cvtdcdtf(ohdate,'MDY') from qs36f.hordhd2 where ohrcd=1 and       
                    ohcu#=a.CRGCU# and ohinno=a.CRGRF# fetch first 1 rows only) invdt, (select sum(OhSl$+OhTx$+ohgl$+ohsp$+ohcod$) from qs36f.hordhd2 where      
                    ohrcd=1 and ohcu#=a.CRGCU# and ohinno=a.CRGRF# group by ohcu#,ohinno) invamt, (a.CRGPD$+a.CRGDS$+a.CRGAJ$) paid,                                  
                    (select ctpinv.cvtdcdtf(CHDBCD,'YMD') from qs36f.crbhd01 where chdbc#=a.crgbc#) paydt from qs36f.cshrg01 a                                                
                    where a.CRGBC# IN (SELECT CHDBC# FROM qs36f.crbhd01 WHERE CHDBC#=a.CRGBC# AND CHDSTS='C'))                                                  
                    select custno,receipt,invoice,invdt,invamt,paydt, paid,invamt-paid balance from y "
        Dim extraQuery = "where invdt between '{1}' and '{2}' order by invoice "

        Dim resultQuery = If(lstDates IsNot Nothing, String.Format(query + extraQuery, lstDates(0), lstDates(1)), query)
        'Dim QueryOk = String.Format(resultQuery, lstDates(0), lstDates(1))

        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            result = objDatos.GetDataFromDatabase(resultQuery, dsOut, dt, messageOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetCustPaymentDataBySelection(ByRef dsResult As DataSet, prepareQuery As String, ByRef Optional messageOut As String = Nothing) As Integer
        Dim result As Integer = -1
        'messageOut = Nothing
        dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim query = "with y as ( select a.CRGCU# custno,a.crgcka receipt,a.crgrf# invoice, (select ctpinv.cvtdcdtf(ohdate,'MDY') from qs36f.hordhd2 where ohrcd=1 and       
                    ohcu#=a.CRGCU# and ohinno=a.CRGRF# fetch first 1 rows only) invdt, (select sum(OhSl$+OhTx$+ohgl$+ohsp$+ohcod$) from qs36f.hordhd2 where      
                    ohrcd=1 and ohcu#=a.CRGCU# and ohinno=a.CRGRF# group by ohcu#,ohinno) invamt, (a.CRGPD$+a.CRGDS$+a.CRGAJ$) paid,                                  
                    (select ctpinv.cvtdcdtf(CHDBCD,'YMD') from qs36f.crbhd01 where chdbc#=a.crgbc#) paydt from qs36f.cshrg01 a                                                
                    where a.CRGBC# IN (SELECT CHDBC# FROM qs36f.crbhd01 WHERE CHDBC#=a.CRGBC# AND CHDSTS='C') {0})                                                  
                    select custno,receipt,invoice,invdt,invamt,paydt, paid,invamt-paid balance from y order by invoice "
        'Dim extraQuery = "where invdt between '{1}' and '{2}' order by invoice "

        'Dim resultQuery = If(lstDates IsNot Nothing, String.Format(query + extraQuery, lstDates(0), lstDates(1)), query)
        Dim QueryOk = String.Format(query, prepareQuery)

        Try
            Dim dsOut = New DataSet()
            Dim objDatos = New ClsRPGClientHelper()
            Dim dt As DataTable = New DataTable()
            result = objDatos.GetDataFromDatabase(QueryOk, dsOut, dt, messageOut)
            dsResult = dsOut
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return result
        End Try
    End Function

    Public Function GetClientDataByNo(cliNo As String, ByRef dsResult As DataSet) As Integer
        Dim exMessage As String = " "
        Dim ds = New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1
        Dim dt As DataTable = New DataTable()
        Dim dsOut = New DataSet()
        Try
            Dim objDatos = New ClsRPGClientHelper()
            Dim Sql = "select cunum, cunam from qs36f.cscumst where cunum = " & cliNo & " "
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            If dsOut IsNot Nothing Then
                dsResult = dsOut
            Else
                dsResult = Nothing
            End If
            Return affectedRows
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return affectedRows
        End Try
    End Function

    Public Function getVendorTypeByVendorNum(vendorNo As String, Optional ByVal flag As Integer = 0) As Data.DataSet
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim ds = New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1

        Dim objDatos = New ClsRPGClientHelper()
        Dim dt As DataTable = New DataTable()
        Dim dsOut = New DataSet()
        Dim Sql = "select vmvtyp, vmname from qs36f.vnmas where vmvnum = " & vendorNo & " "
        Try
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            If dsOut IsNot Nothing Then
                Return dsOut
            Else
                Return Nothing
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function

    Public Function getOEMVendorCodes(cntrCode As String) As Data.DataSet
        Dim exMessage As String = " "
        Dim result As Integer = -1
        Dim ds As New DataSet()
        ds.Locale = CultureInfo.InvariantCulture
        Dim affectedRows As Integer = -1

        Dim objDatos = New ClsRPGClientHelper()
        Dim dt As DataTable = New DataTable()
        Dim dsOut = New DataSet()
        Dim Sql = "select CNTDE1 from qs36f.cntrll where cnt01 = " & cntrCode & " "
        Try
            affectedRows = objDatos.GetDataFromDatabase(Sql, dsOut, dt)
            'result = objDatos.GetOdBcDataFromDatabase(Sql, ds)
            If dsOut IsNot Nothing Then
                Return dsOut
            Else
                Return Nothing
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            objLog.writeLog(strLogCadenaCabecera, objLog.ErrorTypeEnum.Exception, ex.Message, ex.ToString())
            Return Nothing
        End Try
    End Function


#Region "DISPOSABLE"

    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
