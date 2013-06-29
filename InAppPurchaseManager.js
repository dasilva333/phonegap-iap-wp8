/** 
 * A phonegap plugin to enable WP8 In-App Purchases.
 * Author: Toby Kavukattu
 * Version: 0.1
 * License: MIT
 *
 * Based on the iOS plugin by Matt Kane & Guillaume Charhon
 * https://github.com/phonegap/phonegap-plugins/tree/master/iOS/InAppPurchaseManager)
 * https://github.com/usmart/InAppPurchaseManager-EXAMPLE)
 */


var InAppPurchaseManager = function() { 
	cordova.exec(null,null,'InAppPurchaseManager',"setup",[]);
}

/**
 * Makes an in-app purchase. 
 * 
 * @param {String} productId The product identifier. e.g. "com.example.MyApp.myproduct"
 * @param {int} quantity 
 */

InAppPurchaseManager.prototype.makePurchase = function(productId, quantity) {
	var q = parseInt(quantity);
	if(!q) {
		q = 1;
	}
	return cordova.exec(function (result) {
	    if (result == "PaymentTransactionStatePurchased") {
	        window.plugins.inAppPurchaseManager.onPurchased("",productId,"");
	    }
	}, null, 'InAppPurchaseManager', 'makePurchase', [productId, q]);
}

/**
 * Restores previously completed purchases.
 * The restored transactions are passed to the onRestored callback, so make sure you define a handler for that first.
 * 
 */

InAppPurchaseManager.prototype.restoreCompletedTransactions = function() {
    return cordova.exec(function (productId) {
        window.plugins.inAppPurchaseManager.onRestored("", productId, "");
    }, null, 'InAppPurchaseManager', 'restoreCompletedTransactions', []);
}


/**
 * Retrieves the localised product data, including price (as a localised string), name, description.
 *
 * @param {String} productId The product identifier. e.g. "ABC123"
 * @param {Function} successCallback Called once for each returned product id. Signature is function(productId, title, description, price)
 * @param {Function} failCallback Called once for each invalid product id. Signature is function(productId)
 */

InAppPurchaseManager.prototype.requestProductData = function(productId, successCallback, failCallback) {
cordova.exec(
        function (price) {
            successCallback(productId, "", "", price);
        },
        failCallback,
        'InAppPurchaseManager',
        'requestProductData', [productId]);
}

/**
 * Retrieves localised product data, including price (as localised
 * string), name, description of multiple products.
 *
 * @param {Array} productIds
 *   An array of product identifier strings.
 *
 * @param {Function} callback
 *   Called once with the result of the products request. Signature:
 *
 *     function(validProducts, invalidProductIds)
 *
 *   where validProducts receives an array of objects of the form
 *
 *     {
 *      id: "<productId>",
 *      title: "<localised title>",
 *      description: "<localised escription>",
 *      price: "<localised price>"
 *     }
 *
 *  and invalidProductIds receives an array of product identifier
 *  strings which were rejected by the app store.
 */
InAppPurchaseManager.prototype.requestProductsData = function (productIds, callback) {
    alert("Not Implemented Yet");
};

/* function(transactionIdentifier, productId, transactionReceipt) */
InAppPurchaseManager.prototype.onPurchased = null;

/* function(originalTransactionIdentifier, productId, originalTransactionReceipt) */
InAppPurchaseManager.prototype.onRestored = null;

/* function(errorCode, errorText) */
InAppPurchaseManager.prototype.onFailed = null;

cordova.addConstructor(function()  {
	// shim to work in 1.5 and 1.6
	if (!window.Cordova) {
		window.Cordova = cordova;
	};
					   
	if(!window.plugins) {
		window.plugins = {};
	}
	window.plugins.inAppPurchaseManager = InAppPurchaseManager.manager = new InAppPurchaseManager();
});