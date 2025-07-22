using System;
using System.Collections.Generic;
using System.Threading;
using CGK.Descriptor.Service;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using PurchaseWrapper.Descriptor;
using PurchaseWrapper.Error;
using PurchaseWrapper.Model;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;


namespace PurchaseWrapper.Service
{
    public class PurchaseService : IDetailedStoreListener
    {
        private const string ENVIRONMENT = "production";
        private const int INITIALIZATION_TIMEOUT_SECONDS = 10;

        private readonly IPurchaseDataHolder _purchaseDataHolder;
        private readonly PurchaseProductConfig _purchaseProduct;
        private readonly IPurchaseAnalytics _purchaseAnalytics;
        private readonly RestoreService _restoreService;
        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;
        private UniTaskCompletionSource _initCompletionSource;
        private UniTaskCompletionSource _purchaseCompletionSource;

        private bool _inited;

        public PurchaseService(DescriptorHolder descriptorHolder, IPurchaseDataHolder purchaseDataHolder,
            IPurchaseAnalytics purchaseAnalytics, RestoreService restoreService)
        {
            _restoreService = restoreService;
            _purchaseAnalytics = purchaseAnalytics;
            _purchaseDataHolder = purchaseDataHolder;
            _purchaseProduct = descriptorHolder.GetDescriptor<PurchaseProductConfig>();
        }

        public async UniTask Init()
        {

            var options = new InitializationOptions()
                .SetEnvironmentName(ENVIRONMENT);

            await UnityServices.InitializeAsync(options);
            _initCompletionSource = new UniTaskCompletionSource();

            ConfigurationBuilder builder = CreateConfigurations();
            
            UnityPurchasing.Initialize(this, builder);
            
            try
            {
                var timeoutTask = UniTask.Delay(TimeSpan.FromSeconds(INITIALIZATION_TIMEOUT_SECONDS), 
                    cancellationToken: CancellationToken.None);
                var result = await UniTask.WhenAny(_initCompletionSource.Task, timeoutTask);

                if (result == 1) // timeoutTask completed first
                {
                    throw new PurchaseInitError(InitializationFailureReason.PurchasingUnavailable, 
                        "Initialization timed out due to no internet connection or service unavailability");
                }
            }
            finally
            {
                _initCompletionSource = null;
            }

        }

        [CanBeNull]
        public Product GetProductInfo(string productId)
        {
            if (_storeController == null)
            {
                return null;
            }
            return _storeController.products.WithStoreSpecificID(productId);
        }

        public async UniTask Purchase(string productId)
        {
            _purchaseCompletionSource = new UniTaskCompletionSource();
            if (!_inited)
            {
                throw new PurchaseProcessionError(productId, PurchaseFailureReason.PurchasingUnavailable);
            }

            _storeController.InitiatePurchase(productId);
            await _purchaseCompletionSource.Task;
            _purchaseCompletionSource = null;
        }

        private ConfigurationBuilder CreateConfigurations()
        {
            StandardPurchasingModule module = StandardPurchasingModule.Instance();
            if (_purchaseProduct.Fake)
            {
                module.useFakeStoreUIMode = FakeStoreUIMode.Default;
                module.useFakeStoreAlways = true;
            }

            ConfigurationBuilder configurationBuilder =
                ConfigurationBuilder.Instance(module);

            foreach (PurchaseProductDescriptor purchaseProductDescriptor in _purchaseProduct.Products)
            {
                if (purchaseProductDescriptor.Overrides == null || purchaseProductDescriptor.Overrides.Count == 0)
                {
                    configurationBuilder.AddProduct(purchaseProductDescriptor.ProductId,
                        WrapProductType(purchaseProductDescriptor.PurchaseProductType));
                }
                else
                {
                    configurationBuilder.AddProduct(purchaseProductDescriptor.ProductId,
                        WrapProductType(purchaseProductDescriptor.PurchaseProductType),
                        OverrideProducts(purchaseProductDescriptor.Overrides));
                }
            }

            return configurationBuilder;
        }

        private IDs OverrideProducts(List<PurchaseProductOverride> overrides)
        {
            IDs ids = new IDs();
            foreach (PurchaseProductOverride purchaseProductOverride in overrides)
            {
                ids.Add(purchaseProductOverride.ProductId, WrapStores(purchaseProductOverride.StoreType));
            }

            return ids;
        }

        private string WrapStores(StoreType storeType)
        {
            switch (storeType)
            {
                case StoreType.GooglePlay:
                    return GooglePlay.Name;
                case StoreType.AppleStore:
                    return AppleAppStore.Name;
                default:
                    throw new ArgumentOutOfRangeException(nameof(storeType), storeType, null);
            }
        }

        private ProductType WrapProductType(PurchaseProductType purchaseProductType)
        {
            switch (purchaseProductType)
            {
                case PurchaseProductType.Consumable:
                    return ProductType.Consumable;
                case PurchaseProductType.NonConsumable:
                    return ProductType.NonConsumable;
                case PurchaseProductType.Subscription:
                    return ProductType.Subscription;
                default:
                    throw new ArgumentOutOfRangeException(nameof(purchaseProductType), purchaseProductType, null);
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            _initCompletionSource?.TrySetException(new PurchaseInitError(error));
            _inited = false;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            _initCompletionSource?.TrySetException(new PurchaseInitError(error, message));
            _inited = false;
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            bool validPurchase = true;

#if (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try
            {
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
                
            }
            catch (IAPSecurityException)
            {
                Debug.LogError("VALIDATION ERROR");
                validPurchase = false;
            }
#endif

            if (validPurchase || _purchaseProduct.Fake)
            {
                _purchaseDataHolder.SavePurchase(purchaseEvent.purchasedProduct.definition.id);

                if (_purchaseCompletionSource != null) // Null if was restoring product
                {
                    if (!_purchaseProduct.Fake)
                    {
                        _purchaseAnalytics.PurchaseCompleted(purchaseEvent.purchasedProduct);
                    }
                }
                else
                {
                    _restoreService.RestorePurchases(purchaseEvent.purchasedProduct.definition.id);
                }

                _purchaseCompletionSource?.TrySetResult();
            }
            else
            {
                _purchaseCompletionSource?.TrySetException(new PurchaseProcessionError(purchaseEvent.purchasedProduct.definition.id, PurchaseFailureReason.SignatureInvalid));
                Debug.LogError("Purchase validation failed. Skipping reward logic.");
            }

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            _purchaseCompletionSource?.TrySetException(
                new PurchaseProcessionError(product.definition.id, failureReason));
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            _purchaseCompletionSource?.TrySetException(
                new PurchaseProcessionError(product.definition.id, failureDescription.reason));
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
            _initCompletionSource?.TrySetResult();
            _inited = true;
        }
    }
}