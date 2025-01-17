﻿using Microsoft.CodeAnalysis;
using Rocks.Extensions;
using System.CodeDom.Compiler;

namespace Rocks.Builders.Create;

internal static class MethodExpectationsExtensionsMethodBuilder
{
	internal static void Build(IndentedTextWriter writer, MethodMockableResult result)
	{
		var method = result.Value;
		var isExplicitImplementation = result.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.Yes;
		var mockTypeName = result.MockType.GetFullyQualifiedName();
		var containingTypeName = method.ContainingType.GetFullyQualifiedName();
		var namingContext = new VariableNamingContext(method);

		var thisParameter = isExplicitImplementation ?
			$"this global::Rocks.Expectations.ExplicitMethodExpectations<{mockTypeName}, {containingTypeName}> @{namingContext["self"]}" :
			$"this global::Rocks.Expectations.MethodExpectations<{mockTypeName}> @{namingContext["self"]}";
		var instanceParameters = method.Parameters.Length == 0 ? thisParameter :
			string.Join(", ", thisParameter,
				string.Join(", ", method.Parameters.Select(_ =>
				{
					if (_.Type.IsEsoteric())
					{
						var argName = _.Type.IsPointer() ?
							PointerArgTypeBuilder.GetProjectedFullyQualifiedName(_.Type, result.MockType) :
							RefLikeArgTypeBuilder.GetProjectedFullyQualifiedName(_.Type, result.MockType);
						return $"{argName} @{_.Name}";
					}
					else
					{
						var requiresNullable = _.RequiresForcedNullableAnnotation() ? "?" : string.Empty;
						return $"global::Rocks.Argument<{_.Type.GetFullyQualifiedName()}{requiresNullable}> @{_.Name}";
					}
				})));

		var callbackDelegateTypeName = method.RequiresProjectedDelegate() ?
			MockProjectedDelegateBuilder.GetProjectedCallbackDelegateFullyQualifiedName(method, result.MockType) :
			method.ReturnsVoid ? 
				DelegateBuilder.Build(method.Parameters) : 
				DelegateBuilder.Build(method.Parameters, method.ReturnType);
		var returnType = method.ReturnsVoid ? string.Empty :
			method.ReturnType.IsRefLikeType ?
				MockProjectedDelegateBuilder.GetProjectedReturnValueDelegateFullyQualifiedName(method, result.MockType) :
				method.ReturnType.GetFullyQualifiedName();
		var adornmentsType = method.ReturnsVoid ?
			$"global::Rocks.MethodAdornments<{mockTypeName}, {callbackDelegateTypeName}>" :
			method.ReturnType.IsPointer() ?
				$"{MockProjectedTypesAdornmentsBuilder.GetProjectedAdornmentFullyQualifiedNameName(method.ReturnType, result.MockType, AdornmentType.Method, result.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.Yes)}<{mockTypeName}, {callbackDelegateTypeName}>" :
				$"global::Rocks.MethodAdornments<{mockTypeName}, {callbackDelegateTypeName}, {returnType}>";
		var (returnValue, newAdornments) = (adornmentsType, $"new {adornmentsType}");

		var addMethod = method.ReturnsVoid ? "Add" :
			method.ReturnType.IsPointer() ?
				MockProjectedTypesAdornmentsBuilder.GetProjectedAddExtensionMethodName(method.ReturnType) : 
				$"Add<{returnType}>";

		var constraints = method.GetConstraints();
		var extensionConstraints = constraints.Length > 0 ?
			method.Parameters.Length == 0 ? $" {string.Join(" ", constraints)} " : $" {string.Join(" ", constraints)}" : 
			method.Parameters.Length == 0 ? " " : "";

		if (method.Parameters.Length == 0)
		{
			writer.WriteLine($"internal static {returnValue} {method.GetName()}({instanceParameters}){extensionConstraints}=>");
			writer.Indent++;
			writer.WriteLine($"{newAdornments}(@{namingContext["self"]}.{addMethod}({result.MemberIdentifier}, new global::System.Collections.Generic.List<global::Rocks.Argument>()));");
			writer.Indent--;
		}
		else
		{
			writer.WriteLine($"internal static {returnValue} {method.GetName()}({instanceParameters}){extensionConstraints}");
			writer.WriteLine("{");
			writer.Indent++;

			foreach(var parameter in method.Parameters)
			{
				writer.WriteLine($"global::System.ArgumentNullException.ThrowIfNull(@{parameter.Name});");
			}

			var parameters = string.Join(", ", method.Parameters.Select(_ =>
			{
				if (_.HasExplicitDefaultValue)
				{
					return $"@{_.Name}.Transform({_.ExplicitDefaultValue.GetDefaultValue(_.Type)})";
				}
				else if (_.RefKind == RefKind.Out)
				{
					return $"global::Rocks.Arg.Any<{_.Type.GetFullyQualifiedName()}{(_.RequiresForcedNullableAnnotation() ? "?" : string.Empty)}>()";
				}
				else
				{
					return $"@{_.Name}";
				}
			}));
			writer.WriteLine($"return {newAdornments}(@{namingContext["self"]}.{addMethod}({result.MemberIdentifier}, new global::System.Collections.Generic.List<global::Rocks.Argument>({method.Parameters.Length}) {{ {parameters} }}));");

			writer.Indent--;
			writer.WriteLine("}");
		}
	}
}