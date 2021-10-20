Imports System.Configuration
Imports System.Globalization
Imports Microsoft.Win32

Public Class CustPayment : Implements IDisposable

    Public Function GetNewCustPayments(clientNo As String, lstDates As List(Of String), ByRef dsResult As DataSet, ByRef Optional messageOut As String = Nothing) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CustPayment()
            result = objDal.GetNewCustPayments(clientNo, lstDates, dsResult, messageOut)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetCustPaymentDataByCliNo(clientNo As String, lstDates As List(Of String), ByRef dsResult As DataSet, ByRef Optional messageOut As String = Nothing) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CustPayment()
            result = objDal.GetCustPaymentDataByCliNo(clientNo, lstDates, dsResult, messageOut)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetAllCustPaymentData(ByRef dsResult As DataSet, Optional lstDates As List(Of String) = Nothing, ByRef Optional messageOut As String = Nothing) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CustPayment()
            result = objDal.GetAllCustPaymentData(dsResult, lstDates, messageOut)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetCustPaymentDataBySelection(ByRef dsResult As DataSet, prepareQuery As List(Of String), ByRef Optional messageOut As String = Nothing) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CustPayment()
            Dim resultQuery As String = Nothing
            If prepareQuery.Count = 4 Then
                resultQuery = " And " + prepareQuery(0) + "'" & prepareQuery(1) & "'" + " And " + prepareQuery(2) + "'" & prepareQuery(3) & "'"
            Else
                resultQuery = " And " + prepareQuery(0) + "'" & prepareQuery(1) & "'"
            End If
            result = objDal.GetCustPaymentDataBySelection(dsResult, resultQuery, messageOut)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try
    End Function

    Public Function GetClientDataByNo(cliNo As String, ByRef dsResult As DataSet) As Integer
        dsResult = New DataSet()
        Dim result As Integer = -1
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CustPayment()
            result = objDal.GetClientDataByNo(cliNo, dsResult)
            Return result
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return result
        End Try

    End Function

    Public Function adjustDatetimeFormat(documentName As String, documentExt As String) As String

        Dim exMessage As String = Nothing
        Try
            Dim name As String = Nothing
            Dim culture As CultureInfo = CultureInfo.CreateSpecificCulture("en-US")
            Dim dtfi As DateTimeFormatInfo = culture.DateTimeFormat
            dtfi.DateSeparator = "."

            Dim now As DateTime = DateTime.Now
            Dim halfName = now.ToString("G", dtfi)
            halfName = halfName.Replace(" ", ".")
            halfName = halfName.Replace(":", "")
            Dim fileName = documentName & "." & halfName & "." & documentExt
            Return fileName
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try

    End Function

    Public Function Determine_OfficeVersion() As String
        Dim exMessage As String = " "
        Dim strExt As String = Nothing
        Try
            Dim strEVersionSubKey As String = "\Excel.Application\CurVer" '/HKEY_CLASSES_ROOT/Excel.Application/Curver

            Dim strValue As String 'Value Present In Above Key
            Dim strVersion As String 'Determines Excel Version
            Dim strExtension() As String = {"xls", "xlsx"}

            Dim rkVersion As RegistryKey = Nothing 'Registry Key To Determine Excel Version
            rkVersion = Registry.ClassesRoot.OpenSubKey(name:=strEVersionSubKey, writable:=False) 'Open Registry Key

            If Not rkVersion Is Nothing Then 'If Key Exists
                strValue = rkVersion.GetValue(String.Empty) 'get Value
                strValue = strValue.Substring(strValue.LastIndexOf(".") + 1) 'Store Value

                Select Case strValue 'Determine Version
                    Case "7"
                        strVersion = "95"
                        strExt = strExtension(0)
                    Case "8"
                        strVersion = "97"
                        strExt = strExtension(0)
                    Case "9"
                        strVersion = "2000"
                        strExt = strExtension(0)
                    Case "10"
                        strVersion = "2002"
                        strExt = strExtension(0)
                    Case "11"
                        strVersion = "2003"
                        strExt = strExtension(0)
                    Case "12"
                        strVersion = "2007"
                        strExt = strExtension(1)
                    Case "14"
                        strVersion = "2010"
                        strExt = strExtension(1)
                    Case "15"
                        strVersion = "2013"
                        strExt = strExtension(1)
                    Case "16"
                        strVersion = "2016"
                        strExt = strExtension(1)
                    Case Else
                        strExt = strExtension(1)
                End Select

                Return strExt
            Else
                Return strExt
            End If
        Catch ex As Exception
            exMessage = ex.Message + ". " + ex.ToString

            Return strExt
        End Try
    End Function

#Region "Vendor methods - Not in use"

    Public Function getVendorTypeByVendorNum(vendorNo As String, Optional ByVal flag As Integer = 0) As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Dim TcpPartNo As String = " "
        Try
            Dim objDal = New DAL.CustPayment()
            dsResult = objDal.getVendorTypeByVendorNum(vendorNo, flag)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

    Public Function isVendorAccepted(vendorNo As String) As Boolean
        Dim exMessage As String = " "
        Try
            'Dim vendorType = getVendorTypeByVendorNum(vendorNo)
            Dim ds As DataSet = getVendorTypeByVendorNum(vendorNo)
            If ds IsNot Nothing Then
                Dim vendorType = ds.Tables(0).Rows(0).ItemArray(0).ToString()
                Dim vendorName = ds.Tables(0).Rows(0).ItemArray(1).ToString()
                Dim listDeniedCodes = ConfigurationManager.AppSettings("vendorCodesDenied")
                'VendorCodesDenied.Split(",")
                Dim containsDenied = listDeniedCodes.AsEnumerable().Any(Function(x As String) x = "'" & vendorType & "'")
                If Not containsDenied Then
                    Dim OEMContain = getOEMVendorCodes(ConfigurationManager.AppSettings("vendorOEMCodeDenied"))
                    Dim containsOEM = OEMContain.Tables(0).AsEnumerable().Any(Function(x) Trim(x.ItemArray(0).ToString()) = Trim(vendorNo))
                    If Not containsOEM Then
                        'frmLoadExcel.lblVendorDesc.Text = vendorName
                        'MessageBox.Show("The vendor " & RTrim(vendorName) & " is an accepted vendor for the operation.", "CTP System", MessageBoxButtons.OK)
                        Return True
                    Else
                        'MessageBox.Show("The vendor " & RTrim(vendorName) & " is not an accepted vendor for the operation.", "CTP System", MessageBoxButtons.OK)
                        Return False
                    End If
                Else
                    'MessageBox.Show("The vendor " & RTrim(vendorName) & " is not an accepted vendor for the operation.", "CTP System", MessageBoxButtons.OK)
                    Return False
                End If
            End If
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            'Log.Error(exMessage)
            Return False
        End Try
    End Function

    Public Function getOEMVendorCodes(cntrCode As String) As DataSet
        Dim dsResult = New DataSet()
        Dim exMessage As String = " "
        Try
            Dim objDal = New DAL.CustPayment()
            dsResult = objDal.getOEMVendorCodes(cntrCode)
            Return dsResult
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            Return Nothing
        End Try
    End Function

#End Region

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
