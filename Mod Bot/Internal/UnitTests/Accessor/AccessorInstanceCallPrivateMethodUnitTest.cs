using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace InternalModBot.UnitTests
{
    public class AccessorInstanceCallPrivateMethodUnitTest : UnitTest
    {
        public override string CommandActivator => "AccessorInstanceCallPrivateMethod";

        public override bool IsExpectedResult(object[] result)
        {
            if ((int)result[0] != 5)
            {
                debug.Log(CommandActivator + ": Expected: 5, Got: " + result[0]);
                return false;
            }

            if ((int)result[1] != (5 + 13))
            {
                debug.Log(CommandActivator + ": Expected: " + (5 + 13) + ", Got: " + result[1]);
                return false;
            }

            return true;
        }

        public override object[] RunTest()
        {
            Accessor accessor = new Accessor(typeof(AccessorTestMethods), new AccessorTestMethods());

            object[] results = new object[2];

            results[0] = accessor.CallPrivateMethod("Get5");
            results[1] = accessor.CallPrivateMethod("Add", new object[] { 5, 13 });

            return results;
        }

        private class AccessorTestMethods
        {
            int Get5()
            {
                return 5;
            }

            int Add(int num1, int num2)
            {
                return num1 + num2;
            }
        }
    }
}
