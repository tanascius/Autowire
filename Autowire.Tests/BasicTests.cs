using Autowire.Registration;
using Autowire.Resolving;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests
{
	[TestFixture]
	public class BasicTests
	{
		#region Testobjects: IBar, Bar, BarDerived, BarDerived2, AbstractClass, PrivateConstructor
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable ConvertToStaticClass

		private interface IBar {}

		private class Bar : IBar {}

		private sealed class BarDerived : Bar {}

		private sealed class BarDerived2 : Bar {}

		private abstract class AbstractClass {}

		private sealed class PrivateConstructor
		{
			private PrivateConstructor() {}
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore ConvertToStaticClass
		#endregion

		[Test]
		[Description( "Register a type. Resolve it." )]
		public void RegisterAndResolveByClass()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				var bar = container.Resolve<Bar>();

				Assert.That( bar, Is.Not.Null );
			}
		}

		[Test]
		[Description( "Register a type. Resolve a class by an interface of that type." )]
		public void RegisterAndResolveByInterface()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				var bar = container.Resolve<IBar>();

				Assert.That( bar, Is.Not.Null );
			}
		}

		[Test]
		[Description( "Register an interface should fail." )]
		[ExpectedException( typeof( RegisterException ) )]
		public void CannotRegisterInterface()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<IBar>();
			}
		}

		[Test]
		[Description( "Register an abstract class should fail." )]
		[ExpectedException( typeof( RegisterException ) )]
		public void CannotRegisterAbstractClass()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<AbstractClass>();
			}
		}

		[Test]
		[Description( "Register a base type and a derived type. Resolve a base class." )]
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
				container.Resolve<Bar>();
			}
		}

		[Test]
		[Description( "Register two derived types. Resolvíng a base class (that was not explicitly registered) should fail." )]
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
		[Description( "Register a derived type. Resolvíng a base class (that was not explicitly registered). Register the base class explicitly. Now resolve it a again." )]
		public void RegisterAndResolveDerivedTypeAndInstance()
		{
			using( var container = new Container( true ) )
			{
				// At first register the derived type
				container.Register.Type<BarDerived>();

				// Resolve the base type
				var implicitlyResolvedBar = container.Resolve<Bar>();

				// Now register the base type itself ...
				var bar = new Bar();
				container.Register.Instance( bar );

				// Resolve will return the explicitly registered base type
				var explicitlyResolvedBar = container.Resolve<Bar>();

				Assert.That( explicitlyResolvedBar, Is.EqualTo( bar ) );
				Assert.That( explicitlyResolvedBar, Is.Not.EqualTo( implicitlyResolvedBar ) );
			}
		}

		[Test]
		[Description( "Register a type without using generics. Resolve it." )]
		public void RegisterByGivenType()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type( typeof( Bar ) );
				var bar = container.Resolve<IBar>();

				Assert.That( bar, Is.Not.Null );
			}
		}

		[Test]
		[Description( "Resolve a class that is not registered. The container is configured not to throw an exception, so the resolved value will be null." )]
		public void ResolveUnknownType()
		{
			using( var container = new Container( false ) )
			{
				var instance = container.Resolve<IBar>();
				Assert.IsNull( instance );
			}
		}

		[Test]
		[Description( "Resolve a class that is not registered. The container is configured to throw an exception." )]
		[ExpectedException( typeof( ResolveException ) )]
		public void ResolveUnknownTypeWithException()
		{
			using( var container = new Container( true ) )
			{
				container.Resolve<IBar>();
			}
		}

		[Test]
		[Description( "Register a type. Resolve it without using generics." )]
		public void ResolveByGivenType()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				var bar = container.Resolve( typeof( IBar ) );

				Assert.That( bar, Is.Not.Null );
			}
		}

		[Test]
		[Description( "Register a type without using generics. Resolve it without using generics." )]
		public void RegisterAndResolveByGivenType()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type( typeof( Bar ) );
				var bar = container.Resolve( typeof( IBar ) );

				Assert.That( bar, Is.Not.Null );
			}
		}

		[Test]
		[Description( "Register a type with a private constructor, only. Resolve it." )]
		public void ResolvePrivateConstructor()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<PrivateConstructor>();
				var privateConstructor = container.Resolve<PrivateConstructor>();

				Assert.That( privateConstructor, Is.Not.Null );
			}
		}
	}
}
