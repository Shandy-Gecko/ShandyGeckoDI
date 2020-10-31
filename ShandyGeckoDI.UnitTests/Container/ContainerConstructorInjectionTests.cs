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

			var actual = container.BuildUpType<Test1>();
			
			Assert.IsNotNull(actual);
		}

		[Test]
		public void ConstructorInjectionTest()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			var injection = new Test1();

			container.RegisterInstance(injection);

			var actual = container.BuildUpType<Test2>();
			
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

			var actual = container.BuildUpType<Test3>();
			
			Assert.IsNotNull(actual);
			Assert.AreEqual(injection, actual.Test1);
			Assert.IsNotNull(actual.Test2);
		}

		[Test]
		public void ConstructorParameterInjection()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			var test1 = new Test1();
			var test2 = container.BuildUpType<Test2>(new Parameter(test1));
			
			Assert.NotNull(test2);
			Assert.AreEqual(test1, test2.Test1);
		}
		
		[Test]
		public void ConstructorParameterOverProviderInjection()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			
			var test1Param = new Test1();
			var test1Provider = new Test1();

			container.RegisterInstance(test1Provider);
			
			var test2 = container.BuildUpType<Test2>(new Parameter(test1Param));
			
			Assert.NotNull(test2);
			Assert.AreEqual(test1Param, test2.Test1);
			Assert.AreNotEqual(test1Provider, test2.Test1);
		}
	}
}