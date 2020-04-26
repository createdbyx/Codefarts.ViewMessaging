// <copyright>
//   Copyright (c) 2012 Codefarts
//   All rights reserved.
//   contact@codefarts.com
//   http://www.codefarts.com
// </copyright>

#if NET20
namespace System.ViewMessaging
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a read-only, generic collection of key/value pairs.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of the value.
    /// </typeparam>
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Fields

        /// <summary>The _dictionary.</summary>
        private readonly IDictionary<TKey, TValue> dictionary;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey,TValue}" /> class. Initializes a new instance
        ///     of the <see cref="ReadOnlyDictionary{TKey, TValue}" /> class.
        /// </summary>
        public ReadOnlyDictionary()
        {
            this.dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey,TValue}"/> class.
        ///     Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary to encapsulate.
        /// </param>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public ICollection<TKey> Keys
        {
            get
            {
                return this.dictionary.Keys;
            }
        }

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that
        ///     implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public ICollection<TValue> Values
        {
            get
            {
                return this.dictionary.Values;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The the element with the specified key.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// This dictionary is read-only
        /// </exception>
        public TValue this[TKey key]
        {
            get
            {
                return this.dictionary[key];
            }

            set
            {
                throw new NotSupportedException("This dictionary is read-only");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        /// This dictionary is read-only.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        /// This dictionary is read-only
        /// </exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="System.NotSupportedException">This dictionary is read-only</exception>
        public void Clear()
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>;
        ///     otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.dictionary.Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the
        ///     specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise,
        ///     false.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="arrayIndex">
        /// Index of the array.
        /// </param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the
        ///     collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to remove.
        /// </param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if
        ///     <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// This dictionary is read-only
        /// </exception>
        public bool Remove(TKey key)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if
        ///     <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        /// This dictionary is read-only
        /// </exception>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException("This dictionary is read-only");
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key whose value to get.
        /// </param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found; otherwise, the default
        ///     value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element
        ///     with the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.dictionary.TryGetValue(key, out value);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.dictionary as IEnumerable).GetEnumerator();
        }

        #endregion
    }
}
#endif