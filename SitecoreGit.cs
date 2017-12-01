using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Data;
using Sitecore.FakeDb;
using Siteore.Fakedb.Test;
using Siteore.Fakedb.Test.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siteore.Fakedb.Tests
{
	[TestClass]
	public class SitecoreGit
	{

		private Db db = new Db();

		[TestMethod]
		public void should_return_author_byline_for_a_japanese_article()
		{
			var author1 = ItemBuilder.anItem(db)
									.havingName("Author1")
									.havingSitecoreTemplateID(ID.Parse(ID.NewID))
									.havingField("Title", "田中 正朗")
									.havingLanguages("ja")
									.BuildDBItemWithFields();

			var author2 = ItemBuilder.anItem(db)
									   .havingName("Author2")
									   .havingSitecoreTemplateID(ID.NewID)
									   .havingField("Title", "田中 正朗")
									   .havingLanguages("ja")
										.BuildDBItemWithFields();

			var article = ItemBuilder.anItem(db)
									 .havingName(@"アジア太平洋地域におけるプライベートエクイティ: リバウンド、グローバリゼーション、その他の動き")
									 .havingSitecoreID(Guid.NewGuid().ToString())
									 .havingMultiListField("Authors", author1, author2)
									 .havingLanguages("ja")
									 .BuildDBItemWithFields();

			var authorAPI = new AuthorAPI(db);
			//Both Authors empty
			var authors = authorAPI.GetAuthors(article);

			Assert.IsNotNull(authors);
			Assert.AreEqual("田中 正朗", authors[0]);
		}

		[TestMethod]
		public void should_return_author_byline_for_an_article_in_case_of_more_than_two_authors()
		{

			var author1 = ItemBuilder.anItem(db)
									.havingSitecoreTemplateID(ID.NewID)
									.havingName("Author1")
									.havingField("Title", "David Chinn")
									.BuildDBItemWithFields();

			var author2 = ItemBuilder.anItem(db)
									.havingSitecoreTemplateID(ID.NewID)
									.havingName("Author2")
									.havingField("Title", "Ondrej Burkacky")
									.BuildDBItemWithFields();


			var author3 = ItemBuilder.anItem(db)
									.havingSitecoreTemplateID(ID.NewID)
									.havingName("Author3")
									.havingField("Title", "Kevin Dehoff")
									.BuildDBItemWithFields();

			var article = ItemBuilder.anItem(db)
									 .havingName(@"Internaltional Defense Sales")
									 .havingSitecoreID(Guid.NewGuid().ToString())
									 .havingMultiListField("Authors", author1, author2, author3)
									 .BuildDBItemWithFields();

			var authorAPI = new AuthorAPI(db);
			//Both authors same as given above
			var articleByline = authorAPI.GetAuthors(article);


			Assert.IsNotNull(articleByline);

		}
	}
}
