using System;
using System.Diagnostics;
using Autowire.Registration;
using NUnit.Framework;

namespace Autowire.Tests.Performance
{
	[TestFixture]
	public class PerformanceTests
	{
		#region test objects: IBar, Bar, AutoInjectConstructorClass, AutoInjectFieldClass, AutoInjectPropertyClass, AutoInjectMethodClass
		// ReSharper disable ClassNeverInstantiated.Local
		// ReSharper disable RedundantDefaultFieldInitializer
		// ReSharper disable UnusedAutoPropertyAccessor.Local
		// ReSharper disable UnusedMember.Local
#pragma warning disable 169

		private interface IBar {}

		private class Bar : IBar {}

		private sealed class AutoInjectConstructorClass
		{
			public AutoInjectConstructorClass( IBar barInjected, IBar barNotInjected )
			{
				BarInjected = barInjected;
				BarNotInjected = barNotInjected;
			}

			private IBar BarInjected { get; set; }

			private IBar BarNotInjected { get; set; }
		}

		private class AutoInjectFieldClass
		{
			public IBar BarFieldInjected = null;

			public static IBar StaticBar = null;

			private readonly IBar m_PrivateBar = null;

			public bool IsPrivateBarSet
			{
				get { return m_PrivateBar != null; }
			}
		}

		private sealed class AutoInjectPropertyClass
		{
			public IBar BarPropertyInjected { get; set; }

			public static IBar StaticBar { get; set; }
		}

		private sealed class AutoInjectMethodClass
		{
			private IBar BarInjected { get; set; }

			public IBar BarNotInjected { get; private set; }

			private static IBar StaticBar { get; set; }

			public void Inject( IBar bar )
			{
				BarInjected = bar;
			}

			public static void StaticInject( IBar bar )
			{
				StaticBar = bar;
			}
		}

#pragma warning restore 169
		// ReSharper restore ClassNeverInstantiated.Local
		// ReSharper restore RedundantDefaultFieldInitializer
		// ReSharper restore UnusedAutoPropertyAccessor.Local
		// ReSharper restore UnusedMember.Local
		#endregion

		[Test]
		public void ResolveLotsOfObjects()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Register.Type<Bar>();
				foreach( var t in SetUpFixture.DynamicTypes )
				{
					var instance = container.Resolve( t );
					Assert.IsNotNull( instance );
					Assert.IsInstanceOfType( t, instance );
				}
			}
		}

		[Test]
		public void GenericResolve()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Register.Type<Bar>();
				MeasureTestcase( () => container.Resolve<IBar>() );
			}
		}

		[Test]
		public void UniversalResolve()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Register.Type<Bar>();
				MeasureTestcase( () => container.Resolve( typeof( IBar ) ) );
			}
		}

		[Test]
		public void AutoInjectConstructor()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Configure<AutoInjectConstructorClass>().Arguments( Argument.UserProvided( "barNotInjected" ) );
				container.Register.Type<Bar>();
				container.Register.Type<AutoInjectConstructorClass>();
				MeasureTestcase( () => container.Resolve<AutoInjectConstructorClass>( Argument.Null<IBar>() ) );
			}
		}

		[Test]
		public void AutoInjectField()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Configure<AutoInjectFieldClass>().InjectField( "BarFieldInjected" );
				container.Register.Type<Bar>();
				container.Register.Type<AutoInjectFieldClass>();
				MeasureTestcase( () => container.Resolve<AutoInjectFieldClass>() );
			}
		}

		[Test]
		public void AutoInjectProperty()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Configure<AutoInjectPropertyClass>().InjectProperty( "BarPropertyInjected" );
				container.Register.Type<Bar>();
				container.Register.Type<AutoInjectPropertyClass>();
				MeasureTestcase( () => container.Resolve<AutoInjectPropertyClass>() );
			}
		}

		[Test]
		public void AutoInjectMethod()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Configure<AutoInjectMethodClass>().InjectMethod( "Inject" );
				container.Register.Type<Bar>();
				container.Register.Type<AutoInjectMethodClass>();
				MeasureTestcase( () => container.Resolve<AutoInjectMethodClass>() );
			}
		}

		private static void RegisterDynamicClasses( IContainer container )
		{
			foreach( var t in SetUpFixture.DynamicTypes )
			{
				container.Register.Type( t );
			}
		}

		private static void MeasureTestcase( Action action )
		{
			const int runs = 1000000;
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			for( var i = 0; i < runs; i++ )
			{
				action.Invoke();
			}
			stopwatch.Stop();

			Debug.WriteLine( stopwatch.ElapsedMilliseconds );
		}
	}
}
