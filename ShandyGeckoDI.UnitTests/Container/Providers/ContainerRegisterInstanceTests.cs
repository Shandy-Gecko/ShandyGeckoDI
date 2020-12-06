using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container
{
	public class ContainerRegisterInstanceTests
	{
		private const string TestName = "TestName";
		
		private interface ITest
		{
		}
		
		private interface ITest2
		{
		}
		
		private interface ITest3
		{
			
		}
		
		private class TestClass : ITest, ITest2
		{
		}
		
		[Test]
		public void ContainerRegisterInstanceTest()
		{
			var container = new GeckoContainer();
			var testClass = new TestClass();
			container.RegisterInstance(testClass);
			
			Assert.IsTrue(container.IsKeyRegistered<TestClass>());
			
			var actual = container.Resolve<TestClass>();
			Assert.AreEqual(testClass, actual);
		}
		
		[Test]
		public void ContainerRegisterInstanceWithNameTest()
		{
			var container = new GeckoContainer();
			var testClass = new TestClass();
			
			container.RegisterInstance(testClass).WithName(TestName);

			Assert.IsFalse(container.IsKeyRegistered<TestClass>());
			Assert.IsTrue(container.IsKeyRegistered<TestClass>(TestName));
			
			var actual = container.Resolve<TestClass>(TestName);
			Assert.AreEqual(testClass, actual);
		}

		[Test]
		public void ContainerRegisterInstanceAsInterfaceTest()
		{
			var container = new GeckoContainer();
			var testClass = new TestClass();

			container.RegisterInstance(testClass).As<ITest>();
			
			Assert.IsTrue(container.IsKeyRegistered<TestClass>());
			Assert.IsTrue(container.IsKeyRegistered<ITest>());
			
			var actual = container.Resolve<ITest>();
			Assert.AreEqual(testClass, actual);
		}

		[Test]
		public void ContainerRegisterInstanceWithNameAsInterfaceTest()
		{
			var container = new GeckoContainer();
			var testClass = new TestClass();

			container.RegisterInstance(testClass).WithName(TestName).As<ITest>();
			
			Assert.IsFalse(container.IsKeyRegistered<TestClass>());
			Assert.IsTrue(container.IsKeyRegistered<TestClass>(TestName));

			Assert.IsFalse(container.IsKeyRegistered<ITest>());
			Assert.IsTrue(container.IsKeyRegistered<ITest>(TestName));
			
			var actual = container.Resolve<ITest>(TestName);
			Assert.AreEqual(testClass, actual);
		}

		[Test]
		public void ContainerRegisterInstanceAsMultipleInterfacesTest()
		{
			var container = new GeckoContainer();
			var testClass = new TestClass();

			container.RegisterInstance(testClass).As<ITest>().As<ITest2>();
			
			Assert.IsTrue(container.IsKeyRegistered<TestClass>());
			Assert.IsTrue(container.IsKeyRegistered<ITest>());
			Assert.IsTrue(container.IsKeyRegistered<ITest2>());
			
			var actual = container.Resolve<ITest2>();
			Assert.AreEqual(testClass, actual);
		}

		[Test]
		public void ContainerRegisterInstanceAsWrongInterfaceTest()
		{
			var container = new GeckoContainer();
			var testClass = new TestClass();

			Assert.Throws<ContainerException>(() =>
			{
				container.RegisterInstance(testClass).As<ITest3>();
			});
		}
	}
}