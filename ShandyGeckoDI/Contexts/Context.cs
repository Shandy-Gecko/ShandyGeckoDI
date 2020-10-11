using System;
using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI.Context
{
	public abstract class Context : IDisposable
	{
		private List<ContainerRegistry> _containerRegistries = new List<ContainerRegistry>();

		private Container _container;

		public Container Container => _container;

		public ContainerRegistry RegisterProvider<T>(IObjectProvider provider)
		{
			var key = new ContainerKey(typeof(T));
			var value = new ContainerRegistry(provider);

			_containerRegistries.Add(value);
			
			value.AddKey(key);

			return value;
		}

		public void SetUp(Container container)
		{
			_container = container;
			
			PreInstall();
			Install();
			PostInstall();
		}

		public void Dispose()
		{
			foreach (var registry in _containerRegistries)
			{
				registry.ObjectProvider.Dispose();
			}
		}

		protected abstract void Install();

		private void PreInstall()
		{
			
		}

		private void PostInstall()
		{
			foreach (var containerValue in _containerRegistries)
			{
				_container.AddContainerRegistry(containerValue);
			}
		}
	}
}