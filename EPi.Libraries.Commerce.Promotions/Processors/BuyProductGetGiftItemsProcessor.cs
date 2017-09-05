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

    using EPiServer.Commerce.Extensions;
    using EPiServer.Commerce.Marketing;
    using EPiServer.Commerce.Marketing.Extensions;
    using EPiServer.Commerce.Order;
    using EPiServer.Commerce.Validation;
    using EPiServer.Core;
    using EPiServer.Framework.Localization;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// The processor responsible for evaluating if a promotion of type <see cref="T:EPi.Libraries.Commerce.Promotions.Models.BuyProductGetGiftItems" /> should
    /// apply a reward to an order group.
    /// </summary>
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    [CLSCompliant(false)]
    public class BuyProductGetGiftItemsProcessor : EntryPromotionProcessorBase<BuyProductGetGiftItems>
    {
        /// <summary>
        /// The fulfillment evaluator
        /// </summary>
        private readonly FulfillmentEvaluator fulfillmentEvaluator;

        /// <summary>
        /// The gift item factory
        /// </summary>
        private readonly GiftItemFactory giftItemFactory;

        /// <summary>
        /// The localization service
        /// </summary>
        private readonly LocalizationService localizationService;

        /// <summary>
        /// The target evaluator
        /// </summary>
        private readonly CollectionTargetEvaluator targetEvaluator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuyProductGetGiftItemsProcessor" /> class.
        /// </summary>
        /// <param name="targetEvaluator">The target evaluator.</param>
        /// <param name="fulfillmentEvaluator">The service that is used to evaluate the fulfillment status of the promotion.</param>
        /// <param name="giftItemFactory">The service that is used to get applicable gift items.</param>
        /// <param name="localizationService">Service to handle localization of text strings.</param>
        public BuyProductGetGiftItemsProcessor(
            CollectionTargetEvaluator targetEvaluator,
            FulfillmentEvaluator fulfillmentEvaluator,
            GiftItemFactory giftItemFactory,
            LocalizationService localizationService)
        {
            ParameterValidator.ThrowIfNull(() => targetEvaluator, value: targetEvaluator);
            ParameterValidator.ThrowIfNull(() => fulfillmentEvaluator, value: fulfillmentEvaluator);
            ParameterValidator.ThrowIfNull(() => giftItemFactory, value: giftItemFactory);
            ParameterValidator.ThrowIfNull(() => localizationService, value: localizationService);

            this.targetEvaluator = targetEvaluator;
            this.fulfillmentEvaluator = fulfillmentEvaluator;
            this.giftItemFactory = giftItemFactory;
            this.localizationService = localizationService;
        }

        /// <summary>
        /// Verify that the current promotion can potentially be fulfilled
        /// </summary>
        /// <param name="promotionData">The promotion to evaluate.</param>
        /// <param name="context">The context for the promotion processor evaluation.</param>
        /// <returns><c>true</c> if the current promotion can potentially be fulfilled; otherwise, <c>false</c>.</returns>
        /// <remarks>This method is intended to be a very quick pre-check to avoid doing more expensive operations.
        /// Used to verify basic things, for example a Buy-3-pay-for-2 promotion needs at least three items in the cart.
        /// If we have less than three we can skip further processing.</remarks>
        /// <exception cref="ArgumentNullException">Line or discount items is null.</exception>
        protected override bool CanBeFulfilled(BuyProductGetGiftItems promotionData, PromotionProcessorContext context)
        {
            if (promotionData == null)
            {
                return false;
            }

            if (context == null)
            {
                return false;
            }

            IEnumerable<ILineItem> lineItems = this.GetLineItems(orderForm: context.OrderForm);

            if (lineItems.Any() && (promotionData?.GiftItems != null && promotionData.GiftItems.Any()))
            {
                return promotionData.GiftItems.Any();
            }

            return false;
        }

        /// <summary>
        /// Evaluates a promotion against an order form.  Implementations should use context.OrderForm for evaluations.
        /// </summary>
        /// <param name="promotionData">The promotion to evaluate.</param>
        /// <param name="context">The context for the promotion processor evaluation.</param>
        /// <returns>A <see cref="T:EPiServer.Commerce.Marketing.RewardDescription" /> telling whether the promotion was fulfilled,
        /// which items the promotion was applied to and to which amount.</returns>
        /// <exception cref="ArgumentNullException">Applicable codes is null.</exception>
        /// <exception cref="OverflowException">The sum for the quantities is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
        protected override RewardDescription Evaluate(
            BuyProductGetGiftItems promotionData,
            PromotionProcessorContext context)
        {
            if (promotionData == null)
            {
                return this.NotFulfilledRewardDescription(
                    null,
                    context: context,
                    fulfillmentStatus: FulfillmentStatus.NotFulfilled);
            }

            if (context == null)
            {
                return this.NotFulfilledRewardDescription(
                    promotionData: promotionData,
                    context: null,
                    fulfillmentStatus: FulfillmentStatus.NotFulfilled);
            }

            FulfillmentStatus fulfillmentStatus = promotionData.Condition.GetFulfillmentStatus(
                orderForm: context.OrderForm,
                targetEvaluator: this.targetEvaluator,
                fulfillmentEvaluator: this.fulfillmentEvaluator);

            if (!fulfillmentStatus.HasFlag(flag: FulfillmentStatus.Fulfilled))
            {
                return this.NotFulfilledRewardDescription(
                    promotionData: promotionData,
                    context: context,
                    fulfillmentStatus: fulfillmentStatus);
            }

            IEnumerable<ILineItem> lineItems = this.GetLineItems(orderForm: context.OrderForm);

            IList<string> applicableCodes = this.targetEvaluator.GetApplicableCodes(
                lineItemsInOrder: lineItems,
                targets: promotionData.Condition.Items,
                matchRecursive: true);

            if (!applicableCodes.Any())
            {
                return this.NotFulfilledRewardDescription(
                    promotionData: promotionData,
                    context: context,
                    fulfillmentStatus: FulfillmentStatus.NotFulfilled);
            }

            IEnumerable<RedemptionDescription> redemptions = this.GetRedemptions(
                promotionData: promotionData,
                context: context,
                applicableCodes: applicableCodes);

            return RewardDescription.CreateGiftItemsReward(
                status: fulfillmentStatus,
                redemptions: redemptions,
                promotion: promotionData,
                description: fulfillmentStatus.GetRewardDescriptionText(localizationService: this.localizationService));
        }

        /// <summary>
        /// Gets the items for a promotion.
        /// </summary>
        /// <param name="promotionData">The promotion data to get items for.</param>
        /// <returns>The promotion condition and reward items.</returns>
        protected override PromotionItems GetPromotionItems(BuyProductGetGiftItems promotionData)
        {
            
            if (promotionData == null)
            {
                return new PromotionItems(
                    null,
                    new CatalogItemSelection(
                        null,
                        type: CatalogItemSelectionType.All,
                        includesSubcategories: true),
                    new CatalogItemSelection(
                        Enumerable.Empty<ContentReference>(),
                        type: CatalogItemSelectionType.Specific,
                        includesSubcategories: false));
            }

            return new PromotionItems(
                promotion: promotionData,
                condition: new CatalogItemSelection(
                    null,
                    type: CatalogItemSelectionType.All,
                    includesSubcategories: true),
                reward: new CatalogItemSelection(
                    items: promotionData.GiftItems,
                    type: CatalogItemSelectionType.Specific,
                    includesSubcategories: false));
        }

        /// <summary>
        /// Gets the redemptions.
        /// </summary>
        /// <param name="promotionData">The promotion data.</param>
        /// <param name="context">The context.</param>
        /// <param name="applicableCodes">The applicable codes.</param>
        /// <returns>A list of <see cref="RedemptionDescription"/>.</returns>
        /// <exception cref="ArgumentNullException">Line items or applicable codes is null.</exception>
        /// <exception cref="OverflowException">The sum for the quantities is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
        protected IEnumerable<RedemptionDescription> GetRedemptions(
            BuyProductGetGiftItems promotionData,
            PromotionProcessorContext context,
            IEnumerable<string> applicableCodes)
        {
            if (promotionData == null)
            {
                return Enumerable.Empty<RedemptionDescription>();
            }

            if (context == null)
            {
                return Enumerable.Empty<RedemptionDescription>();
            }

            decimal quantity = this.GetLineItems(orderForm: context.OrderForm)
                .Where(li => applicableCodes.Contains(value: li.Code)).Sum(li => li.Quantity);

            if (quantity < promotionData.Condition.RequiredQuantity)
            {
                return Enumerable.Empty<RedemptionDescription>();
            }

            AffectedEntries giftItems =
                this.giftItemFactory.CreateGiftItems(entryLinks: promotionData.GiftItems, processorContext: context);

            return giftItems == null
                       ? Enumerable.Empty<RedemptionDescription>()
                       : new[] { this.CreateRedemptionDescription(affectedEntries: giftItems) };
        }

        /// <summary>
        /// Not fulfilled reward description. Will be returned when CanBeFulfilled is false.
        /// </summary>
        /// <param name="promotionData">The promotion that was evaluated.</param>
        /// <param name="context">The context for the promotion processor evaluation.</param>
        /// <param name="fulfillmentStatus">The fulfillment level of the promotion.</param>
        /// <returns>A <see cref="T:EPiServer.Commerce.Marketing.RewardDescription" /> for the not fulfilled promotion.</returns>
        protected override RewardDescription NotFulfilledRewardDescription(
            BuyProductGetGiftItems promotionData,
            PromotionProcessorContext context,
            FulfillmentStatus fulfillmentStatus)
        {
            return RewardDescription.CreateGiftItemsReward(
                status: fulfillmentStatus,
                redemptions: Enumerable.Empty<RedemptionDescription>(),
                promotion: promotionData,
                description: FulfillmentStatus.NotFulfilled.GetRewardDescriptionText(localizationService: this.localizationService));
        }
    }
}