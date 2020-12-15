using NUnit.Framework;
using NUnit.Framework.Internal;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container
{
	public class ConstructorNamedInjectionTests
	{
		private class Test1
		{
		}
		
		private class Test2
		{
			public Test1 Test0 { get; }
			public Test1 Test1 { get; }
			
			public Test2(Test1 test0, Test1 test1)
			{
				Test0 = test0;
				Test1 = test1;
			}
		}
		
		[Test]
		public void ConstructorInjectionTest()
		{
			var container = new GeckoContainer();
			var injection0 = new Test1();
			var injection1 = new Test1();

			container.RegisterInstance(injection0).WithName("Test0");
			container.RegisterInstance(injection1).WithName("Test1");

			var actual = container.BuildUpType<Test2>();
			
			Assert.IsNotNull(actual);
			Assert.AreEqual(injection0, actual.Test0);
			Assert.AreEqual(injection1, actual.Test1);
		}
	}
}