using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public class LifeTimeException : Exception
	{
		public LifeTimeException(string message) : base(message) {}
	}
}