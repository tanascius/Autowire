using System;

namespace Autowire
{
	/// <summary>Interface for classes that wrap null arguments but preserve the type of the argument</summary>
	public interface INullArg
	{
		/// <summary>Gets the type of the argument.</summary>
		Type Type { get; }
	}

	/// <summary>Class that wraps null arguments but preserves the type of the argument</summary>
	public sealed class NullArg : INullArg
	{
		/// <summary>Creates a new instance of the <see cref="NullArg" /> class.</summary>
		public static NullArg New<T>()
		{
			return new NullArg( typeof( T ) );
		}

		/// <summary>Creates a new instance of the <see cref="NullArg" /> class.</summary>
		/// <param name="type">The type of the argument.</param>
		public static NullArg New( Type type )
		{
			return new NullArg( type );
		}

		/// <summary>Initializes a new instance of the <see cref="NullArg" /> class.</summary>
		/// <param name="type">The type of the argument.</param>
		private NullArg( Type type )
		{
			Type = type;
		}

		/// <summary>Gets the type of the argument.</summary>
		public Type Type { get; private set; }
	}
}
