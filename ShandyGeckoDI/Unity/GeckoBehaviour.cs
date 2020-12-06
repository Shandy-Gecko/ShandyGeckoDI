#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace ShandyGecko.ShandyGeckoDI.Unity
{
	public class GeckoBehaviour : MonoBehaviour
	{
		private GeckoContainer _rootContainer;

		public bool BuiltUp { get; private set; }
		
		protected virtual void Awake()
		{
			BuildUp();
		}

		//TODO использовать для провайдеров
		public void BuildUp(params Parameter[] parameters)
		{
			if (BuiltUp)
				return;

			BuiltUp = true;
			TryGetRootContainer();
			_rootContainer.BuildUp(this, parameters);
		}

		private void TryGetRootContainer()
		{
			if (_rootContainer != null)
				return;

			_rootContainer = UnityContainerProvider.Instance.RootContainer;
		}
	}
}
#endif