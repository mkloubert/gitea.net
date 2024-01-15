using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gitea.API.v1.Assets;
using Newtonsoft.Json;

namespace Gitea.API.v1.Releases
{
    public class AssetsEndpoint
    {
        private readonly Release _release;

        public AssetsEndpoint(Release release)
        {
            _release = release;
        }
        
        /// <summary>
        /// Returns a list of all releases of the underlying repository.
        /// </summary>
        /// <returns>The list of releases.</returns>
        public async Task<List<Asset>> GetAllAsync()
        {
            return await GetAllAsync<List<Asset>>();
        }
        
        /// <summary>
        /// Returns a collection of all Releases of the underlying repository.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns>The collection of releases.</returns>
        public async Task<TCollection> GetAllAsync<TCollection>()
            where TCollection : global::System.Collections.Generic.ICollection<Asset>, new()
        {
            using (var rest = _release.Author.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync($"repos/{HttpUtility.UrlEncode(_release.Author.Username)}/{HttpUtility.UrlEncode(_release.Repository.Name)}/releases/{_release.Id}/assets");

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError:
                            exception = new ApiException(JsonConvert.DeserializeObject<ApiError>
                                (
                                    await resp.Content.ReadAsStringAsync()
                                ),
                                (int)resp.StatusCode, resp.ReasonPhrase);
                            break;

                        default:
                            exception = new UnexpectedResponseException((int)resp.StatusCode,
                                                                        resp.ReasonPhrase);
                            break;
                    }
                }

                if (exception != null)
                {
                    throw exception;
                }

                var json = await resp.Content.ReadAsStringAsync();

                var repoList = JsonConvert.DeserializeObject<IEnumerable<Asset>>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );

                var userRepos = new TCollection();
                using (var e = repoList.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        var r = e.Current;
                        r.Release = _release;

                        userRepos.Add(r);
                    }
                }
                

                return userRepos;
            }
        }


        public AssetBuilder Create()
        {
            return new AssetBuilder(_release);
        }

        public async Task<Asset> GetAssetByIdAsync(int assetId)
        {
            using (var rest = _release.Author.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync($"repos/{HttpUtility.UrlEncode(_release.Author.Username)}/{HttpUtility.UrlEncode(_release.Repository.Name)}/releases/{_release.Id}/assets/{assetId}");

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.NotFound:
                            exception = new ApiException(JsonConvert.DeserializeObject<ApiError>
                                (
                                    await resp.Content.ReadAsStringAsync()
                                ),
                                (int)resp.StatusCode, resp.ReasonPhrase);
                            break;

                        default:
                            exception = new UnexpectedResponseException((int)resp.StatusCode,
                                resp.ReasonPhrase);
                            break;
                    }
                }
                if (exception != null)
                {
                    throw exception;
                }
                var json = await resp.Content.ReadAsStringAsync();
                var asset = JsonConvert.DeserializeObject<Asset>
                (
                    await resp.Content.ReadAsStringAsync()
                );
                asset.Release = _release;
                return asset;
            }
        }

        public async Task DeleteAssetByIdAsync(int assetId)
        {
            using (var rest = _release.Author.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.DeleteAsync($"repos/{HttpUtility.UrlEncode(_release.Author.Username)}/{HttpUtility.UrlEncode(_release.Repository.Name)}/releases/{_release.Id}/assets/{assetId}");

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.NoContent)
                {
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.NotFound:
                            exception = new ApiException(JsonConvert.DeserializeObject<ApiError>
                                (
                                    await resp.Content.ReadAsStringAsync()
                                ),
                                (int)resp.StatusCode, resp.ReasonPhrase);
                            break;

                        default:
                            exception = new UnexpectedResponseException((int)resp.StatusCode,
                                resp.ReasonPhrase);
                            break;
                    }
                }
                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        public Task<Asset> EditAssetAsync(Asset asset)
        {
            return EditAssetByIdAsync(asset.Id, asset.Name);
        }

        public async Task<Asset> EditAssetByIdAsync(int assetId, string newName)
        {
            using (var rest = _release.Author.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.PatchAsync($"repos/{HttpUtility.UrlEncode(_release.Author.Username)}/{HttpUtility.UrlEncode(_release.Repository.Name)}/releases/{_release.Id}/assets/{assetId}",
                    new StringContent(JsonConvert.SerializeObject(new {name = newName}), Encoding.UTF8, "application/json"));

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.Created)
                {
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError:
                        case HttpStatusCode.NotFound:
                            exception = new ApiException(JsonConvert.DeserializeObject<ApiError>
                                (
                                    await resp.Content.ReadAsStringAsync()
                                ),
                                (int)resp.StatusCode, resp.ReasonPhrase);
                            break;

                        default:
                            exception = new UnexpectedResponseException((int)resp.StatusCode,
                                resp.ReasonPhrase);
                            break;
                    }
                }
                if (exception != null)
                {
                    throw exception;
                }
                var asset = JsonConvert.DeserializeObject<Asset>
                (
                    await resp.Content.ReadAsStringAsync()
                );
                asset.Release = _release;
                return asset;
            }
        }
    }
}