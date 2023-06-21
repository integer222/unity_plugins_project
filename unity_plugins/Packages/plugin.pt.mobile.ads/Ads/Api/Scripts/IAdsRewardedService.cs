using System;

namespace Plugins.Ads.Api
{
    public interface IAdsRewardedService
    {
        event Action<bool> RewardedVideoReady;
        bool IsReady { get; }
        void Start();
        void Show(Action onVideoFinished);
    }

    public class DefaultAdsRewardedService : IAdsRewardedService
    {
        public event Action<bool> RewardedVideoReady;
        public bool IsReady => false;
        

        public void Start()
        {
        }

        public void Show(Action onVideoFinished)
        {
        }
    }
}