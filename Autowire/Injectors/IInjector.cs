namespace Autowire.Injectors
{
	/// <summary>Interface for field, property or method injectors</summary>
	internal interface IInjector
	{
		/// <summary>Inject the field/property-values or method-arguments for the given instance.</summary>
		void Inject( object instance );
	}
}