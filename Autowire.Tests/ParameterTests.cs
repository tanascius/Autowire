using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests
{
	[TestFixture]
	public class ParameterTests
	{
		#region Test objects: IFoo, Foo, IBar, Bar, BarDerived, BarDerived2, Args1, Args2, Args3, Args4
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable MemberHidesStaticFromOuterClass
		// ReSharper disable UnusedAutoPropertyAccessor.Local
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local

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

		private interface IBar {}

		private class Bar : IBar {}

		private sealed class BarDerived : Bar {}

		private sealed class BarDerived2 : Bar {}

		private sealed class DuplicatedConstructor
		{
			public DuplicatedConstructor( IBar bar ) {}

			public DuplicatedConstructor( string text ) {}
		}

		private sealed class Args1
		{
			public Args1( string arg ) {}
		}

		private sealed class Args2
		{
			public Args2( string arg1, string arg2 ) {}
		}

		private sealed class Args3
		{
			public Args3( string arg1, string arg2, string arg3 ) {}
		}

		private sealed class Args4
		{
			private string Arg1 { get; set; }
			private string Arg2 { get; set; }
			private string Arg3 { get; set; }
			private string Arg4 { get; set; }

			public Args4( string arg1, string arg2, string arg3, string arg4 )
			{
				Arg1 = arg1;
				Arg2 = arg2;
				Arg3 = arg3;
				Arg4 = arg4;
			}
		}

		private sealed class Args8
		{
			public Args8( string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8 ) {}
		}

		private sealed class InjectedArgument
		{
			public InjectedArgument( IFoo foo, IBar bar )
			{
				Assert.IsNotNull( foo );
				Assert.IsInstanceOfType( typeof( Foo ), foo );
				Assert.AreEqual( bar, foo.Bar );
			}
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore MemberHidesStaticFromOuterClass
		// ReSharper restore UnusedAutoPropertyAccessor.Local
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
		#endregion

		[Test]
		public void PassArgumentToConstructor()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Foo>().Arguments( Argument.UserProvided( "bar" ) );

				container.Register.Type<Foo>();

				var bar = new Bar();
				var foo = container.Resolve<IFoo>( bar );

				Assert.AreEqual( foo.Bar, bar );
			}
		}

		[Test]
		public void PassArgumentToConstructorByGivenType()
		{
			using( var container = new Container( true ) )
			{
				container.Configure( typeof( Foo ) ).Arguments( Argument.UserProvided( "bar" ) );

				container.Register.Type( typeof( Foo ) );

				var bar = new Bar();
				var foo = ( Foo ) container.Resolve( typeof( IFoo ), bar );

				Assert.AreEqual( bar, foo.Bar );
			}
		}

		[Test]
		public void PassDerivedArgumentToConstructor()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Foo>().Arguments( Argument.UserProvided( "bar" ) );

				container.Register.Type<Foo>();

				var bar = new BarDerived();
				var foo = container.Resolve<IFoo>( bar );

				Assert.AreEqual( foo.Bar, bar );
			}
		}

		[Test]
		[ExpectedException( typeof( ResolveException ) )]
		public void PassNullToConstructor()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Foo>();
				container.Resolve<IFoo>( null );
			}
		}

		[Test]
		[Description( "Register Foo, but not Bar. Try to resolve Foo - will return null" )]
		public void CtorResolveReturnsNull()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Foo>();
				var foo = container.Resolve<IFoo>();
				Assert.That( foo, Is.Null );
			}
		}

		[Test]
		public void PassNullArgumentToConstructor()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Foo>().Arguments( Argument.UserProvided( "bar" ) );

				container.Register.Type<Foo>();
				var foo = container.Resolve<IFoo>( NullArg.New<Bar>() );

				Assert.IsNull( foo.Bar );
			}
		}

		[Test]
		public void RegisterAndResolve1Arg()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Args1>().Arguments( Argument.UserProvided( "arg" ) );

				container.Register.Type<Args1>();

				Assert.IsNotNull( container.Resolve<Args1>( "a" ) );
				Assert.IsNotNull( container.Resolve( typeof( Args1 ), "a" ) );
			}
		}

		[Test]
		public void RegisterAndResolve2Args()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Args2>().Arguments( Argument.UserProvided( "arg1" ), Argument.UserProvided( "arg2" ) );

				container.Register.Type<Args2>();

				Assert.IsNotNull( container.Resolve<Args2>( "a", "b" ) );
				Assert.IsNotNull( container.Resolve( typeof( Args2 ), "a", "b" ) );
			}
		}

		[Test]
		public void RegisterAndResolve3Args()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Args3>().Arguments( Argument.UserProvided( "arg1" ), Argument.UserProvided( "arg2" ) );
				container.Configure<Args3>().Arguments( Argument.UserProvided( "arg3" ) );

				container.Register.Type<Args3>();

				Assert.IsNotNull( container.Resolve<Args3>( "a", "b", "c" ) );
				Assert.IsNotNull( container.Resolve( typeof( Args3 ), "a", "b", "c" ) );
			}
		}

		[Test]
		public void RegisterAndResolve4Args()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Args4>().Arguments( Argument.UserProvided( "arg1" ), Argument.UserProvided( "arg2" ) );
				container.Configure<Args4>().Arguments( Argument.UserProvided( "arg3" ), Argument.UserProvided( "arg4" ) );

				container.Register.Type<Args4>();

				Assert.IsNotNull( container.Resolve<Args4>( "a", "b", "c", "d" ) );
				Assert.IsNotNull( container.Resolve( typeof( Args4 ), "a", "b", "c", "d" ) );
			}
		}

		[Test]
		public void RegisterAndResolve8Args()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Args8>().Arguments( Argument.UserProvided( "arg1" ), Argument.UserProvided( "arg2" ) );
				container.Configure<Args8>().Arguments( Argument.UserProvided( "arg3" ), Argument.UserProvided( "arg4" ) );
				container.Configure<Args8>().Arguments( Argument.UserProvided( "arg5" ), Argument.UserProvided( "arg6" ) );
				container.Configure<Args8>().Arguments( Argument.UserProvided( "arg7" ), Argument.UserProvided( "arg8" ) );

				container.Register.Type<Args8>();

				Assert.IsNotNull( container.Resolve<Args8>( "a", "b", "c", "d", "e", "f", "g", "h" ) );
				Assert.IsNotNull( container.Resolve( typeof( Args8 ), "a", "b", "c", "d", "e", "f", "g", "h" ) );
			}
		}

		[Test]
		public void ResolveMultipleImplementedInterface()
		{
			using( var container = new Container( true ) )
			{
				// Configure the argument's type
				container.Configure<Foo>().Arguments( Argument.UseType<BarDerived>( "bar" ) );

				// Registere some possbile variants
				container.Register.Type<Foo>();
				container.Register.Instance( new Bar() );
				container.Register.Type<BarDerived>().WithScope( Scope.Singleton );
				container.Register.Type<BarDerived2>().WithScope( Scope.Singleton );

				// Can be resolved because of the configuration
				var foo = container.Resolve<Foo>();
				var bar = container.Resolve<BarDerived>();
				Assert.AreEqual( bar, foo.Bar );
			}
		}

		[Test]
		[ExpectedException( typeof( RegisterException ) )]
		public void SameContructor()
		{
			using( var container = new Container( true ) )
			{
				// Problem is automatic injection ... no argument is given by the user
				// So which constructor should be used? Throw an error
				container.Register.Type<DuplicatedConstructor>();
			}
		}

		[Test]
		public void UseInjectedArgument()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<Foo>();
				container.Register.Type<InjectedArgument>();

				Assert.IsNotNull( container.Resolve<InjectedArgument>() );
			}
		}
	}
}
