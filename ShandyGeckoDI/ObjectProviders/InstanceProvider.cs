using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public class InstanceProvider<T> : IObjectProvider
	{
		private T _instance;
		private bool _isDisposed;
		
		public InstanceProvider(T obj)
		{
			_instance = obj;
		}
		
		public Type GetObjectType()
		{
			return typeof(T);
		}
		
		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			if (_instance is IDisposable disposable)
			{
				disposable.Dispose();
			}

			_isDisposed = true;
			_instance = default;
		}

		public object GetObject(GeckoContainer geckoContainer, params Parameter[] parameters)
		{
			return _instance;
		}
	}
}