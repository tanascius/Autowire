using System;
using System.Diagnostics;

namespace Autowire.Utils.Extensions
{
	/// <summary>Stellt Extensions für <see cref="object"/>s zur Verfügung</summary>
	public static class ObjectExtensions
	{
		#region CheckNullArgument()
		/// <summary>Throws an <see cref="ArgumentNullException"/> if neccessary.</summary>
		/// <param name="checkMe">The object that is checked agaist null</param>
		/// <param name="paramName">The parametername that is shown.</param>
		/// <typeparam name="T">Eine Typbeschränkung auf Klassen.</typeparam>
		public static void CheckNullArgument<T>( this T checkMe, string paramName )
		{
			if( !Equals( checkMe, null ) )
			{
				return;
			}
			Debug.Fail( "ArgumentNullException( {0} )".FormatUi( paramName ) );
			throw new ArgumentNullException( paramName );
		}
		#endregion

		#region ToString()
		/// <summary>Returns a <see cref="string"/> that represents the current <see cref="object"/>. If the object is null the given defaultValue will be returned.</summary>
		/// <param name="convertToString">The object that will be represented as a string.</param>
		/// <param name="defaultValue">The default value for null objects.</param>
		/// <returns>A stringrepresentation of the given object.</returns>
		public static string ToString( this object convertToString, string defaultValue )
		{
			return convertToString != null ? convertToString.ToString() : defaultValue;
		}
		#endregion
	}
}
