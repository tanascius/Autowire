using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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

			public IBar CreateBar( string name )
			{
				return m_BarFactory.ResolveByName( name );
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
			public ResolveAllTestClass( IEnumerable<IBar> barFactory )
			{
				Bars = barFactory.ToArray();
			}

			public IList<IBar> Bars { get; private set; }
		}

		// ReSharper restore MemberHidesStaticFromOuterClass
		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore UnusedMember.Local
		#endregion

		[Test]
		[Description( "Resolve an object, that can create a bar at runtime dynamically" )]
		public void SimpleResolver()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				container.Register.Type<SimpleResolverTestClass>();

				var factoryTestClass = container.Resolve<SimpleResolverTestClass>();
				var bar = factoryTestClass.CreateBar();

				Assert.That( bar, Is.InstanceOfType( typeof( IBar ) ) );
			}
		}

		[Test]
		[Description( "Resolve an object, that can create a bar at runtime dynamically. This time there are other registered types, that can resolve bars, too." )]
		public void UseSameResolverTwice()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				container.Register.Type<SimpleResolverTestClass>();
				container.Register.Type<AnotherSimpleResolverTestClass>();

				var factoryTestClass = container.Resolve<SimpleResolverTestClass>();
				var bar = factoryTestClass.CreateBar();

				Assert.That( bar, Is.InstanceOfType( typeof( IBar ) ) );
			}
		}

		[Test]
		[Description( "Resolve an object, that can create a named bar at runtime dynamically." )]
		public void NamedResolver()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<NamedResolverTestClass>();

				// Bar "a" is just for distraction
				container.Register.Instance( "a", new Bar() );

				// bBar will be resolved
				var bBar = new Bar();
				container.Register.Instance( "b", bBar );

				var factoryTestClass = container.Resolve<NamedResolverTestClass>();

				Assert.That( factoryTestClass.CreateBar( "b" ), Is.EqualTo( bBar ) );
			}
		}

		[Test]
		[Description( "Resolve an object, that can create foo at runtime dynamically while the neccessary bar parameter is provided during resolution." )]
		public void ResolveFactoryWithArguments()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Foo>().Arguments( Argument.UserProvided( "bar" ) );

				container.Register.Type<ParametrizedResolverTestClass>();
				container.Register.Type<Bar>();
				container.Register.Type<Foo>();

				var factoryTestClass = container.Resolve<ParametrizedResolverTestClass>();
				var bar = new Bar();
				var foo = factoryTestClass.CreateFoo( bar );

				Assert.That( foo.Bar, Is.EqualTo( bar ) );
			}
		}

		[Test]
		[Description( "Resolve an object, that can create a collection of all bars at runtime dynamically." )]
		public void ResolveAllTest()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();
				container.Register.Type<ResolveAllTestClass>();

				var hasBars = container.Resolve<ResolveAllTestClass>();

				Assert.That( hasBars.Bars.Count, Is.EqualTo( 3 ) );
			}
		}
	}
}
