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

    /// <summary>
    /// Class PromotionalBanner.
    /// </summary>
    public class PromotionalBanner
    {
        /// <summary>
        /// Gets or sets the banner URL.
        /// </summary>
        /// <value>The banner URL.</value>
        public Uri BannerUrl { get; set; }

        /// <summary>
        /// Gets or sets the campaign description.
        /// </summary>
        /// <value>The campaign description.</value>
        public string CampaignDescription { get; set; }

        /// <summary>
        /// Gets or sets the name of the campaign.
        /// </summary>
        /// <value>The name of the campaign.</value>
        public string CampaignName { get; set; }

        /// <summary>
        /// Gets or sets the promotion description.
        /// </summary>
        /// <value>The promotion description.</value>
        public string PromotionDescription { get; set; }

        /// <summary>
        /// Gets or sets the name of the promotion.
        /// </summary>
        /// <value>The name of the promotion.</value>
        public string PromotionName { get; set; }
    }
}