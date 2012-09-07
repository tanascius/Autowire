using System;
using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A <see cref="ConfigurationElement"/> for an Autowire <see cref="Container"/>.</summary>
	public class ContainerConfig : ConfigurationElement
	{
		///<summary>Returns a collection of <see cref="TypeConfig"/>s</summary>
		[ConfigurationProperty( "types", IsDefaultCollection = false )]
		[ConfigurationCollection( typeof( TypeCollection ) )]
		public TypeCollection Types
		{
			get { return ( TypeCollection ) this["types"]; }
		}

		///<summary>The name of the ContainerConfig section</summary>
		[ConfigurationProperty( "name" )]
		public string Name
		{
			get { return ( string ) base["name"]; }
		}

		///<summary>Creates a <see cref="Container"/> for this configuration.</summary>
		public IContainer Create( bool throwIfUnableToResolve = false )
		{
			var container = new Container( throwIfUnableToResolve );
			foreach( TypeConfig typeConfig in Types )
			{
				var type = Type.GetType( typeConfig.Name, true );
				container.Register.Type( type ).WithScope( typeConfig.Scope );
			}
			return container;
		}
	}
}
