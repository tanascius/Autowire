using System;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class FeatureTests
	{
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
			using( var container = new Container() )
			{
				container.Configure<IFoo>().Argument( Argument.Create( "bar", new Bar() ) );
				container.Configure<IFoo>().AfterResolve( ( c, foo ) => ( ( IFoo ) foo ).Text = "bleh" );

				container.Register.Type<Foo>();

				var fooResolved = container.Resolve<Foo>();

				Assert.AreEqual( "bleh", fooResolved.Text );
			}
		}

		[Test]
		[Description( "Methods will be injected after properties and fields -> method injection can be used for init methods" )]
		public void MethodInjectionCanBeUsedAsInitMethod()
		{
			using( var container = new Container() )
			{
				container.Configure<AfterInjection>().InjectProperty( "InjectMe" );
				container.Configure<AfterInjection>().InjectMethod( "Init" );

				const bool thisIsTrue = true;
				container.Register.Instance( thisIsTrue );
				container.Register.Type<AfterInjection>();

				var afterInjection = container.Resolve<AfterInjection>();
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
					childContainer.Configure<Foo>().Argument( Argument.Create( "bar", new Bar() ) );

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
					childContainer.Configure<AutoInjectConstructor>().Argument( Argument.Create( "barNotInjected", new Bar() ) );
					childContainer.Configure<AutoInjectConstructor>().Argument( Argument.Named( "barInjected", "bar" ) );

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
				container.Configure<AutoInjectConstructor>().Argument( Argument.Create( "barNotInjected", NullArg.New<Bar>() ) );
				container.Configure<AutoInjectConstructor>().Argument( Argument.Named( "barInjected", "bar" ) );

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

	internal sealed class Disposable : IDisposable
	{
		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			IsDisposed = true;
		}
	}

	internal sealed class AfterInjection
	{
		private void Init()
		{
			Assert.IsTrue( InjectMe );
			InitWasCalled = true;
		}

		private bool InjectMe { get; set; }

		public bool InitWasCalled { get; private set; }
	}
}
