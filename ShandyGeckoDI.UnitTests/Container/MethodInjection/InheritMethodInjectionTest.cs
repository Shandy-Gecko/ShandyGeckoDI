using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.MethodInjection
{
	public class InheritMethodInjectionTest
	{
		public class Test
		{
		}

		public class Parent
		{
			public Test Test { get; private set; }

			[Dependency]
			public void SetDeps(Test test)
			{
				Test = test;
			}
		}
		
		public class Child : Parent
		{
		}
		
		[Test]
		public void InheritMethodInjection()
		{
			var container = new GeckoContainer();
			var injection1 = new Test();

			container.RegisterInstance(injection1);

			var actual = container.BuildUpType<Child>();
			
			Assert.IsNotNull(actual);
			Assert.AreEqual(injection1, actual.Test);
		}
	}
}