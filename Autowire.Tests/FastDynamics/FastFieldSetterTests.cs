using System;
using System.Reflection;
using Autowire.Utils.FastDynamics;
using NUnit.Framework;

namespace Autowire.Tests.FastDynamics
{
	[TestFixture]
	public class FastFieldSetterTests
	{
		[Test]
		[Description( "Set a public field" )]
		public void SetField()
		{
			var fieldInfo = typeof( TestClassForFieldSetter ).GetField( "PublicField" );
			var fastFieldSetter = new FastFieldSetter( fieldInfo );

			var testClassForFieldSetter = new TestClassForFieldSetter();
			fastFieldSetter.Set( testClassForFieldSetter, "bleh" );

			Assert.AreEqual( "bleh", testClassForFieldSetter.PublicField );
		}

		[Test]
		[Description( "Set a private field" )]
		public void SetPrivateField()
		{
			var fieldInfo = typeof( TestClassForFieldSetter ).GetField( "m_PrivateField", BindingFlags.Instance | BindingFlags.NonPublic );
			var fastPropertySetter = new FastFieldSetter( fieldInfo );

			var testClassForFieldSetter = new TestClassForFieldSetter();
			fastPropertySetter.Set( testClassForFieldSetter, "bleh" );

			testClassForFieldSetter.AssertPrivateFieldIsEqual( "bleh" );
		}

		[Test]
		[Description( "Set a readonly field" )]
		public void SetReadonlyField()
		{
			var fieldInfo = typeof( TestClassForFieldSetter ).GetField( "m_ReadonlyField", BindingFlags.Instance | BindingFlags.NonPublic );
			var fastFieldSetter = new FastFieldSetter( fieldInfo );

			var testClassForFieldSetter = new TestClassForFieldSetter();
			fastFieldSetter.Set( testClassForFieldSetter, "bleh" );

			testClassForFieldSetter.AssertReadonlyFieldIsEqual( "bleh" );
		}

		[Test, ExpectedException( typeof( InvalidCastException ) )]
		[Description( "Set an integer value to a string field" )]
		public void SetFieldWrongType()
		{
			var fieldInfo = typeof( TestClassForFieldSetter ).GetField( "PublicField" );
			var fastFieldSetter = new FastFieldSetter( fieldInfo );

			var testClassForFieldSetter = new TestClassForFieldSetter();
			fastFieldSetter.Set( testClassForFieldSetter, 5 );
		}
	}

	#region Testclasses
	internal sealed class TestClassForFieldSetter
	{
#pragma warning disable 649
		private readonly string m_ReadonlyField;
		private string m_PrivateField;
		public string PublicField;
#pragma warning restore 649

		public void AssertPrivateFieldIsEqual( object value )
		{
			Assert.AreEqual( value, m_PrivateField );
		}

		public void AssertReadonlyFieldIsEqual( object value )
		{
			Assert.AreEqual( value, m_ReadonlyField );
		}
	}
	#endregion
}