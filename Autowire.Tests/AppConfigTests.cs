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
			Assert.That( section.Containers.Count, Is.EqualTo( 3 ) );

			Assert.That( section.Containers["structure"], Is.Not.Null );

			Assert.That( section.Containers["structure"].Types.Count, Is.EqualTo( 2 ) );
			Assert.That( section.Containers["structure"].Types[0].Name, Is.EqualTo( "singleton_type" ) );
			Assert.That( section.Containers["structure"].Types[0].Scope, Is.EqualTo( Scope.Singleton ) );
			Assert.That( section.Containers["structure"].Types[1].Name, Is.EqualTo( "default_type" ) );
			Assert.That( section.Containers["structure"].Types[1].Scope, Is.EqualTo( Scope.Default ) );
		}

		[Test]
		public void CreateContainer()
		{
			var section = ( AutowireConfigurationSection ) ConfigurationManager.GetSection( "autowire" );
			using( var container = section.Containers["foo_and_bar"].Create() )
			{
				Assert.That( container.IsRegistered( typeof( EnumerableTests.Bar ) ), Is.True );
				Assert.That( container.IsRegistered( typeof( EnumerableTests.Foo ) ), Is.True );
			}
		}
	}
}
