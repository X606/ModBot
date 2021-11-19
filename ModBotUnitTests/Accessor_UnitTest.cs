using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModLibrary;
using System;
using System.Reflection;

namespace ModBotUnitTests
{
    [TestClass]
    public class Accessor_UnitTest
    {
        public class ClassA
        {
            public float FloatValue => getFloat();

            protected virtual float getFloat()
            {
                return -1.8f;
            }

            string getString()
            {
                return "str";
            }
        }

        public class ClassB : ClassA
        {
            protected override float getFloat()
            {
                return base.getFloat() * 3.14f;
            }
        }

        public class ClassC : ClassB
        {
            protected override float getFloat()
            {
                return base.getFloat() * 5f;
            }
        }

        public class ClassD : ClassA
        {
            protected override float getFloat()
            {
                return base.getFloat() * 9.81f;
            }
        }

        int returnIntValue(IComparable comparable)
        {
            return comparable.CompareTo(90);
        }

        int returnIntValue(IConvertible convertible = null)
        {
            if (convertible == null)
                return returnIntValue() + 1;

            return convertible.ToInt32(null);
        }

        const int returnIntValue_Result = 3984267;
        int returnIntValue()
        {
            return returnIntValue_Result;
        }

        [TestMethod("CallPrivateMethod<InstanceType, ReturnType>(string, InstanceType, object[])")]
        public void CallPrivateMethod_1()
        {
            Assert.AreEqual(returnIntValue_Result, Accessor.CallPrivateMethod<Accessor_UnitTest, int>(nameof(returnIntValue), this), "returnIntValue();");

            Assert.ThrowsException<AmbiguousMatchException>(delegate
            {
                // Ambiguous match between returnIntValue(IComparable) and returnIntValue(IConvertible)
                Accessor.CallPrivateMethod<Accessor_UnitTest, int>(nameof(returnIntValue), this, new object[] { 7.6f });
            });

            Assert.AreEqual(Convert.ToInt32(7.6f), Accessor.CallPrivateMethod<Accessor_UnitTest, int>(nameof(returnIntValue), this, new object[] { 7.6f }, new Type[] { typeof(IConvertible) }), "returnIntValue(7.6f);");

            Assert.ThrowsException<MissingMethodException>(delegate
            {
                Accessor.CallPrivateMethod<Accessor_UnitTest, int>("Invalid-Method", this, new object[] { null, null, 3 });
            }, "Invalid-Method(null, null, 3)");

            ClassA classA = new ClassA();
            ClassB classB = new ClassB();
            ClassB classC = new ClassC();
            ClassA classD = new ClassD();

            Assert.AreEqual(classA.FloatValue, Accessor.CallPrivateMethod<ClassA, float>("getFloat", classA), "classA");
            Assert.AreEqual(classB.FloatValue, Accessor.CallPrivateMethod<ClassB, float>("getFloat", classB), "classB");
            Assert.AreEqual(classC.FloatValue, Accessor.CallPrivateMethod<ClassA, float>("getFloat", classC), "classC");
            Assert.AreEqual(classD.FloatValue, Accessor.CallPrivateMethod<ClassA, float>("getFloat", classD), "classD");
        }
    }
}
