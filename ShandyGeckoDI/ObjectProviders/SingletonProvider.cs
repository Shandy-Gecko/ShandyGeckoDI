using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public class SingletonProvider<T> : IObjectProvider
	{
		private bool _isCreated;
		private T _instance;
		
		public Type GetObjectType()
		{
			return typeof(T);
		}

		public void Dispose()
		{
			if (!_isCreated)
				return;
			
			if (_instance is IDisposable disposable)
				disposable.Dispose();

			_isCreated = false;
			_instance = default;
		}

		public object GetObject(GeckoContainer geckoContainer, params Parameter[] parameters)
		{
			if (_isCreated)
			{
				return _instance;
			}

			_instance = geckoContainer.BuildUpType<T>(parameters);
			_isCreated = true;

			return _instance;
		}
	}
}