using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A collections of <see cref="TypeConfig"/>s.</summary>
	public class TypeCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new TypeConfig();
		}

		protected override object GetElementKey( ConfigurationElement element )
		{
			return ( ( TypeConfig ) element ).Name;
		}

		protected override string ElementName
		{
			get { return "type"; }
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		public TypeConfig this[ int index ]
		{
			get { return ( TypeConfig ) BaseGet( index ); }
		}
	}
}
