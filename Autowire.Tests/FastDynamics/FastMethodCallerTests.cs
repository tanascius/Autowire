using System;
using System.Reflection;
using Autowire.Utils.FastDynamics;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests.FastDynamics
{
	internal class TestClassForMethodCalls
	{
		public string Arg1 { get; private set; }
		public int Arg2 { get; private set; }

		public void Set1Arg( string arg )
		{
			Arg1 = arg;
		}

		private void Set2Args( string arg1, int arg2 )
		{
			Arg1 = arg1;
			Arg2 = arg2;
		}
	}

	internal class TestClassForGenericMethodCalls<T>
	{
		public string MethodArg { get; private set; }
		public T NestedArg { get; private set; }

		private void UngenericMethod( string arg )
		{
			MethodArg = arg;
		}

		internal void SetGenericArg1<TU>( TU arg1, int anyArg1, TU arg2 )
		{
			MethodArg = arg1.ToString();
		}

		protected void SetGenericArg2<TU>( TU arg1, int anyArg1, T arg2 )
		{
			MethodArg = arg1.ToString();
			NestedArg = arg2;
		}
	}

	[TestFixture]
	public class FastMethodCallerTests
	{
		[Test]
		public void CallMethod1Arg()
		{
			var methodInfo = typeof( TestClassForMethodCalls ).GetMethod( "Set1Arg", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var fastMethodCaller = new FastMethodCaller( methodInfo );

			var testClass = new TestClassForMethodCalls();
			fastMethodCaller.Call( testClass, "bleh" );

			Assert.That( testClass.Arg1, Is.EqualTo( "bleh" ) );
			Assert.That( testClass.Arg2, Is.EqualTo( 0 ) );
		}

		[Test]
		public void CallMethod2Args()
		{
			var methodInfo = typeof( TestClassForMethodCalls ).GetMethod( "Set2Args", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var fastMethodCaller = new FastMethodCaller( methodInfo );

			var testClass = new TestClassForMethodCalls();
			fastMethodCaller.Call( testClass, "bleh", 5 );

			Assert.That( testClass.Arg1, Is.EqualTo( "bleh" ) );
			Assert.That( testClass.Arg2, Is.EqualTo( 5 ) );
		}

		[Test, ExpectedException( typeof( InvalidCastException ) )]
		public void CallMethodWrongArgType()
		{
			var methodInfo = typeof( TestClassForMethodCalls ).GetMethod( "Set2Args", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var fastMethodCaller = new FastMethodCaller( methodInfo );

			var testClass = new TestClassForMethodCalls();
			fastMethodCaller.Call( testClass, 5, "bleh" );
		}

		[Test]
		public void CallMethodOnGenericType()
		{
			var methodInfo = typeof( TestClassForGenericMethodCalls<> ).GetMethod( "UngenericMethod", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var fastMethodCaller = new FastMethodCaller( methodInfo );

			var testClass = new TestClassForGenericMethodCalls<string>();
			fastMethodCaller.Call( testClass, "bleh" );

			Assert.That( testClass.MethodArg, Is.EqualTo( "bleh" ) );
		}

		[Test]
		public void CallMethodGenericArgType()
		{
			var methodInfo = typeof( TestClassForGenericMethodCalls<> ).GetMethod( "SetGenericArg1", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var fastMethodCaller = new FastMethodCaller( methodInfo );

			var testClass = new TestClassForGenericMethodCalls<string>();
			fastMethodCaller.Call( testClass, "bleh", 5, "bloo" );

			Assert.That( testClass.MethodArg, Is.EqualTo( "bleh" ) );
		}

		[Test]
		public void CallMethodGenericArgTypeNested()
		{
			var methodInfo = typeof( TestClassForGenericMethodCalls<> ).GetMethod( "SetGenericArg2", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			var fastMethodCaller = new FastMethodCaller( methodInfo );

			var testClass = new TestClassForGenericMethodCalls<int>();
			fastMethodCaller.Call( testClass, "bleh", 5, 5 );

			Assert.That( testClass.MethodArg, Is.EqualTo( "bleh" ) );
			Assert.That( testClass.NestedArg, Is.EqualTo( 5 ) );
		}
	}
}
