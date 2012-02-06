using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autowire.Utils.Extensions;
using Autowire.Utils.FastDynamics;

namespace Autowire.Factories
{
	/// <summary>A factory for a given type's constructor that can be invoked to resolve an instance.</summary>
	internal sealed class Factory : IFactory
	{
		private readonly object m_FastInvokerLocker = new object();

		private readonly bool m_HasParameters;
		private readonly bool m_HasUserParameters;
		private readonly Collection<Parameter> m_Parameters = new Collection<Parameter>();
		private readonly Collection<IDisposable> m_DisposableInstances = new Collection<IDisposable>();
		private readonly Dictionary<Type, FastInvoker> m_GenericFastInvoker = new Dictionary<Type, FastInvoker>();
		private readonly ConstructorInfo m_ConstructorInfo;
		private readonly TypeInformation m_TypeInformation;
		private readonly Container m_Container;
		private readonly Type m_Type;
		private readonly bool m_IsGenericType;
		private readonly FastInvoker m_FastInvoker;

		#region Constructor
		/// <summary>Initializes a new instance of the <see cref="Factory" /> class.</summary>
		/// <param name="container">The <see cref="Container"/> that is used.</param>
		/// <param name="constructorInfo">The constructor that can be invoked by this entry.</param>
		/// <param name="typeInformation">Sets, whether an instance is handled as a singleton or not.</param>
		/// <param name="isRegisteredByUser">True, if this factory was registered by the user, false if it was regeistered automatically.</param>
		/// <param name="configuration">The <see cref="TypeConfiguration"/> for this type.</param>
		public Factory( Container container, ConstructorInfo constructorInfo, TypeInformation typeInformation, bool isRegisteredByUser, TypeConfiguration configuration )
		{
			m_Container = container;
			m_ConstructorInfo = constructorInfo;
			m_TypeInformation = typeInformation;
			IsRegisteredByUser = isRegisteredByUser;

			m_Type = constructorInfo.DeclaringType;
			m_IsGenericType = constructorInfo.DeclaringType.IsGenericType;

			if( !m_IsGenericType )
			{
				m_FastInvoker = new FastInvoker( m_ConstructorInfo, null );
			}

			var parameters = constructorInfo.GetParameters();
			for( var i = 0; i < parameters.Length; i++ )
			{
				var parameterInfo = parameters[i];
				Argument argument;
				configuration.Arguments.TryGetValue( parameterInfo.Name, out argument );

				var parameter = new Parameter( container, parameterInfo, argument );
				m_Parameters.Add( parameter );
				m_HasUserParameters |= parameter.IsUserInput;

				if( parameter.IsUserInput || parameterInfo.ParameterType.GetInterface( "IResolver" ) == null )
				{
					continue;
				}

				if( !m_Container.IsRegistered( parameterInfo.ParameterType ) )
				{
					m_Container.Register.Type( string.Empty, parameterInfo.ParameterType ).WithScope( Scope.Singleton );
				}
			}
			m_HasParameters = m_Parameters.Count != 0;
		}
		#endregion

		#region IsRegisteredByUser
		public bool IsRegisteredByUser { get; private set; }
		#endregion

		#region Invoke()
		public object Invoke( IContainer container, Type type, object[] args )
		{
			// For singletons we have an own method
			if( m_TypeInformation.Scope != Scope.Instance )
			{
				return InvokeSingleton( container, type, args );
			}

			// Create the instance and inject properties/methods
			var instance = m_HasParameters ? CreateInstaceWithArguments( container, type, args ) : GetFastInvoker( type ).Invoke();
			m_TypeInformation.Inject( instance );

			// Is the instance disposeable?
			if( m_TypeInformation.IsDisposeable )
			{
				m_DisposableInstances.Add( instance as IDisposable );
			}

			// Return the instance - finally :)
			return instance;
		}

		private object InvokeSingleton( IContainer container, Type type, object[] args )
		{
			var instance = m_TypeInformation.SingletonInstance;
			if( instance == null )
			{
				lock( m_TypeInformation )
				{
					instance = m_TypeInformation.SingletonInstance;
					if( instance == null )
					{
						// Create a new instance
						instance = m_HasParameters ? CreateInstaceWithArguments( container, type, args ) : GetFastInvoker( type ).Invoke();

						// Register an instance without parameters
						if( m_HasUserParameters )
						{
							m_Container.SetAsSingleton( null, instance );
						}

						// Set the instance FIRST
						m_TypeInformation.SingletonInstance = instance;

						// Now, inject - this enables us to do recusive injections for fields/properties/methods
						// A type can inject itself this way (useful for injecting lists where the type is contained)
						m_TypeInformation.Inject( instance );

						// Is the instance disposeable?
						if( m_TypeInformation.IsDisposeable )
						{
							m_DisposableInstances.Add( instance as IDisposable );
						}
					}
				}
			}
			return instance;
		}
		#endregion

