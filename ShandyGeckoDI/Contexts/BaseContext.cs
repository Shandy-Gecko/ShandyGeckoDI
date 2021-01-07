using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI
{
	public abstract class BaseContext : IContext
	{
		private readonly List<ContainerRegistry> _containerRegistries = new List<ContainerRegistry>();

		public List<ContainerRegistry> ContainerRegistries => _containerRegistries;

		public void Dispose()
		{
			foreach (var registry in _containerRegistries)
			{
				registry.ObjectProvider.Dispose();
				registry.OnContextDispose();
			}
			
			ContainerRegistries.Clear();
		}

		internal void AddRegistry(ContainerRegistry registry)
		{
			_containerRegistries.Add(registry);
		}

		public abstract int Lifetime { get; }
	}
}