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
using System.Collections.Generic;

namespace Standard.Licensing.Validation
{
    internal class ValidationChainBuilder : IStartValidationChain, IValidationChain
    {
        private readonly List<ILicenseValidator> validators;
        private ILicenseValidator currentValidatorChain;
        private readonly License license;

        public ValidationChainBuilder(License license)
        {
            this.license = license;
            validators = new List<ILicenseValidator>();
        }

        public ILicenseValidator StartValidatorChain()
        {
            return currentValidatorChain = new LicenseValidator();
        }

        public void CompleteValidatorChain()
        {
            if (currentValidatorChain == null)
                return;

            validators.Add(currentValidatorChain);
            currentValidatorChain = null;
        }

        public ICompleteValidationChain When(Predicate<License> predicate)
        {
            currentValidatorChain.ValidateWhen = predicate;
            return this;
        }

        public IStartValidationChain And()
        {
            CompleteValidatorChain();
            return this;
        }

        public IEnumerable<IValidationFailure> AssertValidLicense()
        {
            CompleteValidatorChain();

            foreach (var validator in validators)
            {
                if (validator.ValidateWhen != null && !validator.ValidateWhen(license))
                    continue;

                if (!validator.Validate(license))
                    yield return validator.FailureResult
                                 ?? new GeneralValidationFailure
                                        {
                                            Message = "License validation failed!"
                                        };
            }
        }
    }
}