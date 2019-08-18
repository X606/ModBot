using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ModLibrary
{
    public delegate void FakeAction();
    public class fakeAction
    {
        public fakeAction(MethodInfo _method, object _instance)
        {
            method = _method;
            instance = _instance;
        }

        public fakeAction(MethodInfo _method, object _instance, object[] _args)
        {
            method = _method;
            instance = _instance;
            args = _args;
        }

        public void Invoke(object[] parms)
        {
            method.Invoke(instance, parms);
        }

        public void Invoke()
        {
            method.Invoke(instance, args);
        }

        public MethodInfo method;

        public object instance;

        public object[] args;
    }
}
