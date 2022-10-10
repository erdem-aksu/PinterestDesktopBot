using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PinterestDesktopBot.PinterestClient.Api;
using PinterestDesktopBot.PinterestClient.Api.Exceptions;
using PinterestDesktopBot.PinterestClient.Api.Responses;
using PinterestDesktopBot.PinterestClient.Api.Session;
using PinterestDesktopBot.PinterestClient.Api.SessionHandlers;
using PinterestDesktopBot.PinterestClient.Extensions;
using PinterestDesktopBot.PinterestClient.Serialization;
using RestSharp;

namespace PinterestDesktopBot.PinterestClient.Http
{
    internal class PinterestHttpClient
    {
        private UserSessionData _user;

        private StateData _stateData;

        private readonly bool _autoLogin;

        private readonly ISessionHandler _sessionHandler;

        private readonly CookieContainer _cookieContainer;

        private readonly Uri _baseAddress;

        private readonly Uri _appBaseAddress;

        private ProxyData _proxyData;

        private string _appCsrfToken = string.Empty;

        private static readonly IDictionary<string, string> RequestHeaders = new Dictionary<string, string>()
        {
            {"User-Agent", PinterestApiConstants.HeaderUserAgent},
            {"Accept", "application/json, text/javascript, */*; q=0.01"},
            {"Accept-Language", "en-US,en;q=0.5"},
            {"X-Pinterest-AppState", "active"},
            {"X-Requested-With", "XMLHttpRequest"}
        };

        private static readonly IDictionary<string, string> RequestHeaders2 = new Dictionary<string, string>()
        {
            {"User-Agent", PinterestApiConstants.HeaderUserAgent},
            {
                "Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"
            },
            {"Accept-Language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7"},
            {"sec-fetch-dest", "document"},
            {"sec-fetch-mode", "navigate"},
            {"sec-fetch-site", "cross-site"},
            {"upgrade-insecure-requests", "1"},
        };

