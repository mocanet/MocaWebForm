Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Drawing
Imports System.Drawing.Design

<Assembly: TagPrefix("Moca.Web.UI", "Moca")> 
<Assembly: WebResource("Moca.debug.png", "img/png")> 

Namespace Web.UI

	''' <summary>
	''' デバッグ印
	''' </summary>
	''' <remarks></remarks>
	<DefaultProperty("ReleaseUrl") _
	, ToolboxData("<{0}:DebugManager runat=server></{0}:DebugManager>") _
	, ToolboxBitmap(GetType(resourceDummy), "debug.bmp") _
	, Designer(GetType(DebugManagerDesigner)) _
	> _
	Public Class DebugManager
		Inherits WebControl

		<Bindable(True) _
		, Category("Appearance") _
		, DefaultValue("") _
		, Description("本番サイトのＵＲＬを入力してください") _
		, Editor(GetType(System.Web.UI.Design.UrlEditor), GetType(UITypeEditor)) _
		> _
		Property ReleaseUrl() As String
			Get
				Dim s As String = CStr(ViewState("ReleaseUrl"))
				If s Is Nothing Then
					Return String.Empty
				Else
					Return s
				End If
			End Get
			Set(ByVal Value As String)
				ViewState("ReleaseUrl") = Value
			End Set
		End Property

		Protected Overrides Sub RenderContents(ByVal writer As System.Web.UI.HtmlTextWriter)
			Try
				If Me.Context IsNot Nothing Then
					If Not Me.Context.IsDebuggingEnabled Then
						Exit Sub
					End If
				End If

				Dim rsname As String = "Moca.debug.png"
				Dim rstype As Type = GetType(DebugManager)
				Dim cs As ClientScriptManager = Page.ClientScript
				Dim img As String = cs.GetWebResourceUrl(rstype, rsname)

				cs.RegisterClientScriptResource(rstype, rsname)

				writer.AddAttribute(HtmlTextWriterAttribute.Id, "Debug")
				writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "absolute")
				writer.AddStyleAttribute(HtmlTextWriterStyle.Top, "0px")
				writer.AddStyleAttribute(HtmlTextWriterStyle.Left, "0px")
				writer.RenderBeginTag(HtmlTextWriterTag.Div)
				writer.AddAttribute(HtmlTextWriterAttribute.Id, "DebugImg")
				writer.AddAttribute(HtmlTextWriterAttribute.Src, img)
				writer.AddAttribute(HtmlTextWriterAttribute.Class, "Debug")
				writer.AddAttribute("ondblclick", "window.open('" + ReleaseUrl + "')")
				writer.AddAttribute(HtmlTextWriterAttribute.Alt, "これはデバッグ環境のしるしです！！")
				writer.RenderBeginTag(HtmlTextWriterTag.Img)
				writer.RenderEndTag()
				writer.RenderEndTag()
			Catch ex As Exception
				writer.Write("Error building DebugSign:<br>")
				writer.Write(ex.Message)
			End Try
		End Sub

	End Class

End Namespace

