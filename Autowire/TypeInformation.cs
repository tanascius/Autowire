using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using Autowire.Injectors;
using Autowire.Utils.Extensions;
using Autowire.Utils.Tuples;

namespace Autowire
{
	/// <summary>Holds the informations for a type. It will inject fields, properties and methods and keep singleton instances.</summary>
	internal sealed class TypeInformation
	{
		private readonly IContainer m_Container;
		private readonly Dictionary<int, object> m_SingletonInstances = new Dictionary<int, object>();
		private readonly Collection<IInjector> m_Injectors = new Collection<IInjector>();
		private readonly Collection<Tuple<TypeInformation, FieldInfo>> m_ExternalInjectors = new Collection<Tuple<TypeInformation, FieldInfo>>();
		private readonly TypeConfiguration m_Configuration;
		private readonly bool m_HasExternalInjectors;
		private readonly bool m_HasInjectors;

		#region Constructor
		/// <summary>Initializes a new instance of the <see cref="TypeInformation" /> class.</summary>
		/// <param name="container">The <see cref="Container"/> which is used to resolve injected instances.</param>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="type">The type which is described.</param>
		/// <param name="configurationManager">The configurationManager of the container.</param>
		public TypeInformation( IContainer container, string name, Type type, TypeConfigurationManager configurationManager )
		{
			m_Container = container;
			m_Configuration = configurationManager.Build( name, type );

			// Check if we are responsible for another object's injections
			foreach( var fieldInfo in m_Configuration.InjectForComponents )
			{
				var typeInformation = new TypeInformation( m_Container, String.Empty, fieldInfo.FieldType, configurationManager );
				m_ExternalInjectors.Add( Tuple.Create( typeInformation, fieldInfo ) );
			}
			m_HasExternalInjectors = m_ExternalInjectors.Count != 0;

			// Check if there are fields that need to be injected
			foreach( var field in m_Configuration.InjectedFields )
			{
				var fieldName = field.Key;
				FieldInfo fieldInfo = null;
				FirstType( type, t => ( fieldInfo = t.GetField( fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) ) != null );
				if( fieldInfo == null )
				{
					throw new RegisterException( type, "Field '{0}' was not found.".FormatUi( field ) );
				}

				// Create an injector and register a corresponding resolver
				m_Injectors.Add( new SetterInjector( container, fieldInfo, field.Value ) );
				RegisterResolver( fieldInfo.FieldType, name );
			}

			// Check if there are properties that need to be injected
			foreach( var property in m_Configuration.InjectedProperties )
			{
				var propertyName = property.Key;
				PropertyInfo propertyInfo = null;
				FirstType( type, t => ( propertyInfo = t.GetProperty( propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) ) != null && propertyInfo.GetSetMethod( true ) != null );
				if( propertyInfo == null )
				{
					throw new RegisterException( type, "Property '{0}' was not found.".FormatUi( property ) );
				}

				// We need a non-abstract setter
				var methodInfo = propertyInfo.GetSetMethod( true );
				if( methodInfo == null || methodInfo.IsAbstract )
				{
					throw new RegisterException( type, "Property '{0}' has no non-abstract setter.".FormatUi( property ) );
				}

				// Create an injector and register a corresponding resolver
				m_Injectors.Add( new SetterInjector( container, propertyInfo, property.Value ) );
				RegisterResolver( propertyInfo.PropertyType, name );
			}

			// Check if there are methods that need to be injected
			foreach( var method in m_Configuration.InjectedMethods )
			{
				var methodName = method.Key;
				MethodInfo methodInfo = null;
				FirstType( type, t => ( methodInfo = t.GetMethod( methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance ) ) != null );
				if( methodInfo == null )
				{
					throw new RegisterException( type, "Method '{0}' was not found.".FormatUi( methodName ) );
				}

				// Should be non-abstract
				if( methodInfo.IsAbstract )
				{
					throw new RegisterException( type, "Method '{0}' is too abstract.".FormatUi( methodName ) );
				}

				// Create an injector and register a corresponding resolver
				m_Injectors.Add( new MethodInjector( container, methodInfo, method.Value ) );
				RegisterResolver( methodInfo.DeclaringType, name );
			}

			m_HasInjectors = m_Injectors.Count != 0;

			// Are instances of this type disposable?
			IsDisposeable = type.GetInterface( "IDisposable" ) == typeof( IDisposable );
		}

		private static void FirstType( Type type, Func<Type, bool> callback )
		{
			var baseType = type;
			do
			{
				if( callback( baseType ) )
				{
					return;
				}
				baseType = baseType.BaseType;
			}
			while( baseType != null );
		}

		/// <summary>Registers <see cref="Resolver"/>s for injected parameters, if needed.</summary>
		/// <param name="type">The type of the injected parameter.</param>
		/// <param name="name">The name under which we register.</param>
		private void RegisterResolver( Type type, string name )
		{
			if( type.GetInterface( "IResolver" ) == null )
			{
				return;
			}
			if( !m_Container.IsRegistered( name, type ) )
			{
				m_Container.Register.Type( name, type ).WithScope( Scope.Singleton );
			}
		}
		#endregion

		#region IsSingleton, GetSingletonInstance(), IsDisposeable
		/// <summary>Gets whether the type is implementing <see cref="IDisposable"/>.</summary>
		public Scope Scope
		{
			get { return m_Configuration.Scope; }
		}

		public object SingletonInstance
		{
			get
			{
				var hashcode = Scope == Scope.SingletonPerThread ? Thread.CurrentThread.GetHashCode() : 0;
				object instance;
				m_SingletonInstances.TryGetValue( hashcode, out instance );
				return instance;
			}
			set
			{
				var hashcode = Scope == Scope.SingletonPerThread ? Thread.CurrentThread.GetHashCode() : 0;
				m_SingletonInstances.Add( hashcode, value );
			}
		}

		/// <summary>Gets whether the type is implementing <see cref="IDisposable"/>.</summary>
		public bool IsDisposeable { get; private set; }
		#endregion

		#region Inject()
		/// <summary>Injects the fields, properties and methods.</summary>
		/// <param name="instance">The newly created instance that still needs fields, properties and methods injected.</param>
		public void Inject( object instance )
		{
			if( m_HasInjectors )
			{
				// Inject fields and properties
				for( var i = 0; i < m_Injectors.Count; i++ )
				{
					var injector = m_Injectors[i];
					injector.Inject( instance );
				}
			}

			if( m_HasExternalInjectors )
			{
				for( var i = 0; i < m_ExternalInjectors.Count; i++ )
				{
					m_ExternalInjectors[i].Item1.Inject( m_ExternalInjectors[i].Item2.GetValue( instance ) );
				}
			}

			// Call user-defined init-method
			if( m_Configuration.Callback != null )
			{
				m_Configuration.Callback.Invoke( m_Container, instance );
			}
		}
		#endregion
	}
}