using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A collections of <see cref="ContainerConfig"/>s.</summary>
	public class ContainerCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new ContainerConfig();
		}

		protected override object GetElementKey( ConfigurationElement element )
		{
			return ( ( ContainerConfig ) element ).Name;
		}

		protected override string ElementName
		{
			get { return "container"; }
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		public ContainerConfig this[ int index ]
		{
			get { return ( ContainerConfig ) BaseGet( index ); }
		}

		public new ContainerConfig this[ string name ]
		{
			get { return ( ContainerConfig ) BaseGet( name ); }
		}
	}
}
