using System;

namespace Autowire
{
	/// <summary>Handler, that can be used during the registration of an assembly to alter the configuration of single types.</summary>
	/// <param name="type">The type that will be registered.</param>
	/// <param name="typeConfiguration">The <see cref="ITypeConfiguration"/> that is currently used and can be altered..</param>
	public delegate void RegistrationHandler( Type type, ITypeConfiguration typeConfiguration );
}
