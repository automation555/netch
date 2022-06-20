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
            int i;
            int j=0;
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            
            
        }
    }
}
