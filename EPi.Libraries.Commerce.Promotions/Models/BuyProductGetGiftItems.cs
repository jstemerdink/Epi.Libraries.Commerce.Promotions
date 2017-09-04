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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Commerce.Marketing;
    using EPiServer.Commerce.Marketing.DataAnnotations;
    using EPiServer.Commerce.Marketing.Promotions;
    using EPiServer.Core;
    using EPiServer.DataAnnotations;

    /// <summary>
    /// Class BuyProductGetGiftItems.
    /// </summary>
    /// <seealso cref="EPiServer.Commerce.Marketing.EntryPromotion" />
    [ContentType(
        GUID = "8a820143-0b0e-46f4-a177-815c482e8510",
        GroupName = "entrypromotion",
        Order = 10500,
        DisplayName = "Buy product, get gift",
        Description = "Buy at least X items from categories/entries and get a gift.")]
    [ImageUrl("Images/SpendAmountGetGiftItems.png")]
    [CLSCompliant(false)]
    public class BuyProductGetGiftItems : EntryPromotion
    {
        /// <summary>
        /// Gets or sets the condition for the promotion that needs to be fulfilled before the discount is applied..
        /// </summary>
        /// <value>The condition.</value>
        [Display(Order = 10)]
        [PromotionRegion("Condition")]
        public virtual PurchaseQuantity Condition { get; set; }

        /// <summary>
        /// Gets or sets the gift items list that will be applied.
        /// </summary>
        /// <value>The gift items.</value>
        [Display(Order = 20)]
        [PromotionRegion("Reward")]
        [AllowedTypes(typeof(VariationContent), typeof(PackageContent))]
        public virtual IList<ContentReference> GiftItems { get; set; }
    }
}