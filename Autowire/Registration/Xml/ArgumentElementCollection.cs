using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A collections of <see cref="ArgumentElement"/>s.</summary>
	public class ArgumentElementCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new ArgumentElement();
		}

		protected override object GetElementKey( ConfigurationElement element )
		{
			return ( ( ArgumentElement ) element ).Name;
		}

		protected override string ElementName
		{
			get { return "argument"; }
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.BasicMap; }
		}

		public ArgumentElement this[ int index ]
		{
			get { return ( ArgumentElement ) BaseGet( index ); }
		}
	}
}
