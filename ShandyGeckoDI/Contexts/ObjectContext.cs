namespace ShandyGecko.ShandyGeckoDI
{
	public abstract class ObjectContext : BaseContext
	{
		private Container _container;

		public override Container Container => _container;

		public ObjectContext(Container container)
		{
			_container = container;
			
			SetUp();
		}
	}
}