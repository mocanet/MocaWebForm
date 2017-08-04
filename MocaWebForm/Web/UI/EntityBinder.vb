
Imports System.Reflection

Imports Moca.Attr
Imports Moca.Util

Namespace Web.UI

	''' <summary>
	''' エンティティバインダー
	''' </summary>
	''' <remarks></remarks>
	Public Class EntityBinder

#Region " Declare "

		''' <summary></summary>
		Private Const C_MSG As String = "'{0}.{1}.{2}' へ '{3}.{4}' を設定できませんでした。"
		''' <summary></summary>
		Private Const C_MSG2 As String = "'{0}.{1}' へ '{2}.{3}.{4}' を設定できませんでした。"

		''' <summary>検証</summary>
		Private _validator As Validator

#Region " _entityBindInfo "

		''' <summary>
		''' 検証時のデータたち
		''' </summary>
		''' <remarks></remarks>
		Private Class _entityBindInfo

			Private _columnNames As IDictionary(Of String, String)

			Private _errorColumns As IList(Of String)

#Region " コンストラクタ "

			Public Sub New(ByVal bind As Object, ByVal entity As Object)
				Me.BindTarget = bind
				Me.EntityTarget = entity

				Me.EntityPropertyInfos = ClassUtil.GetProperties(Me.EntityTarget)
				Me.UpdateEntityStop = False
				Me.TableDefinition = Nothing
				Me.IsValid = True

				_errorColumns = New List(Of String)
			End Sub

#End Region

#Region " Property "

			Property BindTarget As Object
			Property EntityTarget As Object

			Property EntityPropertyInfos As PropertyInfo()

			Property EntityPropertyInfo As PropertyInfo
			Property ControlPropertyInfo As PropertyInfo
			Property ControlAttribute As BindControlAttribute

			Property Control As Object

			Property Value As Object

			Property ValidateMethod As UpdateEntityValidate

			Property TableDefinition As Object
			Property TableDefinitionFieldInfo As FieldInfo

			Property UpdateEntityStop As Boolean

			Property IsValid As Boolean

			ReadOnly Property IsError(ByVal caption As String) As Boolean
				Get
					Return _errorColumns.Contains(caption)
				End Get
			End Property

			ReadOnly Property ErrorColumns As IList(Of String)
				Get
					Return _errorColumns
				End Get
			End Property

			ReadOnly Property TableDefinitionColumn(ByVal name As String) As Moca.Db.DbInfoColumn
				Get
					If Me.TableDefinition Is Nothing Then
						Return Nothing
					End If

					If _columnNames Is Nothing Then
						_getColumnNames()
					End If

					Dim columnName As String = String.Empty
					If Not _columnNames.TryGetValue(name, columnName) Then
						Return Nothing
					End If

					Dim column As Moca.Db.DbInfoColumn = Nothing
					column = TryCast(Me.TableDefinitionFieldInfo.FieldType.InvokeMember(columnName, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, Me.TableDefinition, New Object() {}), Moca.Db.DbInfoColumn)
					Return column
				End Get
			End Property

#End Region
#Region " Methods "

			Public Sub AddError(ByVal caption As String)
				_errorColumns.Add(caption)
			End Sub

			Private Sub _getColumnNames()
				_columnNames = New Dictionary(Of String, String)

				Dim props() As PropertyInfo = Me.TableDefinitionFieldInfo.FieldType.GetProperties()
				For Each prop As PropertyInfo In props
					Dim attrColumn As Moca.Db.Attr.ColumnAttribute = ClassUtil.GetCustomAttribute(Of Moca.Db.Attr.ColumnAttribute)(prop)
					Dim columnName As String = prop.Name
					If attrColumn IsNot Nothing Then
						columnName = attrColumn.ColumnName
					End If
					_columnNames.Add(columnName, prop.Name)
				Next
			End Sub

#End Region

		End Class

#End Region

#End Region

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
			_validator = New Validator
		End Sub

#End Region

#Region " Methods "

		''' <summary>
		''' コントロールに対してエンティティをバインドする
		''' </summary>
		''' <param name="bindTarget"></param>
		''' <param name="entity"></param>
		''' <remarks></remarks>
		Public Sub BindEntity(ByVal bindTarget As Object, ByVal entity As Object)
			Dim value As Object
			Dim bindInfo As _entityBindInfo = New _entityBindInfo(bindTarget, entity)

			For Each entityPropertyInfo As PropertyInfo In bindInfo.EntityPropertyInfos
				bindInfo.EntityPropertyInfo = entityPropertyInfo
				' エンティティのBindControl属性を取得
				bindInfo.ControlAttribute = _getAttrBindControl(bindInfo.EntityPropertyInfo)
				If bindInfo.ControlAttribute Is Nothing Then
					' BindControl属性無しは次へ
					Continue For
				End If

				' エンティティの値を取得
				value = _getEntityValue(bindInfo.EntityTarget, bindInfo.EntityPropertyInfo)

				' エンティティのBindControl属性分繰り返す
				For Each controlName As String In bindInfo.ControlAttribute.ControlName
					' ターゲットのコントロールプロパティを取得
					bindInfo.ControlPropertyInfo = ClassUtil.GetProperties(bindInfo.BindTarget.GetType, controlName)
					If bindInfo.ControlPropertyInfo Is Nothing Then
						Continue For
						'If value Is Nothing Then
						'	' コントロールが存在せず、エンティティの値も無いときは次へ
						'	Continue For
						'End If
						'' エンティティの値が存在するときは、RadioButtonのケースとして再度コントロール取得
						'bindInfo.ControlPropertyInfo = ClassUtil.GetProperties(bindInfo.BindTarget.GetType, controlName & Value.ToString)
						'If bindInfo.ControlPropertyInfo Is Nothing Then
						'	' それでもないときは次へ
						'	Continue For
						'End If
					End If

					' コントロールを取得
					bindInfo.Control = _getBindControl(bindInfo.BindTarget, bindInfo.ControlPropertyInfo)

					' 値を設定
					If TypeOf bindInfo.Control Is TextBox Then
						bindInfo.Value = value
						If CType(bindInfo.Control, TextBox).TextMode = TextBoxMode.Password Then
							_setTargetPropertyPassword(bindInfo)
						Else
							_setTargetProperty(bindInfo, "Text")
						End If
					ElseIf TypeOf bindInfo.Control Is HiddenField Then
						bindInfo.Value = value
						_setTargetProperty(bindInfo, "Value")
					ElseIf TypeOf bindInfo.Control Is Label Then
						bindInfo.Value = value
						_setTargetProperty(bindInfo, "Text")
					ElseIf TypeOf bindInfo.Control Is DropDownList Then
						bindInfo.Value = value
						_setTargetPropertySelectedValue(bindInfo)
					ElseIf TypeOf bindInfo.Control Is ListBox Then
						bindInfo.Value = value
						_setTargetPropertySelectedValue(bindInfo)
					ElseIf TypeOf bindInfo.Control Is RadioButtonList Then
						bindInfo.Value = value
						_setTargetPropertySelectedValue(bindInfo)
					ElseIf TypeOf bindInfo.Control Is RadioButton Then
						bindInfo.Value = (bindInfo.ControlPropertyInfo.Name.IndexOf(value.ToString) >= 0)
						_setTargetProperty(bindInfo, "Checked")
					ElseIf TypeOf bindInfo.Control Is CheckBox Then
						bindInfo.Value = value
						_setTargetProperty(bindInfo, "Checked")
					ElseIf TypeOf bindInfo.Control Is CheckBoxList Then
						bindInfo.Value = value
						_setTargetPropertySelectedValues(bindInfo)
					ElseIf TypeOf bindInfo.Control Is LinkButton Then
						bindInfo.Value = value
						_setTargetProperty(bindInfo, "Text")
					ElseIf TypeOf bindInfo.Control Is Button Then
						bindInfo.Value = value
						_setTargetProperty(bindInfo, "Text")
					ElseIf TypeOf bindInfo.Control Is HyperLink Then
						bindInfo.Value = value
						_setTargetProperty(bindInfo, "Text")
					ElseIf TypeOf bindInfo.Control Is Calendar Then
						bindInfo.Value = value
						_setTargetPropertySelectedDate(bindInfo)
					End If
				Next
			Next
		End Sub

		''' <summary>
		''' エンティティに対してコントロールの値を設定する
		''' </summary>
		''' <param name="bindTarget"></param>
		''' <param name="entity"></param>
		''' <param name="validateMethod"></param>
		''' <remarks></remarks>
		Public Function UpdateEntity(ByVal bindTarget As Object, ByVal entity As Object, Optional ByVal validateMethod As UpdateEntityValidate = Nothing) As Boolean
			Dim bindInfo As _entityBindInfo = New _entityBindInfo(bindTarget, entity)
			bindInfo.ValidateMethod = validateMethod

			'テーブル定義を取得
			_getTableDefinition(bindInfo)

			' エンティティのプロパティ数分繰り返す
			For Each entityPropertyInfo As PropertyInfo In bindInfo.EntityPropertyInfos
				bindInfo.EntityPropertyInfo = entityPropertyInfo
				' エンティティのBindControl属性を取得
				bindInfo.ControlAttribute = _getAttrBindControl(bindInfo.EntityPropertyInfo)
				If bindInfo.ControlAttribute Is Nothing Then
					' エンティティの値を取得
					bindInfo.Value = _getEntityValue(bindInfo.EntityTarget, bindInfo.EntityPropertyInfo)
					' バインド先は無くても検証があればやる
					_validate(bindInfo)
					If bindInfo.UpdateEntityStop Then
						Return bindInfo.IsValid
					End If
					' BindControl属性無しは次へ
					Continue For
				End If

				Dim setFlag As Boolean = False

				' エンティティのBindControl属性分繰り返す
				For Each controlName As String In bindInfo.ControlAttribute.ControlName
					' ターゲットのプロパティを取得
					bindInfo.ControlPropertyInfo = ClassUtil.GetProperties(bindInfo.BindTarget.GetType, controlName)
					If bindInfo.ControlPropertyInfo Is Nothing Then
						' コントロールが無いときは次へ
						Continue For
					End If

					' コントロール自体を取得
					bindInfo.Control = _getBindControl(bindInfo.BindTarget, bindInfo.ControlPropertyInfo)
					If bindInfo.Control Is Nothing Then
						Continue For
					End If

					' 値を設定
					If TypeOf bindInfo.Control Is TextBox Then
						_setEntiryProperty(bindInfo, "Text")
						setFlag = True
					ElseIf TypeOf bindInfo.Control Is HiddenField Then
						_setEntiryProperty(bindInfo, "Value")
						setFlag = True
					ElseIf TypeOf bindInfo.Control Is DropDownList Then
						_setEntiryPropertySelectedValue(bindInfo)
						setFlag = True
					ElseIf TypeOf bindInfo.Control Is ListBox Then
						_setEntiryPropertySelectedValue(bindInfo)
						setFlag = True
					ElseIf TypeOf bindInfo.Control Is RadioButtonList Then
						_setEntiryPropertySelectedValue(bindInfo)
						setFlag = True
					ElseIf TypeOf bindInfo.Control Is RadioButton Then
						_setEntiryPropertyRadioButtonChecked(bindInfo)
						setFlag = True
					ElseIf TypeOf bindInfo.Control Is CheckBox Then
						_setEntiryProperty(bindInfo, "Checked")
						setFlag = True
					ElseIf TypeOf bindInfo.Control Is CheckBoxList Then
						_setEntiryPropertySelectedValues(bindInfo)
						setFlag = True
					ElseIf TypeOf bindInfo.Control Is FileUpload Then
						If bindInfo.EntityPropertyInfo.PropertyType.Equals(GetType(String)) Then
							_setEntiryProperty(bindInfo, "FileName")
						ElseIf bindInfo.EntityPropertyInfo.PropertyType.Equals(GetType(Byte())) Then
							_setEntiryProperty(bindInfo, "FileBytes")
						ElseIf bindInfo.EntityPropertyInfo.PropertyType.Equals(GetType(System.IO.Stream)) Then
							_setEntiryProperty(bindInfo, "FileContent")
						End If
						setFlag = True
					End If
					If Not bindInfo.UpdateEntityStop Then
						Continue For
					End If

					Return bindInfo.IsValid
				Next
				If Not setFlag Then
					' エンティティの値を取得
					bindInfo.Value = _getEntityValue(bindInfo.EntityTarget, bindInfo.EntityPropertyInfo)
					' バインド先は無くても検証があればやる
					_validate(bindInfo)
					If bindInfo.UpdateEntityStop Then
						Return bindInfo.IsValid
					End If
				End If
			Next
			Return bindInfo.IsValid
		End Function

