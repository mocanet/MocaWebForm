
Imports Moca.Aop
Imports Moca.Exceptions

Namespace Web.Interceptor

	''' <summary>
	''' ビューステートを扱うときに使用する Getter メソッドインターセプター
	''' </summary>
	''' <remarks></remarks>
	Public Class ViewStateGetInterceptor
		Inherits AbstractHttpInterceptor
		Implements IMethodInterceptor

		''' <summary>ビューステート名</summary>
		Private _name As String

		''' <summary>log4net logger</summary>
		Private ReadOnly _mylog As log4net.ILog = log4net.LogManager.GetLogger(String.Empty)

#Region " コンストラクタ "

		''' <summary>
		''' コンストラクタ
		''' </summary>
		''' <param name="name">ビューステート名</param>
		''' <remarks></remarks>
		Public Sub New(ByVal name As String)
			_name = name
		End Sub

#End Region

		''' <summary>
		''' メソッド実行
		''' </summary>
		''' <param name="invocation">Interceptorからインターセプトされているメソッドの情報</param>
		''' <returns>該当するセッションオブジェクト</returns>
		''' <remarks>
		''' ビューステート名を元にビューステートからオブジェクトを返す。
		''' </remarks>
		Public Function Invoke(ByVal invocation As Aop.IMethodInvocation) As Object Implements Aop.IMethodInterceptor.Invoke
            Dim contents As IHttpContentsForm
            Dim methodName As String = invocation.This.GetType.FullName & "." & invocation.Method.Name

            checkHttpContents(invocation.This)

            contents = DirectCast(invocation.This, IHttpContentsForm)

            _mylog.DebugFormat("(Aspect:{0}) ViewState Getter.{1}", methodName, _name)

			Return contents.ViewState(_name)
		End Function

	End Class

End Namespace
