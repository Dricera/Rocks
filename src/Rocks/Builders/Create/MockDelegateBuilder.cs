﻿using Microsoft.CodeAnalysis;
using Rocks.Extensions;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Rocks.Builders.Create
{
	internal static class MockDelegateBuilder
	{
		internal static string GetDelegateName(IMethodSymbol method) =>
			method.GetName(extendedName: "Callback");

		internal static string GetDelegateDefinition(IMethodSymbol method)
		{
			var returnType = method.ReturnType.GetName();
			var methodParameters = string.Join(", ", method.Parameters.Select(_ =>
			{
				var direction = _.RefKind == RefKind.Ref ? "ref " : _.RefKind == RefKind.Out ? "out " : string.Empty;
				var parameter = $"{direction}{(_.IsParams ? "params " : string.Empty)}{_.Type.GetName()} {_.Name}";
				return $"{(_.GetAttributes().Length > 0 ? $"{_.GetAttributes().GetDescription()} " : string.Empty)}{parameter}";
			}));
			var isUnsafe = method.IsUnsafe() ? "unsafe " : string.Empty;
			return $"public {isUnsafe}delegate {returnType} {MockDelegateBuilder.GetDelegateName(method)}({methodParameters});";
		}

		internal static void Build(IndentedTextWriter writer, MockInformation information)
		{
			static void BuildDelegate(IndentedTextWriter writer, IMethodSymbol method) => 
				writer.WriteLine(MockDelegateBuilder.GetDelegateDefinition(method));

			static void BuildDelegates(IndentedTextWriter writer, IEnumerable<IMethodSymbol> methods)
			{
				foreach (var method in methods)
				{
					BuildDelegate(writer, method);
				}
			}

			static void BuildProperties(IndentedTextWriter writer, MockInformation information)
			{
				var getPropertyMethods = information.Properties
					.Where(_ => _.Value.GetMethod is not null && _.Value.Type.IsPointer() &&
						_.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.No)
					.Select(_ => _.Value.GetMethod!);
				BuildDelegates(writer, getPropertyMethods);

				var setPropertyMethods = information.Properties
					.Where(_ => _.Value.SetMethod is not null && _.Value.Type.IsPointer() &&
						_.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.No)
					.Select(_ => _.Value.SetMethod!);
				BuildDelegates(writer, setPropertyMethods);

				var explicitGetPropertyMethodGroups = information.Properties
					.Where(_ => _.Value.GetMethod is not null && _.Value.Type.IsPointer() &&
						_.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.Yes)
					.GroupBy(_ => _.Value.ContainingType);

				foreach (var explicitGetPropertyMethodGroup in explicitGetPropertyMethodGroups)
				{
					BuildDelegate(writer, explicitGetPropertyMethodGroup.First().Value.GetMethod!);
				}

				var explicitSetPropertyMethodGroups = information.Properties
					.Where(_ => _.Value.SetMethod is not null && _.Value.Type.IsPointer() &&
						_.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.Yes)
					.GroupBy(_ => _.Value.ContainingType);

				foreach (var explicitSetPropertyMethodGroup in explicitSetPropertyMethodGroups)
				{
					BuildDelegate(writer, explicitSetPropertyMethodGroup.First().Value.SetMethod!);
				}
			}

			if (information.Methods.Length > 0)
			{
				var methods = information.Methods
					.Where(_ => (_.Value.Parameters.Any(_ => _.RefKind == RefKind.Ref || _.RefKind == RefKind.Out || _.Type.IsPointer()) ||
						!_.Value.ReturnsVoid && _.Value.ReturnType.IsPointer()) &&
						_.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.No)
					.Select(_ => _.Value);
				BuildDelegates(writer, methods);

				var explicitMethodGroups = information.Methods
					.Where(_ => (_.Value.Parameters.Any(_ => _.RefKind == RefKind.Ref || _.RefKind == RefKind.Out || _.Type.IsPointer()) ||
						!_.Value.ReturnsVoid && _.Value.ReturnType.IsPointer()) &&
						_.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.Yes)
					.GroupBy(_ => _.Value.ContainingType);

				foreach (var explicitMethodGroup in explicitMethodGroups)
				{
					BuildDelegate(writer, explicitMethodGroup.First().Value);
				}
			}

			if (information.Properties.Length > 0)
			{
				BuildProperties(writer, information);
			}
		}
	}
}