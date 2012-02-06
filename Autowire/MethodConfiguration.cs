using System.Collections.Generic;

namespace Autowire
{
	/// <summary>Holds the configuration of an injected method.</summary>
	internal class MethodConfiguration : IArgumentConfiguration
	{
		private readonly IDictionary<string, Argument> m_Arguments = new Dictionary<string, Argument>();

		public IArgumentConfiguration Argument( Argument argument )
		{
			m_Arguments.Add( argument.ArgumentName, argument );
			return this;
		}

		/// <summary>Contains all configured arguments of method.</summary>
		public IDictionary<string, Argument> Arguments
		{
			get { return m_Arguments; }
		}
	}
}
