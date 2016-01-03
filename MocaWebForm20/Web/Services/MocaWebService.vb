
Imports Moca.Di

Namespace Web.Services

	Public Class MocaWebService
		Inherits System.Web.Services.WebService

#Region " Declare "

		''' <summary>ページに対しての依存性注入</summary>
		Private _injector As MocaPageInjector

#Region " Logging For Log4net "
		''' <summary>Logging For Log4net</summary>
		Private Shared ReadOnly _mylog As log4net.ILog = log4net.LogManager.GetLogger(String.Empty)
#End Region
#End Region

#Region " コンストラクタ "

		''' <summary>
		''' デフォルトコンストラクタ
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
			MyBase.New()
			Try
				' 属性による依存性の注入
				_injector = New MocaPageInjector()
				_injector.Inject(Me)
			Catch ex As Exception
				_mylog.Debug(ex)
			End Try
		End Sub

		''' <summary>
		''' デコンストラクタ
		''' </summary>
		''' <remarks></remarks>
		Protected Overrides Sub Finalize()
			MyBase.Finalize()
		End Sub

#End Region

		Private Sub MocaWebService_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed
			_injector.DaoDispose(Me)
		End Sub

	End Class

End Namespace
