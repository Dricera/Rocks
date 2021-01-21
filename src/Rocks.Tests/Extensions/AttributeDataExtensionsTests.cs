﻿using ADETAttributes;
using ADETTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Rocks.Extensions;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace ADETTypes
{
	public class TypeOfThis { }

	public class OpenGeneric<T1, T2> { }
}

namespace ADETAttributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class MethodAttribute
		: Attribute
	{
		public MethodAttribute(Type value) =>
			this.Value = value;

		public Type Value { get; }
	}
}

namespace Rocks.Tests.Extensions
{
	public enum MyValue
	{
		ThisOne, ThatOne, AnotherOne
	}

	[AttributeUsage(AttributeTargets.All)]
	public sealed class MyTestAttribute
		: Attribute
	{
		public MyTestAttribute(string a, double b, int c, uint d, Type e, int[] f, MyValue g) =>
			(this.A, this.B, this.C, this.D, this.E, this.F, this.G) =
				(a, b, c, d, e, f, g);

		public string A { get; }
		public double B { get; }
		public int C { get; }
		public uint D { get; }
		public Type E { get; }
		public int[] F { get; }
		public MyValue G { get; }

		public int NamedA { get; set; }
	}

	public static class AttributeDataExtensionsTests
	{
		[Test]
		public static void GetNamespaces()
		{
			var (attributes, compilation) = AttributeDataExtensionsTests.GetAttributes(
@$"using {typeof(TypeOfThis).Namespace};
using {typeof(MethodAttribute).Namespace};

public interface IA
{{
	[Method(typeof(TypeOfThis))]
	void Foo();
}}");

			var namespaces = attributes[0].GetNamespaces();

			Assert.Multiple(() =>
			{
				Assert.That(namespaces.Count, Is.EqualTo(2));
				Assert.That(namespaces.Any(_ => _.Name == typeof(TypeOfThis).Namespace), Is.True);
				Assert.That(namespaces.Any(_ => _.Name == typeof(MethodAttribute).Namespace), Is.True);
			});
		}

		[Test]
		public static void GetDescription()
		{
			var (attributes, compilation) = AttributeDataExtensionsTests.GetAttributes(
@"using Rocks.Tests.Extensions;
using System;

public interface IA
{
	[MyTest(""a value"", 12.34, 22, 44, typeof(Guid), new[] { 6, 7 }, MyValue.ThisOne, NamedA = 44)]
	void Foo();
}");

			Assert.Multiple(() =>
			{
				Assert.That(attributes[0].GetDescription(), Is.EqualTo(@"MyTest(""a value"", 12.34, 22, 44, typeof(Guid), new[] { 6, 7 }, (MyValue)0, NamedA = 44)"));
			});
		}

		[Test]
		public static void GetDescriptionWithOpenGeneric()
		{
			var (attributes, compilation) = AttributeDataExtensionsTests.GetAttributes(
@"using Rocks.Tests.Extensions;
using System;

public interface IA
{
	[MyTest(""a value"", 12.34, 22, 44, typeof(OpenGeneric<,>), new[] { 6, 7 }, MyValue.ThisOne, NamedA = 44)]
	void Foo();
}");

			Assert.Multiple(() =>
			{
				Assert.That(attributes[0].GetDescription(), Is.EqualTo(@"MyTest(""a value"", 12.34, 22, 44, typeof(OpenGeneric<, >), new[] { 6, 7 }, (MyValue)0, NamedA = 44)"));
			});
		}

		[Test]
		public static void GetDescriptionForArrayOfAttributes()
		{
			var (attributes, compilation) = AttributeDataExtensionsTests.GetAttributes(
@"using Rocks.Tests.Extensions;
using System;

public interface IA
{
	[MyTest(""a value"", 12.34, 22, 44, typeof(Guid), new[] { 6, 7 }, MyValue.ThisOne)]
	[MyTest(""b value"", 22.34, 33, 55, typeof(string), new[] { 8, 9 }, MyValue.ThatOne)]
	void Foo();
}");

			Assert.Multiple(() =>
			{
				Assert.That(attributes.GetDescription(compilation), 
					Is.EqualTo(@"[MyTest(""a value"", 12.34, 22, 44, typeof(Guid), new[] { 6, 7 }, (MyValue)0), MyTest(""b value"", 22.34, 33, 55, typeof(string), new[] { 8, 9 }, (MyValue)1)]"));
			});
		}

		[Test]
		public static void GetDescriptionForArrayWithTarget()
		{
			var (attributes, compilation) = AttributeDataExtensionsTests.GetAttributes(
@"using Rocks.Tests.Extensions;
using System;

public interface IA
{
	[MyTest(""a value"", 12.34, 22, 44, typeof(Guid), new[] { 6, 7 }, MyValue.ThisOne)]
	[MyTest(""b value"", 22.34, 33, 55, typeof(string), new[] { 8, 9 }, MyValue.ThatOne)]
	void Foo();
}");

			Assert.Multiple(() =>
			{
				Assert.That(attributes.GetDescription(compilation, AttributeTargets.Method), 
					Is.EqualTo(@"[method: MyTest(""a value"", 12.34, 22, 44, typeof(Guid), new[] { 6, 7 }, (MyValue)0), MyTest(""b value"", 22.34, 33, 55, typeof(string), new[] { 8, 9 }, (MyValue)1)]"));
			});
		}

		private static (ImmutableArray<AttributeData>, Compilation) GetAttributes(string source)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			var model = compilation.GetSemanticModel(syntaxTree, true);

			var methodSyntax = syntaxTree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>().Single();
			var methodSymbol = model.GetDeclaredSymbol(methodSyntax)!;

			return (methodSymbol.GetAttributes(), compilation);
		}
	}
}
