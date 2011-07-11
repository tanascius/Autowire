using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class InjectionTests
	{
		[Test]
		public void UsingInjectForConstructor()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectConstructor>().Argument( Argument.UserProvided( "barNotInjected" ) );

				var bar = new Bar();
				container.Register.Instance( bar );
				container.Register.Type<AutoInjectConstructor>();

				var autoInject = container.Resolve<AutoInjectConstructor>( NullArg.New<IBar>() );

				Assert.IsNotNull( autoInject );
				Assert.IsNotNull( autoInject.BarInjected );
				Assert.IsNull( autoInject.BarNotInjected );
				Assert.AreEqual( autoInject.BarInjected, bar );
			}
		}

		[Test]
		public void UsingDerivedInjectForConstructor()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectConstructor>().Argument( Argument.UserProvided( "barNotInjected" ) );

				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<AutoInjectConstructor>();

				var autoInject = container.Resolve<AutoInjectConstructor>( NullArg.New<IBar>() );

				Assert.IsNotNull( autoInject.BarInjected );
				Assert.IsNull( autoInject.BarNotInjected );
				Assert.AreEqual( autoInject.BarInjected, container.Resolve<IBar>() );
			}
		}

		[Test, ExpectedException( typeof( ResolveException ) )]
		public void InjectNotRegisteredConstructorArgument()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<AutoInjectConstructor>().Argument( Argument.UserProvided( "barNotInjected" ) );

				container.Register.Type<AutoInjectConstructor>();
				container.Resolve<AutoInjectConstructor>( new Bar() );
			}
		}

		[Test]
		public void InjectField()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectField>().InjectField( "BarFieldInjected" );
				container.Configure<AutoInjectField>().InjectField( "PrivateBar" );

				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<AutoInjectField>();

				var autoInject = container.Resolve<AutoInjectField>();

				Assert.IsNull( AutoInjectField.StaticBar );
				Assert.IsNotNull( autoInject.BarFieldInjected );
				Assert.IsTrue( autoInject.IsPrivateBarSet );
				Assert.AreEqual( autoInject.BarFieldInjected, container.Resolve<IBar>() );
			}
		}

		[Test, ExpectedException( typeof( ResolveException ) )]
		public void InjectNotRegisteredField()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<AutoInjectField>().InjectField( "BarFieldInjected" );

				container.Register.Type<AutoInjectField>();

				container.Resolve<AutoInjectField>();
			}
		}

		[Test]
		public void InjectProperty()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectProperty>().InjectProperty( "BarPropertyInjected" );

				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<AutoInjectProperty>();

				var autoInject = container.Resolve<AutoInjectProperty>();

				Assert.IsNull( AutoInjectProperty.StaticBar );
				Assert.IsNotNull( autoInject.BarPropertyInjected );
				Assert.AreEqual( autoInject.BarPropertyInjected, container.Resolve<IBar>() );
			}
		}

		[Test, ExpectedException( typeof( ResolveException ) )]
		public void InjectNotRegisteredProperty()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<AutoInjectProperty>().InjectProperty( "BarPropertyInjected" );

				container.Register.Type<AutoInjectProperty>();

				container.Resolve<AutoInjectProperty>();
			}
		}

		[Test]
		public void InjectMethod()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectMethod>().InjectMethod( "Inject" );

				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<AutoInjectMethod>();

				var autoInjectMethod = container.Resolve<AutoInjectMethod>();

				Assert.IsNotNull( autoInjectMethod );
				Assert.IsNotNull( autoInjectMethod.BarInjected );
				Assert.IsNull( autoInjectMethod.BarNotInjected );
				Assert.IsNull( AutoInjectMethod.StaticBar );
			}
		}

		[Test, ExpectedException( typeof( ResolveException ) )]
		public void InjectNotRegisteredMethod()
		{
			using( var container = new Container( true ) )
			{
				container.Configure<AutoInjectMethod>().InjectMethod( "Inject" );

				container.Register.Type<AutoInjectMethod>();

				container.Resolve<AutoInjectMethod>();
			}
		}

		[Test]
		public void InjectDerivedFieldProperty()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectField>().InjectField( "BarFieldInjected" );
				container.Configure<AutoInjectField>().InjectField( "PrivateBar" );

				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<DerivedAutoInject>();

				var derivedAutoInject = container.Resolve<DerivedAutoInject>();

				Assert.IsNotNull( derivedAutoInject );
				Assert.IsNotNull( derivedAutoInject.BarFieldInjected );
				Assert.IsTrue( derivedAutoInject.IsPrivateBarSet );

				// This is a property of an interface -> not injected
				Assert.IsNull( derivedAutoInject.BarPropertyInjected );
			}
		}

		[Test]
		public void AbstractPropertiesAreNotInjected()
		{
			using( var container = new Container() )
			{
				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<DerivedFromAbstractBaseClass>();

				var derivedAutoInject = container.Resolve<DerivedFromAbstractBaseClass>();

				Assert.IsNotNull( derivedAutoInject );
				Assert.IsNull( derivedAutoInject.Bar );
			}
		}

		[Test]
		public void BaseMethodsAreInjected()
		{
			using( var container = new Container() )
			{
				container.Configure<DerivedFromBaseClassWithMethod>().InjectMethod( "SetBar" );

				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<DerivedFromBaseClassWithMethod>();

				var derivedAutoInject = container.Resolve<DerivedFromBaseClassWithMethod>();

				Assert.IsNotNull( derivedAutoInject );
				Assert.IsNotNull( derivedAutoInject.Bar );
			}
		}

		[Test]
		public void InjectForOther()
		{
			using( var container = new Container() )
			{
				container.Configure<InjectForOther>().InjectForComponent( "m_DoInjectMe" ).InjectField( "BarField" );
				container.Configure<InjectForOther>().InjectForComponent( "m_DoInjectMeToo" ).InjectProperty( "BarProperty" );
				container.Configure<InjectForOther>().InjectForComponent( "m_AndMeToo" ).InjectMethod( "BarMethod" );

				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<InjectForOther>();

				var injectForOther = container.Resolve<InjectForOther>();

				Assert.IsNotNull( injectForOther );
				injectForOther.Test();
			}
		}

		[Test, ExpectedException( typeof( ConfigureException ) )]
		public void InjectForOtherFails()
		{
			using( var container = new Container() )
			{
				container.Configure<InjectForOtherFails>().InjectForComponent( "DoInjectMe" );

				container.Register.Type<InjectForOtherFails>();
			}
		}
	}

	#region Testclasses
	internal sealed class AutoInjectConstructor
	{
		public AutoInjectConstructor( IBar barInjected, IBar barNotInjected )
		{
			BarInjected = barInjected;
			BarNotInjected = barNotInjected;
		}

		public IBar BarInjected { get; private set; }

		public IBar BarNotInjected { get; private set; }
	}

	internal class AutoInjectField
	{
		public IBar BarFieldInjected;

		public static IBar StaticBar;

#pragma warning disable 649
		private IBar PrivateBar;
#pragma warning restore 649

		public bool IsPrivateBarSet
		{
			get { return PrivateBar != null; }
		}
	}

	internal interface IAutoInjectProperty
	{
		IBar BarPropertyInjected { get; }
	}

	internal class DerivedAutoInject : AutoInjectField, IAutoInjectProperty
	{
		public IBar BarPropertyInjected { get; private set; }
	}

	internal sealed class AutoInjectNamedField
	{
#pragma warning disable 649
		public IBar BarFieldInjected;
#pragma warning restore 649
	}

	internal sealed class AutoInjectProperty
	{
		public IBar BarPropertyInjected { get; set; }

		public static IBar StaticBar { get; set; }
	}

	internal sealed class AutoInjectNamedProperty
	{
		public IBar BarPropertyInjected { get; set; }
	}

	internal sealed class AutoInjectMethod
	{
		public IBar BarInjected { get; private set; }

		public IBar BarNotInjected { get; private set; }

		public static IBar StaticBar { get; set; }

		public void Inject( IBar bar )
		{
			BarInjected = bar;
		}

		public static void StaticInject( IBar bar )
		{
			StaticBar = bar;
		}
	}

	internal sealed class AutoInjectNamedMethod
	{
		public IBar BarInjected { get; private set; }

		public void Inject( IBar bar )
		{
			BarInjected = bar;
		}
	}

	internal abstract class AbstractBaseClass
	{
		protected IBar BaseBar { get; private set; }

		public abstract IBar Bar { get; }
	}

	internal sealed class DerivedFromAbstractBaseClass : AbstractBaseClass
	{
#pragma warning disable 649
		private IBar m_Bar;
#pragma warning restore 649

		public override IBar Bar
		{
			get { return m_Bar; }
		}
	}

	internal abstract class BaseClassWithMethod
	{
		public IBar Bar;

		private void SetBar( IBar bar )
		{
			Bar = bar;
		}
	}

	internal sealed class DerivedFromBaseClassWithMethod : BaseClassWithMethod {}

	internal sealed class InjectForOther
	{
		private readonly DoInjectMe m_DoInjectMe = new DoInjectMe();
		private readonly DoInjectMeToo m_DoInjectMeToo = new DoInjectMeToo();
		private readonly AndMeToo m_AndMeToo = new AndMeToo();

		public void Test()
		{
			Assert.IsNotNull( m_DoInjectMe.BarField );
			Assert.IsNotNull( m_DoInjectMeToo.BarProperty );
			Assert.IsNotNull( m_AndMeToo.Bar );
		}

		internal sealed class DoInjectMe
		{
			public IBar BarField;
		}

		internal sealed class DoInjectMeToo
		{
			public IBar BarProperty { get; set; }
		}

		internal sealed class AndMeToo
		{
			public void BarMethod( IBar bar )
			{
				Bar = bar;
			}

			public IBar Bar;
		}
	}

	internal sealed class InjectForOtherFails {}
	#endregion
}