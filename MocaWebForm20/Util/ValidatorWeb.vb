
Imports Moca.Util
Imports Moca.Web.UI

Namespace Util

	<Flags()> _
	Public Enum ValidateWebTypes As Integer
		''' <summary>「無し」又は「正常」</summary>
		None = 0
		''' <summary>必須</summary>
		Required = 1
	End Enum

	''' <summary>
	''' 値の検証クラス
	''' </summary>
	''' <remarks></remarks>
	Public Class ValidatorWeb
		Inherits Validator

		Public Overloads Function Verify(ByVal value As RadioButtonGroup, ByVal validates As ValidateWebTypes) As ValidateWebTypes
			Dim rc As ValidateWebTypes

			rc = ValidateWebTypes.None

			If IsValidateWebType(validates, ValidateWebTypes.Required) Then
				If value.GetSelected Is Nothing Then
					rc = ValidateWebTypes.Required
				End If
			End If

			Return rc
		End Function

		Public Overloads Function Verify(ByVal value As IList(Of RadioButton), ByVal validates As ValidateWebTypes) As ValidateWebTypes
			Dim rc As ValidateWebTypes

			rc = ValidateWebTypes.None

			If IsValidateWebType(validates, ValidateWebTypes.Required) Then
				Dim checked As Boolean
				checked = False
				For Each Val As RadioButton In value
					If Val.Checked Then
						checked = True
					End If
				Next
				If Not checked Then
					rc = ValidateWebTypes.Required
				End If
			End If

			Return rc
		End Function

		Public Overloads Function Verify(ByVal value As IList(Of CheckBox), ByVal validates As ValidateWebTypes) As ValidateWebTypes

			Return ValidateWebTypes.None
		End Function

		''' <summary>
		''' 検証種別のチェック
		''' </summary>
		''' <param name="validates">チェック対象</param>
		''' <param name="targetType">含まれているかチェックする検証種別</param>
		''' <returns>True: 含まれている、False: 含まれていない</returns>
		''' <remarks></remarks>
		Public Function IsValidateWebType(ByVal validates As ValidateWebTypes, ByVal targetType As ValidateWebTypes) As Boolean
			Return ((validates And targetType) = targetType)
		End Function

	End Class

End Namespace
