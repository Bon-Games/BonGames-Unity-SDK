using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BonGames.UniConfigurator
{
    public abstract class UniDatabase<T> : UnityEngine.ScriptableObject, ISerializationCallbackReceiver
        where T : IUniRecord
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.Indented,
        };


        [SerializeField, HideInInspector] private List<string> _configurationJsons = new();

        private readonly List<T> _configurations = new();     
        public int Count => _configurations.Count;

#if UNITY_EDITOR
        private bool _isDirty = false;
        
        public bool IsDirty => _isDirty || UnityEditor.EditorUtility.IsDirty(this);

        public void SetThisDirty()
        {
            _isDirty = true;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        public T this[int index]
        {
            set 
            {
                _configurations[index] = value;
                _configurationJsons[index] = Serialize(value);
            }
            get { return _configurations[index]; }
        }

        public string GetRawValue(int index)
        {
            return index < _configurationJsons.Count ? _configurationJsons[index] : string.Empty;
        }

        public T GetConfigurationById(string id)
        {
            return _configurations.FirstOrDefault(it => it.Guid == id);
        }

        public void OnAfterDeserialize()
        {
            _configurations.Clear();
            for (int i = 0; i < _configurationJsons.Count; i++)
            {
                T config = Deserialize(_configurationJsons[i]);
                if (config == null) continue;
                _configurations.Add(config);
            }
            // Debug.Log($"Deserialize there's {_configurations.Count} item(s)");
            OnDeserialized();
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (!_isDirty || !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
            _configurationJsons.Clear();
            for (int i = 0; i < _configurations.Count; i++)
            {
                _configurationJsons.Add(Serialize(_configurations[i]));
            }
            // Debug.Log($"Serialize there's {_configurationJsons.Count} item(s)");
            OnSerialized();
        }

        protected virtual void OnSerialized() { }
        
        protected virtual void OnDeserialized() { }

        public static T Deserialize(string json)
        {
            try
            {
                return (T)JsonConvert.DeserializeObject(json, JsonSerializerSettings);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return default;
            }
        }

        public static string Serialize<U>(U config) where U : IUniRecord
        {
            return JsonConvert.SerializeObject(config, JsonSerializerSettings);
        }

        public void RemoveAt(int index)
        {
            _configurations.RemoveAt(index);
            _configurationJsons.RemoveAt(index);
        }

        public void Add(T item)
        {
            _configurations.Add(item);
            _configurationJsons.Add(Serialize(item));
        }

        public void Clear()
        {
            _configurations.Clear();
            _configurationJsons.Clear();
        }
    }
}
