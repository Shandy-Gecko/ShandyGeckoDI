using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public class FactoryMethodProvider<T> : IObjectProvider
	{
		private readonly Func<T> _factoryMethod;
		
		public FactoryMethodProvider(Func<T> factoryMethod = null)
		{
			_factoryMethod = factoryMethod;
		}

		public Type GetObjectType()
		{
			return typeof(T);
		}

		public object GetObject(GeckoContainer geckoContainer, params Parameter[] parameters)
		{
			if (_factoryMethod == null)
				return geckoContainer.BuildUpType<T>(parameters);

			var createdObj = _factoryMethod();
			return geckoContainer.BuildUp(createdObj, parameters);
		}
		
		public void Dispose()
		{
		}
	}
}