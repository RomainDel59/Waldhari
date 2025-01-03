// This class is under MIT License,
// copyrighted to https://github.com/lucasvinbr,
// and comes from https://github.com/lucasvinbr/GTA5GangMod.
// I adapt it to my use.

using System;

namespace Common.Files
{
    [Serializable]
    public abstract class File<T> where T : File<T>, new()
    {
        protected string FileName => typeof(T).Name;

        /// <summary>
        /// Loads an instance from a file or creates a default one.
        /// </summary>
        public void Load()
        {
            T loadedInstance = PersistenceHandler.LoadFromFile<T>(FileName);
            if (loadedInstance == null)
            {
                SetDefaults();
                Save();
                SetInstance((T)this);
            }
            else
            {
                SetInstance(loadedInstance);
            }
        }

        /// <summary>
        /// Saves the current instance to a file.
        /// </summary>
        public void Save()
        {
            PersistenceHandler.SaveToFile((T)this, FileName);
        }

        /// <summary>
        /// Sets all default values. Must be implemented by the derived class.
        /// </summary>
        protected abstract void SetDefaults();

        protected abstract void SetInstance(T instance);
    }
}