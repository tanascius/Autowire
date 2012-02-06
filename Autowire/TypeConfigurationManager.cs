using System;
using System.Collections.Generic;
using Autowire.KeyGenerators;

namespace Autowire
{
	internal class TypeConfigurationManager
	{
		private readonly Dictionary<int, TypeConfiguration> m_Configurations = new Dictionary<int, TypeConfiguration>();
		private readonly Dictionary<int, TypeConfiguration> m_BuiltConfigurations = new Dictionary<int, TypeConfiguration>();

		#region Get()
		/// <summary>Returns a new <see cref="ITypeConfiguration"/> for the given type (and its injectedName).</summary>
		/// <param name="injectedName">The injected name to be used.</param>
		/// <param name="type">The type that is configured.</param>
		public ITypeConfiguration Get( string injectedName, Type type )
		{
			var key = KeyGenerator.GetSimpleKey( injectedName, type );

			if( m_BuiltConfigurations.ContainsKey( key ) )
			{
				throw new ConfigureException( type, "This type was already registered and can not be configured anymore." );
			}

			TypeConfiguration typeConfiguration;
			if( !m_Configurations.TryGetValue( key, out typeConfiguration ) )
			{
				typeConfiguration = new TypeConfiguration( this, type );
				m_Configurations.Add( key, typeConfiguration );
			}

			return typeConfiguration;
		}
		#endregion

		#region Update()
		/// <summary>Updated the configuration of the given name (+ its injectedName)</summary>
		/// <param name="injectedName">The injected name to be used.</param>
		/// <param name="type">The type that is configured.</param>
		/// <param name="configuration">The new configuration.</param>
		public void Update( string injectedName, Type type, TypeConfiguration configuration )
		{
			var key = KeyGenerator.GetSimpleKey( injectedName, type );

			if( m_Configurations.ContainsKey( key ) )
			{
				m_Configurations[key] = configuration;
			}
			else
			{
				m_Configurations.Add( key, configuration );
			}
		}
		#endregion

		#region Build()
		/// <summary>Creates a complete configuration for the given type (+ its injectedName).</summary>
		/// <param name="injectedName">The injected name to be used.</param>
		/// <param name="type">The type that is configured.</param>
		/// <remarks>A complete configuration is a combined configuration of the type, its basetypes, its interfaces and generic types.</remarks>
		public TypeConfiguration Build( string injectedName, Type type )
		{
			var key = KeyGenerator.GetSimpleKey( injectedName, type );

			TypeConfiguration typeConfiguration;
			if( !m_BuiltConfigurations.TryGetValue( key, out typeConfiguration ) )
			{
				typeConfiguration = BuildConfigurationHelper( injectedName, type );
				m_BuiltConfigurations.Add( key, typeConfiguration );
			}
			return typeConfiguration;
		}

		private TypeConfiguration BuildConfigurationHelper( string name, Type type )
		{
			// Get the arguments of the type
			TypeConfiguration configuration;
			TypeConfiguration returnConfiguration = null;

			var key = KeyGenerator.GetSimpleKey( name, type );
			if( m_Configurations.TryGetValue( key, out configuration ) )
			{
				returnConfiguration = configuration;
			}

			// Get the arguments of generic typedefinitions
			if( type.IsGenericType )
			{
				var generticTypeDefinition = type.GetGenericTypeDefinition();
				if( type != generticTypeDefinition )
				{
					key = KeyGenerator.GetSimpleKey( name, generticTypeDefinition );
					if( m_Configurations.TryGetValue( key, out configuration ) )
					{
						if( returnConfiguration == null )
						{
							returnConfiguration = configuration;
						}
						else
						{
							returnConfiguration.CombineWith( configuration );
						}
					}
				}
			}

			// Get the arguments of the base types
			var baseType = type.BaseType;
			while( baseType != null )
			{
				key = KeyGenerator.GetSimpleKey( name, baseType );
				baseType = baseType.BaseType;
				if( !m_Configurations.TryGetValue( key, out configuration ) )
				{
					continue;
				}
				if( returnConfiguration == null )
				{
					returnConfiguration = configuration;
				}
				else
				{
					returnConfiguration.CombineWith( configuration );
				}
			}

			// Get the arguments of the interfaces
			foreach( var interfaceType in type.GetInterfaces() )
			{
				key = KeyGenerator.GetSimpleKey( name, interfaceType );
				if( !m_Configurations.TryGetValue( key, out configuration ) )
				{
					continue;
				}
				if( returnConfiguration == null )
				{
					returnConfiguration = configuration;
				}
				else
				{
					returnConfiguration.CombineWith( configuration );
				}
			}

			return returnConfiguration ?? new TypeConfiguration( this, type );
		}
		#endregion
	}
}
