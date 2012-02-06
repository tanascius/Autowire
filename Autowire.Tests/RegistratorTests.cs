using System;
using System.Reflection;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class RegistratorTests
	{
#if DEBUG
		[Test]
		public void RegisterAllTypes()
		{
			using( var container = new Container() )
			{
				container.Register.Assembly( Assembly.GetExecutingAssembly(), IgnoreExceptionTypes );
				Assert.GreaterOrEqual( container.Register.Count, 50 );

				var publicMarkedType = container.Resolve<PublicMarkedType>();
				Assert.IsNotNull( publicMarkedType );
			}
		}
#endif

		[Test]
		public void ProvideArguments()
		{
			using( var container = new Container( true ) )
			{
				RegistrationHandler registrationHandler = delegate( Type type, ITypeConfiguration configuration )
				{
					if( type == typeof( PublicMarkedTypeWithParameter ) )
					{
						configuration.Argument( Argument.Create( "argument", "aValue" ) );
					}
					IgnoreExceptionTypes( type, configuration );
				};

				container.Register.Assembly( Assembly.GetExecutingAssembly(), registrationHandler );

				var typeWithParameter = container.Resolve<PublicMarkedTypeWithParameter>();

				Assert.IsNotNull( typeWithParameter );
				Assert.AreEqual( "aValue", typeWithParameter.Argument );
			}
		}

		private static void IgnoreExceptionTypes( Type type, ITypeConfiguration configuration )
		{
			switch( type.Name )
			{
				case "TestClassForInvokation":
				case "GenericTestClassForInvokationOneParameter`1":
				case "GenericClass`1":
				case "ComplexGenericClass`1":
				case "DuplicatedConstructor":
					configuration.Ignore();
					break;
			}
		}
	}

	public sealed class PublicMarkedType {}

	public sealed class PublicMarkedTypeWithParameter
	{
		public PublicMarkedTypeWithParameter( string argument )
		{
			Argument = argument;
		}

		public string Argument { get; set; }
	}
}
