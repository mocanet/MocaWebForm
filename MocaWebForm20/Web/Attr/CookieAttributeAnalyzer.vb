
Imports Moca.Aop
Imports Moca.Attr
Imports Moca.Di
Imports Moca.Util

Namespace Web.Attr

	''' <summary>
	''' クッキー属性解析
	''' </summary>
	''' <remarks></remarks>
	Public Class CookieAttributeAnalyzer
		Implements IAttributeAnalyzer

		Public Function Analyze(ByVal target As System.Type) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal target As Object, ByVal field As System.Reflection.FieldInfo) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			' Interface ？
			If Not field.FieldType.IsInterface() Then
				Return Nothing
			End If

			If TryCast(target, Page) Is Nothing _
			 And TryCast(target, MasterPage) Is Nothing _
			 And TryCast(target, WebService) Is Nothing _
			 And TryCast(target, UserControl) Is Nothing _
			 And TryCast(target, HttpApplication) Is Nothing Then
				Return Nothing
			End If

			Dim attr As CookieAttribute

			attr = ClassUtil.GetCustomAttribute(Of CookieAttribute)(field)
			If attr Is Nothing Then
				Return Nothing
			End If

            Return attr.CreateComponent(Of MocaComponent4Http)(target, field)
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal method As System.Reflection.MethodInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal prop As System.Reflection.PropertyInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal targetType As System.Type, ByVal method As System.Reflection.EventInfo) As Aop.IAspect() Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

	End Class

End Namespace
