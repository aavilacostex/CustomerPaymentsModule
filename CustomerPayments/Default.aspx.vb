Imports CustomerPayments.DTO

Public Class _Default
    Inherits Page

    Private Shared strLogCadenaCabecera As String = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString()
    Dim strLogCadena As String = Nothing

    Dim objLog = New Logs()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'Session("userid") = Nothing 'forcing session timeout
        Dim url As String = Nothing
        Try

            If Session("userid") Is Nothing Then

                'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "GetLocalDB()", True)

                url = String.Format("Login.aspx?data={0}", "Session Expired!")
                Session("url") = url
                Response.Redirect(url, False)

            Else
                Dim welcomeMsg = ConfigurationManager.AppSettings("UserWelcome")
                lblUserLogged.Text = String.Format(welcomeMsg, Session("username").ToString().Trim(), Session("userid").ToString().Trim())
                hdWelcomeMess.Value = lblUserLogged.Text
                Response.Redirect("CustPaymentModule.aspx", False)
            End If

            If Not IsPostBack Then

                'ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "GetLocalDB()", True)

                'If Session("userid") IsNot Nothing Then
                '    Dim welcomeMsg = ConfigurationManager.AppSettings("UserWelcome")
                '    lblUserLogged.Text = String.Format(welcomeMsg, Session("username").ToString().Trim(), Session("userid").ToString().Trim())
                'Else
                '    If String.IsNullOrEmpty(url) Then
                '        FormsAuthentication.RedirectToLoginPage()
                '    End If
                'End If

            Else

            End If
        Catch ex As Exception

            Dim usr = If(Session("userid") IsNot Nothing, Session("userid").ToString(), "N/A")
            writeLog(strLogCadenaCabecera, Logs.ErrorTypeEnum.Exception, "An Exception occurs: " + ex.Message + " for the user: " + usr, " at time: " + DateTime.Now.ToString())
        End Try

    End Sub

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

    Public Sub SendMessage(methodMessage As String, detailInfo As String)
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & "')", True)
    End Sub

    Structure messageType
        Const success = "success"
        Const warning = "warning"
        Const info = "info"
        Const [Error] = "Error"
    End Structure

#Region "Logs"

    Public Sub writeLog(strLogCadenaCabecera As String, strLevel As Logs.ErrorTypeEnum, strMessage As String, strDetails As String)
        strLogCadena = strLogCadenaCabecera + " " + System.Reflection.MethodBase.GetCurrentMethod().ToString()
        Dim userid = If(DirectCast(Session("userid"), String) IsNot Nothing, DirectCast(Session("userid"), String), "N/A")
        objLog.WriteLog(strLevel, "CustomerPaymentsApp" & strLevel, strLogCadena, userid, strMessage, strDetails)
    End Sub

#End Region

End Class