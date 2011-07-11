using System;
using System.Reflection;
using Autowire.Utils.FastDynamics;
using NUnit.Framework;

namespace Autowire.Tests.FastDynamics
{
	[TestFixture]
	public class FastInvokerTests
	{
		[Test]
		public void CreateInstanceNoArg()
		{
			var fastInvoker = new FastInvoker( typeof( TestClassForInvokation ).GetConstructors()[0] );
			var instance = (TestClassForInvokation)fastInvoker.Invoke();

			Assert.IsNotNull( instance );
			Assert.AreEqual( null, instance.Arg1 );
			Assert.AreEqual( 0, instance.Arg2 );
		}

		[Test]
		public void RegisterAndResolveBoundGeneric()
		{
			var fastInvoker = new FastInvoker( typeof( Resolver<Bar> ).GetConstructors( BindingFlags.Instance | BindingFlags.NonPublic )[0] );
			var instance = fastInvoker.Invoke( new Container() );

			Assert.IsNotNull( instance );
		}

		[Test]
		public void CreateInstance1Arg()
		{
			var fastInvoker = new FastInvoker( typeof( TestClassForInvokation ).GetConstructors()[1] );
			var instance = (TestClassForInvokation)fastInvoker.Invoke( "bleh" );

			Assert.IsNotNull( instance );
			Assert.AreEqual( "bleh", instance.Arg1 );
			Assert.AreEqual( 0, instance.Arg2 );
		}

		[Test]
		public void CreateInstance2Args()
		{
			var fastInvoker = new FastInvoker( typeof( TestClassForInvokation ).GetConstructors()[2] );
			var instance = (TestClassForInvokation)fastInvoker.Invoke( "bleh", 5 );

			Assert.IsNotNull( instance );
			Assert.AreEqual( "bleh", instance.Arg1 );
			Assert.AreEqual( 5, instance.Arg2 );
		}

		[Test, ExpectedException( typeof( InvalidCastException ) )]
		public void CreateInstanceWrongArgType()
		{
			var fastInvoker = new FastInvoker( typeof( TestClassForInvokation ).GetConstructors()[2] );
			fastInvoker.Invoke( 5, "bleh" );
		}

		[Test]
		public void CreateGenericInstanceNoParameter()
		{
			var fastInvoker = new FastInvoker( typeof( TestClassForInvokation ).GetConstructors()[0] );
			var instance = fastInvoker.Invoke();

			Assert.IsNotNull( instance );
			Assert.IsInstanceOfType( typeof( TestClassForInvokation ), instance );
		}

		[Test]
		public void CreateGenericInstanceOneParameter()
		{
			var fastInvoker = new FastInvoker( typeof( GenericTestClassForInvokationOneParameter<> ).GetConstructors()[0], typeof( string ) );
			var instance = fastInvoker.Invoke();

			Assert.IsNotNull( instance );
			Assert.IsInstanceOfType( typeof( GenericTestClassForInvokationOneParameter<string> ), instance );
		}

		[Test]
		public void CreateGenericInstanceTwoParameters()
		{
			var fastInvoker = new FastInvoker( typeof( GenericTestClassForInvokationTwoParameters<,> ).GetConstructors()[0], typeof( string ), typeof( int ) );
			var instance = fastInvoker.Invoke();

			Assert.IsNotNull( instance );
			Assert.IsInstanceOfType( typeof( GenericTestClassForInvokationTwoParameters<string, int> ), instance );
		}

		[Test]
		public void CreateGenericInstanceNoParameterOneArgument()
		{
			var fastInvoker = new FastInvoker( typeof( GenericTestClassForInvokationOneParameter<> ).GetConstructors()[1], typeof( string ) );
			var instance = (GenericTestClassForInvokationOneParameter<string>)fastInvoker.Invoke( "bleh" );

			Assert.IsNotNull( instance );
			Assert.AreEqual( "bleh", instance.Property );
		}
	}

	#region Testclasses
	internal class GenericTestClassForInvokationOneParameter<T>
	{
		public GenericTestClassForInvokationOneParameter() {}

		public GenericTestClassForInvokationOneParameter( T value )
		{
			Property = value;
		}

		public T Property { get; set; }
	}

	internal class GenericTestClassForInvokationTwoParameters<T, TU>
	{
		public T Property1 { get; set; }
		public TU Property2 { get; set; }
	}

	internal class TestClassForInvokation
	{
		public TestClassForInvokation() {}

		public TestClassForInvokation( string arg )
		{
			Arg1 = arg;
		}

		public TestClassForInvokation( string arg1, int arg2 )
		{
			Arg1 = arg1;
			Arg2 = arg2;
		}

		public string Arg1 { get; private set; }
		public int Arg2 { get; private set; }
	}
	#endregion
}