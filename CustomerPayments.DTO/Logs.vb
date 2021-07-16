Imports System.Configuration
Imports CustomerPayments.UTIL

Public Class Logs : Implements IDisposable

    Private Shared eventLog1 As EventLog = New EventLog("CustPayments-Log", GetComputerName(), "CustPayments-Net")

    Private Shared strconnSQL As String
    Public Shared Property SQLCon() As String
        Get
            Return strconnSQL
        End Get
        Set(ByVal value As String)
            strconnSQL = value
        End Set
    End Property

    Public Enum ErrorTypeEnum
        [Start]
        [Stop]
        [Information]
        [Error]
        [Trace]
        [Warning]
        [Exception]
    End Enum

    Public Sub New()
        SQLCon = ConfigurationManager.AppSettings("strconnSQL").ToString()
    End Sub

    Public Sub WriteLog(ErrorType As ErrorTypeEnum, strTipo As String, strMethod As String, strUser As String, strMessage As String, strDetails As String)
        Dim exMessage As String = Nothing
        Try
            Dim logMapping As String = If(String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings("LogMapping").ToLower()),
                                            "all", System.Configuration.ConfigurationManager.AppSettings("LogMapping").ToLower())

            If System.String.Compare(logMapping.ToLower(), "none", System.StringComparison.Ordinal) = 0 Then
            Else
                WriteToLogDB(ErrorType, strTipo, strMethod, strUser, strMessage, strDetails)
                If ErrorType.Equals(ErrorTypeEnum.Error) Then
                    'send email
                End If
            End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
        End Try
    End Sub

    Private Shared Sub WriteToLogDB(ErrorType As ErrorTypeEnum, strTipo As String, strMethod As String, strUsuario As String, strMessage As String, strDetalle As String)
        Dim exMessage As String = Nothing
        Dim rowsAffected As Integer = -1
        Try

            'Dim sqlQuery As String = "DELETE FROM {0} WHERE USERID = '{1}'"

            Dim dt As DateTime = DateTime.Now
            Dim curDate As String = dt.ToString()

            Dim sqlQuery As String = "INSERT INTO dbCTPSystem.dbo.CtpSystemLog (LOGAPP,LOGLEVEL,LOGTYPE,LOGUSER,LOGORIGEN,LOGMESSAGE,LOGEXCEPTION,LOGDATE )
                                        VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')"
            Dim sqlFormattedQuery As String = String.Format(sqlQuery, "CTPSystem-Web", ErrorType, strTipo, strUsuario, strMethod, strMessage, strDetalle, curDate)

            Dim objDatos = New ClsRPGClientHelper()

            Dim rsResult = objDatos.ExecuteNotQueryCommand(sqlFormattedQuery, SQLCon)
            'objDatos.InsertDataInDatabaseSQL(sqlFormattedQuery, rowsAffected)
            'If rowsAffected > 0 Then
            '    'okok
            'Else
            '    'no process
            'End If

        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
            If Not EventLog.SourceExists("CTPSystem-Log") Then
                EventLog.CreateEventSource("CTPSystem-Net", "CTPSystem-Log")
            End If
            eventLog1 = New EventLog("CTPSystem-Log", Environment.MachineName, "CTPSystem-Net")
            eventLog1.WriteEntry("Error: " + ex.Message, EventLogEntryType.Error)
        End Try
    End Sub

    Public Shared Function GetComputerName() As String
        Dim exMessage As String = Nothing
        Try
            Dim ComputerName As String
            ComputerName = Environment.MachineName
            Return ComputerName
        Catch ex As Exception
            exMessage = ex.ToString + ". " + ex.Message + ". " + ex.ToString
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
