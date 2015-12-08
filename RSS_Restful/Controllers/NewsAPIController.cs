using HtmlAgilityPack;
using RSS_Restful.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Web.Http;

namespace RSS_Restful.Controllers
{
    public class NewsAPIController : ApiController
    {
        [HttpGet]
        [Route("api/news/all")]
        public Rss20FeedFormatter GetAll()
        {
            SyndicationFeed feed = new SyndicationFeed
            {
                Title = new TextSyndicationContent("My Feed")
            };
            feed.Authors.Add(new SyndicationPerson("NPC"));
            feed.Description = new TextSyndicationContent("Test rss");

            List<SyndicationItem> ls = BuildSyndicationItem(GetNews());
            feed.Items = ls;
            return new Rss20FeedFormatter(feed);
        }

        //[HttpGet]
        //[Route("api/news/test")]
        //public SyndicationFeed GetTest()
        //{
        //    SyndicationFeed feed = new SyndicationFeed
        //    {
        //        Title = new TextSyndicationContent("My Feed")
        //    };

        //    List<SyndicationItem> ls = BuildSyndicationItem(GetNews());
        //    feed.Items = ls;

        //    Rss20FeedFormatter rss = new Rss20FeedFormatter(feed);
            
        //    return rss.Feed;
        //}

        private List<NewsModel> GetNews()
        {
            List<NewsModel> ln = new List<NewsModel>();

            string index = "http://www.fit.hcmus.edu.vn/vn/";
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            HtmlWeb htmlWeb = new HtmlWeb();
            htmlDoc = htmlWeb.Load(index);

            HtmlNodeCollection day_month = htmlDoc.DocumentNode.SelectNodes("//td[@class=\"day_month\"]");
            HtmlNodeCollection post_year = htmlDoc.DocumentNode.SelectNodes("//td[@class=\"post_year\"]");
            HtmlNodeCollection post_title = htmlDoc.DocumentNode.SelectNodes("//td[@class=\"post_title\"]/a");

            if (post_title != null)
            {
                for (int i = 0; i < post_title.Count; i++)
                {
                    NewsModel news = new NewsModel();
                    news.Id = i;
                    news.Title = post_title[i].Attributes["title"].Value;
                    news.Address = index + post_title[i].Attributes["href"].Value;
                    news.CreateAt = new DateTime(int.Parse(post_year.ElementAt(i).InnerText), int.Parse(day_month.ElementAt(2 * i + 1).InnerText), int.Parse(day_month.ElementAt(2 * i).InnerText));

                    ln.Add(news);
                }
            }

            return ln;
        }

        private List<SyndicationItem> BuildSyndicationItem(List<NewsModel> ln)
        {
            List<SyndicationItem> items = new List<SyndicationItem>();
            foreach (NewsModel news in ln)
            {
                SyndicationItem item = new SyndicationItem()
                {
                    Title = new TextSyndicationContent(news.Title),
                    BaseUri = new Uri(news.Address),
                    LastUpdatedTime = news.CreateAt
                };
                items.Add(item);
            }
            return items;
        }
    }
}
