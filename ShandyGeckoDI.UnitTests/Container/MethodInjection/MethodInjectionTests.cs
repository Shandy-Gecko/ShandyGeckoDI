using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.MethodInjection
{
	public class MethodInjectionTests
	{
		private class Test1
		{
		}
		
		private class Test2
		{
			public Test1 Test1 { get; private set; }

			[Dependency]
			public void SetDeps(Test1 test1)
			{
				Test1 = test1;
			}
		}
		
		[Test]
		public void MethodInjectionTest()
		{
			var container = new GeckoContainer();
			var injection1 = new Test1();

			container.RegisterInstance(injection1);

			var actual = container.BuildUpType<Test2>();
			
			Assert.IsNotNull(actual);
			Assert.AreEqual(injection1, actual.Test1);
		}
	}
}