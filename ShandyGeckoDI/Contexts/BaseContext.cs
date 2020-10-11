using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI.Context
{
	public abstract class BaseContext : IContext
	{
		private readonly List<ContainerRegistry> _containerRegistries = new List<ContainerRegistry>();

		public abstract Container Container { get; }

		public ContainerRegistry RegisterProvider<T>(IObjectProvider provider)
		{
			var key = new ContainerKey(typeof(T));
			var value = new ContainerRegistry(provider);

			_containerRegistries.Add(value);
			
			value.AddKey(key);

			return value;
		}

		public ContainerRegistry RegisterInstance<T>(T obj)
		{
			return RegisterProvider<T>(new InstanceProvider(obj));
		}

		public void Dispose()
		{
			foreach (var registry in _containerRegistries)
			{
				Container.RemoveContainerRegistry(registry);
				registry.ObjectProvider.Dispose();
			}
		}

		protected void SetUp()
		{
			PreInstall();
			Install();
			PostInstall();
		}
		
		/// <summary>
		/// Override to register types
		/// </summary>
		protected abstract void Install();

		protected virtual void PreInstall()
		{
		}

		private void PostInstall()
		{
			foreach (var containerValue in _containerRegistries)
			{
				Container.AddContainerRegistry(containerValue);
			}
		}
	}
}