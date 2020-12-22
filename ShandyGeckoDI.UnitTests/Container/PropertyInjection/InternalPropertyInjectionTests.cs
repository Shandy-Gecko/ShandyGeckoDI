using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.PropertyInjection
{
	public class InternalPropertyInjectionTests
	{
		private class Test1
		{
		}

		private class TestInternalDep
		{
			[Dependency] internal Test1 Test1 { get; set; }
		}
		
		[Test]
		public void TestResolveInternalDependency()
		{
			var container = new GeckoContainer();
			var test1 = new Test1();

			container.RegisterInstance(test1);
			
			var test2 = new TestInternalDep();
			container.BuildUp(test2);
			
			Assert.AreEqual(test1, test2.Test1);
		}
	}
}