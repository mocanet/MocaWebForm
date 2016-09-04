
Imports System.Reflection
Imports System.Text
Imports Moca.Di
Imports Moca.Util
Imports Moca.Db
Imports Moca.Db.Attr
Imports Moca.web.Attr

Namespace Web.UI

	''' <summary>
	''' ASP.NET 標準の Web Page クラスの拡張版
	''' </summary>
	''' <remarks>
	''' 業務アプリを作成するときにあると便利な機能達を提供します。
	''' </remarks>
	Public Class MocaPage
		Inherits System.Web.UI.Page

#Region " Declare "

		''' <summary>Web サーバー変数コレクション</summary>
		Protected serverVariables As IServerVariables

		''' <summary>ページに対しての依存性注入</summary>
		Private _injector As MocaPageInjector

		''' <summary>Web用ユーティリティ</summary>
		Private _webUtil As New WebUtil

		''' <summary>エンティティバインダー</summary>
		Private _entityBinder As EntityBinder

		''' <summary>ボタンのコマンド処理用</summary>
		Private _commandHandlers As IDictionary(Of Type, Object)

#End Region

#Region " コンストラクタ／デストラクタ "

		''' <summary>
		''' デフォルトコンストラクタ
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
			MyBase.New()

			' 属性による依存性の注入
			_injector = New MocaPageInjector()
			_injector.Inject(Me)

			_entityBinder = New EntityBinder
			_commandHandlers = New Dictionary(Of Type, Object)
		End Sub

		''' <summary>
		''' デストラクタ
		''' </summary>
		''' <remarks></remarks>
		Protected Overrides Sub Finalize()
			MyBase.Finalize()
			_injector.DaoDispose(Me)
		End Sub

#End Region
#Region " プロパティ "

		''' <summary>
		''' ログインユーザーのドメイン
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected ReadOnly Property LoginUserDomain() As String
			Get
				Dim ary() As String
				ary = Me.serverVariables.LogonUser.Split("\".ToCharArray)
				Return ary(0)
			End Get
		End Property

		''' <summary>
		''' ログインユーザーID
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected ReadOnly Property LoginUserId() As String
			Get
				Dim ary() As String
				ary = Me.serverVariables.LogonUser.Split("\".ToCharArray)
				If ary.Length = 1 Then
					Return ary(0)
				End If
				Return ary(1)
			End Get
		End Property

		''' <summary>
		''' Transfer メソッドによるページ遷移かどうか返す。
		''' </summary>
		''' <returns>True は Transfer メソッドによるページ遷移、False は Transfer メソッドによるページ遷移ではない。</returns>
		''' <remarks></remarks>
		Protected ReadOnly Property IsPreviousPageTransfer() As Boolean
			Get
				Return PreviousPage IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' クライアントから別ページによるページ遷移かどうか返す。
		''' </summary>
		''' <returns>True はクライアントから別ページによるページ遷移、False はクライアントから別ページによるページ遷移ではない。</returns>
		''' <remarks></remarks>
		Protected ReadOnly Property IsPreviousPagePostBack() As Boolean
			Get
				If Not IsPreviousPageTransfer() Then
					Return False
				End If

				Return PreviousPage.IsCrossPagePostBack()
			End Get
		End Property

#End Region

#Region " メソッド "

		''' <summary>
		''' デフォルトページへリダイレクト
		''' </summary>
		''' <remarks></remarks>
		Protected Sub RedirectDefaultUrl()
			Response.Redirect(FormsAuthentication.DefaultUrl)
		End Sub

		''' <summary>
		''' Transfer メソッドによるページ遷移かどうかチェックし、
		''' 直接アクセスの時はトップページを表示する。
		''' </summary>
		''' <remarks></remarks>
		Protected Sub ChkPreviousPageTransfer()
			If IsPreviousPageTransfer() Then
				Return
			End If

			RedirectDefaultUrl()
		End Sub

		''' <summary>
		''' クライアントから別ページによるページ遷移かどうかチェックし、
		''' 直接アクセスの時はトップページを表示する。
		''' </summary>
		''' <remarks></remarks>
		Protected Sub ChkPreviousPagePostBack()
			If IsPreviousPagePostBack() Then
				Return
			End If

			RedirectDefaultUrl()
		End Sub

		''' <summary>
		''' コンボボックスの値設定
		''' </summary>
		''' <param name="cbo"></param>
		''' <param name="valueField"></param>
		''' <param name="textField"></param>
		''' <param name="val"></param>
		''' <remarks></remarks>
		Protected Sub bindComboBox(ByVal cbo As DropDownList, ByVal valueField As String, ByVal textField As String, ByVal val As Object, Optional ByVal bolBrank As Boolean = False, Optional ByVal strBrank As String = "")
			cbo.Items.Clear()
			cbo.DataSource = Nothing
			cbo.DataValueField = valueField
			cbo.DataTextField = textField
			cbo.DataSource = val
			cbo.DataBind()
			If bolBrank Then
				cbo.Items.Insert(0, strBrank)
			End If
			cbo.SelectedIndex = -1
		End Sub

		''' <summary>
		''' コンボボックスの値設定
		''' </summary>
		''' <param name="lst"></param>
		''' <param name="valueField"></param>
		''' <param name="textField"></param>
		''' <param name="val"></param>
		''' <remarks></remarks>
		Protected Sub bindListBox(ByVal lst As ListBox, ByVal valueField As String, ByVal textField As String, ByVal val As Object, Optional ByVal bolBrank As Boolean = False, Optional ByVal strBrank As String = "")
			lst.Items.Clear()
			lst.Items.Clear()
			lst.DataSource = Nothing
			lst.DataValueField = valueField
			lst.DataTextField = textField
			lst.DataSource = val
			lst.DataBind()
			If bolBrank Then
				lst.Items.Insert(0, strBrank)
			End If
			lst.SelectedIndex = -1
		End Sub

		''' <summary>
		''' ボタンがクリックされたときのコマンドをハンドラーへ追加
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <param name="commandName"></param>
		''' <param name="handler"></param>
		''' <remarks></remarks>
		Protected Sub AddCommandHandler(Of T)(ByVal commandName As String, ByVal handler As T)
			Dim val As IDictionary(Of String, T) = Nothing
			Dim buf As Object = Nothing

			If Not _commandHandlers.TryGetValue(GetType(T), buf) Then
				buf = New Dictionary(Of String, T)
				_commandHandlers.Add(GetType(T), buf)
			End If
			DirectCast(buf, IDictionary(Of String, T)).Add(commandName, handler)
		End Sub

		''' <summary>
		''' ボタンがクリックされたときのコマンドをハンドラーから取得
		''' </summary>
		''' <typeparam name="T"></typeparam>
		''' <param name="commandName"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function GetCommandHandler(Of T)(ByVal commandName As String) As T
			Dim val As IDictionary(Of String, T) = Nothing
			Dim buf As Object = Nothing

			If Not _commandHandlers.TryGetValue(GetType(T), buf) Then
				buf = New Dictionary(Of String, T)
				_commandHandlers.Add(GetType(T), buf)
			End If

			Dim handler As T = Nothing
			DirectCast(buf, IDictionary(Of String, T)).TryGetValue(commandName, handler)
			Return handler
		End Function

