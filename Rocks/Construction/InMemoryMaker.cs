﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Rocks.Construction
{
	internal sealed class InMemoryMaker
	{
		internal Type Mock { get; private set; }

		internal InMemoryMaker(Type baseType,
			ReadOnlyDictionary<string, ReadOnlyCollection<HandlerInformation>> handlers,
			SortedSet<string> namespaces, Options options)
		{
			var builder = new InMemoryBuilder(baseType, handlers, namespaces, options);
			builder.Build();

			var compiler = new InMemoryCompiler(new List<SyntaxTree> { builder.Tree }, options.Level,
				new List<Assembly> { baseType.Assembly }.AsReadOnly());
			compiler.Compile();

			this.Mock = compiler.Result.GetType($"{baseType.Namespace}.{builder.TypeName}");
		}
	}
}