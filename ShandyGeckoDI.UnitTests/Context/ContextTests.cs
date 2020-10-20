using NUnit.Framework;

namespace ShandyGeckoDI.UnitTests.Context
{
	public class ContextTests
	{
		private class TestClass
		{
		}
		
		[Test]
		public void SetContextOnRegistryTest()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			var context = new ShandyGecko.ShandyGeckoDI.Context();

			container.RegisterInstance(new TestClass(), context);
			Assert.AreEqual(1, context.ContainerRegistries.Count);
		}
		
		[Test]
		public void SetContextWithSetterTest()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			var context = new ShandyGecko.ShandyGeckoDI.Context();

			container.RegisterInstance(new TestClass()).SetContext(context);
			Assert.AreEqual(1, context.ContainerRegistries.Count);
		}

		[Test]
		public void ContextDisposeText()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			var context = new ShandyGecko.ShandyGeckoDI.Context();
			
			container.RegisterInstance(new TestClass(), context);
			Assert.AreEqual(1, context.ContainerRegistries.Count);
			
			context.Dispose();
			Assert.AreEqual(0, context.ContainerRegistries.Count);
			Assert.IsFalse(container.IsKeyRegistered<TestClass>());
		}
	}
}