
Imports System.Reflection
Imports System.Text
Imports Moca.Di
Imports Moca.Util
Imports Moca.Db
Imports Moca.Db.Attr
Imports Moca.web.Attr

Namespace Web.UI

	''' <summary>
	''' ASP.NET �W���� Web Page �N���X�̊g����
	''' </summary>
	''' <remarks>
	''' �Ɩ��A�v�����쐬����Ƃ��ɂ���ƕ֗��ȋ@�\�B��񋟂��܂��B
	''' </remarks>
	Public Class MocaPage
		Inherits System.Web.UI.Page

#Region " Declare "

		''' <summary>Web �T�[�o�[�ϐ��R���N�V����</summary>
		Protected serverVariables As IServerVariables

		''' <summary>�y�[�W�ɑ΂��Ă̈ˑ�������</summary>
		Private _injector As MocaPageInjector

		''' <summary>Web�p���[�e�B���e�B</summary>
		Private _webUtil As New WebUtil

		''' <summary>�G���e�B�e�B�o�C���_�[</summary>
		Private _entityBinder As EntityBinder

		''' <summary>�{�^���̃R�}���h�����p</summary>
		Private _commandHandlers As IDictionary(Of Type, Object)

#End Region

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

			_entityBinder = New EntityBinder
			_commandHandlers = New Dictionary(Of Type, Object)
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

		''' <summary>
		''' ���O�C�����[�U�[�̃h���C��
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
		''' ���O�C�����[�U�[ID
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
		''' Transfer ���\�b�h�ɂ��y�[�W�J�ڂ��ǂ����Ԃ��B
		''' </summary>
		''' <returns>True �� Transfer ���\�b�h�ɂ��y�[�W�J�ځAFalse �� Transfer ���\�b�h�ɂ��y�[�W�J�ڂł͂Ȃ��B</returns>
		''' <remarks></remarks>
		Protected ReadOnly Property IsPreviousPageTransfer() As Boolean
			Get
				Return PreviousPage IsNot Nothing
			End Get
		End Property

		''' <summary>
		''' �N���C�A���g����ʃy�[�W�ɂ��y�[�W�J�ڂ��ǂ����Ԃ��B
		''' </summary>
		''' <returns>True �̓N���C�A���g����ʃy�[�W�ɂ��y�[�W�J�ځAFalse �̓N���C�A���g����ʃy�[�W�ɂ��y�[�W�J�ڂł͂Ȃ��B</returns>
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

#Region " ���\�b�h "

		''' <summary>
		''' �f�t�H���g�y�[�W�փ��_�C���N�g
		''' </summary>
		''' <remarks></remarks>
		Protected Sub RedirectDefaultUrl()
			Response.Redirect(FormsAuthentication.DefaultUrl)
		End Sub

		''' <summary>
		''' Transfer ���\�b�h�ɂ��y�[�W�J�ڂ��ǂ����`�F�b�N���A
		''' ���ڃA�N�Z�X�̎��̓g�b�v�y�[�W��\������B
		''' </summary>
		''' <remarks></remarks>
		Protected Sub ChkPreviousPageTransfer()
			If IsPreviousPageTransfer() Then
				Return
			End If

			RedirectDefaultUrl()
		End Sub

		''' <summary>
		''' �N���C�A���g����ʃy�[�W�ɂ��y�[�W�J�ڂ��ǂ����`�F�b�N���A
		''' ���ڃA�N�Z�X�̎��̓g�b�v�y�[�W��\������B
		''' </summary>
		''' <remarks></remarks>
		Protected Sub ChkPreviousPagePostBack()
			If IsPreviousPagePostBack() Then
				Return
			End If

			RedirectDefaultUrl()
		End Sub

		''' <summary>
		''' �R���{�{�b�N�X�̒l�ݒ�
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
		''' �R���{�{�b�N�X�̒l�ݒ�
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
		''' �{�^�����N���b�N���ꂽ�Ƃ��̃R�}���h���n���h���[�֒ǉ�
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
		''' �{�^�����N���b�N���ꂽ�Ƃ��̃R�}���h���n���h���[����擾
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
		''' �y�[�W�ƃG���e�B�e�B���o�C���h����
		''' </summary>
		''' <param name="page"></param>
		''' <param name="entity"></param>
		''' <remarks></remarks>
		Protected Sub BindEntity(ByVal page As Page, ByVal entity As Object)
			_entityBinder.BindEntity(page, entity)
		End Sub

		''' <summary>
		''' �y�[�W�̓��͓��e���G���e�B�e�B�֔��f����
		''' </summary>
		''' <param name="page"></param>
		''' <param name="entity"></param>
		''' <param name="validateMethod">���͒l���؂���Ƃ��̓��\�b�h���w��</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function UpdateEntity(ByVal page As Page, ByVal entity As Object, Optional ByVal validateMethod As UpdateEntityValidate = Nothing) As Boolean
			Return _entityBinder.UpdateEntity(page, entity, validateMethod)
		End Function

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
        <Obsolete("No Support Method")>
        Private Function ToQueryString(ByVal values As Object) As String
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

		''' <summary>
		''' �p�X���[�h�̈Í���
		''' </summary>
		''' <param name="passwd">�Í�������p�X���[�h</param>
		''' <param name="passwdFormat">�Í�������</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Function HashPassword(ByVal passwd As String, ByVal passwdFormat As String) As String
			Return FormsAuthentication.HashPasswordForStoringInConfigFile(passwd, passwdFormat)
		End Function

#End Region

	End Class

End Namespace
