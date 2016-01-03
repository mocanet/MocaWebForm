
Imports System.Reflection
Imports Moca.Di
Imports Moca.Util
Imports Moca.Db
Imports Moca.Db.Attr

Namespace Web.UI

	''' <summary>
	''' ASP.NET �W���� Web MasterPage �N���X�̊g����
	''' </summary>
	''' <remarks>
	''' �Ɩ��A�v�����쐬����Ƃ��ɂ���ƕ֗��ȋ@�\�B��񋟂��܂��B
	''' </remarks>
	Public Class MocaMasterPage
		Inherits System.Web.UI.MasterPage

		''' <summary>�y�[�W�ɑ΂��Ă̈ˑ�������</summary>
		Private _injector As MocaPageInjector

		Private _webUtil As New WebUtil

		''' <summary>Web �T�[�o�[�ϐ��R���N�V����</summary>
		Protected serverVariables As IServerVariables

#Region " �R���X�g���N�^�^�f�X�g���N�^ "

		''' <summary>
		''' �f�t�H���g�R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
			MyBase.New()

			' �����ɂ��ˑ����̒���
			_injector = New MocaPageInjector()
			_injector.Inject(Me)
		End Sub

		''' <summary>
		''' �f�X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Protected Overrides Sub Finalize()
			MyBase.Finalize()
			_injector.DaoDispose(Me)
		End Sub

#End Region
#Region " �v���p�e�B "

#End Region
#Region " �C�x���g "

#End Region

		''' <summary>
		''' �w�肳�ꂽ�v���p�e�B����N�G���[��������쐬����B
		''' </summary>
		''' <param name="values"></param>
		''' <returns></returns>
		''' <remarks>
		''' �����o�������ŏo���Ȃ����\�b�h�B
		''' ���߃v���L�V�C���X�^���X����^�̏�񂪂��܂����Ȃ����߁B
		''' </remarks>
		<Obsolete("No Support Method")> _
		Public Function ToQueryString(ByVal values As Object) As String
			Return _webUtil.ToQueryString(values)
		End Function

		''' <summary>
		''' �w�肳�ꂽ�v���p�e�B����N�G���[��������쐬����B
		''' </summary>
		''' <param name="values">�N�G���[�l��ێ������I�u�W�F�N�g</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function ToQueryString(Of T)(ByVal values As Object, Optional ByVal questionMark As Boolean = True) As String
			Return _webUtil.ToQueryString(Of T)(values, questionMark)
		End Function

		''' <summary>
		''' ���V�X�e���̉��z�J�����g�f�B���N�g���܂ł�URL
		''' </summary>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function GetUrlBase() As String
			Dim urlBase As StringBuilder = New StringBuilder

			' ���z�f�B���N�g���܂ł�URL���쐬�ihttp://hoge.com/hoge�j
			urlBase.Append(Me.Request.Url.Scheme)
			urlBase.Append(System.Uri.SchemeDelimiter)
			urlBase.Append(Me.Request.Url.Authority)
			urlBase.Append(Me.Request.Url.LocalPath.Substring(0, Me.Request.Url.LocalPath.IndexOf("/"c, 2)))
			urlBase.Append("/")

			Return urlBase.ToString
		End Function

	End Class

End Namespace
