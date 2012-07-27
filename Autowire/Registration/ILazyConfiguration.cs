using System;

namespace Autowire.Registration
{
	/// <summary>Allows the configuration of scope and a callback.</summary>
	public interface ILazyConfiguration
	{
		/// <summary>Sets the scope of the type.</summary>
		/// <param name="scope">The <see cref="Scope"/> that is used for this type.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		ILazyConfiguration WithScope( Scope scope );

		/// <summary>Installs a callback, that is called after the each resolution.</summary>
		/// <param name="callback">The callback that is used.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		ILazyConfiguration AfterResolve( Action<IContainer, object> callback );
	}
}