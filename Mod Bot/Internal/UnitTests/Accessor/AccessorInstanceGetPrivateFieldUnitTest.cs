using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace InternalModBot.UnitTests
{
    public class AccessorInstanceGetPrivateFieldUnitTest : UnitTest
    {
        AccessorTestFields _accessorTestFieldsInstance;

        public override string CommandActivator => "AccessorInstanceGetPrivateField";

        public override bool IsExpectedResult(object[] result)
        {
            if ((float)result[0] != 1f)
            {
                debug.Log(CommandActivator + ": Expected 1.0, Got: " + result[0]);
                return false;
            }

            if ((string)result[1] != "TestValue1")
            {
                debug.Log(CommandActivator + ": Expected \"TestValue1\", Got: \"" + result[1] + "\"");
                return false;
            }

            return true;
        }

        public override object[] RunTest()
        {
            Accessor accessor = new Accessor(typeof(AccessorTestFields), _accessorTestFieldsInstance);

            object[] results = new object[2];

            results[0] = accessor.GetPrivateField("_floatingPointValue");
            results[1] = accessor.GetPrivateField("_stringValue");

            return results;
        }

        public override void SetupUnitTest()
        {
            _accessorTestFieldsInstance = new AccessorTestFields(1f, "TestValue1");
        }

        public override void Cleanup()
        {
            _accessorTestFieldsInstance = null;
        }

        private class AccessorTestFields
        {
            float _floatingPointValue;

            string _stringValue;

            public AccessorTestFields(float floatingPointValue, string stringValue)
            {
                _floatingPointValue = floatingPointValue;
                _stringValue = stringValue;
            }

            public float GetFloatingPointValue()
            {
                return _floatingPointValue;
            }

            public string GetStringValue()
            {
                return _stringValue;
            }
        }
    }
}
