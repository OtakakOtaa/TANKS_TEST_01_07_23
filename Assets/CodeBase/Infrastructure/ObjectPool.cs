using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public sealed class ObjectPool<TObject> where TObject : Behaviour
    {
        private readonly List<TObject> _source = new ();
        private readonly TObject _prefab;

        public ObjectPool(TObject prefab)
        {
            _prefab = prefab;
        }
        
        public TObject Fetch(Vector3 position)
        {
            var item = GetItem();
            item.transform.position = position;
            item.gameObject.SetActive(true);
            return item;
        }

        public void Put(TObject item)
            => item.gameObject.SetActive(false);

        private TObject GetItem()
        {
            var item = _source.FirstOrDefault(i => i.gameObject.active is false);
            
            if (item != default) return item;
            item = Object.Instantiate(_prefab);
            _source.Add(item);
            
            return item;
        }
    }
}