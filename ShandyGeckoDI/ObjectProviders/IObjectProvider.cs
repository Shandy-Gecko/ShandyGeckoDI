using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public interface IObjectProvider : IDisposable
	{
		Type GetObjectType();
		
		object GetObject(GeckoContainer geckoContainer, params Parameter[] parameters);
	}
}