using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.MethodInjection
{
	public class ParameterMethodInjectionTests
	{
		private class Test1
		{
		}
		
		private class Test2
		{
			public Test1 TestParam1 { get; private set; }
			public Test1 TestParam2 { get; private set; }

			[Dependency]
			public void SetDeps(Test1 testParam1, Test1 testParam2)
			{
				TestParam1 = testParam1;
				TestParam2 = testParam2;
			}
		}
		
		[Test]
		public void ParameterMethodInjectionTest()
		{
			var container = new GeckoContainer();
			var param1 = new Test1();
			var param2 = new Test1();

			var actual = container.BuildUpType<Test2>(
				new Parameter(param1,"TestParam1"),
				new Parameter(param2,"TestParam2"));
			
			Assert.IsNotNull(actual);
			Assert.AreEqual(param1, actual.TestParam1);
			Assert.AreEqual(param2, actual.TestParam2);
			Assert.AreNotEqual(actual.TestParam1, actual.TestParam2);
		}
	}
}