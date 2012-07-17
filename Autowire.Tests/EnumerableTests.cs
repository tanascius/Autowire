using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests
{
	[TestFixture]
	public class EnumerableTests
	{
		#region Testobjects: IBar, Bar, BarDerived, BarDerived2, IFoo, Foo
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable MemberHidesStaticFromOuterClass

		internal interface IBar {}

		internal class Bar : IBar {}

		internal sealed class BarDerived : Bar {}

		internal sealed class BarDerived2 : Bar {}

		internal interface IFoo
		{
			IBar Bar { get; }
		}

		internal sealed class Foo : IFoo
		{
			public Foo( IBar bar )
			{
				Bar = bar;
			}

			public IBar Bar { get; private set; }
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore MemberHidesStaticFromOuterClass
		#endregion

		#region Helper classes: ResolvePropertyCollection, ResolveCtorCollection, ResolverOfIEnumerableAsConstructor, ResolverOfIEnumerableAsProperty
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable RedundantDefaultFieldInitializer
		// ReSharper disable UnusedAutoPropertyAccessor.Local

		private class ResolvePropertyCollection
		{
			public IEnumerable<IBar> BarsProperty { get; private set; }
			public readonly IEnumerable<IBar> BarsField = null;
		}

		private class ResolveCtorCollection
		{
			public ResolveCtorCollection( IEnumerable<IBar> bars )
			{
				Bars = bars;
			}

			public IEnumerable<IBar> Bars { get; private set; }
		}

		private class ResolverOfIEnumerableAsConstructor
		{
			public ResolverOfIEnumerableAsConstructor( Resolver<IEnumerable<IBar>> resolver )
			{
				Resolver = resolver;
			}

			public Resolver<IEnumerable<IBar>> Resolver { get; private set; }
		}

		private class ResolverOfIEnumerableAsProperty
		{
			public Resolver<IEnumerable<IBar>> Resolver { get; private set; }
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore RedundantDefaultFieldInitializer
		// ReSharper restore UnusedAutoPropertyAccessor.Local
		#endregion

		[Test]
		public void GetNormalInstanceAsEnumberable()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>().WithScope( Scope.Singleton );

				var bar = container.Resolve<Bar>();
				Assert.That( bar, Is.Not.Null );

				var bars = container.ResolveAll<Bar>();
				Assert.That( bars, Is.Not.Null );
				Assert.That( bars.Count, Is.EqualTo( 1 ) );
				Assert.That( bars[0], Is.EqualTo( bar ) );
			}
		}

		[Test]
		public void GetAllIBars()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>();
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();

				var bars = container.ResolveAll<IBar>();
				Assert.That( bars, Is.Not.Null );
				Assert.That( bars.Count, Is.EqualTo( 3 ) );
			}
		}

		[Test]
		public void GetIEnumerableOfIBars()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>();
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();

				var bars = container.Resolve<IEnumerable<IBar>>();
				Assert.That( bars, Is.Not.Null );
				Assert.That( bars.Count(), Is.EqualTo( 3 ) );
			}
		}

		[Test]
		public void GetConstructorResolver()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>();
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();
				container.Register.Type<ResolverOfIEnumerableAsConstructor>();

				var resolver = container.Resolve<ResolverOfIEnumerableAsConstructor>();
				Assert.That( resolver, Is.Not.Null );

				var bars = resolver.Resolver.Resolve();
				Assert.That( bars.Count(), Is.EqualTo( 3 ) );
				Assert.That( bars, Is.All.Not.Null );
			}
		}

		[Test]
		public void GetPropertyResolver()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<ResolverOfIEnumerableAsProperty>().InjectProperty( "Resolver" );

				container.Register.Type<Bar>();
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();
				container.Register.Type<ResolverOfIEnumerableAsProperty>();

				var resolver = container.Resolve<ResolverOfIEnumerableAsProperty>();
				Assert.That( resolver, Is.Not.Null );

				var bars = resolver.Resolver.Resolve();
				Assert.That( bars.Count(), Is.EqualTo( 3 ) );
				Assert.That( bars, Is.All.Not.Null );
			}
		}

		[Test]
		public void EnumerableConstructorIsInjected()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>();
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();
				container.Register.Type<ResolveCtorCollection>();

				var resolveEnumTestClass = container.Resolve<ResolveCtorCollection>();
				Assert.That( resolveEnumTestClass, Is.Not.Null );
				Assert.That( resolveEnumTestClass.Bars.Count(), Is.EqualTo( 3 ) );
				Assert.That( resolveEnumTestClass.Bars, Is.All.Not.Null );
			}
		}

		[Test]
		public void EnumerablePropertyAndFieldIsInjected()
		{
			using( var container = new Container() )
			{
				container.Configure<ResolvePropertyCollection>().InjectProperty( "BarsProperty" );
				container.Configure<ResolvePropertyCollection>().InjectField( "BarsField" );

				container.Register.Type<Bar>();
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();
				container.Register.Type<ResolvePropertyCollection>();

				var resolveEnumTestClass = container.Resolve<ResolvePropertyCollection>();
				Assert.That( resolveEnumTestClass, Is.Not.Null );
				Assert.That( resolveEnumTestClass.BarsProperty.Count(), Is.EqualTo( 3 ) );
				Assert.That( resolveEnumTestClass.BarsProperty, Is.All.Not.Null );
				Assert.That( resolveEnumTestClass.BarsField.Count(), Is.EqualTo( 3 ) );
				Assert.That( resolveEnumTestClass.BarsField, Is.All.Not.Null );
			}
		}
	}
}
