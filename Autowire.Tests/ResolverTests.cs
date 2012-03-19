using System.Collections.Generic;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class ResolverTests
	{
		#region Test objects: IBar, Bar, BarDerived, BarDerived2, IFoo, Foo
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable MemberHidesStaticFromOuterClass
		// ReSharper disable UnusedMember.Local

		private interface IBar {}

		private class Bar : IBar {}

		private sealed class BarDerived : Bar {}

		private sealed class BarDerived2 : Bar {}

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

		private class SimpleResolverTestClass
		{
			private readonly Resolver<IBar> m_BarFactory;

			public SimpleResolverTestClass( Resolver<IBar> barFactory )
			{
				m_BarFactory = barFactory;
			}

			public IBar CreateBar()
			{
				return m_BarFactory.Resolve();
			}
		}

		private class AnotherSimpleResolverTestClass
		{
			private readonly Resolver<IBar> m_BarFactory;

			public AnotherSimpleResolverTestClass( Resolver<IBar> barFactory )
			{
				m_BarFactory = barFactory;
			}

			public IBar CreateBar()
			{
				return m_BarFactory.Resolve();
			}
		}

		private class NamedResolverTestClass
		{
			private readonly Resolver<IBar> m_BarFactory;

			public NamedResolverTestClass( Resolver<IBar> barFactory )
			{
				m_BarFactory = barFactory;
			}

			public IBar CreateBar()
			{
				return m_BarFactory.ResolveByName( "at" );
			}
		}

		private class ParametrizedResolverTestClass
		{
			private readonly Resolver<IFoo> m_FooFactory;

			public ParametrizedResolverTestClass( Resolver<IFoo> fooFactory )
			{
				m_FooFactory = fooFactory;
			}

			public IFoo CreateFoo( IBar bar )
			{
				return m_FooFactory.Resolve( bar );
			}
		}

		private class ResolveAllTestClass
		{
			public ResolveAllTestClass( Resolver<IBar> barFactory )
			{
				Bars = barFactory.ResolveAll();
			}

			public IList<IBar> Bars { get; private set; }
		}

		// ReSharper restore MemberHidesStaticFromOuterClass
		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore UnusedMember.Local
		#endregion

		[Test]
		public void SimpleResolver()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>();
				container.Register.Type<SimpleResolverTestClass>();

				var factoryTestClass = container.Resolve<SimpleResolverTestClass>();
				Assert.IsNotNull( factoryTestClass );

				var bar = factoryTestClass.CreateBar();
				Assert.IsNotNull( bar );
				Assert.IsInstanceOfType( typeof( IBar ), bar );
			}
		}

		[Test]
		public void UseSameResolverTwice()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>();
				container.Register.Type<SimpleResolverTestClass>();
				container.Register.Type<AnotherSimpleResolverTestClass>();

				var factoryTestClass = container.Resolve<SimpleResolverTestClass>();
				Assert.IsNotNull( factoryTestClass );

				var bar = factoryTestClass.CreateBar();
				Assert.IsNotNull( bar );
				Assert.IsInstanceOfType( typeof( IBar ), bar );
			}
		}

		[Test]
		public void NamedResolver()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>( "at" );
				container.Register.Type<NamedResolverTestClass>( "at" );

				var factoryTestClass = container.ResolveByName<NamedResolverTestClass>( "at" );
				Assert.IsNotNull( factoryTestClass );

				var bar = factoryTestClass.CreateBar();
				Assert.IsNotNull( bar );
				Assert.IsInstanceOfType( typeof( IBar ), bar );
			}
		}

		[Test]
		public void ResolveFactoryWithArguments()
		{
			using( var container = new Container() )
			{
				container.Configure<Foo>().Arguments( Argument.UserProvided( "bar" ) );

				container.Register.Type<Bar>();
				container.Register.Type<Foo>();
				container.Register.Type<ParametrizedResolverTestClass>();

				var factoryTestClass = container.Resolve<ParametrizedResolverTestClass>();
				Assert.IsNotNull( factoryTestClass );

				var foo = factoryTestClass.CreateFoo( new Bar() );
				Assert.IsNotNull( foo );
				Assert.IsInstanceOfType( typeof( IFoo ), foo );
			}
		}

		[Test]
		public void ResolveAllTest()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>();
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();
				container.Register.Type<ResolveAllTestClass>();

				var hasBars = container.Resolve<ResolveAllTestClass>();
				Assert.IsNotNull( hasBars );

				Assert.GreaterOrEqual( hasBars.Bars.Count, 3 );
			}
		}
	}
}
