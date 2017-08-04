
Imports Moca.Di

Namespace Web.UI

	''' <summary>
	''' ASP.NET 標準の UserControl クラスの拡張版
	''' </summary>
	''' <remarks>
	''' 業務アプリを作成するときにあると便利な機能達を提供します。
	''' </remarks>
	Public Class MocaUserControl
		Inherits System.Web.UI.UserControl

#Region " Declare "

		''' <summary>ページに対しての依存性注入</summary>
		Private _injector As MocaPageInjector

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

	End Class

End Namespace
