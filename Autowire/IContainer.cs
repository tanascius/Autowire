using System;
using System.Collections;
using System.Collections.Generic;
using Autowire.Registration;

namespace Autowire
{
	/// <summary>Interface for Autowire-<see cref="Container"/>s.</summary>
	public interface IContainer : IDisposable
	{
		/// <summary>Configures a type.</summary>
		/// <typeparam name="T">The type that is configured.</typeparam>
		/// <returns>The configuration manager of the type.</returns>
		ITypeConfiguration Configure<T>();

		/// <summary>Configures a type.</summary>
		/// <typeparam name="T">The type that is configured.</typeparam>
		/// <param name="name">The name of type that is configured.</param>
		/// <returns>The configuration manager of the type.</returns>
		ITypeConfiguration Configure<T>( string name );

		/// <summary>Configures a type.</summary>
		/// <param name="type">The type that is configured.</param>
		/// <returns>The configuration manager of the type.</returns>
		ITypeConfiguration Configure( Type type );

		/// <summary>Configures a type.</summary>
		/// <param name="type">The type that is configured.</param>
		/// <param name="name">The name of type that is configured.</param>
		/// <returns>The configuration manager of the type.</returns>
		ITypeConfiguration Configure( Type type, string name );

		/// <summary>Returns an instance of <see cref="IRegistrator"/>, which allows to register types, instances and whole assemblies.</summary>
		IRegistrator Register { get; }

		/// <summary>Checks whether a type is already registered.</summary>
		/// <param name="type">The type that is is checked.</param>
		/// <returns>True, when the type is already registered, oserwise false.</returns>
		bool IsRegistered( Type type );

		/// <summary>Checks whether a type is already registered.</summary>
		/// <param name="name">The name under which the type could be registered.</param>
		/// <param name="type">The type that is is checked.</param>
		/// <returns>True, when the type is already registered, oserwise false.</returns>
		bool IsRegistered( string name, Type type );

		/// <summary>Tries to resolves an instance by using the given type and arguments.</summary>
		/// <typeparam name="T">The type of which an instance will be created.</typeparam>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>The resolved instance or null.</returns>
		T Resolve<T>( params object[] args );

		/// <summary>Tries to resolves an instance by using the given type and arguments.</summary>
		/// <param name="type">The type of which an instance will be created.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>The resolved instance or null.</returns>
		object Resolve( Type type, params object[] args );

		/// <summary>Tries to resolves an instance by using the given name, type and arguments.</summary>
		/// <typeparam name="T">The type of which an instance will be created.</typeparam>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>The resolved instance or null.</returns>
		T ResolveByName<T>( string name, params object[] args );

		/// <summary>Tries to resolves an instance by using the given name, type and arguments.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="type">The type of which an instance will be created.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>The resolved instance or null.</returns>
		object ResolveByName( string name, Type type, params object[] args );

		/// <summary>Resolves all possible instance by using the given type and arguments.</summary>
		/// <typeparam name="T">The type of which an instance will be created.</typeparam>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList{T}"/> containing all resolved instances.</returns>
		IList<T> ResolveAll<T>( params object[] args );

		/// <summary>Resolves all possible instance by using the given name, type and arguments.</summary>
		/// <typeparam name="T">The type of which an instance will be created.</typeparam>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList{T}"/> containing all resolved instances.</returns>
		IList<T> ResolveAllByName<T>( string name, params object[] args );

		/// <summary>Resolves all possible instance by using the given type and arguments.</summary>
		/// <param name="type">The type of which an instance will be created.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList{T}"/> containing all resolved instances.</returns>
		IList ResolveAll( Type type, params object[] args );

		/// <summary>Resolves all possible instance by using the given name, type and arguments.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="type">The type of which an instance will be created.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList{T}"/> containing all resolved instances.</returns>
		IList ResolveAllByName( string name, Type type, params object[] args );

		/// <summary>Creates a child container.</summary>
		IContainer CreateChild();
	}
}
