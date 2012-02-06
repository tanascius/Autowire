using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Autowire.Tests.Performance
{
	[TestFixture]
	public class PerformanceTests
	{
		[Test]
		public void ResolveLotsOfObjects()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Register.Type<Bar>();
				for( var i = 0; i < SetUpFixture.DynamicTypes.Length; i++ )
				{
					var instance = container.Resolve( SetUpFixture.DynamicTypes[i] );
					Assert.IsNotNull( instance );
					Assert.IsInstanceOfType( SetUpFixture.DynamicTypes[i], instance );
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
				container.Configure<AutoInjectConstructor>().Argument( Argument.UserProvided( "barNotInjected" ) );
				container.Register.Type<Bar>();
				container.Register.Type<AutoInjectConstructor>();
				MeasureTestcase( () => container.Resolve<AutoInjectConstructor>( NullArg.New<IBar>() ) );
			}
		}

		[Test]
		public void AutoInjectField()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Configure<AutoInjectField>().InjectField( "BarFieldInjected" );
				container.Register.Type<Bar>();
				container.Register.Type<AutoInjectField>();
				MeasureTestcase( () => container.Resolve<AutoInjectField>() );
			}
		}

		[Test]
		public void AutoInjectProperty()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Configure<AutoInjectProperty>().InjectProperty( "BarPropertyInjected" );
				container.Register.Type<Bar>();
				container.Register.Type<AutoInjectProperty>();
				MeasureTestcase( () => container.Resolve<AutoInjectProperty>() );
			}
		}

		[Test]
		public void AutoInjectMethod()
		{
			using( var container = new Container( true ) )
			{
				RegisterDynamicClasses( container );
				container.Configure<AutoInjectMethod>().InjectMethod( "Inject" );
				container.Register.Type<Bar>();
				container.Register.Type<AutoInjectMethod>();
				MeasureTestcase( () => container.Resolve<AutoInjectMethod>() );
			}
		}

		private static void RegisterDynamicClasses( IContainer container )
		{
			for( var i = 0; i < SetUpFixture.DynamicTypes.Length; i++ )
			{
				container.Register.Type( SetUpFixture.DynamicTypes[i] );
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
