﻿using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Rocks.Descriptors
{
	public static class CannotMockSealedTypeDescriptor
	{
		internal static Diagnostic Create(ITypeSymbol type) =>
			Diagnostic.Create(new(CannotMockSealedTypeDescriptor.Id, CannotMockSealedTypeDescriptor.Title,
				string.Format(CultureInfo.CurrentCulture, CannotMockSealedTypeDescriptor.Message, 
					type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)),
				DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
				helpLinkUri: HelpUrlBuilder.Build(
					CannotMockSealedTypeDescriptor.Id, CannotMockSealedTypeDescriptor.Title)), type.Locations[0]);

		public const string Id = "ROCK1";
		public const string Message = "The type {0} is sealed and cannot be mocked";
		public const string Title = "Cannot Mock Sealed Types";
	}
}