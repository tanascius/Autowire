using System;
using System.Collections.Generic;
using System.Reflection;
using Autowire.Utils.Extensions;

namespace Autowire
{
	/// <summary>Holds the configuration given by the user of a type.</summary>
	internal class TypeConfiguration : ITypeConfiguration
	{
		private readonly TypeConfigurationManager m_TypeConfigurationManager;
		private readonly IDictionary<string, Argument> m_Arguments = new Dictionary<string, Argument>();
		private readonly HashSet<KeyValuePair<string, string>> m_InjectedProperties = new HashSet<KeyValuePair<string, string>>();
		private readonly HashSet<KeyValuePair<string, string>> m_InjectedFields = new HashSet<KeyValuePair<string, string>>();
		private readonly HashSet<KeyValuePair<string, MethodConfiguration>> m_InjectedMethods = new HashSet<KeyValuePair<string, MethodConfiguration>>();
		private readonly HashSet<FieldInfo> m_InjectForComponents = new HashSet<FieldInfo>();

		public TypeConfiguration( TypeConfigurationManager typeConfigurationManager, Type type )
		{
			m_TypeConfigurationManager = typeConfigurationManager;
			Type = type;
		}

		#region Type
		public Type Type { get; private set; }
		#endregion

		#region Ignore()
		public void Ignore()
		{
			IsIgnored = true;
		}
		#endregion

		#region WithScope()
		public ILazyConfiguration WithScope( Scope scope )
		{
			Scope = scope;
			return this;
		}

		ITypeConfiguration ITypeConfiguration.WithScope( Scope scope )
		{
			Scope = scope;
			return this;
		}
		#endregion

		#region Argument()
		IArgumentConfiguration IArgumentConfiguration.Arguments( params Argument[] arguments )
		{
			foreach( var argument in arguments )
			{
				m_Arguments.Add( argument.ArgumentName, argument );
			}
			return this;
		}

		ITypeConfiguration ITypeConfiguration.Arguments( params Argument[] arguments )
		{
			foreach( var argument in arguments )
			{
				m_Arguments.Add( argument.ArgumentName, argument );
			}
			return this;
		}
		#endregion

		#region InjectProperty()
		public IMembersConfiguration InjectProperty( string propertyName )
		{
			return ( ( ITypeConfiguration ) this ).InjectProperty( propertyName, String.Empty );
		}

		public IMembersConfiguration InjectProperty( string propertyName, string injectedName )
		{
			return ( ( ITypeConfiguration ) this ).InjectProperty( propertyName, injectedName );
		}

		ITypeConfiguration ITypeConfiguration.InjectProperty( string propertyName )
		{
			return ( ( ITypeConfiguration ) this ).InjectProperty( propertyName, String.Empty );
		}

		ITypeConfiguration ITypeConfiguration.InjectProperty( string propertyName, string injectedName )
		{
			m_InjectedProperties.Add( new KeyValuePair<string, string>( propertyName, injectedName ) );
			return this;
		}
		#endregion

		#region InjectField()
		public IMembersConfiguration InjectField( string fieldName )
		{
			return ( ( ITypeConfiguration ) this ).InjectField( fieldName, string.Empty );
		}

		public IMembersConfiguration InjectField( string fieldName, string injectedName )
		{
			return ( ( ITypeConfiguration ) this ).InjectField( fieldName, injectedName );
		}

		ITypeConfiguration ITypeConfiguration.InjectField( string fieldName )
		{
			return ( ( ITypeConfiguration ) this ).InjectField( fieldName, string.Empty );
		}

		ITypeConfiguration ITypeConfiguration.InjectField( string fieldName, string injectedName )
		{
			m_InjectedFields.Add( new KeyValuePair<string, string>( fieldName, injectedName ) );
			return this;
		}
		#endregion

		#region InjectMethod()
		public IArgumentConfiguration InjectMethod( string methodName )
		{
			var configuration = new MethodConfiguration();
			m_InjectedMethods.Add( new KeyValuePair<string, MethodConfiguration>( methodName, configuration ) );
			return configuration;
		}
		#endregion

		#region InjectForComponent()
		public IMembersConfiguration InjectForComponent( string name )
		{
			// Try to get a field with the specified name
			var fieldInfo = Type.GetField( name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			if( fieldInfo == null )
			{
				throw new ConfigureException( Type, "The field '{0}' was not found.".FormatUi( name ) );
			}

			m_InjectForComponents.Add( fieldInfo );
			return m_TypeConfigurationManager.Get( String.Empty, fieldInfo.FieldType );
		}
		#endregion

		#region AfterResolve()
		public ILazyConfiguration AfterResolve( Action<IContainer, object> callback )
		{
			Callback = callback;
			return this;
		}

		ITypeConfiguration ITypeConfiguration.AfterResolve( Action<IContainer, object> callback )
		{
			Callback = callback;
			return this;
		}
		#endregion

		#region IsIgnored, Scope, Callback
		/// <summary>True, when the type should be ignored during registration, false otherwise.</summary>
		internal bool IsIgnored { get; private set; }

		/// <summary>Returns the scope of the type.</summary>
		internal Scope Scope { get; private set; }

		/// <summary>The callback, that is called after the each resolution.</summary>
		internal Action<IContainer, object> Callback;
		#endregion

		#region Arguments
		/// <summary>Contains all configured arguments of the type's constructor.</summary>
		internal IDictionary<string, Argument> Arguments
		{
			get { return m_Arguments; }
		}
		#endregion

		#region InjectedProperties, InjectedFields, InjectedMethods, InjectForComponents
		/// <summary>Contains all properties that will be injected.</summary>
		internal HashSet<KeyValuePair<string, string>> InjectedProperties
		{
			get { return m_InjectedProperties; }
		}

		/// <summary>Contains all fields that will be injected.</summary>
		internal HashSet<KeyValuePair<string, string>> InjectedFields
		{
			get { return m_InjectedFields; }
		}

		/// <summary>Contains all methods that will be injected.</summary>
		internal HashSet<KeyValuePair<string, MethodConfiguration>> InjectedMethods
		{
			get { return m_InjectedMethods; }
		}

		internal HashSet<FieldInfo> InjectForComponents
		{
			get { return m_InjectForComponents; }
		}
		#endregion

		#region CombineWith()
		internal void CombineWith( TypeConfiguration configuration )
		{
			// Scopes and IsIgnored are not copied ...
			// TypeConfigurationManager will choose the best TypeConfiguration for each type,
			// it will already have a scope and IsIgnored set which we will not overwrite!

			// Arguments are for constructors, they have to be provided with the best
			// TypeConfiguration, too -> no copy

			// Only the first Callback can be used
			if( Callback == null )
			{
				Callback = configuration.Callback;
			}

			m_InjectedProperties.UnionWith( configuration.InjectedProperties );
			m_InjectedFields.UnionWith( configuration.InjectedFields );
			m_InjectedMethods.UnionWith( configuration.InjectedMethods );
			m_InjectForComponents.UnionWith( configuration.InjectForComponents );
		}
		#endregion
	}
}
