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
namespace EPi.Libraries.Commerce.Promotions.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Commerce.Marketing;
    using EPiServer.Commerce.Marketing.DataAnnotations;
    using EPiServer.Core;
    using EPiServer.DataAnnotations;

    /// <summary>
    /// The <see cref="P:BuyBundleGetItemDiscount.Discount" /> will be applied to any SKUs that are part of the <see cref="P:BuyBundleGetItemDiscount.Bundle" />.
    /// </summary>
    [ContentType(GUID = "59631059-835F-436E-B164-AA43F31A93EF", GroupName = "entrypromotion", Order = 10900, DisplayName = "Buy bundle, get discount")]
    [ImageUrl("Images/BuyFromCategoryGetItemDiscount.png")]
    [CLSCompliant(false)]
    public class BuyBundleGetItemDiscount : EntryPromotion
    {
        /// <summary>
        /// Gets or sets the Bundle. Any SKUs that belong to this bundle will get a discount.
        /// </summary>
        /// <value>The category.</value>
        [AllowedTypes(typeof(BundleContent))]
        [Display(Order = 10)]
        [PromotionRegion("Condition")]
        [UIHint("catalogentry")]
        public virtual ContentReference Bundle { get; set; }

        /// <summary>
        /// Gets or sets the discount. The reward values that should be applied.
        /// </summary>
        /// <value>The discount.</value>
        [Display(Order = 20)]
        [PromotionRegion("Discount")]
        public virtual MonetaryReward Discount { get; set; }
    }
}