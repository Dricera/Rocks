﻿using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Rocks.Tests.Generators;

public static class NamingGeneratorTests
{
	[Test]
	public static async Task GenerateWhenNonVirtualMembersAreNeededAsync()
	{
		var code =
			"""
			using Rocks;
			using System;
			using System.IO;

			public class IHaveDelegate
			{
				public Func<Stream, Stream> Processor { get; init; }   
				public virtual void Foo() { }
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<IHaveDelegate>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfIHaveDelegateExtensions
			{
				internal static global::Rocks.Expectations.MethodExpectations<global::IHaveDelegate> Methods(this global::Rocks.Expectations.Expectations<global::IHaveDelegate> @self) =>
					new(@self);
				
				internal sealed class ConstructorProperties
				{
					internal global::System.Func<global::System.IO.Stream, global::System.IO.Stream>? Processor { get; init; }
				}
				
				internal static global::IHaveDelegate Instance(this global::Rocks.Expectations.Expectations<global::IHaveDelegate> @self, ConstructorProperties? @constructorProperties)
				{
					if (!@self.WasInstanceInvoked)
					{
						@self.WasInstanceInvoked = true;
						return new RockIHaveDelegate(@self, @constructorProperties);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockIHaveDelegate
					: global::IHaveDelegate
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					public RockIHaveDelegate(global::Rocks.Expectations.Expectations<global::IHaveDelegate> @expectations, ConstructorProperties? @constructorProperties)
					{
						this.handlers = @expectations.Handlers;
						if (@constructorProperties is not null)
						{
							this.Processor = @constructorProperties.Processor!;
						}
					}
					
					[global::Rocks.MemberIdentifier(0, "bool Equals(object? @obj)")]
					public override bool Equals(object? @obj)
					{
						if (this.handlers.TryGetValue(0, out var @methodHandlers))
						{
							foreach (var @methodHandler in @methodHandlers)
							{
								if (global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<object?>>(@methodHandler.Expectations[0]).IsValid(@obj))
								{
									var @result = @methodHandler.Method is not null ?
										global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<object?, bool>>(@methodHandler.Method)(@obj) :
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<bool>>(@methodHandler).ReturnValue;
									@methodHandler.IncrementCallCount();
									return @result!;
								}
							}
							
							throw new global::Rocks.Exceptions.ExpectationException("No handlers match for bool Equals(object? @obj)");
						}
						else
						{
							return base.Equals(@obj);
						}
					}
					
					[global::Rocks.MemberIdentifier(1, "int GetHashCode()")]
					public override int GetHashCode()
					{
						if (this.handlers.TryGetValue(1, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							var @result = @methodHandler.Method is not null ?
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<int>>(@methodHandler.Method)() :
								global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<int>>(@methodHandler).ReturnValue;
							@methodHandler.IncrementCallCount();
							return @result!;
						}
						else
						{
							return base.GetHashCode();
						}
					}
					
					[global::Rocks.MemberIdentifier(2, "string? ToString()")]
					public override string? ToString()
					{
						if (this.handlers.TryGetValue(2, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							var @result = @methodHandler.Method is not null ?
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<string?>>(@methodHandler.Method)() :
								global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<string?>>(@methodHandler).ReturnValue;
							@methodHandler.IncrementCallCount();
							return @result!;
						}
						else
						{
							return base.ToString();
						}
					}
					
					[global::Rocks.MemberIdentifier(3, "void Foo()")]
					public override void Foo()
					{
						if (this.handlers.TryGetValue(3, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							if (@methodHandler.Method is not null)
							{
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Action>(@methodHandler.Method)();
							}
							
							@methodHandler.IncrementCallCount();
						}
						else
						{
							base.Foo();
						}
					}
					
				}
			}
			
			internal static class MethodExpectationsOfIHaveDelegateExtensions
			{
				internal static global::Rocks.MethodAdornments<global::IHaveDelegate, global::System.Func<object?, bool>, bool> Equals(this global::Rocks.Expectations.MethodExpectations<global::IHaveDelegate> @self, global::Rocks.Argument<object?> @obj)
				{
					global::System.ArgumentNullException.ThrowIfNull(@obj);
					return new global::Rocks.MethodAdornments<global::IHaveDelegate, global::System.Func<object?, bool>, bool>(@self.Add<bool>(0, new global::System.Collections.Generic.List<global::Rocks.Argument>(1) { @obj }));
				}
				internal static global::Rocks.MethodAdornments<global::IHaveDelegate, global::System.Func<int>, int> GetHashCode(this global::Rocks.Expectations.MethodExpectations<global::IHaveDelegate> @self) =>
					new global::Rocks.MethodAdornments<global::IHaveDelegate, global::System.Func<int>, int>(@self.Add<int>(1, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
				internal static global::Rocks.MethodAdornments<global::IHaveDelegate, global::System.Func<string?>, string?> ToString(this global::Rocks.Expectations.MethodExpectations<global::IHaveDelegate> @self) =>
					new global::Rocks.MethodAdornments<global::IHaveDelegate, global::System.Func<string?>, string?>(@self.Add<string?>(2, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
				internal static global::Rocks.MethodAdornments<global::IHaveDelegate, global::System.Action> Foo(this global::Rocks.Expectations.MethodExpectations<global::IHaveDelegate> @self) =>
					new global::Rocks.MethodAdornments<global::IHaveDelegate, global::System.Action>(@self.Add(3, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IHaveDelegate_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenNamesAreKeywordsAsync()
	{
		var code =
			"""
			using Rocks;
			
			public interface IUseKeyword
			{
			    void Foo(string @namespace, string @event, string @property);   
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<IUseKeyword>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfIUseKeywordExtensions
			{
				internal static global::Rocks.Expectations.MethodExpectations<global::IUseKeyword> Methods(this global::Rocks.Expectations.Expectations<global::IUseKeyword> @self) =>
					new(@self);
				
				internal static global::IUseKeyword Instance(this global::Rocks.Expectations.Expectations<global::IUseKeyword> @self)
				{
					if (!@self.WasInstanceInvoked)
					{
						@self.WasInstanceInvoked = true;
						return new RockIUseKeyword(@self);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockIUseKeyword
					: global::IUseKeyword
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					public RockIUseKeyword(global::Rocks.Expectations.Expectations<global::IUseKeyword> @expectations)
					{
						this.handlers = @expectations.Handlers;
					}
					
					[global::Rocks.MemberIdentifier(0, "void Foo(string @namespace, string @event, string @property)")]
					public void Foo(string @namespace, string @event, string @property)
					{
						if (this.handlers.TryGetValue(0, out var @methodHandlers))
						{
							var @foundMatch = false;
							
							foreach (var @methodHandler in @methodHandlers)
							{
								if (global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler.Expectations[0]).IsValid(@namespace) &&
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler.Expectations[1]).IsValid(@event) &&
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler.Expectations[2]).IsValid(@property))
								{
									@foundMatch = true;
									
									if (@methodHandler.Method is not null)
									{
										global::System.Runtime.CompilerServices.Unsafe.As<global::System.Action<string, string, string>>(@methodHandler.Method)(@namespace, @event, @property);
									}
									
									@methodHandler.IncrementCallCount();
									break;
								}
							}
							
							if (!@foundMatch)
							{
								throw new global::Rocks.Exceptions.ExpectationException("No handlers match for void Foo(string @namespace, string @event, string @property)");
							}
						}
						else
						{
							throw new global::Rocks.Exceptions.ExpectationException("No handlers were found for void Foo(string @namespace, string @event, string @property)");
						}
					}
					
				}
			}
			
