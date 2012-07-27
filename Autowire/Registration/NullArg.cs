using System;

namespace Autowire.Registration
{
	/// <summary>Class that wraps null arguments but preserves the type of the argument</summary>
	public sealed class NullArg
	{
		/// <summary>Initializes a new instance of the <see cref="NullArg" /> class.</summary>
		/// <param name="type">The type of the argument.</param>
		internal NullArg( Type type )
		{
			Type = type;
		}

		/// <summary>Gets the type of the argument.</summary>
		internal Type Type { get; private set; }
	}
}
