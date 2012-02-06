namespace Autowire.Utils.Tuples
{
	/// <summary>Creates different tuples.</summary>
	public static class Tuple
	{
		/// <summary>Creates a tuple with two values.</summary>
		/// <typeparam name="TFirst">Type of the first value.</typeparam>
		/// <typeparam name="TSecond">Type of the second value.</typeparam>
		/// <param name="first">The first value.</param>
		/// <param name="second">The second value.</param>
		/// <returns>A <see cref="Tuple{TFirst,TSecond}"/>.</returns>
		public static Tuple<TFirst, TSecond> Create<TFirst, TSecond>( TFirst first, TSecond second )
		{
			return new Tuple<TFirst, TSecond>( first, second );
		}
	}
}
