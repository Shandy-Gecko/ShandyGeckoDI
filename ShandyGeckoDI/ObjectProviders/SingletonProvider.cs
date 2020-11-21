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

		public object GetObject(GeckoContainer geckoContainer, params Parameter[] parameters)
		{
			if (_isCreated)
			{
				return _instance;
			}

			_instance = geckoContainer.BuildUpType<T>(parameters);
			_isCreated = true;

			return _instance;
		}
	}
}