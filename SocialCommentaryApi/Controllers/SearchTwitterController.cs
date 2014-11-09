using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SocialCommentaryApi.Service.Twitter;
using StreamContent = LinqToTwitter.StreamContent;

namespace SocialCommentaryApi.Controllers
{
    [RoutePrefix("api/twitter")]
    public class TwitterController : ApiController
    {
        private readonly TwitterSearch _twitterSearch;

        public TwitterController(TwitterSearch twitterSearch)
        {
            _twitterSearch = twitterSearch;
        }

        // GET: api/SearchTwitter/5
        [Route("{query}/{count=10}")]
        public HttpResponseMessage Get(string query, int count)
        {
            var response = Request.CreateResponse();
            int[] ctr = { 0 };
            // Create push content with a delegate that will get called when it is time to write out 
            // the response.
            response.Content = new PushStreamContent(
                async (outputStream, httpContent, transportContext) =>
                {
                    try
                    {
                        await _twitterSearch.DoFilterStreamAsync(query, async arg =>
                        {
                            var buffer =
                                Encoding.UTF8.GetBytes(string.Format("{0}{1}", (ctr[0] == 0 ? "[" : ","), arg.Content.Replace("\n"," ")));
                            // Write out data to output stream
                            await outputStream.WriteAsync(buffer, 0, buffer.Length);
                            return ctr[0] != count && ++ctr[0] != count;
                        });
                    }
                    catch (HttpException ex)
                    {
                        if (ex.ErrorCode == -2147023667) // The remote host closed the connection. 
                            ctr[0] = count;
                    }
                    finally
                    {
                        // Close output stream as we are done
                        var buffer = Encoding.UTF8.GetBytes("]");
                        outputStream.WriteAsync(buffer, 0, buffer.Length).ContinueWith(c => outputStream.Close());
                    }
                });

            return response;
        }
    }
}
