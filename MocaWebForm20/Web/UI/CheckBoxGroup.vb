
Namespace Web.UI

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
	Public Class CheckBoxGroup

		''' <summary>チェックボックス化するItemを格納する</summary>
		Private _aryButton As IList(Of CheckBox)

		Private _aryVal As IDictionary(Of String, Object)

		Public Sub New()
			_aryButton = New List(Of CheckBox)
			_aryVal = New Dictionary(Of String, Object)
		End Sub

		''' <summary>
		''' チェックボックスを追加する
		''' </summary>
		''' <param name="ctrl">グループにしたいチェックボックス</param>
		''' <remarks>
		''' </remarks>
		Public Sub Add(ByRef ctrl As CheckBox)
			Dim lValue As Object = Nothing
			Add(ctrl, lValue)
		End Sub

		''' <summary>
		''' チェックボックスを追加する
		''' </summary>
		''' <param name="ctrl">グループにしたいチェックボックス</param>
		''' <param name="value">保持したい値</param>
		''' <remarks>
		''' value にて指定された値を Tag プロパティにて保持します。
		''' </remarks>
		Public Sub Add(ByRef ctrl As CheckBox, ByVal value As Object)
			_aryButton.Add(ctrl)
			ctrl.Checked = False

			' イベントフックするときはここで
			'AddHandler ctrl.c, AddressOf _radioButton_Click

			If value Is Nothing Then
				Exit Sub
			End If

			_aryVal.Add(ctrl.ID, value)
		End Sub

		''' <summary>
		''' 現在選択されているチェックボックスコントロールを返す
		''' </summary>
		''' <returns></returns>
		''' <remarks>
		''' 未選択時は Length=0 を返します。
		''' </remarks>
		Public Function GetSelected() As CheckBox()
			Dim aryLst As ArrayList = New ArrayList

			For Each btn As CheckBox In _aryButton
				If btn.Checked Then
					aryLst.Add(btn)
				End If
			Next

			Return DirectCast(aryLst.ToArray(GetType(CheckBox)), CheckBox())
		End Function

		''' <summary>
		''' 現在選択されているチェックボックスコントロールの値を返す
		''' </summary>
		''' <returns></returns>
		''' <remarks>
		''' タグプロパティに値が設定されている事が前提ですので、<see cref="Add" /> メソッドは Value 指定してください。
		''' </remarks>
		Public Function GetSelectedValue() As Object()
			Dim sel() As CheckBox
			Dim aryLst As ArrayList = New ArrayList

			sel = GetSelected()
			For Each chk As CheckBox In sel
				aryLst.Add(_aryVal.Item(chk.ID))
			Next

			Return aryLst.ToArray()
		End Function

		''' <summary>
		''' 現在選択されているチェックボックスコントロールの文字列を返す
		''' </summary>
		''' <returns></returns>
		''' <remarks>
		''' タグプロパティに値が設定されている事が前提ですので、<see cref="Add" /> メソッドは Value 指定してください。
		''' </remarks>
		Public Function GetSelectedText() As String()
			Dim sel() As CheckBox
			Dim aryLst As ArrayList = New ArrayList

			sel = GetSelected()
			For Each chk As CheckBox In sel
				aryLst.Add(_aryVal.Item(chk.Text))
			Next

			Return DirectCast(aryLst.ToArray(GetType(String)), String())
		End Function

		''' <summary>
		''' 指定された値のチェックボックスを選択する
		''' </summary>
		''' <param name="btn">選択したいCheckBoxを設定</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function SetSelected(ByVal btn As CheckBox) As CheckBox
			Dim radButton As CheckBox

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
		''' 指定された値のチェックボックスを選択する
		''' </summary>
		''' <param name="value"></param>
		''' <returns></returns>
		''' <remarks>
		''' タグプロパティに値が設定されている事が前提ですので、<see cref="Add" /> メソッドは Value 指定してください。
		''' </remarks>
		Public Function SetSelected(ByVal value As Object) As CheckBox
			Dim btn As CheckBox

			Dim result As CheckBox = Nothing

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

		Public Sub SetSelected(ByVal values As ICollection)
			Dim btn As CheckBox
			Dim chk As CheckBox = New CheckBox

			For Each value As Object In values
				For Each btn In _aryButton
					If Not _aryVal.Item(btn.ID).Equals(value) Then
						Continue For
					End If
					btn.Checked = True
				Next
			Next
		End Sub

		''' <summary>
		''' 指定された値のチェックボックスが選択されているか返す
		''' </summary>
		''' <param name="value"></param>
		''' <returns></returns>
		''' <remarks>
		''' タグプロパティに値が設定されている事が前提ですので、<see cref="Add" /> メソッドは Value 指定してください。
		''' </remarks>
		Public Function IsSelected(ByVal value As Object) As Boolean
			Dim btn As CheckBox

			Dim result As CheckBox = Nothing

			For Each btn In _aryButton
				If _aryVal.Item(btn.ID).Equals(value) Then
					Return btn.Checked
				End If
			Next

			Return False
		End Function

		''' <summary>
		''' 選択されている項目のタイトルを指定された区切り文字で連結して返す
		''' </summary>
		''' <param name="delimiter"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function ToStringText(Optional ByVal delimiter As String = ", ") As String
			Dim chks() As CheckBox
			Dim sb As StringBuilder = New StringBuilder

			chks = GetSelected()
			For Each chk As CheckBox In chks
				sb.Append(IIf(sb.Length = 0, String.Empty, delimiter))
				sb.Append(chk.Text)
			Next

			Return sb.ToString
		End Function

	End Class

End Namespace
