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

	}
}
