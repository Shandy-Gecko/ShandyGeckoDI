using System;
using System.Collections.Generic;

namespace ShandyGecko.ShandyGeckoDI
{
	public class ContainerRegistry
	{
		private readonly HashSet<ContainerKey> _keys = new HashSet<ContainerKey>();
		private readonly HashSet<Parameter> _parameters = new HashSet<Parameter>();

		private Container _container;
		
		public BaseContext Context { get; private set; }
		public bool IsNonLazy { get; private set; }
		public IObjectProvider ObjectProvider { get; }

		public ContainerRegistry(IObjectProvider objectProvider, Container container, BaseContext context = null)
		{
			ObjectProvider = objectProvider;
			_container = container;
			Context = context;

			TryAddRegistryToContext();
		}

		public void AddKey(ContainerKey key)
		{
			if (_keys.Contains(key))
			{
				throw new ContainerException($"Trying to add duplicate ContainerKey {key}!");
			}

			_keys.Add(key);
		}

		public ContainerRegistry As<T>()
		{
			var key = new ContainerKey(typeof(T));
			
			AddKey(key);
			_container.AddContainerRegistry(key, this);

			return this;
		}

		public ContainerRegistry WithName(string name)
		{
			foreach (var key in _keys)
			{
				_container.RemoveContainerRegistry(key);
				key.SetName(name);
				_container.AddContainerRegistry(key, this);
			}
			
			return this;
		}

		public ContainerRegistry NonLazy()
		{
			IsNonLazy = true;	
			return this;
		}

		public ContainerRegistry SetContext(BaseContext context)
		{
			Context = context;
			TryAddRegistryToContext();

			return this;
		}

		public ContainerRegistry SetParameter(Parameter parameter)
		{
			if (_parameters.Contains(parameter))
			{
				throw new ContainerException($"Parameter {parameter} already added to ContainerRegistry {this}");
			}

			_parameters.Add(parameter);
			
			return this;
		}
		
		public ContainerRegistry SetParameter(object obj, Type type, string name = "")
		{
			var newParameter = new Parameter(obj, type, name);

			return SetParameter(newParameter);
		}
		
		public ContainerRegistry SetParameter<T>(T obj, string name = "")
		{
			return SetParameter(obj, typeof(T), name);
		}

		public ContainerRegistry SetParameter(object obj, string name = "")
		{
			return SetParameter(obj, obj.GetType(), name);
		}

		public ContainerRegistry SetParameters(params Parameter[] parameters)
		{
			foreach (var parameter in parameters)
			{
				SetParameter(parameter);
			}

			return this;
		}

		public ContainerRegistry SetParameters(params object[] parameters)
		{
			foreach (var o in parameters)
			{
				SetParameter(o);
			}

			return this;
		}

		internal void OnContextDispose()
		{
			foreach (var key in _keys)
			{
				_container.RemoveContainerRegistry(key);
			}
		}

		private void TryAddRegistryToContext()
		{
			Context?.AddRegistry(this);
		}
		
		//TODO добавить методы по подстановке в атрибут параметра или конструктор
	}
}