using System.Collections.Generic;
using System.Linq;
using ShandyGecko.LogSystem;
using ShandyGecko.ShandyGeckoDI.LifeTimeProvider;

namespace ShandyGecko.ShandyGeckoDI
{
	public class LifeTimeChecker : ILifeTimeCheckerReactions
	{
		[LogFilter] public const string Tag = "LifeTimeChecker";
		
		private readonly List<ContainerRegistry> _stack = new List<ContainerRegistry>();

		public LifeTimeReaction NullContextReaction { get; set; }
		public LifeTimeReaction LifeTimeErrorReaction { get; set; }

		public LifeTimeChecker()
		{
			NullContextReaction = LifeTimeReaction.NoReaction;
			LifeTimeErrorReaction = LifeTimeReaction.ThrowException;
		}
		
		public void Push(ContainerRegistry containerRegistry)
		{
			_stack.Add(containerRegistry);
			
			CheckLifeTime();
		}

		public void Pop()
		{
			_stack.RemoveAt(_stack.Count - 1);
		}

		private void CheckLifeTime()
		{
			var stackCount = _stack.Count;
			
			if (stackCount <= 1)
				return;

			var parentRegistry = _stack.ElementAt(stackCount - 2);
			var childRegistry = _stack.ElementAt(stackCount - 1);

			if (parentRegistry.Context == null)
			{
				HandleNullContextError(parentRegistry);
				return;
			}

			if (childRegistry.Context == null)
			{
				HandleNullContextError(childRegistry);
				return;
			}

			if (parentRegistry.Context.Lifetime <= childRegistry.Context.Lifetime)
			{
				HandleContextLifeTimeError(parentRegistry, childRegistry);
			}
		}

		private void HandleNullContextError(ContainerRegistry containerRegistry)
		{
			var message = $"ContainerRegistry {containerRegistry} has null context!";

			switch (NullContextReaction)
			{
				case LifeTimeReaction.NoReaction:
					break;
				case LifeTimeReaction.LogError:
					Log.Error(Tag, message);
					break;
				case LifeTimeReaction.ThrowException:
					throw new LifeTimeException(message);
			}
		}

		private void HandleContextLifeTimeError(ContainerRegistry parentRegistry, ContainerRegistry childRegistry)
		{
			var message = $"Parent ContainerRegistry {parentRegistry} has context with lifetime {parentRegistry.Context.Lifetime}" +
			              $" less or equal than child ContainerRegistry {childRegistry} with context lifetime {childRegistry.Context.Lifetime}";

			switch (LifeTimeErrorReaction)
			{
				case LifeTimeReaction.NoReaction:
					break;
				case LifeTimeReaction.LogError:
					Log.Error(Tag, message);
					break;
				case LifeTimeReaction.ThrowException:
					throw new LifeTimeException(message);
			}
		}
	}
}