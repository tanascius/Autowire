using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A <see cref="ConfigurationElement"/> for an Autowire <see cref="Argument"/>.</summary>
	public class ArgumentElement : ConfigurationElement
	{
		///<summary>The name of the argument</summary>
		[ConfigurationProperty( "name", IsKey = true, IsRequired = true )]
		public string Name
		{
			get { return ( string ) base["name"]; }
		}

		///<summary>The <see cref="ArgumentType"/> of the argument</summary>
		[ConfigurationProperty( "type" )]
		public ArgumentType Type
		{
			get { return ( ArgumentType )base["type"]; }
		}
	}

	public enum ArgumentType
	{
		Userprovided,
	}
}
