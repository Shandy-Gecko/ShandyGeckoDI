using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public interface IObjectProvider : IDisposable
	{
		object GetObject(GeckoContainer geckoContainer, params Parameter[] parameters);
	}
}