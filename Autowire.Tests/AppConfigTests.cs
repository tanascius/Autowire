using System.Configuration;
using Autowire.Registration;
using Autowire.Registration.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests
{
	[TestFixture]
	public class AppConfigTests
	{
		[Test]
		public void CheckConfigurationSectionStructure()
		{
			var section = ( AutowireConfigurationSection ) ConfigurationManager.GetSection( "autowire" );
			Assert.That( section.Containers.Count, Is.EqualTo( 2 ) );

			Assert.That( section.Containers["structure"], Is.Not.Null );

			var containerSection = section.Containers["structure"];
			Assert.That( containerSection.Types.Count, Is.EqualTo( 3 ) );

			Assert.That( containerSection.Types[0].Name, Is.EqualTo( "singleton_type" ) );
			Assert.That( containerSection.Types[0].Scope, Is.EqualTo( Scope.Singleton ) );

			Assert.That( containerSection.Types[1].Name, Is.EqualTo( "default_type" ) );
			Assert.That( containerSection.Types[1].Scope, Is.EqualTo( Scope.Default ) );

			Assert.That( containerSection.Types[2].Name, Is.EqualTo( "with_ctor" ) );
			Assert.That( containerSection.Types[2].ConstructorArguments.Count, Is.EqualTo( 2 ) );
			Assert.That( containerSection.Types[2].ConstructorArguments[0].Name, Is.EqualTo( "bar" ) );
		}

		[Test]
		public void CreateContainer()
		{
			var section = ( AutowireConfigurationSection ) ConfigurationManager.GetSection( "autowire" );
			using( var container = section.Containers["foo_and_bar"].Create( true ) )
			{
				// Check that bar is no singleton
				var bar1 = container.Resolve<EnumerableTests.Bar>();
				var bar2 = container.Resolve<EnumerableTests.Bar>();
				Assert.That( bar1, Is.Not.EqualTo( bar2 ) );

				// Check that foo is a singleton
				var foo1 = container.Resolve<EnumerableTests.Foo>();
				var foo2 = container.Resolve<EnumerableTests.Foo>();
				Assert.That( foo1, Is.EqualTo( foo2 ) );
			}
		}
	}
}
