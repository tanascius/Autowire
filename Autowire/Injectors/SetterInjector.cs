using System;
using System.Reflection;
using Autowire.Resolving;
using Autowire.Utils.Extensions;
using Autowire.Utils.FastDynamics;

namespace Autowire.Injectors
{
	/// <summary>Injects a field or property for newly created instances.</summary>
	internal sealed class SetterInjector : IInjector
	{
		private readonly IContainer m_Container;
		private readonly MemberInfo m_PropertyInfo;
		private readonly string m_InjectedName;
		private readonly IFastSetter m_FastSetter;
		private readonly Type m_InjectedType;

		#region Constructors
		/// <summary>Initializes a new instance of the <see cref="SetterInjector" /> class.</summary>
		public SetterInjector( IContainer container, PropertyInfo propertyInfo, string injectedName ) : this( container, injectedName )
		{
			propertyInfo.CheckNullArgument( "propertyInfo" );

			m_PropertyInfo = propertyInfo;
			m_FastSetter = new FastPropertySetter( propertyInfo );
			m_InjectedType = propertyInfo.PropertyType;
		}

		/// <summary>Initializes a new instance of the <see cref="SetterInjector" /> class.</summary>
		public SetterInjector( IContainer container, FieldInfo fieldInfo, string injectedName ) : this( container, injectedName )
		{
			fieldInfo.CheckNullArgument( "fieldInfo" );

			m_PropertyInfo = fieldInfo;
			m_FastSetter = new FastFieldSetter( fieldInfo );
			m_InjectedType = fieldInfo.FieldType;
		}

		private SetterInjector( IContainer container, string injectedName )
		{
			container.CheckNullArgument( "container" );
			m_Container = container;
			m_InjectedName = injectedName;
		}
		#endregion

		#region Inject()
		/// <summary>Inject the field or property for the given instance.</summary>
		public void Inject( object instance )
		{
			object injectedArgument;
			try
			{
				injectedArgument = m_Container.ResolveByName( m_InjectedName, GetParameterType( instance ) );
			}
			catch( ResolveException exception )
			{
				// Replace the generic "type can not be resolved exception" text with a more specific exception text
				throw new ResolveException( m_PropertyInfo.DeclaringType, "The injected property '{0}' (of type '{1}') can not be resolved.".FormatUi( m_PropertyInfo.Name, m_InjectedType.Name ), exception );
			}
			if( injectedArgument == null )
			{
				throw new ResolveException( m_PropertyInfo.DeclaringType, "The injected property '{0}' (of type '{1}') can not be resolved.".FormatUi( m_PropertyInfo.Name, m_InjectedType.Name ) );
			}
			m_FastSetter.Set( instance, injectedArgument );
		}

		private Type GetParameterType( object instance )
		{
			return m_InjectedType.IsGenericParameter ? instance.GetType().GetGenericArguments()[m_InjectedType.GenericParameterPosition] : m_InjectedType;
		}
		#endregion
	}
}
