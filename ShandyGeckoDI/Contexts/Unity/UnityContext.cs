#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace ShandyGecko.ShandyGeckoDI.Unity
{
	//TODO заменить на GeckoBehaviour
	public abstract class UnityContext : MonoBehaviour
	{
		private Context _context;

		protected Context Context => _context ?? (_context = new Context());
		protected abstract GeckoContainer Container { get; }

		protected abstract void RegisterTypes(GeckoContainer container, Context context);
		
		protected virtual void Awake()
		{
			RegisterTypes(Container, Context);
		}

		protected virtual void OnDestroy()
		{
			Context.Dispose();
		}
	}
}
#endif