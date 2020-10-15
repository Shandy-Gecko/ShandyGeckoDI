using NUnit.Framework;

namespace ShandyGecko.ShandyGeckoDI.Tests.Context
{
	internal class TestClassWithDependency
	{
		[Dependency] public TestInjectClass TestInjectClass { get; set; }
	}

	internal class TestInjectClass
	{
	}

	[TestFixture]
	public class ResolveDependencyTests
	{
		[Test]
		public void TestDependencyAttribute()
		{
			var container = new ShandyGeckoDI.Container();
			var context = new ObjectContext(container);

			var testInjectClass = new TestInjectClass();
			context.RegisterInstance(testInjectClass);
			context.InstallDependencies();
			
			var testClassWithDependency = new TestClassWithDependency();
			context.BuildUp(testClassWithDependency);
			
			Assert.IsNotNull(testClassWithDependency.TestInjectClass);
			Assert.AreEqual(testInjectClass, testClassWithDependency.TestInjectClass);
		}
	}
}