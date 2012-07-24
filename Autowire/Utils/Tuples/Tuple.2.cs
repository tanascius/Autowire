namespace Autowire.Utils.Tuples
{
#if NET35

	/// <summary>Holds two values.</summary>
	/// <typeparam name="TItem1">Type of the first value.</typeparam>
	/// <typeparam name="TItem2">Type of the second value.</typeparam>
	public sealed class Tuple<TItem1, TItem2> : IEquatable<Tuple<TItem1, TItem2>>
	{
		/// <summary>Initializes a new instance of the <see cref="Tuple{TItem1,TItem2}" /> class.</summary>
		/// <param name="item1">The first value.</param>
		/// <param name="item2">The second value.</param>
		public Tuple( TItem1 item1, TItem2 item2 )
		{
			Item1 = item1;
			Item2 = item2;
		}

		/// <summary>The first value.</summary>
		public TItem1 Item1 { get; set; }

		/// <summary>The second value.</summary>
		public TItem2 Item2 { get; set; }

		/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
		/// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals( Tuple<TItem1, TItem2> other )
		{
			if( other == null )
			{
				return false;
			}
			return EqualityComparer<TItem1>.Default.Equals( Item1, other.Item1 ) && EqualityComparer<TItem2>.Default.Equals( Item2, other.Item2 );
		}

		/// <summary>Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.</summary>
		/// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
		/// <returns>true if the specified <see cref="object"/> is equal to the current <see cref="object"/> otherwise, false.</returns>
		public override bool Equals( object obj )
		{
			return Equals( obj as Tuple<TItem1, TItem2> );
		}

		/// <summary>Serves as a hash function for a particular type.</summary>
		/// <returns>A hash code for the current <see cref="object"/>.</returns>
		public override int GetHashCode()
		{
			return EqualityComparer<TItem1>.Default.GetHashCode( Item1 ) * 37 + EqualityComparer<TItem2>.Default.GetHashCode( Item2 );
		}
	}

#endif
}
