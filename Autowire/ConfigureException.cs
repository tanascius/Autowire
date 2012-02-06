using System;
using Autowire.Utils.Extensions;

namespace Autowire
{
	/// <summary>Is thrown, when a type cannot be registered by a <see cref="Container"/>.</summary>
	[Serializable]
	public sealed class ConfigureException : AutowireException
	{
		/// <summary>Initializes a new instance of the <see cref="ConfigureException" /> class.</summary>
		/// <param name="message">The message that is used for the exception.</param>
		public ConfigureException( string message ) : base( message ) {}

		/// <summary>Initializes a new instance of the <see cref="ConfigureException" /> class.</summary>
		/// <param name="type">The type that could not be configured.</param>
		/// <param name="message">The message that is used for the exception.</param>
		public ConfigureException( Type type, string message ) : base( "The type '{0}' can not be configured.\n{1}".FormatUi( type.Name, message ) ) {}
	}
}
