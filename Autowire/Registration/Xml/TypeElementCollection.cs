using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A collections of <see cref="TypeElement"/>s.</summary>
	public class TypeElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new TypeElement();
		}

		protected override object GetElementKey( ConfigurationElement element )
		{
			return ( ( TypeElement ) element ).Name;
		}

		protected override string ElementName
		{
			get { return "type"; }
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		public TypeElement this[ int index ]
		{
			get { return ( TypeElement ) BaseGet( index ); }
		}
	}
}
