
Namespace Web.UI

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
	Public Class RadioButtonGroup

		'''' <summary>
		'''' 矢印ボタン押下時のボタン移動方法
		'''' </summary>
		'''' <remarks></remarks>
		'Private Enum MoveType
		'    forward = 0
		'    [Next]
		'End Enum

		''' <summary>ラジオボタン化するItemを格納する</summary>
		Private _aryButton As IList(Of RadioButton)

		Private _aryVal As IDictionary(Of String, Object)

		Public Sub New()
			_aryButton = New List(Of RadioButton)
			_aryVal = New Dictionary(Of String, Object)
		End Sub

#Region " イベント "

		'''' <summary>
		'''' ラジオボタンクリックイベント
		'''' </summary>
		'''' <param name="sender"></param>
		'''' <param name="e"></param>
		'''' <remarks></remarks>
		'Private Sub _radioButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
		'    SetSelected(DirectCast(sender, RadioButton))
		'End Sub

		'''' <summary>
		'''' ラジオボタンキーダウンイベント
		'''' </summary>
		'''' <param name="sender"></param>
		'''' <param name="e"></param>
		'''' <remarks></remarks>
		'Private Sub _radioButton_PreviewKeyDown(ByVal sender As Object, ByVal e As PreviewKeyDownEventArgs)
		'    Select Case e.KeyData
		'        Case Keys.Left, Keys.Up
		'            _setForcus(sender, MoveType.forward)
		'        Case Keys.Right, Keys.Down
		'            _setForcus(sender, MoveType.Next)
		'    End Select
		'End Sub

#End Region

		''' <summary>
		''' ラジオボタンを追加する
		''' </summary>
		''' <param name="ctrl">グループにしたいラジオボタン</param>
		''' <remarks>
		''' </remarks>
		Public Sub Add(ByRef ctrl As RadioButton)
			Dim lValue As Object = Nothing
			Add(ctrl, lValue)
		End Sub

		''' <summary>
		''' ラジオボタンを追加する
		''' </summary>
		''' <param name="ctrl">グループにしたいラジオボタン</param>
		''' <param name="value">保持したい値</param>
		''' <remarks>
		''' value にて指定された値を Tag プロパティにて保持します。
		''' </remarks>
		Public Sub Add(ByRef ctrl As RadioButton, ByVal value As Object)
			_aryButton.Add(ctrl)
			ctrl.Checked = False
			'AddHandler ctrl.c, AddressOf _radioButton_Click
			'AddHandler ctrl.PreviewKeyDown, AddressOf _radioButton_PreviewKeyDown

			If value Is Nothing Then
				Exit Sub
			End If

			_aryVal.Add(ctrl.ID, value)
		End Sub

		''' <summary>
		''' 現在選択されているラジオボタンコントロールを返す
		''' </summary>
		''' <returns></returns>
		''' <remarks>
		''' 未選択時は Nothing を返します。
		''' </remarks>
		Public Function GetSelected() As RadioButton
			For Each btn As RadioButton In _aryButton
				If btn.Checked Then
					Return btn
				End If
			Next

			Return Nothing
		End Function

		''' <summary>
		''' 現在選択されているラジオボタンコントロールの値を返す
		''' </summary>
		''' <returns></returns>
		''' <remarks>
		''' タグプロパティに値が設定されている事が前提ですので、<see cref="Add" /> メソッドは Value 指定してください。
		''' </remarks>
		Public Function GetSelectedValue() As Object
			Dim sel As RadioButton

			sel = GetSelected()
			If sel Is Nothing Then
				Return Nothing
			End If

			Return _aryVal.Item(sel.ID)
		End Function

		''' <summary>
		''' 現在選択されているラジオボタンコントロールの文字列を返す
		''' </summary>
		''' <returns></returns>
		''' <remarks>
		''' タグプロパティに値が設定されている事が前提ですので、<see cref="Add" /> メソッドは Value 指定してください。
		''' </remarks>
		Public Function GetSelectedText() As String
			Dim sel As RadioButton

			sel = GetSelected()
			If sel Is Nothing Then
				Return Nothing
			End If

			Return sel.Text
		End Function

		''' <summary>
		''' 指定された値のラジオボタンを選択する
		''' </summary>
		''' <param name="btn">選択したいRadioButtonを設定</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function SetSelected(ByVal btn As RadioButton) As RadioButton
			Dim radButton As RadioButton

			If Not btn.Checked Then
				btn.Checked = True
			End If

			For Each radButton In _aryButton
				If Not radButton.Equals(btn) Then
					radButton.Checked = False
				End If
			Next

			Return btn
		End Function

		''' <summary>
		''' 指定された値のラジオボタンを選択する
		''' </summary>
		''' <param name="value"></param>
		''' <returns></returns>
		''' <remarks>
		''' タグプロパティに値が設定されている事が前提ですので、<see cref="Add" /> メソッドは Value 指定してください。
		''' </remarks>
		Public Function SetSelected(ByVal value As Object) As RadioButton
			Dim btn As RadioButton

			Dim result As RadioButton = Nothing

			For Each btn In _aryButton
				If _aryVal.Item(btn.ID).Equals(value) Then
					btn.Checked = True
					result = btn
				Else
					btn.Checked = False
				End If
			Next

			Return result
		End Function

		''' <summary>
		''' 指定された値のラジオボタンが選択されているか返す
		''' </summary>
		''' <param name="value"></param>
		''' <returns></returns>
		''' <remarks>
		''' タグプロパティに値が設定されている事が前提ですので、<see cref="Add" /> メソッドは Value 指定してください。
		''' </remarks>
		Public Function IsSelected(ByVal value As Object) As Boolean
			Dim btn As RadioButton

			Dim result As RadioButton = Nothing

			For Each btn In _aryButton
				If _aryVal.Item(btn.ID).Equals(value) Then
					Return btn.Checked
				End If
			Next

			Return False
		End Function

		'''' <summary>
		'''' 指定されたラジオボタンから指定された方向へカーソルを移動する。
		'''' </summary>
		'''' <param name="sender"></param>
		'''' <param name="type"></param>
		'''' <remarks></remarks>
		'Private Sub _setForcus(ByVal sender As Object, ByVal type As MoveType)
		'    Dim btn As RadioButton
		'    Dim idx As Integer

		'    btn = DirectCast(sender, RadioButton)
		'    idx = 0

		'    For ii As Integer = 0 To _aryButton.Count - 1
		'        btn = _aryButton(ii)
		'        If sender.Equals(btn) Then
		'            If type = MoveType.forward Then
		'                idx = ii - 1
		'            Else
		'                idx = ii + 1
		'            End If
		'            Exit For
		'        End If
		'    Next

		'    If idx < 0 Then
		'        idx = _aryButton.Count - 1
		'    End If
		'    If idx >= _aryButton.Count Then
		'        idx = 0
		'    End If

		'    _aryButton(idx).Focus()
		'End Sub

	End Class

End Namespace
