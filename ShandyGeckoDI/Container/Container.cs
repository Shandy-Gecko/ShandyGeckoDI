using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShandyGecko.ShandyGeckoDI
{
	public class Container : IDisposable
	{
		private readonly Dictionary<ContainerKey, ContainerRegistry> _containerRegistries =
			new Dictionary<ContainerKey, ContainerRegistry>();

		public ContainerRegistry RegisterProvider<T>(IObjectProvider provider, BaseContext context = null)
		{
			var key = new ContainerKey(typeof(T));
			var value = new ContainerRegistry(provider, this, context);

			_containerRegistries.Add(key, value);
			
			value.AddKey(key);

			return value;
		}

		public bool IsKeyRegistered<T>(string name = "")
		{
			return IsKeyRegistered(new ContainerKey(typeof(T), name));
		}
		
		public bool IsKeyRegistered(ContainerKey containerKey)
		{
			return _containerRegistries.ContainsKey(containerKey);
		}
		
		public ContainerRegistry RegisterInstance<T>(T obj, BaseContext context = null)
		{
			return RegisterProvider<T>(new InstanceProvider<T>(obj), context);
		}
		
		public T BuildUp<T>() where T : new()
		{
			var obj = new T();
			return obj;
		}
		
		public T BuildUp<T>(T obj)
		{
			BuildUp(obj.GetType(), obj);
			return obj;
		}
		
		internal void AddContainerRegistry(ContainerKey key, ContainerRegistry containerRegistry)
		{
			if (IsKeyRegistered(key))
			{
				throw new ContainerException($"Container already has key {key}");
			}
			
			_containerRegistries.Add(key, containerRegistry);
		}

		internal void RemoveContainerRegistry(ContainerKey key)
		{
			if (!IsKeyRegistered(key))
			{
				throw new ContainerException($"Container doesn't have key {key}");
			}

			_containerRegistries.Remove(key);
		}

		private void BuildUp(Type type, object obj)
		{
			if (type.BaseType != typeof(object))
				BuildUp(type.BaseType, obj);
			
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

			foreach (var property in properties)
			{
				var dependencyAttr = property.GetCustomAttributes(typeof(DependencyAttribute), true).FirstOrDefault() as DependencyAttribute;
				if (dependencyAttr == null)
				{
					continue;
				}

				ResolveDependency(obj, type, property, dependencyAttr.Name);
			}
		}

		private void ResolveDependency(object obj, Type type, PropertyInfo propertyInfo, string name)
		{
			var propertyType = propertyInfo.PropertyType;
			var objProvider = GetObjectProviderFromContainer(propertyType, name);

			var createdObj = objProvider.GetObject(this);
			
			var setter = propertyInfo.GetSetMethod(true);
			if (setter == null)
			{
				throw new ContainerException($"Property {propertyInfo} has null setter");
			}
			
			setter.Invoke(obj, new[] {createdObj});
		}

		private IObjectProvider GetObjectProviderFromContainer(Type type, string name)
		{
			var containerKey = new ContainerKey(type, name);
			if (!_containerRegistries.ContainsKey(containerKey))
			{
				throw new ContainerException($"Key {containerKey} isn't registered!");
			}

			var containerRegistry = _containerRegistries[containerKey];

			if (containerRegistry == null)
			{
				throw new ContainerException($"Key {containerKey} has null object provider");
			}

			return containerRegistry.ObjectProvider;
		}

		public void Dispose()
		{
			foreach (var containerRegistry in _containerRegistries.Values)
			{
				containerRegistry.ObjectProvider.Dispose();
			}
		}
	}
}