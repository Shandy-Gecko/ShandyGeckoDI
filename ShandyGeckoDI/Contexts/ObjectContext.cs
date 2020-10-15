namespace ShandyGecko.ShandyGeckoDI
{
	public class ObjectContext : BaseObjectContext
	{
		public ObjectContext(Container container) : base(container)
		{
		}

		public void InstallDependencies()
		{
			SetUp();
		}
	}
}