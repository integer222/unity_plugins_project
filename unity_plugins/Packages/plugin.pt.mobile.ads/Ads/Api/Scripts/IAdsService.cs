namespace Plugins.Ads.Api
{
    public interface IAdsService
    {
        IAdsBannerService BannerService { get; }
        IAdsRewardedService RewardedService { get; }
    }
}