using System;

namespace Autowire
{
	/// <summary>Baseclass for all Autowire specific exceptions.</summary>
	[Serializable]
	public abstract class AutowireException : Exception
	{
		/// <summary>Initializes a new instance of the <see cref="AutowireException" /> class.</summary>
		protected AutowireException( string message ) : base( message ) {}

		/// <summary>Initializes a new instance of the <see cref="AutowireException" /> class.</summary>
		protected AutowireException( string message, Exception innerException ) : base( message, innerException ) {}
	}
}
