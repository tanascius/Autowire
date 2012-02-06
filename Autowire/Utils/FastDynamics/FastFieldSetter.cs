using System;
using System.Reflection;
using System.Reflection.Emit;
using Autowire.Utils.Extensions;

namespace Autowire.Utils.FastDynamics
{
	/// <summary>Sets the field of an instance in a very fast and dynamic way.</summary>
	public sealed class FastFieldSetter : IFastSetter
	{
		private readonly FieldInfo m_FieldInfo;
		private readonly object m_LockObject = new object();
		private Action<object, object> m_SetField;

		/// <summary>Initializes a new instance of the <see cref="FastFieldSetter" /> class.</summary>
		public FastFieldSetter( FieldInfo fieldInfo )
		{
			fieldInfo.CheckNullArgument( "fieldInfo" );
			m_FieldInfo = fieldInfo;
		}

		#region Set()
		/// <summary>Sets the specified field of the given instance to a new value.</summary>
		/// <param name="instance">The instance of which the field value will be changed.</param>
		/// <param name="value">The value that will be set.</param>
		public void Set( object instance, object value )
		{
			if( m_SetField == null )
			{
				lock( m_LockObject )
				{
					if( m_SetField == null )
					{
						m_SetField = CreateSetter( instance );
					}
				}
			}
			m_SetField.Invoke( instance, value );
		}
		#endregion

		#region CreateSetter()
		/// <summary>Create a dynamic method that will set the specified field.</summary>
		private Action<object, object> CreateSetter( object instance )
		{
			var fieldInfo = GetFieldInfo( instance );

			var objectType = typeof( object );
			var type = fieldInfo.DeclaringType;

			// Create the signature for our method (all parameters are typeof( object ))
			var methodSignature = new[]
			{
				objectType, objectType
			};

			// Create the dynamic method
			var dynMethod = new DynamicMethod( "DM$FIELD_INJECTOR_" + type.Name + "_" + fieldInfo.Name, null, methodSignature, type );
			var ilGen = dynMethod.GetILGenerator();

			// First argument will be the object of which the field will be set
			ilGen.Emit( OpCodes.Ldarg_0 );

			// Second argument is the value - we will unbox for typechecking
			ilGen.Emit( OpCodes.Ldarg_1 );
			ilGen.Emit( OpCodes.Unbox_Any, fieldInfo.FieldType );

			ilGen.Emit( OpCodes.Stfld, fieldInfo );
			ilGen.Emit( OpCodes.Ret );

			// Compile the dynamic method and return the delegate
			return ( Action<object, object> ) dynMethod.CreateDelegate( typeof( Action<object, object> ) );
		}
		#endregion

		#region GetFieldInfo()
		private FieldInfo GetFieldInfo( object instance )
		{
			return m_FieldInfo.FieldType.IsGenericParameter ? instance.GetType().GetField( m_FieldInfo.Name ) : m_FieldInfo;
		}
		#endregion
	}
}
