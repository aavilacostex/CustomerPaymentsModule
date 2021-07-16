Imports System.IO

Public Class WebForm1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim exMessage As String = Nothing
        Try

            Dim path = ""
            Dim file As FileInfo = New FileInfo(path)
            Dim response As System.Web.HttpResponse = System.Web.HttpContext.Current.Response
            response.ClearContent()
            response.Clear()
            response.ContentType = "application/octet-stream"
            response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name + ";")
            response.AddHeader("Content-Length", file.Length.ToString())
            'response.TransmitFile(Server.MapPath("FileDownload.csv"))
            response.WriteFile(path)
            response.Flush()
            response.End()

        Catch ex As Exception
            exMessage = ex.Message
            Dim x = exMessage
        End Try
    End Sub

End Class