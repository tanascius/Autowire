using System;
using Autowire.Registration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests
{
	[TestFixture]
	internal class ResolveFuncTests
	{
		#region Test objects: IBar, Bar, FuncTestObject, NamedFuncTestObject
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable MemberHidesStaticFromOuterClass

		private interface IBar {}

		private class Bar : IBar {}

		private interface IFoo
		{
			IBar Bar { get; }
		}

		private sealed class Foo : IFoo
		{
			public Foo( IBar bar )
			{
				Bar = bar;
			}

			public IBar Bar { get; private set; }
		}

		private class FuncTestObject
		{
			public FuncTestObject( Func<IBar> barFactory )
			{
				Bar = barFactory();
			}

			public IBar Bar { get; private set; }
		}

		private class FuncWithParameter
		{
			private readonly Func<IBar, IFoo> m_FooFactory;

			public FuncWithParameter( Func<IBar, IFoo> fooFactory )
			{
				m_FooFactory = fooFactory;
			}

			public IFoo GetFoo( IBar bar )
			{
				return m_FooFactory( bar );
			}
		}

		private class NamedFuncTestObject
		{
			private readonly Func<string, IBar> m_BarFactory;

			public NamedFuncTestObject( Func<string, IBar> barFactory )
			{
				m_BarFactory = barFactory;
			}

			public IBar GetBar( string name )
			{
				return m_BarFactory( name );
			}
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore MemberHidesStaticFromOuterClass
		#endregion

		[Test]
		public void ResolveSimpleFunc()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<FuncTestObject>();
				container.Register.Type<Bar>();

				var funcTestObject = container.Resolve<FuncTestObject>();

				Assert.That( funcTestObject.Bar, Is.TypeOf( typeof( Bar ) ) );
			}
		}

		[Test]
		public void ResolveFuncWithParameter()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Foo>().Arguments( Argument.UserProvided( "bar" ) );
				container.Register.Type<FuncWithParameter>();
				container.Register.Type<Foo>();

				var funcWithParameter = container.Resolve<FuncWithParameter>();
				var bar = new Bar();
				var foo = funcWithParameter.GetFoo( bar );

				Assert.That( foo.Bar, Is.EqualTo( bar ) );
			}
		}

		[Test]
		public void ResolveNamedFunc()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<NamedFuncTestObject>();

				// Bar "a" is just for distraction
				container.Register.Instance( "a", new Bar() );

				// bBar will be resolved
				var bBar = new Bar();
				container.Register.Instance( "b", bBar );

				var namedFuncTestObject = container.Resolve<NamedFuncTestObject>();
				var resolvedBar = namedFuncTestObject.GetBar( "b" );

				Assert.That( resolvedBar, Is.EqualTo( bBar ) );
			}
		}
	}
}
