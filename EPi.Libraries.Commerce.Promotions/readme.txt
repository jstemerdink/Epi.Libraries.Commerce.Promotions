
Include in the package:

Custom promotions:
- Buy product, get gift item.
- Buy bundle, get item discount.

Coupon code provider:
If you want to add a more dynamic check for a coupon code to a promotion, you can use a "couponcode provider". 

You can attach the coupon provider to an existing promotion, or to a custom one. 
Though probably a custom promotion would be the best way to go, so you can hide the coupon code textbox.

Use "RemoteCouponProviderBase" as a base for your provider.

[ServiceConfiguration(typeof(IRemoteCouponProvider), Lifecycle = ServiceInstanceScope.Singleton)]
public class BuyFromCategoryGetItemDiscountRemoteCouponProvider : RemoteCouponProviderBase<BuyFromCategoryGetItemDiscount>
{
    public override string GetCouponCode()
    {
        return "RemoteCoupon";
    }
}


Don't forget to override the default implementation of the coupon filter.


services.AddSingleton<ICouponFilter, CustomCouponFilter>();


Extensions:
An extension to get all promotional banners (a custom class) for a market.
