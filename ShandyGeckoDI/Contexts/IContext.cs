using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public interface IContext : IDisposable
	{
		int Lifetime { get; }
	}
}