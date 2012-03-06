using System;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	internal class ResolveFuncTests
	{
		#region Test objects: IBar, Bar, FuncTestObject
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable MemberHidesStaticFromOuterClass

		private interface IBar {}

		private class Bar : IBar {}

		private class FuncTestObject
		{
			public FuncTestObject( Func<IBar> barFactory )
			{
				Bar = barFactory();
			}

			public IBar Bar { get; private set; }
		}

		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore MemberHidesStaticFromOuterClass
		#endregion

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
}
