#if !DEBUG

using NUnit.Framework;

namespace Autowire.Tests
{
	[SetUpFixture]
	public class SetUpFixture
	{
		[SetUp]
		public void Setup()
		{
			Assert.Fail( "Unittests can be executed in debug mode, only." );
		}
	}
}

#endif