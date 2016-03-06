using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SRPCommon.Util;

namespace ShaderUnit.TestRenderer
{
	public class RenderTestBase
	{
		private Bitmap _imageResult;
		private RenderTestHarness _harness;
		private readonly string _assetDir;

		public RenderTestBase(string assetDirectory = null)
		{
			// Conventionally use "Assets" if nothing specified.
			assetDirectory = assetDirectory ?? "Assets";
			_assetDir = PathUtils.FindPathInTree(TestCaseAssemblyDir, assetDirectory);
		}

		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public async Task TearDown()
		{
			// Dispose the test harness.
			_harness?.Dispose();
			_harness = null;

			// Report result.
			var context = TestContext.CurrentContext;
			await TestReporter.Instance.TestCompleteAsync(
				context.Test.FullName, context.Result.Outcome.Status == TestStatus.Passed, _imageResult);

			// Clear state ready for the next run (NUnit re-uses class instances).
			_imageResult = null;
		}

		protected IComputeHarness CreateComputeHarness()
		{
			if (_harness != null)
			{
				throw new ShaderUnitException("Can only create one harness per test run.");
			}
			_harness = new RenderTestHarness(new TestRenderer(), _assetDir);
			return _harness;
		}

		protected IRenderHarness CreateRenderHarness(int width, int height)
		{
			if (_harness != null)
			{
				throw new ShaderUnitException("Can only create one harness per test run.");
			}
			_harness = new RenderTestHarness(new TestRenderer(width, height), _assetDir);
			return _harness;
		}

		protected void CompareImage(Bitmap result, string imageDirectory = null)
		{
			// Stash result for reporting.
			Assert.That(_imageResult, Is.Null, "Can only compare one image per test");
			_imageResult = result;

			// Load the image to compare against.
			var context = TestContext.CurrentContext;
			var expectedImageFilename = Path.Combine(GetExpectedResultDir(imageDirectory), context.Test.FullName + ".png");
			Assert.That(File.Exists(expectedImageFilename), "No expected image to compare against.");
			var expected = new Bitmap(expectedImageFilename);

			// Compare the images.
			AssertEx.ImagesEqual(expected, result);
		}

		private string GetExpectedResultDir(string relativePath)
		{
			// Conventionally look in "ExpectedResults" if nothing was specified.
			relativePath = relativePath ?? "ExpectedResults";

			return PathUtils.FindPathInTree(TestCaseAssemblyDir, relativePath);
		}

		// Get the location of the assembly of the derived object (i.e. the test case).
		private string TestCaseAssemblyDir => Path.GetDirectoryName(GetType().Assembly.Location);
	}
}
