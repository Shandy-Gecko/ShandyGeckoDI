using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container
{
	public class ContainerConstructorInjectionTests
	{
		private class Test1
		{
		}
		
		private class Test2
		{
			public Test1 Test1 { get; }
			
			public Test2(Test1 test1)
			{
				Test1 = test1;
			}
		}
		
		private class Test3
		{
			public Test1 Test1 { get; }
			public Test2 Test2 { get; }

			public Test3(Test1 test1)
			{
				Test1 = test1;
			}

			[Dependency]
			public Test3(Test1 test1, Test2 test2)
			{
				Test1 = test1;
				Test2 = test2;
			}
		}

		[Test]
		public void DefaultConstructorTest()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			container.RegisterSingletone<Test1>();

			var actual = container.BuildUpConstructor<Test1>();
			
			Assert.IsNotNull(actual);
		}

		[Test]
		public void ConstructorInjectionTest()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			var injection = new Test1();

			container.RegisterInstance(injection);
			container.RegisterSingletone<Test2>();
			
			var actual = container.BuildUpConstructor<Test2>();
			
			Assert.IsNotNull(actual);
			Assert.AreEqual(injection, actual.Test1);
		}

		[Test]
		public void ConstructorWithDependencyAttributeInjectionTest()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			var injection = new Test1();

			container.RegisterInstance(injection);
			container.RegisterSingletone<Test2>();
			container.RegisterSingletone<Test3>();
			
			var actual = container.BuildUpConstructor<Test3>();
			
			Assert.IsNotNull(actual);
			Assert.AreEqual(injection, actual.Test1);
			Assert.IsNotNull(actual.Test2);
		}
	}
}