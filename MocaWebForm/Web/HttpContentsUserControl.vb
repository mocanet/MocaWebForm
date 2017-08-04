
Imports System.Reflection

Namespace Web

	''' <summary>
	''' Http時のインタセプターで使用するUserControlコンテンツ
	''' </summary>
	''' <remarks></remarks>
	Public Class HttpContentsUserControl
		Inherits MarshalByRefObject
		Implements IHttpContentsForm

		Private _target As UserControl

		Private _queryStringMap As Hashtable

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <param name="target">UserControl</param>
		''' <remarks></remarks>
		Public Sub New(ByVal target As UserControl)
			_target = target
			_queryStringMap = New Hashtable
		End Sub

#End Region

#Region " プロパティ "

		''' <summary>
		''' UserControlプロパティ
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property UserControl() As System.Web.UI.UserControl
			Get
				Return _target
			End Get
			Set(ByVal value As System.Web.UI.UserControl)
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
				Return _queryStringMap
			End Get
		End Property

		Public ReadOnly Property Request As System.Web.HttpRequest Implements IHttpContentsForm.Request
			Get
				Return _target.Request
			End Get
		End Property

		Public ReadOnly Property Response As System.Web.HttpResponse Implements IHttpContentsForm.Response
			Get
				Return _target.Response
			End Get
		End Property

		Public ReadOnly Property Session As System.Web.SessionState.HttpSessionState Implements IHttpContentsForm.Session
			Get
				Return _target.Session
			End Get
		End Property

		Public ReadOnly Property ViewState As System.Web.UI.StateBag Implements IHttpContentsForm.ViewState
			Get
				Dim bindFlg As BindingFlags = BindingFlags.GetProperty Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance

				Return TryCast(_target.GetType.InvokeMember("ViewState", bindFlg, Nothing, _target, New Object() {}), StateBag)
			End Get
		End Property

#End Region

	End Class

End Namespace
