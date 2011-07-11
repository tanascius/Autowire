using System;

namespace Autowire.Factories
{
	/// <summary>Special entry, that holds the <see cref="Container"/>.</summary>
	/// <remarks>This is neccessary for factories, which need the <see cref="Container"/> injected.</remarks>
	internal sealed class SelfFactory : IFactory
	{
		private readonly IContainer m_Container;
		private readonly Type m_Type;

		/// <summary>Initializes a new instance of the <see cref="SelfFactory" /> class.</summary>
		public SelfFactory( IContainer container )
		{
			m_Container = container;
			m_Type = typeof( IContainer );
		}

		/// <summary>true, if this factory was registered by the user, false if it was regeistered automatically.</summary>
		public bool IsRegisteredByUser
		{
			get { return true; }
		}

		/// <summary>Returns the <see cref="Container"/>-</summary>
		/// <param name="container">The container which is used to resolve arguments.</param>
		/// <param name="type">The type that has to be invoked.</param>
		/// <param name="args">Arguments are ignored.</param>
		public object Invoke( IContainer container, Type type, object[] args )
		{
			return m_Container;
		}

		/// <summary>Returns true, when the factory is able to create an instance for the given parameters, otherwise false.</summary>
		public bool CanInvoke( Type type, object[] args )
		{
			return type.IsAssignableFrom( m_Type );
		}

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {}
	}
}