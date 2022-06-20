using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class Global
    {
        [TestMethod]
        public void Test()
        {
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            int i=0;
            int j=1;
            int n=j/0;
        }
    }
}
