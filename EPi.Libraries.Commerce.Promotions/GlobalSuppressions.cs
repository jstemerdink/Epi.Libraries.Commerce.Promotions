
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Code Smell", "S3904:Assemblies should have version information", Justification = "Will be added by GitVersion")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1016:MarkAssembliesWithAssemblyVersion")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "EPi.Libraries.Commerce.Promotions.Contracts.IRemoteCouponProvider.#GetCouponCode()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Scope = "member", Target = "EPi.Libraries.Commerce.Promotions.Validators.BuyBundleGetItemDiscountValidator.#AddErrorsIfNeeded(EPi.Libraries.Commerce.Promotions.Models.BuyBundleGetItemDiscount,System.Collections.Generic.List`1<EPiServer.Validation.ValidationError>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "EPi.Libraries.Commerce.Promotions.Validators.BuyBundleGetItemDiscountValidator.#AddErrorsIfNeeded(EPi.Libraries.Commerce.Promotions.Models.BuyBundleGetItemDiscount,System.Collections.Generic.List`1<EPiServer.Validation.ValidationError>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "EPi.Libraries.Commerce.Promotions.Providers.CustomCouponFilter.#GetCodeEqualityComparer()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Scope = "member", Target = "EPi.Libraries.Commerce.Promotions.Models.BuyProductGetGiftItems.#GiftItems")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "EPi.Libraries.Commerce.Promotions.Processors.BuyBundleGetItemDiscountProcessor.#GetStatusForBuyBundlePromotion(System.Collections.Generic.IEnumerable`1<System.String>,System.Collections.Generic.IEnumerable`1<EPiServer.Commerce.Order.ILineItem>)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "member", Target = "EPi.Libraries.Commerce.Promotions.Extensions.PromotionExtensions.#GetAllPromotionalBannersForMarket(Mediachase.Commerce.IMarket)")]

