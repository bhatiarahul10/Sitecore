using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Siteore.Fakedb.Test.Common
{
	public class ItemBuilder : IDisposable
	{
		private string _name;
		private Item _parent;
		private string _fieldName;
		private string _sitecoreID;
		private ID _sitecoreTemplateID;
		Db _siteCoreDb;
		private string _fieldValue;
		private string languages;
		private Dictionary<string, string> _fields = new Dictionary<string, string>();
		private Dictionary<string, List<Item>> _multiListfields = new Dictionary<string, List<Item>>();

		public ItemBuilder(Db db)
		{
			_siteCoreDb = db;
		}

		public static ItemBuilder anItem(Db db)
		{
			return new ItemBuilder(db);
		}

		public ItemBuilder havingName(string name)
		{
			_name = name;
			return this;
		}

		public ItemBuilder havingLanguages(string languages)
		{
			this.languages = languages;
			return this;
		}

		public ItemBuilder havingField(string fieldName, string value)
		{
			_fields.Add(fieldName, value);
			return this;
		}

		public ItemBuilder havingFieldName(string fieldName)
		{
			_fieldName = fieldName;
			return this;
		}

		public ItemBuilder havingFieldValue(string value)
		{
			_fieldValue = value;
			return this;
		}


		public ItemBuilder havingMultiListField(string fieldName, params Item[] items)
		{
			_multiListfields.Add(fieldName, items.ToList<Item>());
			return this;
		}

		public ItemBuilder havingSitecoreID(string sitecoreID)
		{
			_sitecoreID = sitecoreID;
			return this;
		}

		public Item BuildDBItemWithFields()
		{
			var tempSitecoreId = _sitecoreTemplateID ?? ID.NewID;
			CreateTemplate(tempSitecoreId);

			var dbItem = new DbItem(_name, ID.NewID, tempSitecoreId);
			if (!string.IsNullOrEmpty(languages))
			{
				using (new LanguageSwitcher(languages))
				{
					_siteCoreDb.Add(dbItem);
				}
			}
			else
				_siteCoreDb.Add(dbItem);

			var item = _siteCoreDb.Database.GetItem(dbItem.ID, Language.Parse(languages ?? "en"));
			SetFields(item);
		
			return item;
		}

		private void CreateTemplate(ID tempSitecoreId)
		{
			if (ID.IsNullOrEmpty(_sitecoreTemplateID) || _siteCoreDb.GetItem(_sitecoreTemplateID) == null)
			{
				var itemTemplate = new DbTemplate("Base Template", tempSitecoreId);

				foreach (var f in _fields)
				{
					itemTemplate.Add(new DbField(f.Key, ID.NewID));
				}

				foreach (var f in _multiListfields)
				{
					var field = new DbField(f.Key, ID.NewID) { Type = "Multilist" };
					itemTemplate.Add(field);
				}

				_siteCoreDb.Add(itemTemplate);
			}
		}

		private void SetFields(Item item)
		{
			using (new SecurityDisabler())
			{
				item.Editing.BeginEdit();
				foreach (var f in _fields)
				{
					item.Fields[f.Key].Value = f.Value;
				};

				foreach (var f in _multiListfields)
				{
					MultilistField mf = item.Fields[f.Key];
					foreach (var v in f.Value)
					{
						mf.Add(v.ID.ToString());
					}
				}
				item.Editing.EndEdit();
			}
		}

		public ItemBuilder havingSitecoreTemplateID(ID sitecoreTemplateID)
		{
			_sitecoreTemplateID = sitecoreTemplateID;
			return this;
		}

		public void Dispose()
		{
			_siteCoreDb = null;
		}
	}
}
