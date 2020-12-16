using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.PropertyInjection
{
	public class PropertyInjectionTests
	{
		private class Test1
		{
		}

		private class TestPublicDep
		{
			[Dependency] public Test1 Test1 { get; set; }
		}
		
		private class TestNamedDep
		{
			[Dependency("Test1")] public Test1 Test1 { get; set; }
			[Dependency("test1")] public Test1 Test2 { get; set; }
		}

		[Test]
		public void TestResolveDependency()
		{
			var container = new GeckoContainer();
			var test1 = new Test1();

			container.RegisterInstance(test1);
			
			var test2 = new TestPublicDep();
			container.BuildUp(test2);
			
			Assert.AreEqual(test1, test2.Test1);
		}

		[Test]
		public void TestResolveNamedDependency()
		{
			var container = new GeckoContainer();
			var test1 = new Test1();
			var test2 = new Test1();

			container.RegisterInstance(test1).WithName("Test1");
			container.RegisterInstance(test2).WithName("test1");
			
			var testNamedDep = new TestNamedDep();
			container.BuildUp(testNamedDep);
			
			Assert.AreEqual(test1, testNamedDep.Test1);
			Assert.AreEqual(test2, testNamedDep.Test2);
		}
		
		//TODO дописать - нужно проверить инъекцию во все варианты доступности свойства
	}
}