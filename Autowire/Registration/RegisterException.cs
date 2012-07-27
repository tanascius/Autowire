using System;
using Autowire.Utils.Extensions;

namespace Autowire.Registration
{
	/// <summary>Is thrown, when a type cannot be registered by a <see cref="Container"/>.</summary>
	[Serializable]
	public sealed class RegisterException : AutowireException
	{
		/// <summary>Initializes a new instance of the <see cref="RegisterException" /> class.</summary>
		/// <param name="type">The type that could not be registered.</param>
		/// <param name="message">The message that is used for the exception.</param>
		public RegisterException( Type type, string message ) : base( "An instance of '{0}' can not be registered.\n{1}".FormatUi( type.Name, message ) ) {}
	}
}
