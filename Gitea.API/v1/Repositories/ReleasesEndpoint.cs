using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gitea.API.v1.Releases;
using Gitea.API.v1.Users;
using Newtonsoft.Json;

namespace Gitea.API.v1.Repositories
{
    /// <summary>
    /// An endpoint for repository's releases
    /// </summary>
    public class ReleasesEndpoint
    {
        private readonly Repository _repository;

        internal ReleasesEndpoint(Repository repository)
        {
            _repository = repository;
        }

        
        /// <summary>
        /// Returns a list of all releases of the underlying repository.
        /// </summary>
        /// <returns>The list of releases.</returns>
        public async Task<List<Release>> GetAllAsync()
        {
            return await GetAllAsync<List<Release>>();
        }
        
        /// <summary>
        /// Returns a collection of all Releases of the underlying repository.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns>The collection of releases.</returns>
        public async Task<TCollection> GetAllAsync<TCollection>()
            where TCollection : global::System.Collections.Generic.ICollection<Release>, new()
        {
            using (var rest = _repository.Owner.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync($"repos/{HttpUtility.UrlEncode(_repository.Owner.Username)}/{HttpUtility.UrlEncode(_repository.Name)}/releases");

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

                var repoList = JsonConvert.DeserializeObject<IEnumerable<Release>>
                    (
                        await resp.Content.ReadAsStringAsync()
                    );

                var userRepos = new TCollection();
                using (var e = repoList.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        var r = e.Current;
                        r.Author.Endpoint = _repository.Owner.Endpoint;
                        r.Repository = _repository;
                        foreach (var asset in r.Assets)
                        {
                            asset.Release = r;
                        }

                        userRepos.Add(r);
                    }
                }
                

                return userRepos;
            }
        }


        public ReleaseBuilder Create()
        {
            return new ReleaseBuilder(_repository);
        }

        public async Task<Release> GetLatestRelease()
        {
            using (var rest = _repository.Owner.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync($"repos/{HttpUtility.UrlEncode(_repository.Owner.Username)}/{HttpUtility.UrlEncode(_repository.Name)}/releases/latest");

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
                var release = JsonConvert.DeserializeObject<Release>
                (
                    await resp.Content.ReadAsStringAsync()
                );
                if (release != null)
                {
                    release.Repository = _repository;
                    release.Author.Endpoint = _repository.Owner.Endpoint;
                }
                return release;
            }
        }

        public async Task<Release> GetReleaseByTagAsync(string tagName)
        {
            using (var rest = _repository.Owner.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync($"repos/{HttpUtility.UrlEncode(_repository.Owner.Username)}/{HttpUtility.UrlEncode(_repository.Name)}/releases/tags/{tagName}");

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
                var release = JsonConvert.DeserializeObject<Release>
                (
                    await resp.Content.ReadAsStringAsync()
                );
                if (release != null)
                {
                    release.Repository = _repository;
                    release.Author.Endpoint = _repository.Owner.Endpoint;
                }
                return release;
            }
        }
        
        public async Task DeleteReleaseByTagAsync(string tagName)
        {
            using (var rest = _repository.Owner.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.DeleteAsync($"repos/{HttpUtility.UrlEncode(_repository.Owner.Username)}/{HttpUtility.UrlEncode(_repository.Name)}/releases/tags/{tagName}");

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
        
        public async Task<Release> GetReleaseByIdAsync(int releaseId)
        {
            using (var rest = _repository.Owner.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.GetAsync($"repos/{HttpUtility.UrlEncode(_repository.Owner.Username)}/{HttpUtility.UrlEncode(_repository.Name)}/releases/{releaseId}");

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
                var release = JsonConvert.DeserializeObject<Release>
                (
                    await resp.Content.ReadAsStringAsync()
                );
                if (release != null)
                {
                    release.Repository = _repository;
                    release.Author.Endpoint = _repository.Owner.Endpoint;
                }
                return release;
            }
        }

        public async Task DeleteReleaseByIdAsync(int releaseId)
        {
            using (var rest = _repository.Owner.Endpoint.Client.CreateBaseClient())
            {
                var resp = await rest.DeleteAsync($"repos/{HttpUtility.UrlEncode(_repository.Owner.Username)}/{HttpUtility.UrlEncode(_repository.Name)}/releases/{releaseId}");

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

        public Task<Release> UpdateReleaseAsync(Release release)
        {
            return UpdateReleaseAsync(release.Id, release.Body, release.Draft, release.Name, release.Prerelease,
                release.TagName, release.TargetCommitish);
        }

        public async Task<Release> UpdateReleaseAsync(int releaseId, string body = null, bool? draft = null, string name = null,
            bool? prerelease = null, string tagName = null, string targetCommitish = null)
        {
            var properties = new Dictionary<string, object>();
            properties["body"] = body;
            properties["draft"] = draft;
            properties["name"] = name;
            properties["prerelease"] = prerelease;
            properties["tag_nam"] = tagName;
            properties["target_commitish"] = targetCommitish;


            var json = JsonConvert.SerializeObject(properties,
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            
            using (var rest = _repository.Owner.Endpoint.Client.CreateBaseClient())
            {
                
                var resp = await rest.PatchAsync($"repos/{_repository.Owner.Username}/{_repository.Name}/releases/{releaseId}",
                    new StringContent(json, Encoding.UTF8, "application/json")
                );

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

                var release = JsonConvert.DeserializeObject<Release>
                (
                    await resp.Content.ReadAsStringAsync()
                );

                if (release != null)
                {
                    release.Repository = _repository;
                    release.Author = _repository.Owner;
                }

                return release;
            }
        }
        
    }
    
    
    
    
    
}