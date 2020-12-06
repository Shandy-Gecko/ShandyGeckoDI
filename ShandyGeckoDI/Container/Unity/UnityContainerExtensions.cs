#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace ShandyGecko.ShandyGeckoDI
{
	public static class UnityContainerExtensions
	{
		public static ContainerRegistry RegisterSceneObject<T>(this GeckoContainer geckoContainer, string path, BaseContext context = null) where T : Object
		{
			return geckoContainer.RegisterProvider<T>(new FindObjectByPathProvider<T>(path), context);
		}

		public static ContainerRegistry RegisterUnityObject<T>(this GeckoContainer geckoContainer, BaseContext context = null) where T : Object
		{
			return geckoContainer.RegisterProvider<T>(new FindObjectProvider<T>(), context);
		}
	}
}
#endif