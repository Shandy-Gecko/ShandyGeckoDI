using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI
{
	public class ContainerRegistry
	{
		private readonly HashSet<ContainerKey> _keys = new HashSet<ContainerKey>();

		private Container _container;
		
		public BaseContext Context { get; private set; }
		public bool IsNonLazy { get; private set; }
		public IObjectProvider ObjectProvider { get; }

		public ContainerRegistry(IObjectProvider objectProvider, Container container, BaseContext context = null)
		{
			ObjectProvider = objectProvider;
			_container = container;
			Context = context;

			TryAddRegistryToContext();
		}

		public void AddKey(ContainerKey key)
		{
			if (_keys.Contains(key))
			{
				throw new ContainerException($"Trying to add duplicate ContainerKey {key}!");
			}

			_keys.Add(key);
		}

		public ContainerRegistry As<T>()
		{
			var key = new ContainerKey(typeof(T));
			
			AddKey(key);
			_container.AddContainerRegistry(key, this);

			return this;
		}

		public ContainerRegistry WithName(string name)
		{
			foreach (var key in _keys)
			{
				_container.RemoveContainerRegistry(key);
				key.SetName(name);
				_container.AddContainerRegistry(key, this);
			}
			
			return this;
		}

		public ContainerRegistry NonLazy()
		{
			IsNonLazy = true;	
			return this;
		}

		public ContainerRegistry SetContext(BaseContext context)
		{
			Context = context;
			TryAddRegistryToContext();

			return this;
		}

		internal void OnContextDispose()
		{
			foreach (var key in _keys)
			{
				_container.RemoveContainerRegistry(key);
			}
		}

		private void TryAddRegistryToContext()
		{
			Context?.AddRegistry(this);
		}
		
		//TODO добавить методы по подстановке в атрибут параметра или конструктор
	}
}