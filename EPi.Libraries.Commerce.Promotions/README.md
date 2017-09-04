# EPi.Libraries.Commerce.Promotions

[![Build status](https://ci.appveyor.com/api/projects/status/a51v82pm4p374ojs/branch/master?svg=true)](https://ci.appveyor.com/project/jstemerdink/epi-libraries-promotions/branch/master)
[![Quality Gate](https://sonarqube.com/api/badges/gate?key=jstemerdink:EPi.Libraries.Commerce.Promotions)](https://sonarqube.com/dashboard/index/jstemerdink:EPi.Libraries.Commerce.Promotions)
[![GitHub version](https://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Commerce.Promotions.svg)](https://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Commerce.Promotions)
[![Platform](https://img.shields.io/badge/platform-.NET%204.6.1-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/EPiServer%20Commerce-%2011.0.0-orange.svg?style=flat)](http://world.episerver.com/commerce/)
[![GitHub license](https://img.shields.io/badge/license-MIT%20license-blue.svg?style=flat)](LICENSE)
[![Stories in Backlog](https://badge.waffle.io/jstemerdink/EPi.Libraries.Commerce.Promotions.svg?label=enhancement&title=Backlog)](http://waffle.io/jstemerdink/EPi.Libraries.Commerce.Promotions)

## About
This projects collects some things regarding promotions that I blogged about.

## Content

### Custom promotions
I have added some custom promotions that may be useful.

*Buy product, get gift item.* Read my blog post about it [here](https://jstemerdink.wordpress.com/2017/02/28/buy-products-get-gift/)

*Buy bundle, get item discount.* Read my blog post about it [here](https://jstemerdink.wordpress.com/2017/02/28/buy-products-get-gift/)

### Coupon code provider
If you want to add a more dynamic check for a coupon code to a promotion, you can use a "couponcode provider". 

You can attach the coupon provider to an existing promotion, or to a custom one. 
Though probably a custom promotion would be the best way to go, so you can hide the coupon code textbox.

Use ```RemoteCouponProviderBase``` as a base for your provider.

```
[ServiceConfiguration(typeof(IRemoteCouponProvider), Lifecycle = ServiceInstanceScope.Singleton)]
public class BuyFromCategoryGetItemDiscountRemoteCouponProvider : RemoteCouponProviderBase<BuyFromCategoryGetItemDiscount>
{
    public override string GetCouponCode()
    {
        return "RemoteCoupon";
    }
}
```

Don't forget to override the default implementation of the coupon filter.

```
services.AddSingleton<ICouponFilter, CustomCouponFilter>();
```

Read my blog post about it [here](https://jstemerdink.wordpress.com/2017/08/16/a-custom-coupon-code-provider/)

### Extensions

An extension to get all promotional banners (a custom class) for a market.

Read my blog post about it [here](https://jstemerdink.wordpress.com/2016/07/18/new-promotions-and-their-banners/)



> *Powered by ReSharper*