﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rocks.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static Rocks.Extensions.MethodBaseExtensions;
using static Rocks.Extensions.MethodInfoExtensions;
using static Rocks.Extensions.TypeExtensions;

namespace Rocks.Construction
{
	internal abstract class Builder
	{
		internal Builder(Type baseType,
			ReadOnlyDictionary<string, ReadOnlyCollection<HandlerInformation>> handlers,
			SortedSet<string> namespaces, Options options)
		{
         this.BaseType = baseType;
			this.Handlers = handlers;
			this.Namespaces = namespaces;
			this.Options = options;
		}

		internal virtual void Build()
		{
			this.Tree = this.MakeTree();
		}

		private List<string> GetGeneratedConstructors()
		{
			var generatedConstructors = new List<string>();
			var constructorName = this.GetConstructorName();

         foreach (var constructor in this.BaseType.GetConstructors(Constants.Reflection.PublicInstance))
			{
				var parameters = constructor.GetParameters();

				if (parameters.Length > 0)
				{
					generatedConstructors.Add(string.Format(Constants.CodeTemplates.ConstructorTemplate,
						constructorName, constructor.GetArgumentNameList(), constructor.GetParameters(this.Namespaces)));
				}
			}

			return generatedConstructors;
		}

		private List<string> GetGeneratedMethods()
		{
			var generatedMethods = new List<string>();

			foreach (var baseMethod in this.BaseType.GetMethods(Constants.Reflection.PublicInstance)
				.Where(_ => !_.IsSpecialName && _.IsVirtual))
			{
				var methodDescription = baseMethod.GetMethodDescription(this.Namespaces);
				var containsRefAndOrOutParameters = baseMethod.ContainsRefAndOrOutParameters();

				// Either the base method contains no refs/outs, or the user specified a delegate
				// to use to handle that method (remember, types with methods with refs/outs are gen'd
				// each time, and that's the only reason the handlers are passed in.
				if (!containsRefAndOrOutParameters || this.Handlers.ContainsKey(methodDescription))
				{
					var delegateCast = !containsRefAndOrOutParameters ?
						baseMethod.GetDelegateCast() :
						this.Handlers[methodDescription][0].Method.GetType().GetSafeName(baseMethod, this.Namespaces);
					var argumentNameList = baseMethod.GetArgumentNameList();
					var outInitializers = !containsRefAndOrOutParameters ? string.Empty : baseMethod.GetOutInitializers();

					if(!containsRefAndOrOutParameters && baseMethod.GetParameters().Length > 0)
					{
						var expectationChecks = baseMethod.GetExpectationChecks();
						var expectationExceptionMessage = baseMethod.GetExpectationExceptionMessage();

						if (baseMethod.ReturnType != typeof(void))
						{
							generatedMethods.Add(string.Format(baseMethod.ReturnType.IsValueType ||
								(baseMethod.ReturnType.IsGenericParameter && (baseMethod.ReturnType.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == 0) ?
									Constants.CodeTemplates.FunctionWithValueTypeReturnValueMethodTemplate :
									Constants.CodeTemplates.FunctionWithReferenceTypeReturnValueMethodTemplate,
								methodDescription, argumentNameList, baseMethod.ReturnType.GetSafeName(), expectationChecks, delegateCast, outInitializers, expectationExceptionMessage));
						}
						else
						{
							generatedMethods.Add(string.Format(Constants.CodeTemplates.ActionMethodTemplate,
								methodDescription, argumentNameList, expectationChecks, delegateCast, outInitializers, expectationExceptionMessage));
						}
					}
					else
					{
						if (baseMethod.ReturnType != typeof(void))
						{
							generatedMethods.Add(string.Format(baseMethod.ReturnType.IsValueType ||
								(baseMethod.ReturnType.IsGenericParameter && (baseMethod.ReturnType.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == 0) ?
									Constants.CodeTemplates.FunctionWithValueTypeReturnValueAndNoArgumentsMethodTemplate :
									Constants.CodeTemplates.FunctionWithReferenceTypeReturnValueAndNoArgumentsMethodTemplate,
								methodDescription, argumentNameList, baseMethod.ReturnType.GetSafeName(), delegateCast, outInitializers));
						}
						else
						{
							generatedMethods.Add(string.Format(Constants.CodeTemplates.ActionMethodWithNoArgumentsTemplate,
								methodDescription, argumentNameList, delegateCast, outInitializers));
						}
					}
				}
				else
				{
					generatedMethods.Add(string.Format(Constants.CodeTemplates.RefOutNotImplementedMethodTemplate, methodDescription));
				}
			}

			return generatedMethods;
		}

		private List<string> GetGeneratedProperties()
		{
			var generatedProperties = new List<string>();

			foreach (var baseProperty in this.BaseType.GetProperties(Constants.Reflection.PublicInstance)
				.Where(_ => (_.CanRead ? _.GetMethod : _.SetMethod).IsVirtual))
			{
				var propertyImplementations = new List<string>();

				if(baseProperty.CanRead)
				{
					var getMethod = baseProperty.GetMethod;
					var getMethodDescription = getMethod.GetMethodDescription(this.Namespaces);
					var getArgumentNameList = getMethod.GetArgumentNameList();
					var getDelegateCast = getMethod.GetDelegateCast();

					if (getMethod.GetParameters().Length > 0)
					{
						var getExpectationChecks = getMethod.GetExpectationChecks();
						var getExpectationExceptionMessage = getMethod.GetExpectationExceptionMessage();
						propertyImplementations.Add(string.Format(getMethod.ReturnType.IsValueType ?
							Constants.CodeTemplates.PropertyGetWithValueTypeReturnValueTemplate :
							Constants.CodeTemplates.PropertyGetWithReferenceTypeReturnValueTemplate,
							getMethodDescription, getArgumentNameList, getMethod.ReturnType.GetSafeName(), getExpectationChecks, getDelegateCast, getExpectationExceptionMessage));
					}
					else
					{
						propertyImplementations.Add(string.Format(getMethod.ReturnType.IsValueType ?
							Constants.CodeTemplates.PropertyGetWithValueTypeReturnValueAndNoIndexersTemplate :
							Constants.CodeTemplates.PropertyGetWithReferenceTypeReturnValueAndNoIndexersTemplate,
							getMethodDescription, getArgumentNameList, getMethod.ReturnType.GetSafeName(), getDelegateCast));
					}
				}

				if (baseProperty.CanWrite)
				{
					var setMethod = baseProperty.SetMethod;
					var setMethodDescription = setMethod.GetMethodDescription(this.Namespaces);
					var setArgumentNameList = setMethod.GetArgumentNameList();
					var setDelegateCast = setMethod.GetDelegateCast();

					if (setMethod.GetParameters().Length > 0)
					{
						var setExpectationChecks = setMethod.GetExpectationChecks();
						var setExpectationExceptionMessage = setMethod.GetExpectationExceptionMessage();
						propertyImplementations.Add(string.Format(Constants.CodeTemplates.PropertySetTemplate,
							setMethodDescription, setArgumentNameList, setExpectationChecks, setDelegateCast, setExpectationExceptionMessage));
					}
					else
					{
						propertyImplementations.Add(string.Format(Constants.CodeTemplates.PropertySetAndNoIndexersTemplate,
							setMethodDescription, setArgumentNameList, setDelegateCast));
					}
				}

				// Generate the property template, based on indexes or not.
				this.Namespaces.Add(baseProperty.PropertyType.Namespace);
				var indexers = baseProperty.GetIndexParameters();

				if (indexers.Length > 0)
				{
					var parameters = string.Join(", ",
						from indexer in indexers
						let _ = this.Namespaces.Add(indexer.ParameterType.Namespace)
						select $"{indexer.ParameterType.Name} {indexer.Name}");

					// Indexer
					generatedProperties.Add(string.Format(Constants.CodeTemplates.PropertyIndexerTemplate,
						baseProperty.PropertyType.Name, parameters, string.Join(Environment.NewLine, propertyImplementations)));
				}
				else
				{
					// Normal
					generatedProperties.Add(string.Format(Constants.CodeTemplates.PropertyTemplate,
						baseProperty.PropertyType.GetSafeName(), baseProperty.Name,
						string.Join(Environment.NewLine, propertyImplementations)));
				}
			}

			return generatedProperties;
		}

		private string GetConstructorName() => this.TypeName.Split('<').First();

		private string MakeCode()
		{
			var methods = this.GetGeneratedMethods();
			var constructors = this.GetGeneratedConstructors();
			var properties = this.GetGeneratedProperties(); 
			var events = this.BaseType.GetImplementedEvents(this.Namespaces);

			this.Namespaces.Add(this.BaseType.Namespace);
			this.Namespaces.Add(typeof(ExpectationException).Namespace);
			this.Namespaces.Add(typeof(IMock).Namespace);
			this.Namespaces.Add(typeof(HandlerInformation).Namespace);
			this.Namespaces.Add(typeof(string).Namespace);
			this.Namespaces.Add(typeof(ReadOnlyDictionary<,>).Namespace);

			return string.Format(Constants.CodeTemplates.ClassTemplate,
				string.Join(Environment.NewLine,
					(from @namespace in this.Namespaces
					 select $"using {@namespace};")),
				this.TypeName, this.BaseType.GetSafeName(),
				string.Join(Environment.NewLine, methods),
				string.Join(Environment.NewLine, properties), events, 
				string.Join(Environment.NewLine, constructors),
				this.BaseType.Namespace, 
				this.Options.Serialization == SerializationOptions.Supported ? 
					"[Serializable]" : string.Empty,
				this.Options.Serialization == SerializationOptions.Supported ?
					string.Format(Constants.CodeTemplates.ConstructorNoArgumentsTemplate, this.GetConstructorName()) : string.Empty, 
				this.GetConstructorName());
		}

		private SyntaxTree MakeTree()
		{
			var @class = this.MakeCode();
			SyntaxTree tree = null;

			if (this.Options.CodeFile == CodeFileOptions.Create)
			{
				Directory.CreateDirectory(this.GetDirectoryForFile());
				var fileName = Path.Combine(this.GetDirectoryForFile(), 
					$"{this.TypeName.Replace("<", string.Empty).Replace(">", string.Empty)}.cs");
				tree = SyntaxFactory.SyntaxTree(
					SyntaxFactory.ParseSyntaxTree(@class)
						.GetCompilationUnitRoot().NormalizeWhitespace(),
					path: fileName, encoding: new UTF8Encoding(false, true));
				File.WriteAllText(fileName, tree.GetText().ToString());
			}
			else
			{
				tree = SyntaxFactory.ParseSyntaxTree(@class);
			}

			return tree;
		}

		protected abstract string GetDirectoryForFile();

		internal Options Options { get; }
      internal SyntaxTree Tree { get; private set; }
		internal Type BaseType { get; }
		internal ReadOnlyDictionary<string, ReadOnlyCollection<HandlerInformation>> Handlers { get; }
		internal SortedSet<string> Namespaces { get; }
		internal string TypeName { get; set; }
	}
}