using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autowire.Factories;
using Autowire.KeyGenerators;
using Autowire.Utils.Extensions;
using Autowire.Utils.FastDynamics;

namespace Autowire
{
	/// <summary>The DI-container that will register and resolve types.</summary>
	public sealed class Container : IContainer
	{
		private static readonly ConstructorInfo m_GenericCollectionConstructor = typeof( Collection<> ).GetConstructors()[0];
		private static readonly Dictionary<Type, FastInvoker> m_CollectionInvokers = new Dictionary<Type, FastInvoker>();

		private readonly object m_ResolveLock = new object();
		private readonly Registrator m_Registrator;
		private readonly Container m_Parent;
		private readonly Dictionary<int, IList<IFactory>> m_Factories = new Dictionary<int, IList<IFactory>>();
		private readonly bool m_ThrowIfUnableToResolve;

		#region DEBUG
#if DEBUG
		/// <summary>Gets the count of all factories.</summary>
		public int FactoryCount
		{
			get { return m_Factories.Count; }
		}
#endif
		#endregion

		#region Constructors
		/// <summary>Initializes a new instance of the <see cref="Container" /> class.</summary>
		public Container()
		{
			// Register the Container itself
			m_Factories.Add( typeof( IContainer ).GetHashCode(), new[]
			{
				new SelfFactory( this )
			} );

			m_Registrator = new Registrator( this );
		}

		/// <summary>Initializes a new instance of the <see cref="Container" /> class.</summary>
		public Container( bool throwIfUnableToResolve ) : this()
		{
			m_ThrowIfUnableToResolve = throwIfUnableToResolve;
		}

		/// <summary>Initializes a new instance of the <see cref="Container" /> class.</summary>
		private Container( Container parent ) : this()
		{
			m_Parent = parent;
		}
		#endregion

		#region Configure()
		/// <summary>Configures a type.</summary>
		/// <typeparam name="T">The type that is configured.</typeparam>
		/// <returns>The configuration manager of the type.</returns>
		[DebuggerStepThrough]
		public ITypeConfiguration Configure<T>()
		{
			return m_Registrator.Configure( typeof( T ), string.Empty );
		}

		/// <summary>Configures a type.</summary>
		/// <typeparam name="T">The type that is configured.</typeparam>
		/// <param name="name">The name of type that is configured.</param>
		/// <returns>The configuration manager of the type.</returns>
		[DebuggerStepThrough]
		public ITypeConfiguration Configure<T>( string name )
		{
			return m_Registrator.Configure( typeof( T ), name );
		}

		/// <summary>Configures a type.</summary>
		/// <param name="type">The type that is configured.</param>
		/// <returns>The configuration manager of the type.</returns>
		[DebuggerStepThrough]
		public ITypeConfiguration Configure( Type type )
		{
			return m_Registrator.Configure( type, string.Empty );
		}

		/// <summary>Configures a type.</summary>
		/// <param name="type">The type that is configured.</param>
		/// <param name="name">The name of type that is configured.</param>
		/// <returns>The configuration manager of the type.</returns>
		[DebuggerStepThrough]
		public ITypeConfiguration Configure( Type type, string name )
		{
			return m_Registrator.Configure( type, name );
		}
		#endregion

		#region Register, IsRegistered()
		/// <summary>Returns an instance of <see cref="IRegistrator"/>, which allows to register types, instances and whole assemblies.</summary>
		public IRegistrator Register
		{
			get { return m_Registrator; }
		}

		/// <summary>Checks whether a type is already registered.</summary>
		/// <param name="type">The type that is is checked.</param>
		/// <returns>True, when the type is already registered, oserwise false.</returns>
		[DebuggerStepThrough]
		public bool IsRegistered( Type type )
		{
			return IsRegistered( string.Empty, type );
		}

		/// <summary>Checks whether a type is already registered.</summary>
		/// <param name="name">The name under which the type could be registered.</param>
		/// <param name="type">The type that is is checked.</param>
		/// <returns>True, when the type is already registered, oserwise false.</returns>
		public bool IsRegistered( string name, Type type )
		{
			AssertNotDisposed();
			return m_Registrator.IsRegistered( name, type );
		}
		#endregion

		#region Resolve(), ResolveByName()
		/// <summary>Tries to resolves an instance by using the given type and arguments.</summary>
		/// <typeparam name="T">The type of which an instance will be created.</typeparam>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>The resolved instance or null.</returns>
		[DebuggerStepThrough]
		public T Resolve<T>( params object[] args )
		{
			var type = typeof( T );
			return ( T ) ResolveHelper( this, string.Empty, type, 0, args );
		}

		/// <summary>Tries to resolves an instance by using the given name, type and arguments.</summary>
		/// <typeparam name="T">The type of which an instance will be created.</typeparam>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>The resolved instance or null.</returns>
		[DebuggerStepThrough]
		public T ResolveByName<T>( string name, params object[] args )
		{
			var type = typeof( T );
			return ( T ) ResolveHelper( this, name, type, 0, args );
		}

