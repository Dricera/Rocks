﻿using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Rocks.Extensions;

internal static class IPropertySymbolExtensions
{
	internal static ImmutableHashSet<INamespaceSymbol> GetNamespaces(this IPropertySymbol self)
	{
		static IEnumerable<INamespaceSymbol> GetParameterNamespaces(IParameterSymbol parameter)
		{
			foreach (var parameterTypeNamespaces in parameter.Type.GetNamespaces())
			{
				yield return parameterTypeNamespaces;
			}

			foreach (var attributeNamespace in parameter.GetAttributes().SelectMany(_ => _.GetNamespaces()))
			{
				yield return attributeNamespace;
			}
		}

		var namespaces = ImmutableHashSet.CreateBuilder<INamespaceSymbol>();

		namespaces.AddRange(self.GetAttributes().SelectMany(_ => _.GetNamespaces()));
		namespaces.AddRange(self.Type.GetNamespaces());

		if (self.IsIndexer)
		{
			namespaces.AddRange(self.Parameters.SelectMany(_ => GetParameterNamespaces(_)));
		}

		return namespaces.ToImmutable();
	}

	internal static bool IsUnsafe(this IPropertySymbol self) =>
		self.IsIndexer ? (self.Parameters.Any(_ => _.Type.IsPointer()) || self.Type.IsPointer()) : self.Type.IsPointer();

	internal static PropertyAccessor GetAccessors(this IPropertySymbol self)
	{
		if (self.GetMethod is not null && self.SetMethod is not null)
		{
			return self.SetMethod.IsInitOnly ? PropertyAccessor.GetAndInit : PropertyAccessor.GetAndSet;
		}
		else if (self.GetMethod is not null)
		{
			return PropertyAccessor.Get;
		}
		else
		{
			return self.SetMethod!.IsInitOnly ? PropertyAccessor.Init : PropertyAccessor.Set;
		}
	}
}