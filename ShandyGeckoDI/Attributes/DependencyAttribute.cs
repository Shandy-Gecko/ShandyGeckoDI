using System;

namespace ShandyGecko.ShandyGeckoDI
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false)]
	public class DependencyAttribute : Attribute
	{
		public string Name { get; private set; }
		
		public DependencyAttribute(string name)
		{
			Name = name;
		}
		
		public DependencyAttribute() : this("") {}
	}
}