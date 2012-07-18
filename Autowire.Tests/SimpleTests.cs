using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class SimpleTests
	{
		#region Testobjects: IBar, Bar, BarDerived, BarDerived2, AbstractClass, PrivateConstructor
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable ConvertToStaticClass

		private interface IBar {}

		private class Bar : IBar {}

		private sealed class BarDerived : Bar { }

		private sealed class BarDerived2 : Bar { }

		private abstract class AbstractClass {}

		private sealed class PrivateConstructor
		{
			private PrivateConstructor() {}
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore ConvertToStaticClass
		#endregion

		[Test]
		public void RegisterAndResolveByClass()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				var bar = container.Resolve<Bar>();

				Assert.IsNotNull( bar );
			}
		}

		[Test]
		public void RegisterAndResolveByInterface()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				var bar = container.Resolve<IBar>();

				Assert.IsNotNull( bar );
			}
		}

		[Test]
		[ExpectedException( typeof( RegisterException ) )]
		public void CannotRegisterInterface()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<IBar>();
			}
		}

		[Test]
		[ExpectedException( typeof( RegisterException ) )]
		public void CannotRegisterAbstractClass()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<AbstractClass>();
			}
		}

		[Test]
		public void RegisterAndResolveDerivedType()
		{
			using( var container = new Container( true ) )
			{
				// At first the base type
				container.Register.Type<Bar>();

				// Now the derived type, which will implicitly register the basetype again
				container.Register.Type<BarDerived>();

				// Should be no problem althought we have two possible resolves, now
				// That is, because the basetype was registered at least once explicitly
				var bar = container.Resolve<Bar>();

				Assert.IsNotNull( bar );
			}
		}

		[Test]
		[ExpectedException( typeof( ResolveException ) )]
		public void RegisterAndResolveDerivedTypeFails()
		{
			using( var container = new Container( true ) )
			{
				// At first the derived type - which will implicitly register the basetype, too
				container.Register.Type<BarDerived>();
				container.Register.Type<BarDerived2>();

				// Should be a problem, because we have two possible resolves
				container.Resolve<Bar>();
			}
		}

		[Test]
		public void RegisterAndResolveDerivedTypeAndInstance()
		{
			using( var container = new Container( true ) )
			{
				// At first register the derived type
				container.Register.Type<BarDerived>();

				// Resolve the base type
				container.Resolve<Bar>();

				// Now the basetype itself ...
				var bar = new Bar();
				container.Register.Instance( bar );

				// Should be no problem, though
				var barResolved = container.Resolve<Bar>();

				Assert.IsNotNull( barResolved );
				Assert.AreEqual( bar, barResolved );
			}
		}

		[Test]
		public void RegisterByGivenType()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type( typeof( Bar ) );
				var bar = container.Resolve<IBar>();

				Assert.IsNotNull( bar );
			}
		}

		[Test]
		public void ResolveUnknownType()
		{
			using( var container = new Container() )
			{
				var instance = container.Resolve<IBar>();

				Assert.IsNull( instance );
			}
		}

		[Test]
		[ExpectedException( typeof( ResolveException ) )]
		public void ResolveUnknownTypeWithException()
		{
			using( var container = new Container( true ) )
			{
				container.Resolve<IBar>();
			}
		}

		[Test]
		public void ResolveByGivenType()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				var bar = container.Resolve( typeof( IBar ) );

				Assert.IsNotNull( bar );
			}
		}

		[Test]
		public void RegisterAndResolveByGivenType()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type( typeof( Bar ) );
				var bar = container.Resolve( typeof( IBar ) );

				Assert.IsNotNull( bar );
			}
		}

		[Test]
		public void ResolvePrivateConstructor()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<PrivateConstructor>();
				var privateConstructor = container.Resolve<PrivateConstructor>();

				Assert.IsNotNull( privateConstructor );
			}
		}
	}
}
