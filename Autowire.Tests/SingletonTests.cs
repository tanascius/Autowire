using System.Threading;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class SingletonTests
	{
		#region Testobjects: IBar, Bar, IFoo, Foo
		private interface IBar {}

		private class Bar : IBar {}

		private interface IFoo {}

		private sealed class Foo : IFoo
		{
			public Foo( IBar bar ) {}
		}
		#endregion

		[Test]
		public void ConfigureSingleton()
		{
			using( var container = new Container() )
			{
				container.Configure<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<Bar>();

				var bar1 = container.Resolve<Bar>();
				var bar2 = container.Resolve<Bar>();

				Assert.AreEqual( bar1, bar2 );
			}
		}

		[Test]
		public void RegisterSingleton()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>().WithScope( Scope.Singleton );

				var bar1 = container.Resolve<Bar>();
				var bar2 = container.Resolve<Bar>();

				Assert.AreEqual( bar1, bar2 );
			}
		}

		[Test]
		public void InitializeSingletonAndUseGivenInstance()
		{
			using( var container = new Container() )
			{
				var bar = new Bar();
				container.Register.Instance( bar );

				var bar1 = container.Resolve<IBar>();
				var bar2 = container.Resolve<Bar>();

				Assert.AreEqual( bar, bar1 );
				Assert.AreEqual( bar1, bar2 );
			}
		}

		[Test]
		public void InitializeSingletonAndUseGivenInstanceWithParameters()
		{
			using( var container = new Container() )
			{
				var foo = new Foo( new Bar() );
				container.Register.Instance( foo );

				var resolvedFoo = container.Resolve<Foo>();

				Assert.AreEqual( foo, resolvedFoo );
			}
		}

		[Test]
		public void InitializeSingletonWithParameters()
		{
			using( var container = new Container() )
			{
				container.Configure<Foo>().Argument( Argument.UserProvided( "bar" ) );

				container.Register.Type<Foo>().WithScope( Scope.Singleton );

				container.Resolve<IFoo>( new Bar() );
				var resolvedFoo = container.Resolve<IFoo>();

				Assert.IsNotNull( resolvedFoo );
			}
		}

		[Test]
		public void InitializeSingletonWithDifferentCtors()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Foo>().WithScope( Scope.Singleton );

				var bar1 = container.Resolve<Foo>( new Bar() );
				var bar2 = container.Resolve<Foo>( "" );
				var bar3 = container.Resolve<Foo>();

				Assert.AreEqual( bar1, bar2 );
				Assert.AreEqual( bar2, bar3 );
			}
		}

		[Test]
		public void SameSingletonForEveryThread()
		{
			using( var container = new Container() )
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
			using( var container = new Container() )
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
