#if UNITASK_SUPPORT
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Mine.Code.Framework.Manager.RequestBuilder
{
    public class Request
    {
        public Request(
            Func<UniTask<UnityWebRequest>> getAsync, 
            Func<DownloadHandler, UniTask<UnityWebRequest>> getWithHandlerAsync, 
            Func<string, UniTask<UnityWebRequest>> postAsync,
            Func<UploadHandler, DownloadHandler, UniTask<UnityWebRequest>> postWithHandlerAsync, 
            Func<string, UniTask<UnityWebRequest>> putAsync,  
            Func<UploadHandler, DownloadHandler, UniTask<UnityWebRequest>> putWithHandlerAsync, 
            Func<UniTask<UnityWebRequest>> deleteAsync)
        {
            GetAsync = getAsync;
            GetWithHandlerAsync = getWithHandlerAsync;
            PostAsync = postAsync;
            PostWithHandlerAsync = postWithHandlerAsync;
            PutAsync = putAsync;
            PutWithHandlerAsync = putWithHandlerAsync;
            DeleteAsync = deleteAsync;
        }

        public Func<UniTask<UnityWebRequest>> GetAsync { get; }
        public Func<DownloadHandler, UniTask<UnityWebRequest>> GetWithHandlerAsync { get; }
        public Func<string, UniTask<UnityWebRequest>> PostAsync { get; }
        public Func<UploadHandler, DownloadHandler, UniTask<UnityWebRequest>> PostWithHandlerAsync { get; }
        public Func<string, UniTask<UnityWebRequest>> PutAsync { get; }
        public Func<UploadHandler, DownloadHandler, UniTask<UnityWebRequest>> PutWithHandlerAsync { get; }
        public Func<UniTask<UnityWebRequest>> DeleteAsync { get; }

        public static RequestBuilder Builder(string baseUrl) => new RequestBuilder().SetBaseUrl(baseUrl);
    
        public class RequestBuilder
        {
            string baseUrl;
            string path;
    
            Func<DownloadHandler> getDownloadHandler;
            Func<UploadHandler> getUploadHandler;
            List<(string name, string value)> requestHeaderList = new();

            public RequestBuilder SetBaseUrl(string baseUrl) { this.baseUrl = baseUrl; return this; }
            public RequestBuilder SetPath(string path) { this.path = path; return this; }
    
            public RequestBuilder SetDownloadHandler(Func<DownloadHandler> getDownloadHandler) { this.getDownloadHandler = getDownloadHandler; return this; }
            public RequestBuilder SetUploadHandler(Func<UploadHandler> getUploadHandler) { this.getUploadHandler = getUploadHandler; return this; }
            public RequestBuilder SetRequestHeader((string name, string value) requestHeader) { requestHeaderList.Add(requestHeader); return this; }

            public Request Build()
            {
                async Task<UnityWebRequest> SendAsync(UnityWebRequest request)
                {
                    if (getDownloadHandler != null) request.downloadHandler = getDownloadHandler();
                    if (getUploadHandler != null) request.uploadHandler = getUploadHandler();

                    foreach (var requestHeader in requestHeaderList) request.SetRequestHeader(requestHeader.name, requestHeader.value);

                    await request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success) Debug.Log($"{request.result} : {request.error}");

                    return request;
                }

                async UniTask<UnityWebRequest> GetAsync() => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "GET") {downloadHandler = new DownloadHandlerBuffer()});
                async UniTask<UnityWebRequest> GetWithHandlerAsync(DownloadHandler downloadHandler) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "GET") {downloadHandler = downloadHandler});
                async UniTask<UnityWebRequest> PostAsync(string post) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "POST") { uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(post)), downloadHandler = new DownloadHandlerBuffer() });
                async UniTask<UnityWebRequest> PostWithHandlerAsync(UploadHandler uploadHandler, DownloadHandler downloadHandler) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "POST") { uploadHandler = uploadHandler, downloadHandler = downloadHandler });
                async UniTask<UnityWebRequest> PutAsync(string body) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "PUT") { uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)), downloadHandler = new DownloadHandlerBuffer() });
                async UniTask<UnityWebRequest> PutWithHandlerAsync(UploadHandler uploadHandler, DownloadHandler downloadHandler) => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "PUT") { uploadHandler = uploadHandler, downloadHandler = downloadHandler });
                async UniTask<UnityWebRequest> DeleteAsync() => await SendAsync(new UnityWebRequest($"{baseUrl}/{path}", "DELETE"));

                return new(GetAsync, GetWithHandlerAsync, PostAsync, PostWithHandlerAsync, PutAsync, PutWithHandlerAsync, DeleteAsync);
            }
        }
    }
}
#endif