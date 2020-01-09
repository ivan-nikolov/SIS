using System.Collections.Generic;
using System.IO;
using SIS.MvcFramework.ViewEngine;
using Xunit;

namespace SIS.MvcFramework.Tests
{
    public class TestSisViewEngine
    {
        [Theory]
        [InlineData("TestWithoutCSharpCode")]
        [InlineData("UseForForeachAndIf")]
        [InlineData("UseModelData")]
        public void TestGetHtml(string testFileName)
        {
            IViewEngine viewEngine = new SisViewEngine();

            var viewFileName = $"ViewTests/{testFileName}.html";
            var resultFileName = $"ViewTests/{testFileName}.Result.html";

            var viewContent = File.ReadAllText(viewFileName);
            var expectedResult = File.ReadAllText(resultFileName);
            var actualResult = viewEngine.GetHtml<object>(viewContent, new TestViewModel() 
            {
                StringValue = "str",
                ListValues = new List<string>() { "123", "val1", string.Empty }
            });

            Assert.Equal(expectedResult.TrimEnd(), actualResult.TrimEnd());
        }
    }
}
