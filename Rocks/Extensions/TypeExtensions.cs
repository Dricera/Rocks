﻿using Rocks.Construction;
using Rocks.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Rocks.Extensions.AssemblyExtensions;

namespace Rocks.Extensions
{
	internal static class TypeExtensions
	{
		internal static bool AddNamespaces(this Type @this, SortedSet<string> namespaces)
		{
			namespaces.Add(@this.Namespace);

			if (@this.IsGenericType)
			{
				foreach (var genericType in @this.GetGenericArguments())
				{
					genericType.AddNamespaces(namespaces);
				}
			}

			return true;
		}
		internal static string GetFullName(this Type @this)
		{
			return @this.GetFullName(new SortedSet<string>());
		}

		internal static string GetFullName(this Type @this, SortedSet<string> namespaces)
		{
			var dissector = new TypeDissector(@this);

			var pointer = dissector.IsPointer ? "*" : string.Empty;
			var array = dissector.IsArray ? "[]" : string.Empty;

			return $"{dissector.SafeName}{dissector.RootType.GetGenericArguments(namespaces).Arguments}{pointer}{array}";
      }

		internal static ReadOnlyCollection<MockableResult<MethodInfo>> GetMockableMethods(this Type @this, NameGenerator generator)
		{
			var methods = new HashSet<MockableResult<MethodInfo>>(@this.GetMethods(ReflectionValues.PublicNonPublicInstance)
				.Where(_ => !_.IsSpecialName && _.IsVirtual && !_.IsFinal &&
					_.DeclaringType.Assembly.CanBeSeenByMockAssembly(_.IsPublic, _.IsPrivate, _.IsFamily, _.IsFamilyOrAssembly, generator))
				.Select(_ => new MockableResult<MethodInfo>(_, false)));

			if (@this.IsInterface)
			{
				var namespaces = new SortedSet<string>();

				foreach (var @interface in @this.GetInterfaces())
				{
					var interfaceMethods = @interface.GetMethods()
						.Where(_ => !_.IsSpecialName);

					foreach (var interfaceMethod in interfaceMethods)
					{
						if (interfaceMethod.CanBeSeenByMockAssembly(generator))
						{
							var matchMethodGroups = methods.GroupBy(_ => interfaceMethod.Match(_.Value)).ToDictionary(_ => _.Key);

							if (!matchMethodGroups.ContainsKey(MethodMatch.Exact))
							{
								methods.Add(new MockableResult<MethodInfo>(
									interfaceMethod, matchMethodGroups.ContainsKey(MethodMatch.DifferByReturnTypeOnly)));
							}
						}
					}
				}
			}

			return methods.ToList().AsReadOnly();
		}

		internal static ReadOnlyCollection<PropertyMockableResult> GetMockableProperties(this Type @this, NameGenerator generator)
		{
			var properties = new HashSet<PropertyMockableResult>(
				from property in @this.GetProperties(ReflectionValues.PublicNonPublicInstance)
				let canGet = property.CanRead && property.GetMethod.IsVirtual && !property.GetMethod.IsFinal &&
					property.GetMethod.DeclaringType.Assembly.CanBeSeenByMockAssembly(
					property.GetMethod.IsPublic, property.GetMethod.IsPrivate, property.GetMethod.IsFamily, property.GetMethod.IsFamilyOrAssembly, generator)
				let canSet = property.CanWrite && property.SetMethod.IsVirtual && !property.SetMethod.IsFinal &&
					property.SetMethod.DeclaringType.Assembly.CanBeSeenByMockAssembly(
					property.SetMethod.IsPublic, property.SetMethod.IsPrivate, property.SetMethod.IsFamily, property.SetMethod.IsFamilyOrAssembly, generator)
				where canGet || canSet
				select new PropertyMockableResult(property, false,
					(canGet && canSet ? PropertyAccessors.GetAndSet : (canGet ? PropertyAccessors.Get : PropertyAccessors.Set))));

			if (@this.IsInterface)
			{
				var namespaces = new SortedSet<string>();

				foreach (var @interface in @this.GetInterfaces())
				{
					foreach (var interfaceProperty in @interface.GetMockableProperties(generator))
					{
						if (interfaceProperty.Value.GetDefaultMethod().CanBeSeenByMockAssembly(generator))
						{
							var matchMethodGroups = properties.GroupBy(_ => interfaceProperty.Value.GetDefaultMethod().Match(_.Value.GetDefaultMethod())).ToDictionary(_ => _.Key);

							if (!matchMethodGroups.ContainsKey(MethodMatch.Exact))
							{
								properties.Add(new PropertyMockableResult(interfaceProperty.Value,
									matchMethodGroups.ContainsKey(MethodMatch.DifferByReturnTypeOnly), interfaceProperty.Accessors));
							}
						}
					}
				}
			}

			return properties.ToList().AsReadOnly();
		}

