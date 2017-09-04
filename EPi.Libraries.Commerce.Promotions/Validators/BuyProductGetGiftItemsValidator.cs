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
    using System.Linq;

    using EPi.Libraries.Commerce.Promotions.Models;

    using EPiServer.Commerce.Validation;
    using EPiServer.Framework.Localization;
    using EPiServer.Validation;

    /// <summary>
    /// Class BuyProductGetGiftItemsValidator.
    /// </summary>
    /// <seealso cref="T:EPiServer.Validation.IValidate{Valtech.BeterBed.Web.Business.Marketing.BuyProductGetGiftItems}" />
    [CLSCompliant(false)]
    public class BuyProductGetGiftItemsValidator : IValidate<BuyProductGetGiftItems>
    {
        /// <summary>
        /// The localization service
        /// </summary>
        private readonly LocalizationService localizationService;


        /// <summary>
        /// Initializes a new instance of the <see cref="BuyProductGetGiftItemsValidator"/> class.
        /// </summary>
        /// <param name="localizationService">The localization service.</param>
        public BuyProductGetGiftItemsValidator(LocalizationService localizationService)
        {
            ParameterValidator.ThrowIfNull(() => localizationService, value: localizationService);
            this.localizationService = localizationService;
        }

        /// <summary>Validates the specified promotion.</summary>
        /// <param name="instance">The instance that will be validated.</param>
        /// <returns>Validation errors for any empty collection property.</returns>
        public IEnumerable<ValidationError> Validate(BuyProductGetGiftItems instance)
        {
            ParameterValidator.ThrowIfNull(() => instance, value: instance);
            List<ValidationError> validationErrors = new List<ValidationError>();
            this.AddErrorIfNoGiftItem(promotion: instance, validationErrors: validationErrors);
            return validationErrors;
        }

        /// <summary>
        /// Adds the error if no gift item.
        /// </summary>
        /// <param name="promotion">The promotion.</param>
        /// <param name="validationErrors">The validation errors.</param>
        private void AddErrorIfNoGiftItem(BuyProductGetGiftItems promotion, List<ValidationError> validationErrors)
        {
            if (promotion.GiftItems != null && promotion.GiftItems.Any())
            {
                return;
            }

            List<ValidationError> validationErrorList = validationErrors;
            ValidationError validationError = new ValidationError();
            validationError.Severity = ValidationErrorSeverity.Error;
            validationError.ValidationType = ValidationErrorType.StorageValidation;
            validationError.PropertyName = "GiftItems";
            string errorMessage = this.localizationService.GetString("/commerce/validation/nogiftitem");
            validationError.ErrorMessage = errorMessage;
            validationErrorList.Add(item: validationError);
        }
    }
}