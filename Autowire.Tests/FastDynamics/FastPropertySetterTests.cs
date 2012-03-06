using System;
using System.Reflection;
using Autowire.Utils.FastDynamics;
using NUnit.Framework;

namespace Autowire.Tests.FastDynamics
{
	[TestFixture]
	public class FastPropertySetterTests
	{
		#region test objects:
		// ReSharper disable UnusedAutoPropertyAccessor.Local

		internal class TestClassForPropertySetter
		{
			public string PublicSetter { get; set; }

			private string PrivateSetter { get; set; }

			public void CheckPrivateSetter( string expected )
			{
				Assert.AreEqual( expected, PrivateSetter );
			}
		}

		internal class DerivedClass : TestClassForPropertySetter {}

		// ReSharper restore UnusedAutoPropertyAccessor.Local
		#endregion

		[Test]
		public void SetProperty()
		{
			var propertyInfo = typeof( TestClassForPropertySetter ).GetProperty( "PublicSetter" );
			var fastPropertySetter = new FastPropertySetter( propertyInfo );

			var testClassForPropertySetter = new TestClassForPropertySetter();
			fastPropertySetter.Set( testClassForPropertySetter, "bleh" );

			Assert.AreEqual( "bleh", testClassForPropertySetter.PublicSetter );
		}

		[Test, ExpectedException( typeof( InvalidCastException ) )]
		public void SetPropertyWrongType()
		{
			var propertyInfo = typeof( TestClassForPropertySetter ).GetProperty( "PublicSetter" );
			var fastPropertySetter = new FastPropertySetter( propertyInfo );

			var testClassForPropertySetter = new TestClassForPropertySetter();
			fastPropertySetter.Set( testClassForPropertySetter, 5 );
		}

		[Test]
		public void SetPrivateProperty()
		{
			var propertyInfo = typeof( TestClassForPropertySetter ).GetProperty( "PrivateSetter", BindingFlags.Instance | BindingFlags.NonPublic );
			var fastPropertySetter = new FastPropertySetter( propertyInfo );

			var testClassForPropertySetter = new TestClassForPropertySetter();
			fastPropertySetter.Set( testClassForPropertySetter, "bleh" );
		}

		[Test]
		public void SetPrivateDerivedProperty()
		{
			var propertyInfo = typeof( DerivedClass ).BaseType.GetProperty( "PrivateSetter", BindingFlags.Instance | BindingFlags.NonPublic );
			Assert.IsNotNull( propertyInfo );
			var fastPropertySetter = new FastPropertySetter( propertyInfo );

			var testClassForPropertySetter = new DerivedClass();
			fastPropertySetter.Set( testClassForPropertySetter, "bleh" );

			testClassForPropertySetter.CheckPrivateSetter( "bleh" );
		}
	}
}
