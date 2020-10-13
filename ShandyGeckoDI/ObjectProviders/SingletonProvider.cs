using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public class SingletonProvider<T> : IObjectProvider
	{
		private readonly Func<T> _factoryMethod;
		
		private bool _isCreated;
		private T _instance;

		public SingletonProvider() : this(Activator.CreateInstance<T>)
		{
		}
		
		public SingletonProvider(Func<T> factoryMethod)
		{
			_factoryMethod = factoryMethod;
		}
		
		public void Dispose()
		{
		}

		public object GetObject(IContext context)
		{
			if (_isCreated)
			{
				return _instance;
			}

			_instance = _factoryMethod();
			context.BuildUp(_instance);
			_isCreated = true;

			return _instance;
		}
	}
}