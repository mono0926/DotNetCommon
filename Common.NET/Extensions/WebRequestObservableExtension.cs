using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Mono.Framework.Common.Lib;

namespace Mono.Framework.Common.Extensions
{
    public static class WebRequestObservableExtension
    {
        private static Rss10FeedFormatter formatter;

        public static IObservable<string> SelectValue(this IObservable<XElement> observable)
        {
            return observable.Select(x => x.Value);
        }

        public static IObservable<XElement> GetResultObservable(this WebClient client)
        {
            var o = Observable.FromEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs>(
                    h => (sender, e) => h(e),
                    h => client.OpenReadCompleted += h,
                    h => client.OpenReadCompleted -= h)
                    .Select(e =>
                    {
                        if (e.Error != null)
                        {
                            throw e.Error;
                        }

                        return XElement.Load(e.Result);
                    });
            return o.Take(1);
        }

        [Obsolete]
        public static IObservable<XElement> GetResultObservableFoSecret(this WebClient client)
        {
            var o = Observable.FromEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs>(
                    h => (sender, e) => h(e),
                    h => client.OpenReadCompleted += h,
                    h => client.OpenReadCompleted -= h)
                    .Select(e =>
                    {
                        if (e.Error != null)
                        {
                            throw e.Error;
                        }

                        var reader = new StreamReader(e.Result);
                        return XElement.Parse(string.Join("\n", reader.ReadToEnd().Split(new char[] { '\n' }).Take(2)));
                    });
            return o.Take(1);
        }

        public static IObservable<SyndicationFeed> GetSydicationObservable(this WebClient client)
        {
            var o = Observable.FromEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs>(
                    h => (sender, e) => h(e),
                    h => client.OpenReadCompleted += h,
                    h => client.OpenReadCompleted -= h)
                    .Select(e =>
                    {
                        if (e.Error != null)
                        {
                            throw e.Error;
                        }

                        if (formatter == null)
                        {
                            formatter = new Rss10FeedFormatter();
                        }
                        formatter.ReadFrom(XmlReader.Create(e.Result));
                        var feed = formatter.Feed;
                        return feed;
                    }
                    );
            return o.Take(1);
        }

        public static IObservable<SyndicationFeed> GetSydicationAtomObservable(this WebClient client)
        {
            var o = Observable.FromEvent<OpenReadCompletedEventHandler, OpenReadCompletedEventArgs>(
                    h => (sender, e) => h(e),
                    h => client.OpenReadCompleted += h,
                    h => client.OpenReadCompleted -= h)
                    .Select(e =>
                    {
                        if (e.Error != null)
                        {
                            throw e.Error;
                        }
                        using (XmlReader reader = XmlReader.Create(e.Result))
                        {
                            return SyndicationFeed.Load(reader);
                        }
                    });
            return o.Take(1);
        }
    }
}