        private static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        };

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = ContractResolver,
        };

        public PinterestHttpClient(UserSessionData user, bool autoLogin = false, string stateDataPath = null,
            ProxyData proxyData = null)
        {
            _user = user;
            _stateData = new StateData
            {
                User = _user
            };

            if (autoLogin && !string.IsNullOrEmpty(stateDataPath) && Directory.Exists(stateDataPath))
            {
                _autoLogin = true;
                _sessionHandler = new FileSessionHandler(this, Path.Combine(stateDataPath, _user.Username + ".bin"));
            }

            _cookieContainer = new CookieContainer();
            _baseAddress = new Uri(PinterestApiConstants.UrlBase);
            _proxyData = proxyData;
            _appBaseAddress = new Uri(PinterestApiConstants.DeveloperUrlBase);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, object options = null,
            bool shouldLogin = false, JsonSerializerSettings serializerSettings = null, bool bookmarks = false)
        {
            if (shouldLogin)
            {
                await Login();
            }

            requestUri = BuildPath(requestUri, options, serializerSettings, bookmarks);

            var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return await SendAsync(message, bookmarks);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, object value, object options = null,
            bool shouldLogin = false, JsonSerializerSettings serializerSettings = null, string sourceUrl = null)
        {
            if (shouldLogin)
            {
                await Login();
            }

            requestUri = BuildPath(requestUri, options, serializerSettings);

            var message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = GetFormUrlEncodedContent(value, serializerSettings, sourceUrl),
            };

            return await SendAsync(message);
        }

        public async Task<HttpResponseMessage> PostAppAsync(string requestUri, object value, object options = null,
            bool shouldLogin = true, JsonSerializerSettings serializerSettings = null)
        {
            if (shouldLogin)
            {
                await Login();
            }

            requestUri = BuildPath(requestUri, options, serializerSettings);

            var message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = GetAppJsonContent(value, serializerSettings),
            };

            return await SendAppAsync(message);
        }

        public async Task<HttpResponseMessage> PatchAppAsync(string requestUri, object value, object options = null,
            bool shouldLogin = true, JsonSerializerSettings serializerSettings = null)
        {
            if (shouldLogin)
            {
                await Login();
            }

            requestUri = BuildPath(requestUri, options, serializerSettings);

            var message = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = GetAppJsonContent(value, serializerSettings),
            };

            return await SendAppAsync(message);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, bool shouldLogin = false)
        {
            if (shouldLogin)
            {
                await Login();
            }

            return await SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
        }

        public async Task<HttpResponseMessage> UploadAsync(string requestUri, byte[] bytes, string fileName,
            string name = "img", bool shouldLogin = true)
        {
            if (shouldLogin)
            {
                await Login();
            }

            var message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = GetUploadContent(bytes, fileName, name)
            };

            return await SendAsync(message);
        }

        public async Task Login()
        {
            if (_autoLogin)
            {
                _sessionHandler.Load();
            }

            if (_stateData.IsLoggedIn) return;

            if (string.IsNullOrEmpty(_user.Username) || string.IsNullOrEmpty(_user.Password))
            {
                throw new ArgumentNullException(_user.Username);
            }

            using (var loginRes = await PostAsync(PinterestApiConstants.ResourceLogin,
                new {username_or_email = _user.Username, password = _user.Password}))
            {
                if (!loginRes.IsSuccessStatusCode)
                {
                    throw new PinterestAuthorizationException(loginRes.ReasonPhrase);
                }
            }

            _stateData.IsLoggedIn = true;

            if (_autoLogin)
            {
                _sessionHandler.Save();
            }
        }

        public void Logout()
        {
            _stateData.CsrfToken = string.Empty;
            _stateData.IsLoggedIn = false;

            foreach (Cookie co in _cookieContainer.GetCookies(_baseAddress))
            {
                co.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            }

            _stateData.Cookies = _cookieContainer;
            _stateData.RawCookies = _cookieContainer.GetCookies(_baseAddress).Cast<Cookie>().ToList();

            if (_autoLogin)
            {
                _sessionHandler.Save();
            }
        }

        public StateData GetStateData()
        {
            return _stateData;
        }

        public void SetStateData(StateData stateData)
        {
            _stateData = stateData;
        }

        public ProxyData GetProxyData()
        {
            return _proxyData;
        }

        public void SetProxyData(ProxyData proxyData)
        {
            _proxyData = proxyData;
        }

        public void SetUser(UserSessionData user, bool? isLoggedIn = null)
        {
            _user = user;
            _stateData.User = user;
            _stateData.IsLoggedIn = isLoggedIn ?? _stateData.IsLoggedIn;
        }

        public List<Cookie> GetCookies()
        {
            return _stateData.RawCookies;
        }

        public bool HasBookmark()
        {
            return _stateData.Bookmarks != null && _stateData.Bookmarks.Any();
        }

        public IEnumerable<string> GetBookmarks()
        {
            return _stateData.Bookmarks;
        }

        public void SetBookmarks(IEnumerable<string> bookmarks)
        {
            _stateData.Bookmarks = bookmarks;
        }

        public Stream GetStateDataAsStream()
        {
            return SerializationHelper.SerializeToStream(_stateData);
        }

        public void LoadStateDataFromStream(Stream stream)
        {
            var data = SerializationHelper.DeserializeFromStream<StateData>(stream);

            if (_user.Password != data.User.Password)
            {
                return;
            }

            _user = data.User;

            foreach (var cookie in data.RawCookies)
            {
                _cookieContainer.Add(_baseAddress, cookie);
            }

            _stateData.RawCookies = data.RawCookies;
            _stateData.Bookmarks = data.Bookmarks;
            _stateData.AppVersion = data.AppVersion;
            _stateData.CsrfToken = data.CsrfToken;
            _stateData.IsLoggedIn = data.IsLoggedIn;
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, bool bookmarks = false)
        {
            await InitCsrfToken();

            SetHeaders(message);

            using (var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseCookies = true,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler)
                {BaseAddress = _baseAddress, Timeout = TimeSpan.FromSeconds(30)})
            {
                var response = await client.SendAsync(message);

                _stateData.Cookies = handler.CookieContainer;
                _stateData.RawCookies = _stateData.Cookies.GetCookies(response.RequestMessage.RequestUri)
                    .Cast<Cookie>()
                    .ToList();
                _stateData.CsrfToken = _stateData.RawCookies
                                           .First(cookie => cookie.Name == PinterestApiConstants.CsrfCookieField)?.Value
                                       ?? string.Empty;
                _stateData.Bookmarks = null;

                if (response.Headers.Contains(PinterestApiConstants.AppVersionHeaderField))
                {
                    _stateData.AppVersion =
                        response.Headers.GetValues(PinterestApiConstants.AppVersionHeaderField).First();
                }

                if (bookmarks)
                {
                    try
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var res = JsonConvert.DeserializeObject<JObject>(content, JsonSerializerSettings);

                        _stateData.Bookmarks = res.SelectToken("resource.options.bookmarks").ToObject<List<string>>();
                    }
                    catch (Exception)
                    {
                        _stateData.Bookmarks = null;
                    }
                }

                return response;
            }
        }

        private async Task<HttpResponseMessage> SendAppAsync(HttpRequestMessage message, bool bookmarks = false)
        {
            await InitAppCsrfToken();

            SetAppHeaders(message);

            using (var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseCookies = true,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler) {BaseAddress = _appBaseAddress})
            {
                var response = await client.SendAsync(message);

                return response;
            }
        }

        private async Task InitCsrfToken()
        {
            if (!string.IsNullOrEmpty(_stateData.CsrfToken)) return;

            using (var handler = new HttpClientHandler()
            {
                CookieContainer = _cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseCookies = true,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler)
                {BaseAddress = _baseAddress, Timeout = TimeSpan.FromSeconds(30)})
            {
                await client.GetAsync(client.BaseAddress);

                var responseCookies = handler.CookieContainer.GetCookies(client.BaseAddress);

                _stateData.CsrfToken = responseCookies[PinterestApiConstants.CsrfCookieField]?.Value ?? string.Empty;
            }
        }

        private async Task InitAppCsrfToken()
        {
            if (!string.IsNullOrEmpty(_appCsrfToken)) return;

            using (var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseCookies = true,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler) {BaseAddress = _appBaseAddress})
            {
                var response = await client.GetAsync(client.BaseAddress);

                var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var matches = Regex.Matches(html,
                    "<meta name=\"csrf-token\" content=\"((.(?<!(\")))*)\">");

                if (matches.Count > 0)
                {
                    _appCsrfToken = matches[0].Groups[1].Value;
                }
                else
                {
                    throw new Exception("Csrf token not found.");
                }
            }
        }

        public async Task<string> GetAppAccessToken(string appId, string secret)
        {
            using (var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseCookies = true,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler))
            {
                var options = new
                {
                    response_type = "code",
                    redirect_uri = PinterestApiConstants.OauthRedirectUrl,
                    client_id = appId,
                    scope = "read_public,write_public,read_relationship,write_relationship",
                    state = string.Empty,
                };

                var url = options.GetType().GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(options).ToString())
                    .Aggregate(PinterestApiConstants.OauthUrlBase,
                        (current, pair) => current.AddQueryParam(pair.Key, pair.Value));

                var message = new HttpRequestMessage(HttpMethod.Get, url);

                var originUri = new Uri(url);
                var originUrl = originUri.Scheme + "://" + originUri.Host + '/';

                foreach (var header in RequestHeaders)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

                message.Headers.Add("Referer", originUrl);
                message.Headers.Add("Origin", originUrl);


                var response = await client.SendAsync(message);

                var html = await response.Content.ReadAsStringAsync();

                var matches = Regex.Matches(html,
                    "<input type=\"hidden\" name=\"csrf_token\" value=\"((.(?<!(\")))*)\"/>");

                string csrfToken;

                if (matches.Count > 0)
                {
                    csrfToken = matches[0].Groups[1].Value;
                }
                else
                {
                    throw new Exception("Csrf token not found.");
                }

                var message2 = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new System.Net.Http.FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("csrf_token", csrfToken)
                    })
                };

                foreach (var header in RequestHeaders)
                {
                    message2.Headers.Add(header.Key, header.Value);
                }

                message2.Headers.Add("Referer", originUrl);
                message2.Headers.Add("Origin", originUrl);

                var response2 = await client.SendAsync(message2);

                var html2 = await response2.Content.ReadAsStringAsync();

                var matches2 = Regex.Matches(html2, "data-code=\"((.(?<!(\")))*)\"");

                string code;

                if (matches2.Count > 0)
                {
                    code = matches2[0].Groups[1].Value;
                }
                else
                {
                    throw new Exception("Code not found.");
                }

                var tokenData = new
                {
                    grant_type = "authorization_code",
                    client_id = appId,
                    client_secret = secret,
                    code
                };

                var message3 = new HttpRequestMessage(HttpMethod.Post, PinterestApiConstants.OauthAccessUrlBase)
                {
                    Content = new System.Net.Http.FormUrlEncodedContent(tokenData.GetType().GetProperties()
                        .ToDictionary(p => p.Name, p => p.GetValue(tokenData).ToString()))
                };

                var response3 = await client.SendAsync(message3);

                var accessToken = JsonConvert.DeserializeObject<JObject>(await response3.Content.ReadAsStringAsync())
                    .Value<string>("access_token");

                return accessToken;
            }
        }

        public async Task<string> GetAccessTokenNew(List<Cookie> cookies)
        {
            var csrfCookie = cookies.FirstOrDefault(c => c.Name == PinterestApiConstants.CsrfCookieField);

            _stateData.CsrfToken = csrfCookie?.Value ?? string.Empty;

            var requestUri = BuildPath(PinterestApiConstants.ResourceOAuth);

            var message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = GetFormUrlEncodedContent(new
                    {
                        consumer_id = "1447676",
                        response_type = "code",
                        redirect_uri = "https://app.sproutsocial.com/oauthhandler/pinterestCallback",
                        authorized = "true"
                    }, null,
                    "/oauth/?consumer_id=1447676&redirect_uri=https%3A%2F%2Fapp.sproutsocial.com%2Foauthhandler%2FpinterestCallback&response_type=code&state=1396034_true")
            };

            SetHeaders(message);

            var cookieContainer = new CookieContainer();

            foreach (var cookie in cookies)
            {
                cookieContainer.Add(cookie);
            }

            using (var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 2,
                UseCookies = true,
                UseProxy = _proxyData != null,
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null
            })
            using (var client = new HttpClient(handler)
            {
                BaseAddress = csrfCookie != null ? new Uri($"https://{csrfCookie.Domain}/") : _baseAddress,
                Timeout = TimeSpan.FromSeconds(30)
            })
            {
                var response = await client.SendAsync(message);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PinterestAuthorizationException(response.ReasonPhrase);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var res = JsonConvert.DeserializeObject<BaseResponse<JObject>>(content, JsonSerializerSettings);

                return res.ResourceResponse.Data.Value<string>("access_token");
            }
        }

        public async Task<string> GetAccessTokenNew(bool auth = true)
        {
            using (var loginRes = await PostAsync(PinterestApiConstants.ResourceOAuth,
                new
                {
                    consumer_id = "1447676",
                    response_type = "code",
                    redirect_uri = "https://app.sproutsocial.com/oauthhandler/pinterestCallback",
                    authorized = "true"
                }, null, auth, null,
                "/oauth/?consumer_id=1447676&redirect_uri=https%3A%2F%2Fapp.sproutsocial.com%2Foauthhandler%2FpinterestCallback&response_type=code&state=1396034_true")
            )
            {
                if (!loginRes.IsSuccessStatusCode)
                {
                    throw new PinterestAuthorizationException(loginRes.ReasonPhrase);
                }

                var content = await loginRes.Content.ReadAsStringAsync().ConfigureAwait(false);

                var res = JsonConvert.DeserializeObject<BaseResponse<JObject>>(content, JsonSerializerSettings);

                return res.ResourceResponse.Data.Value<string>("access_token");
            }
        }

        public async Task<string> GetAccessTokenNewV2(bool auth = true)
        {
            if (auth)
            {
                await Login();
            }

            var client = new RestClient()
            {
                Proxy = _proxyData != null
                    ? new WebProxy(_proxyData.Address, false, new string[] { },
                        new NetworkCredential(_proxyData.Username, _proxyData.Password))
                    : null,
                CookieContainer = _cookieContainer,
                FollowRedirects = true,
                UserAgent = PinterestApiConstants.HeaderUserAgent,
            };

            var baseUrl = "https://www.newchic.com/index.php?com=pinit&t=login&products_id=1655044";

            var message = new RestRequest(baseUrl, Method.GET);

            message.AddHeaders(RequestHeaders2);

            var response = await client.ExecuteAsync(message);

            var matches = Regex.Matches(response.Content,
                "<input type=\"hidden\" name=\"csrf_token\" value=\"((.(?<!(\")))*)\"/>");

            string csrfToken;

            if (matches.Count > 0)
            {
                csrfToken = matches[0].Groups[1].Value;
            }
            else
            {
                throw new Exception("Csrf token not found.");
            }

            var url = response.ResponseUri;

            var originUri = url;
            var originUrl = originUri.Scheme + "://" + originUri.Host + '/';

            var message2 = new RestRequest(url, Method.POST);

            message2.AddHeaders(RequestHeaders);
            message2.AddParameter("csrf_token", csrfToken, ParameterType.GetOrPost);
            message2.AddHeader("Referer", originUrl);
            message2.AddHeader("Origin", originUrl);

            var response2 = await client.ExecuteAsync(message2);

            var redirectUri = Regex.Match(response2.Content, "data-rewritten_redirect_uri=\"((.(?<!(\")))*)\"")
                .Groups[1].Value.Replace("&amp;", "&");

            var message3 = new RestRequest(redirectUri, Method.GET);

            message3.AddHeaders(RequestHeaders2);
            message3.AddHeader("Referer", url.ToString());

            var response3 = await client.ExecuteAsync(message3);

            var accessToken = response3.Cookies.FirstOrDefault(c => c.Name == "pintrest_token")?.Value;

            if (accessToken == null)
            {
                throw new Exception("Access token not found.");
            }

            return accessToken;
        }

        private static System.Net.Http.FormUrlEncodedContent GetFormUrlEncodedContent(object obj,
            JsonSerializerSettings serializerSettings = null, string sourceUrl = null)
        {
            var data = new
            {
                context = new object(),
                options = obj
            };

            var wrapper = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("source_url", sourceUrl ?? string.Empty),
                new KeyValuePair<string, string>("data",
                    JsonConvert.SerializeObject(data, serializerSettings ?? JsonSerializerSettings))
            };

            return new System.Net.Http.FormUrlEncodedContent(wrapper);
        }

        private static StringContent GetAppJsonContent(object obj,
            JsonSerializerSettings serializerSettings = null)
        {
            return new StringContent(JsonConvert.SerializeObject(obj, serializerSettings ?? JsonSerializerSettings),
                Encoding.UTF8, "application/json");
        }

        private string BuildPath(string basePath, object options = null,
            JsonSerializerSettings serializerSettings = null, bool bookmarks = false)
        {
            if (options == null) return basePath;

            var path = basePath;

            if (!path.EndsWith("/"))
                path += "/";

            var optionsPairs = options.GetType() == typeof(Dictionary<string, object>)
                ? (Dictionary<string, object>) options
                : options.GetType().GetProperties()
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(options, null));

            if (bookmarks && _stateData.Bookmarks != null)
            {
                if (_stateData.Bookmarks.Any() && _stateData.Bookmarks.First() != "-end-")
                {
                    optionsPairs.Add("bookmarks", _stateData.Bookmarks);
                }
            }

            var data = new
            {
                context = new object(),
                options = optionsPairs
            };

            var wrapper = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("source_url", string.Empty),
                new KeyValuePair<string, string>("data",
                    JsonConvert.SerializeObject(data, serializerSettings ?? JsonSerializerSettings))
            };

            return wrapper.Aggregate(path,
                (current, pair) => Extensions.QueryStringExtensions.AddQueryParam(current, pair.Key, pair.Value));
        }

        private static MultipartFormDataContent GetUploadContent(byte[] bytes, string fileName, string name)
        {
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.MimeUtility.GetMimeMapping(fileName));

            return new MultipartFormDataContent {{content, name, fileName}};
        }

        private void SetHeaders(HttpRequestMessage request)
        {
            foreach (var header in RequestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            request.Headers.Add("Referer", PinterestApiConstants.UrlBase);
            request.Headers.Add("Origin", PinterestApiConstants.UrlBase);
            request.Headers.Add("X-APP-VERSION", _stateData.AppVersion);
            request.Headers.Add("X-CSRFToken", _stateData.CsrfToken);
        }

        private void SetAppHeaders(HttpRequestMessage request)
        {
            foreach (var header in RequestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            request.Headers.Add("Referer", PinterestApiConstants.DeveloperUrlBase);
            request.Headers.Add("Origin", PinterestApiConstants.DeveloperUrlBase);
            request.Headers.Add("X-CSRFToken", _appCsrfToken);
        }
    }
}