		/// <summary>Tries to resolves an instance by using the given type and arguments.</summary>
		/// <param name="type">The type of which an instance will be created.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>The resolved instance or null.</returns>
		[DebuggerStepThrough]
		public object Resolve( Type type, params object[] args )
		{
			return ResolveHelper( this, string.Empty, type, 0, args );
		}

		/// <summary>Tries to resolves an instance by using the given name, type and arguments.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="type">The type of which an instance will be created.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>The resolved instance or null.</returns>
		[DebuggerStepThrough]
		public object ResolveByName( string name, Type type, params object[] args )
		{
			return ResolveHelper( this, name, type, 0, args );
		}
		#endregion

		#region ResolveHelper()
		private object ResolveHelper( IContainer container, string name, Type type, int simpleKey, object[] args )
		{
			AssertNotDisposed();

			if( simpleKey == 0 )
			{
				try
				{
					simpleKey = KeyGenerator.GetSimpleKey( name, type, args );
				}
				catch( ResolveException )
				{
					if( m_ThrowIfUnableToResolve )
					{
						throw;
					}
					return null;
				}
			}

			// Get all factories that are registered for this (type, args) combination
			IList<IFactory> factories;
			if( !m_Factories.TryGetValue( simpleKey, out factories ) )
			{
				lock( m_ResolveLock )
				{
					if( !m_Factories.TryGetValue( simpleKey, out factories ) )
					{
						// No factories found? Generate keys for all combinations of the arguments
						var keys = ResolveKeyGenerator.GetKeys( simpleKey, name, type, args );

						// Try to get a matching factory
						var factory = GetFactory( keys, type, args );
						if( factory == null )
						{
							// We can not resolve this type - maybe our parent can
							if( m_Parent != null )
							{
								// But we will pass and preserve the container reference.
								// If there is a childcontainer involved, and an argument needs to
								// be resolved later, we will try to resolve on the childcontainer,
								// at first - not on the parentcontainer
								return m_Parent.ResolveHelper( container, name, type, simpleKey, args );
							}

							// So far no one can resolve -> test, wether an IEnumerable is expected
							if( type.IsGenericType && type.GetInterface( "IEnumerable", false ) != null )
							{
								// Use ResolveAll() to get a list of all types fitting into the list
								var parameterType = type.GetGenericArguments()[0];

								// Again: we have to pass and preserve the container reference.
								// If there is a childcontainer involved, and an argument needs to
								// be resolved later, we will try to resolve on the childcontainer,
								// at first - not on the parentcontainer
								return ResolveAllByNameHelper( container, name, parameterType );
							}

							// Bad luck - no resolve possible
							if( m_ThrowIfUnableToResolve )
							{
								throw new ResolveException( type, "The type '{0}' can not be resolved. Maybe constructor arguments are not given as expected?".FormatUi( type.Name ) );
							}
							return null;
						}

						// The factory we found can be registered for later use under the simpleKey
						factories = new List<IFactory>
						{
							factory
						};
						m_Factories.Add( simpleKey, factories );
					}
				}
			}

			if( factories.Count == 0 )
			{
				Debug.Fail( "Only an empty list of factories was resolved!" );
				throw new ResolveException( type, "The type '{0}' is unknown".FormatUi( type.Name ) );
			}
			if( factories.Count == 1 || factories[0].IsRegisteredByUser )
			{
				try
				{
					return factories[0].Invoke( container, type, args );
				}
				catch( ResolveException )
				{
					if( m_ThrowIfUnableToResolve )
					{
						throw;
					}
				}
			}
			if( m_ThrowIfUnableToResolve )
			{
				throw new ResolveException( type, "{0} possible resolves were found for this type.".FormatUi( factories.Count ) );
			}
			return null;
		}
		#endregion

		#region GetFactories(), GetFactory()
		private readonly object m_FactoriesLock = new object();

		internal IList<IFactory> GetFactories( int key )
		{
			IList<IFactory> factories;
			if( !m_Factories.TryGetValue( key, out factories ) )
			{
				lock( m_FactoriesLock )
				{
					if( !m_Factories.TryGetValue( key, out factories ) )
					{
						factories = new List<IFactory>();
						m_Factories.Add( key, factories );
					}
				}
			}
			return factories;
		}

