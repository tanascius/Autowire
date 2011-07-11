using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Autowire.Utils.Extensions;

namespace Autowire.Utils.FastDynamics
{
	/// <summary>Creates a delegate that uses <see cref="IContainer.Resolve"/> to resolve instances lazily. </summary>
	public class FastDelegateFactory
	{
		private readonly IContainer m_Container; // used though reflection
		private readonly Type m_FuncType;

		/// <summary>Initializes a new instance of the <see cref = "FastDelegateFactory" /> class.</summary>
		public FastDelegateFactory( IContainer container, Type funcType )
		{
			funcType.CheckNullArgument( "funcType" );
			funcType.CheckNullArgument( "container" );
			m_Container = container;
			m_FuncType = funcType;

			Delegate = CreateDelegate();
		}

		/// <summary>The delegate that was created.</summary>
		public Delegate Delegate { get; private set; }

		private Delegate CreateDelegate()
		{
			var genericArguments = m_FuncType.GetGenericArguments();
			var returnType = genericArguments[genericArguments.Length - 1];
			var parameterTypes = new[] {GetType()};
			if( genericArguments.Length > 1 )
			{
				parameterTypes = parameterTypes.Concat( genericArguments.Take( genericArguments.Length - 1 ) ).ToArray();
			}

			var dynMethod = new DynamicMethod( "", returnType, parameterTypes, GetType(), true );
			var containerFieldInfo = GetType().GetField( "m_Container", BindingFlags.Instance | BindingFlags.NonPublic );
			var ilGenerator = dynMethod.GetILGenerator();

			// Load the m_Container field, first push `this` as an argument (because it is an instance field)
			ilGenerator.Emit( OpCodes.Ldarg_0 );
			ilGenerator.Emit( OpCodes.Ldfld, containerFieldInfo );

			// Create a new array for the arguments
			ilGenerator.Emit( OpCodes.Ldc_I4, genericArguments.Length - 1 );
			ilGenerator.Emit( OpCodes.Newarr, typeof( object ) );

			// Fill with supplied arguments
			if( genericArguments.Length > 1 )
			{
				// Create local variable
				ilGenerator.DeclareLocal( typeof( object[] ) );

				// Store the array in the local variable
				ilGenerator.Emit( OpCodes.Stloc_0 );
				ilGenerator.Emit( OpCodes.Ldloc_0 );

				for( var i = 0; i < genericArguments.Length - 1; i++ )
				{
					// Assign an argument to the array
					ilGenerator.Emit( OpCodes.Ldc_I4, i );
					ilGenerator.Emit( OpCodes.Ldarg, i + 1 );
					ilGenerator.Emit( OpCodes.Stelem_Ref );
					ilGenerator.Emit( OpCodes.Ldloc_0 );
				}
			}

			// Get and call the resolve method
			var resolveMethod = typeof( IContainer ).GetMethod( "Resolve", new[] {typeof( object[] )} );
			resolveMethod = resolveMethod.MakeGenericMethod( new[] {returnType} );
			ilGenerator.Emit( OpCodes.Callvirt, resolveMethod );

			// Return the created instance
			ilGenerator.Emit( OpCodes.Ret );

			// Build the delegate we need - it is an instance delegate, so pass `this`
			return dynMethod.CreateDelegate( m_FuncType, this );
		}
	}
}
