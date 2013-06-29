/*
 * A phonegap plugin to enable WP8 In-App Purchases.
 * Author: Toby Kavukattu
 * Version: 0.1
 * License: MIT
 * 
 * Based on the iOS plugin by Matt Kane & Guillaume Charhon
 * https://github.com/phonegap/phonegap-plugins/tree/master/iOS/InAppPurchaseManager)
 * https://github.com/usmart/InAppPurchaseManager-EXAMPLE)
 */


using System;
using System.Collections.Generic;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.JSON;
using WPCordovaClassLib.Cordova.Commands;
#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
#else
using Windows.ApplicationModel.Store;
#endif

using System.Windows.Threading;

namespace Cordova.Extension.Commands
{
    public class InAppPurchaseManager : BaseCommand
    {
        public void setup(string args)
        {
            this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK)); 
        }

        public async void restoreCompletedTransactions(string args)
        {
            try
            {
                // get all in-app products for current app
                ListingInformation productList = await CurrentApp.LoadListingInformationAsync();
 
                foreach (KeyValuePair<string, ProductListing> product in productList.ProductListings)
                {
                    ProductLicense productLicense = null;
                    if (CurrentApp.LicenseInformation.ProductLicenses.TryGetValue(product.Key, out productLicense))
                    {
                        if (productLicense.IsActive)
                        {
                            this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK, product.Key));
                        }
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                this.DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Unknown Error"));
            }
        }

        public async void requestProductData(string args)
        {
            try
            {
                string InAppProductKey = JsonHelper.Deserialize<string[]>(args)[0];

                // get specific in-app product by ID
                ListingInformation products = await CurrentApp.LoadListingInformationByProductIdsAsync(new string[] { InAppProductKey });

                ProductListing productListing = null;
                if (!products.ProductListings.TryGetValue(InAppProductKey, out productListing))
                {
                    this.DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Could not find product information"));
                    return;
                }
                

                System.Diagnostics.Debug.WriteLine(productListing.FormattedPrice);

                var results = new Dictionary<string, string>();
                results["productId"]= InAppProductKey;
                results["title"] = productListing.Name;

                this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK, productListing.FormattedPrice));
                return;
            }
            
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                this.DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Unknown Error"));
            }
             
        }

        public async void makePurchase(string args)
        {
            try
            {
                string InAppProductKey = JsonHelper.Deserialize<string[]>(args)[0];


                // get specific in-app product by ID
                ListingInformation products = await CurrentApp.LoadListingInformationByProductIdsAsync(new string[] { InAppProductKey });

                ProductListing productListing = null;
                if (!products.ProductListings.TryGetValue(InAppProductKey, out productListing))
                {
                    this.DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Could not find product information"));
                    return;
                }

                // RequestProductPurchaseAsync requires use of the UI thread, so we use the dispatcher
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(async () => {
                    try
                    {
                        await CurrentApp.RequestProductPurchaseAsync(productListing.ProductId, false);

                        ProductLicense productLicense = null;
                        if (CurrentApp.LicenseInformation.ProductLicenses.TryGetValue(InAppProductKey, out productLicense))
                        {
                            if (productLicense.IsActive)
                            {
                                this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK, "PaymentTransactionStatePurchased"));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //User cancelled the purchase
                        //this.DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Unknown Error"));
                    }
                });

                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                this.DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Unknown Error"));
            }
        }
    
    }
}