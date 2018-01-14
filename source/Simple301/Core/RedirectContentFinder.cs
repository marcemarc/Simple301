using Umbraco.Web.Routing;
using System.Linq;

namespace Simple301.Core
{
    /// <summary>
    /// Normally in Simple 301 Package this Content finder to be injected at the start of the Umbraco pipeline that first
    /// looks for any redirects that match the path + query
    /// this gives you the ability to setup a redirect for a url, before Umbraco intercepts it
    /// But this causea a potential problem with the Redirect Url Management Content Finder
    /// Because you 'Could' setup a redirect from a Url to an Umbraco page, that then subsequently could be renamed in Umbraco pointing to the same url you are redirecting from
    /// you would get an infinite redirect loop
    /// think you set up rule for /about-us to go to Umbraco page called 'about' with url /about and then this gets renamed in umbraco to about us /about-us ...
    /// because this is possible, Umbraco Turn off the Redirect Url Management tracking facility if Simple 301 is installed
    /// but Simple 301 doesn't do tracking like this, just simple redirects :-(
    /// If we position this ContentFinder to be queued 'After' the Redirect Url Management Content Finder, then this problem shouldn't occur
    /// as in the example the published /about-us/ page would be found before the fall through to this finder and any redirect rule for /about-us/
    /// much safer
    /// what this would mean though is you won't be able to setup a redirect for an Umbraco Url that is published, a published Umbraco Url will always take precedence
    /// these redirects will only be actions if no other content is found by a finder
    /// consequently it will be slightly slower to perform a redirect!
    /// but maybe this trade off is worthwhile if you can have the core tracking of changes and Simple 301 handling umm simple redirects
    /// </summary>
    public class RedirectContentFinder : IContentFinder
    {
        public bool TryFindContent(PublishedContentRequest request)
        {
            //Get the requested URL path + query
            var path = request.Uri.PathAndQuery.ToLower();

            //Check the table
            var matchedRedirect = RedirectRepository.FindRedirect(path);
            if (matchedRedirect == null) return false;

            //Found one, set the 301 redirect on the request and return
            request.SetRedirectPermanent(matchedRedirect.NewUrl);
            return true;
        }
    }
}
