using System;

namespace Autowire.Factories
{
	/// <summary>A factory that returns always a singleton instance.</summary>
	internal class InstanceFactory : IFactory
	{
		private readonly object m_Instance;
		private readonly Type m_Type;

		/// <summary>Initializes a new instance of the <see cref="InstanceFactory" /> class.</summary>
		public InstanceFactory( object instance )
		{
			m_Instance = instance;
			m_Type = instance.GetType();
		}

		/// <summary>true, if this factory was registered by the user, false if it was regeistered automatically.</summary>
		public bool IsRegisteredByUser
		{
			get { return true; }
		}

		/// <summary>Creates an instance of the given type.</summary>
		/// <param name="container">The container which is used to resolve arguments.</param>
		/// <param name="type">The type that has to be invoked.</param>
		/// <param name="args">All not-injected arguments for the used constructor.</param>
		public object Invoke( IContainer container, Type type, object[] args )
		{
			return m_Instance;
		}

		/// <summary>Returns true, when the factory is able to create an instance for the given parameters, otherwise false.</summary>
		public bool CanInvoke( Type type, object[] args )
		{
			return type.IsAssignableFrom( m_Type );
		}

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			GC.SuppressFinalize( this );
			return;
		}
	}
}