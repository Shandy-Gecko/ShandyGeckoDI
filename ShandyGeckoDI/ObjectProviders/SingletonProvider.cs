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

		public object GetObject(Container container, params Parameter[] parameters)
		{
			if (_isCreated)
			{
				return _instance;
			}

			_instance = container.BuildUpConstructorAndProperties<T>(parameters);
			_isCreated = true;

			return _instance;
		}
	}
}