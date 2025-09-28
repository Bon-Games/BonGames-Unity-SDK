using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BonGames.UniConfigurator
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(TestDatabase))]
    public class TestDatabaseEditor : UniDatabaseEditor<ITestRecord> { }
#endif

    public interface ITestRecord : IUniRecord { }

    public class TestRecordBase : ITestRecord
    {
        private string _id;
        public string GetId()
        {
            return _id;
        }

        public TestRecordBase()
        {
            _id = System.Guid.NewGuid().ToString();
        }
    }

    [CreateAssetMenu(fileName = "TestDatabase", menuName = "UniConfigurator/TestDatabase", order = 0)]
    public class TestDatabase : UniDatabase<ITestRecord>
    {

    }
}
