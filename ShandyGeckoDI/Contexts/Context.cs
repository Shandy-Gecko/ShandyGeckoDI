namespace ShandyGecko.ShandyGeckoDI
{
	public class Context : BaseContext
	{
		public override int Lifetime { get; }

		public Context()
		{
			Lifetime = 0;
		}
		
		public Context(int lifetime)
		{
			Lifetime = lifetime;
		}
	}
}