using NUnit.Framework;

namespace ShandyGeckoDI.UnitTests.Container
{
	public class ContainerTests
	{
		private class TestClass
		{
		}
		
		[Test]
		public void ContainerRegisterInstanceTest()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			container.RegisterInstance(new TestClass());
			
			Assert.IsTrue(container.IsKeyRegistered<TestClass>());
		}
		
		[Test]
		public void ContainerRegisterInstanceWithNameTest()
		{
			var name = "TestName";
			
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			container.RegisterInstance(new TestClass()).WithName(name);

			Assert.IsFalse(container.IsKeyRegistered<TestClass>());
			Assert.IsTrue(container.IsKeyRegistered<TestClass>(name));
		}
	}
}