Imports System.Web.UI
Imports System.String

Namespace Web.UI

	Friend Class DebugManagerDesigner
		Inherits ControlDesigner

		Protected ctrl As DebugManager

		Public Overrides Sub Initialize(ByVal component As System.ComponentModel.IComponent)
			If Not TypeOf component Is DebugManager Then
				Exit Sub
			End If
			MyBase.Initialize(component)
			ctrl = DirectCast(component, DebugManager)
		End Sub

		Public Overrides Function GetDesignTimeHtml() As String
			Dim rsname As String = "Moca.debug.png"
			Dim rstype As Type = GetType(DebugManagerDesigner)
			Dim cs As ClientScriptManager = ctrl.Page.ClientScript
			Dim img As String = cs.GetWebResourceUrl(rstype, rsname)

			cs.RegisterClientScriptResource(rstype, rsname)

			Dim sb As StringBuilder

			sb = New StringBuilder()

			sb.Append("<div id=""" + ctrl.ID + """ style=""position:absolute;top:0px;left:0px;font-size:9px;"">")
			sb.Append("<img id=""DebugImg"" src=""" + img + """ class=""Debug"" alt=""これはデバッグ環境のしるしです！！"" style=""vertical-align:text-top;"" />")
			sb.Append("Debug Manager : " + ctrl.ID)
			sb.Append("</div>")

			Return sb.ToString
		End Function

	End Class

End Namespace