			internal static class MethodExpectationsOfIUseKeywordExtensions
			{
				internal static global::Rocks.MethodAdornments<global::IUseKeyword, global::System.Action<string, string, string>> Foo(this global::Rocks.Expectations.MethodExpectations<global::IUseKeyword> @self, global::Rocks.Argument<string> @namespace, global::Rocks.Argument<string> @event, global::Rocks.Argument<string> @property)
				{
					global::System.ArgumentNullException.ThrowIfNull(@namespace);
					global::System.ArgumentNullException.ThrowIfNull(@event);
					global::System.ArgumentNullException.ThrowIfNull(@property);
					return new global::Rocks.MethodAdornments<global::IUseKeyword, global::System.Action<string, string, string>>(@self.Add(0, new global::System.Collections.Generic.List<global::Rocks.Argument>(3) { @namespace, @event, @property }));
				}
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IUseKeyword_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithConstructorWithSelfNameAsync()
	{
		var code = 
			"""
			using Rocks;
			
			public class HaveNamingConflicts
			{
				public HaveNamingConflicts(string self, string expectations) { }
				public virtual void Foo() { }
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<HaveNamingConflicts>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfHaveNamingConflictsExtensions
			{
				internal static global::Rocks.Expectations.MethodExpectations<global::HaveNamingConflicts> Methods(this global::Rocks.Expectations.Expectations<global::HaveNamingConflicts> @self) =>
					new(@self);
				
				internal static global::HaveNamingConflicts Instance(this global::Rocks.Expectations.Expectations<global::HaveNamingConflicts> @self1, string @self, string @expectations)
				{
					if (!@self1.WasInstanceInvoked)
					{
						@self1.WasInstanceInvoked = true;
						return new RockHaveNamingConflicts(@self1, @self, @expectations);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockHaveNamingConflicts
					: global::HaveNamingConflicts
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					public RockHaveNamingConflicts(global::Rocks.Expectations.Expectations<global::HaveNamingConflicts> @expectations1, string @self, string @expectations)
						: base(@self, @expectations)
					{
						this.handlers = @expectations1.Handlers;
					}
					
					[global::Rocks.MemberIdentifier(0, "bool Equals(object? @obj)")]
					public override bool Equals(object? @obj)
					{
						if (this.handlers.TryGetValue(0, out var @methodHandlers))
						{
							foreach (var @methodHandler in @methodHandlers)
							{
								if (global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<object?>>(@methodHandler.Expectations[0]).IsValid(@obj))
								{
									var @result = @methodHandler.Method is not null ?
										global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<object?, bool>>(@methodHandler.Method)(@obj) :
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<bool>>(@methodHandler).ReturnValue;
									@methodHandler.IncrementCallCount();
									return @result!;
								}
							}
							
							throw new global::Rocks.Exceptions.ExpectationException("No handlers match for bool Equals(object? @obj)");
						}
						else
						{
							return base.Equals(@obj);
						}
					}
					
					[global::Rocks.MemberIdentifier(1, "int GetHashCode()")]
					public override int GetHashCode()
					{
						if (this.handlers.TryGetValue(1, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							var @result = @methodHandler.Method is not null ?
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<int>>(@methodHandler.Method)() :
								global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<int>>(@methodHandler).ReturnValue;
							@methodHandler.IncrementCallCount();
							return @result!;
						}
						else
						{
							return base.GetHashCode();
						}
					}
					
					[global::Rocks.MemberIdentifier(2, "string? ToString()")]
					public override string? ToString()
					{
						if (this.handlers.TryGetValue(2, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							var @result = @methodHandler.Method is not null ?
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<string?>>(@methodHandler.Method)() :
								global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<string?>>(@methodHandler).ReturnValue;
							@methodHandler.IncrementCallCount();
							return @result!;
						}
						else
						{
							return base.ToString();
						}
					}
					
					[global::Rocks.MemberIdentifier(3, "void Foo()")]
					public override void Foo()
					{
						if (this.handlers.TryGetValue(3, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							if (@methodHandler.Method is not null)
							{
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Action>(@methodHandler.Method)();
							}
							
							@methodHandler.IncrementCallCount();
						}
						else
						{
							base.Foo();
						}
					}
					
				}
			}
			
			internal static class MethodExpectationsOfHaveNamingConflictsExtensions
			{
				internal static global::Rocks.MethodAdornments<global::HaveNamingConflicts, global::System.Func<object?, bool>, bool> Equals(this global::Rocks.Expectations.MethodExpectations<global::HaveNamingConflicts> @self, global::Rocks.Argument<object?> @obj)
				{
					global::System.ArgumentNullException.ThrowIfNull(@obj);
					return new global::Rocks.MethodAdornments<global::HaveNamingConflicts, global::System.Func<object?, bool>, bool>(@self.Add<bool>(0, new global::System.Collections.Generic.List<global::Rocks.Argument>(1) { @obj }));
				}
				internal static global::Rocks.MethodAdornments<global::HaveNamingConflicts, global::System.Func<int>, int> GetHashCode(this global::Rocks.Expectations.MethodExpectations<global::HaveNamingConflicts> @self) =>
					new global::Rocks.MethodAdornments<global::HaveNamingConflicts, global::System.Func<int>, int>(@self.Add<int>(1, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
				internal static global::Rocks.MethodAdornments<global::HaveNamingConflicts, global::System.Func<string?>, string?> ToString(this global::Rocks.Expectations.MethodExpectations<global::HaveNamingConflicts> @self) =>
					new global::Rocks.MethodAdornments<global::HaveNamingConflicts, global::System.Func<string?>, string?>(@self.Add<string?>(2, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
				internal static global::Rocks.MethodAdornments<global::HaveNamingConflicts, global::System.Action> Foo(this global::Rocks.Expectations.MethodExpectations<global::HaveNamingConflicts> @self) =>
					new global::Rocks.MethodAdornments<global::HaveNamingConflicts, global::System.Action>(@self.Add(3, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "HaveNamingConflicts_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenIndexerParameterNamesMatchVariablesAsync()
	{
		var code =
			"""
			using Rocks;
			
			public interface IHaveNamingConflicts
			{
				int this[string methodHandlers, string methodHandler, string result, string result2, string self] { get; }
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<IHaveNamingConflicts>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfIHaveNamingConflictsExtensions
			{
				internal static global::Rocks.Expectations.IndexerExpectations<global::IHaveNamingConflicts> Indexers(this global::Rocks.Expectations.Expectations<global::IHaveNamingConflicts> @self) =>
					new(@self);
				
				internal static global::Rocks.Expectations.IndexerGetterExpectations<global::IHaveNamingConflicts> Getters(this global::Rocks.Expectations.IndexerExpectations<global::IHaveNamingConflicts> @self) =>
					new(@self);
				
				internal static global::IHaveNamingConflicts Instance(this global::Rocks.Expectations.Expectations<global::IHaveNamingConflicts> @self)
				{
					if (!@self.WasInstanceInvoked)
					{
						@self.WasInstanceInvoked = true;
						return new RockIHaveNamingConflicts(@self);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockIHaveNamingConflicts
					: global::IHaveNamingConflicts
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					public RockIHaveNamingConflicts(global::Rocks.Expectations.Expectations<global::IHaveNamingConflicts> @expectations)
					{
						this.handlers = @expectations.Handlers;
					}
					
					[global::Rocks.MemberIdentifier(0, "this[string @methodHandlers, string @methodHandler, string @result, string @result2, string @self]")]
					public int this[string @methodHandlers, string @methodHandler, string @result, string @result2, string @self]
					{
						get
						{
							if (this.handlers.TryGetValue(0, out var @methodHandlers1))
							{
								foreach (var @methodHandler1 in @methodHandlers1)
								{
									if (global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[0]).IsValid(@methodHandlers) &&
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[1]).IsValid(@methodHandler) &&
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[2]).IsValid(@result) &&
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[3]).IsValid(@result2) &&
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[4]).IsValid(@self))
									{
										var @result1 = @methodHandler1.Method is not null ?
											global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<string, string, string, string, string, int>>(@methodHandler1.Method)(@methodHandlers, @methodHandler, @result, @result2, @self) :
											global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<int>>(@methodHandler1).ReturnValue;
										@methodHandler1.IncrementCallCount();
										return @result1!;
									}
								}
								
								throw new global::Rocks.Exceptions.ExpectationException("No handlers match for this[string @methodHandlers, string @methodHandler, string @result, string @result2, string @self]");
							}
							
							throw new global::Rocks.Exceptions.ExpectationException("No handlers were found for this[string @methodHandlers, string @methodHandler, string @result, string @result2, string @self])");
						}
					}
				}
			}
			
			internal static class IndexerGetterExpectationsOfIHaveNamingConflictsExtensions
			{
				internal static global::Rocks.IndexerAdornments<global::IHaveNamingConflicts, global::System.Func<string, string, string, string, string, int>, int> This(this global::Rocks.Expectations.IndexerGetterExpectations<global::IHaveNamingConflicts> @self1, global::Rocks.Argument<string> @methodHandlers, global::Rocks.Argument<string> @methodHandler, global::Rocks.Argument<string> @result, global::Rocks.Argument<string> @result2, global::Rocks.Argument<string> @self)
				{
					global::System.ArgumentNullException.ThrowIfNull(@methodHandlers);
					global::System.ArgumentNullException.ThrowIfNull(@methodHandler);
					global::System.ArgumentNullException.ThrowIfNull(@result);
					global::System.ArgumentNullException.ThrowIfNull(@result2);
					global::System.ArgumentNullException.ThrowIfNull(@self);
					return new global::Rocks.IndexerAdornments<global::IHaveNamingConflicts, global::System.Func<string, string, string, string, string, int>, int>(@self1.Add<int>(0, new global::System.Collections.Generic.List<global::Rocks.Argument>(5) { @methodHandlers, @methodHandler, @result, @result2, @self }));
				}
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IHaveNamingConflicts_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenMethodParameterNamesMatchVariablesAsync()
	{
		var code =
			"""
			using Rocks;
			
			public interface IHaveNamingConflicts
			{
				int Foo(string methodHandlers, string methodHandler, string result, string result2, string self);
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<IHaveNamingConflicts>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfIHaveNamingConflictsExtensions
			{
				internal static global::Rocks.Expectations.MethodExpectations<global::IHaveNamingConflicts> Methods(this global::Rocks.Expectations.Expectations<global::IHaveNamingConflicts> @self) =>
					new(@self);
				
				internal static global::IHaveNamingConflicts Instance(this global::Rocks.Expectations.Expectations<global::IHaveNamingConflicts> @self)
				{
					if (!@self.WasInstanceInvoked)
					{
						@self.WasInstanceInvoked = true;
						return new RockIHaveNamingConflicts(@self);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockIHaveNamingConflicts
					: global::IHaveNamingConflicts
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					public RockIHaveNamingConflicts(global::Rocks.Expectations.Expectations<global::IHaveNamingConflicts> @expectations)
					{
						this.handlers = @expectations.Handlers;
					}
					
					[global::Rocks.MemberIdentifier(0, "int Foo(string @methodHandlers, string @methodHandler, string @result, string @result2, string @self)")]
					public int Foo(string @methodHandlers, string @methodHandler, string @result, string @result2, string @self)
					{
						if (this.handlers.TryGetValue(0, out var @methodHandlers1))
						{
							foreach (var @methodHandler1 in @methodHandlers1)
							{
								if (global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[0]).IsValid(@methodHandlers) &&
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[1]).IsValid(@methodHandler) &&
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[2]).IsValid(@result) &&
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[3]).IsValid(@result2) &&
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<string>>(@methodHandler1.Expectations[4]).IsValid(@self))
								{
									var @result1 = @methodHandler1.Method is not null ?
										global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<string, string, string, string, string, int>>(@methodHandler1.Method)(@methodHandlers, @methodHandler, @result, @result2, @self) :
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<int>>(@methodHandler1).ReturnValue;
									@methodHandler1.IncrementCallCount();
									return @result1!;
								}
							}
							
							throw new global::Rocks.Exceptions.ExpectationException("No handlers match for int Foo(string @methodHandlers, string @methodHandler, string @result, string @result2, string @self)");
						}
						
						throw new global::Rocks.Exceptions.ExpectationException("No handlers were found for int Foo(string @methodHandlers, string @methodHandler, string @result, string @result2, string @self)");
					}
					
				}
			}
			
			internal static class MethodExpectationsOfIHaveNamingConflictsExtensions
			{
				internal static global::Rocks.MethodAdornments<global::IHaveNamingConflicts, global::System.Func<string, string, string, string, string, int>, int> Foo(this global::Rocks.Expectations.MethodExpectations<global::IHaveNamingConflicts> @self1, global::Rocks.Argument<string> @methodHandlers, global::Rocks.Argument<string> @methodHandler, global::Rocks.Argument<string> @result, global::Rocks.Argument<string> @result2, global::Rocks.Argument<string> @self)
				{
					global::System.ArgumentNullException.ThrowIfNull(@methodHandlers);
					global::System.ArgumentNullException.ThrowIfNull(@methodHandler);
					global::System.ArgumentNullException.ThrowIfNull(@result);
					global::System.ArgumentNullException.ThrowIfNull(@result2);
					global::System.ArgumentNullException.ThrowIfNull(@self);
					return new global::Rocks.MethodAdornments<global::IHaveNamingConflicts, global::System.Func<string, string, string, string, string, int>, int>(@self1.Add<int>(0, new global::System.Collections.Generic.List<global::Rocks.Argument>(5) { @methodHandlers, @methodHandler, @result, @result2, @self }));
				}
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IHaveNamingConflicts_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenConstructorHasParameterNameThatMatchesConstructorPropertiesAsync()
	{
		var code =
			"""
			using Rocks;
			
			public class HasRequiredProperty
			{
				public HasRequiredProperty(string constructorProperties) { }

				public virtual void Foo() { }

				public required string Data { get; set; }
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<HasRequiredProperty>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfHasRequiredPropertyExtensions
			{
				internal static global::Rocks.Expectations.MethodExpectations<global::HasRequiredProperty> Methods(this global::Rocks.Expectations.Expectations<global::HasRequiredProperty> @self) =>
					new(@self);
				
				internal sealed class ConstructorProperties
				{
					internal required string? Data { get; init; }
				}
				
				internal static global::HasRequiredProperty Instance(this global::Rocks.Expectations.Expectations<global::HasRequiredProperty> @self, ConstructorProperties @constructorProperties1, string @constructorProperties)
				{
					if (@constructorProperties1 is null)
					{
						throw new global::System.ArgumentNullException(nameof(@constructorProperties1));
					}
					if (!@self.WasInstanceInvoked)
					{
						@self.WasInstanceInvoked = true;
						return new RockHasRequiredProperty(@self, @constructorProperties1, @constructorProperties);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockHasRequiredProperty
					: global::HasRequiredProperty
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					[global::System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
					public RockHasRequiredProperty(global::Rocks.Expectations.Expectations<global::HasRequiredProperty> @expectations, ConstructorProperties @constructorProperties1, string @constructorProperties)
						: base(@constructorProperties)
					{
						this.handlers = @expectations.Handlers;
						this.Data = @constructorProperties1.Data!;
					}
					
					[global::Rocks.MemberIdentifier(0, "bool Equals(object? @obj)")]
					public override bool Equals(object? @obj)
					{
						if (this.handlers.TryGetValue(0, out var @methodHandlers))
						{
							foreach (var @methodHandler in @methodHandlers)
							{
								if (global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<object?>>(@methodHandler.Expectations[0]).IsValid(@obj))
								{
									var @result = @methodHandler.Method is not null ?
										global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<object?, bool>>(@methodHandler.Method)(@obj) :
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<bool>>(@methodHandler).ReturnValue;
									@methodHandler.IncrementCallCount();
									return @result!;
								}
							}
							
							throw new global::Rocks.Exceptions.ExpectationException("No handlers match for bool Equals(object? @obj)");
						}
						else
						{
							return base.Equals(@obj);
						}
					}
					
					[global::Rocks.MemberIdentifier(1, "int GetHashCode()")]
					public override int GetHashCode()
					{
						if (this.handlers.TryGetValue(1, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							var @result = @methodHandler.Method is not null ?
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<int>>(@methodHandler.Method)() :
								global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<int>>(@methodHandler).ReturnValue;
							@methodHandler.IncrementCallCount();
							return @result!;
						}
						else
						{
							return base.GetHashCode();
						}
					}
					
					[global::Rocks.MemberIdentifier(2, "string? ToString()")]
					public override string? ToString()
					{
						if (this.handlers.TryGetValue(2, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							var @result = @methodHandler.Method is not null ?
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<string?>>(@methodHandler.Method)() :
								global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<string?>>(@methodHandler).ReturnValue;
							@methodHandler.IncrementCallCount();
							return @result!;
						}
						else
						{
							return base.ToString();
						}
					}
					
					[global::Rocks.MemberIdentifier(3, "void Foo()")]
					public override void Foo()
					{
						if (this.handlers.TryGetValue(3, out var @methodHandlers))
						{
							var @methodHandler = @methodHandlers[0];
							if (@methodHandler.Method is not null)
							{
								global::System.Runtime.CompilerServices.Unsafe.As<global::System.Action>(@methodHandler.Method)();
							}
							
							@methodHandler.IncrementCallCount();
						}
						else
						{
							base.Foo();
						}
					}
					
				}
			}
			
			internal static class MethodExpectationsOfHasRequiredPropertyExtensions
			{
				internal static global::Rocks.MethodAdornments<global::HasRequiredProperty, global::System.Func<object?, bool>, bool> Equals(this global::Rocks.Expectations.MethodExpectations<global::HasRequiredProperty> @self, global::Rocks.Argument<object?> @obj)
				{
					global::System.ArgumentNullException.ThrowIfNull(@obj);
					return new global::Rocks.MethodAdornments<global::HasRequiredProperty, global::System.Func<object?, bool>, bool>(@self.Add<bool>(0, new global::System.Collections.Generic.List<global::Rocks.Argument>(1) { @obj }));
				}
				internal static global::Rocks.MethodAdornments<global::HasRequiredProperty, global::System.Func<int>, int> GetHashCode(this global::Rocks.Expectations.MethodExpectations<global::HasRequiredProperty> @self) =>
					new global::Rocks.MethodAdornments<global::HasRequiredProperty, global::System.Func<int>, int>(@self.Add<int>(1, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
				internal static global::Rocks.MethodAdornments<global::HasRequiredProperty, global::System.Func<string?>, string?> ToString(this global::Rocks.Expectations.MethodExpectations<global::HasRequiredProperty> @self) =>
					new global::Rocks.MethodAdornments<global::HasRequiredProperty, global::System.Func<string?>, string?>(@self.Add<string?>(2, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
				internal static global::Rocks.MethodAdornments<global::HasRequiredProperty, global::System.Action> Foo(this global::Rocks.Expectations.MethodExpectations<global::HasRequiredProperty> @self) =>
					new global::Rocks.MethodAdornments<global::HasRequiredProperty, global::System.Action>(@self.Add(3, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "HasRequiredProperty_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithMultipleNamespacesAsync()
	{
		var code =
			"""
			using Rocks;
			
			namespace Namespace1
			{
			  public class Thing { }
			  public class Stuff { }
			}

			namespace Namespace2
			{
			  public class Thing { }
			}

			public interface IUsesThing
			{
			  void Use(Namespace2.Thing thing, Namespace1.Stuff stuff);
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<IUsesThing>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfIUsesThingExtensions
			{
				internal static global::Rocks.Expectations.MethodExpectations<global::IUsesThing> Methods(this global::Rocks.Expectations.Expectations<global::IUsesThing> @self) =>
					new(@self);
				
				internal static global::IUsesThing Instance(this global::Rocks.Expectations.Expectations<global::IUsesThing> @self)
				{
					if (!@self.WasInstanceInvoked)
					{
						@self.WasInstanceInvoked = true;
						return new RockIUsesThing(@self);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockIUsesThing
					: global::IUsesThing
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					public RockIUsesThing(global::Rocks.Expectations.Expectations<global::IUsesThing> @expectations)
					{
						this.handlers = @expectations.Handlers;
					}
					
					[global::Rocks.MemberIdentifier(0, "void Use(global::Namespace2.Thing @thing, global::Namespace1.Stuff @stuff)")]
					public void Use(global::Namespace2.Thing @thing, global::Namespace1.Stuff @stuff)
					{
						if (this.handlers.TryGetValue(0, out var @methodHandlers))
						{
							var @foundMatch = false;
							
							foreach (var @methodHandler in @methodHandlers)
							{
								if (global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<global::Namespace2.Thing>>(@methodHandler.Expectations[0]).IsValid(@thing) &&
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<global::Namespace1.Stuff>>(@methodHandler.Expectations[1]).IsValid(@stuff))
								{
									@foundMatch = true;
									
									if (@methodHandler.Method is not null)
									{
										global::System.Runtime.CompilerServices.Unsafe.As<global::System.Action<global::Namespace2.Thing, global::Namespace1.Stuff>>(@methodHandler.Method)(@thing, @stuff);
									}
									
									@methodHandler.IncrementCallCount();
									break;
								}
							}
							
							if (!@foundMatch)
							{
								throw new global::Rocks.Exceptions.ExpectationException("No handlers match for void Use(global::Namespace2.Thing @thing, global::Namespace1.Stuff @stuff)");
							}
						}
						else
						{
							throw new global::Rocks.Exceptions.ExpectationException("No handlers were found for void Use(global::Namespace2.Thing @thing, global::Namespace1.Stuff @stuff)");
						}
					}
					
				}
			}
			
			internal static class MethodExpectationsOfIUsesThingExtensions
			{
				internal static global::Rocks.MethodAdornments<global::IUsesThing, global::System.Action<global::Namespace2.Thing, global::Namespace1.Stuff>> Use(this global::Rocks.Expectations.MethodExpectations<global::IUsesThing> @self, global::Rocks.Argument<global::Namespace2.Thing> @thing, global::Rocks.Argument<global::Namespace1.Stuff> @stuff)
				{
					global::System.ArgumentNullException.ThrowIfNull(@thing);
					global::System.ArgumentNullException.ThrowIfNull(@stuff);
					return new global::Rocks.MethodAdornments<global::IUsesThing, global::System.Action<global::Namespace2.Thing, global::Namespace1.Stuff>>(@self.Add(0, new global::System.Collections.Generic.List<global::Rocks.Argument>(2) { @thing, @stuff }));
				}
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IUsesThing_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithArrayAsync()
	{
		var code =
			"""
			using Rocks;
			using System;

			public class MethodInformation { }

			public interface IUseMethodInformation
			{
				MethodInformation[] Methods { get; }
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<IUseMethodInformation>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfIUseMethodInformationExtensions
			{
				internal static global::Rocks.Expectations.PropertyExpectations<global::IUseMethodInformation> Properties(this global::Rocks.Expectations.Expectations<global::IUseMethodInformation> @self) =>
					new(@self);
				
				internal static global::Rocks.Expectations.PropertyGetterExpectations<global::IUseMethodInformation> Getters(this global::Rocks.Expectations.PropertyExpectations<global::IUseMethodInformation> @self) =>
					new(@self);
				
				internal static global::IUseMethodInformation Instance(this global::Rocks.Expectations.Expectations<global::IUseMethodInformation> @self)
				{
					if (!@self.WasInstanceInvoked)
					{
						@self.WasInstanceInvoked = true;
						return new RockIUseMethodInformation(@self);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockIUseMethodInformation
					: global::IUseMethodInformation
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					public RockIUseMethodInformation(global::Rocks.Expectations.Expectations<global::IUseMethodInformation> @expectations)
					{
						this.handlers = @expectations.Handlers;
					}
					
					[global::Rocks.MemberIdentifier(0, "get_Methods()")]
					public global::MethodInformation[] Methods
					{
						get
						{
							if (this.handlers.TryGetValue(0, out var @methodHandlers))
							{
								var @methodHandler = @methodHandlers[0];
								var @result = @methodHandler.Method is not null ?
									global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<global::MethodInformation[]>>(@methodHandler.Method)() :
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<global::MethodInformation[]>>(@methodHandler).ReturnValue;
								@methodHandler.IncrementCallCount();
								return @result!;
							}
							
							throw new global::Rocks.Exceptions.ExpectationException("No handlers were found for get_Methods())");
						}
					}
				}
			}
			
			internal static class PropertyGetterExpectationsOfIUseMethodInformationExtensions
			{
				internal static global::Rocks.PropertyAdornments<global::IUseMethodInformation, global::System.Func<global::MethodInformation[]>, global::MethodInformation[]> Methods(this global::Rocks.Expectations.PropertyGetterExpectations<global::IUseMethodInformation> @self) =>
					new global::Rocks.PropertyAdornments<global::IUseMethodInformation, global::System.Func<global::MethodInformation[]>, global::MethodInformation[]>(@self.Add<global::MethodInformation[]>(0, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IUseMethodInformation_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithNestedTypeAsync()
	{
		var code =
			"""
			using Rocks;
			using System;

			public interface IOperation
			{
				public struct OperationList { }

				OperationList Operations { get; }
			}

			public static class Test
			{
				public static void Go()
				{
					var expectations = Rock.Create<IOperation>();
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			internal static class CreateExpectationsOfIOperationExtensions
			{
				internal static global::Rocks.Expectations.PropertyExpectations<global::IOperation> Properties(this global::Rocks.Expectations.Expectations<global::IOperation> @self) =>
					new(@self);
				
				internal static global::Rocks.Expectations.PropertyGetterExpectations<global::IOperation> Getters(this global::Rocks.Expectations.PropertyExpectations<global::IOperation> @self) =>
					new(@self);
				
				internal static global::IOperation Instance(this global::Rocks.Expectations.Expectations<global::IOperation> @self)
				{
					if (!@self.WasInstanceInvoked)
					{
						@self.WasInstanceInvoked = true;
						return new RockIOperation(@self);
					}
					else
					{
						throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
					}
				}
				
				private sealed class RockIOperation
					: global::IOperation
				{
					private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
					
					public RockIOperation(global::Rocks.Expectations.Expectations<global::IOperation> @expectations)
					{
						this.handlers = @expectations.Handlers;
					}
					
					[global::Rocks.MemberIdentifier(0, "get_Operations()")]
					public global::IOperation.OperationList Operations
					{
						get
						{
							if (this.handlers.TryGetValue(0, out var @methodHandlers))
							{
								var @methodHandler = @methodHandlers[0];
								var @result = @methodHandler.Method is not null ?
									global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<global::IOperation.OperationList>>(@methodHandler.Method)() :
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<global::IOperation.OperationList>>(@methodHandler).ReturnValue;
								@methodHandler.IncrementCallCount();
								return @result!;
							}
							
							throw new global::Rocks.Exceptions.ExpectationException("No handlers were found for get_Operations())");
						}
					}
				}
			}
			
			internal static class PropertyGetterExpectationsOfIOperationExtensions
			{
				internal static global::Rocks.PropertyAdornments<global::IOperation, global::System.Func<global::IOperation.OperationList>, global::IOperation.OperationList> Operations(this global::Rocks.Expectations.PropertyGetterExpectations<global::IOperation> @self) =>
					new global::Rocks.PropertyAdornments<global::IOperation, global::System.Func<global::IOperation.OperationList>, global::IOperation.OperationList>(@self.Add<global::IOperation.OperationList>(0, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IOperation_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithConstraintAsync()
	{
		var code =
			"""
			using Rocks;
			using System;

			namespace Namespace1
			{
				public interface IConstraint { }
			}

			namespace Namespace2
			{
				public interface IUseConstraint
				{
					void Foo<T>(T value) where T : Namespace1.IConstraint;
				}
			}

			namespace MockTest
			{
				public static class Test
				{
					public static void Go()
					{
						var expectations = Rock.Create<Namespace2.IUseConstraint>();
					}
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks.Extensions;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			#nullable enable
			
			namespace Namespace2
			{
				internal static class CreateExpectationsOfIUseConstraintExtensions
				{
					internal static global::Rocks.Expectations.MethodExpectations<global::Namespace2.IUseConstraint> Methods(this global::Rocks.Expectations.Expectations<global::Namespace2.IUseConstraint> @self) =>
						new(@self);
					
					internal static global::Namespace2.IUseConstraint Instance(this global::Rocks.Expectations.Expectations<global::Namespace2.IUseConstraint> @self)
					{
						if (!@self.WasInstanceInvoked)
						{
							@self.WasInstanceInvoked = true;
							return new RockIUseConstraint(@self);
						}
						else
						{
							throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
						}
					}
					
					private sealed class RockIUseConstraint
						: global::Namespace2.IUseConstraint
					{
						private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
						
						public RockIUseConstraint(global::Rocks.Expectations.Expectations<global::Namespace2.IUseConstraint> @expectations)
						{
							this.handlers = @expectations.Handlers;
						}
						
						[global::Rocks.MemberIdentifier(0, "void Foo<T>(T @value)")]
						public void Foo<T>(T @value)
							where T : global::Namespace1.IConstraint
						{
							if (this.handlers.TryGetValue(0, out var @methodHandlers))
							{
								var @foundMatch = false;
								
								foreach (var @methodHandler in @methodHandlers)
								{
									if (((@methodHandler.Expectations[0] as global::Rocks.Argument<T>)?.IsValid(@value) ?? false))
									{
										@foundMatch = true;
										
										if (@methodHandler.Method is not null && @methodHandler.Method is global::System.Action<T> @method)
										{
											@method(@value);
										}
										
										@methodHandler.IncrementCallCount();
										break;
									}
								}
								
								if (!@foundMatch)
								{
									throw new global::Rocks.Exceptions.ExpectationException("No handlers match for void Foo<T>(T @value)");
								}
							}
							else
							{
								throw new global::Rocks.Exceptions.ExpectationException("No handlers were found for void Foo<T>(T @value)");
							}
						}
						
					}
				}
				
				internal static class MethodExpectationsOfIUseConstraintExtensions
				{
					internal static global::Rocks.MethodAdornments<global::Namespace2.IUseConstraint, global::System.Action<T>> Foo<T>(this global::Rocks.Expectations.MethodExpectations<global::Namespace2.IUseConstraint> @self, global::Rocks.Argument<T> @value) where T : global::Namespace1.IConstraint
					{
						global::System.ArgumentNullException.ThrowIfNull(@value);
						return new global::Rocks.MethodAdornments<global::Namespace2.IUseConstraint, global::System.Action<T>>(@self.Add(0, new global::System.Collections.Generic.List<global::Rocks.Argument>(1) { @value }));
					}
				}
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IUseConstraint_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}