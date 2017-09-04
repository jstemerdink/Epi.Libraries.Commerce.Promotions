// Copyright © 2017 Jeroen Stemerdink.
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
namespace EPi.Libraries.Commerce.Promotions.Validators
{
    using System;
    using System.Collections.Generic;

    using EPi.Libraries.Commerce.Promotions.Models;

    using EPiServer.Commerce.Validation;
    using EPiServer.Core;
    using EPiServer.Framework.Localization;
    using EPiServer.Validation;

    /// <summary>
    /// Class BuyBundleGetItemDiscountValidator.
    /// </summary>
    /// <seealso cref="EPiServer.Commerce.Validation.PromotionDataValidatorBase{BuyBundleGetItemDiscount}" />
    [CLSCompliant(false)]
    public class BuyBundleGetItemDiscountValidator : PromotionDataValidatorBase<BuyBundleGetItemDiscount>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuyBundleGetItemDiscountValidator"/> class. 
        /// </summary>
        /// <param name="localizationService">
        /// The localization service.
        /// </param>
        public BuyBundleGetItemDiscountValidator(LocalizationService localizationService)
            : base(localizationService: localizationService)
        {
        }

        /// <summary>
        /// Adds the errors if needed.
        /// </summary>
        /// <param name="promotion">The promotion data.</param>
        /// <param name="validationErrors">The validation errors.</param>
        protected override void AddErrorsIfNeeded(
            BuyBundleGetItemDiscount promotion,
            List<ValidationError> validationErrors)
        {
            if (!ContentReference.IsNullOrEmpty(contentLink: promotion.Bundle))
            {
                return;
            }

            if (validationErrors == null)
            {
                return;
            }

            List<ValidationError> validationErrorList = validationErrors;
            ValidationError validationError = new ValidationError();
            validationError.Severity = ValidationErrorSeverity.Error;
            validationError.ValidationType = ValidationErrorType.StorageValidation;
            validationError.PropertyName = "Bundle";

            string str = this.LocalizationService.GetString("/commerce/validation/buyfrombundlerequired");
            validationError.ErrorMessage = str;
            validationErrorList.Add(item: validationError);
        }
    }
}