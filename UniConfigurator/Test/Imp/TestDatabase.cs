using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BonGames.UniConfigurator
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(TestDatabase))]
    public class TestDatabaseEditor : UniDatabaseEditor<IUniRecord> { }
#endif

    public interface ITestRecord : IUniRecord { }

    public class TestRecordBase : ITestRecord
    {
        public string Guid { get; set; }

        public TestRecordBase()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
    }
    public class Weapon : IUniRecord
    {
        public string Guid { get; set; }
        public string Name = "Weapon";
    }

    public class Skill : IUniRecord
    {
        public string Guid { get; set; }
        public int Level;
    }

    [CreateAssetMenu(fileName = "TestDatabase", menuName = "UniConfigurator/TestDatabase", order = 0)]
    public class TestDatabase : UniDatabase<IUniRecord>
    {

    }
}
