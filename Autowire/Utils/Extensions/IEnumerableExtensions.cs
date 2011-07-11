using System;
using System.Collections.Generic;
using System.Linq;

namespace Autowire.Utils.Extensions
{
	/// <summary>Stellt Extensions für <see cref="IEnumerable{T}"/>s zur Verfügung</summary>
	public static class IEnumerableExtensions
	{
		#region AddMissing()
		///<summary>Adds all missing <see cref="KeyValuePair{TKey,TValue}"/>s from the given <see cref="IDictionary{TKey,TValue}"/>.</summary>
		///<param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> to which the missing elements will be added.</param>
		///<param name="addFromHere">The <see cref="IDictionary{TKey,TValue}"/> which will provide the missing elements.</param>
		///<typeparam name="TKey">The type of the keys of the <see cref="IDictionary{TKey,TValue}"/>s.</typeparam>
		///<typeparam name="TValue">The type of the values of the <see cref="IDictionary{TKey,TValue}"/>s.</typeparam>
		public static void AddMissing<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> addFromHere )
		{
			foreach( var pair in addFromHere )
			{
				if( !dictionary.ContainsKey( pair.Key ) )
				{
					dictionary.Add( pair );
				}
			}
		}
		#endregion

		#region Apply()
		/// <summary>Applies an action to every element of the list.</summary>
		public static void Apply<T>( this IEnumerable<T> enumerable, Action<T> action )
		{
			foreach( var item in enumerable )
			{
				action.Invoke( item );
			}
		}

		/// <summary>Applies an action to every element of the list.</summary>
		public static void Apply<T>( this IEnumerable<T> enumerable, Action<T, int> action )
		{
			var i = 0;
			foreach( var item in enumerable )
			{
				action.Invoke( item, i++ );
			}
		}
		#endregion

		#region MaxOrDefault()
		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the maximum result value 
		/// if the sequence is not empty. Otherwise returns the specified default value.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TResult">The type of the maximum value.</typeparam>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The maximum value in the sequence or the default value if the sequence is empty.</returns>
		public static TResult MaxOrDefault<TSource, TResult>( this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult defaultValue )
		{
			return source.Any() ? source.Max( selector ) : defaultValue;
		}
		#endregion

		#region MinOrDefault()
		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the minimum result value 
		/// if the sequence is not empty. Otherwise returns the specified default value.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TResult">The type of the minimum value.</typeparam>
		/// <param name="source">A sequence of values to determine the minimum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The minimum value in the sequence or the default value if the sequence is empty.</returns>
		public static TResult MinOrDefault<TSource, TResult>( this IEnumerable<TSource> source, Func<TSource, TResult> selector, TResult defaultValue )
		{
			return source.Any() ? source.Min( selector ) : defaultValue;
		}
		#endregion

		#region Shuffle()
		///<summary>Performs a Fisher-Yates-Durstenfeld shuffle.</summary>
		///<param name="source">The sequence that will be shuffled.</param>
		///<exception cref="ArgumentNullException">Parameter <para>source</para> cannot be null.</exception>
		public static IEnumerable<T> Shuffle<T>( this IEnumerable<T> source )
		{
			if( source == null )
			{
				throw new ArgumentNullException( "source" );
			}

			var random = new Random();
			var items = source.ToArray();

			for( var i = 0; i < items.Length; i++ )
			{
				var k = random.Next( i, items.Length );
				yield return items[k];
				items[k] = items[i];
			}
		}
		#endregion
	}
}