using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LinqToTwitter;
using Ninject.Extensions.Logging;

namespace SocialCommentaryApi.Service.Twitter
{
    public class TwitterSearch
    {
        private readonly ILogger _logger;
        private readonly TwitterContext _twitterCtx;

        public TwitterSearch(ILogger logger, TwitterContext twitterCtx)
        {
            _logger = logger;
            _twitterCtx = twitterCtx;
        }

        public async Task DoFilterStreamAsync(string query, Func<StreamContent, Task<bool>> handleStreamResponse)
        {
            _logger.Debug("Streamed Content:");
            var cancelTokenSrc = new CancellationTokenSource();

            try
            {
                await (from strm in _twitterCtx.Streaming.WithCancellation(cancelTokenSrc.Token)
                       where strm.Type == StreamingType.Filter && strm.Track == query
                       select strm)
                    .StartAsync(
                        async strm =>
                        {
                            // We're only interested in Status type content
                            if (strm.EntityType != StreamEntityType.Status)
                                return;

                            //this will run indefinitely until the cancelTokenSrc.Cancel() is called in the Action delegate
                            if (!await handleStreamResponse(strm))
                                cancelTokenSrc.Cancel();
                        });
            }
            catch (OperationCanceledException)
            {
                _logger.Debug("Stream cancelled.");
            }
        }
    }
}