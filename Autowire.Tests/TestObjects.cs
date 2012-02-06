namespace Autowire.Tests
{
	internal interface ICommon {}

	internal interface IFoo : ICommon
	{
		IBar Bar { get; }
		string Text { get; set; }
	}

	internal sealed class Foo : IFoo
	{
		public Foo( IBar bar )
		{
			Bar = bar;
		}

		public string Text { get; set; }
		public IBar Bar { get; private set; }
	}

	internal interface IBar : ICommon {}

	internal class Bar : IBar {}

	internal sealed class BarDerived : Bar {}

	internal sealed class BarDerived2 : Bar {}

	internal abstract class AbstractClass {}

	internal sealed class Args1
	{
		public Args1( string arg ) {}
	}

	internal sealed class Args2
	{
		public Args2( string arg1, string arg2 ) {}
	}

	internal sealed class Args3
	{
		public Args3( string arg1, string arg2, string arg3 ) {}
	}

	internal sealed class Args4
	{
		public string Arg1 { get; set; }
		public string Arg2 { get; set; }
		public string Arg3 { get; set; }
		public string Arg4 { get; set; }

		public Args4( string arg1, string arg2, string arg3, string arg4 )
		{
			Arg1 = arg1;
			Arg2 = arg2;
			Arg3 = arg3;
			Arg4 = arg4;
		}
	}
}
