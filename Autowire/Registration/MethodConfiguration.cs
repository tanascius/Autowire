using System.Collections.Generic;

namespace Autowire.Registration
{
	/// <summary>Holds the configuration of an injected method.</summary>
	internal class MethodConfiguration : IArgumentConfiguration
	{
		private readonly IDictionary<string, Argument> m_Arguments = new Dictionary<string, Argument>();

		IArgumentConfiguration IArgumentConfiguration.Arguments( params Argument[] arguments )
		{
			foreach( var argument in arguments )
			{
				m_Arguments.Add( argument.ArgumentName, argument );
			}
			return this;
		}

		/// <summary>Contains all configured arguments of method.</summary>
		public IDictionary<string, Argument> Arguments
		{
			get { return m_Arguments; }
		}
	}
}
