Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then

                If Session("UserDataText") IsNot Nothing Then
                    Dim welcomeMsg = ConfigurationManager.AppSettings("UserWelcome")
                    Dim userData = DirectCast(Session("UserDataText"), String)
                    lblUserLogged.Text = String.Format(welcomeMsg, userData.Split("-")(1).Trim(), userData.Split("-")(0).Trim())
                Else
                    FormsAuthentication.RedirectToLoginPage()
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