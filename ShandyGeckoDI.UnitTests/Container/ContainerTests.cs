using NUnit.Framework;

namespace ShandyGecko.ShandyGeckoDI.Tests.Container
{
	public class ContainerTests
	{
		[Test]
		public void AddContainerRegistryTest()
		{
			var container = new ShandyGeckoDI.Container();
			var key = new ContainerKey(typeof(object));
			
			var containerRegistry = new ContainerRegistry(null);
			containerRegistry.AddKey(key);
			
			container.AddContainerRegistry(containerRegistry);
			
			Assert.IsTrue(container.IsKeyRegistered(key));
		}
		
		[Test]
		public void RemoveContainerRegistryTest()
		{
			var container = new ShandyGeckoDI.Container();
			var key = new ContainerKey(typeof(object));
			
			var containerRegistry = new ContainerRegistry(null);
			containerRegistry.AddKey(key);
			
			container.AddContainerRegistry(containerRegistry);
			Assert.IsTrue(container.IsKeyRegistered(key));
			
			container.RemoveContainerRegistry(containerRegistry);
			Assert.IsFalse(container.IsKeyRegistered(key));
		}
	}
}