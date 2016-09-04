
Imports System.Reflection
Imports Moca.Di
Imports Moca.Util
Imports Moca.Db
Imports Moca.Db.Attr

Namespace Web.UI

	''' <summary>
	''' ASP.NET 標準の Web MasterPage クラスの拡張版
	''' </summary>
	''' <remarks>
	''' 業務アプリを作成するときにあると便利な機能達を提供します。
	''' </remarks>
	Public Class MocaMasterPage
		Inherits System.Web.UI.MasterPage

		''' <summary>ページに対しての依存性注入</summary>
		Private _injector As MocaPageInjector

		Private _webUtil As New WebUtil

		''' <summary>Web サーバー変数コレクション</summary>
		Protected serverVariables As IServerVariables

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

#End Region
#Region " イベント "

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
		<Obsolete("No Support Method")> _
		Public Function ToQueryString(ByVal values As Object) As String
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

	End Class

End Namespace
