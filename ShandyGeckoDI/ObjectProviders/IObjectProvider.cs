using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public interface IObjectProvider : IDisposable
	{
		object GetObject(IContext context);
	}
}