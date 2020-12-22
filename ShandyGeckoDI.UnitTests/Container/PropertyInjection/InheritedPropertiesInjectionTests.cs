using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.PropertyInjection
{
	public class InheritedPropertiesInjectionTests
	{
		private class Test
		{
		}

		private abstract class TestParent
		{
			[Dependency] public Test TestDep { get; set; }
		}

		private class TestChild : TestParent
		{
		}

		[Test]
		public void TestResolveInheritedDependency()
		{
			var container = new GeckoContainer();
			var test = new Test();
			container.RegisterInstance(test);

			var testChild = new TestChild();
			container.BuildUp(testChild);

			Assert.AreEqual(test, testChild.TestDep);
		}
	}
}