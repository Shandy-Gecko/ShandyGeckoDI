using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI
{
	public class Container
	{
		private readonly Dictionary<ContainerKey, ContainerRegistry> _containerRegistries =
			new Dictionary<ContainerKey, ContainerRegistry>();

		public ContainerRegistry GetContainerRegistry(ContainerKey key)
		{
			return _containerRegistries[key];
		}
		
		public bool IsKeyRegistered(ContainerKey key)
		{
			return _containerRegistries.ContainsKey(key);
		}

		public void AddContainerRegistry(ContainerRegistry registry)
		{
			foreach (var key in registry.ContainerKeys)
			{
				if (_containerRegistries.ContainsKey(key))
				{
					throw new ContainerException($"Container already has key with value {key}");
				}
				
				_containerRegistries.Add(key, registry);
			}
		}

		public void RemoveContainerRegistry(ContainerRegistry registry)
		{
			foreach (var key in registry.ContainerKeys)
			{
				if (!_containerRegistries.ContainsKey(key))
				{
					//TODO Error ?
					continue;
				}

				_containerRegistries.Remove(key);
			}
		}
	}
}