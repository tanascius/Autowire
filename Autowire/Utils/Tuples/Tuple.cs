namespace Autowire.Utils.Tuples
{
#if NET35

	/// <summary>Creates different tuples.</summary>
	public static class Tuple
	{
		/// <summary>Creates a tuple with two values.</summary>
		/// <typeparam name="TItem1">Type of the first value.</typeparam>
		/// <typeparam name="TItem2">Type of the second value.</typeparam>
		/// <param name="item1">The first value.</param>
		/// <param name="item2">The second value.</param>
		/// <returns>A <see cref="Tuple{TItem1,TItem2}"/>.</returns>
		public static Tuple<TItem1, TItem2> Create<TItem1, TItem2>( TItem1 item1, TItem2 item2 )
		{
			return new Tuple<TItem1, TItem2>( item1, item2 );
		}
	}

#endif
}