		internal static ReadOnlyCollection<EventInfo> GetMockableEvents(this Type @this, NameGenerator generator)
		{
			var events = new HashSet<EventInfo>(@this.GetEvents(ReflectionValues.PublicNonPublicInstance)
				.Where(_ => _.AddMethod.IsVirtual && !_.AddMethod.IsFinal && _.AddMethod.CanBeSeenByMockAssembly(generator)));

			if (@this.IsInterface)
			{
				foreach (var @interface in @this.GetInterfaces())
				{
					events.UnionWith(@interface.GetMockableEvents(generator));
				}
			}

			return events.ToList().AsReadOnly();
		}

		internal static MethodInfo FindMethod(this Type @this, int methodHandle)
		{
			var foundMethod = (from method in @this.GetMethods()
									 where method.MetadataToken == methodHandle
									 select method).FirstOrDefault();

			if (foundMethod == null)
			{
				var types = new List<Type>(@this.GetInterfaces());

				if (@this.BaseType != null)
				{
					types.Add(@this.BaseType);
				}

				return (from type in types
						  let baseMethod = type.FindMethod(methodHandle)
						  where baseMethod != null
						  select baseMethod).FirstOrDefault();
			}
			else
			{
				return foundMethod;
			}
		}

		internal static PropertyInfo FindProperty(this Type @this, string name)
		{
			var property = @this.GetProperty(name);

			if (property == null)
			{
				throw new PropertyNotFoundException($"Property {name} on type {@this.Name} was not found.");
			}

			return property;
		}

		internal static PropertyInfo FindProperty(this Type @this, string name, PropertyAccessors accessors)
		{
			var property = @this.FindProperty(name);
			TypeExtensions.CheckPropertyAccessors(property, accessors);
			return property;
		}

		private static void CheckPropertyAccessors(PropertyInfo property, PropertyAccessors accessors)
		{
			if (accessors == PropertyAccessors.Get || accessors == PropertyAccessors.GetAndSet)
			{
				if (!property.CanRead)
				{
					throw new PropertyNotFoundException($"Property {property.Name} on type {property.DeclaringType.Name} cannot be read from.");
				}
			}

			if (accessors == PropertyAccessors.Set || accessors == PropertyAccessors.GetAndSet)
			{
				if (!property.CanWrite)
				{
					throw new PropertyNotFoundException($"Property {property.Name} on type {property.DeclaringType.Name} cannot be written to.");
				}
			}
		}

		internal static PropertyInfo FindProperty(this Type @this, Type[] indexers)
		{
			var property = (from p in @this.GetProperties()
								 where p.GetIndexParameters().Any()
								 let pTypes = p.GetIndexParameters().Select(pi => pi.ParameterType).ToArray()
								 where ObjectEquality.AreEqual(pTypes, indexers)
								 select p).SingleOrDefault();

			if (property == null)
			{
				throw new PropertyNotFoundException($"Indexer on type {@this.Name} with argument types [{string.Join(", ", indexers.Select(_ => _.Name))}] was not found.");
			}

			return property;
		}

		internal static PropertyInfo FindProperty(this Type @this, Type[] indexers, PropertyAccessors accessors)
		{
			var property = @this.FindProperty(indexers);
			TypeExtensions.CheckPropertyAccessors(property, accessors);
			return property;
		}

		internal static bool IsUnsafeToMock(this Type @this)
		{
			return @this.IsPointer ||
				@this.GetMethods(ReflectionValues.PublicNonPublicInstance).Where(m => m.IsUnsafeToMock()).Any() ||
				@this.GetProperties(ReflectionValues.PublicNonPublicInstance).Where(p => p.GetDefaultMethod().IsUnsafeToMock(false)).Any() ||
				@this.GetEvents(ReflectionValues.PublicNonPublicInstance).Where(e => e.AddMethod.IsUnsafeToMock(false)).Any();
		}

