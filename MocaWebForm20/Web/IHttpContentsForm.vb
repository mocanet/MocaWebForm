
Namespace Web

    ''' <summary>
    ''' Http���̃C���^�Z�v�^�[�Ŏg�p����R���e���c�C���^�t�F�[�X
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IHttpContentsForm
        Inherits IHttpContents

        ''' <summary>
        ''' �r���[�X�e�[�g�v���p�e�B
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' Page ���́AMasterPage �̂ǂ��炩�ɃI�u�W�F�N�g�����݂���Ƃ��́A���݂��Ă���I�u�W�F�N�g�� StateBag ��Ԃ��B
        ''' �����ɃI�u�W�F�N�g�����݂��Ă���Ƃ��́A����ł͂��肦�Ȃ��B
        ''' </remarks>
        ReadOnly Property ViewState() As StateBag

    End Interface

End Namespace
