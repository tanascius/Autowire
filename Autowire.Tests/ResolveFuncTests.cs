using System;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	internal class ResolveFuncTests
	{
		[Test]
		public void ResolveSimpleFunc()
		{
			using( var container = new Container( true ) )
			{
				container.Register.Type<FuncTestObject>();
				container.Register.Type<Bar>();

				var factoryTestClass = container.Resolve<FuncTestObject>();

				Assert.IsNotNull( factoryTestClass );
				Assert.IsNotNull( factoryTestClass.Bar );
				Assert.IsInstanceOfType( typeof( IBar ), factoryTestClass.Bar );
			}
		}
	}

	internal class FuncTestObject
	{
		public FuncTestObject( Func<IBar> barFactory )
		{
			Bar = barFactory();
		}

		public IBar Bar { get; set; }
	}
}
