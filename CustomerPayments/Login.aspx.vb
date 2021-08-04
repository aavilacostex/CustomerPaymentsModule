Imports System.DirectoryServices
Imports System.DirectoryServices.AccountManagement

Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("userid") IsNot Nothing Then
            Response.Redirect("CustPaymentModule.aspx", False)
        End If

        Try
            'If Not IsPostBack Then
            '    If Request.QueryString.Count > 0 And Request.QueryString("data") IsNot Nothing Then
            '        Dim prevUrl = Request.QueryString("data").ToString()
            '        If Not String.IsNullOrEmpty(prevUrl) Then
            '            lblOptionalMessage.Text = prevUrl
            '        Else
            '            lblOptionalMessage.Text = ""
            '        End If
            '    End If
            'Else

            'End If
        Catch ex As Exception

        End Try

    End Sub

    Structure messageType
        Const success = "success"
        Const warning = "warning"
        Const info = "info"
        Const [Error] = "Error"
    End Structure

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        Dim methodMessage As String = Nothing
        Dim sr As SearchResult = Nothing
        Dim dct As Dictionary(Of String, String) = New Dictionary(Of String, String)()
        Dim strLdap As String = Nothing
        Try
            Dim user = UserName.Text.Trim()
            Dim pass = Password.Text.Trim()

            'Using Principal Context

#Region "User Principal"

            Dim blFlag = getLDAPConnectionString("", strLdap, False, True)
            If blFlag Then

                Using pc As PrincipalContext = New PrincipalContext(ContextType.Domain, strLdap, user, ContextOptions.SimpleBind.ToString())
                    Dim validCred = pc.ValidateCredentials(user, pass)
                    If validCred Then
                        Dim strUser = UserPrincipal.FindByIdentity(New PrincipalContext(ContextType.Domain, strLdap), IdentityType.SamAccountName, user)
                        'BuildUserSearcherFromPrincipal(strUser)

                        If strUser IsNot Nothing Then
                            Session("UserPrincipal") = strUser
                            Session("username") = strUser.Name
                            Session("userid") = strUser.SamAccountName.ToUpper()

                            FormsAuthentication.RedirectFromLoginPage(UserName.Text, False)
                        Else
                            lblOptionalMessage.Text = "There is an error getting data for the user: " + user
                        End If
                    Else
                        lblOptionalMessage.Text = "The user name or password is incorrect."
                    End If
                End Using

            Else
                lblOptionalMessage.Text = "There is an error getting data for Company Domain: Costex.com"
            End If

#End Region

            'using Directory Services

#Region "Directory Services"

            'If AuthenticateUser(user, pass, sr) Then

            '    If sr IsNot Nothing Then

            '        Dim propNames = sr.Properties.PropertyNames
            '        Dim strValue As String = Nothing
            '        For Each prp As String In propNames
            '            strValue = If(sr.Properties(prp).Count > 0, sr.Properties(prp)(0).ToString(), Nothing)
            '            dct.Add(prp, strValue)
            '        Next

            '        Session("UserLoginData") = dct

            '    End If

            '    Session("UserDataText") = dct("samaccountname").ToString().ToUpper() + " - " + dct("name").ToString()
            '    Session("userid") = dct("samaccountname").ToString().ToUpper()

            '    FormsAuthentication.RedirectFromLoginPage(UserName.Text, False)
            '    'Response.Redirect("Default.aspx", False)
            'Else

            '    Session("userid") = Nothing

            '    methodMessage = "There is an error in the credential validation for the user: " + user.ToUpper()
            '    SendMessage(methodMessage, messageType.info)
            'End If

