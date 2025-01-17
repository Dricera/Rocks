﻿using Microsoft.CodeAnalysis;
using Rocks.Builders.Shim;
using Rocks.Extensions;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace Rocks.Builders.Create;

internal static class MockTypeBuilder
{
	internal static void Build(IndentedTextWriter writer, MockInformation information, Compilation compilation)
	{
		var typeToMock = information.TypeToMock!;
		var kind = typeToMock.Type.IsRecord ? "record" : "class";
		var mockTypeName = $"Rock{typeToMock.FlattenedName}";

		writer.WriteLine($"private sealed {kind} {mockTypeName}");
		writer.Indent++;
		writer.WriteLine($": {typeToMock.ReferenceableName}{(information.Events.Results.Length > 0 ? $", global::Rocks.IRaiseEvents" : string.Empty)}");
		writer.Indent--;

		writer.WriteLine("{");
		writer.Indent++;

		MockTypeBuilder.BuildShimFields(writer, information);
		MockTypeBuilder.BuildRefReturnFields(writer, information);

		writer.WriteLine($"private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;");
		writer.WriteLine();

		if (information.Constructors.Length > 0)
		{
			foreach (var constructor in information.Constructors)
			{
				MockConstructorBuilder.Build(writer, typeToMock, compilation, constructor.Parameters, information.Shims);
			}
		}
		else
		{
			MockConstructorBuilder.Build(writer, typeToMock, compilation, ImmutableArray<IParameterSymbol>.Empty, information.Shims);
		}

		writer.WriteLine();

		var raiseEvents = information.Events.Results.Length > 0;

		foreach (var method in information.Methods.Results)
		{
			if (method.Value.ReturnsVoid)
			{
				MockMethodVoidBuilder.Build(writer, method, raiseEvents, compilation);
			}
			else
			{
				MockMethodValueBuilder.Build(writer, method, raiseEvents, compilation);
			}
		}

		foreach (var property in information.Properties.Results.Where(_ => !_.Value.IsIndexer))
		{
			MockPropertyBuilder.Build(writer, property, raiseEvents, compilation);
		}

		foreach (var indexer in information.Properties.Results.Where(_ => _.Value.IsIndexer))
		{
			MockIndexerBuilder.Build(writer, indexer, raiseEvents, compilation);
		}

		if (information.Events.Results.Length > 0)
		{
			writer.WriteLine();
			MockEventsBuilder.Build(writer, information.Events.Results, compilation);
		}

		MockTypeBuilder.BuildShimTypes(writer, information, mockTypeName, compilation);
		writer.Indent--;
		writer.WriteLine("}");
	}

	private static void BuildShimTypes(IndentedTextWriter writer, MockInformation information, string mockTypeName,
		Compilation compilation)
	{
		foreach (var shimType in information.Shims)
		{
			writer.WriteLine();
			ShimBuilder.Build(writer, shimType, mockTypeName, compilation, information);
		}
	}

	private static void BuildShimFields(IndentedTextWriter writer, MockInformation information)
	{
		foreach (var shimType in information.Shims)
		{
			writer.WriteLine($"private readonly {shimType.GetFullyQualifiedName()} shimFor{shimType.GetName(TypeNameOption.Flatten)};");
		}
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