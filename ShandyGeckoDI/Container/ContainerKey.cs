using System;

namespace ShandyGecko.ShandyGeckoDI
{
	public class ContainerKey
	{
		private const int Seed = 781;
		
		public Type Type { get; }
		public string Name { get; private set; }

		public ContainerKey(Type type)
		{
			Type = type;
		}

		public void SetName(string name)
		{
			Name = name;
		}

		public override bool Equals(object obj)
		{
			if (obj is ContainerKey key)
			{
				return key.Name == Name && key.Type == Type;
			}

			return false;
		}

		public override int GetHashCode()
		{
			var typeHashCode = Type == null ? 0 : Type.GetHashCode() + Seed;
			var nameHashCode = string.IsNullOrEmpty(Name) ? 0 : Name.GetHashCode() - Seed;

			return typeHashCode ^ nameHashCode;
		}
	}
}