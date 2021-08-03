Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'Session("userid") = Nothing 'forcing session timeout
        Dim url As String = Nothing
        Try

            If Session("userid") Is Nothing Then
                url = String.Format("Login.aspx?data={0}", "Session Expired!")
                Response.Redirect(url, False)
            End If

            If Not IsPostBack Then

                If Session("userid") IsNot Nothing Then
                    Dim welcomeMsg = ConfigurationManager.AppSettings("UserWelcome")
                    lblUserLogged.Text = String.Format(welcomeMsg, Session("username").ToString().Trim(), Session("userid").ToString().Trim())
                Else
                    If String.IsNullOrEmpty(url) Then
                        FormsAuthentication.RedirectToLoginPage()
                    End If
                End If

            End If
        Catch ex As Exception

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

End Class