		internal static string Validate(this Type @this, SerializationOptions options, NameGenerator generator)
		{
			if (@this.IsSealed && !@this.GetConstructors()
				.Where(_ => _.GetParameters().Length == 1 &&
					typeof(ReadOnlyDictionary<int, ReadOnlyCollection<HandlerInformation>>).IsAssignableFrom(_.GetParameters()[0].ParameterType)).Any())
			{
				return ErrorMessages.GetCannotMockSealedType(@this.GetSafeName());
			}

			if (options == SerializationOptions.Supported && !@this.IsInterface &&
				@this.GetConstructor(Type.EmptyTypes) == null)
			{
				return ErrorMessages.GetCannotMockTypeWithSerializationRequestedAndNoPublicNoArgumentConstructor(@this.GetSafeName());
			}

			if (@this.IsAbstract &&
				(@this.GetConstructors(ReflectionValues.NonPublicInstance).Where(_ => _.IsAssembly).Any() ||
            @this.GetMethods(ReflectionValues.NonPublicInstance).Where(_ => _.IsAssembly && _.IsAbstract).Any() ||
				@this.GetProperties(ReflectionValues.NonPublicInstance).Where(_ => _.GetDefaultMethod().IsAssembly && _.GetDefaultMethod().IsAbstract).Any() ||
				@this.GetEvents(ReflectionValues.NonPublicInstance).Where(_ => _.AddMethod.IsAssembly && _.AddMethod.IsAbstract).Any()))
			{
				if (!@this.Assembly.CanBeSeenByMockAssembly(false, false, false, false, generator))
				{
					return ErrorMessages.GetCannotMockTypeWithInternalAbstractMembers(@this.GetSafeName());
				}
			}

			return string.Empty;
		}

		internal static Type GetRootElementType(this Type @this)
		{
			var root = @this;

			while (root.HasElementType)
			{
				root = root.GetElementType();
			}

			return root;
		}

		internal static bool CanBeSeenByMockAssembly(this Type @this, NameGenerator generator)
		{
			var root = @this.GetRootElementType();
			return root.IsPublic || (root.Assembly.GetCustomAttributes<InternalsVisibleToAttribute>()
				.Where(_ => _.AssemblyName == generator.AssemblyName).Any());
		}

		internal static bool ContainsRefAndOrOutParameters(this Type @this)
		{
			return (from method in @this.GetMethods(ReflectionValues.PublicInstance)
					  where method.ContainsRefAndOrOutParameters()
					  select method).Any();
		}

		internal static string GetSafeName(this Type @this)
		{
			return @this.GetSafeName(null, null);
		}

		internal static string GetSafeName(this Type @this, MethodBase context, SortedSet<string> namespaces)
		{
			var name = !string.IsNullOrWhiteSpace(@this.FullName) ?
				@this.FullName.Split('`')[0].Split('.').Last().Replace("+", ".") :
				@this.Name.Split('`')[0];

			return name;
		}

		internal static GenericArgumentsResult GetGenericArguments(this Type @this, SortedSet<string> namespaces)
		{
			var arguments = string.Empty;
			var constraints = string.Empty;

			if (@this.IsGenericType)
			{
				var genericArguments = new List<string>();
				var genericConstraints = new List<string>();

				foreach (var argument in @this.GetGenericArguments())
				{
					genericArguments.Add($"{argument.GetSafeName()}{argument.GetGenericArguments(namespaces).Arguments}");
					var constraint = argument.GetConstraints(namespaces);

					if (!string.IsNullOrWhiteSpace(constraint))
					{
						genericConstraints.Add(constraint);
					}
				}

				arguments = $"<{string.Join(", ", genericArguments)}>";
				// TODO: This should not add a space in front. The Maker class
				// should adjust the constraints to have a space in front.
				constraints = genericConstraints.Count == 0 ?
					string.Empty : $"{string.Join(" ", genericConstraints)}";
			}

			return new GenericArgumentsResult(arguments, constraints);
		}

		internal static string GetConstraints(this Type @this, SortedSet<string> namespaces)
		{
			if (@this.IsGenericParameter)
			{
				var constraints = @this.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
				var constraintedTypes = @this.GetGenericParameterConstraints();

				if (constraints == GenericParameterAttributes.None && constraintedTypes.Length == 0)
				{
					return string.Empty;
				}
				else
				{
					var constraintValues = new List<string>();

					if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
					{
						constraintValues.Add("struct");
					}
					else
					{
						foreach (var constraintedType in constraintedTypes.OrderBy(_ => _.IsClass ? 0 : 1))
						{
							constraintValues.Add(constraintedType.GetSafeName());
							namespaces.Add(constraintedType.Namespace);
						}

						if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
						{
							constraintValues.Add("class");
						}
						if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
						{
							constraintValues.Add("new()");
						}
					}

					return $"where {@this.GetSafeName()} : {string.Join(", ", constraintValues)}";
				}
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