#Region " Private "

		Private Sub _getTableDefinition(ByVal bindInfo As _entityBindInfo)
			If bindInfo.ValidateMethod Is Nothing Then
				Return
			End If

			Dim builder As Moca.Db.EntityBuilder = New Moca.Db.EntityBuilder
			builder.SetColumnInfo(bindInfo.EntityTarget)

			'TODO: インタフェースチェックする予定
			Dim info As FieldInfo = Nothing
			Dim fields() As FieldInfo = bindInfo.EntityTarget.GetType.GetFields(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
			For Each field As FieldInfo In fields
				Dim attr As Moca.Db.Attr.TableAttribute = ClassUtil.GetCustomAttribute(Of Moca.Db.Attr.TableAttribute)(field.FieldType)
				If attr Is Nothing Then
					Continue For
				End If
				info = field
			Next
			'info = bindInfo.EntityTarget.GetType.GetField("tableDefinition", BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Public)
			If info Is Nothing Then
				Return
			End If

			Dim obj As Object
			obj = bindInfo.EntityTarget.GetType.InvokeMember(info.Name, BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.GetField, Nothing, bindInfo.EntityTarget, New Object() {})
			bindInfo.TableDefinition = obj
			bindInfo.TableDefinitionFieldInfo = info
		End Sub

		''' <summary>
		''' バインドコントロール属性を取得する
		''' </summary>
		''' <param name="propInfo"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getAttrBindControl(ByVal propInfo As PropertyInfo) As BindControlAttribute
			Return ClassUtil.GetCustomAttribute(Of BindControlAttribute)(propInfo)
		End Function

		''' <summary>
		''' エンティティのキャプション属性を取得し文字列を返す
		''' </summary>
		''' <param name="propInfo"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getCaption(ByVal propInfo As PropertyInfo) As String
			Dim attr As CaptionAttribute
			attr = ClassUtil.GetCustomAttribute(Of CaptionAttribute)(propInfo)
			If attr Is Nothing Then
				Return String.Empty
			End If
			Return attr.Caption
		End Function

		''' <summary>
		''' エンティティのフォーマット属性を取得し文字列を返す
		''' </summary>
		''' <param name="propInfo"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getFormat(ByVal propInfo As PropertyInfo) As String
			Dim attr As FormatAttribute
			attr = ClassUtil.GetCustomAttribute(Of FormatAttribute)(propInfo)
			If attr Is Nothing Then
				Return String.Empty
			End If
			Return attr.Format
		End Function

		''' <summary>
		''' エンティティのテーブル列名を返す
		''' </summary>
		''' <param name="propInfo"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getTableColumnName(ByVal propInfo As PropertyInfo) As String
			Dim attr As Moca.Db.Attr.ColumnAttribute
			attr = ClassUtil.GetCustomAttribute(Of Moca.Db.Attr.ColumnAttribute)(propInfo)
			If attr Is Nothing Then
				Return propInfo.Name
			End If
			Return attr.ColumnName
		End Function

		''' <summary>
		''' 検証属性を取得する
		''' </summary>
		''' <param name="propInfo"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getAttrValidate(ByVal propInfo As PropertyInfo) As ValidateAttribute
			Return ClassUtil.GetCustomAttribute(Of ValidateAttribute)(propInfo)
		End Function

		''' <summary>
		''' エンティティの値を取得する
		''' </summary>
		''' <param name="entity"></param>
		''' <param name="propInfo"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getEntityValue(ByVal entity As Object, ByVal propInfo As PropertyInfo) As Object
			Return entity.GetType.InvokeMember(propInfo.Name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, entity, New Object() {})
		End Function

		''' <summary>
		''' エンティティへ値を設定する
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <remarks></remarks>
		Private Sub _setEntityValue(ByVal bindInfo As _entityBindInfo)
			' 検証
			If Not _validate(bindInfo) Then
				Return
			End If
			If bindInfo.UpdateEntityStop Then
				Return
			End If

			Dim value As Object = bindInfo.Value
			If value IsNot Nothing Then
				If bindInfo.EntityPropertyInfo.PropertyType.IsPrimitive Then
					If value IsNot Nothing Then
						value = Convert.ChangeType(value, bindInfo.EntityPropertyInfo.PropertyType)
					End If
				Else
					Select Case Convert.GetTypeCode(value)
						Case TypeCode.Object
							bindInfo.Value = value
						Case Else
							'If value Is Nothing Then
							'	value = Convert.ChangeType(value, bindInfo.EntityPropertyInfo.PropertyType)
							'End If
							value = Convert.ChangeType(value, bindInfo.EntityPropertyInfo.PropertyType)
					End Select
				End If
			End If

			bindInfo.EntityTarget.GetType.InvokeMember(bindInfo.EntityPropertyInfo.Name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.SetProperty, Nothing, bindInfo.EntityTarget, New Object() {value})
		End Sub

		''' <summary>
		''' 検証
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <remarks></remarks>
		Private Function _validate(ByVal bindInfo As _entityBindInfo) As Boolean
			If bindInfo.ValidateMethod Is Nothing Then
				' 検証省略時は処理終了
				Return True
			End If

			' 検証種別取得
			Dim attrValid As ValidateAttribute = _getAttrValidate(bindInfo.EntityPropertyInfo)
			If attrValid Is Nothing Then
				Return True
			End If

			' キャプション取得
			Dim caption As String = _getCaption(bindInfo.EntityPropertyInfo)
			Dim columnName As String = _getTableColumnName(bindInfo.EntityPropertyInfo)

			Dim args As UpdateEntityValidateArgs = New UpdateEntityValidateArgs(bindInfo.ErrorColumns)
			args.Caption = caption
			args.EntityPropertyName = bindInfo.EntityPropertyInfo.Name
			args.ValidateType = attrValid.ValidateType
			args.Min = attrValid.Min
			args.Max = attrValid.Max
			args.Value = bindInfo.Value
			args.IsValid = True
			args.ValidateStop = False

			Dim columnDefinition As Moca.Db.DbInfoColumn = bindInfo.TableDefinitionColumn(columnName)
			If columnDefinition IsNot Nothing Then
				If args.Max Is Nothing Then
					If _validator.IsValidLenghtMaxB(args.ValidateType) Then
						args.Max = columnDefinition.MaxLength
					Else
						args.Max = columnDefinition.MaxLengthB
					End If
				End If
			End If

			Dim value As Object = args.Value
			If IsArray(value) Then
				value = CType(args.Value, Array).Length
			End If
			Dim verifyValue As String = String.Empty
			If value IsNot Nothing Then
				verifyValue = value.ToString
			End If
			args.ValidateResultType = _validator.Verify(verifyValue, args.ValidateType, args.Min, args.Max)
			If args.ValidateResultType <> ValidateTypes.None Then
				args.IsValid = False
				bindInfo.AddError(caption)
			End If

			bindInfo.ValidateMethod.Invoke(bindInfo.Control, args)
			bindInfo.UpdateEntityStop = args.ValidateStop
			If bindInfo.IsValid Then
				bindInfo.IsValid = args.IsValid
			End If
			Return args.IsValid
		End Function

		''' <summary>
		''' バインド先のコントロールを取得する
		''' </summary>
		''' <param name="bindTarget"></param>
		''' <param name="propInfo"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _getBindControl(ByVal bindTarget As Object, ByVal propInfo As PropertyInfo) As Object
			Return bindTarget.GetType.InvokeMember(propInfo.Name, BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.GetProperty, Nothing, bindTarget, New Object() {})
		End Function

		''' <summary>
		''' バインド先のコントロールへエンティティの値を設定する
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <param name="name"></param>
		''' <remarks></remarks>
		Private Sub _setTargetProperty(ByVal bindInfo As _entityBindInfo, ByVal name As String)
			Try
				Dim format As String = _getFormat(bindInfo.EntityPropertyInfo)

				Dim value As Object = bindInfo.Value
				If Convert.GetTypeCode(value) = TypeCode.DateTime Then
					If String.IsNullOrEmpty(format) Then
						value = value.ToString
					Else
						value = String.Format("{0:" & format & "}", value)
					End If
				Else
					If Not String.IsNullOrEmpty(format) Then
						value = String.Format(format, value)
					Else
						If value IsNot Nothing Then
							If value.GetType.IsPrimitive Then
								If Not TypeOf value Is Boolean Then
									value = value.ToString
								End If
							End If
						End If
					End If
				End If
				bindInfo.Control.GetType.InvokeMember(name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.SetProperty, Nothing, bindInfo.Control, New Object() {value})
			Catch ex As Exception
				Throw New Moca.Exceptions.MocaRuntimeException(ex, String.Format(C_MSG, bindInfo.ControlPropertyInfo.DeclaringType.ToString, bindInfo.ControlPropertyInfo.Name, name, bindInfo.EntityPropertyInfo.DeclaringType.ToString, bindInfo.EntityPropertyInfo.Name))
			End Try
		End Sub

		''' <summary>
		''' バインド先のコントロールへエンティティの値を設定する（パスワードのとき）
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <remarks></remarks>
		Private Sub _setTargetPropertyPassword(ByVal bindInfo As _entityBindInfo)
			Dim attr As Object
			Try
				attr = bindInfo.Control.GetType.InvokeMember("Attributes", BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, bindInfo.Control, New Object() {})
				attr.GetType.InvokeMember("Add", BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.InvokeMethod, Nothing, attr, New Object() {"Value", bindInfo.Value})
			Catch ex As Exception
				Throw New Moca.Exceptions.MocaRuntimeException(ex, String.Format(C_MSG, bindInfo.ControlPropertyInfo.DeclaringType.ToString, bindInfo.ControlPropertyInfo.Name, "Text", bindInfo.EntityPropertyInfo.DeclaringType.ToString, bindInfo.EntityPropertyInfo.Name))
			End Try
		End Sub

		''' <summary>
		''' バインド先のコントロールへエンティティの値を設定する（コンボボックスやリストボックスのとき）
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <remarks></remarks>
		Private Sub _setTargetPropertySelectedValue(ByVal bindInfo As _entityBindInfo)
			Try
				If TypeOf bindInfo.Value Is Array Then
					_setTargetPropertySelectedValues(bindInfo)
					Return
				End If
				CallByName(bindInfo.Control, "SelectedValue", CallType.Set, New Object() {bindInfo.Value})
			Catch ex As Moca.Exceptions.MocaRuntimeException
				Throw ex
			Catch ex As Exception
				Throw New Moca.Exceptions.MocaRuntimeException(ex, String.Format(C_MSG, bindInfo.ControlPropertyInfo.DeclaringType.ToString, bindInfo.ControlPropertyInfo.Name, "SelectedValue", bindInfo.EntityPropertyInfo.DeclaringType.ToString, bindInfo.EntityPropertyInfo.Name))
			End Try
		End Sub

		''' <summary>
		''' バインド先のコントロールへエンティティの値を設定する（カレンダーのとき）
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <remarks></remarks>
		Private Sub _setTargetPropertySelectedDate(ByVal bindInfo As _entityBindInfo)
			Try
				CallByName(bindInfo.Control, "SelectedDate", CallType.Set, New Object() {bindInfo.Value})
			Catch ex As Exception
				Throw New Moca.Exceptions.MocaRuntimeException(ex, String.Format(C_MSG, bindInfo.ControlPropertyInfo.DeclaringType.ToString, bindInfo.ControlPropertyInfo.Name, "SelectedValue", bindInfo.EntityPropertyInfo.DeclaringType.ToString, bindInfo.EntityPropertyInfo.Name))
			End Try
		End Sub

		''' <summary>
		''' バインド先のコントロールへエンティティの値を設定する（複数選択リストボックスのとき）
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <remarks></remarks>
		Private Sub _setTargetPropertySelectedValues(ByVal bindInfo As _entityBindInfo)
			Dim items As Object
			Dim control As Object = bindInfo.Control
			Dim val As Object = String.Empty
			Try
				If bindInfo.Value Is Nothing Then
					Return
				End If
				items = bindInfo.Control.GetType.InvokeMember("Items", BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, bindInfo.Control, New Object() {})
				If items IsNot Nothing Then
					Dim item As Object
					For Each val In CType(bindInfo.Value, Array)
						If val Is Nothing Then
							Continue For
						End If
						item = items.GetType.InvokeMember("FindByValue", BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.InvokeMethod, Nothing, items, New Object() {val.ToString})
						If item IsNot Nothing Then
							bindInfo.Control = item
							bindInfo.Value = True
							_setTargetProperty(bindInfo, "Selected")
						End If
					Next
				End If
			Catch ex As Moca.Exceptions.MocaRuntimeException
				Throw ex
			Catch ex As Exception
				Throw New Moca.Exceptions.MocaRuntimeException(ex, String.Format(C_MSG, bindInfo.ControlPropertyInfo.DeclaringType.ToString, bindInfo.ControlPropertyInfo.Name, String.Format("Items.FindByValue({0}).Selected", val.ToString), bindInfo.EntityPropertyInfo.DeclaringType.ToString, bindInfo.EntityPropertyInfo.Name))
			Finally
				bindInfo.Control = control
			End Try
		End Sub

		''' <summary>
		''' エンティティへバインド先のコントロールの値を設定する
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <param name="name"></param>
		''' <remarks></remarks>
		Private Sub _setEntiryProperty(ByVal bindInfo As _entityBindInfo, ByVal name As String)
			Dim value As Object

			' コントロールの値を取得
			value = bindInfo.Control.GetType.InvokeMember(name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, bindInfo.Control, New Object() {})

			Try
				bindInfo.Value = value
				_setEntityValue(bindInfo)
			Catch ex As Exception
				Throw New Moca.Exceptions.MocaRuntimeException(ex, String.Format(C_MSG2, bindInfo.EntityPropertyInfo.DeclaringType.ToString, bindInfo.EntityPropertyInfo.Name, bindInfo.ControlPropertyInfo.DeclaringType.ToString, bindInfo.ControlPropertyInfo.Name, name))
			End Try
		End Sub

		Private Sub _setEntiryPropertyRadioButtonChecked(ByVal bindInfo As _entityBindInfo)
			Const name As String = "Checked"
			Const name2 As String = "GroupName"
			Dim value As Boolean
			Dim groupName As String

			' コントロールの値を取得
			value = CType(bindInfo.Control.GetType.InvokeMember(name, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, bindInfo.Control, New Object() {}), Boolean)

			Try
				' 
				If Not value Then
					Return
				End If
				' コントロールのGroupNameを取得
				groupName = TryCast(bindInfo.Control.GetType.InvokeMember(name2, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, bindInfo.Control, New Object() {}), String)
				bindInfo.Value = bindInfo.ControlPropertyInfo.Name.Replace(groupName, String.Empty)
				_setEntityValue(bindInfo)
			Catch ex As Exception
				Throw New Moca.Exceptions.MocaRuntimeException(ex, String.Format(C_MSG2, bindInfo.EntityPropertyInfo.DeclaringType.ToString, bindInfo.EntityPropertyInfo.Name, bindInfo.ControlPropertyInfo.DeclaringType.ToString, bindInfo.ControlPropertyInfo.Name, name))
			End Try

		End Sub

		''' <summary>
		''' エンティティへバインド先のコントロールの値を設定する（コンボボックスやリストボックスのとき）
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <remarks></remarks>
		Private Sub _setEntiryPropertySelectedValue(ByVal bindInfo As _entityBindInfo)
			' SelectionMode 判定
			Dim selectionModeInfo As PropertyInfo
			selectionModeInfo = bindInfo.Control.GetType.GetProperty("SelectionMode")
			If selectionModeInfo Is Nothing Then
				_setEntiryProperty(bindInfo, "SelectedValue")
				Return
			End If
			If CType(selectionModeInfo.GetValue(bindInfo.Control, Nothing), Integer) = 0 Then
				' 一行選択時
				_setEntiryProperty(bindInfo, "SelectedValue")
				Return
			End If

			_setEntiryPropertySelectedValues(bindInfo)
		End Sub

		''' <summary>
		''' エンティティへバインド先のコントロールの値を設定する（複数選択リストボックスのとき）
		''' </summary>
		''' <param name="bindInfo"></param>
		''' <remarks></remarks>
		Private Sub _setEntiryPropertySelectedValues(ByVal bindInfo As _entityBindInfo)
			Dim values As ArrayList = New ArrayList

			Try
				Dim items As Object
				items = bindInfo.Control.GetType.InvokeMember("Items", BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, bindInfo.Control, New Object() {})
				If items IsNot Nothing Then
					For Each item As Object In CType(items, ListItemCollection)
						Dim selected As Boolean
						selected = DirectCast(item.GetType.InvokeMember("Selected", BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, item, New Object() {}), Boolean)
						If Not selected Then
							Continue For
						End If
						Dim value As Object
						value = item.GetType.InvokeMember("Value", BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.GetProperty, Nothing, item, New Object() {})
						values.Add(Convert.ChangeType(value, bindInfo.EntityPropertyInfo.PropertyType.GetElementType))
					Next
				End If

				If values.Count = 0 Then
					bindInfo.Value = Nothing
				Else
					Dim elementType As Type = Type.GetType(bindInfo.EntityPropertyInfo.PropertyType.GetElementType.ToString)
					bindInfo.Value = values.ToArray(elementType)
				End If
				_setEntityValue(bindInfo)
			Catch ex As Exception
				Throw New Moca.Exceptions.MocaRuntimeException(ex, String.Format(C_MSG2, bindInfo.EntityPropertyInfo.DeclaringType.ToString, bindInfo.EntityPropertyInfo.Name, bindInfo.ControlPropertyInfo.DeclaringType.ToString, bindInfo.ControlPropertyInfo.Name, "Items().Selected"))
			End Try
		End Sub

#End Region

#End Region

	End Class

End Namespace
