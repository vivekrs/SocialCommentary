using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fpr;
using LinqToTwitter;
using Ninject.Extensions.Logging;
using SocialCommentaryApi.Models;
using SocialCommentaryApi.Service.DataStore;
using SocialCommentaryApi.Service.Helpers;

namespace SocialCommentaryApi.Service.Twitter
{
    public class TwitterSearch
    {
        private readonly ILogger _logger;
        private readonly TwitterContext _twitterCtx;
        private readonly IDataStore _dataStore;

        public TwitterSearch(ILogger logger, TwitterContext twitterCtx, IDataStore dataStore)
        {
            _logger = logger;
            _twitterCtx = twitterCtx;
            _dataStore = dataStore;
        }

        public async Task DoFilterStreamAsync(string query, Func<string, Task<bool>> handleStreamResponse)
        {
            _logger.Debug("Streamed Content:");
            var cancelTokenSrc = new CancellationTokenSource();

            try
            {
                var streams = from strm in _twitterCtx.Streaming.WithCancellation(cancelTokenSrc.Token)
                    where strm.Type == StreamingType.Filter && strm.Track == query
                    select strm;

                //this will run indefinitely until the cancelTokenSrc.Cancel() is called in the Action delegate
                await streams.StartAsync(async strm => await ProcessTweet(strm, handleStreamResponse, cancelTokenSrc));
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Stream cancelled.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private async Task ProcessTweet(StreamContent strm, Func<string, Task<bool>> handleStreamResponse, CancellationTokenSource cancelTokenSrc)
        {
            if (strm.EntityType != StreamEntityType.Status)
                return;

            var response = TypeAdapter.Adapt<ScTweet>(strm);
            var responseString = response.JsonSerialize();

            _dataStore.Save(response.Id.ToString(), response.CreatedOn, strm.Content, response.GetType(),
                responseString);

            //this will run indefinitely until the cancelTokenSrc.Cancel() is called in the Action delegate
            if (!await handleStreamResponse(responseString))
                cancelTokenSrc.Cancel();
        }
    }
}