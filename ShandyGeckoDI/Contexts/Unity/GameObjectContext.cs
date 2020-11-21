#if UNITY_5_3_OR_NEWER
using ShandyGecko.LogSystem;
using UnityEngine;

namespace ShandyGecko.ShandyGeckoDI.Unity
{
	/// <summary>
	/// Контекст, который уничтожается при уничтожении объекта
	/// </summary>
	[RequireComponent(typeof(Object))]
	public abstract class GameObjectContext : UnityContext
	{
		[LogFilter] public const string Tag = "GameObjectContext";
		
		private string _objectName;
		
		protected override GeckoContainer Container => UnityContainerProvider.Instance.RootContainer;
		
		protected override void Awake()
		{
			_objectName = name;
			Log.Info(Tag, $"Adding context to object {_objectName}");
			
			base.Awake();
		}

		protected override void OnDestroy()
		{
			Log.Info(Tag, $"Disposing context from object {_objectName}");
			_objectName = null;

			base.OnDestroy();
		}
	}
}
#endif