using MayMayShop.API.Interfaces.Services;

namespace MayMayShop.API.Services
{
    public static class ServicesFactory
    {
        public static ICommonServices GetCommonServices()
        {
            return new CommonServices();
        }

        public static IMayMayShopServices GetPackageServices()
        {
            return new MayMayShopServices();
        }
    }

}
