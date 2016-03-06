
Imports Moca.Aop
Imports Moca.Exceptions

Namespace Web.Interceptor

	''' <summary>
	''' �r���[�X�e�[�g�������Ƃ��Ɏg�p���� Getter ���\�b�h�C���^�[�Z�v�^�[
	''' </summary>
	''' <remarks></remarks>
	Public Class ViewStateGetInterceptor
		Inherits AbstractHttpInterceptor
		Implements IMethodInterceptor

		''' <summary>�r���[�X�e�[�g��</summary>
		Private _name As String

		''' <summary>log4net logger</summary>
		Private ReadOnly _mylog As log4net.ILog = log4net.LogManager.GetLogger(String.Empty)

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="name">�r���[�X�e�[�g��</param>
		''' <remarks></remarks>
		Public Sub New(ByVal name As String)
			_name = name
		End Sub

#End Region

		''' <summary>
		''' ���\�b�h���s
		''' </summary>
		''' <param name="invocation">Interceptor����C���^�[�Z�v�g����Ă��郁�\�b�h�̏��</param>
		''' <returns>�Y������Z�b�V�����I�u�W�F�N�g</returns>
		''' <remarks>
		''' �r���[�X�e�[�g�������Ƀr���[�X�e�[�g����I�u�W�F�N�g��Ԃ��B
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