using System;
using System.Threading.Tasks;
using Plugins.Ads.Api;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Plugins.Ads.Yandex
{
    public class YandexAdsRewardedService : IAdsRewardedService
    {
        private readonly string _id;
        private readonly int _errorRefreshMilliseconds;
        private readonly int _refreshMilliseconds;

        private RewardedAd _rewardedAd;
        private Action _onVideoFinished;


        public event Action<bool> RewardedVideoReady;
        public bool IsReady => _rewardedAd?.IsLoaded() ?? false;
        private RewardedState _rewardedVideoState = RewardedState.None;

        public YandexAdsRewardedService(string id, int refreshMilliseconds, int errorRefreshMilliseconds)
        {
            _id = id;
            _refreshMilliseconds =  refreshMilliseconds;
            _errorRefreshMilliseconds = errorRefreshMilliseconds;
        }


        public void Start()
        {
            Debug.Log("[Ads] YandexAdsRewardedService Start");
            if (_rewardedAd == null || !_rewardedAd.IsLoaded())
            {
                Debug.Log("[Ads] YandexAdsRewardedService InitRewarded Call");
                InitRewarded();
            }
        }

        public void Show(Action onVideoFinished)
        {
            Debug.Log("[Ads] YandexAdsRewardedService Show");
            if (_rewardedAd != null && _rewardedAd.IsLoaded())
            {
                _rewardedAd?.Show();
                _onVideoFinished = onVideoFinished;
            }
        }

        private void InitRewarded()
        {
            if (_rewardedVideoState == RewardedState.Loading)
            {
                return;
            }

            _rewardedVideoState = RewardedState.Loading;
            RewardedVideoReady?.Invoke(false);
            _rewardedAd?.Destroy();
            _rewardedAd = new RewardedAd(_id);
            _rewardedAd.OnRewarded += RewardedAdOnRewarded;
            _rewardedAd.OnRewardedAdDismissed += RewardedAdOnRewardedAdDismissed;
            _rewardedAd.OnRewardedAdLoaded += RewardedAdOnRewardedAdLoaded;
            _rewardedAd.OnRewardedAdFailedToLoad += RewardedAdOnRewardedAdFailedToLoad;
            _rewardedAd.LoadAd(new AdRequest.Builder().Build());
        }
        
        private async void  RefreshAsync()
        {
            Debug.Log("[Ads] RefreshAsync");
            if (_errorRefreshMilliseconds <= 0)
            {
                Debug.Log("[Ads] RefreshAsync invalid");
                return;
            }
            await Task.Delay(_errorRefreshMilliseconds);
            InitRewarded();
        }

        private void RewardedAdOnRewardedAdDismissed(object sender, EventArgs e)
        {
            Debug.Log("[Ads] RewardedAdOnRewardedAdDismissed");
            _onVideoFinished = null;
            InitRewarded();
        }

        private void RewardedAdOnRewarded(object sender, Reward e)
        {
            Debug.Log("[Ads] RewardedAdOnRewarded: " + e.amount);
            _onVideoFinished?.Invoke();
            _onVideoFinished = null;
        }

        private void RewardedAdOnRewardedAdFailedToLoad(object sender, AdFailureEventArgs e)
        {
            Debug.Log("[Ads] RewardedAdOnRewardedAdFailedToLoad: " + e.Message);
            _rewardedVideoState = RewardedState.Fail;
            RewardedVideoReady?.Invoke(false);
            //RefreshAsync();
        }

        private void RewardedAdOnRewardedAdLoaded(object sender, EventArgs e)
        {
            Debug.Log("[Ads] RewardedAdOnRewardedAdLoaded");
            _rewardedVideoState = RewardedState.Loaded;
            RewardedVideoReady?.Invoke(true);
        }
    }
}