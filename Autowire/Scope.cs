namespace Autowire
{
	/// <summary>The scope in which instances of a type are used and created.</summary>
	public enum Scope
	{
		/// <summary>Every Resolve() call will create a new instance.</summary>
		Instance,

		/// <summary>Every Resolve() call will return the same instance.</summary>
		Singleton,

		/// <summary>For every thread every Resolve() call will return the same instance.</summary>
		SingletonPerThread,

		/// <summary>Every Resolve() call will create a new instance.</summary>
		Default = Instance
	}
}