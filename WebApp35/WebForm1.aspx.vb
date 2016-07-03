
Public Class WebForm1
    Inherits Moca.Web.UI.MocaPage

    <Moca.Attr.Implementation(GetType(Test))>
    Protected tes As ITest

    Protected mySession As ISession

    Protected vs As IVeweState

    <Moca.Web.Attr.Cookie(Moca.Web.Attr.CookieType.Request)>
    Protected cookieReq As ICookie

    <Moca.Web.Attr.Cookie(Moca.Web.Attr.CookieType.Response)>
    Protected cookieRes As ICookie

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim data As String = String.Empty
        If cookieReq.Data IsNot Nothing Then
            data = cookieReq.Data.Value
        End If

        Me.Label1.Text = String.Format("[ITest]={0}", tes.Test)
        Me.Label2.Text = String.Format("[ISession]={0}", mySession.Data)
        Me.Label3.Text = String.Format("[IVeweState]={0}", vs.Data)
        Me.Label4.Text = String.Format("[ICookie]={0}", data)
        mySession.Data &= " Hoge"
        If vs.Data Is Nothing Then
            vs.Data = "0"
        End If
        vs.Data = CInt(vs.Data) + 1
        If cookieReq.Data Is Nothing Then
            data = "0"
        End If
        cookieRes.Data.Value = CInt(data) + 100
    End Sub

End Class

Public Interface ITest

    Function Test() As String

End Interface

Public Class Test
    Inherits MarshalByRefObject
    Implements ITest

    Private Function ITest_Test() As String Implements ITest.Test
        Return "Test Return"
    End Function

End Class

<Moca.Web.Attr.Session()>
Public Interface ISession

    <Moca.Web.Attr.SessionName("d")>
    Property Data As String

End Interface

<Moca.Web.Attr.ViewState()>
Public Interface IVeweState

    <Moca.Web.Attr.ViewStateName("d")>
    Property Data As String

End Interface

Public Interface ICookie

    <Moca.Web.Attr.CookieName("d")>
    Property Data As HttpCookie

End Interface