#End Region

            'SearchOneUser(user)
        Catch ex As Exception

        End Try
    End Sub

    Private Function getLDAPConnectionString(defSearch As String, ByRef strLdap As String, Optional flag As Boolean = False, Optional method As Boolean = False) As Boolean
        Dim blResult As Boolean = False
        strLdap = Nothing
        'method = false(Directory Services), true(User Principal)
        Try
            Dim de As DirectoryEntry = New DirectoryEntry("LDAP://RootDSE")
            If de IsNot Nothing Then

                If Not method Then
                    If flag Then
                        strLdap = "LDAP://" + de.Properties("defaultNamingContext")(0).ToString()
                    Else
                        strLdap = de.Properties("defaultNamingContext")(0).ToString()
                    End If
                Else
                    strLdap = de.Properties("ldapServiceName")(0).ToString().Split("@")(1).ToString().Trim()
                End If

                'strLdap = If(flag, strLdap = "LDAP://" + de.Properties("defaultNamingContext")(0).ToString(), strLdap = de.Properties("defaultNamingContext")(0).ToString())
                blResult = If(String.IsNullOrEmpty(strLdap), False, True)
            End If

            Return blResult
        Catch ex As Exception
            Dim msg = ex.Message
            Return blResult
        End Try
    End Function

    Public Function ValidateCredentials(userName As String, password As String, ldap As String) As Boolean
        Dim blResult As Boolean = False
        Try
            Dim de As DirectoryEntry = New DirectoryEntry(ldap, userName, password)
            Dim dSearch As DirectorySearcher = New DirectorySearcher(de)
            Dim results As SearchResult = Nothing

            results = dSearch.FindOne()
            blResult = True
            Return blResult
        Catch ex As Exception
            Return blResult
        End Try
    End Function

    Public Function AuthenticateUser(userName As String, password As String, ByRef sr As SearchResult) As Boolean
        Dim blResult As Boolean = False
        Dim strLdap As String = Nothing
        Dim dSearch As DirectorySearcher = Nothing
        Dim results As SearchResult = Nothing
        Try
            Dim blFlag = getLDAPConnectionString("", strLdap, True)
            If blFlag Then

                Dim validCred = ValidateCredentials(userName, password, strLdap)
                If validCred Then

                    Dim de As DirectoryEntry = New DirectoryEntry(strLdap)
                    If de IsNot Nothing Then
                        dSearch = BuildUserSearcher(de)
                        dSearch.Filter = "(&(objectCategory=User)(objectClass=person)(sAMAccountName=" + userName + "*))"
                        sr = dSearch.FindOne()
                    Else
                        'not user found
                        sr = Nothing
                        lblOptionalMessage.Text = "There is an error getting data for the user: " + userName
                    End If
                    blResult = True
                Else
                    'not valid cred
                    sr = Nothing
                    lblOptionalMessage.Text = "The user name or password is incorrect."
                End If
            Else
                'not valid domain
                lblOptionalMessage.Text = "There is an error getting data for Company Domain: Costex.com"
                sr = Nothing
            End If

            Return blResult
        Catch ex As Exception
            sr = Nothing
            Dim msg = ex.Message
            lblOptionalMessage.Text = "An error has occurred."
            Return blResult
        End Try
    End Function

    Private Function BuildUserSearcherFromPrincipal(usr As UserPrincipal) As Dictionary(Of String, String)
        'create dictionary to work with the user data
        Try

        Catch ex As Exception

        End Try
    End Function

    Private Function BuildUserSearcher(de As DirectoryEntry) As DirectorySearcher
        Dim ds As DirectorySearcher = Nothing
        Try
            ds = New DirectorySearcher(de)
            ds.PropertiesToLoad.Add("name")
            ds.PropertiesToLoad.Add("mail")
            ds.PropertiesToLoad.Add("givenname")
            ds.PropertiesToLoad.Add("userPrincipalName")
            ds.PropertiesToLoad.Add("distinguishedName")
            ds.PropertiesToLoad.Add("department")
            ds.PropertiesToLoad.Add("description")
            ds.PropertiesToLoad.Add("userPassword")
            ds.PropertiesToLoad.Add("telephoneNumber")
            ds.PropertiesToLoad.Add("homePhone")
            ds.PropertiesToLoad.Add("mobile")
            ds.PropertiesToLoad.Add("personalTitle")
            ds.PropertiesToLoad.Add("title")
            ds.PropertiesToLoad.Add("sAMAccountName")
            ds.PropertiesToLoad.Add("sAMAccountType")
            ds.PropertiesToLoad.Add("postalAddress")
            ds.PropertiesToLoad.Add("postalCode")
            ds.PropertiesToLoad.Add("streetAddress")
            ds.PropertiesToLoad.Add("thumbnailPhoto")
            ds.PropertiesToLoad.Add("sn")

            Return ds

        Catch ex As Exception
            Dim msg = ex.Message
            Return ds
        End Try
    End Function

    Public Sub SendMessage(methodMessage As String, detailInfo As String)
        ScriptManager.RegisterStartupScript(Me, Page.GetType, "Message", "messageFormSubmitted('" & methodMessage & " ', '" & detailInfo & "')", True)
    End Sub

    'Protected Sub Validate_User(sender As Object, e As EventArgs)
    '    Dim user = lgAuthentication.UserName
    '    'txtUser.Text.Trim()
    '    Dim pass = lgAuthentication.Password
    '    lgAuthentication.FailureText = ""
    '    'txtPass.Text.Trim()
    '    Try
    '        If AuthenticateUser(user, pass) Then

    '            Dim lgControl = DirectCast(Master.FindControl("lgLoginName"), LoginName)
    '            lgControl.FormatString = "pepe"

    '            Response.Redirect("Default.aspx", False)
    '        Else
    '            lgAuthentication.FailureText = "Username and/or password is incorrect."
    '            'FormsAuthentication.RedirectFromLoginPage(user, lgAuthentication.RememberMeSet)
    '        End If
    '    Catch ex As Exception
    '        Dim msg = ex.Message
    '    End Try

    'End Sub

    Public Sub setFieldValue(ctr As Control, str As String)
        Try
            Dim myCtr = DirectCast(ctr, Label)
            myCtr.Text = str
        Catch ex As Exception
            Dim pp = ex.Message
        End Try
    End Sub

End Class