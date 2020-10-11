using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public class ContainerException : Exception
	{
		public ContainerException(string msg) : base(msg)
		{
		}
	}
}