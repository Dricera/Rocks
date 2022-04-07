using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Rocks.Tests;

public static class StaticAbstractMembersInInterfacesTests
{
	// TODO: Need property and operator tests as well
	[Test]
	public static async Task GenerateAsync()
	{
		var code =
			"""
			using Rocks;
			using System;
			
			#nullable enable
			namespace MockTests
			{
				public interface IHaveStaticAbstractMembers
				{
					static abstract void Foo();
				}
				
				public static class Test
				{
					public static void Generate()
					{
						var rock = Rock.Create<IHaveStaticAbstractMembers>();
					}
				}
			}
			""";

		var generatedCode =
			"""
			using Rocks;
			using Rocks.Exceptions;
			using Rocks.Expectations;
			using System;
			using System.Collections.Generic;
			using System.Collections.Immutable;
			
			#nullable enable
			namespace MockTests
			{
				internal static class CreateExpectationsOfIHaveStaticAbstractMembersExtensions
				{
					internal static MethodExpectations<IHaveStaticAbstractMembers> Methods(this Expectations<IHaveStaticAbstractMembers> self) =>
						new(self);
					
					internal static IHaveStaticAbstractMembers Instance(this Expectations<IHaveStaticAbstractMembers> self)
					{
						if (self.Mock is null)
						{
							var mock = new RockIHaveStaticAbstractMembers(self);
							self.Mock = mock;
							return mock;
						}
						else
						{
							throw new NewMockInstanceException("Can only create a new mock once.");
						}
					}
					
					private sealed class RockIHaveStaticAbstractMembers
						: IHaveStaticAbstractMembers, IMock
					{
						private readonly Dictionary<int, List<HandlerInformation>> handlers;
						
						public RockIHaveStaticAbstractMembers(Expectations<IHaveStaticAbstractMembers> expectations) =>
							this.handlers = expectations.Handlers;
						
						[MemberIdentifier(0, "static void Foo()")]
						public static void Foo()
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
								throw new ExpectationException("No handlers were found for static void Foo()");
							}
						}
						
						
						Dictionary<int, List<HandlerInformation>> IMock.Handlers => this.handlers;
					}
				}
				
				internal static class MethodExpectationsOfIHaveStaticAbstractMembersExtensions
				{
					internal static MethodAdornments<IHaveStaticAbstractMembers, Action> Foo(this MethodExpectations<IHaveStaticAbstractMembers> self) =>
						new MethodAdornments<IHaveStaticAbstractMembers, Action>(self.Add(0, new List<Argument>()));
				}
			}
			
			""";

		await TestAssistants.RunAsync<RockCreateGenerator>(code,
			new[] { (typeof(RockCreateGenerator), "IHaveStaticAbstractMembers_Rock_Create.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}