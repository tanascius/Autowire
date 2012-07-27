using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Autowire.Registration
{
	/// <summary>Interface for classes that allow to register types, instances and whole assemblies.</summary>
	public interface IRegistrator
	{
#if DEBUG
		/// <summary>Gets the count of all registered types.</summary>
		int Count { get; }
#endif

		///<summary>Registrationhandlers are called during the configuration of each type.</summary>
		/// <remarks>Especially useful for registration of whole assemblies to exclude some types.</remarks>
		event Action<Type, ITypeConfiguration> RegistrationHandler;

		/// <summary>Registers a type.</summary>
		/// <typeparam name="T">The type, that will be registered.</typeparam>
		ILazyConfiguration Type<T>() where T : class;

		/// <summary>Registers a type under the given name.</summary>
		/// <typeparam name="T">The type, that will be registered.</typeparam>
		/// <param name="name">The name that is used to identify the type during instantiation.</param>
		ILazyConfiguration Type<T>( string name ) where T : class;

		/// <summary>Registers a type.</summary>
		/// <param name="type">The type, that will be registered.</param>
		[SuppressMessage( "Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#" )]
		ILazyConfiguration Type( Type type );

		/// <summary>Registers a type.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="type">The type, that will be registered.</param>
		[SuppressMessage( "Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "1#" )]
		ILazyConfiguration Type( string name, Type type );

		/// <summary>Registers an instance as a singleton.</summary>
		/// <param name="instance">The instance that will be registered.</param>
		[SuppressMessage( "Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#" )]
		void Instance( object instance );

		/// <summary>Registers an instance as a singleton.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="instance">The instance that will be registered.</param>
		[SuppressMessage( "Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "1#" )]
		void Instance( string name, object instance );

		/// <summary>Registers all types of an assembly.</summary>
		[SuppressMessage( "Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#" )]
		void Assembly( Assembly assembly );

		/// <summary>Registers all types of an assembly.</summary>
		/// <param name="assembly">The assembly to be registered.</param>
		/// <param name="registrationHandler">Is called for every type, so that the type's scope can be set.</param>
		[SuppressMessage( "Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#" )]
		void Assembly( Assembly assembly, Action<Type, ITypeConfiguration> registrationHandler );

		/// <summary>Registers all types of an assembly.</summary>
		/// <param name="name">The name of the assembly.</param>
		void AssemblyByName( string name );

		/// <summary>Registers all types of an assembly.</summary>
		/// <param name="name">The name of the assembly.</param>
		/// <param name="registrationHandler">Is called for every type, so that the type's scope can be set.</param>
		void AssemblyByName( string name, Action<Type, ITypeConfiguration> registrationHandler );

		/// <summary>Registers all types of an assembly, when the assembly can be loaded.</summary>
		/// <param name="name">The name of the assembly.</param>
		bool TryAssemblyByName( string name );

		/// <summary>Registers all types of an assembly, when the assembly can be loaded.</summary>
		/// <param name="name">The name of the assembly.</param>
		/// <param name="registrationHandler">Is called for every type, so that the type's scope can be set.</param>
		bool TryAssemblyByName( string name, Action<Type, ITypeConfiguration> registrationHandler );
	}
}
