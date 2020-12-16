using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.ConstructorInjection
{
	public class ConstructorInheritInjectionTests
	{
		private class Test
		{
			
		}
		
		private class ParentTest
		{
			public Test Test { get; }

			public ParentTest(Test test)
			{
				Test = test;
			}
		}
		
		private class ChildTest : ParentTest
		{
			public ChildTest(Test test) : base(test)
			{
			}
		}

		[Test]
		public void TestInjectInInheritedConstructor()
		{
			var container = new GeckoContainer();
			var test = new Test();

			container.RegisterInstance(test);

			var child = container.BuildUpType<ChildTest>();
			
			Assert.IsNotNull(child);
			Assert.AreEqual(test, child.Test);
		}
	}
}