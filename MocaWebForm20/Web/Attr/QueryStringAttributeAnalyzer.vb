
Imports Moca.Aop
Imports Moca.Attr
Imports Moca.Util

Namespace Web.Attr

	''' <summary>
	''' クエリー文字列属性の解析
	''' </summary>
	''' <remarks></remarks>
	Public Class QueryStringAttributeAnalyzer
		Implements IAttributeAnalyzer

		Public Function Analyze(ByVal target As System.Type) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			Return Nothing
		End Function

		Public Function Analyze(ByVal target As Object, ByVal field As System.Reflection.FieldInfo) As Di.MocaComponent Implements IAttributeAnalyzer.Analyze
			' Interface ？
			If Not field.FieldType.IsInterface() Then
				Return Nothing
			End If

			Dim attr As QueryStringAttribute

			attr = ClassUtil.GetCustomAttribute(Of QueryStringAttribute)(field.FieldType)
			If attr Is Nothing Then
				Return Nothing
			End If

			Return attr.CreateComponent(target, field)
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
