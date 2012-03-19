using System;

namespace Autowire
{
	/// <summary>Represents an argument for a constructor or a method.</summary>
	public class Argument
	{
		/// <summary>Creates a new argument with a value.</summary>
		/// <param name="argumentName">The name of the argument as coded in the signature of the constructor or method.</param>
		/// <param name="value">The value of the argument.</param>
		public static Argument Static( string argumentName, object value )
		{
			if( value == null )
			{
				throw new ConfigureException( "The value can not be null. Use Argument.UserProvided() for missing arguments or NullType<> for null argumetns." );
			}
			return new Argument( argumentName, null, value, null );
		}

		/// <summary>Defines that an argument will be provided during resolution time by the user.</summary>
		/// <param name="argumentName">The name of the argument as coded in the signature of the constructor or method.</param>
		public static Argument UserProvided( string argumentName )
		{
			return new Argument( argumentName, null, null, null );
		}

		/// <summary>Defines a name for an argument will be injected with this name.</summary>
		/// <param name="argumentName">The name of the argument as coded in the signature of the constructor or method.</param>
		/// <param name="injectionName">The name under which the argument's type was registered.</param>
		public static Argument Named( string argumentName, string injectionName )
		{
			return new Argument( argumentName, injectionName, null, null );
		}

		/// <summary>Defines a type for an argument that will be used for injection.</summary>
		/// <typeparam name="T">The type that is used for injection.</typeparam>
		/// <param name="argumentName">The argument that is affected.</param>
		/// <remarks>
		/// Neccessary for the injection of base classes or interfaces of which more than one derived class is registered.
		/// Otherwise a <see cref="ResolveException"/> would be thrown ("_n_ possible resolves were found for this type.").
		/// </remarks>
		public static Argument UseType<T>( string argumentName )
		{
			return new Argument( argumentName, null, null, typeof( T ) );
		}

		/// <summary>Defines a type for an argument that will be used for injection.</summary>
		/// <param name="argumentName">The argument that is affected.</param>
		/// <param name="type">The type that is used for injection.</param>
		/// <remarks>
		/// Neccessary for the injection of base classes or interfaces of which more than one derived class is registered.
		/// Otherwise a <see cref="ResolveException"/> would be thrown ("_n_ possible resolves were found for this type.").
		/// </remarks>
		public static Argument UseType( string argumentName, Type type )
		{
			return new Argument( argumentName, null, null, type );
		}

		private Argument( string argumentName, string injectionName, object value, Type type )
		{
			ArgumentName = argumentName;
			InjectionName = injectionName;
			Value = value;
			Type = type;
		}

		internal string ArgumentName { get; private set; }
		internal string InjectionName { get; private set; }
		internal object Value { get; private set; }
		internal Type Type { get; private set; }
	}
}
