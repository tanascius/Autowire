using System.Threading;
using Autowire.Registration;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests
{
	[TestFixture]
	public class SingletonTests
	{
		#region Testobjects: IBar, Bar, IFoo, Foo
		// ReSharper disable UnusedParameter.Local

		private interface IBar {}

		private class Bar : IBar {}

		private interface IFoo {}

		private sealed class Foo : IFoo
		{
			public Foo( IBar bar ) {}
		}

		// ReSharper restore UnusedParameter.Local
		#endregion

		[Test]
		[Description( "Configure a singleton. Register the type. Resolve it twice." )]
		public void ConfigureSingleton()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<Bar>();

				var bar1 = container.Resolve<Bar>();
				var bar2 = container.Resolve<Bar>();

				Assert.That( bar1, Is.EqualTo( bar2 ) );
			}
		}

		[Test]
		[Description( "Register a type as singleton. Resolve it twice." )]
		public void RegisterSingleton()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>().WithScope( Scope.Singleton );

				var bar1 = container.Resolve<Bar>();
				var bar2 = container.Resolve<Bar>();

				Assert.That( bar1, Is.EqualTo( bar2 ) );
			}
		}

		[Test]
		[Description( "Register an instance. Resolve it twice via interface and type." )]
		public void InitializeSingletonAndUseGivenInstance()
		{
			using( var container = new Container( true ) )
			{
				var bar = new Bar();
				container.Register.Instance( bar );

				var bar1 = container.Resolve<IBar>();
				var bar2 = container.Resolve<Bar>();

				Assert.That( bar1, Is.EqualTo( bar ) );
				Assert.That( bar2, Is.EqualTo( bar ) );
			}
		}

		[Test]
		[Description( "Ignore a type. Register an instance of it. Resolve it." )]
		public void RegisterIgnoredTypeAsSingleton()
		{
			using( var container = new Container( true ) )
			{
				// Configure type as ignored
				container.Configure<Bar>().Ignore();

				// Now register an instance
				var bar = new Bar();
				container.Register.Instance( bar );

				var resolvedBar = container.Resolve<IBar>();

				Assert.That( resolvedBar, Is.EqualTo( bar ) );
			}
		}

		[Test]
		[Description( "Register an instance with a argument. Resolve it without giving the argument." )]
		public void InitializeSingletonInstanceWithParameter()
		{
			using( var container = new Container( true ) )
			{
				var foo = new Foo( new Bar() );
				container.Register.Instance( foo );

				var resolvedFoo = container.Resolve<Foo>();

				Assert.That( resolvedFoo, Is.EqualTo( foo ) );
			}
		}

		[Test]
		[Description( "Register a singleton type with an user provided argument. Resolve it." )]
		public void InitializeSingletonWithParameter()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Foo>().Arguments( Argument.UserProvided( "bar" ) );

				container.Register.Type<Foo>().WithScope( Scope.Singleton );

				var firstResolve = container.Resolve<IFoo>( new Bar() );
				var secondResolve = container.Resolve<IFoo>( new Bar() );
				var thirdResolve = container.Resolve<IFoo>();

				Assert.That( secondResolve, Is.EqualTo( firstResolve ) );
				Assert.That( thirdResolve, Is.EqualTo( firstResolve ) );
			}
		}

		[Test]
		public void SameSingletonForEveryThread()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>().WithScope( Scope.Singleton );

				Bar bar1OfThread1 = null;
				Bar bar2OfThread1 = null;
				var thread1 = new Thread( delegate( object o )
				{
					bar1OfThread1 = container.Resolve<Bar>();
					bar2OfThread1 = container.Resolve<Bar>();
				} );
				thread1.Start();
				thread1.Join();
				Assert.IsNotNull( bar1OfThread1 );
				Assert.IsNotNull( bar2OfThread1 );
				Assert.AreEqual( bar1OfThread1, bar2OfThread1 );

				Bar bar1OfThread2 = null;
				Bar bar2OfThread2 = null;
				var thread2 = new Thread( delegate( object o )
				{
					bar1OfThread2 = container.Resolve<Bar>();
					bar2OfThread2 = container.Resolve<Bar>();
				} );
				thread2.Start();
				thread2.Join();
				Assert.IsNotNull( bar1OfThread2 );
				Assert.IsNotNull( bar2OfThread2 );
				Assert.AreEqual( bar1OfThread2, bar2OfThread2 );

				Assert.AreEqual( bar1OfThread1, bar1OfThread2 );
			}
		}

		[Test]
		public void CreateOneSingletonPerThread()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>().WithScope( Scope.SingletonPerThread );

				Bar bar1OfThread1 = null;
				Bar bar2OfThread1 = null;
				var thread1 = new Thread( delegate( object o )
				{
					bar1OfThread1 = container.Resolve<Bar>();
					bar2OfThread1 = container.Resolve<Bar>();
				} );
				thread1.Start();
				thread1.Join();
				Assert.IsNotNull( bar1OfThread1 );
				Assert.IsNotNull( bar2OfThread1 );
				Assert.AreEqual( bar1OfThread1, bar2OfThread1 );

				Bar bar1OfThread2 = null;
				Bar bar2OfThread2 = null;
				var thread2 = new Thread( delegate( object o )
				{
					bar1OfThread2 = container.Resolve<Bar>();
					bar2OfThread2 = container.Resolve<Bar>();
				} );
				thread2.Start();
				thread2.Join();
				Assert.IsNotNull( bar1OfThread2 );
				Assert.IsNotNull( bar2OfThread2 );
				Assert.AreEqual( bar1OfThread2, bar2OfThread2 );

				Assert.AreNotEqual( bar1OfThread1, bar1OfThread2 );
			}
		}
	}
}
