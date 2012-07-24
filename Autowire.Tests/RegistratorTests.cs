using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autowire.Tests
{
	[TestFixture]
	public class RegistratorTests
	{
		#region test obejcts: IBar, Bar, PublicMarkedType, PublicMarkedTypeWithParameter
		// ReSharper disable ClassNeverInstantiated.Local

		private interface IBar {}

		private class Bar : IBar {}

		private sealed class PublicMarkedType {}

		private sealed class PublicMarkedTypeWithParameter
		{
			public PublicMarkedTypeWithParameter( string argument )
			{
				Argument = argument;
			}

			public string Argument { get; private set; }
		}

		// ReSharper restore ClassNeverInstantiated.Local
		#endregion

#if DEBUG
		[Test]
		public void RegisterAllTypes()
		{
			using( var container = new Container( true ) )
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
				Action<Type, ITypeConfiguration> registrationHandler = delegate( Type type, ITypeConfiguration configuration )
				{
					if( type == typeof( PublicMarkedTypeWithParameter ) )
					{
						configuration.Arguments( Argument.Static( "argument", "aValue" ) );
					}
					IgnoreExceptionTypes( type, configuration );
				};

				container.Register.Assembly( Assembly.GetExecutingAssembly(), registrationHandler );

				var typeWithParameter = container.Resolve<PublicMarkedTypeWithParameter>();

				Assert.IsNotNull( typeWithParameter );
				Assert.AreEqual( "aValue", typeWithParameter.Argument );
			}
		}

		[Test]
		[ExpectedException( typeof( ResolveException ) )]
		public void RegisterIgnoredType()
		{
			using( var container = new Container( true ) )
			{
				// Configure type as ignored and register
				container.Configure<Bar>().Ignore();
				container.Register.Type<Bar>();

				container.Resolve<IBar>();
			}
		}

		[Test]
		public void RegisterAssemblyByName()
		{
			using( var container = new Container( true ) )
			{
				container.Register.AssemblyByName( "Autowire.Tests", IgnoreExceptionTypes );
				Assert.GreaterOrEqual( container.Register.Count, 50 );

				var publicMarkedType = container.Resolve<PublicMarkedType>();
				Assert.IsNotNull( publicMarkedType );
			}
		}

		[Test]
		[ExpectedException( typeof( RegisterException ) )]
		public void RegisterAssemblyByNameWithoutRegistrationHandler()
		{
			using( var container = new Container( true ) )
			{
				container.Register.AssemblyByName( "Autowire.Tests" );
			}
		}

		[Test]
		[ExpectedException( typeof( FileNotFoundException ) )]
		public void RegisterNonExistingAssemblyByName()
		{
			using( var container = new Container( true ) )
			{
				container.Register.AssemblyByName( "whatever" );
			}
		}

		[Test]
		public void RegisterNonExistingAssemblyByName2()
		{
			using( var container = new Container( true ) )
			{
				var registered = container.Register.TryAssemblyByName( "whatever" );
				Assert.That( registered, Is.False );
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
}
