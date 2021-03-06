﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.Evangelism.Data
{
    public interface IStore<T>
    {
        bool Exists(string id);
        T Get(string id);
        void Remove(string id);
        void Update(string id, T obj);
    }

    [Serializable]
    public class MemoryStore<T> : IStore<T> where T: new()
    {
        public static Dictionary<string, T> Objects = new Dictionary<string, T>();
        public T Get(string id)
        {
            if (!Objects.ContainsKey(id)) Objects.Add(id, new T());
            return Objects[id];
        }

        public void Remove(string id)
        {
            if (Objects.ContainsKey(id)) Objects.Remove(id);
        }

        public void Update(string id, T obj)
        {
            Objects[id] = obj;
        }

        public bool Exists(string id) => Objects.ContainsKey(id);

    }

}