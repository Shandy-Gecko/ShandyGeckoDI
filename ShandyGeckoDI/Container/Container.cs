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

		public bool IsKeyRegistered(Type type, string name = "")
		{
			return IsKeyRegistered(new ContainerKey(type, name));
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

		public T Resolve<T>(string name = "", params Parameter[] parameters)
		{
			var obj = Resolve(typeof(T), name, parameters);
			return (T) obj;
		}
		
		public object Resolve(Type type, string name = "", params Parameter[] parameters)
		{
			if (!TryGetObjectProvider(type, name, out var objectProvider))
			{
				throw new ContainerException($"Can't get object for type {type} and name {name}");
			}

			return objectProvider.GetObject(this, parameters);
		} 
		
		public T TryResolve<T>(string name = "", params Parameter[] parameters)
		{
			var obj = TryResolve(typeof(T), name, parameters);
			return (T) obj;
		}
		
		public object TryResolve(Type type, string name = "", params Parameter[] parameters)
		{
			if (!TryGetObjectProvider(type, name, out var objectProvider))
			{
				return null;
			}

			return objectProvider.GetObject(this, parameters);
		} 

		/// <summary>
		/// Create object of type T and BuildUp it
		/// </summary>
		/// <param name="parameters"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T BuildUpType<T>(params Parameter[] parameters)
		{
			var obj = BuildUpConstructorInternal<T>(parameters);
			return BuildUpPropertiesInternal<T>(obj, parameters);
		}

		/// <summary>
		/// BuildUp object of type T
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="parameters"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T BuildUp<T>(T obj, params Parameter[] parameters)
		{
			return BuildUpPropertiesInternal<T>(obj, parameters);
		}
		
		public void Dispose()
		{
			foreach (var containerRegistry in _containerRegistries.Values)
			{
				containerRegistry.ObjectProvider.Dispose();
			}
		}
		
		private T BuildUpConstructorInternal<T>(params Parameter[] parameters)
		{
			var type = typeof(T);
			return (T) BuildUpConstructorInternal(type, parameters);
		}

		private T BuildUpPropertiesInternal<T>(T obj, params Parameter[] parameters)
		{
			BuildUpPropertiesInternal(obj.GetType(), obj, parameters);
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

		private object BuildUpConstructorInternal(Type type, params Parameter[] parameters)
		{
			if (type.BaseType is { } && type.BaseType != typeof(object) && !type.BaseType.IsAbstract)
			{
				return BuildUpConstructorInternal(type.BaseType);	
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
				return ResolveConstructor(constructorsWithDepAttr.First(), parameters);
			}

			// Constructor with min parameters
			var leastParametersConstructor = GetLeastParametersConstructor(constructors);
			if (leastParametersConstructor != null)
			{
				return ResolveConstructor(leastParametersConstructor, parameters);
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

		private void BuildUpPropertiesInternal(Type type, object obj, params Parameter[] parameters)
		{
			if (type.BaseType != typeof(object))
				BuildUpPropertiesInternal(type.BaseType, obj);
			
			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

			foreach (var property in properties)
			{
				var paramAttr = TryGetAttribute<ParameterAttribute>(property);
				if (paramAttr != null)
				{
					ResolveParameterAttribute(obj, property, paramAttr.Name, parameters);
					continue;
				}

				var dependencyAttr = TryGetAttribute<DependencyAttribute>(property);
				if (dependencyAttr != null)
				{
					ResolveDependencyAttribute(obj, property, dependencyAttr.Name);
				}
			}
		}

		private object ResolveConstructor(ConstructorInfo constructorInfo, params Parameter[] parameters)
		{
			var constructorParams = constructorInfo.GetParameters();
			var constructorObjects = new object[constructorParams.Length];

			for (var i = 0; i < constructorParams.Length; i++)
			{
				var constructorParam = constructorParams[i];

				// Cначала параметер
				if (TryGetParameter(parameters, constructorParam, out var registryParameter))
				{
					constructorObjects[i] = registryParameter.Object;
					continue;	
				}

				if (TryGetObjectProvider(constructorParam, out var objectProvider))
				{
					constructorObjects[i] = objectProvider.GetObject(this);
					continue;
				}
				
				throw new ContainerException($"Can't get parameter or object provider for constructor parameter {constructorParam}");
			}
			
			return constructorInfo.Invoke(constructorObjects);
		}
		
		private void ResolveDependencyAttribute(object obj, PropertyInfo propertyInfo, string name)
		{
			var objProvider = GetObjectProviderFromContainer(propertyInfo.PropertyType, name);

			var createdObj = objProvider.GetObject(this);
			
			var setter = propertyInfo.GetSetMethod(true);
			if (setter == null)
			{
				throw new ContainerException($"Property {propertyInfo} has null setter");
			}
			
			setter.Invoke(obj, new[] {createdObj});
		}
		
		private void ResolveParameterAttribute(object obj, PropertyInfo propertyInfo, string name, IEnumerable<Parameter> parameters)
		{
			var parameter = parameters.FirstOrDefault(x => x.Type == propertyInfo.PropertyType && x.Name == name);
			object targetObj = null;

			if (parameter != null)
			{
				targetObj = parameter.Object;
			}
			
			var setter = propertyInfo.GetSetMethod(true);
			if (setter == null)
			{
				throw new ContainerException($"Property {propertyInfo} has null setter");
			}
			
			setter.Invoke(obj, new[] {targetObj});
		}

		private bool TryGetObjectProvider(ParameterInfo parameterInfo, out IObjectProvider objectProvider)
		{
			if (IsKeyRegistered(parameterInfo.ParameterType, parameterInfo.Name))
			{
				objectProvider = GetObjectProviderFromContainer(parameterInfo.ParameterType, parameterInfo.Name);
				return true;
			}
			
			if (IsKeyRegistered(parameterInfo.ParameterType))
			{
				objectProvider = GetObjectProviderFromContainer(parameterInfo.ParameterType, string.Empty);;
				return true;
			}

			objectProvider = null;
			return false;
		}

		private bool TryGetObjectProvider(Type type, string name, out IObjectProvider objectProvider)
		{
			if (IsKeyRegistered(type, name))
			{
				objectProvider = GetObjectProviderFromContainer(type, name);
				return true;
			}

			objectProvider = null;
			return false;
		}
		
		private bool TryGetParameter(IEnumerable<Parameter> parameters, ParameterInfo parameterInfo, out Parameter parameter)
		{
			var parameterWithName =
				parameters.FirstOrDefault(x => x.Name == parameterInfo.Name && x.Type == parameterInfo.ParameterType);

			if (parameterWithName != null)
			{
				parameter = parameterWithName;
				return true;
			}

			var parameterWithoutName = parameters.FirstOrDefault(x => x.Type == parameterInfo.ParameterType);
			if (parameterWithoutName != null)
			{
				parameter = parameterWithoutName;
				return true;
			}

			parameter = null;
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

		private T TryGetAttribute<T>(PropertyInfo propertyInfo)
		{
			return (T) propertyInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();
		}
	}
}