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
namespace EPi.Libraries.Commerce.Promotions.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using EPi.Libraries.Commerce.Promotions.Models;

    using EPiServer;
    using EPiServer.Commerce.Marketing;
    using EPiServer.Commerce.Marketing.Internal;
    using EPiServer.Core;
    using EPiServer.Logging;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;
    using EPiServer.Web.Routing;

    using Mediachase.Commerce;

    /// <summary>
    /// Class PromotionExtensions.
    /// </summary>
    public static class PromotionExtensions
    {
        /// <summary>
        /// The content loader
        /// </summary>
        private static IContentLoader contentLoader;

        /// <summary>
        /// The promotion engine content loader
        /// </summary>
        private static PromotionEngineContentLoader promotionEngineContentLoader;

        /// <summary>
        /// The promotion filters
        /// </summary>
        private static PromotionFilters promotionFilters;

        /// <summary>
        /// The logger
        /// </summary>
        private static ILogger logger;

        /// <summary>
        ///     Gets or sets the content loader.
        /// </summary>
        /// <value>The content loader instance.</value>
        /// <exception cref="ActivationException">if there is are errors resolving the service instance.</exception>
        public static IContentLoader ContentLoader
        {
            get
            {
                if (contentLoader != null)
                {
                    return contentLoader;
                }

                contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

                return contentLoader;
            }

            set => contentLoader = value;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        /// <exception cref="ActivationException">if there is are errors resolving the service instance.</exception>
        public static ILogger Logger
        {
            get
            {
                if (logger != null)
                {
                    return logger;
                }

                logger = ServiceLocator.Current.GetInstance<ILogger>();

                return logger;
            }

            set => logger = value;
        }

        /// <summary>
        ///     Gets or sets the promotion engine content loader.
        /// </summary>
        /// <value>The promotion engine content loader instance.</value>
        /// <exception cref="ActivationException">if there is are errors resolving the service instance.</exception>
        public static PromotionEngineContentLoader PromotionEngineContentLoader
        {
            get
            {
                if (promotionEngineContentLoader != null)
                {
                    return promotionEngineContentLoader;
                }

                promotionEngineContentLoader = ServiceLocator.Current.GetInstance<PromotionEngineContentLoader>();

                return promotionEngineContentLoader;
            }

            set => promotionEngineContentLoader = value;
        }

        /// <summary>
        ///     Gets or sets the promotion filters.
        /// </summary>
        /// <value>The promotion filters instance.</value>
        /// <exception cref="ActivationException">if there is are errors resolving the service instance.</exception>
        public static PromotionFilters PromotionFilters
        {
            get
            {
                if (promotionFilters != null)
                {
                    return promotionFilters;
                }

                promotionFilters = ServiceLocator.Current.GetInstance<PromotionFilters>();

                return promotionFilters;
            }

            set => promotionFilters = value;
        }

        /// <summary>
        /// Gets all promotional banners for market.
        /// </summary>
        /// <param name="market">The market.</param>
        /// <returns>A readonly collection of <see cref="PromotionalBanner" />.</returns>
        /// <exception cref="ActivationException">if there is are errors resolving the service instance.</exception>
        /// <exception cref="ArgumentNullException">if an argument for the logger is null.</exception>
        public static ReadOnlyCollection<PromotionalBanner> GetAllPromotionalBannersForMarket(this IMarket market)
        {
            List<PromotionalBanner> promotionBanners = new List<PromotionalBanner>();

            try
            {
                IList<PromotionData> promotions =
                    PromotionEngineContentLoader.GetEvaluablePromotionsInPriorityOrder(market: market);

                PromotionFilterContext promotionFilterContext = PromotionFilters.Filter(
                    allPromotions: promotions,
                    couponCodes: new List<string>(),
                    requestedStatus: RequestFulfillmentStatus.None);

                foreach (PromotionData promotionData in promotionFilterContext.IncludedPromotions.Where(
                    pd => !ContentReference.IsNullOrEmpty(contentLink: pd.Banner)))
                {
                    string imageUrl = UrlResolver.Current.GetUrl(
                        contentLink: promotionData.Banner,
                        language: null,
                        virtualPathArguments: new VirtualPathArguments { ContextMode = ContextMode.Default });

                    if (string.IsNullOrWhiteSpace(value: imageUrl))
                    {
                        continue;
                    }

                    string campaignName = null;
                    string campaignDescription = null;

                    SalesCampaign campaign = ContentLoader.Get<SalesCampaign>(contentLink: promotionData.ParentLink);

                    if (campaign != null)
                    {
                        campaignName = campaign.Name;

                        campaignDescription = campaign.Description;
                    }

                    PromotionalBanner promotionalBanner =
                        new PromotionalBanner
                            {
                                BannerUrl = new Uri(imageUrl),
                                PromotionDescription = promotionData.Description ?? string.Empty,
                                PromotionName = promotionData.Name ?? string.Empty,
                                CampaignDescription = campaignDescription ?? string.Empty,
                                CampaignName = campaignName ?? string.Empty
                            };

                    promotionBanners.Add(item: promotionalBanner);
                }
            }
            catch (ActivationException activationException)
            {
                Logger.Log(level: Level.Error, message: activationException.Message, exception: activationException);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Logger.Log(level: Level.Error, message: argumentNullException.Message, exception: argumentNullException);
            }
            catch (TypeMismatchException typeMismatchException)
            {
                Logger.Log(level: Level.Error, message: typeMismatchException.Message, exception: typeMismatchException);
            }
            catch (ContentNotFoundException contentNotFoundException)
            {
                Logger.Log(level: Level.Error, message: contentNotFoundException.Message, exception: contentNotFoundException);
            }

            return new ReadOnlyCollection<PromotionalBanner>(list: promotionBanners);
        }
    }
}