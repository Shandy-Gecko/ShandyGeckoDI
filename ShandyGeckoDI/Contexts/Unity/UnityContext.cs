#if UNITY_5_3_OR_NEWER
namespace ShandyGecko.ShandyGeckoDI.Unity
{
	public abstract class UnityContext : GeckoBehaviour
	{
		private Context _context;

		protected Context Context => _context ?? (_context = new Context(LifeTime));
		protected abstract GeckoContainer Container { get; }
		protected abstract int LifeTime { get; }

		protected abstract void RegisterTypes(GeckoContainer container, Context context);
		
		protected override void Awake()
		{
			base.Awake();
			RegisterTypes(Container, Context);
		}

		protected virtual void OnDestroy()
		{
			Context.Dispose();
		}
	}
}
#endif