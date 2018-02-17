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

using System;

namespace Standard.Licensing.Validation
{
    /// <summary>
    /// Represents a <see cref="License"/> validator.
    /// </summary>
    public interface ILicenseValidator
    {
        /// <summary>
        /// Gets or sets the predicate to determine if the <see cref="License"/>
        /// is valid.
        /// </summary>
        Predicate<License> Validate { get; set; }

        /// <summary>
        /// Gets or sets the predicate to determine if the <see cref="ILicenseValidator"/>
        /// should be executed.
        /// </summary>
        Predicate<License> ValidateWhen { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IValidationFailure"/> result. The <see cref="IValidationFailure"/>
        /// will be returned to the application when the <see cref="ILicenseValidator"/> fails.
        /// </summary>
        IValidationFailure FailureResult { get; set; }
    }
}