using System;
using Autowire.Utils.FastDynamics;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests.FastDynamics
{
	[TestFixture]
	public class FastDelegateFactoryTests
	{
		[Test]
		public void CreateDelegateNoArg()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();

				var fastDelegateFactory = new FastDelegateFactory( container, typeof( Func<IBar> ) );

				var barFactory = ( Func<IBar> ) fastDelegateFactory.Delegate;
				Assert.IsNotNull( barFactory );

				var bar = barFactory();
				Assert.That( bar, Is.Not.Null );
			}
		}

		[Test]
		public void CreateDelegateOneUserProvidedArg()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<Foo>().Argument( Argument.UserProvided( "bar" ) );
				container.Register.Type<Bar>();
				container.Register.Type<Foo>();

				var fastDelegateFactory = new FastDelegateFactory( container, typeof( Func<IBar, IFoo> ) );
				var @delegate = ( Func<IBar, IFoo> ) fastDelegateFactory.Delegate;

				Assert.IsNotNull( @delegate );
				var bar = new Bar();
				var foo = @delegate( bar );
				Assert.That( foo, Is.Not.Null );
				Assert.That( foo.Bar, Is.EqualTo( bar ) );
			}
		}

		[Test]
		public void CreateDelegateOneInjectedArg()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<Bar>();
				container.Register.Type<Foo>();

				var fastDelegateFactory = new FastDelegateFactory( container, typeof( Func<IFoo> ) );
				var @delegate = ( Func<IFoo> ) fastDelegateFactory.Delegate;

				Assert.IsNotNull( @delegate );
				var foo = @delegate();
				Assert.That( foo, Is.Not.Null );
				Assert.That( foo.Bar, Is.Not.Null );
			}
		}
	}
}
