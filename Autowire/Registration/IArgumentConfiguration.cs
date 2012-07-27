using System.Diagnostics.CodeAnalysis;

namespace Autowire.Registration
{
	/// <summary>Allows the configuration of arguments.</summary>
	public interface IArgumentConfiguration
	{
		/// <summary>Adds an argument for the type.</summary>
		/// <param name="argument">The argument that is used during construction.</param>
		/// <returns>An <see cref="IArgumentConfiguration"/> to add other arguments.</returns>
		[SuppressMessage( "Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#" )]
		IArgumentConfiguration Arguments( params Argument[] argument );
	}
}