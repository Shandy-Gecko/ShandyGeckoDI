#if UNITY_5_3_OR_NEWER
using System;
using UnityEngine;

namespace ShandyGecko.ShandyGeckoDI.Unity
{
	public class UnityContainerProvider : MonoBehaviour
	{
		private static UnityContainerProvider _instance;
		private GeckoContainer _geckoContainer;

		public GeckoContainer RootContainer => _geckoContainer;
		
		public static UnityContainerProvider Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<UnityContainerProvider>();
				}

				if (_instance == null)
				{
					throw new Exception("Can't get UnityContainerProvider!");
				}

				return _instance;
			}
		}

		public void SetContainer(GeckoContainer geckoContainer)
		{
			if (_geckoContainer != null)
			{
				throw new Exception("Container is already set!");
			}

			_geckoContainer = geckoContainer;
		}
		
	}
}
#endif