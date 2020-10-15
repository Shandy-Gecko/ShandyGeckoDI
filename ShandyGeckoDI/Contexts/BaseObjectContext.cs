namespace ShandyGecko.ShandyGeckoDI
{
	public abstract class BaseObjectContext : BaseContext
	{
		private Container _container;

		public override Container Container => _container;

		public BaseObjectContext(Container container)
		{
			_container = container;
			
			SetUp();
		}

		protected override void Install()
		{
		}
	}
}