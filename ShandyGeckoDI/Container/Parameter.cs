using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public class Parameter
	{
		public object Object { get; }
		public Type Type { get; }
		public string Name { get; }

		public Parameter(object obj, string name = "")
		{
			Object = obj;
			Type = obj.GetType();
			Name = name;
		}

		public Parameter(object obj, Type type, string name = "")
		{
			Object = obj;
			Type = type;
			Name = name;
		}
	}
}