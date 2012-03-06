using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class RegisterByNameTests
	{
		#region Test objects: Args1, Args2, Args3, Args4
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable FieldCanBeMadeReadOnly.Local
		// ReSharper disable MemberCanBePrivate.Local
		// ReSharper disable RedundantDefaultFieldInitializer
		// ReSharper disable UnusedAutoPropertyAccessor.Local
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local

		private interface IBar {}

		private class Bar : IBar {}

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
			public string Arg1 { get; set; }
			public string Arg2 { get; set; }
			public string Arg3 { get; set; }
			public string Arg4 { get; set; }

			public Args4( string arg1, string arg2, string arg3, string arg4 )
			{
				Arg1 = arg1;
				Arg2 = arg2;
				Arg3 = arg3;
				Arg4 = arg4;
			}
		}

		private sealed class AutoInjectConstructor
		{
			public AutoInjectConstructor( IBar barInjected, IBar barNotInjected )
			{
				BarInjected = barInjected;
				BarNotInjected = barNotInjected;
			}

			public IBar BarInjected { get; private set; }

			public IBar BarNotInjected { get; private set; }
		}

		private sealed class AutoInjectNamedField
		{
			public IBar BarFieldInjected = null;
		}

		private sealed class AutoInjectNamedProperty
		{
			public IBar BarPropertyInjected { get; set; }
		}

		private sealed class AutoInjectNamedMethod
		{
			public IBar BarInjected { get; private set; }

			public void Inject( IBar bar )
			{
				BarInjected = bar;
			}
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore FieldCanBeMadeReadOnly.Local
		// ReSharper restore MemberCanBePrivate.Local
		// ReSharper restore RedundantDefaultFieldInitializer
		// ReSharper restore UnusedAutoPropertyAccessor.Local
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
		#endregion

		[Test]
		public void RegisterByName()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>( "bar1" ).WithScope( Scope.Singleton );
				container.Register.Type<Bar>( "bar2" );

				var bar1A = container.ResolveByName<IBar>( "bar1" );
				var bar1B = container.ResolveByName<IBar>( "bar1" );
				var bar2A = container.ResolveByName<IBar>( "bar2" );
				var bar2B = container.ResolveByName<IBar>( "bar2" );

				Assert.IsNotNull( bar1A );
				Assert.IsNotNull( bar1B );
				Assert.AreEqual( bar1A, bar1B );

				Assert.IsNotNull( bar2A );
				Assert.IsNotNull( bar2B );
				Assert.AreNotEqual( bar2A, bar2B );

				Assert.AreNotEqual( bar1A, bar2A );
			}
		}

		[Test]
		public void RegisterAndResolve1ArgWithName()
		{
			using( var container = new Container() )
			{
				container.Configure<Args1>( "name" ).Argument( Argument.UserProvided( "arg" ) );

				container.Register.Type<Args1>( "name" );

				Assert.IsNotNull( container.ResolveByName<Args1>( "name", "a" ) );
				Assert.IsNotNull( container.ResolveByName( "name", typeof( Args1 ), "a" ) );
			}
		}

		[Test]
		public void RegisterAndResolve2ArgsWithName()
		{
			using( var container = new Container() )
			{
				container.Configure<Args2>( "name" ).Argument( Argument.UserProvided( "arg1" ) );
				container.Configure<Args2>( "name" ).Argument( Argument.UserProvided( "arg2" ) );

				container.Register.Type<Args2>( "name" );

				Assert.IsNotNull( container.ResolveByName<Args2>( "name", "a", "b" ) );
				Assert.IsNotNull( container.ResolveByName( "name", typeof( Args2 ), "a", "b" ) );
			}
		}

		[Test]
		public void RegisterAndResolve3ArgsWithName()
		{
			using( var container = new Container() )
			{
				container.Configure<Args3>( "name" ).Argument( Argument.UserProvided( "arg1" ) );
				container.Configure<Args3>( "name" ).Argument( Argument.UserProvided( "arg2" ) );
				container.Configure<Args3>( "name" ).Argument( Argument.UserProvided( "arg3" ) );

				container.Register.Type<Args3>( "name" );

				Assert.IsNotNull( container.ResolveByName<Args3>( "name", "a", "b", "c" ) );
				Assert.IsNotNull( container.ResolveByName( "name", typeof( Args3 ), "a", "b", "c" ) );
			}
		}

		[Test]
		public void RegisterAndResolve4ArgsWithName()
		{
			using( var container = new Container() )
			{
				container.Configure<Args4>( "name" ).Argument( Argument.UserProvided( "arg1" ) );
				container.Configure<Args4>( "name" ).Argument( Argument.UserProvided( "arg2" ) );
				container.Configure<Args4>( "name" ).Argument( Argument.UserProvided( "arg3" ) );
				container.Configure<Args4>( "name" ).Argument( Argument.UserProvided( "arg4" ) );

				container.Register.Type<Args4>( "name" );

				Assert.IsNotNull( container.ResolveByName<Args4>( "name", "a", "b", "c", "d" ) );
				Assert.IsNotNull( container.ResolveByName( "name", typeof( Args4 ), "a", "b", "c", "d" ) );
			}
		}

		[Test]
		public void InjectNamedConstructor()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<AutoInjectConstructor>().Argument( Argument.UserProvided( "barNotInjected" ) );
				container.Configure<AutoInjectConstructor>().Argument( Argument.Named( "barInjected", "bar" ) );

				var bar = new Bar();
				container.Register.Instance( "bar", bar );
				container.Register.Type<AutoInjectConstructor>();

				var autoInject = container.Resolve<AutoInjectConstructor>( NullArg.New<IBar>() );

				Assert.IsNotNull( autoInject );
				Assert.IsNotNull( autoInject.BarInjected );
				Assert.IsNull( autoInject.BarNotInjected );
				Assert.AreEqual( autoInject.BarInjected, bar );
			}
		}

		[Test]
		public void InjectNamedField()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<AutoInjectNamedField>().InjectField( "BarFieldInjected", "bar" );

				container.Register.Type<Bar>( "bar" ).WithScope( Scope.Singleton );
				container.Register.Type<AutoInjectNamedField>();

				var autoInject = container.Resolve<AutoInjectNamedField>();

				Assert.IsNotNull( autoInject.BarFieldInjected );
				Assert.AreEqual( autoInject.BarFieldInjected, container.ResolveByName<IBar>( "bar" ) );
			}
		}

		[Test]
		public void InjectNamedProperty()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectNamedProperty>().InjectProperty( "BarPropertyInjected", "bar" );

				container.Register.Type<Bar>( "bar" ).WithScope( Scope.Singleton );
				container.Register.Type<AutoInjectNamedProperty>();

				var property = container.Resolve<AutoInjectNamedProperty>();

				Assert.IsNotNull( property.BarPropertyInjected );
				Assert.AreEqual( property.BarPropertyInjected, container.ResolveByName<IBar>( "bar" ) );
			}
		}

		[Test]
		public void InjectNamedMethod()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectNamedMethod>().InjectMethod( "Inject" ).Argument( Argument.Named( "bar", "bar" ) );

				var bar = new Bar();
				container.Register.Instance( "bar", bar );
				container.Register.Type<AutoInjectNamedMethod>();

				var autoInjectNamedMethod = container.Resolve<AutoInjectNamedMethod>();

				Assert.IsNotNull( autoInjectNamedMethod );
				Assert.IsNotNull( autoInjectNamedMethod.BarInjected );
				Assert.AreEqual( autoInjectNamedMethod.BarInjected, bar );
			}
		}
	}
}
