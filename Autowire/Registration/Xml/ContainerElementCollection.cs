using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A collections of <see cref="ContainerElement"/>s.</summary>
	public class ContainerElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new ContainerElement();
		}

		protected override object GetElementKey( ConfigurationElement element )
		{
			return ( ( ContainerElement ) element ).Name;
		}

		protected override string ElementName
		{
			get { return "container"; }
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		public ContainerElement this[ int index ]
		{
			get { return ( ContainerElement ) BaseGet( index ); }
		}

		public new ContainerElement this[ string name ]
		{
			get { return ( ContainerElement ) BaseGet( name ); }
		}
	}
}
