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

		public object GetObject(IContext context)
		{
			return _instance;
		}
	}
}