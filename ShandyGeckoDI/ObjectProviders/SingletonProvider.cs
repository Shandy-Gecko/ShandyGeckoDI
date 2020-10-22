namespace ShandyGecko.ShandyGeckoDI
{
	public class SingletonProvider<T> : IObjectProvider
	{
		private bool _isCreated;
		private T _instance;
		private BaseContext _context;

		public SingletonProvider(BaseContext context)
		{
			_context = context;
		}

		public void Dispose()
		{
		}

		public object GetObject(Container container)
		{
			if (_isCreated)
			{
				return _instance;
			}

			//TODO передать список параметров для этого провайдера
			_instance = container.BuildUpConstructor<T>();
			container.BuildUpProperties(_instance);
			_isCreated = true;

			return _instance;
		}
	}
}