		#region CanInvoke()
		public bool CanInvoke( Type type, object[] args )
		{
			var unboundType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
			if( !unboundType.IsAssignableFrom( m_Type ) )
			{
				return false;
			}

			var parameterIndex = 0;
			for( var i = 0; i < m_Parameters.Count; i++ )
			{
				var parameter = m_Parameters[i];
				if( !parameter.IsUserInput )
				{
					continue;
				}
				if( parameter.Type.IsGenericType || parameter.Type.IsGenericParameter )
				{
					// HACK here the exact type must be looked up
					parameterIndex++;
					continue;
				}
				var providedArg = args[parameterIndex++];
				var providedNullArg = providedArg as INullArg;
				var providedType = providedNullArg != null ? providedNullArg.Type : providedArg.GetType();
				if( !parameter.Type.IsAssignableFrom( providedType ) )
				{
					return false;
				}
			}

			return true;
		}
		#endregion

		#region CreateInstaceWithArguments()
		/// <summary>Returns an instance which needs arguments to be constructed.</summary>
		/// <param name="container">The container which is used to resolve arguments.</param>
		/// <param name="type">The type that has to be invoked.</param>
		/// <param name="args">All arguments which are not auto-injected.</param>
		private object CreateInstaceWithArguments( IContainer container, Type type, object[] args )
		{
			// Ok, so we have arguments ... at first we need to check if there are injected arguments missing
			var userProvidedArgumentIndex = 0;
			var argumentIndex = 0;
			var argumentsToUse = new object[m_Parameters.Count];
			for( var i = 0; i < m_Parameters.Count; i++ )
			{
				// So check every argument if it is user-provided or needs to be injected
				var parameter = m_Parameters[i];
				if( parameter.IsUserInput )
				{
					var arg = args[userProvidedArgumentIndex++];
					if( arg is INullArg )
					{
						arg = null;
					}
					argumentsToUse[argumentIndex++] = arg;
				}
				else
				{
					object injectedArgument;
					if( parameter.HasValue )
					{
						injectedArgument = parameter.Value;
					}
					else
					{
						var parameterType = parameter.Type;
						if( parameterType.IsGenericParameter )
						{
							parameterType = type.GetGenericArguments()[parameterType.GenericParameterPosition];
						}
						injectedArgument = container.ResolveByName( parameter.InjectedName, parameterType );
						if( injectedArgument == null )
						{
							var injectedName = string.IsNullOrEmpty( parameter.InjectedName ) ? "" : ", injected name = '{0}'".FormatUi( parameter.InjectedName );
							var message = "The injected parameter '{0}' (of type '{1}'{2}) can not be resolved.".FormatUi( parameter.Name, parameter.Type.Name, injectedName );
							throw new ResolveException( type, message );
						}
					}
					argumentsToUse[argumentIndex++] = injectedArgument;
				}
			}

			return GetFastInvoker( type ).Invoke( argumentsToUse );
		}
		#endregion

		#region GetFastInvoker()
		private FastInvoker GetFastInvoker( Type type )
		{
			if( !m_IsGenericType )
			{
				return m_FastInvoker;
			}

			FastInvoker fastInvoker;
			if( !m_GenericFastInvoker.TryGetValue( type, out fastInvoker ) )
			{
				lock( m_FastInvokerLocker )
				{
					if( !m_GenericFastInvoker.TryGetValue( type, out fastInvoker ) )
					{
						var genericParameterTypes = type.GetGenericArguments();
						fastInvoker = new FastInvoker( m_ConstructorInfo, genericParameterTypes );
						m_GenericFastInvoker.Add( type, fastInvoker );
					}
				}
			}

			return fastInvoker;
		}
		#endregion

		#region IDisposable
		///<summary>Returns, whether the Dispose()-method was already called for this object.</summary>
		private bool m_IsDisposed;

		///<summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		// SuppressMessage, da sich FxCop an der AutoProperty stört :/
		[SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly" )]
		public void Dispose()
		{
			if( m_IsDisposed )
			{
				return;
			}
			m_IsDisposed = true;

			// Cleanup managed Resources
			m_DisposableInstances.Apply( item => item.Dispose() );

			GC.SuppressFinalize( this );
		}
		#endregion
	}
}