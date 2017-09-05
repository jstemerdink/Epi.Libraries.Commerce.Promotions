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
namespace EPi.Libraries.Commerce.Promotions.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EPi.Libraries.Commerce.Promotions.Models;

    using EPiServer;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Commerce.Catalog.Linking;
    using EPiServer.Commerce.Marketing;
    using EPiServer.Commerce.Marketing.Promotions;
    using EPiServer.Commerce.Order;
    using EPiServer.Commerce.Validation;
    using EPiServer.Core;
    using EPiServer.Framework.Localization;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class BuyBundleGetItemDiscountProcessor.
    /// </summary>
    /// <seealso cref="EPiServer.Commerce.Marketing.Promotions.GetItemDiscountProcessorBase{BuyBundleGetItemDiscount}" />
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    [CLSCompliant(false)]
    public class BuyBundleGetItemDiscountProcessor : GetItemDiscountProcessorBase<BuyBundleGetItemDiscount>
    {
        /// <summary>
        /// The content loader
        /// </summary>
        private readonly IContentLoader contentLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuyBundleGetItemDiscountProcessor" /> class.
        /// </summary>
        /// <param name="collectionTargetEvaluator">The service that is used to evaluate the target properties.</param>
        /// <param name="localizationService">The service that is used to handle localization.</param>
        /// <param name="contentLoader">The content loader.</param>
        public BuyBundleGetItemDiscountProcessor(
            CollectionTargetEvaluator collectionTargetEvaluator,
            LocalizationService localizationService,
            IContentLoader contentLoader)
            : base(
                targetEvaluator: collectionTargetEvaluator,
                localizationService: localizationService,
                targetGetter: GetTargetItems,
                discountGetter: x => x.Discount)
        {
            ParameterValidator.ThrowIfNull(() => collectionTargetEvaluator, value: collectionTargetEvaluator);
            this.contentLoader = contentLoader;
        }

        /// <summary>
        /// Determines whether [contains all items] [the specified targets].
        /// </summary>
        /// <param name="lineItemCodes">The line item codes.</param>
        /// <param name="targetCodes">The target codes.</param>
        /// <returns><c>true</c> if [contains all items] [the specified targets]; otherwise, <c>false</c>.</returns>
        protected static bool ContainsAllItems(IEnumerable<string> lineItemCodes, IEnumerable<string> targetCodes)
        {
            return !targetCodes.Except(second: lineItemCodes).Any();
        }

        /// <summary>
        /// Gets the target items.
        /// </summary>
        /// <param name="promotionData">The promotion data.</param>
        /// <returns>The DiscountItems.</returns>
        protected static DiscountItems GetTargetItems(BuyBundleGetItemDiscount promotionData)
        {
            if (promotionData == null)
            {
                return new DiscountItems { Items = new List<ContentReference>(), MatchRecursive = false };
            }

            IEnumerable<ContentReference> entries = ListBundleEntries(referenceToBundle: promotionData.Bundle)
                .Select(e => e.Child);

            return new DiscountItems { Items = entries.ToList(), MatchRecursive = false };
        }

        /// <summary>
        /// Lists the bundle entries.
        /// </summary>
        /// <param name="referenceToBundle">The reference to bundle.</param>
        /// <returns>A list of <see cref="BundleEntry"/>.</returns>
        protected static IEnumerable<BundleEntry> ListBundleEntries(ContentReference referenceToBundle)
        {
            IRelationRepository relationRepository = ServiceLocator.Current.GetInstance<IRelationRepository>();

            // Relations to bundle entries are of type BundleEntry
            List<BundleEntry> bundleEntries = relationRepository.GetChildren<BundleEntry>(parentLink: referenceToBundle)
                .ToList();

            return bundleEntries;
        }

        /// <summary>
        /// Verify that the current promotion can potentially be fulfilled
        /// </summary>
        /// <param name="promotion">The promotion to evaluate.</param>
        /// <param name="context">The context for the promotion processor evaluation.</param>
        /// <returns><c>true</c> if the current promotion can potentially be fulfilled; otherwise, <c>false</c>.</returns>
        /// <remarks>This method is intended to be a very quick pre-check to avoid doing more expensive operations.
        /// Used to verify basic things, for example a Buy-3-pay-for-2 promotion needs at least three items in the cart.
        /// If we have less than three we can skip further processing.</remarks>
        protected override bool CanBeFulfilled(
            BuyBundleGetItemDiscount promotion,
            PromotionProcessorContext context)
        {
            if (promotion == null)
            {
                return false;
            }

            if (context == null)
            {
                return false;
            }

            if (base.CanBeFulfilled(promotion: promotion, context: context))
            {
                return !ContentReference.IsNullOrEmpty(contentLink: promotion.Bundle);
            }

            return false;
        }

        /// <summary>
        /// Implements promotion specific logic for determining the fulfillment status of the promotion.
        /// </summary>
        /// <param name="promotionData">The promotion data.</param>
        /// <param name="context">The context.</param>
        /// <returns>The calculated fulfillment status as a <see cref="T:EPiServer.Commerce.Marketing.FulfillmentStatus" /> value.</returns>
        protected override FulfillmentStatus GetFulfillmentStatus(
            BuyBundleGetItemDiscount promotionData,
            PromotionProcessorContext context)
        {
            if (promotionData == null)
            {
                return FulfillmentStatus.NotFulfilled;
            }

            if (context == null)
            {
                return FulfillmentStatus.NotFulfilled;
            }

            IEnumerable<ILineItem> lineItems = context.OrderForm.GetAllLineItems().Where(item => !item.IsGift);

            IEnumerable<ContentReference> targets = ListBundleEntries(referenceToBundle: promotionData.Bundle)
                .Select(e => e.Child);

            IList<string> applicableCodes = targets
                .Select(
                    contentReference => this.contentLoader.Get<EntryContentBase>(contentLink: contentReference)
                        ?.Code).Where(code => !string.IsNullOrWhiteSpace(value: code)).ToList();

            return this.GetStatusForBuyBundlePromotion(
                codes: applicableCodes,
                lineItems: lineItems);
        }

        /// <summary>
        /// Gets information about the settings for a specific instance of a promotion type.
        /// Used when displaying promotion information to a site visitor/shopper.
        /// </summary>
        /// <param name="promotionData">The promotion data to get items for.</param>
        /// <returns>The promotion condition and reward items.</returns>
        /// <remarks><para>
        /// This method is intended to be used on a site to display information about a promotion to a visitor/shopper.
        /// </para>
        /// <para>
        /// It is never used during the evaluation of the promotion, it only exists to provide information about the settings for this instance of a promotion type.
        /// So a use case for this could be that you have a "Buy 3 get the cheapest for one for free" promotion. And you want to display information to the visitor/shopper
        /// that "If you buy three items from the category cooking books, you will get the cheapest one for free".
        /// </para>
        /// <para>
        /// This method should not be called explicitly from the site code, but will be called from the IPromotionEngine extension method GetPromotionItemsForCampaign.
        /// </para></remarks>
        protected override PromotionItems GetPromotionItems(BuyBundleGetItemDiscount promotionData)
        {
            CatalogItemSelection catalogItemSelection;

            if (promotionData == null)
            {
                catalogItemSelection = new CatalogItemSelection(
                    Enumerable.Empty<ContentReference>(),
                    type: CatalogItemSelectionType.Specific,
                    includesSubcategories: false);

                return new PromotionItems(
                    null,
                    condition: catalogItemSelection,
                    reward: catalogItemSelection);
            }

            IEnumerable<ContentReference> entries = ListBundleEntries(referenceToBundle: promotionData.Bundle).Select(e => e.Child);

            catalogItemSelection = new CatalogItemSelection(
                items: entries,
                type: CatalogItemSelectionType.Specific,
                includesSubcategories: false);

            return new PromotionItems(
                promotion: promotionData,
                condition: catalogItemSelection,
                reward: catalogItemSelection);
        }

        /// <summary>
        /// Gets all <see cref="T:EPiServer.Commerce.Marketing.AffectedEntries" />s affected by a given promotion.
        /// </summary>
        /// <param name="promotionData">The promotion used to evaluate the product codes.</param>
        /// <param name="context">The context for the promotion processor evaluation.</param>
        /// <param name="applicableCodes">A collection of product codes to be checked against a promotion.</param>
        /// <returns>A list of applicable <see cref="T:EPiServer.Commerce.Marketing.RedemptionDescription" /></returns>
        protected override IEnumerable<RedemptionDescription> GetRedemptions(
            BuyBundleGetItemDiscount promotionData,
            PromotionProcessorContext context,
            IEnumerable<string> applicableCodes)
        {
            List<RedemptionDescription> redemptionDescriptionList = new List<RedemptionDescription>();

            if (promotionData == null)
            {
                return redemptionDescriptionList;
            }

            if (context == null)
            {
                return redemptionDescriptionList;
            }

            decimal val2 = this.GetLineItems(orderForm: context.OrderForm)
                .Where(li => applicableCodes.Contains(value: li.Code)).Sum(li => li.Quantity);

            decimal num = Math.Min(this.GetMaxRedemptions(redemptions: promotionData.RedemptionLimits), val2: val2);

            for (int index = 0; index < num; ++index)
            {
                AffectedEntries entries =
                    context.EntryPrices.ExtractEntries(codes: applicableCodes, quantity: decimal.One);
                if (entries != null)
                {
                    redemptionDescriptionList.Add(this.CreateRedemptionDescription(affectedEntries: entries));
                }
            }

            return redemptionDescriptionList;
        }

        /// <summary>
        /// Gets the fulfillment status using <paramref name="codes" /> for Buy from category get item discount promotion.
        /// </summary>
        /// <param name="codes">The codes for eligible products.</param>
        /// <param name="lineItems">The line items in current order form.</param>
        /// <returns>The fulfillment status.</returns>
        protected FulfillmentStatus GetStatusForBuyBundlePromotion(
            IEnumerable<string> codes,
            IEnumerable<ILineItem> lineItems)
        {
            return !ContainsAllItems(lineItems.Select(l => l.Code), targetCodes: codes)
                       ? FulfillmentStatus.NotFulfilled
                       : FulfillmentStatus.Fulfilled;
        }
    }
}