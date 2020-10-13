using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShandyGecko.ShandyGeckoDI
{
	public abstract class BaseContext : IContext
	{
		private readonly List<ContainerRegistry> _containerRegistries = new List<ContainerRegistry>();

		public abstract Container Container { get; }

		public ContainerRegistry RegisterProvider<T>(IObjectProvider provider)
		{
			var key = new ContainerKey(typeof(T));
			var value = new ContainerRegistry(provider);

			_containerRegistries.Add(value);
			
			value.AddKey(key);

			return value;
		}

		public ContainerRegistry RegisterInstance<T>(T obj)
		{
			return RegisterProvider<T>(new InstanceProvider<T>(obj));
		}
		
		public T BuildUp<T>() where T : new()
		{
			var obj = new T();
			return obj;
		}

		public void Dispose()
		{
			foreach (var registry in _containerRegistries)
			{
				Container.RemoveContainerRegistry(registry);
				registry.ObjectProvider.Dispose();
			}
		}

		public T BuildUp<T>(T obj)
		{
			BuildUp(obj.GetType(), obj);
			return obj;
		}

		protected void SetUp()
		{
			PreInstall();
			Install();
			PostInstall();
		}
		
		/// <summary>
		/// Override to register types
		/// </summary>
		protected abstract void Install();

		protected virtual void PreInstall()
		{
		}

		private void PostInstall()
		{
			foreach (var containerValue in _containerRegistries)
			{
				Container.AddContainerRegistry(containerValue);
			}
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
			if (!Container.IsKeyRegistered(containerKey))
			{
				throw new ContainerException($"Key {containerKey} isn't registered!");
			}

			var containerRegistry = Container.GetContainerRegistry(containerKey);

			if (containerRegistry == null)
			{
				throw new ContainerException($"Key {containerKey} has null object provider");
			}

			return containerRegistry.ObjectProvider;
		}
	}
}