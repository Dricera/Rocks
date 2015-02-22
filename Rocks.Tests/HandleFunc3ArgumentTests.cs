﻿using NUnit.Framework;

namespace Rocks.Tests
{
	[TestFixture]
	public sealed class HandleFunc3ArgumentTests
	{
		[Test]
		public void Make()
		{
			var rock = Rock.Create<IHandleFunc3ArgumentTests>();
			rock.HandleFunc(_ => _.ReferenceTarget(default(int), default(int), default(int)));
			rock.HandleFunc(_ => _.ValueTarget(default(int), default(int), default(int)));

			var chunk = rock.Make();
			chunk.ReferenceTarget(1, 2, 3);
			chunk.ValueTarget(10, 20, 30);

			rock.Verify();
		}

		[Test]
		public void MakeWithHandler()
		{
			var argumentA = 0;
			var argumentB = 0;
			var argumentC = 0;
			var stringReturnValue = "a";
			var intReturnValue = 1;

			var rock = Rock.Create<IHandleFunc3ArgumentTests>();
			rock.HandleFunc<int, int, int, string>(_ => _.ReferenceTarget(default(int), default(int), default(int)),
				(a, b, c) => { argumentA = a; argumentB = b; argumentC = c; return stringReturnValue; });
			rock.HandleFunc<int, int, int, int>(_ => _.ValueTarget(default(int), default(int), default(int)),
				(a, b, c) => { argumentA = a; argumentB = b; argumentC = c; return intReturnValue; });
			
			var chunk = rock.Make();
			Assert.AreEqual(stringReturnValue, chunk.ReferenceTarget(1, 2, 3), nameof(chunk.ReferenceTarget));
			Assert.AreEqual(1, argumentA, nameof(argumentA));
			Assert.AreEqual(2, argumentB, nameof(argumentB));
			Assert.AreEqual(3, argumentC, nameof(argumentC));
			Assert.AreEqual(intReturnValue, chunk.ValueTarget(10, 20, 30), nameof(chunk.ValueTarget));
			Assert.AreEqual(10, argumentA, nameof(argumentA));
			Assert.AreEqual(20, argumentB, nameof(argumentB));
			Assert.AreEqual(30, argumentC, nameof(argumentC));

			rock.Verify();
		}

		[Test]
		public void MakeWithExpectedCallCount()
		{
			var rock = Rock.Create<IHandleFunc3ArgumentTests>();
			rock.HandleFunc(_ => _.ReferenceTarget(default(int), default(int), default(int)), 2);
			rock.HandleFunc(_ => _.ValueTarget(default(int), default(int), default(int)), 2);

			var chunk = rock.Make();
			chunk.ReferenceTarget(1, 2, 3);
			chunk.ReferenceTarget(1, 2, 3);
			chunk.ValueTarget(10, 20, 30);
			chunk.ValueTarget(10, 20, 30);

			rock.Verify();
		}

		[Test]
		public void MakeWithHandlerAndExpectedCallCount()
		{
			var argumentA = 0;
			var argumentB = 0;
			var argumentC = 0;
			var stringReturnValue = "a";
			var intReturnValue = 1;

			var rock = Rock.Create<IHandleFunc3ArgumentTests>();
			rock.HandleFunc<int, int, int, string>(_ => _.ReferenceTarget(default(int), default(int), default(int)),
				(a, b, c) => { argumentA = a; argumentB = b; argumentC = c; return stringReturnValue; }, 2);
			rock.HandleFunc<int, int, int, int>(_ => _.ValueTarget(default(int), default(int), default(int)),
				(a, b, c) => { argumentA = a; argumentB = b; argumentC = c; return intReturnValue; }, 2);

			var chunk = rock.Make();
			Assert.AreEqual(stringReturnValue, chunk.ReferenceTarget(1, 2, 3), nameof(chunk.ReferenceTarget));
			Assert.AreEqual(1, argumentA, nameof(argumentA));
			Assert.AreEqual(2, argumentB, nameof(argumentB));
			Assert.AreEqual(3, argumentC, nameof(argumentC));
			Assert.AreEqual(stringReturnValue, chunk.ReferenceTarget(100, 200, 300), nameof(chunk.ReferenceTarget));
			Assert.AreEqual(100, argumentA, nameof(argumentA));
			Assert.AreEqual(200, argumentB, nameof(argumentB));
			Assert.AreEqual(300, argumentC, nameof(argumentC));
			Assert.AreEqual(intReturnValue, chunk.ValueTarget(10, 20, 30), nameof(chunk.ValueTarget));
			Assert.AreEqual(10, argumentA, nameof(argumentA));
			Assert.AreEqual(20, argumentB, nameof(argumentB));
			Assert.AreEqual(30, argumentC, nameof(argumentC));
			Assert.AreEqual(intReturnValue, chunk.ValueTarget(1000, 2000, 3000), nameof(chunk.ValueTarget));
			Assert.AreEqual(1000, argumentA, nameof(argumentA));
			Assert.AreEqual(2000, argumentB, nameof(argumentB));
			Assert.AreEqual(3000, argumentC, nameof(argumentC));

			rock.Verify();
		}
	}

	public interface IHandleFunc3ArgumentTests
	{
		string ReferenceTarget(int a, int b, int c);
		int ValueTarget(int a, int b, int c);
	}
}