using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A <see cref="ConfigurationElement"/> for an Autowire type.</summary>
	public class TypeElement : ConfigurationElement
	{
		///<summary>The name of the type</summary>
		[ConfigurationProperty( "name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return ( string ) base["name"]; }
		}

		///<summary>The <see cref="Scope"/> of the type</summary>
		[ConfigurationProperty( "scope" )]
		public Scope Scope
		{
			get { return ( Scope ) base["scope"]; }
		}

		///<summary>Returns a collection of <see cref="ArgumentElement"/>s</summary>
		[ConfigurationProperty( "ctor", IsDefaultCollection = false )]
		[ConfigurationCollection( typeof( ArgumentElementCollection ) )]
		public ArgumentElementCollection ConstructorArguments
		{
			get { return ( ArgumentElementCollection ) this["ctor"]; }
		}
	}
}
