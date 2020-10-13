using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public interface IContext : IDisposable
	{
		T BuildUp<T>(T obj);
	}
}