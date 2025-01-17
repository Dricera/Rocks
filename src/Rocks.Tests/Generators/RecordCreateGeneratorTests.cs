﻿using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Rocks.Tests.Generators;

public static class RecordCreateGeneratorTests
{
	[Test]
	public static async Task GenerateAsync()
	{
		var code =
			"""
			using Rocks;
			using System;

			namespace MockTests
			{
				public record RecordTest
				{
					public RecordTest() { }

					public virtual void Foo() { }
				}

				public static class Test
				{
					public static void Generate()
					{
						var rock = Rock.Create<RecordTest>();
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
			
			namespace MockTests
			{
				internal static class CreateExpectationsOfRecordTestExtensions
				{
					internal static global::Rocks.Expectations.MethodExpectations<global::MockTests.RecordTest> Methods(this global::Rocks.Expectations.Expectations<global::MockTests.RecordTest> @self) =>
						new(@self);
					
					internal static global::Rocks.Expectations.PropertyExpectations<global::MockTests.RecordTest> Properties(this global::Rocks.Expectations.Expectations<global::MockTests.RecordTest> @self) =>
						new(@self);
					
					internal static global::Rocks.Expectations.PropertyGetterExpectations<global::MockTests.RecordTest> Getters(this global::Rocks.Expectations.PropertyExpectations<global::MockTests.RecordTest> @self) =>
						new(@self);
					
					internal static global::MockTests.RecordTest Instance(this global::Rocks.Expectations.Expectations<global::MockTests.RecordTest> @self)
					{
						if (!@self.WasInstanceInvoked)
						{
							@self.WasInstanceInvoked = true;
							return new RockRecordTest(@self);
						}
						else
						{
							throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
						}
					}
					internal static global::MockTests.RecordTest Instance(this global::Rocks.Expectations.Expectations<global::MockTests.RecordTest> @self, global::MockTests.RecordTest @original)
					{
						if (!@self.WasInstanceInvoked)
						{
							@self.WasInstanceInvoked = true;
							return new RockRecordTest(@self, @original);
						}
						else
						{
							throw new global::Rocks.Exceptions.NewMockInstanceException("Can only create a new mock once.");
						}
					}
					
					private sealed record RockRecordTest
						: global::MockTests.RecordTest
					{
						private readonly global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Rocks.HandlerInformation>> handlers;
						
						public RockRecordTest(global::Rocks.Expectations.Expectations<global::MockTests.RecordTest> @expectations)
						{
							this.handlers = @expectations.Handlers;
						}
						public RockRecordTest(global::Rocks.Expectations.Expectations<global::MockTests.RecordTest> @expectations, global::MockTests.RecordTest @original)
							: base(@original)
						{
							this.handlers = @expectations.Handlers;
						}
						
						[global::Rocks.MemberIdentifier(2, "void Foo()")]
						public override void Foo()
						{
							if (this.handlers.TryGetValue(2, out var @methodHandlers))
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
						
						[global::Rocks.MemberIdentifier(3, "string ToString()")]
						public override string ToString()
						{
							if (this.handlers.TryGetValue(3, out var @methodHandlers))
							{
								var @methodHandler = @methodHandlers[0];
								var @result = @methodHandler.Method is not null ?
									global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<string>>(@methodHandler.Method)() :
									global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<string>>(@methodHandler).ReturnValue;
								@methodHandler.IncrementCallCount();
								return @result!;
							}
							else
							{
								return base.ToString();
							}
						}
						
						[global::Rocks.MemberIdentifier(4, "bool PrintMembers(global::System.Text.StringBuilder @builder)")]
						protected override bool PrintMembers(global::System.Text.StringBuilder @builder)
						{
							if (this.handlers.TryGetValue(4, out var @methodHandlers))
							{
								foreach (var @methodHandler in @methodHandlers)
								{
									if (global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.Argument<global::System.Text.StringBuilder>>(@methodHandler.Expectations[0]).IsValid(@builder))
									{
										var @result = @methodHandler.Method is not null ?
											global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<global::System.Text.StringBuilder, bool>>(@methodHandler.Method)(@builder) :
											global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<bool>>(@methodHandler).ReturnValue;
										@methodHandler.IncrementCallCount();
										return @result!;
									}
								}
								
								throw new global::Rocks.Exceptions.ExpectationException("No handlers match for bool PrintMembers(global::System.Text.StringBuilder @builder)");
							}
							else
							{
								return base.PrintMembers(@builder);
							}
						}
						
						[global::Rocks.MemberIdentifier(5, "int GetHashCode()")]
						public override int GetHashCode()
						{
							if (this.handlers.TryGetValue(5, out var @methodHandlers))
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
						
						[global::Rocks.MemberIdentifier(6, "get_EqualityContract()")]
						protected override global::System.Type EqualityContract
						{
							get
							{
								if (this.handlers.TryGetValue(6, out var @methodHandlers))
								{
									var @methodHandler = @methodHandlers[0];
									var @result = @methodHandler.Method is not null ?
										global::System.Runtime.CompilerServices.Unsafe.As<global::System.Func<global::System.Type>>(@methodHandler.Method)() :
										global::System.Runtime.CompilerServices.Unsafe.As<global::Rocks.HandlerInformation<global::System.Type>>(@methodHandler).ReturnValue;
									@methodHandler.IncrementCallCount();
									return @result!;
								}
								else
								{
									return base.EqualityContract;
								}
							}
						}
					}
				}
				
				internal static class MethodExpectationsOfRecordTestExtensions
				{
					internal static global::Rocks.MethodAdornments<global::MockTests.RecordTest, global::System.Action> Foo(this global::Rocks.Expectations.MethodExpectations<global::MockTests.RecordTest> @self) =>
						new global::Rocks.MethodAdornments<global::MockTests.RecordTest, global::System.Action>(@self.Add(2, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
					internal static global::Rocks.MethodAdornments<global::MockTests.RecordTest, global::System.Func<string>, string> ToString(this global::Rocks.Expectations.MethodExpectations<global::MockTests.RecordTest> @self) =>
						new global::Rocks.MethodAdornments<global::MockTests.RecordTest, global::System.Func<string>, string>(@self.Add<string>(3, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
					internal static global::Rocks.MethodAdornments<global::MockTests.RecordTest, global::System.Func<global::System.Text.StringBuilder, bool>, bool> PrintMembers(this global::Rocks.Expectations.MethodExpectations<global::MockTests.RecordTest> @self, global::Rocks.Argument<global::System.Text.StringBuilder> @builder)
					{
						global::System.ArgumentNullException.ThrowIfNull(@builder);
						return new global::Rocks.MethodAdornments<global::MockTests.RecordTest, global::System.Func<global::System.Text.StringBuilder, bool>, bool>(@self.Add<bool>(4, new global::System.Collections.Generic.List<global::Rocks.Argument>(1) { @builder }));
					}
					internal static global::Rocks.MethodAdornments<global::MockTests.RecordTest, global::System.Func<int>, int> GetHashCode(this global::Rocks.Expectations.MethodExpectations<global::MockTests.RecordTest> @self) =>
						new global::Rocks.MethodAdornments<global::MockTests.RecordTest, global::System.Func<int>, int>(@self.Add<int>(5, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
				}
				
				internal static class PropertyGetterExpectationsOfRecordTestExtensions
				{
					internal static global::Rocks.PropertyAdornments<global::MockTests.RecordTest, global::System.Func<global::System.Type>, global::System.Type> EqualityContract(this global::Rocks.Expectations.PropertyGetterExpectations<global::MockTests.RecordTest> @self) =>
						new global::Rocks.PropertyAdornments<global::MockTests.RecordTest, global::System.Func<global::System.Type>, global::System.Type>(@self.Add<global::System.Type>(6, new global::System.Collections.Generic.List<global::Rocks.Argument>()));
				}
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "RecordTest_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}