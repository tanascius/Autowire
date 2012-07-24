using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Autowire.Factories;
using Autowire.KeyGenerators;
using Autowire.Utils.Extensions;

namespace Autowire
{
	/// <summary>Helps to register types, instances and even complete assemblies.</summary>
	internal class Registrator : IRegistrator
	{
		private readonly Container m_Container;
		private readonly HashSet<int> m_RegisteredTypes = new HashSet<int>();
		private readonly object m_RegisterLock = new object();
		private readonly TypeConfigurationManager m_TypeConfigurationManager = new TypeConfigurationManager();

		#region DEBUG
#if DEBUG
		public int Count
		{
			get { return m_RegisteredTypes.Count; }
		}
#endif
		#endregion

		#region Constructor
		/// <summary>Initializes a new instance of the <see cref="Registrator" /> class.</summary>
		public Registrator( Container container )
		{
			m_Container = container;
		}
		#endregion

		#region Configure
		[DebuggerStepThrough]
		public ITypeConfiguration Configure( Type type, string name )
		{
			return m_TypeConfigurationManager.Get( name, type );
		}
		#endregion

		#region Type()
		[DebuggerStepThrough]
		public ILazyConfiguration Type<T>() where T : class
		{
			return Type( string.Empty, typeof( T ) );
		}

		[DebuggerStepThrough]
		public ILazyConfiguration Type<T>( string name ) where T : class
		{
			return Type( name, typeof( T ) );
		}

		[DebuggerStepThrough]
		public ILazyConfiguration Type( Type type )
		{
			return Type( string.Empty, type );
		}

		public ILazyConfiguration Type( string name, Type type )
		{
			// Only classed can be registered (interfaces have no constructor)
			if( !type.IsClass || type.IsAbstract )
			{
				throw new RegisterException( type, "Only non-abstract classes can be registered." );
			}

			// Get the configuration
			var configuration = m_TypeConfigurationManager.Build( name, type );
			OnRegistrationHandler( type, configuration );
			m_TypeConfigurationManager.Update( name, type, configuration );

			// Use the same TypeInformation for all AutowireFactories of this type ...
			var typeInformation = new TypeInformation( m_Container, name, type, m_TypeConfigurationManager );

			if( configuration.IsIgnored )
			{
				return configuration;
			}

			// Already registered? Well, we don't throw an error - but log the problem at least
			var typeKey = KeyGenerator.GetSimpleKey( name, type );
			lock( m_RegisterLock )
			{
				if( m_RegisteredTypes.Contains( typeKey ) )
				{
					throw new RegisterException( type, "This type is already registered." );
				}
				m_RegisteredTypes.Add( typeKey );
			}

			// All keys of registered constructors will be kept in this hashset
			// This way we are able to find unresolvable constructors (unresolvable, because
			// they share the same key and we can not decide which one to invoke, later)
			// This can happen, because of the usage of [InjectAttribute] ...
			var registeredKeys = new HashSet<int>();

			// For each constructor we need a seperate registration
			foreach( var constructorInfo in  type.GetConstructors( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) )
			{
				// Get the keys for this constructor
				var keyGenerator = new RegisterKeyGenerator( name, type, constructorInfo, configuration.Arguments );
				var keys = keyGenerator.GetKeys();
				foreach( var key in keys )
				{
					// Check if we already had a constructor with the same signature
					if( registeredKeys.Contains( key ) )
					{
						var message = "The type '{0}' has some constructors with the same signature.\r\nOne of them is: '{1}'.".FormatUi( type, constructorInfo );
						throw new RegisterException( type, message );
					}
					registeredKeys.Add( key );

					lock( m_RegisterLock )
					{
						var factories = m_Container.GetFactories( key );
						var factory = new Factory( m_Container, constructorInfo, typeInformation, key == typeKey, configuration );
						if( key == typeKey )
						{
							factories.Insert( 0, factory );
						}
						else
						{
							factories.Add( factory );
						}
					}
				}
			}

			return configuration;
		}
		#endregion

		#region Instance()
		[DebuggerStepThrough]
		public void Instance( object instance )
		{
			Instance( string.Empty, instance );
		}

		public void Instance( string name, object instance )
		{
			// Already registered?
			var type = instance.GetType();
			var typeKey = KeyGenerator.GetSimpleKey( name, type );
			if( m_RegisteredTypes.Contains( typeKey ) )
			{
				throw new RegisterException( type, "Tried to register '{0}' again.".FormatUi( type.Name ) );
			}
			m_RegisteredTypes.Add( typeKey );

			var keys = new RegisterKeyGenerator( name, instance ).GetKeys();
			var instanceFactory = new InstanceFactory( instance );
			foreach( var factories in keys.Select( key => m_Container.GetFactories( key ) ) )
			{
				factories.Insert( 0, instanceFactory );
			}
		}
		#endregion

		#region IsRegistered()
		/// <summary>Checks whether a type is already registered.</summary>
		/// <param name="name">The name under which the type could be registered.</param>
		/// <param name="type">The type that is is checked.</param>
		/// <returns>True, when the type is already registered, oserwise false.</returns>
		internal bool IsRegistered( string name, Type type )
		{
			var typeKey = KeyGenerator.GetSimpleKey( name, type );
			return m_RegisteredTypes.Contains( typeKey );
		}
		#endregion

		#region Assembly()
		[DebuggerStepThrough]
		public void Assembly( Assembly assembly )
		{
			Assembly( assembly, null );
		}

		public void Assembly( Assembly assembly, Action<Type, ITypeConfiguration> registrationHandler )
		{
			if( registrationHandler != null )
			{
				RegistrationHandler += registrationHandler;
			}

			try
			{
				var types = assembly.GetTypes();
				foreach( var type in types )
				{
					if( type.IsAbstract || !type.IsClass )
					{
						continue;
					}

					var name = string.Empty;
					var configuration = m_TypeConfigurationManager.Build( name, type );
					Type( name, type ).WithScope( configuration.Scope );
				}
			}
			finally
			{
				if( registrationHandler != null )
				{
					RegistrationHandler -= registrationHandler;
				}
			}
		}
		#endregion

		#region AssemblyByName()
		[DebuggerStepThrough]
		public void AssemblyByName( string name )
		{
			if( !TryAssemblyByName( name, null ) )
			{
				throw new FileNotFoundException( "The assembly '{0}' can not be resolved.".FormatUi( name ) );
			}
		}

		public void AssemblyByName( string name, Action<Type, ITypeConfiguration> registrationHandler )
		{
			if( !TryAssemblyByName( name, registrationHandler ) )
			{
				throw new FileNotFoundException( "The assembly '{0}' can not be resolved.".FormatUi( name ) );
			}
		}
		#endregion

		#region TryAssemblyByName()
		[DebuggerStepThrough]
		public bool TryAssemblyByName( string name )
		{
			return TryAssemblyByName( name, null );
		}

		public bool TryAssemblyByName( string name, Action<Type, ITypeConfiguration> registrationHandler )
		{
			name = name.ToUpperInvariant();
			foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies().Where( assembly => assembly.GetName().Name.ToUpperInvariant() == name ) )
			{
				Assembly( assembly, registrationHandler );
				return true;
			}
			return false;
		}
		#endregion

		#region OnRegistrationHandler(), RegistrationHandler
		private void OnRegistrationHandler( Type type, ITypeConfiguration typeConfiguration )
		{
			if( RegistrationHandler != null )
			{
				RegistrationHandler( type, typeConfiguration );
			}
		}

		public event Action<Type, ITypeConfiguration> RegistrationHandler;
		#endregion
	}
}
