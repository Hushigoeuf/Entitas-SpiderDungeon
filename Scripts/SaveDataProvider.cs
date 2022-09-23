using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public abstract class SaveDataValue<T> where T : IComparable, IConvertible
    {
        public readonly string Key;

        protected T _value;

        public bool IsExists { get; private set; }
        public bool IsChanged { get; private set; }

        public T Value
        {
            get => _value;
            set
            {
                if (!IsExists) IsExists = true;
                else if (Compare(_value, value)) return;

                _value = value;
                IsChanged = true;
            }
        }

        public SaveDataValue(string key, T defaultValue)
        {
            Key = key;
            IsExists = DefaultServices.SaveData.KeyExists(key);

            if (IsExists) Value = Load(key, defaultValue);
            else Value = defaultValue;
        }

        public virtual void Save()
        {
            if (!IsChanged) return;
            IsChanged = false;
            Save(Key, Value);
        }

        protected abstract bool Compare(T first, T second);

        protected abstract T Load(string key, T defaultValue);

        protected abstract void Save(string key, T value);
    }

    public sealed class SaveDataValue_Int : SaveDataValue<int>
    {
        public SaveDataValue_Int(string key, int defaultValue) : base(key, defaultValue)
        {
        }

        protected override bool Compare(int first, int second) => first == second;

        protected override int Load(string key, int defaultValue) =>
            DefaultServices.SaveData.LoadInt(key, defaultValue);

        protected override void Save(string key, int value)
        {
            DefaultServices.SaveData.SaveInt(key, value);
        }
    }

    public sealed class SaveDataValue_Bool : SaveDataValue<bool>
    {
        public SaveDataValue_Bool(string key, bool defaultValue) : base(key, defaultValue)
        {
        }

        protected override bool Compare(bool first, bool second) => first == second;

        protected override bool Load(string key, bool defaultValue) =>
            DefaultServices.SaveData.LoadBool(key, defaultValue);

        protected override void Save(string key, bool value)
        {
            DefaultServices.SaveData.SaveBool(key, value);
        }
    }

    [AddComponentMenu(nameof(GameEngine) + "/" + nameof(SaveDataProvider))]
    public sealed class SaveDataProvider : MonoBehaviour
    {
        private static readonly Dictionary<string, SaveDataValue_Int> IntValues =
            new Dictionary<string, SaveDataValue_Int>();

        private static readonly Dictionary<string, SaveDataValue_Bool> BoolValues =
            new Dictionary<string, SaveDataValue_Bool>();

        private void OnDestroy()
        {
            SaveAll();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveAll();
        }

        private static void InitValueInt(string key, int defaultValue)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (!IntValues.ContainsKey(key))
                IntValues.Add(key, new SaveDataValue_Int(key, defaultValue));
        }

        private static void InitValueBool(string key, bool defaultValue)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (!BoolValues.ContainsKey(key))
                BoolValues.Add(key, new SaveDataValue_Bool(key, defaultValue));
        }

        public static bool IsExistsInt(string key, int defaultValue)
        {
            InitValueInt(key, defaultValue);
            return IntValues[key].IsExists;
        }

        public static bool IsExistsBool(string key, bool defaultValue)
        {
            InitValueBool(key, defaultValue);
            return BoolValues[key].IsExists;
        }

        public static int GetInt(string key, int defaultValue)
        {
            InitValueInt(key, defaultValue);
            return IntValues[key].Value;
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            InitValueBool(key, defaultValue);
            return BoolValues[key].Value;
        }

        public static void SetInt(string key, int value)
        {
            InitValueInt(key, value);
            IntValues[key].Value = value;
        }

        public static void SetBool(string key, bool value)
        {
            InitValueBool(key, value);
            BoolValues[key].Value = value;
        }

        public static void SaveAll()
        {
            if (IntValues.Count != 0)
                foreach (var key in IntValues.Keys)
                    IntValues[key].Save();
            if (BoolValues.Count != 0)
                foreach (var key in BoolValues.Keys)
                    BoolValues[key].Save();
        }
    }
}