using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public interface IObjectProvider : IDisposable
	{
		object GetObject(Context.BaseContext baseContext);
	}
}