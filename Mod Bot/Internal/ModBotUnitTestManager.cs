using InternalModBot.UnitTests;
using ModLibrary;
using PlayFab.GroupsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InternalModBot
{
    /// <summary>
    /// Handles the Unit Tests for the API methods in Mod-Bot
    /// </summary>
    public static class ModBotUnitTestManager
    {
        static readonly UnitTest[] _unitTests = new UnitTest[]
        {
            new AccessorInstanceCallPrivateMethodUnitTest()
        };

        /// <summary>
        /// Runs all defined Unit Tests
        /// </summary>
        public static void RunAllUnitTests()
        {
            int unsuccessfulUnitTests = 0;
            foreach (UnitTest unitTest in _unitTests)
            {
                if (!RunUnitTest(unitTest))
                    unsuccessfulUnitTests++;
            }

            if (unsuccessfulUnitTests == 0)
            {
                debug.Log("All unit tests successful!", Color.green);
            }
            else
            {
                debug.Log(unsuccessfulUnitTests + " Unit Tests failed", Color.red / 2f);
            }
        }

        /// <summary>
        /// Attemps to run a defined unit test with the given name
        /// </summary>
        /// <param name="unitTestName"></param>
        /// <returns></returns>
        public static bool TryRunUnitTest(string unitTestName)
        {
            foreach (UnitTest unitTest in _unitTests)
            {
                if (unitTest.IsActivatorMatching(unitTestName))
                {
                    RunUnitTest(unitTest);
                    return true;
                }
            }

            return false;
        }

        static bool RunUnitTest(UnitTest unitTest)
        {
            object[] result = unitTest.RunTest();

            bool wasSuccessful;
            if (unitTest.IsExpectedResult(result))
            {
                debug.Log("UnitTest successful: " + unitTest.CommandActivator, Color.green);
                wasSuccessful = true;
            }
            else
            {
                debug.Log("UnitTest failed: " + unitTest.CommandActivator + ". Result did not match expected result", Color.red);
                wasSuccessful = false;
            }

            unitTest.Cleanup();

            return wasSuccessful;
        }
    }

    /// <summary>
    /// Base class for all Unit Tests
    /// </summary>
    public abstract class UnitTest
    {
        /// <summary>
        /// Returns the string used to activate this Unit Test
        /// </summary>
        public abstract string CommandActivator { get; }

        /// <summary>
        /// Returns <see langword="true"/> if the Unit Test was successful
        /// </summary>
        /// <param name="result">The return value of <see cref="RunTest"/></param>
        /// <returns></returns>
        public abstract bool IsExpectedResult(object[] result);

        /// <summary>
        /// Runs the Unit Test
        /// </summary>
        /// <returns></returns>
        public abstract object[] RunTest();

        /// <summary>
        /// Called just before <see cref="RunTest"/>
        /// </summary>
        public virtual void SetupUnitTest() { }

        /// <summary>
        /// Expected to clean up anything changed by the unit test
        /// </summary>
        public virtual void Cleanup() { }

        /// <summary>
        /// Returns <see langword="true"/> if the given string matches the <see cref="CommandActivator"/>
        /// </summary>
        /// <param name="activator"></param>
        /// <returns></returns>
        public bool IsActivatorMatching(string activator)
        {
            return CommandActivator.ToLower() == activator.ToLower();
        }
    }
}
