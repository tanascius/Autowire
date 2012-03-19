using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Autowire.KeyGenerators
{
	internal sealed class ResolveKeyGenerator : KeyGenerator
	{
		private static readonly Dictionary<int, Collection<int>> m_CalculatedKeys = new Dictionary<int, Collection<int>>();
		private readonly Type[] m_ParameterTypes;

		#region static: GetKeys()
		public static Collection<int> GetKeys( int simpleKey, string name, Type type, params object[] args )
		{
			Collection<int> calculatedKeys;
			if( !m_CalculatedKeys.TryGetValue( simpleKey, out calculatedKeys ) )
			{
				calculatedKeys = new ResolveKeyGenerator( name, type, args ).GetKeys();
				m_CalculatedKeys.Add( simpleKey, calculatedKeys );
			}

			return calculatedKeys;
		}
		#endregion

		#region Constructor
		/// <summary>Initializes a new instance of the <see cref="KeyGenerator" /> class.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="type">The type for which the keys are required.</param>
		/// <param name="args">The arguments of the new instance. Their type will be used for the key generation.</param>
		private ResolveKeyGenerator( string name, Type type, params object[] args ) : base( type, name )
		{
			m_ParameterTypes = new Type[args.Length];
			for( var i = 0; i < args.Length; i++ )
			{
				var nullArg = args[i] as INullArg;
				m_ParameterTypes[i] = nullArg == null ? args[i].GetType() : nullArg.Type;
			}
		}
		#endregion

		#region GetKeys()
		public override Collection<int> GetKeys()
		{
			var keys = new Collection<int>();

			// Create keys only for specific parameters (given in the constructor)
			// We have to take care of generic types - they are registered by their GenericTypeDefinition
			var typeHashcode = m_Type.IsGenericType ? m_Type.GetGenericTypeDefinition().GetHashCode() : m_Type.GetHashCode();
			GetKeys( keys, m_ParameterTypes, typeHashcode ^ m_NameKey );

			return keys;
		}

		private static void GetKeys( ICollection<int> keys, Type[] parameterTypes, int key )
		{
			if( parameterTypes.Length == 0 )
			{
				// No more parameters left? -> stop recursion
				keys.Add( key );
				return;
			}

			// Get the remaining parameters - they will be kept static in this interation
			var remainingParameterTypes = parameterTypes.Length > 1 ? new Type[parameterTypes.Length - 1] : Type.EmptyTypes;
			for( var i = 1; i < parameterTypes.Length; i++ )
			{
				remainingParameterTypes[i - 1] = parameterTypes[i];
			}

			// Get current parameter - this one will be modified
			var nextParameterType = parameterTypes[0];

			// Test all basetypes of the current parameter
			var baseType = nextParameterType;
			while( baseType != null )
			{
				GetKeys( keys, remainingParameterTypes, key ^ baseType.GetHashCode() * m_KeyModifier[remainingParameterTypes.Length] );
				baseType = baseType.BaseType;
			}

			// Test all interfaces of the current parameter
			var interfaceTypes = nextParameterType.GetInterfaces();
			for( var i = 0; i < interfaceTypes.Length; i++ )
			{
				var interfaceType = interfaceTypes[i];
				GetKeys( keys, remainingParameterTypes, key ^ interfaceType.GetHashCode() * m_KeyModifier[remainingParameterTypes.Length] );
			}
		}
		#endregion
	}
}
