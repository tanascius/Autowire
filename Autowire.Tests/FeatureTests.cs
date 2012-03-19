using System;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class FeatureTests
	{
		#region Testobjects: IBar, Bar, IFoo, Foo, InjectCallback, AutoInjectConstructor, Disposable, MethodInjectedLast
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable MemberHidesStaticFromOuterClass
		// ReSharper disable UnusedAutoPropertyAccessor.Local
		// ReSharper disable UnusedMember.Local

		private interface IBar {}

		private class Bar : IBar {}

		private interface IFoo
		{
			IBar Bar { get; set; }
		}

		private sealed class Foo : IFoo
		{
			public Foo( IBar bar )
			{
				Bar = bar;
			}

			public IBar Bar { get; set; }
		}

		private sealed class InjectCallback
		{
			public int Number { get; set; }
		}

		private sealed class AutoInjectConstructor
		{
			public AutoInjectConstructor( IBar barInjected )
			{
				BarInjected = barInjected;
			}

			public IBar BarInjected { get; private set; }
		}

		private sealed class Disposable : IDisposable
		{
			public bool IsDisposed { get; private set; }

			public void Dispose()
			{
				IsDisposed = true;
			}
		}

		private sealed class MethodInjectedLast
		{
			private void Init()
			{
				Assert.IsTrue( InjectMe );
				InitWasCalled = true;
			}

			private bool InjectMe { get; set; }

			public bool InitWasCalled { get; private set; }
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore MemberHidesStaticFromOuterClass
		// ReSharper restore UnusedAutoPropertyAccessor.Local
		// ReSharper restore UnusedMember.Local
		#endregion

		[Test]
		public void DisposeHasToBeCalled()
		{
			Disposable disposable;

			using( var container = new Container() )
			{
				container.Register.Type<Disposable>();
				disposable = container.Resolve<Disposable>();
				Assert.IsNotNull( disposable );
			}

			Assert.IsTrue( disposable.IsDisposed );
		}

		[Test]
		public void DoInitAfterResolve()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<InjectCallback>().AfterResolve( ( c, ic ) => ( ( InjectCallback ) ic ).Number = 42 );
				container.Register.Type<InjectCallback>();

				var injectCallback = container.Resolve<InjectCallback>();
				Assert.AreEqual( 42, injectCallback.Number );
			}
		}

		[Test]
		[Description( "Methods will be injected after properties and fields -> method injection can be used for init methods" )]
		public void MethodInjectionCanBeUsedAsInitMethod()
		{
			using( var container = new Container() )
			{
				container.Configure<MethodInjectedLast>().InjectProperty( "InjectMe" );
				container.Configure<MethodInjectedLast>().InjectMethod( "Init" );

				const bool thisIsTrue = true;
				container.Register.Instance( thisIsTrue );
				container.Register.Type<MethodInjectedLast>();

				var afterInjection = container.Resolve<MethodInjectedLast>();
				Assert.IsNotNull( afterInjection );
				Assert.IsTrue( afterInjection.InitWasCalled );
			}
		}

		[Test]
		public void CreateChildContainer()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>().WithScope( Scope.Singleton );

				using( var childContainer = container.CreateChild() )
				{
					childContainer.Configure<Foo>().Arguments( Argument.Create( "bar", new Bar() ) );

					childContainer.Register.Type<Foo>().WithScope( Scope.Singleton );

					Assert.IsNotNull( childContainer.Resolve<IFoo>() );
					Assert.IsNull( container.Resolve<IFoo>() );
				}
			}
		}

		[Test, Description( "Container registeres A, creates a child container, which registriers B (that needs A injected)" )]
		public void UseChildContainerAndParentInjection()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>( "bar" ).WithScope( Scope.Singleton );

				using( var childContainer = container.CreateChild() )
				{
					childContainer.Configure<AutoInjectConstructor>().Arguments( Argument.Named( "barInjected", "bar" ) );

					childContainer.Register.Type<AutoInjectConstructor>().WithScope( Scope.Singleton );

					Assert.IsNotNull( childContainer.ResolveByName<IBar>( "bar" ) );
					Assert.IsNotNull( childContainer.Resolve<AutoInjectConstructor>() );
					Assert.IsNull( container.Resolve<AutoInjectConstructor>() );
				}
			}
		}

		[Test, Description( "Container registeres A (that needs B injected), creates a child container, which registriers B" )]
		public void UseChildContainerAndChildInjection()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectConstructor>().Arguments( Argument.Named( "barInjected", "bar" ) );

				container.Register.Type<AutoInjectConstructor>();

				using( var childContainer = container.CreateChild() )
				{
					childContainer.Register.Type<Bar>( "bar" );

					var autoInjectConstructor = childContainer.Resolve<AutoInjectConstructor>();
					Assert.IsNotNull( autoInjectConstructor );
					Assert.IsNotNull( autoInjectConstructor.BarInjected );
				}
			}
		}
	}
}
