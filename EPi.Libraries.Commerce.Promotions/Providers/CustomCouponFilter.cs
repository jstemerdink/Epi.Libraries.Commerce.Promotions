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
namespace EPi.Libraries.Commerce.Promotions.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EPi.Libraries.Commerce.Promotions.Contracts;

    using EPiServer;
    using EPiServer.Commerce.Marketing;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class CustomCouponFilter.
    /// </summary>
    /// <seealso cref="ICouponFilter" />
    [ServiceConfiguration(typeof(ICouponFilter), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CustomCouponFilter : ICouponFilter
    {
        /// <summary>
        /// Filters promotions by supplied coupon codes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <param name="couponCodes">The coupon codes added to the current order form.</param>
        /// <returns>The same <see cref="T:EPiServer.Commerce.Marketing.PromotionFilterContext" /> after filtering
        /// <see cref="P:EPiServer.Commerce.Marketing.PromotionFilterContext.IncludedPromotions" /> and adding applied coupon
        /// codes using <see cref="M:EPiServer.Commerce.Marketing.PromotionFilterContext.AddCouponCode(System.Guid,System.String)" />.</returns>
        public virtual PromotionFilterContext Filter(
            PromotionFilterContext filterContext,
            IEnumerable<string> couponCodes)
        {
            if (filterContext == null)
            {
                return null;
            }

            List<string> codes = couponCodes.ToList();

            foreach (PromotionData includedPromotion in filterContext.IncludedPromotions)
            {
                if (includedPromotion == null)
                {
                    continue;
                }

                IRemoteCouponProvider remoteCouponProvider =
                    GetRemoteCouponCodeProviderForPromotion(promotionData: includedPromotion);

                string code = remoteCouponProvider == null
                                  ? includedPromotion.Coupon?.Code ?? string.Empty
                                  : remoteCouponProvider.GetCouponCode();

                if (string.IsNullOrEmpty(value: code))
                {
                    continue;
                }

                if (codes.Contains(value: code, comparer: this.GetCodeEqualityComparer()))
                {
                    filterContext.AddCouponCode(promotionGuid: includedPromotion.ContentGuid, promotionCode: code);
                }
                else
                {
                    filterContext.ExcludePromotion(
                        promotion: includedPromotion,
                        reason: FulfillmentStatus.CouponCodeRequired,
                        createDescription: filterContext.RequestedStatuses.HasFlag(flag: RequestFulfillmentStatus.NotFulfilled));
                }
            }

            return filterContext;
        }

        /// <summary>
        /// Gets the comparer used when comparing the supplied coupon codes with the ones defined on the promotions.
        /// </summary>
        /// <returns><see cref="P:System.StringComparer.OrdinalIgnoreCase" /></returns>
        protected virtual IEqualityComparer<string> GetCodeEqualityComparer()
        {
            return StringComparer.OrdinalIgnoreCase;
        }

        /// <summary>
        /// Gets the remote coupon code provider.
        /// </summary>
        /// <param name="promotionData">The promotion data.</param>
        /// <returns>An instance of IRemoteCouponProvider.</returns>
        private static IRemoteCouponProvider GetRemoteCouponCodeProviderForPromotion(PromotionData promotionData)
        {
            Type promotionType = promotionData.GetOriginalType();

            IRemoteCouponProvider provider = ServiceLocator.Current.GetAllInstances<IRemoteCouponProvider>()
                .SingleOrDefault(p => p.ProviderFor == promotionType);

            return provider;
        }
    }
}