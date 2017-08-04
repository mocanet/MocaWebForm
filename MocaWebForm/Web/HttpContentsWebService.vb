
Imports System.Reflection

Namespace Web

	''' <summary>
	''' Http時のインタセプターで使用するWebServiceコンテンツ
	''' </summary>
	''' <remarks></remarks>
	Public Class HttpContentsWebService
		Inherits MarshalByRefObject
		Implements IHttpContentsForm

		Private _target As WebService

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <param name="target">WebService</param>
		''' <remarks></remarks>
		Public Sub New(ByVal target As WebService)
			_target = target
		End Sub

#End Region

#Region " プロパティ "

		''' <summary>
		''' WebServiceプロパティ
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property WebService() As System.Web.Services.WebService
			Get
				Return _target
			End Get
			Set(ByVal value As System.Web.Services.WebService)
				_target = value
			End Set
		End Property

#End Region

#Region " Implements "

		Public ReadOnly Property Target As Object Implements IHttpContentsForm.Target
			Get
				Return _target
			End Get
		End Property

		Public ReadOnly Property Application As System.Web.HttpApplicationState Implements IHttpContentsForm.Application
			Get
				Return _target.Application
			End Get
		End Property

		Public ReadOnly Property QueryStringMap As System.Collections.Hashtable Implements IHttpContentsForm.QueryStringMap
			Get
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property Request As System.Web.HttpRequest Implements IHttpContentsForm.Request
			Get
				Return _target.Context.Request
			End Get
		End Property

		Public ReadOnly Property Response As System.Web.HttpResponse Implements IHttpContentsForm.Response
			Get
				Return _target.Context.Response
			End Get
		End Property

		Public ReadOnly Property Session As System.Web.SessionState.HttpSessionState Implements IHttpContentsForm.Session
			Get
				Return _target.Session
			End Get
		End Property

		Public ReadOnly Property ViewState As System.Web.UI.StateBag Implements IHttpContentsForm.ViewState
			Get
				Return Nothing
			End Get
		End Property

#End Region

	End Class

End Namespace
