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

		public ContainerRegistry RegisterSingletone<T>(BaseContext context = null)
		{
			return RegisterProvider<T>(new SingletonProvider<T>(context));
		}
		
		public T BuildUpConstructor<T>()
		{
			var type = typeof(T);
			return (T) BuildUpConstructor(type);
		}

		public T BuildUpProperties<T>(T obj)
		{
			BuildUpProperties(obj.GetType(), obj);
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

		private object BuildUpConstructor(Type type)
		{
			if (type.BaseType != typeof(object))
			{
				return BuildUpConstructor(type.BaseType);	
			}

			var constructors = type.GetConstructors();

			// Constructor with dependency attribute
			var constructorsWithDepAttr = GetConstructorsWithDependencyAttribute(constructors).ToList();
			var count = constructorsWithDepAttr.Count();
			if (count > 1)
			{
				throw new ContainerException("There is more than 1 constructor with Dependency attribute!");
			}

			if (count != 0)
			{
				return ResolveConstructor(constructorsWithDepAttr.First());
			}

			// Constructor with min parameters
			var leastParametersConstructor = GetLeastParametersConstructor(constructors);
			if (leastParametersConstructor != null)
			{
				return ResolveConstructor(leastParametersConstructor);
			}
			
			// Default
			return Activator.CreateInstance(type);
		}

		private IEnumerable<ConstructorInfo> GetConstructorsWithDependencyAttribute(IEnumerable<ConstructorInfo> constructors)
		{
			var dependencyAttrType = typeof(DependencyAttribute);
			return constructors.Where(x =>
				x.CustomAttributes.FirstOrDefault(attributeData => attributeData.AttributeType == dependencyAttrType) != null);
		}

		private ConstructorInfo GetLeastParametersConstructor(IEnumerable<ConstructorInfo> constructors)
		{
			var minParamsCount = constructors.Min(x => x.GetParameters().Length);

			return constructors.FirstOrDefault(x => x.GetParameters().Length == minParamsCount);
		}

		private void BuildUpProperties(Type type, object obj)
		{
			if (type.BaseType != typeof(object))
				BuildUpProperties(type.BaseType, obj);
			
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

		private object ResolveConstructor(ConstructorInfo constructorInfo)
		{
			var constructorParams = constructorInfo.GetParameters();
			var constructorObjects = new object[constructorParams.Length];

			for (var i = 0; i < constructorParams.Length; i++)
			{
				var parameter = constructorParams[i];

				if (!TryGetObjectProviderFromContainer(parameter.ParameterType, parameter.Name, out var objProvider))
				{
					objProvider = GetObjectProviderFromContainer(parameter.ParameterType, string.Empty);
				}

				constructorObjects[i] = objProvider.GetObject(this);
			}
			
			return constructorInfo.Invoke(constructorObjects);
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

		private bool TryGetObjectProviderFromContainer(Type type, string name, out IObjectProvider objectProvider)
		{
			if (IsKeyRegistered<Type>(name))
			{
				objectProvider = GetObjectProviderFromContainer(type, name);
				return true;
			}

			objectProvider = null;
			return false;
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