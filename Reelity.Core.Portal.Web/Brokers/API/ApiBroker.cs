// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using RESTFulSense.Clients;
using System.Threading.Tasks;

namespace Reelity.Core.Portal.Web.Brokers.API
{
    public class ApiBroker : IApiBroker
    {
        private readonly IRESTFulApiFactoryClient apiClinet;

        public ApiBroker(IRESTFulApiFactoryClient apiClinet)
        {
            this.apiClinet = apiClinet;
        }

        private async ValueTask<T> GetAsync<T>(string relativeUrl) =>
            await this.apiClinet.GetContentAsync<T>(relativeUrl);

        private async ValueTask<T> PostAsync<T>(string relativeUrl, T content) =>
            await this.apiClinet.PostContentAsync<T>(relativeUrl, content);

        private async ValueTask<T> PuAsync<T>(string relativeUrl, T content) =>
            await this.apiClinet.PutContentAsync<T>(relativeUrl, content);

        private async ValueTask<T> DeleteAsync<T>(string relativeUrl) =>
            await this.apiClinet.DeleteContentAsync<T>(relativeUrl);
    }
}
