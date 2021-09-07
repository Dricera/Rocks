﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Rocks.Diagnostics;
using Rocks.Tests.Targets;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Rocks.Tests
{
	public static class RockCreateGeneratorTests
	{
		[Test]
		public static async Task GenerateWhenTargetTypeContainsCompilerGeneratedMembersAsync()
		{
			var code =
@"using Rocks;
using System;

namespace MockTests
{
	public interface IContainNullableReferences
	{
		string? DoSomething(string? a, string b) => string.Empty;
	}
	
	public static class Test
	{
		public static void Generate()
		{
			var rock = Rock.Create<IContainNullableReferences>();
		}
	}
}";

			var generatedCode =
@"using Rocks;
using Rocks.Exceptions;
using Rocks.Expectations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace MockTests
{
	internal static class CreateExpectationsOfIContainNullableReferencesExtensions
	{
		internal static MethodExpectations<IContainNullableReferences> Methods(this Expectations<IContainNullableReferences> self) =>
			new(self);
		
		internal static IContainNullableReferences Instance(this Expectations<IContainNullableReferences> self)
		{
			var mock = new RockIContainNullableReferences(self);
			self.Mocks.Add(mock);
			return mock;
		}
		
		private sealed class RockIContainNullableReferences
			: IContainNullableReferences, IMock
		{
			private readonly ImmutableDictionary<int, ImmutableArray<HandlerInformation>> handlers;
			
			public RockIContainNullableReferences(Expectations<IContainNullableReferences> expectations) =>
				this.handlers = expectations.CreateHandlers();
			
			[MemberIdentifier(0, ""string? DoSomething(string? a, string b)"")]
			public string? DoSomething(string? a, string b)
			{
				if (this.handlers.TryGetValue(0, out var methodHandlers))
				{
					foreach (var methodHandler in methodHandlers)
					{
						if (((methodHandler.Expectations[0] as Argument<string?>)?.IsValid(a) ?? false) &&
							((methodHandler.Expectations[1] as Argument<string>)?.IsValid(b) ?? false))
						{
							var result = methodHandler.Method is not null ?
								((Func<string?, string, string?>)methodHandler.Method)(a, b) :
								((HandlerInformation<string?>)methodHandler).ReturnValue;
							methodHandler.IncrementCallCount();
							return result!;
						}
					}
				}
				
				throw new ExpectationException(""No handlers were found for string? DoSomething(string? a, string b)"");
			}
			
			
			ImmutableDictionary<int, ImmutableArray<HandlerInformation>> IMock.Handlers => this.handlers;
		}
	}
	
	internal static class MethodExpectationsOfIContainNullableReferencesExtensions
	{
		internal static MethodAdornments<IContainNullableReferences, Func<string?, string, string?>, string?> DoSomething(this MethodExpectations<IContainNullableReferences> self, Argument<string?> a, Argument<string> b) =>
			new MethodAdornments<IContainNullableReferences, Func<string?, string, string?>, string?>(self.Add<string?>(0, new List<Argument> { a, b }));
	}
}
";

			await TestAssistants.RunAsync<RockCreateGenerator>(code,
				new[] { (typeof(RockCreateGenerator), "IContainNullableReferences_Rock_Create.g.cs", generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		public static async Task GenerateWhenTargetTypeIsValidAsync()
		{
			var code =
@"using Rocks;
using System;

namespace MockTests
{
	public interface ITest
	{
		void Foo();
	}

	public static class Test
	{
		public static void Generate()
		{
			var rock = Rock.Create<ITest>();
		}
	}
}";

			var generatedCode =
@"using Rocks;
using Rocks.Exceptions;
using Rocks.Expectations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace MockTests
{
	internal static class CreateExpectationsOfITestExtensions
	{
		internal static MethodExpectations<ITest> Methods(this Expectations<ITest> self) =>
			new(self);
		
		internal static ITest Instance(this Expectations<ITest> self)
		{
			var mock = new RockITest(self);
			self.Mocks.Add(mock);
			return mock;
		}
		
		private sealed class RockITest
			: ITest, IMock
		{
			private readonly ImmutableDictionary<int, ImmutableArray<HandlerInformation>> handlers;
			
			public RockITest(Expectations<ITest> expectations) =>
				this.handlers = expectations.CreateHandlers();
			
			[MemberIdentifier(0, ""void Foo()"")]
			public void Foo()
			{
				if (this.handlers.TryGetValue(0, out var methodHandlers))
				{
					var methodHandler = methodHandlers[0];
					if (methodHandler.Method is not null)
					{
						((Action)methodHandler.Method)();
					}
					
					methodHandler.IncrementCallCount();
				}
				else
				{
					throw new ExpectationException(""No handlers were found for void Foo())"");
				}
			}
			
			
			ImmutableDictionary<int, ImmutableArray<HandlerInformation>> IMock.Handlers => this.handlers;
		}
	}
	
	internal static class MethodExpectationsOfITestExtensions
	{
		internal static MethodAdornments<ITest, Action> Foo(this MethodExpectations<ITest> self) =>
			new MethodAdornments<ITest, Action>(self.Add(0, new List<Argument>()));
	}
}
";

			await TestAssistants.RunAsync<RockCreateGenerator>(code,
				new[] { (typeof(RockCreateGenerator), "ITest_Rock_Create.g.cs", generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		public static async Task GenerateWhenTargetTypeIsInGlobalNamespaceAsync()
		{
			var code =
@"using Rocks;
using System;

public static class Runner
{
	public static void Run() 
	{
		var rock = Rock.Create<ITest>();
	}
}

public interface ITest
{
	void Foo();
}";

			var generatedCode =
@"using Rocks;
using Rocks.Exceptions;
using Rocks.Expectations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
internal static class CreateExpectationsOfITestExtensions
{
	internal static MethodExpectations<ITest> Methods(this Expectations<ITest> self) =>
		new(self);
	
	internal static ITest Instance(this Expectations<ITest> self)
	{
		var mock = new RockITest(self);
		self.Mocks.Add(mock);
		return mock;
	}
	
	private sealed class RockITest
		: ITest, IMock
	{
		private readonly ImmutableDictionary<int, ImmutableArray<HandlerInformation>> handlers;
		
		public RockITest(Expectations<ITest> expectations) =>
			this.handlers = expectations.CreateHandlers();
		
		[MemberIdentifier(0, ""void Foo()"")]
		public void Foo()
		{
			if (this.handlers.TryGetValue(0, out var methodHandlers))
			{
				var methodHandler = methodHandlers[0];
				if (methodHandler.Method is not null)
				{
					((Action)methodHandler.Method)();
				}
				
				methodHandler.IncrementCallCount();
			}
			else
			{
				throw new ExpectationException(""No handlers were found for void Foo())"");
			}
		}
		
		
		ImmutableDictionary<int, ImmutableArray<HandlerInformation>> IMock.Handlers => this.handlers;
	}
}

internal static class MethodExpectationsOfITestExtensions
{
	internal static MethodAdornments<ITest, Action> Foo(this MethodExpectations<ITest> self) =>
		new MethodAdornments<ITest, Action>(self.Add(0, new List<Argument>()));
}
";

			await TestAssistants.RunAsync<RockCreateGenerator>(code,
				new[] { (typeof(RockCreateGenerator), "ITest_Rock_Create.g.cs", generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		public static async Task GenerateWhenTargetTypeIsValidForRockRepositoryAsync()
		{
			var code =
@"using Rocks;
using System;

namespace MockTests
{
	public interface ITest
	{
		void Foo();
	}

	public static class Test
	{
		public static void Generate()
		{
			var repository = new RockRepository();
			var rock = repository.Create<ITest>();
		}
	}
}";

			var generatedCode =
@"using Rocks;
using Rocks.Exceptions;
using Rocks.Expectations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace MockTests
{
	internal static class CreateExpectationsOfITestExtensions
	{
		internal static MethodExpectations<ITest> Methods(this Expectations<ITest> self) =>
			new(self);
		
		internal static ITest Instance(this Expectations<ITest> self)
		{
			var mock = new RockITest(self);
			self.Mocks.Add(mock);
			return mock;
		}
		
		private sealed class RockITest
			: ITest, IMock
		{
			private readonly ImmutableDictionary<int, ImmutableArray<HandlerInformation>> handlers;
			
			public RockITest(Expectations<ITest> expectations) =>
				this.handlers = expectations.CreateHandlers();
			
			[MemberIdentifier(0, ""void Foo()"")]
			public void Foo()
			{
				if (this.handlers.TryGetValue(0, out var methodHandlers))
				{
					var methodHandler = methodHandlers[0];
					if (methodHandler.Method is not null)
					{
						((Action)methodHandler.Method)();
					}
					
					methodHandler.IncrementCallCount();
				}
				else
				{
					throw new ExpectationException(""No handlers were found for void Foo())"");
				}
			}
			
			
			ImmutableDictionary<int, ImmutableArray<HandlerInformation>> IMock.Handlers => this.handlers;
		}
	}
	
	internal static class MethodExpectationsOfITestExtensions
	{
		internal static MethodAdornments<ITest, Action> Foo(this MethodExpectations<ITest> self) =>
			new MethodAdornments<ITest, Action>(self.Add(0, new List<Argument>()));
	}
}
";

			await TestAssistants.RunAsync<RockCreateGenerator>(code,
				new[] { (typeof(RockCreateGenerator), "ITest_Rock_Create.g.cs", generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		public static async Task GenerateWhenInvocationExistsInTopLevelStatementsAsync()
		{
			var code =
@"using MockTests;
using Rocks;
using System;

var rock = Rock.Create<ITest>();

namespace MockTests
{
	public interface ITest
	{
		void Foo();
	}
}";

			var generatedCode =
@"using Rocks;
using Rocks.Exceptions;
using Rocks.Expectations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace MockTests
{
	internal static class CreateExpectationsOfITestExtensions
	{
		internal static MethodExpectations<ITest> Methods(this Expectations<ITest> self) =>
			new(self);
		
		internal static ITest Instance(this Expectations<ITest> self)
		{
			var mock = new RockITest(self);
			self.Mocks.Add(mock);
			return mock;
		}
		
		private sealed class RockITest
			: ITest, IMock
		{
			private readonly ImmutableDictionary<int, ImmutableArray<HandlerInformation>> handlers;
			
			public RockITest(Expectations<ITest> expectations) =>
				this.handlers = expectations.CreateHandlers();
			
			[MemberIdentifier(0, ""void Foo()"")]
			public void Foo()
			{
				if (this.handlers.TryGetValue(0, out var methodHandlers))
				{
					var methodHandler = methodHandlers[0];
					if (methodHandler.Method is not null)
					{
						((Action)methodHandler.Method)();
					}
					
					methodHandler.IncrementCallCount();
				}
				else
				{
					throw new ExpectationException(""No handlers were found for void Foo())"");
				}
			}
			
			
			ImmutableDictionary<int, ImmutableArray<HandlerInformation>> IMock.Handlers => this.handlers;
		}
	}
	
	internal static class MethodExpectationsOfITestExtensions
	{
		internal static MethodAdornments<ITest, Action> Foo(this MethodExpectations<ITest> self) =>
			new MethodAdornments<ITest, Action>(self.Add(0, new List<Argument>()));
	}
}
";

			await TestAssistants.RunAsync<RockCreateGenerator>(code,
				new[] { (typeof(RockCreateGenerator), "ITest_Rock_Create.g.cs", generatedCode) },
				Enumerable.Empty<DiagnosticResult>(), OutputKind.ConsoleApplication);
		}

		[Test]
		public static async Task GenerateWhenTargetTypeIsInvalidAsync()
		{
			var code =
@"using Rocks;

namespace MockTests
{
	public interface ITest { }

	public static class Test
	{
		public static void Generate()
		{
			var rock = Rock.Create<ITest>();
		}
	}
}";

			var diagnostic = new DiagnosticResult(TypeHasNoMockableMembersDiagnostic.Id, DiagnosticSeverity.Error)
				.WithSpan(5, 19, 5, 24);
			await TestAssistants.RunAsync<RockCreateGenerator>(code,
				Enumerable.Empty<(Type, string, string)>(),
				new[] { diagnostic });
		}

		[Test]
		public static async Task GenerateWhenTargetTypeHasDiagnosticsAsync()
		{
			var code =
@"using Rocks;

namespace MockTests
{
	public interface ITest 
	{ 
		// Note the missing semicolon
		void Foo()
	}

	public static class Test
	{
		public static void Generate()
		{
			var rock = Rock.Create<ITest>();
		}
	}
}";

			var diagnostic = new DiagnosticResult("CS1002", DiagnosticSeverity.Error)
				.WithSpan(8, 13, 8, 13);
			await TestAssistants.RunAsync<RockCreateGenerator>(code,
				Enumerable.Empty<(Type, string, string)>(),
				new[] { diagnostic });
		}

		[Test]
		public static async Task GenerateWhenTargetTypeIsValidButOtherCodeHasDiagnosticsAsync()
		{
			var code =
@"using Rocks;

namespace MockTests
{
	public interface ITest 
	{ 
		void Foo();
	}

	public static class Test
	{
		public static void Generate()
		{
			var rock = Rock.Create<ITest>();
		}
// Note the missing closing brace
	}";

			var generatedCode =
@"using Rocks;
using Rocks.Exceptions;
using Rocks.Expectations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace MockTests
{
	internal static class CreateExpectationsOfITestExtensions
	{
		internal static MethodExpectations<ITest> Methods(this Expectations<ITest> self) =>
			new(self);
		
		internal static ITest Instance(this Expectations<ITest> self)
		{
			var mock = new RockITest(self);
			self.Mocks.Add(mock);
			return mock;
		}
		
		private sealed class RockITest
			: ITest, IMock
		{
			private readonly ImmutableDictionary<int, ImmutableArray<HandlerInformation>> handlers;
			
			public RockITest(Expectations<ITest> expectations) =>
				this.handlers = expectations.CreateHandlers();
			
			[MemberIdentifier(0, ""void Foo()"")]
			public void Foo()
			{
				if (this.handlers.TryGetValue(0, out var methodHandlers))
				{
					var methodHandler = methodHandlers[0];
					if (methodHandler.Method is not null)
					{
						((Action)methodHandler.Method)();
					}
					
					methodHandler.IncrementCallCount();
				}
				else
				{
					throw new ExpectationException(""No handlers were found for void Foo())"");
				}
			}
			
			
			ImmutableDictionary<int, ImmutableArray<HandlerInformation>> IMock.Handlers => this.handlers;
		}
	}
	
	internal static class MethodExpectationsOfITestExtensions
	{
		internal static MethodAdornments<ITest, Action> Foo(this MethodExpectations<ITest> self) =>
			new MethodAdornments<ITest, Action>(self.Add(0, new List<Argument>()));
	}
}
";

			var diagnostic = new DiagnosticResult("CS1513", DiagnosticSeverity.Error)
				.WithSpan(17, 3, 17, 3);
			await TestAssistants.RunAsync<RockCreateGenerator>(code,
				new[] { (typeof(RockCreateGenerator), "ITest_Rock_Create.g.cs", generatedCode) },
				new[] { diagnostic });
		}
	}
}