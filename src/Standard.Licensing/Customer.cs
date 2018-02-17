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

using System.Xml.Linq;

namespace Standard.Licensing
{
    /// <summary>
    /// The customer of a <see cref="License"/>.
    /// </summary>
    public class Customer : LicenseAttributes
    {
        internal Customer(XElement xmlData)
            : base(xmlData, "CustomerData")
        {
        }

        /// <summary>
        /// Gets or sets the Name of this <see cref="Customer"/>.
        /// </summary>
        public string Name
        {
            get { return GetTag("Name"); }
            set { SetTag("Name", value); }
        }
        
        /// <summary>
        /// Gets or sets the Company of this <see cref="Customer"/>.
        /// </summary>
        public string Company
        {
            get { return GetTag("Company"); }
            set { SetTag("Company", value); }
        }

        /// <summary>
        /// Gets or sets the Email of this <see cref="Customer"/>.
        /// </summary>
        public string Email
        {
            get { return GetTag("Email"); }
            set { SetTag("Email", value); }
        }
    }
}