		/// <summary>Returns a factory for the given type + its arguments.</summary>
		/// <param name="keys">The key of the type.</param>
		/// <param name="type">The type itself.</param>
		/// <param name="args">The type's arguments.</param>
		private IFactory GetFactory( IList<int> keys, Type type, object[] args )
		{
			for( var keyIndex = 0; keyIndex < keys.Count; keyIndex++ )
			{
				var key = keys[keyIndex];
				IList<IFactory> factories;
				if( !m_Factories.TryGetValue( key, out factories ) )
				{
					continue;
				}
				for( var factoryIndex = 0; factoryIndex < factories.Count; factoryIndex++ )
				{
					// We have to check the factory, because in the case of
					// hash collisions, we will get some useless factories ...
					if( factories[factoryIndex].CanInvoke( type, args ) )
					{
						return factories[factoryIndex];
					}
				}
			}
			return null;
		}
		#endregion

		#region ResolveAll(), ResolveAllByName()
		/// <summary>Resolves all possible instance by using the given type and arguments.</summary>
		/// <typeparam name="T">The type of which an instance will be created.</typeparam>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList{T}"/> containing all resolved instances.</returns>
		public IList<T> ResolveAll<T>( params object[] args )
		{
			return ResolveAllByNameHelper<T>( this, string.Empty, args );
		}

		/// <summary>Resolves all possible instance by using the given name, type and arguments.</summary>
		/// <typeparam name="T">The type of which an instance will be created.</typeparam>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList{T}"/> containing all resolved instances.</returns>
		public IList<T> ResolveAllByName<T>( string name, params object[] args )
		{
			return ResolveAllByNameHelper<T>( this, name, args );
		}

		private IList<T> ResolveAllByNameHelper<T>( IContainer container, string name, params object[] args )
		{
			var type = typeof( T );
			var key = KeyGenerator.GetSimpleKey( name, type, args );
			var collection = new Collection<T>();
			IList<IFactory> factories;
			if( m_Factories.TryGetValue( key, out factories ) )
			{
				for( var factoryIndex = 0; factoryIndex < factories.Count; factoryIndex++ )
				{
					collection.Add( ( T ) factories[factoryIndex].Invoke( container, type, args ) );
				}
			}
			return collection;
		}

		/// <summary>Resolves all possible instance by using the given type and arguments.</summary>
		/// <param name="type">The type of which an instance will be created.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList"/> containing all resolved instances.</returns>
		public IList ResolveAll( Type type, params object[] args )
		{
			return ResolveAllByNameHelper( this, string.Empty, type, args );
		}

		/// <summary>Resolves all possible instance by using the given name, type and arguments.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="type">The type of which an instance will be created.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList"/> containing all resolved instances.</returns>
		public IList ResolveAllByName( string name, Type type, params object[] args )
		{
			return ResolveAllByNameHelper( this, name, type, args );
		}

		private IList ResolveAllByNameHelper( IContainer container, string name, Type type, params object[] args )
		{
			var key = KeyGenerator.GetSimpleKey( name, type, args );

			// We will return a typed Collection - to create it we need a FastInvoker
			FastInvoker fastInvoker;
			if( !m_CollectionInvokers.TryGetValue( type, out fastInvoker ) )
			{
				fastInvoker = new FastInvoker( m_GenericCollectionConstructor, type );
				m_CollectionInvokers.Add( type, fastInvoker );
			}
			var collection = ( IList ) fastInvoker.Invoke();

			// Get all factories, that can deliver the type and invoke them
			IList<IFactory> factories;
			if( m_Factories.TryGetValue( key, out factories ) )
			{
				for( var factoryIndex = 0; factoryIndex < factories.Count; factoryIndex++ )
				{
					collection.Add( factories[factoryIndex].Invoke( container, type, args ) );
				}
			}
			return collection;
		}
		#endregion

		#region CreateChild()
		/// <summary>Creates a child container.</summary>
		public IContainer CreateChild()
		{
			return new Container( this );
		}
		#endregion

		#region SetAsSingleton()
		internal void SetAsSingleton( string name, object instance )
		{
			var keys = new RegisterKeyGenerator( name, instance ).GetKeys();
			for( var i = 0; i < keys.Count; i++ )
			{
				var factories = GetFactories( keys[i] );
				factories.Insert( 0, new InstanceFactory( instance ) );
			}
		}
		#endregion

		#region IDisposable
		///<summary>Returns, whether the Dispose()-method was already called for this object.</summary>
		private bool m_IsDisposed;

		///<summary>Makes sure, that the Dispose()-method was not called so far.</summary>
		private void AssertNotDisposed()
		{
			if( m_IsDisposed )
			{
				throw new ObjectDisposedException( GetType().FullName );
			}
		}

		// SuppressMessage, da sich FxCop an der AutoProperty stört :/
		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		[SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly" )]
		public void Dispose()
		{
			if( m_IsDisposed )
			{
				return;
			}
			m_IsDisposed = true;

			// Cleanup managed Resources
			foreach( var factory in m_Factories.Values.SelectMany( factories => factories ) )
			{
				factory.Dispose();
			}

			GC.SuppressFinalize( this );
		}
		#endregion
	}
}
