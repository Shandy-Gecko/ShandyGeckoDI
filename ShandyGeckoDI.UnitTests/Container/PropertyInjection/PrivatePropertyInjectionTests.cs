using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.PropertyInjection
{
	public class PrivatePropertyInjectionTests
	{
		private class Test1
		{
		}

		private class TestPrivateDep
		{
			[Dependency] private Test1 TestDep { get; set; }

			public Test1 GetTestDep()
			{
				return TestDep;
			}
		}
		
		[Test]
		public void TestResolvePrivateDependency()
		{
			var container = new GeckoContainer();
			var test1 = new Test1();
			container.RegisterInstance(test1);
			
			var test2 = new TestPrivateDep();
			container.BuildUp(test2);
			
			Assert.AreEqual(test1, test2.GetTestDep());
		}
	}
}