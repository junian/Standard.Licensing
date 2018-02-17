﻿//
// Copyright © 2012 - 2013 Nauck IT KG     http://www.nauck-it.de
//
// Author:
//  Daniel Nauck        <d.nauck(at)nauck-it.de>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Standard.Licensing
{
    /// <summary>
    /// Represents a dictionary of license attributes.
    /// </summary>
    public class LicenseAttributes
    {
        protected readonly XElement xmlData;
        protected readonly XName childName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseAttributes"/> class.
        /// </summary>
        internal LicenseAttributes(XElement xmlData, XName childName)
        {
            this.xmlData = xmlData ?? new XElement("null");
            this.childName = childName;
        }

        /// <summary>
        /// Adds a new element with the specified key and value
        /// to the collection.
        /// </summary>
        /// <param name="key">The key of the element.</param>
        /// <param name="value">The value of the element.</param>
        public virtual void Add(string key, string value)
        {
            SetChildTag(key, value);
        }

        /// <summary>
        /// Adds all new element into the collection.
        /// </summary>
        /// <param name="features">The dictionary of elements.</param>
        public virtual void AddAll(IDictionary<string, string> features)
        {
            foreach (var feature in features)
                Add(feature.Key, feature.Value);
        }

        /// <summary>
        /// Removes a element with the specified key
        /// from the collection.
        /// </summary>
        /// <param name="key">The key of the element.</param>
        public virtual void Remove(string key)
        {
            var element =
                xmlData.Elements(childName)
                    .FirstOrDefault(e => e.Attribute("name") != null && e.Attribute("name").Value == key);

            if (element != null)
                element.Remove();
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public virtual void RemoveAll()
        {
            xmlData.RemoveAll();
        }

        /// <summary>
        /// Gets the value of a element with the
        /// specified key.
        /// </summary>
        /// <param name="key">The key of the element.</param>
        /// <returns>The value of the element if available; otherwise null.</returns>
        public virtual string Get(string key)
        {
            return GetChildTag(key);
        }

        /// <summary>
        /// Gets all elements.
        /// </summary>
        /// <returns>A dictionary of all elements in this collection.</returns>
        public virtual IDictionary<string, string> GetAll()
        {
            return xmlData.Elements(childName).ToDictionary(e => e.Attribute("name").Value, e => e.Value);
        }

        /// <summary>
        /// Determines whether the specified element is in
        /// this collection.
        /// </summary>
        /// <param name="key">The key of the element.</param>
        /// <returns>true if the collection contains this element; otherwise false.</returns>
        public virtual bool Contains(string key)
        {
            return xmlData.Elements(childName).Any(e => e.Attribute("name") != null && e.Attribute("name").Value == key);
        }

        /// <summary>
        /// Determines whether all specified elements are in
        /// this collection.
        /// </summary>
        /// <param name="keys">The list of keys of the elements.</param>
        /// <returns>true if the collection contains all specified elements; otherwise false.</returns>
        public virtual bool ContainsAll(string[] keys)
        {
            return xmlData.Elements(childName).All(e => e.Attribute("name") != null && keys.Contains(e.Attribute("name").Value));
        }

        protected virtual void SetTag(string name, string value)
        {
            var element = xmlData.Element(name);

            if (element == null)
            {
                element = new XElement(name);
                xmlData.Add(element);
            }

            if (value != null)
                element.Value = value;
        }

        protected virtual void SetChildTag(string name, string value)
        {
            var element =
                xmlData.Elements(childName)
                    .FirstOrDefault(e => e.Attribute("name") != null && e.Attribute("name").Value == name);

            if (element == null)
            {
                element = new XElement(childName);
                element.Add(new XAttribute("name", name));
                xmlData.Add(element);
            }

            if (value != null)
                element.Value = value;
        }

        protected virtual string GetTag(string name)
        {
            var element = xmlData.Element(name);
            return element != null ? element.Value : null;
        }

        protected virtual string GetChildTag(string name)
        {
            var element =
                xmlData.Elements(childName)
                    .FirstOrDefault(e => e.Attribute("name") != null && e.Attribute("name").Value == name);

            return element != null ? element.Value : null;
        }
    }
}