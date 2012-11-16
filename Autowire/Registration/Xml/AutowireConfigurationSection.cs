using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A <see cref="ConfigurationSection"/> for Autowire.</summary>
	public class AutowireConfigurationSection : ConfigurationSection
	{
		///<summary>Returns a collections of <see cref="ContainerElement"/>s.</summary>
		[ConfigurationProperty( "containers", IsDefaultCollection = false )]
		[ConfigurationCollection( typeof( ContainerElementCollection ) )]
		public ContainerElementCollection Containers
		{
			get { return ( ContainerElementCollection ) this["containers"]; }
		}
	}
}
