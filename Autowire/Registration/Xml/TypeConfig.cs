using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A <see cref="ConfigurationElement"/> for an Autowire type.</summary>
	public class TypeConfig : ConfigurationElement
	{
		///<summary>The name of the type</summary>
		[ConfigurationProperty( "name" )]
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
	}
}