#Region " EntityBinder "

		''' <summary>
		''' ページとエンティティをバインドする
		''' </summary>
		''' <param name="page"></param>
		''' <param name="entity"></param>
		''' <remarks></remarks>
		Protected Sub BindEntity(ByVal page As Page, ByVal entity As Object)
			_entityBinder.BindEntity(page, entity)
		End Sub

		''' <summary>
		''' ページの入力内容をエンティティへ反映する
		''' </summary>
		''' <param name="page"></param>
		''' <param name="entity"></param>
		''' <param name="validateMethod">入力値検証するときはメソッドを指定</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function UpdateEntity(ByVal page As Page, ByVal entity As Object, Optional ByVal validateMethod As UpdateEntityValidate = Nothing) As Boolean
			Return _entityBinder.UpdateEntity(page, entity, validateMethod)
		End Function

#End Region

        ''' <summary>
        ''' 指定されたプロパティからクエリー文字列を作成する。
        ''' </summary>
        ''' <param name="values"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 実装出来そうで出来ないメソッド。
        ''' 透過プロキシインスタンスから型の情報がうまく取れないため。
        ''' </remarks>
        <Obsolete("No Support Method")>
        Private Function ToQueryString(ByVal values As Object) As String
            Return _webUtil.ToQueryString(values)
        End Function

        ''' <summary>
        ''' 指定されたプロパティからクエリー文字列を作成する。
        ''' </summary>
        ''' <param name="values">クエリー値を保持したオブジェクト</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToQueryString(Of T)(ByVal values As Object, Optional ByVal questionMark As Boolean = True) As String
			Return _webUtil.ToQueryString(Of T)(values, questionMark)
		End Function

		''' <summary>
		''' 当システムの仮想カレントディレクトリまでのURL
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function GetUrlBase() As String
			Dim urlBase As StringBuilder = New StringBuilder

			' 仮想ディレクトリまでのURLを作成（http://hoge.com/hoge）
			urlBase.Append(Me.Request.Url.Scheme)
			urlBase.Append(System.Uri.SchemeDelimiter)
			urlBase.Append(Me.Request.Url.Authority)
			urlBase.Append(Me.Request.Url.LocalPath.Substring(0, Me.Request.Url.LocalPath.IndexOf("/"c, 2)))
			urlBase.Append("/")

			Return urlBase.ToString
		End Function

		''' <summary>
		''' パスワードの暗号化
		''' </summary>
		''' <param name="passwd">暗号化するパスワード</param>
		''' <param name="passwdFormat">暗号化方式</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function HashPassword(ByVal passwd As String, ByVal passwdFormat As String) As String
			Return FormsAuthentication.HashPasswordForStoringInConfigFile(passwd, passwdFormat)
		End Function

#End Region

	End Class

End Namespace
