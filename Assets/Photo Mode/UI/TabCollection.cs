using System.Collections.Generic;
using System.Linq;

namespace PhotoMode.UI
{
	public class TabCollection
	{
		public class Tab
		{
			private string name;
			public int id;

			public string Name { get => name; set => name = value; }

			public Tab(string name, int id)
			{
				this.Name = name;
				this.id = id;
			}
		}

		List<Tab> tabs = new List<Tab>();
		//Dictionary<Tab, Tab> tabDictionary = new Dictionary<Tab, Tab>();

		public List<Tab> Tabs { get => tabs; set => tabs = value; }

		public int Count => tabs.Count;

		public void AddTabs(IEnumerable<string> tabs)
		{
			this.tabs.AddRange(
				tabs.Select((category, num) => new Tab(category, num))
			);
			//tabDictionary = this.tabs.ToDictionary(tab => tab);
		}

		public Tab FindTabByName(string name)
		{
			return tabs.Find(t => t.Name == name);
		}

		public Tab FindTabById(int id)
		{
			return tabs.Find(t => t.id == id);
		}

		public static implicit operator List<Tab>(TabCollection tabs)
		{
			return tabs.tabs;
		}
	}
}