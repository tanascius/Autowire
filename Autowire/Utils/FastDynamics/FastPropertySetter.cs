using System;
using System.Reflection;
using System.Reflection.Emit;
using Autowire.Utils.Extensions;

namespace Autowire.Utils.FastDynamics
{
	/// <summary>Sets the property of an instance in a very fast and dynamic way.</summary>
	public sealed class FastPropertySetter : IFastSetter
	{
		private readonly object m_LockObject = new object();
		private readonly PropertyInfo m_PropertyInfo;
		private Action<object, object> m_SetProperty;

		/// <summary>Initializes a new instance of the <see cref="FastPropertySetter" /> class.</summary>
		public FastPropertySetter( PropertyInfo propertyInfo )
		{
			propertyInfo.CheckNullArgument( "propertyInfo" );
			m_PropertyInfo = propertyInfo;
		}

		#region Set()
		/// <summary>Set the specified property of the given instance to a new value.</summary>
		/// <param name="instance">The instance of which the property value will be changed.</param>
		/// <param name="value">The value that will be set.</param>
		public void Set( object instance, object value )
		{
			if( m_SetProperty == null )
			{
				lock( m_LockObject )
				{
					if( m_SetProperty == null )
					{
						m_SetProperty = CreateSetter( instance );
					}
				}
			}
			m_SetProperty.Invoke( instance, value );
		}
		#endregion

		#region CreateSetter()
		/// <summary>Create a dynamic method that will set the specified property.</summary>
		private Action<object, object> CreateSetter( object instance )
		{
			var propertyInfo = GetPropertyInfo( instance );

			var objectType = typeof( object );
			var methodInfo = propertyInfo.GetSetMethod( true );
			if( methodInfo == null )
			{
				throw new InvalidOperationException( "The property '{0}.{1}' seems to be write-protected.".FormatUi( m_PropertyInfo.DeclaringType.Name, m_PropertyInfo.PropertyType.Name ) );
			}
			var type = methodInfo.DeclaringType;

			// Create the signature for our method (all parameters are typeof( object ))
			var methodSignature = new[]
			{
				objectType, objectType
			};

			// Create the dynamic method
			var dynMethod = new DynamicMethod( "DM$PROPERTY_INJECTOR_" + type.Name + "_" + propertyInfo.Name, null, methodSignature, type );
			var ilGen = dynMethod.GetILGenerator();

			// First argument will be the object of which the property will be set
			ilGen.Emit( OpCodes.Ldarg_0 );

			// Second argument is the value - we will unbox for typechecking
			ilGen.Emit( OpCodes.Ldarg_1 );
			ilGen.Emit( OpCodes.Unbox_Any, propertyInfo.PropertyType );

			ilGen.Emit( OpCodes.Call, methodInfo );
			ilGen.Emit( OpCodes.Ret );

			// Compile the dynamic method and return the delegate
			return (Action<object, object>)dynMethod.CreateDelegate( typeof( Action<object, object> ) );
		}
		#endregion

		#region GetPropertyInfo()
		private PropertyInfo GetPropertyInfo( object instance )
		{
			return m_PropertyInfo.PropertyType.IsGenericParameter ? instance.GetType().GetProperty( m_PropertyInfo.Name ) : m_PropertyInfo;
		}
		#endregion
	}
}