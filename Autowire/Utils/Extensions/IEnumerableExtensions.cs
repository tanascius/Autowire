using System;
using System.Collections.Generic;

namespace Autowire.Utils.Extensions
{
	/// <summary>Stellt Extensions für <see cref="IEnumerable{T}"/>s zur Verfügung</summary>
	public static class IEnumerableExtensions
	{
		#region Apply()
		/// <summary>Applies an action to every element of the list.</summary>
		public static void Apply<T>( this IEnumerable<T> enumerable, Action<T> action )
		{
			foreach( var item in enumerable )
			{
				action.Invoke( item );
			}
		}
		#endregion
	}
}
