namespace ShandyGecko.ShandyGeckoDI.LifeTimeProvider
{
	public interface ILifeTimeProvider
	{
#if UNITY_5_3_OR_NEWER
		int ProjectLifeTime { get; }
		int SceneLifeTime { get; }
		int GameObjectLifeTime { get; }
#endif
	}
}