using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class ArgumentTests
	{
		#region Testobjects: IBar, Bar, IFoo, Foo, Foo2
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

		private sealed class Foo2 : IFoo
		{
			public Foo2( IBar bar )
			{
				Bar = bar;
			}

			public IBar Bar { get; private set; }
		}

		// ReSharper restore MemberHidesStaticFromOuterClass
		// ReSharper restore ClassNeverInstantiated.Local
		#endregion

		[Test]
		[Description( "Resolve a Foo which needs a Bar, that needs to be auto-injected." )]
		public void AutoinjectArgument()
		{
			using( var container = new Container( true ) )
			{
				var bar = new Bar();
				container.Register.Instance( bar );
				container.Register.Type<Foo>();

				var foo = container.Resolve<IFoo>();

				Assert.IsNotNull( foo );
				Assert.AreEqual( foo.Bar, bar );
			}
		}

		[Test]
		[Description( "Resolve a Foo which needs a Bar, that is given during registration." )]
		public void UseArgumentDuringRegister()
		{
			using( var container = new Container() )
			{
				var bar = new Bar();
				container.Configure<Foo>().Arguments( Argument.Create( "bar", bar ) );
				container.Register.Type<Foo>();

				var foo = container.Resolve<IFoo>();

				Assert.IsNotNull( foo );
				Assert.AreEqual( foo.Bar, bar );
			}
		}

		[Test]
		[Description( "Resolve a Foo which needs a Bar, that is given for IFoo during registration." )]
		public void UseArgumentOfInterfaceDuringRegister()
		{
			using( var container = new Container() )
			{
				var bar = new Bar();
				container.Configure<IFoo>().Arguments( Argument.Create( "bar", bar ) );
				container.Register.Type<Foo>();

				var foo = container.Resolve<IFoo>();

				Assert.IsNotNull( foo );
				Assert.AreEqual( foo.Bar, bar );
			}
		}

		[Test]
		[Description( "Resolve a Foo and a Foo2 which need a Bar, that is given for IFoo and Foo during registration." )]
		public void OverwrittenArgumentDuringRegister()
		{
			using( var container = new Container() )
			{
				var barInterface = new Bar();
				var barForFoo = new Bar();
				container.Configure<IFoo>().Arguments( Argument.Create( "bar", barInterface ) );
				container.Configure<Foo>().Arguments( Argument.Create( "bar", barForFoo ) );
				container.Register.Type<Foo>();
				container.Register.Type<Foo2>();

				var foo = container.Resolve<Foo>();
				Assert.IsNotNull( foo );
				Assert.AreEqual( foo.Bar, barForFoo );

				var foo2 = container.Resolve<Foo2>();
				Assert.IsNotNull( foo2 );
				Assert.AreEqual( foo2.Bar, barInterface );
			}
		}

		[Test]
		[Description( "Resolve a Foo which needs a Bar, that is given during creation." )]
		public void UseArgumentDuringResolve()
		{
			using( var container = new Container() )
			{
				var bar = new Bar();
				container.Configure<Foo>().Arguments( Argument.UserProvided( "bar" ) );
				container.Register.Type<Foo>();

				var foo = container.Resolve<IFoo>( bar );

				Assert.IsNotNull( foo );
				Assert.AreEqual( foo.Bar, bar );
			}
		}

		[Test]
		[ExpectedException( typeof( ResolveException ) )]
		[Description( "Resolve a Foo which needs a Bar, that is given during registration and creation twice." )]
		public void ArgumentDuringRegisterAndResolve()
		{
			using( var container = new Container( true ) )
			{
				var bar = new Bar();
				container.Configure<Foo>().Arguments( Argument.Create( "bar", bar ) );
				container.Register.Type<Foo>();

				container.Resolve<IFoo>( bar );
			}
		}

		[Test]
		[ExpectedException( typeof( ResolveException ) )]
		[Description( "Resolve a Foo which needs a Bar, but is not given at all." )]
		public void ForgottenArgument()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Foo>();
				container.Resolve<IFoo>();
			}
		}
	}
}
