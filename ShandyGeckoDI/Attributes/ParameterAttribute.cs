using System;

namespace ShandyGecko.ShandyGeckoDI
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ParameterAttribute : Attribute
	{
		public string Name { get; private set; }
		
		public ParameterAttribute(string name)
		{
			Name = name;
		}
		
		public ParameterAttribute() : this("") {}
	}
}