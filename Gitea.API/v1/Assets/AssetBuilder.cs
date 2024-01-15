using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Gitea.API.v1.Releases;
using Gitea.API.v1.Repositories;
using Gitea.API.v1.Users;
using Newtonsoft.Json;

namespace Gitea.API.v1.Assets
{
    public class AssetBuilder
    {
        private readonly Release _release;
        /*private readonly User _user;*/

        private string _name;
        private string _fileName;
        private string _ownerName;
        /*private string _reposName;
        private int _releaseId;*/
        private byte[] _attachment;
        
        /*public AssetBuilder(User user)
        {
            _user = user;
        }*/
        public AssetBuilder(Release release)
        {
            _release = release;
        }

        public AssetBuilder SetAssetName(string name)
        {
            _name = name;
            return this;
        }

        public AssetBuilder SetOwner(string ownerName)
        {
            _ownerName = ownerName;
            return this;
        }

        /*public AssetBuilder SetRepository(string reposName)
        {
            _reposName = reposName;
            return this;
        }

        public AssetBuilder SetRelease(int releaseId)
        {
            _releaseId = releaseId;
            return this;
        }*/

        public AssetBuilder SetAttachment(byte[] attachment, string fileName)
        {
            _attachment = attachment;
            _fileName = fileName;
            return this;
        }

        public async Task<Asset> BuildAsync()
        {
            using (var rest = _release.Author.Endpoint.Client.CreateBaseClient())
            {
                var formdata = new MultipartFormDataContent();
                formdata.Add(new ByteArrayContent(_attachment), "attachment", _fileName);
                var resp = await rest.PostAsync(
                    $"repos/{_ownerName}/{_release.Repository.Name}/releases/{_release.Id}/assets?name={_name}",
                    formdata);
                
               

                Exception exception = null;

                if (resp.StatusCode != HttpStatusCode.Created)
                {
                    switch ((int)resp.StatusCode)
                    {
                        case 400:
                        case 401:
                        case 404:
                        case 500:
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
                
                if (asset != null)
                {
                    asset.Release = _release;
                }

                return asset;
            }
        }

    }
}