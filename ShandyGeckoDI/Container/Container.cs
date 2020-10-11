using System;
using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI
{
	public class Container : IDisposable
	{
		private readonly Dictionary<ContainerKey, ContainerRegistry> _containerRegistries =
			new Dictionary<ContainerKey, ContainerRegistry>();

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

		public void Dispose()
		{
			//TODO что если контейнер зареган?
			foreach (var value in _containerRegistries.Values)
			{
				value.ObjectProvider.Dispose();
			}
			
			_containerRegistries.Clear();
		}
	}
}