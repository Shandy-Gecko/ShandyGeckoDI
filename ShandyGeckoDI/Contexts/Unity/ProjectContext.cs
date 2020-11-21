#if UNITY_5_3_OR_NEWER
using ShandyGecko.LogSystem;
using UnityEngine;

namespace ShandyGecko.ShandyGeckoDI.Unity
{
	/// <summary>
	/// Контекст, который унчтожается при выключении игры
	/// </summary>
	[RequireComponent(typeof(GameObject))]
	public abstract class ProjectContext : UnityContext
	{
		[LogFilter] public string Tag = "ProjectContext";

		private GeckoContainer _geckoContainer;
		
		protected abstract string ProjectName { get; }

		protected override GeckoContainer Container => _geckoContainer ?? (_geckoContainer = new GeckoContainer());

		protected override void Awake()
		{
			DontDestroyOnLoad(gameObject);
			
			Log.Info(Tag, $"Adding context to project {ProjectName}");

			RegisterTypes(Container, Context);	
		}

		protected override void OnDestroy()
		{
			Log.Info(Tag, $"Disposing context from project {ProjectName}");
			
			base.OnDestroy();
		}
	}
}
#endif