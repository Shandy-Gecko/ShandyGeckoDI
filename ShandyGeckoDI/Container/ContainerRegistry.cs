using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI
{
	public class ContainerRegistry
	{
		private HashSet<ContainerKey> _keys = new HashSet<ContainerKey>();

		public bool IsNonLazy { get; private set; }
		public IEnumerable<ContainerKey> ContainerKeys => _keys;
		public IObjectProvider ObjectProvider { get; }
		
		public ContainerRegistry(IObjectProvider objectProvider)
		{
			ObjectProvider = objectProvider;
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
			
			return this;
		}

		public ContainerRegistry SetName(string name)
		{
			foreach (var key in _keys)
			{
				key.SetName(name);
			}
			
			return this;
		}

		public ContainerRegistry NonLazy()
		{
			IsNonLazy = true;	
			return this;
		}
	}
}