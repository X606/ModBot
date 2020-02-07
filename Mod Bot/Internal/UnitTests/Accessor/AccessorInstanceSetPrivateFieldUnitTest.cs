using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace InternalModBot.UnitTests
{
    public class AccessorInstanceSetPrivateFieldUnitTest : UnitTest
    {
        AccessorTestFields _accessorTestFieldsInstance;

        public override string CommandActivator => "AccessorInstanceSetPrivateField";

        public override bool IsExpectedResult(object[] result)
        {
            if (_accessorTestFieldsInstance.GetFloatingPointValue() != 2f)
            {
                debug.Log(CommandActivator + ": Expected 2.0, Got: " + _accessorTestFieldsInstance.GetFloatingPointValue());
                return false;
            }

            if (_accessorTestFieldsInstance.GetStringValue() != "TestValue2")
            {
                debug.Log(CommandActivator + ": Expected \"TestValue2\", Got: \"" + _accessorTestFieldsInstance.GetStringValue() + "\"");
                return false;
            }

            return true;
        }

        public override object[] RunTest()
        {
            Accessor accessor = new Accessor(typeof(AccessorTestFields), _accessorTestFieldsInstance);

            accessor.SetPrivateField("_floatingPointValue", 2f);
            accessor.SetPrivateField("_stringValue", "TestValue2");

            return null;
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
