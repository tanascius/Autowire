using System.Collections.Generic;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class EnumerableTests
	{
		#region Testobjects: IBar, Bar, BarDerived, BarDerived2, IFoo, Foo
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable MemberHidesStaticFromOuterClass

		internal interface IBar { }

		internal class Bar : IBar { }

		internal sealed class BarDerived : Bar { }

		internal sealed class BarDerived2 : Bar { }

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
		// ReSharper disable UnusedAutoPropertyAccessor.Local

		private class ResolvePropertyCollection
		{
			public IEnumerable<IBar> BarsProperty { get; private set; }
			public IEnumerable<IBar> BarsField;
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
		// ReSharper restore UnusedAutoPropertyAccessor.Local
		#endregion

		[Test]
		public void GetNormalInstanceAsEnumberable()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>().WithScope( Scope.Singleton );

				var bar = container.Resolve<Bar>();
				Assert.IsNotNull( bar );

				var bars = container.ResolveAll<Bar>();
				Assert.IsNotNull( bars );
				Assert.AreEqual( 1, bars.Count );
				Assert.AreEqual( bar, bars[0] );
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
				Assert.IsNotNull( bars );
				Assert.AreEqual( 3, bars.Count );
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
				Assert.IsNotNull( bars );

				var count = 0;
				foreach( var bar in bars )
				{
					Assert.IsNotNull( bar );
					count++;
				}
				Assert.AreEqual( 3, count );
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
				Assert.IsNotNull( resolver );

				var bars = resolver.Resolver.Resolve();
				var count = 0;
				foreach( var bar in bars )
				{
					Assert.IsNotNull( bar );
					count++;
				}
				Assert.AreEqual( 3, count );
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
				Assert.IsNotNull( resolver );

				var bars = resolver.Resolver.Resolve();
				var count = 0;
				foreach( var bar in bars )
				{
					Assert.IsNotNull( bar );
					count++;
				}
				Assert.AreEqual( 3, count );
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
				Assert.IsNotNull( resolveEnumTestClass );

				var count = 0;
				foreach( var bar in resolveEnumTestClass.Bars )
				{
					Assert.IsNotNull( bar );
					count++;
				}
				Assert.AreEqual( 3, count );
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
				Assert.IsNotNull( resolveEnumTestClass );

				var count = 0;
				foreach( var bar in resolveEnumTestClass.BarsProperty )
				{
					Assert.IsNotNull( bar );
					count++;
				}
				Assert.AreEqual( 3, count );

				count = 0;
				foreach( var bar in resolveEnumTestClass.BarsField )
				{
					Assert.IsNotNull( bar );
					count++;
				}
				Assert.AreEqual( 3, count );
			}
		}
	}
}
