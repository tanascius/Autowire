using System;
using System.Configuration;

namespace Autowire.Registration.Xml
{
	///<summary>A <see cref="ConfigurationElement"/> for an Autowire <see cref="Container"/>.</summary>
	public class ContainerElement : ConfigurationElement
	{
		///<summary>Returns a collection of <see cref="TypeElement"/>s</summary>
		[ConfigurationProperty( "types", IsDefaultCollection = false )]
		[ConfigurationCollection( typeof( TypeElementCollection ) )]
		public TypeElementCollection Types
		{
			get { return ( TypeElementCollection ) this["types"]; }
		}

		///<summary>The name of the ContainerElement section</summary>
		[ConfigurationProperty( "name" )]
		public string Name
		{
			get { return ( string ) base["name"]; }
		}

		///<summary>Creates a <see cref="Container"/> for this configuration.</summary>
		public IContainer Create( bool throwIfUnableToResolve = false )
		{
			var container = new Container( throwIfUnableToResolve );
			foreach( TypeElement typeConfig in Types )
			{
				var type = Type.GetType( typeConfig.Name, true );
				container.Register.Type( type ).WithScope( typeConfig.Scope );
			}
			return container;
		}
	}
}
