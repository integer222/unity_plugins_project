namespace Plugins.Ads.Api
{
    public interface IAdsBannerService
    {
        void Start();
        void Stop();
        bool IsRefresh();
    }

    public class DefaultAdsBannerService : IAdsBannerService
    {
        public void Start()
        {
        }

        public void Stop()
        {
           
        }

        public bool IsRefresh() => false;
    }
}