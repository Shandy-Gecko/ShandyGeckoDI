using System;
using ShandyGecko.ShandyGeckoDI.Context;

namespace ShandyGecko.ShandyGeckoDI
{
	public class InstanceProvider : IObjectProvider
	{
		private object _instance;
		private bool _isDisposed;
		
		public InstanceProvider(object obj)
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
			_instance = null;
		}

		public object GetObject(BaseContext baseContext)
		{
			return _instance;
		}
	}
}