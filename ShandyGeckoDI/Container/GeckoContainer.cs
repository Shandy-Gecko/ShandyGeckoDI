using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ShandyGecko.LogSystem;

namespace ShandyGecko.ShandyGeckoDI
{
	public class GeckoContainer : IDisposable
	{
		[LogFilter] public const string Tag = "GeckoContainer";
		
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

		public T Resolve<T>(string name = "", params Parameter[] parameters)
		{
			var obj = Resolve(typeof(T), name, parameters);
			return (T) obj;
		}
		
		public object Resolve(Type type, string name = "", params Parameter[] parameters)
		{
			if (!TryGetContainerRegistry(type, name, out var containerRegistry))
			{
				throw new ContainerException($"Can't get object for type {type} and name {name}");
			}

			if (!TryGetObjectProvider(containerRegistry, out var objectProvider))
			{
				return null;
			}

			var allParameters = parameters.Concat(containerRegistry.Parameters).ToArray();

			return objectProvider.GetObject(this, allParameters);
		} 
		
		public T TryResolve<T>(string name = "", params Parameter[] parameters)
		{
			var obj = TryResolve(typeof(T), name, parameters);
			return (T) obj;
		}
		
		public object TryResolve(Type type, string name = "", params Parameter[] parameters)
		{
			if (!TryGetContainerRegistry(type, name, out var containerRegistry))
			{
				return null;
			}

			if (!TryGetObjectProvider(containerRegistry, out var objectProvider))
			{
				return null;
			}

			var allParameters = parameters.Concat(containerRegistry.Parameters).ToArray();

			return objectProvider.GetObject(this, allParameters);
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
			return PerformInternalBuildUps(obj, parameters);
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
			return PerformInternalBuildUps(obj, parameters);
		}
		
		public void Dispose()
		{
			foreach (var containerRegistry in _containerRegistries.Values)
			{
				containerRegistry.ObjectProvider.Dispose();
			}
		}

		private T PerformInternalBuildUps<T>(T obj, params Parameter[] parameters)
		{
			obj = BuildUpMethodInternal(obj, parameters);
			return BuildUpPropertiesInternal<T>(obj, parameters);
		}
		
		private T BuildUpConstructorInternal<T>(params Parameter[] parameters)
		{
			//TODO проверка на GeckoBehaviour, в них недолжно быть конструкторов
			var type = typeof(T);
			return (T) BuildUpConstructorInternal(type, parameters);
		}

