using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public interface IObjectProvider : IDisposable
	{
		object GetObject(Container container, params Parameter[] parameters);
	}
}