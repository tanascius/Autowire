﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class GenericTests
	{
		[Test]
		public void RegisterAndResolveGeneric()
		{
			using( var container = new Container() )
			{
				var config = container.Configure( typeof( GenericClass<> ) );
				config.Argument( Argument.UserProvided( "genericValue" ) );
				config.Argument( Argument.UserProvided( "value" ) );

				container.Register.Type( typeof( GenericClass<> ) );

				var bar = container.Resolve<GenericClass<Bar>>();

				Assert.IsNotNull( bar );
			}
		}

		[Test]
		public void RegisterAndResolveExternalGeneric()
		{
			using( var container = new Container() )
			{
				container.Configure( typeof( ComplexGenericClass<Bar> ) ).Argument( Argument.UserProvided( "pair" ) );

				container.Register.Type<Bar>();
				container.Register.Type<ComplexGenericClass<Bar>>();

				var bar = container.Resolve<ComplexGenericClass<Bar>>();

				Assert.IsNotNull( bar );
			}
		}

		[Test]
		public void RegisterAndResolveGenericOfDifferentType()
		{
			using( var container = new Container() )
			{
				var config = container.Configure( typeof( GenericClass<> ) );
				config.Argument( Argument.UserProvided( "genericValue" ) );
				config.Argument( Argument.UserProvided( "value" ) );

				container.Register.Type( typeof( GenericClass<> ) );

				var genericClassBar = container.Resolve<GenericClass<Bar>>();
				var genericClassString = container.Resolve<GenericClass<string>>();

				Assert.IsNotNull( genericClassBar );
				Assert.IsNotNull( genericClassString );
			}
		}

		[Test]
		public void RegisterAndResolveGenericWithArgument()
		{
			using( var container = new Container() )
			{
				var config = container.Configure( typeof( GenericClass<> ) );
				config.Argument( Argument.UserProvided( "genericValue" ) );
				config.Argument( Argument.UserProvided( "value" ) );

				container.Register.Type( typeof( GenericClass<> ) );

				var genericClass = container.Resolve<GenericClass<int>>( 5 );

				Assert.IsNotNull( genericClass );
				Assert.AreEqual( 5, genericClass.Value );
			}
		}

		[Test]
		public void RegisterAndResolveGenericWith2Arguments()
		{
			using( var container = new Container() )
			{
				var config = container.Configure( typeof( GenericClass<> ) );
				config.Argument( Argument.UserProvided( "genericValue" ) );
				config.Argument( Argument.UserProvided( "value" ) );

				container.Register.Type( typeof( GenericClass<> ) );

				var genericClass = container.Resolve<GenericClass<int>>( 5, "blo" );

				Assert.IsNotNull( genericClass );
				Assert.AreEqual( 5, genericClass.Value );
				Assert.AreEqual( "blo", genericClass.StringValue );
			}
		}

		[Test]
		public void RegisterAndResolveGenericWithFixedArgument()
		{
			using( var container = new Container() )
			{
				var config = container.Configure( typeof( GenericClass<> ) );
				config.Argument( Argument.UserProvided( "genericValue" ) );
				config.Argument( Argument.UserProvided( "value" ) );

				container.Register.Type( typeof( GenericClass<> ) );

				var genericClass = container.Resolve<GenericClass<string>>( "bleh" );

				Assert.IsNotNull( genericClass );
				Assert.AreEqual( "bleh", genericClass.StringValue );
			}
		}

		[Test]
		public void RegisterAndResolveComplexGeneric()
		{
			using( var container = new Container() )
			{
				container.Configure( typeof( ComplexGenericClass<> ) ).Argument( Argument.UserProvided( "pair" ) );

				container.Register.Type( typeof( Bar ) );
				container.Register.Type( typeof( ComplexGenericClass<> ) );

				var collection = new Collection<Bar>();
				var argument = new KeyValuePair<Bar, IEnumerable<Bar>>( new Bar(), collection );
				var genericClass = container.Resolve<ComplexGenericClass<Bar>>( argument );

				Assert.IsNotNull( genericClass );
			}
		}

		[Test]
		public void RegisterAndResolveComplexGenericWithInjection()
		{
			using( var container = new Container() )
			{
				container.Configure( typeof( ComplexGenericClass<> ) ).Argument( Argument.UserProvided( "pair" ) );

				container.Register.Type( typeof( Bar ) );
				container.Register.Type( typeof( ComplexGenericClass<> ) );

				var genericClass = container.Resolve<ComplexGenericClass<Bar>>();

				Assert.IsNotNull( genericClass );
			}
		}
	}

	internal sealed class GenericClass<T>
	{
		public GenericClass() {}

		public GenericClass( T genericValue, string value )
		{
			Value = genericValue;
			StringValue = value;
		}

		public GenericClass( T genericValue )
		{
			Value = genericValue;
		}

		public GenericClass( string value )
		{
			StringValue = value;
		}

		public T Value { get; private set; }
		public string StringValue { get; private set; }
	}

	internal sealed class ComplexGenericClass<T>
	{
		public ComplexGenericClass( KeyValuePair<T, IEnumerable<T>> pair ) {}

		public ComplexGenericClass( T arg ) {}
	}
}
