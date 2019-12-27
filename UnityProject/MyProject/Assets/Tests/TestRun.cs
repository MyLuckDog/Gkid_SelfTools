using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

namespace Tests
{
    public class TestRun
    {
        [TestCase("JustForTest")]
        public void TestRunSimplePasses(string str)
        {
            GameObject go = new GameObject("sfe");
            if (go?.name.Length < 12)
            {
                Debug.Log("000000000");
            }
            else
            {
                Debug.Log("1111111111");
            }
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestRunWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
