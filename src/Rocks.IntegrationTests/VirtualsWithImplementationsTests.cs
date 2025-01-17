﻿using NUnit.Framework;

namespace Rocks.IntegrationTests;

public interface IIndexerPolygon
{
	int SideLength { get; }

	double this[int numberOfSides] => this.SideLength * numberOfSides;
}

public interface IMethodPolygon
{
	int NumberOfSides { get; }
	int SideLength { get; }

	double GetPerimeter() => this.SideLength * this.NumberOfSides;
}

public interface IPropertyPolygon
{
	int NumberOfSides { get; }
	int SideLength { get; }

	double Perimeter => this.SideLength * this.NumberOfSides;
}

public class IndexerPolygon
{
	public virtual int SideLength { get; }

	public virtual double this[int numberOfSides] => this.SideLength * numberOfSides;
}

public class MethodPolygon
{
	public virtual int NumberOfSides { get; }
	public virtual int SideLength { get; }

	public virtual double GetPerimeter() => this.SideLength * this.NumberOfSides;
}

public class PropertyPolygon
{
	public virtual int NumberOfSides { get; }
	public virtual int SideLength { get; }

	public virtual double Perimeter => this.SideLength * this.NumberOfSides;
}

public static class VirtualsWithImplementationsTests
{
	[Test]
	public static void CallVirtualIndexerOnInterfaceWithNoExpectation()
	{
		var expectations = Rock.Create<IIndexerPolygon>();
		expectations.Properties().Getters().SideLength().Returns(3);

		var mock = expectations.Instance();
		Assert.That(mock[5], Is.EqualTo(15));

		expectations.Verify();
	}

	[Test]
	public static void CallVirtualMethodOnInterfaceWithNoExpectation()
	{
		var expectations = Rock.Create<IMethodPolygon>();
		expectations.Properties().Getters().SideLength().Returns(3);
		expectations.Properties().Getters().NumberOfSides().Returns(5);

		var mock = expectations.Instance();
		Assert.That(mock.GetPerimeter(), Is.EqualTo(15));

		expectations.Verify();
	}

	[Test]
	public static void CallVirtualPropertyOnInterfaceWithNoExpectation()
	{
		var expectations = Rock.Create<IPropertyPolygon>();
		expectations.Properties().Getters().SideLength().Returns(3);
		expectations.Properties().Getters().NumberOfSides().Returns(5);

		var mock = expectations.Instance();
		Assert.That(mock.Perimeter, Is.EqualTo(15));

		expectations.Verify();
	}

	[Test]
	public static void CallVirtualIndexerOnClassWithNoExpectation()
	{
		var expectations = Rock.Create<IndexerPolygon>();
		expectations.Properties().Getters().SideLength().Returns(3);

		var mock = expectations.Instance();
		Assert.That(mock[5], Is.EqualTo(15));

		expectations.Verify();
	}

	[Test]
	public static void CallVirtualMethodOnClassWithNoExpectation()
	{
		var expectations = Rock.Create<MethodPolygon>();
		expectations.Properties().Getters().SideLength().Returns(3);
		expectations.Properties().Getters().NumberOfSides().Returns(5);

		var mock = expectations.Instance();
		Assert.That(mock.GetPerimeter(), Is.EqualTo(15));

		expectations.Verify();
	}

	[Test]
	public static void CallVirtualPropertyOnClassWithNoExpectation()
	{
		var expectations = Rock.Create<PropertyPolygon>();
		expectations.Properties().Getters().SideLength().Returns(3);
		expectations.Properties().Getters().NumberOfSides().Returns(5);

		var mock = expectations.Instance();
		Assert.That(mock.Perimeter, Is.EqualTo(15));

		expectations.Verify();
	}
}