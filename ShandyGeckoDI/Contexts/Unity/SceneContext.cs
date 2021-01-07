#if UNITY_5_3_OR_NEWER
using ShandyGecko.LogSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShandyGecko.ShandyGeckoDI.Unity
{
	/// <summary>
	/// Контекст, который уничтожается при смене сцены
	/// </summary>
	[RequireComponent(typeof(GameObject))]
	public abstract class SceneContext : UnityContext
	{
		[LogFilter] public const string Tag = "SceneContext";
		
		private string _sceneName;

		protected override GeckoContainer Container => UnityContainerProvider.Instance.RootContainer;
		protected override int LifeTime => Container.LifeTimeProvider.SceneLifeTime;

		protected override void Awake()
		{
			//TODO проверить
			_sceneName = SceneManager.GetActiveScene().name;
			Log.Info(Tag, $"Adding context to scene {_sceneName}");

			base.Awake();
		}

		protected override void OnDestroy()
		{
			Log.Info(Tag, $"Disposing context from scene {_sceneName}");
			_sceneName = null;
			
			base.OnDestroy();
		}
	}
}
#endif