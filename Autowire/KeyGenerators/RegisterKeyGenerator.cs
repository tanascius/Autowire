using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Autowire.KeyGenerators
{
	internal sealed class RegisterKeyGenerator : KeyGenerator
	{
		private readonly Type[] m_ParameterTypes;

		#region Constructors
		/// <summary>Initializes a new instance of the <see cref="KeyGenerator" /> class.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="instance">The instance for which the keys are required.</param>
		public RegisterKeyGenerator( string name, object instance ) : base( instance.GetType(), name )
		{
			m_ParameterTypes = new Type[0];
		}

		/// <summary>Initializes a new instance of the <see cref="KeyGenerator" /> class.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="type">THe type for which the keys are required.</param>
		/// <param name="constructorInfo">Constructor for which combinations the keys are constructed.</param>
		/// <param name="arguments">The arguments that will be used for the object creation.</param>
		public RegisterKeyGenerator( string name, Type type, MethodBase constructorInfo, IDictionary<string, Argument> arguments ) : base( type, name )
		{
			// Collect all types of parameters which are not auto-injected
			var typeCollection = new Collection<Type>();
			foreach( var parameterInfo in constructorInfo.GetParameters() )
			{
				Argument argument;
				if( !arguments.TryGetValue( parameterInfo.Name, out argument ) )
				{
					continue;
				}
				if( argument.Value != null || !string.IsNullOrEmpty( argument.InjectionName ) )
				{
					continue;
				}
				if( argument.Type != null )
				{
					continue;
				}
				typeCollection.Add( parameterInfo.ParameterType );
			}

			m_ParameterTypes = new Type[typeCollection.Count];
			typeCollection.CopyTo( m_ParameterTypes, 0 );
		}
		#endregion

		#region GetKeys()
		public override Collection<int> GetKeys()
		{
			var keys = new Collection<int>();

			// Create keys for all possible constructor combinations
			// And not only for all constructors, but for all baseclasses and interfaces of our type, too
			var baseType = m_Type;

			// Create keys of all baseclasses
			while( baseType != typeof( object ) )
			{
				var hashCode = baseType.GetHashCode() ^ m_NameKey ^ GetParameterHash( m_ParameterTypes );
				keys.Add( hashCode );
				baseType = baseType.BaseType;
			}

			// And create keys of all interfaces
			var interfaceTypes = m_Type.GetInterfaces();
			for( var i = 0; i < interfaceTypes.Length; i++ )
			{
				var interfaceType = interfaceTypes[i];
				var hashCode = interfaceType.GetHashCode() ^ m_NameKey ^ GetParameterHash( m_ParameterTypes );
				keys.Add( hashCode );
			}

			return keys;
		}
		#endregion
	}
}
