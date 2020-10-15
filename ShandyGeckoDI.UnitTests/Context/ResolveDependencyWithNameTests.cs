using NUnit.Framework;

namespace ShandyGecko.ShandyGeckoDI.Tests.Context
{
	internal class TestClassWithNamedDependency
	{
		[Dependency("Test1")] public TestInjectClassWithName TestInjectClass1 { get; set; }
	}
	
	internal class TestInjectClassWithName
	{
	}
	
	[TestFixture]
	public class ResolveDependencyWithNameTests
	{
		[Test]
		public void TestDependencyWithNameAttribute()
		{
			var container = new ShandyGeckoDI.Container();
			var context = new ObjectContext(container);

			var testInjectClassWithName1 = new TestInjectClassWithName();
			context.RegisterInstance(testInjectClassWithName1).WithName("Test1");
			
			var testInjectClassWithName2 = new TestInjectClassWithName();
			context.RegisterInstance(testInjectClassWithName2).WithName("Test2");
			
			context.InstallDependencies();
			
			var testClassWithDependency = new TestClassWithNamedDependency();
			context.BuildUp(testClassWithDependency);
			
			Assert.IsNotNull(testClassWithDependency.TestInjectClass1);
			Assert.AreEqual(testInjectClassWithName1, testClassWithDependency.TestInjectClass1);
			Assert.AreNotEqual(testInjectClassWithName2, testClassWithDependency.TestInjectClass1);
		}
	}
}