﻿using Rocks.CodeGenerationTest.Extensions.Extensions;

namespace Rocks.CodeGenerationTest.Mappings;

internal static class MappedTypes
{
	internal static Dictionary<Type, Dictionary<string, string>> GetMappedTypes() =>
		new Dictionary<Type, Dictionary<string, string>>()
			.AddItems(CslaMappings.GetMappedTypes())
			.AddItems(ComputeSharpMappings.GetMappedTypes());
}