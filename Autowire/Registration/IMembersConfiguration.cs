namespace Autowire.Registration
{
	/// <summary>Allows the configuration of fields, properties and methods.</summary>
	public interface IMembersConfiguration
	{
		/// <summary>The given property will be injected during type resolution.</summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		IMembersConfiguration InjectProperty( string propertyName );

		/// <summary>The given property will be injected during type resolution.</summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="injectedName">The name under which the injected type is registered.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		IMembersConfiguration InjectProperty( string propertyName, string injectedName );

		/// <summary>The given field will be injected during type resolution.</summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		IMembersConfiguration InjectField( string fieldName );

		/// <summary>The given field will be injected during type resolution.</summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="injectedName">The name under which the injected type is registered.</param>
		/// <returns>The <see cref="ITypeConfiguration"/> for further configuration.</returns>
		IMembersConfiguration InjectField( string fieldName, string injectedName );

		/// <summary>The given method will be injected during type resolution.</summary>
		/// <param name="methodName">The name of the method.</param>
		/// <returns>An <see cref="IArgumentConfiguration"/> for configuration of the method's arguments.</returns>
		IArgumentConfiguration InjectMethod( string methodName );
	}
}