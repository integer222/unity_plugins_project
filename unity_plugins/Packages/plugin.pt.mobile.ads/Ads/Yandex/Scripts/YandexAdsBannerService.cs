using System;
using System.Threading;
using System.Threading.Tasks;
using Plugins.Ads.Api;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Plugins.Ads.Yandex
{
    public class YandexAdsBannerService : IAdsBannerService
    {
        private readonly string _id;
        private readonly int _refreshMilliseconds;
        private Banner _banner;
        private bool _isBannerShow;
        private CancellationTokenSource _source;

        private bool _isAutoUpdate;
        private int _errorRefreshMilliseconds;

        public YandexAdsBannerService(string id, int refreshMilliseconds, int errorRefreshMilliseconds)
        {
            _id = id;
            _errorRefreshMilliseconds = errorRefreshMilliseconds;
            _refreshMilliseconds = refreshMilliseconds;
        }

        public void Start()
        {
            Debug.Log("[Ads][YandexAdsBannerService] Start");
            _isBannerShow = true;
            if (_isAutoUpdate)
            {
                return;
            }

            RefreshStop();
            _source = new CancellationTokenSource();
            if (IsRefresh())
            {
                RefreshAsync(_source.Token);
            }
            else
            {
                InitBanner();
            }
        }

        public void Stop()
        {
            RefreshStop();
            _banner?.Destroy();
            _banner = null;
        }

        public bool IsRefresh()
        {
            return _refreshMilliseconds > 0L;
        }

        private void InitBanner()
        {
            _banner?.Destroy();
            var bannerMaxSize = AdSize.FlexibleSize(GetScreenWidthDp(), 50);
            _banner = new Banner(_id, bannerMaxSize, AdPosition.BottomCenter);
            _banner.OnAdLoaded += BannerOnAdLoaded;
            _banner.OnAdFailedToLoad += BannerOnAdFailedToLoad;
            _banner.LoadAd(new AdRequest.Builder().Build());
        }

        private void BannerOnAdFailedToLoad(object sender, AdFailureEventArgs args)
        {
            Debug.Log("[Ads] BannerOnAdFailedToLoad event received with message: " + args.Message);
        }

        private void BannerOnAdLoaded(object sender, EventArgs e)
        {
            Debug.Log("[Ads] BannerOnAdLoaded event received");
            if (_isBannerShow)
            {
                _banner?.Show();
            }
        }

        private async void RefreshAsync(CancellationToken token)
        {
            Debug.Log("[Ads] RefreshAsync RetryReload");
            RefreshLocal();
            _isAutoUpdate = true;
            while (IsRefresh() && !token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_refreshMilliseconds, token);
                    Debug.Log("[Ads] RefreshAsync RetryReload Tick");
                    RefreshLocal();
                }
                catch (TaskCanceledException)
                {
                    Debug.Log("[Ads] RefreshAsync TaskCanceledException");
                    break;
                }
            }

            _isAutoUpdate = false;
        }
        
        private async void RefreshAsync()
        {
            Debug.Log("[Ads] RefreshAsync");
            if (_errorRefreshMilliseconds <= 0)
            {
                Debug.Log("[Ads] RefreshAsync invalid");
                return;
            }
            Stop();
            await Task.Delay(_errorRefreshMilliseconds);
            Start();
        }

        private void RefreshLocal()
        {
            Debug.Log("[Ads][YandexAdsBannerService] ReloadLocal");
            InitBanner();
        }

        private void RefreshStop()
        {
            Debug.Log("[Ads][YandexAdsBannerService] SourceReset");
            if (_source != null)
            {
                _source.Cancel();
                _source.Dispose();
            }

            _source = null;
            _isAutoUpdate = false;
        }
        
        private int GetScreenWidthDp()
        {
            var screenWidth = (int)Screen.safeArea.width;
            return ScreenUtils.ConvertPixelsToDp(screenWidth);
        }
    }
}