using System;
using System.Reflection;
using System.Reflection.Emit;
using Autowire.Utils.Extensions;

namespace Autowire.Utils.FastDynamics
{
	/// <summary>Calls a method of an instance in a very fast and flexible way.</summary>
	public sealed class FastMethodCaller
	{
		private delegate void FastMethodCallHandler( object instance, object[] parameters );

		private readonly object m_LockObject = new object();
		private readonly MethodInfo m_MethodInfo;
		private FastMethodCallHandler m_FastMethodCallHandler;

		/// <summary>Initializes a new instance of the <see cref="FastMethodCaller" /> class.</summary>
		public FastMethodCaller( MethodInfo methodInfo )
		{
			methodInfo.CheckNullArgument( "methodInfo" );
			m_MethodInfo = methodInfo;
		}

		#region Call()
		/// <summary>Call the method of the given object and pass all given arguments</summary>
		public void Call( object instance, params object[] args )
		{
			if( m_FastMethodCallHandler == null )
			{
				lock( m_LockObject )
				{
					if( m_FastMethodCallHandler == null )
					{
						m_FastMethodCallHandler = CreateMethodInvoker( instance, args );
					}
				}
			}
			m_FastMethodCallHandler.Invoke( instance, args );
		}
		#endregion

		#region CreateMethodInvoker()
		/// <summary>Create an action that will call the specified method and pass all required arguments.</summary>
		private FastMethodCallHandler CreateMethodInvoker( object instance, object[] args )
		{
			var methodInfo = GetMethodInfo( instance, args );
			var methodSignature = new[]
			{
				typeof( object ), typeof( object[] )
			};

			// Create the dynamic method
			var type = methodInfo.DeclaringType;
			var dynMethod = new DynamicMethod( "FastMethod_" + type.Name + "_" + methodInfo.Name, null, methodSignature, type, true );
			var ilGenerator = dynMethod.GetILGenerator();

			// First argument will be the object of which the method will be set
			ilGenerator.Emit( OpCodes.Ldarg_0 );

			// Other arguments will be passed thru - we will unbox for typechecking
			var parameterInfos = methodInfo.GetParameters();
			for( var i = 0; i < parameterInfos.Length; i++ )
			{
				ilGenerator.Emit( OpCodes.Ldarg_1 );
				ilGenerator.Emit( OpCodes.Ldc_I4, i );
				ilGenerator.Emit( OpCodes.Ldelem_Ref );
				ilGenerator.Emit( OpCodes.Unbox_Any, parameterInfos[i].ParameterType );
			}

			ilGenerator.Emit( OpCodes.Call, methodInfo );
			ilGenerator.Emit( OpCodes.Ret );

			// Compile the dynamic method and return the delegate
			return (FastMethodCallHandler)dynMethod.CreateDelegate( typeof( FastMethodCallHandler ) );
		}
		#endregion

		#region GetMethodInfo()
		private MethodInfo GetMethodInfo( object instance, object[] args )
		{
			if( !m_MethodInfo.ContainsGenericParameters )
			{
				return m_MethodInfo;
			}

			var methodInfo = m_MethodInfo;
			var parameterInfos = methodInfo.GetParameters();
			var instanceType = instance.GetType();

			// At first we bind the generic parameters which are defined by the class
			var classGenericArguments = instanceType.GetGenericArguments();
			if( classGenericArguments.Length != 0 )
			{
				var parameterTypes = new Type[parameterInfos.Length];
				for( var i = 0; i < parameterInfos.Length; i++ )
				{
					// It has to be a generic parameter without a declaring method -> class defined
					var parameterType = parameterInfos[i].ParameterType;
					if( parameterType.IsGenericParameter && parameterType.DeclaringMethod == null )
					{
						parameterType = classGenericArguments[parameterType.GenericParameterPosition];
					}
					parameterTypes[i] = parameterType;
				}

				// Retrieve the methodInfo from the real instance (which is bound)
				methodInfo = instanceType.GetMethod( methodInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, parameterTypes, null );
			}

			// Next we bind the generic parameters defined by the method itself
			var methodGenericArguments = methodInfo.GetGenericArguments();
			if( methodGenericArguments.Length != 0 )
			{
				var parameterTypes = new Type[methodGenericArguments.Length];
				for( var i = 0; i < args.Length; i++ )
				{
					// It has to be a generic parameter with a declaring method
					var parameterType = parameterInfos[i].ParameterType;
					if( parameterType.IsGenericParameter && parameterType.DeclaringMethod != null )
					{
						parameterTypes[parameterType.GenericParameterPosition] = args[i].GetType();
					}
				}

				// Bind all parameters by calling MakeGenericMethod()
				methodInfo = methodInfo.MakeGenericMethod( parameterTypes );
			}

			return methodInfo;
		}
		#endregion
	}
}