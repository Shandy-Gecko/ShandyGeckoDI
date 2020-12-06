#if UNITY_5_3_OR_NEWER
using System;
using System.Linq;
using ShandyGecko.ShandyGeckoDI.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShandyGecko.ShandyGeckoDI
{
	public class FindObjectByPathProvider<T> : IObjectProvider where T : Object
	{
		private T _instance;
		private string _path;

		public FindObjectByPathProvider(string path)
		{
			_path = path;
		}
		
		public object GetObject(GeckoContainer geckoContainer, params Parameter[] parameters)
		{
			if (!IsDestroyed())
				return _instance;

			_instance = FindObject();

			// TODO возможно понадобится BuildUp компонентов не-GeckoBehaviour
			if (!(_instance is GeckoBehaviour geckoBehaviour))
				return _instance;

			if (!geckoBehaviour.BuiltUp)
			{
				geckoBehaviour.BuildUp(parameters);
			}

			return _instance;
		}
		
		public void Dispose()
		{
			if (IsDestroyed())
				return;
			
			if (_instance is IDisposable disposable)
				disposable.Dispose();

			_instance = default;
		}

		private bool IsDestroyed()
		{
			if (_instance == null)
				return true;
			
			if (typeof(T) == typeof(GameObject))
			{
				var gameObj = (GameObject) (object) _instance;
				return !gameObj;
			}
			
			var component = _instance as Component;
			if (component) return !component.gameObject;

			return true;
		}

		private T FindObject()
		{
			var obj = GameObject.Find(_path);

			if (obj == null)
				throw new ContainerException($"Can't find object of type {typeof(T)} and path {_path}");

			if (typeof(T) == typeof(GameObject))
				return (T) (object) obj;

			var component = obj.GetComponents<Component>().OfType<T>().FirstOrDefault();
			if (component == null)
				throw new ContainerException($"Can't find object of type {typeof(T)} and path {_path}");
			
			return component;
		}
	}
}
#endif