using NUnit.Framework;

namespace Autowire.Tests
{
	[TestFixture]
	public class InjectionTests
	{
		#region Testclasses
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable InconsistentNaming
		// ReSharper disable MemberHidesStaticFromOuterClass
		// ReSharper disable UnusedAutoPropertyAccessor.Local
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
#pragma warning disable 649

		private interface IBar {}

		private class Bar : IBar {}

		private sealed class AutoInjectConstructor
		{
			public AutoInjectConstructor( IBar barInjected, IBar barNotInjected )
			{
				BarInjected = barInjected;
				BarNotInjected = barNotInjected;
			}

			public IBar BarInjected { get; private set; }

			public IBar BarNotInjected { get; private set; }
		}

		private class AutoInjectField
		{
			public IBar BarFieldInjected;

			public static IBar StaticBar;

			private IBar PrivateBar;

			public bool IsPrivateBarSet
			{
				get { return PrivateBar != null; }
			}
		}

		private interface IAutoInjectProperty
		{
			IBar BarPropertyInjected { get; }
		}

		private class DerivedAutoInject : AutoInjectField, IAutoInjectProperty
		{
			public IBar BarPropertyInjected { get; private set; }
		}

		private sealed class AutoInjectProperty
		{
			public IBar BarPropertyInjected { get; set; }

			public static IBar StaticBar { get; set; }
		}

		private sealed class AutoInjectMethod
		{
			public IBar BarInjected { get; private set; }

			public IBar BarNotInjected { get; private set; }

			public static IBar StaticBar { get; private set; }

			public void Inject( IBar bar )
			{
				BarInjected = bar;
			}

			public static void StaticInject( IBar bar )
			{
				StaticBar = bar;
			}
		}

		private abstract class AbstractBaseClass
		{
			protected IBar BaseBar { get; private set; }

			public abstract IBar Bar { get; }
		}

		private sealed class DerivedFromAbstractBaseClass : AbstractBaseClass
		{
			private IBar m_Bar;

			public override IBar Bar
			{
				get { return m_Bar; }
			}
		}

		private abstract class BaseClassWithMethod
		{
			public IBar Bar;

			private void SetBar( IBar bar )
			{
				Bar = bar;
			}
		}

		private sealed class DerivedFromBaseClassWithMethod : BaseClassWithMethod {}

		private sealed class InjectForOtherClass
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

			private sealed class DoInjectMe
			{
				public IBar BarField;
			}

			private sealed class DoInjectMeToo
			{
				public IBar BarProperty { get; set; }
			}

			private sealed class AndMeToo
			{
				public void BarMethod( IBar bar )
				{
					Bar = bar;
				}

				public IBar Bar;
			}
		}

		private sealed class InjectForOtherFailsClass {}

#pragma warning restore 649
		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore InconsistentNaming
		// ReSharper restore MemberHidesStaticFromOuterClass
		// ReSharper restore UnusedAutoPropertyAccessor.Local
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
		#endregion

		[Test]
		public void UsingInjectForConstructor()
		{
			using( var container = new Container() )
			{
				container.Configure<AutoInjectConstructor>().Arguments( Argument.UserProvided( "barNotInjected" ) );

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
				container.Configure<AutoInjectConstructor>().Arguments( Argument.UserProvided( "barNotInjected" ) );

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
				container.Configure<AutoInjectConstructor>().Arguments( Argument.UserProvided( "barNotInjected" ) );

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
				container.Configure<InjectForOtherClass>().InjectForComponent( "m_DoInjectMe" ).InjectField( "BarField" );
				container.Configure<InjectForOtherClass>().InjectForComponent( "m_DoInjectMeToo" ).InjectProperty( "BarProperty" );
				container.Configure<InjectForOtherClass>().InjectForComponent( "m_AndMeToo" ).InjectMethod( "BarMethod" );

				container.Register.Type<Bar>().WithScope( Scope.Singleton );
				container.Register.Type<InjectForOtherClass>();

				var injectForOther = container.Resolve<InjectForOtherClass>();

				Assert.IsNotNull( injectForOther );
				injectForOther.Test();
			}
		}

		[Test, ExpectedException( typeof( ConfigureException ) )]
		public void InjectForOtherFails()
		{
			using( var container = new Container() )
			{
				container.Configure<InjectForOtherFailsClass>().InjectForComponent( "DoInjectMe" );

				container.Register.Type<InjectForOtherFailsClass>();
			}
		}
	}
}
