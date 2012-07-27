using System;

namespace Autowire.Registration
{
	/// <summary>Allows the configuration of a type.</summary>
	public interface ITypeConfiguration : IMembersConfiguration, ILazyConfiguration, IArgumentConfiguration
	{
		/// <summary>Returns the <see cref="Type"/> that is configured.</summary>
		/// <remarks>Useful for extension methods.</remarks>
		Type Type { get; }

		/// <summary>Sets the type to be ignored during registration.</summary>
		/// <remarks>The type can not be registered anymore after calling this function.</remarks>
		void Ignore();

		///<summary>Injection (of fields, properties, methods) will be done for the given component.</summary>
		///<param name="componentName">The name of the component.</param>
		///<returns>A <see cref="IMembersConfiguration"/> for further configuration of the component.</returns>
		IMembersConfiguration InjectForComponent( string componentName );

		#region IArgumentConfiguration
		/// <summary>Adds an argument for the type.</summary>
		/// <param name="argument">The argument that is used during construction.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		new ITypeConfiguration Arguments( params Argument[] argument );
		#endregion

		#region ILazyConfiguration
		/// <summary>Sets the scope of the type.</summary>
		/// <param name="scope">The <see cref="Scope"/> that is used for this type.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		new ITypeConfiguration WithScope( Scope scope );

		/// <summary>Installs a callback, that is called after the each resolution.</summary>
		/// <param name="callback">The callback that is used.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		new ITypeConfiguration AfterResolve( Action<IContainer, object> callback );
		#endregion

		#region IMembersConfiguration
		/// <summary>The given property will be injected during type resolution.</summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		new ITypeConfiguration InjectProperty( string propertyName );

		/// <summary>The given property will be injected during type resolution.</summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="injectedName">The name under which the injected type is registered.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		new ITypeConfiguration InjectProperty( string propertyName, string injectedName );

		/// <summary>The given field will be injected during type resolution.</summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		new ITypeConfiguration InjectField( string fieldName );

		/// <summary>The given field will be injected during type resolution.</summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="injectedName">The name under which the injected type is registered.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		new ITypeConfiguration InjectField( string fieldName, string injectedName );

		/// <summary>The given method will be injected during type resolution.</summary>
		/// <param name="methodName">The name of the method.</param>
		/// <returns>An <see cref="IArgumentConfiguration"/> for configuration of the method's arguments.</returns>
		new IArgumentConfiguration InjectMethod( string methodName );
		#endregion
	}
}
