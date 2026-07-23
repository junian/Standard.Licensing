//
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace Standard.Licensing
{

    /// <summary>
    /// serialize as JSON / deserialize from JSON
    /// </summary>
    public static class JsonSerializeExtension
    {
        /// <summary>
        /// serialize as JSON
        /// </summary>
        /// <param name="license">License instance</param>
        /// <returns>JSON string</returns>
        public static string ToJson(this License license)
        {
            try
            {
                var jsonObj = new LicenseModel
                {
                    Id = license.Id,
                    Type = license.Type,
                    Quantity = license.Quantity,
                    Signature = license.Signature,
                    Expiration = license.Expiration
                };

                if (license.AdditionalAttributes != null)
                {
                    jsonObj.AdditionalAttributes = license.AdditionalAttributes.GetAll();
                }
                if (license.Customer != null)
                {
                    jsonObj.Customer = new Dictionary<string, string> {
                        { "Name", license.Customer.Name },
                        { "Company", license.Customer.Company },
                        { "Email", license.Customer.Email }
                    };
                }
                if (license.ProductFeatures != null)
                {
                    jsonObj.ProductFeatures = license.ProductFeatures.GetAll();
                }

                return JsonSerializer.Serialize(jsonObj);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// deserialize from JSON
        /// </summary>
        /// <param name="sourceString">JSON string</param>
        /// <returns>License instance</returns>
        public static License ToLicense(this string sourceString)
        {
            try
            {
                var jsonObj = JsonSerializer.Deserialize<LicenseModel>(sourceString);
                var xml = new XElement("License");
                xml.SetTag("Id", jsonObj.Id.ToString());
                xml.SetTag("Type", jsonObj.Type.ToString());
                xml.SetTag("Quantity", jsonObj.Quantity.ToString());

                if (jsonObj.ProductFeatures != null)
                {
                    var xmlFeatures = xml.Element("ProductFeatures");
                    if (xmlFeatures == null)
                    {
                        xml.Add(new XElement("ProductFeatures"));
                    }
                    xmlFeatures = xml.Element("ProductFeatures");
                    foreach (var item in jsonObj.ProductFeatures)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            xmlFeatures.SetChildTag("Feature", item.Key, item.Value);
                        }
                    }
                }





                if (jsonObj.AdditionalAttributes != null)
                {
                    var xmlAdditional = xml.Element("LicenseAttributes");
                    if (xmlAdditional == null)
                    {
                        xml.Add(new XElement("LicenseAttributes"));
                    }
                    xmlAdditional = xml.Element("LicenseAttributes");
                    foreach (var item in jsonObj.AdditionalAttributes)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            xmlAdditional.SetChildTag("Attribute", item.Key, item.Value);
                        }
                    }
                }

                if (jsonObj.Customer != null)
                {
                    var xmlCustomer = xml.Element("Customer");
                    if (xmlCustomer == null)
                    {
                        xml.Add(new XElement("Customer"));
                    }
                    xmlCustomer = xml.Element("Customer");
                    foreach (var item in jsonObj.Customer)
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            var node = new XElement(item.Key);
                            node.SetValue(item.Value);
                            xmlCustomer.Add(node);
                        }
                    }
                }
                xml.SetTag("Signature", jsonObj.Signature);
                xml.SetTag("Expiration", jsonObj.Expiration.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture));


                return new License(xml);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static void SetTag(this XElement xmlData, string name, string value)
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

        public static string GetTag(this XElement xmlData, string name)
        {
            var element = xmlData.Element(name);
            return element != null ? element.Value : null;
        }

        public static void SetChildTag(this XElement xmlData, string childName, string name, string value)
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
    }
}
