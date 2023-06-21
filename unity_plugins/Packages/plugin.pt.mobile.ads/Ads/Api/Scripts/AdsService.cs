namespace Plugins.Ads.Api
{
    public class AdsService : IAdsService
    {
        public IAdsBannerService BannerService { get; }
        public IAdsRewardedService RewardedService { get; }

        public AdsService(IAdsBannerService bannerService, IAdsRewardedService rewardedService)
        {
            BannerService = bannerService;
            RewardedService = rewardedService;
        }
    }
}