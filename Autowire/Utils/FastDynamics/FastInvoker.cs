using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Autowire.Utils.Extensions;

namespace Autowire.Utils.FastDynamics
{
	/// <summary>Creates a new instance of a type in a very fast and flexible way.</summary>
	public sealed class FastInvoker
	{
		private delegate void InitializeHandler( object instance, object[] parameters );

		private delegate object FastInvokationHandler( object[] args );

		private readonly object m_LockObject = new object();
		private readonly ConstructorInfo m_ConstructorInfo;
		private readonly Type m_Type;
		private readonly Type m_BoundType;

		private InitializeHandler m_InitializeHandler;
		private FastInvokationHandler m_FastInvokationHandler;

		/// <summary>Initializes a new instance of the <see cref="FastInvoker" /> class.</summary>
		public FastInvoker( ConstructorInfo constructorInfo, params Type[] genericParameterTypes )
		{
			constructorInfo.CheckNullArgument( "constructorInfo" );
			m_ConstructorInfo = constructorInfo;
			m_Type = constructorInfo.DeclaringType;
			m_BoundType = m_Type.ContainsGenericParameters ? m_Type.MakeGenericType( genericParameterTypes ) : m_Type;
		}

		#region Invoke()
		/// <summary>Create an object by using or creating a special factory</summary>
		public object Invoke( params object[] args )
		{
			if( m_FastInvokationHandler == null )
			{
				lock( m_LockObject )
				{
					if( m_FastInvokationHandler == null )
					{
						m_FastInvokationHandler = CreateFactory();
					}
				}
			}
			return m_FastInvokationHandler.Invoke( args );
		}
		#endregion

		#region CreateFactory()
		/// <summary>Create an object that will used as a 'factory' to the specified type</summary>
		private FastInvokationHandler CreateFactory()
		{
			// Get the constructor - this is m_ConstructorInfo for non-generic types,
			// but for generic types we have to find the corresponding constructor manually
			// (m_ConstructorInfo is for generic types the unbound<T> constructor, that
			// can not be used for invokation)
			var constructorInfo = GetConstructor();

			// Get the types of all constructor parameters
			var parameterInfos = constructorInfo.GetParameters();
			var constructorParameterTypes = new Type[parameterInfos.Length];
			for( var i = 0; i < parameterInfos.Length; i++ )
			{
				constructorParameterTypes[i] = parameterInfos[i].ParameterType;
			}

			// Create the signature for our method (all parameters are typeof( object ))
			var methodSignature = new[]
			{
				typeof( object[] )
			};

			// Create the dynamic method
			var dynMethod = new DynamicMethod( "FastInvoker_" + m_BoundType.Name, typeof( object ), methodSignature, m_BoundType, true );
			var ilGenerator = dynMethod.GetILGenerator();

			// All arguments will be passed through - we will unbox for typechecking
			for( var i = 0; i < parameterInfos.Length; i++ )
			{
				ilGenerator.Emit( OpCodes.Ldarg_0 );
				ilGenerator.Emit( OpCodes.Ldc_I4, i );
				ilGenerator.Emit( OpCodes.Ldelem_Ref );
				ilGenerator.Emit( OpCodes.Unbox_Any, constructorParameterTypes[i] );
			}

			ilGenerator.Emit( OpCodes.Newobj, constructorInfo );
			ilGenerator.Emit( OpCodes.Ret );

			// Compile the dynamic method and return the delegate
			return (FastInvokationHandler)dynMethod.CreateDelegate( typeof( FastInvokationHandler ) );
		}
		#endregion

		#region CreateUninitialized(), Initialize()
		/// <summary>Create an uninitialized object by using or creating a special factory. The constructor object will not be called.</summary>
		public object CreateUninitialized()
		{
			return FormatterServices.GetUninitializedObject( m_BoundType );
		}

		/// <summary>Calls the constructor of the passed object to initialize it.</summary>
		public void Initialize( object instance, params object[] args )
		{
			if( m_InitializeHandler == null )
			{
				lock( m_LockObject )
				{
					if( m_InitializeHandler == null )
					{
						m_InitializeHandler = CreateConstructorCaller();
					}
				}
			}
			m_InitializeHandler.Invoke( instance, args );
		}
		#endregion

		#region CreateConstructorCaller()
		/// <summary>Create an action that will call the specified method and pass all required arguments.</summary>
		private InitializeHandler CreateConstructorCaller()
		{
			var methodInfo = GetConstructor();
			var methodSignature = new[]
			{
				typeof( object ), typeof( object[] )
			};

			// Create the dynamic method
			var type = methodInfo.DeclaringType;
			var dynMethod = new DynamicMethod( "FastConstructor_" + type.Name + "_" + methodInfo.Name, null, methodSignature, type, true );
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
			return (InitializeHandler)dynMethod.CreateDelegate( typeof( InitializeHandler ) );
		}
		#endregion

		#region GetConstructor()
		/// <summary>Get the <see cref="ConstructorInfo"/> that has to be used for invokation.</summary>
		private ConstructorInfo GetConstructor()
		{
			// The constructor is m_ConstructorInfo for non-generic types
			// We have a non-generic type, when type equals m_Type - because for generic types
			// m_Type contains the unbound<T> version, while type is the bound version
			if( m_BoundType == m_Type )
			{
				return m_ConstructorInfo;
			}

			// For generic types m_ConstructorInfo is for the unbound<T> version, that can not
			// be used for invokation - so we have to find the corresponding bound version here
			var genericConstructors = m_Type.GetConstructors();
			var possibleConstructors = m_BoundType.GetConstructors();
			for( var i = 0; i < possibleConstructors.Length; i++ )
			{
				var genericConstructor = genericConstructors[i];
				var possibleConstructor = possibleConstructors[i];

				if( genericConstructor == m_ConstructorInfo )
				{
					return possibleConstructor;
				}
			}

			const string message = "No constructor can be found for the given generic parameters.";
			throw new ArgumentException( message );
		}
		#endregion
	}
}