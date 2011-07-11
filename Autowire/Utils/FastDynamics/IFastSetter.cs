using System.Diagnostics.CodeAnalysis;

namespace Autowire.Utils.FastDynamics
{
	/// <summary>Interface for property and field FastSetters.</summary>
	public interface IFastSetter
	{
		/// <summary>Set the specified field/property of the given instance to a new value.</summary>
		/// <param name="instance">The instance of which the field/property value will be changed.</param>
		/// <param name="value">The value that will be set.</param>
		[SuppressMessage( "Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set" )]
		void Set( object instance, object value );
	}
}