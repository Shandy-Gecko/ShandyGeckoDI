using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI
{
	public abstract class BaseContext : IContext
	{
		private readonly List<ContainerRegistry> _containerRegistries = new List<ContainerRegistry>();

		public void Dispose()
		{
			foreach (var registry in _containerRegistries)
			{
				registry.ObjectProvider.Dispose();
				registry.OnContextDispose();
			}
		}

		internal void AddRegistry(ContainerRegistry registry)
		{
			_containerRegistries.Add(registry);
		}
	}
}