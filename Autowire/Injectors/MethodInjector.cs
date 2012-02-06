using System.Reflection;
using Autowire.Utils.Extensions;
using Autowire.Utils.FastDynamics;

namespace Autowire.Injectors
{
	/// <summary>Injects all arguments of a method for newly created instances.</summary>
	internal sealed class MethodInjector : IInjector
	{
		private readonly IContainer m_Container;
		private readonly MethodInfo m_MethodInfo;
		private readonly FastMethodCaller m_FastMethodCaller;
		private readonly Parameter[] m_Parameters;

		/// <summary>Initializes a new instance of the <see cref="SetterInjector" /> class.</summary>
		public MethodInjector( IContainer container, MethodInfo methodInfo, MethodConfiguration configuration )
		{
			container.CheckNullArgument( "container" );
			methodInfo.CheckNullArgument( "methodInfo" );

			m_Container = container;
			m_MethodInfo = methodInfo;
			m_FastMethodCaller = new FastMethodCaller( methodInfo );

			// Get all parameters of the method
			var parameterInfos = methodInfo.GetParameters();
			m_Parameters = new Parameter[parameterInfos.Length];

			// Create Parameter-objects and add them to our collection
			for( var i = 0; i < parameterInfos.Length; i++ )
			{
				// Maybe we got an argument configured for this parameter?
				Argument argument;
				var parameterInfo = parameterInfos[i];
				configuration.Arguments.TryGetValue( parameterInfo.Name, out argument );

				// Create the Parameter (argument is allowed to be null)
				m_Parameters[i] = new Parameter( container, parameterInfo, argument );
			}
		}

		#region Inject()
		/// <summary>Inject the method for the given instance.</summary>
		public void Inject( object instance )
		{
			// Resolve all arguments
			var args = new object[m_Parameters.Length];
			for( var i = 0; i < m_Parameters.Length; i++ )
			{
				var parameter = m_Parameters[i];
				var parameterType = parameter.Type.IsGenericParameter ? instance.GetType().GetGenericArguments()[parameter.Type.GenericParameterPosition] : parameter.Type;
				args[i] = m_Container.ResolveByName( parameter.InjectedName, parameterType );
				if( args[i] == null )
				{
					throw new ResolveException( m_MethodInfo.DeclaringType, "The injected property '{0}' (of type '{1}') can not be resolved.".FormatUi( m_MethodInfo.Name, parameterType.Name ) );
				}
			}

			// Invoke Method
			m_FastMethodCaller.Call( instance, args );
		}
		#endregion
	}
}