		private T BuildUpMethodInternal<T>(T obj, params Parameter[] parameters)
		{
			BuildUpMethodInternal(obj.GetType(), obj, parameters);
			return obj;
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
			var constructors = type.GetConstructors();

			// Constructor with dependency attribute
			var constructorsWithDepAttr = GetConstructorsWithDependencyAttribute(constructors).ToList();
			var count = constructorsWithDepAttr.Count();
			if (count > 1)
			{
				throw new ContainerException("There is more than 1 constructor with Dependency attribute!");
			}

			// First constructor with dependecy attribute
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

		private void BuildUpMethodInternal(Type type, object obj, params Parameter[] parameters)
		{
			if (!(type.BaseType is null) && type.BaseType.IsClass && type.BaseType != typeof(object))
			{
				BuildUpMethodInternal(type.BaseType, obj, parameters);	
			}

			var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			var methodsWithDepAttribute = GetMethodsWithDependencyAttribute(methods);

			var methodCount = methodsWithDepAttribute.Count();
			if (methodCount > 1)
			{
				throw new ContainerException("There is more than 1 method with Dependency attribute!");
			}

			if (methodCount == 0)
			{
				return;
			}

			ResolveMethod(obj, methodsWithDepAttribute.First(), parameters);
		}
		
		private IEnumerable<MethodInfo> GetMethodsWithDependencyAttribute(IEnumerable<MethodInfo> methodInfos)
		{
			var dependencyAttrType = typeof(DependencyAttribute);
			return methodInfos.Where(x =>
				x.CustomAttributes.FirstOrDefault(attributeData => attributeData.AttributeType == dependencyAttrType) != null);
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
			var constructorObjects = GetObjectsByParameterInfos(constructorParams, parameters);

			return constructorInfo.Invoke(constructorObjects);
		}

		private void ResolveMethod(object obj, MethodInfo methodInfo, params Parameter[] parameters)
		{
			var methodParams = methodInfo.GetParameters();
			var methodObjects = GetObjectsByParameterInfos(methodParams, parameters);

			methodInfo.Invoke(obj, methodObjects);
		}

		private object[] GetObjectsByParameterInfos(ParameterInfo[] parameterInfos, params Parameter[] parameters)
		{
			var objects = new object[parameterInfos.Length];

			for (var i = 0; i < parameterInfos.Length; i++)
			{
				var parameterInfo = parameterInfos[i];

				// Cначала параметер
				if (TryGetParameter(parameters, parameterInfo, out var registryParameter))
				{
					objects[i] = registryParameter.Object;
					continue;	
				}

				if (!TryGetContainerRegistry(parameterInfo, out var containerRegistry))
				{
					throw new ContainerException(
						$"Can't get parameter or object provider for parameter {parameterInfo}");	
				}

				if (TryGetObjectProvider(containerRegistry, out var objectProvider))
				{
					objects[i] = objectProvider.GetObject(this);
				}
				else
				{
					Log.Error(Tag, $"Can't get object provider for parameter {parameterInfo}");	
				}
			}

			return objects;
		}
		
		private void ResolveDependencyAttribute(object obj, PropertyInfo propertyInfo, string name)
		{
			var containerRegistry = GetContainerRegistry(propertyInfo.PropertyType, name);
			if (!TryGetObjectProvider(containerRegistry, out var objProvider))
			{
				throw new ContainerException($"Can't get container registry for property {propertyInfo}");
			}

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

		private bool TryGetContainerRegistry(ParameterInfo parameterInfo, out ContainerRegistry containerRegistry)
		{
			var allKeys = _containerRegistries.Keys.Where(x =>
				x.Type == parameterInfo.ParameterType && string.Equals(x.Name, parameterInfo.Name,
					StringComparison.InvariantCultureIgnoreCase));

			if (allKeys.Count() > 1)
			{
				Log.Warning(Tag, $"Found more than one object provider with name {parameterInfo.Name}! " +
				                 "Constructor/method injection isn't case-sensitive!");
			}

			if (allKeys.Any())
			{
				var firstKey = allKeys.First();
				containerRegistry = GetContainerRegistry(firstKey);
				
				return true;
			}

			if (IsKeyRegistered(parameterInfo.ParameterType))
			{
				containerRegistry = GetContainerRegistry(parameterInfo.ParameterType, string.Empty);
				return true;
			}

			containerRegistry = null;
			return false;
		}

		private bool TryGetParameter(IEnumerable<Parameter> parameters, ParameterInfo parameterInfo, out Parameter parameter)
		{
			//TODO Тонкое место - что мы делаем с именами вида Test1 и test1
			var parametersWithName =
				parameters.Where(x =>
					string.Equals(x.Name, parameterInfo.Name, StringComparison.CurrentCultureIgnoreCase) &&
					x.Type == parameterInfo.ParameterType);

			if (parametersWithName.Count() > 1)
			{
				Log.Warning(Tag, $"Found more than one parameter with name {parameterInfo.Name}! " +
				                 "Constructor/method injection isn't case-sensitive!");
			}

			var parameterWithName = parametersWithName.FirstOrDefault();
			
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

		private bool TryGetObjectProvider(ContainerRegistry containerRegistry, out IObjectProvider objectProvider)
		{
			if (containerRegistry == null)
			{
				Log.Error(Tag, "ContainerRegistry is null!");
				objectProvider = null;
				return false;
			}

			if (containerRegistry.ObjectProvider == null)
			{
				Log.Error(Tag, "ObjectProvider is null!");
				objectProvider = null;
				return false;
			}

			objectProvider = containerRegistry.ObjectProvider;
			return true;
		}

		private bool TryGetContainerRegistry(Type type, string name, out ContainerRegistry containerRegistry)
		{
			if (IsKeyRegistered(type, name))
			{
				containerRegistry = GetContainerRegistry(type, name);
				return true;
			}

			containerRegistry = null;
			return false;
		}
		
		private ContainerRegistry GetContainerRegistry(Type type, string name)
		{
			var containerKey = new ContainerKey(type, name);
			return GetContainerRegistry(containerKey);
		}

		private ContainerRegistry GetContainerRegistry(ContainerKey containerKey)
		{
			if (!_containerRegistries.ContainsKey(containerKey))
			{
				throw new ContainerException($"Key {containerKey} isn't registered!");
			}

			var containerRegistry = _containerRegistries[containerKey];

			if (containerRegistry == null)
			{
				throw new ContainerException($"Key {containerKey} has null object provider");
			}

			return containerRegistry;
		}

		private T TryGetAttribute<T>(PropertyInfo propertyInfo)
		{
			return (T) propertyInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault();
		}
	}
}