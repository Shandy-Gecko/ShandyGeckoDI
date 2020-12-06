namespace ShandyGecko.ShandyGeckoDI
{
	public static class ContainerExtensions
	{
		public static ContainerRegistry RegisterInstance<T>(this GeckoContainer geckoContainer, T obj, BaseContext context = null)
		{
			return geckoContainer.RegisterProvider<T>(new InstanceProvider<T>(obj), context);
		}

		public static ContainerRegistry RegisterSingletone<T>(this GeckoContainer geckoContainer, BaseContext context = null)
		{
			return geckoContainer.RegisterProvider<T>(new SingletonProvider<T>(), context);
		}

		public static ContainerRegistry RegisterType<T>(this GeckoContainer geckoContainer, BaseContext context = null)
		{
			return geckoContainer.RegisterProvider<T>(new FactoryMethodProvider<T>(), context);
		}
	}
}