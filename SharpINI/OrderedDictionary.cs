using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpINI
{
	//Yes, .NET Standard defines a OrderedDictionary class
	//But that class only implements IDictionary while we
	//need a IDictionary<TKey, TValue>
	//Therefore we have to create our own OrderedDictionary
	public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		//We could use the integrated keyset in the dictionary
		//which would make inconsistent states much more harder
		//to achive, but it's not that easy to use it.
		//When adding a new key-value using .Add we could either
		//add the key to our keys before or after adding the key
		//to the dictionary
		//If we add it before and then .Add throws an exception,
		//we've added the key to our keys without having it added
		//to the dictionary (of course, we could use try-catch, but
		//that would bloat the code too much as we would have to add
		//it on all places where an .AddKey or .RemoveKey is).
		//The second possibility is to call .AddKey after adding it to
		//the dictionary. This would ensure that the key is only added
		//to our keys if adding it to the dictionary was successfull.
		//But then, we don't know whether the key was already present
		//in the dictionary before and the integrated keyset of the dictionary
		//would therefore be unusable (of course, we could call .ContainsKey
		//before adding it but that would be redundant code and would also
		//bloat too much like above with the try-catch approach)
		protected HashSet<TKey> keySet;
		protected List<TKey> keys;
		protected Dictionary<TKey, TValue> dic;

		public TValue this[TKey key]
		{
			get => this.dic[key];
			set
			{
				this.dic[key] = value;
				this.AddKey(key);
			}
		}

		public ICollection<TKey> Keys => new List<TKey>(this.keys);
		public ICollection<TValue> Values => this.keys.Select((key) => this.dic[key]).ToList();
		public int Count => this.dic.Count;
		public bool IsReadOnly => ((IDictionary)this.dic).IsReadOnly;


		public OrderedDictionary()
		{
			this.dic = new Dictionary<TKey, TValue>();
			this.keys = new List<TKey>();
			this.keySet = new HashSet<TKey>();
		}
		public OrderedDictionary(IDictionary<TKey, TValue> dictionary)
		{
			this.dic = new Dictionary<TKey, TValue>(dictionary);
			this.keys = new List<TKey>(dictionary.Keys);
			this.keySet = new HashSet<TKey>(this.keys);
		}


		public bool TryGetValue(TKey key, out TValue value) => this.dic.TryGetValue(key, out value);

		public void Add(TKey key, TValue value)
		{
			this.dic.Add(key, value);
			this.AddKey(key);
		}
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)this.dic).Add(item);
			this.AddKey(item.Key);
		}

		public bool Remove(TKey key)
		{
			var result = this.dic.Remove(key);
			if(!result)
				return false;

			this.RemoveKey(key);
			return true;
		}
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			var result = ((ICollection<KeyValuePair<TKey, TValue>>)this.dic).Remove(item);
			if(!result)
				return false;

			this.RemoveKey(item.Key);
			return true;
		}

		public void Clear()
		{
			this.dic.Clear();
			this.keys.Clear();
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach(var key in this.keys)
				yield return new KeyValuePair<TKey, TValue>(key, this[key]);
		}
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)this.dic).Contains(item);
		public bool ContainsKey(TKey key) => this.dic.ContainsKey(key);

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)this.dic).CopyTo(array, arrayIndex);



		protected bool AddKey(TKey key)
		{
			if(this.keySet.Contains(key))
				return false;

			this.keySet.Add(key);
			this.keys.Add(key);

			return true;
		}
		protected bool RemoveKey(TKey key)
		{
			if(!this.keySet.Contains(key))
				return false;

			this.keySet.Remove(key);
			this.keys.Remove(key);

			return true;
		}
	}
}
