using System;
using System.Collections.ObjectModel;
using Autowire.Registration;
using Autowire.Resolving;

namespace Autowire.KeyGenerators
{
	/// <summary>Creates all keys for a given type.</summary>
	/// <remarks>
	/// There will be one key for each constructor configuration.
	/// Let's assume the have a type with two constructors:
	///   public Example( Foo foo )
	///   public Example( Foo foo, Bar bar )
	/// where Bar is derived from BarBase, IBar and Foo is derived from IFoo ...
	/// There will be eight keys generated:
	///   1. for the constructor with parameter:  Foo
	///   2. for the constructor with parameter:  IFoo
	///   3. for the constructor with parameters: Foo, Bar
	///   4. for the constructor with parameters: Foo, BarDerived
	///   5. for the constructor with parameters: Foo, IBar
	///   6. for the constructor with parameters: IFoo, Bar
	///   7. for the constructor with parameters: IFoo, BarDerived
	///   8. for the constructor with parameters: IFoo, IBar
	/// </remarks>
	internal abstract class KeyGenerator
	{
		protected static readonly int[] m_KeyModifier = new[]
		{
			5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61
		};

		protected readonly Type m_Type;
		protected readonly int m_NameKey;

		#region static: GetSimpleKey()
		public static int GetSimpleKey( string name, Type type, params object[] args )
		{
			if( args == null )
			{
				throw new ResolveException( type, "Given arguments can not be null. Use NullArg<> instead of passing null." );
			}

			var hashCode = type.GetHashCode() ^ ( string.IsNullOrEmpty( name ) ? 0 : name.GetHashCode() * 3 );

			if( args.Length != 0 )
			{
				for( var i = 0; i < args.Length; i++ )
				{
					var arg = args[i];
					if( arg == null )
					{
						throw new ResolveException( type, "Given arguments can not be null. Use NullArg<> instead of passing null." );
					}
					var nullArg = arg as NullArg;
					var argHashCode = nullArg == null ? arg.GetType().GetHashCode() : nullArg.Type.GetHashCode();
					hashCode ^= argHashCode * m_KeyModifier[args.Length - i - 1];
				}
			}

			return hashCode;
		}
		#endregion

		#region static: GetParameterHash()
		protected static int GetParameterHash( Type[] parameterTypes )
		{
			var hashCode = 0;
			for( var i = 0; i < parameterTypes.Length; i++ )
			{
				if( parameterTypes[i].IsGenericType || parameterTypes[i].IsGenericParameter )
				{
					// HACK here the exact type must be looked up
					hashCode ^= typeof( object ).GetHashCode() * m_KeyModifier[parameterTypes.Length - i - 1];
				}
				else
				{
					hashCode ^= parameterTypes[i].GetHashCode() * m_KeyModifier[parameterTypes.Length - i - 1];
				}
			}
			return hashCode;
		}
		#endregion

		protected KeyGenerator( Type type, string name )
		{
			m_Type = type;
			m_NameKey = string.IsNullOrEmpty( name ) ? 0 : name.GetHashCode() * 3;
		}

		public abstract Collection<int> GetKeys();
	}
}
