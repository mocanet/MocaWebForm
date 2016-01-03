

Namespace Web.UI

	''' <summary>
	''' BootstrapのAlertを管理する
	''' </summary>
	''' <remarks></remarks>
	Public Class BootstrapAlerts

#Region " Declare "

		Private Const C_BTN_ERROR As String = "AlertErrorButton"
		Private Const C_BTN_BLOCK As String = "AlertBlockButton"
		Private Const C_BTN_INFO As String = "AlertInfoButton"
		Private Const C_BTN_SUCCESS As String = "AlertSuccessButton"
		Private Const C_BTN As String = "×"

		Private _closeButtonValues As IDictionary(Of Panel, HiddenField)

		Private _parent As WebControl

		Protected WithEvents alertErrorPanel As Global.System.Web.UI.WebControls.Panel
		Protected WithEvents alertBlockPanel As Global.System.Web.UI.WebControls.Panel
		Protected WithEvents alertInfoPanel As Global.System.Web.UI.WebControls.Panel
		Protected WithEvents alertSuccessPanel As Global.System.Web.UI.WebControls.Panel

#End Region

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <param name="parent">Alertを表示する親のコントロール</param>
		''' <remarks></remarks>
		Public Sub New(ByVal parent As WebControl)
			_closeButtonValues = New Dictionary(Of Panel, HiddenField)()

			_parent = parent

			alertErrorPanel = New Panel
			alertErrorPanel.ID = "alertError"
			alertErrorPanel.Visible = False
			alertErrorPanel.CssClass = "alert alert-error"

			alertBlockPanel = New Panel
			alertBlockPanel.ID = "alertBlock"
			alertBlockPanel.Visible = False
			alertBlockPanel.CssClass = "alert alert-block"

			alertInfoPanel = New Panel
			alertInfoPanel.ID = "alertInfo"
			alertInfoPanel.Visible = False
			alertInfoPanel.CssClass = "alert alert-info"

			alertSuccessPanel = New Panel
			alertSuccessPanel.ID = "alertSuccess"
			alertSuccessPanel.Visible = False
			alertSuccessPanel.CssClass = "alert alert-success"

			_parent.Controls.Add(Me.AlertError)
			_parent.Controls.Add(Me.AlertBlock)
			_parent.Controls.Add(Me.AlertInfo)
			_parent.Controls.Add(Me.AlertSuccess)

			_addButtonValue(alertErrorPanel)
			_addButtonValue(alertBlockPanel)
			_addButtonValue(alertInfoPanel)
			_addButtonValue(alertSuccessPanel)

			AddHandler Me.AlertError.PreRender, AddressOf _preRender
			AddHandler Me.AlertInfo.PreRender, AddressOf _preRender
			AddHandler Me.AlertSuccess.PreRender, AddressOf _preRender
			AddHandler Me.AlertBlock.PreRender, AddressOf _preRender
		End Sub

		Private Sub _addButtonValue(ByVal alert As Panel)
			Dim hid As HiddenField = New HiddenField()
			hid.ID = alert.ID & "Val"

			Me.Parent.Controls.Add(hid)
			_closeButtonValues.Add(alert, hid)
		End Sub

#End Region

#Region " Property "

		''' <summary>
		''' メッセージを持っているか返す
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property HaveMessage As Boolean
			Get
				If Me.AlertError.Controls.Count > 0 Then
					Return True
				End If
				If Me.AlertInfo.Controls.Count > 0 Then
					Return True
				End If
				If Me.AlertSuccess.Controls.Count > 0 Then
					Return True
				End If
				If Me.AlertBlock.Controls.Count > 0 Then
					Return True
				End If
				Return False
			End Get
		End Property

		''' <summary>
		''' Alertを表示する親のコントロール
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Parent As WebControl
			Get
				Return _parent
			End Get
		End Property

		''' <summary>
		''' エラーメッセージ表示コントロール
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property AlertError As System.Web.UI.WebControls.Panel
			Get
				Return Me.alertErrorPanel
			End Get
		End Property

		''' <summary>
		''' 警告メッセージ表示コントロール
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property AlertBlock As System.Web.UI.WebControls.Panel
			Get
				Return Me.alertBlockPanel
			End Get
		End Property

		''' <summary>
		''' メッセージ表示コントロール
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property AlertInfo As System.Web.UI.WebControls.Panel
			Get
				Return Me.alertInfoPanel
			End Get
		End Property

		''' <summary>
		''' 正常メッセージ表示コントロール
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property AlertSuccess As System.Web.UI.WebControls.Panel
			Get
				Return Me.alertSuccessPanel
			End Get
		End Property

		Public Property AlertErrorButton As Boolean
			Get
				Dim btn As Button = DirectCast(AlertError.FindControl(C_BTN_ERROR), Button)
				Return (btn IsNot Nothing)
			End Get
			Set(value As Boolean)
				closeButton(value, AlertError, C_BTN_ERROR)
			End Set
		End Property

		Public Property AlertBlockButton As Boolean
			Get
				Dim btn As Button = DirectCast(AlertBlock.FindControl(C_BTN_BLOCK), Button)
				Return (btn IsNot Nothing)
			End Get
			Set(value As Boolean)
				closeButton(value, AlertBlock, C_BTN_BLOCK)
			End Set
		End Property

		Public Property AlertInfoButton As Boolean
			Get
				Dim btn As Button = DirectCast(AlertInfo.FindControl(C_BTN_INFO), Button)
				Return (btn IsNot Nothing)
			End Get
			Set(value As Boolean)
				closeButton(value, AlertInfo, C_BTN_INFO)
			End Set
		End Property

		Public Property AlertSuccessButton As Boolean
			Get
				Dim btn As Button = DirectCast(AlertSuccess.FindControl(C_BTN_SUCCESS), Button)
				Return (btn IsNot Nothing)
			End Get
			Set(value As Boolean)
				closeButton(value, AlertSuccess, C_BTN_SUCCESS)
			End Set
		End Property

#End Region
#Region " Methods "

		''' <summary>
		''' エラーメッセージ追加
		''' </summary>
		''' <param name="message"></param>
		''' <remarks></remarks>
		Public Sub [Error](ByVal message As String)
			_setMessage(message, AlertError)
		End Sub

		''' <summary>
		''' 警告メッセージ追加
		''' </summary>
		''' <param name="message"></param>
		''' <remarks></remarks>
		Public Sub Block(ByVal message As String)
			_setMessage(message, AlertBlock)
		End Sub

		''' <summary>
		''' メッセージ追加
		''' </summary>
		''' <param name="message"></param>
		''' <remarks></remarks>
		Public Sub Info(ByVal message As String)
			_setMessage(message, AlertInfo)
		End Sub

		''' <summary>
		''' 正常メッセージ追加
		''' </summary>
		''' <param name="message"></param>
		''' <remarks></remarks>
		Public Sub Success(ByVal message As String)
			_setMessage(message, AlertSuccess)
		End Sub

		Private Sub closeButton(ByVal value As Boolean, ByVal alert As Panel, ByVal id As String)
			Dim hid As HiddenField = Nothing
			Dim btn As Literal = DirectCast(alert.FindControl(id), Literal)

			If value Then
				If btn IsNot Nothing Then
					Return
				End If
				btn = New Literal
				btn.ID = id
				alert.Controls.Add(btn)
				If _closeButtonValues.TryGetValue(alert, hid) Then
					btn.Text = String.Format("<button class='close' type='button' onclick='$(""#{0}"").val(1);' data-dismiss='alert'>{1}</button>", hid.ClientID, C_BTN)
				End If
			Else
				alert.Controls.Remove(btn)
			End If
		End Sub

		Private Sub _setMessage(ByVal message As String, ByVal ctrl As Panel)
			If ctrl Is Nothing Then
				Return
			End If

			Dim lbl As Label = New Label()
			lbl.Text = message
			lbl.Attributes.Add("style", "display: block")
			ctrl.Controls.Add(lbl)
			ctrl.Visible = True

			Me._parent.Visible = True
		End Sub

		Private Sub _preRender(sender As Object, e As System.EventArgs)
			Dim ctrl As WebControl = DirectCast(sender, WebControl)
			If ctrl.Controls.Count = 0 Then
				ctrl.Visible = False
				Return
			End If
			Dim hid As HiddenField = Nothing
			If Not _closeButtonValues.TryGetValue(DirectCast(ctrl, Panel), hid) Then
				Return
			End If
			ctrl.Visible = Not (hid.Value = "1")
		End Sub

#End Region

	End Class

End Namespace
