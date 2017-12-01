using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Siteore.Fakedb.Test
{
	public class AuthorAPI
	{
		private const string AuthorTitleField = "Title";
		private const string AuthorAliasTitleField = "Aliased Name";
		private const string ArticleBylinePreposition = "By";
		private const string CommaSeperator = ",";
		private const string SemicolonSeperator = ";";
		private Db _sitecoreFakeDb;
		public AuthorAPI(Db sitecoreFakeDb)
		{
			_sitecoreFakeDb = sitecoreFakeDb;
		}

		public IList<string> GetAuthors(Item article)
		{
			var ctxtLang = article.Language;
			var authorField = ((MultilistField)article.Fields["Authors"]);
			return authorField.GetItems().Select(a =>
			{
				var contextItem = _sitecoreFakeDb.GetItem(a.ID, ctxtLang.Name);
				return contextItem.Fields["Title"].Value;
			}).ToList();
		}

	}
}
