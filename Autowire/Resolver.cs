using System.Collections.Generic;

namespace Autowire
{
	/// <summary>Marker interface for the Resolver{T}</summary>
	internal interface IResolver {}

	/// <summary>Factory-like class, that resolves instances by using the <see cref="Container"/></summary>
	/// <typeparam name="T">Type, which has to be resolved.</typeparam>
	public class Resolver<T> : IResolver
	{
		private readonly IContainer m_Container;

		/// <summary>Initializes a new instance of the <see cref="Resolver{T}" /> class.</summary>
		/// <remarks>Instances of this class are only injected and never created manually.</remarks>
		protected Resolver( IContainer container )
		{
			m_Container = container;
		}

		/// <summary>Resolves an instance of <typeparamref name="T"/>.</summary>
		/// <param name="args">The arguments needed to resolve the instance.</param>
		public T Resolve( params object[] args )
		{
			return m_Container.Resolve<T>( args );
		}

		/// <summary>Resolves an instance of <typeparamref name="TResolve"/>.</summary>
		/// <param name="args">The arguments needed to resolve the instance.</param>
		public TResolve Resolve<TResolve>( params object[] args ) where TResolve : T
		{
			return m_Container.Resolve<TResolve>( args );
		}

		/// <summary>Resolves an instance of <typeparamref name="T"/>.</summary>
		/// <param name="name">The name under which the type was registered.</param>
		/// <param name="args">The arguments needed to resolve the instance.</param>
		public T ResolveByName( string name, params object[] args )
		{
			return m_Container.ResolveByName<T>( name, args );
		}

		/// <summary>Resolves an instance of <typeparamref name="TResolve"/>.</summary>
		/// <param name="name">The name under which the type was registered.</param>
		/// <param name="args">The arguments needed to resolve the instance.</param>
		public TResolve ResolveByName<TResolve>( string name, params object[] args ) where TResolve : T
		{
			return m_Container.ResolveByName<TResolve>( name, args );
		}

		/// <summary>Resolves all possible instance by using the given type and arguments.</summary>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList{T}"/> containing all resolved instances.</returns>
		public IList<T> ResolveAll( params object[] args )
		{
			return m_Container.ResolveAll<T>( args );
		}

		/// <summary>Resolves all possible instance by using the given name, type and arguments.</summary>
		/// <param name="name">The name that is used to identify the type of an instance.</param>
		/// <param name="args">The argumente that are used for construction.</param>
		/// <returns>A <see cref="IList{T}"/> containing all resolved instances.</returns>
		public IList<T> ResolveAllByName( string name, params object[] args )
		{
			return m_Container.ResolveAll<T>( name, args );
		}
	}
}