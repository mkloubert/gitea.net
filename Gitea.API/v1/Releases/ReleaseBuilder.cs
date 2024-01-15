using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gitea.API.v1.Repositories;
using Gitea.API.v1.Users;
using Newtonsoft.Json;

namespace Gitea.API.v1.Releases
{
    public sealed class ReleaseBuilder
    {
        private readonly Repository _repository;
        private readonly User _user;
        protected readonly IDictionary<string, object> _properties = new Dictionary<string, object>();
        private string _body;
        private bool _draft;
        private string _name;
        private bool _prerelease;
        private string _tagName;
        private string _targetCommitish;
        private string _ownerName;
        private string _reposName;
        /*internal ReleaseBuilder(User user)
        {
            _user = user;
        }*/
        internal ReleaseBuilder(Repository repository)
        {
            _repository = repository;
        }

        public ReleaseBuilder SetBody(string body)
        {
            _properties["body"] = body;
            _body = body;
            return this;
        }

        public ReleaseBuilder SetDraft(bool draft)
        {
            _properties["draft"] = draft;
            _draft = draft;
            return this;
        }

        public ReleaseBuilder SetName(string name)
        {
            _properties["name"] = name;
            _name = name;
            return this;
        }

        public ReleaseBuilder SetPrerelease(bool isPrerelease)
        {
            _properties["prerelease"] = isPrerelease;
            _prerelease = isPrerelease;
            return this;
        }

        public ReleaseBuilder SetTagName(string tagName)
        {
            _properties["tag_name"] = tagName;
            _tagName = tagName;
            return this;
        }

        public ReleaseBuilder SetTargetCommitish(string targetCommitish)
        {
            _properties["target_commitish"] = targetCommitish;
            _targetCommitish = targetCommitish;
            return this;
        }
        


        public async Task<Release> BuildAsync()
        {
            using (var rest = _repository.Owner.Endpoint.Client.CreateBaseClient())
            {
                
                var resp = await rest.PostAsync($"repos/{_repository.Owner.Username}/{_repository.Name}/releases",
                    new StringContent(ToJson(), Encoding.UTF8, "application/json")
                );

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.Created)
                {
                    switch ((int)resp.StatusCode)
                    {
                        case 422:
                        case 500:
                        case 409:
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
        
        
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(_properties);
        }
    }
}