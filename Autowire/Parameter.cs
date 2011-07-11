using System;
using System.Reflection;
using Autowire.Utils.FastDynamics;

namespace Autowire
{
	/// <summary>Holds information about the parameter type and if it is provided by the user or should be auto-resolved.</summary>
	internal sealed class Parameter
	{
		/// <summary>Initializes a new instance of the <see cref="Parameter" /> class.</summary>
		/// <param name="container">The <see cref="IContainer"/> which uses the parameter.</param>
		/// <param name="parameterInfo">The underlaying <see cref="ParameterInfo"/> of this parameter.</param>
		/// <param name="argument">The <see cref="Argument"/> that was configured or null.</param>
		public Parameter( IContainer container, ParameterInfo parameterInfo, Argument argument )
		{
			Type = parameterInfo.ParameterType;
			Name = parameterInfo.Name;

			if( argument != null )
			{
				if( argument.Type != null )
				{
					Type = argument.Type;
					InjectedName = argument.InjectionName;
					IsUserInput = false;
					return;
				}

				if( argument.Value is INullArg )
				{
					Value = null;
					HasValue = true;
				}
				else
				{
					Value = argument.Value;
					HasValue = Value != null;
				}
				IsUserInput = argument.Value == null && string.IsNullOrEmpty( argument.InjectionName );
				InjectedName = argument.InjectionName;
			}
			else
			{
				// Do we have a lazy Func<> parameter?
				if( Type.BaseType == typeof( MulticastDelegate ) && Type.Name.StartsWith( "Func" ) )
				{
					// Resolve a delegate and use it as the argument
					Value = new FastDelegateFactory( container, Type ).Delegate;
					HasValue = true;
				}
				IsUserInput = false;
			}
		}

		/// <summary>Gets the type of the parameter.</summary>
		public Type Type { get; private set; }

		/// <summary>Gets the name of the parameter.</summary>
		public string Name { get; private set; }

		/// <summary>The name that was used to register the type.</summary>
		public string InjectedName { get; private set; }

		public object Value { get; private set; }

		public bool HasValue { get; private set; }

		/// <summary>Gets, whether an argument is provided by the user.</summary>
		public bool IsUserInput { get; private set; }
	}
}
