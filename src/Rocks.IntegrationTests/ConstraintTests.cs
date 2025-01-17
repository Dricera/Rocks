﻿using NUnit.Framework;

namespace Rocks.IntegrationTests;

public class BaseForConstraintCase { }

public abstract class BaseForConstraintCase<T>
	: BaseForConstraintCase where T : class
{
	public abstract BaseForConstraintCase<TTarget> As<TTarget>() where TTarget : class;
}

public static class ConstraintTests
{
	[Test]
	public static void Create()
	{
		var expectations = Rock.Create<BaseForConstraintCase<string>>();
		expectations.Methods().As<string>();

		var mock = expectations.Instance();
		mock.As<string>();

		expectations.Verify();
	}
}