using System;

namespace Autowire.Factories
{
	/// <summary>Common interface for autowire-factories</summary>
	internal interface IFactory : IDisposable
	{
		/// <summary>Creates an instance of the given type.</summary>
		/// <param name="container">The container which is used to resolve arguments.</param>
		/// <param name="type">The type that has to be invoked.</param>
		/// <param name="args">All not-injected arguments for the used constructor.</param>
		object Invoke( IContainer container, Type type, object[] args );

		/// <summary>Returns true, when the factory is able to create an instance for the given parameters, otherwise false.</summary>
		bool CanInvoke( Type type, object[] args );

		/// <summary>true, if this factory was registered by the user, false if it was regeistered automatically.</summary>
		bool IsRegisteredByUser { get; }
	}
}