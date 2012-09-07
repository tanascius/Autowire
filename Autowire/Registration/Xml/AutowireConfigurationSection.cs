using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A <see cref="ConfigurationSection"/> for Autowire.</summary>
	public class AutowireConfigurationSection : ConfigurationSection
	{
		///<summary>Returns a collections of <see cref="ContainerConfig"/>s.</summary>
		[ConfigurationProperty( "containers", IsDefaultCollection = false )]
		[ConfigurationCollection( typeof( ContainerCollection ) )]
		public ContainerCollection Containers
		{
			get { return ( ContainerCollection ) this["containers"]; }
		}
	}
}
