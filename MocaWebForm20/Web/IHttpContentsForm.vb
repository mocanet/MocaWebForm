
Namespace Web

    ''' <summary>
    ''' Http時のインタセプターで使用するコンテンツインタフェース
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IHttpContentsForm
        Inherits IHttpContents

        ''' <summary>
        ''' ビューステートプロパティ
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' Page 又は、MasterPage のどちらかにオブジェクトが存在するときは、存在しているオブジェクトの StateBag を返す。
        ''' 両方にオブジェクトが存在しているときは、現状ではありえない。
        ''' </remarks>
        ReadOnly Property ViewState() As StateBag

    End Interface

End Namespace
