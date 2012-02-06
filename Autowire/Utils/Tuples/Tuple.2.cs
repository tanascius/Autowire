using System;
using System.Collections.Generic;

namespace Autowire.Utils.Tuples
{
	/// <summary>Holds two values.</summary>
	/// <typeparam name="TFirst">Type of the first value.</typeparam>
	/// <typeparam name="TSecond">Type of the second value.</typeparam>
	public sealed class Tuple<TFirst, TSecond> : IEquatable<Tuple<TFirst, TSecond>>
	{
		/// <summary>Initializes a new instance of the <see cref="Tuple{TFirst,TSecond}" /> class.</summary>
		/// <param name="first">The first value.</param>
		/// <param name="second">The second value.</param>
		public Tuple( TFirst first, TSecond second )
		{
			Item1 = first;
			Item2 = second;
		}

		/// <summary>The first value.</summary>
		public TFirst Item1 { get; set; }

		/// <summary>The second value.</summary>
		public TSecond Item2 { get; set; }

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals( Tuple<TFirst, TSecond> other )
		{
			if( other == null )
			{
				return false;
			}
			return EqualityComparer<TFirst>.Default.Equals( Item1, other.Item1 ) && EqualityComparer<TSecond>.Default.Equals( Item2, other.Item2 );
		}

		/// <summary>Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.</summary>
		/// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
		/// <returns>true if the specified <see cref="object"/> is equal to the current <see cref="object"/> otherwise, false.</returns>
		public override bool Equals( object obj )
		{
			return Equals( obj as Tuple<TFirst, TSecond> );
		}

		/// <summary>Serves as a hash function for a particular type.</summary>
		/// <returns>A hash code for the current <see cref="object"/>.</returns>
		public override int GetHashCode()
		{
			return EqualityComparer<TFirst>.Default.GetHashCode( Item1 ) * 37 + EqualityComparer<TSecond>.Default.GetHashCode( Item2 );
		}
	}
}
