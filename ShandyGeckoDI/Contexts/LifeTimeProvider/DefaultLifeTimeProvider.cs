namespace ShandyGecko.ShandyGeckoDI.LifeTimeProvider
{
	public class DefaultLifeTimeProvider : ILifeTimeProvider
	{
		//TODO сделать счетчик
#if UNITY_5_3_OR_NEWER
		public int ProjectLifeTime => 1000;
		public int SceneLifeTime => 2000;
		public int GameObjectLifeTime => 3000;
#endif
	}
}