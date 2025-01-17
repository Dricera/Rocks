﻿using Microsoft.CodeAnalysis;
using Rocks.Extensions;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Rocks.Builders.Make;

internal static class MockMakeBuilder
{
	internal static void Build(IndentedTextWriter writer, MockInformation information, Compilation compilation)
	{
		var typeToMock = information.TypeToMock!;
		var kind = typeToMock.Type.IsRecord ? "record" : "class";
		writer.WriteLine($"private sealed {kind} Rock{typeToMock.FlattenedName}");
		writer.Indent++;
		writer.WriteLine($": {typeToMock.ReferenceableName}");
		writer.Indent--;

		writer.WriteLine("{");
		writer.Indent++;

		MockMakeBuilder.BuildRefReturnFields(writer, information);

		if (information.Constructors.Length > 0)
		{
			foreach (var constructor in information.Constructors)
			{
				MockConstructorBuilder.Build(writer, typeToMock, compilation, constructor.Parameters);
			}
		}
		else
		{
			MockConstructorBuilder.Build(writer, typeToMock, compilation, ImmutableArray<IParameterSymbol>.Empty);
		}

		writer.WriteLine();

		var memberIdentifier = 0u;

		foreach (var method in information.Methods.Results)
		{
			if (method.Value.ReturnsVoid)
			{
				MockMethodVoidBuilder.Build(writer, method, compilation);
			}
			else
			{
				MockMethodValueBuilder.Build(writer, method, information.Model, compilation);
			}

			memberIdentifier++;
		}

		foreach (var property in information.Properties.Results.Where(_ => !_.Value.IsIndexer))
		{
			MockPropertyBuilder.Build(writer, property, compilation);
		}

		foreach (var indexer in information.Properties.Results.Where(_ => _.Value.IsIndexer))
		{
			MockIndexerBuilder.Build(writer, indexer, compilation);
		}

		if (information.Events.Results.Length > 0)
		{
			writer.WriteLine();
			MockEventsBuilder.Build(writer, information.Events.Results, compilation);
		}

		writer.Indent--;
		writer.WriteLine("}");
	}

	private static void BuildRefReturnFields(IndentedTextWriter writer, MockInformation information)
	{
		foreach (var method in information.Methods.Results.Where(_ => _.Value.ReturnsByRef || _.Value.ReturnsByRefReadonly))
		{
			writer.WriteLine($"private {method.Value.ReturnType.GetFullyQualifiedName()} rr{method.MemberIdentifier};");
		}

		foreach (var property in information.Properties.Results.Where(_ => _.Value.ReturnsByRef || _.Value.ReturnsByRefReadonly))
		{
			writer.WriteLine($"private {property.Value.Type.GetFullyQualifiedName()} rr{property.MemberIdentifier};");
		}
	}
}