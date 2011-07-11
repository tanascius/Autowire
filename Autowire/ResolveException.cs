using System;
using Autowire.Utils.Extensions;

namespace Autowire
{
	/// <summary>Is thrown, when a type cannot be resolved by a <see cref="Container"/>.</summary>
	[Serializable]
	public sealed class ResolveException : AutowireException
	{
		/// <summary>Initializes a new instance of the <see cref="ResolveException" /> class.</summary>
		/// <param name="type">The type that could not be resolved.</param>
		/// <param name="message">The message that is used for the exception.</param>
		public ResolveException( Type type, string message ) : base( "An instance of '{0}' can not be created.\n{1}".FormatUi( type.Name, message ) ) {}
	}
}