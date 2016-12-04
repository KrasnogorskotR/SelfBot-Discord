using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfBot.Common.Types
{
    public class Pages
    {
        List<string> Titles;
        List<string> Descriptions;
        int MaxItems;
        public int NumberOfPages;

        public Pages(List<string> titles, List<string> descriptions, int maxItems)
        {
            Titles = titles;
            Descriptions = descriptions;
            MaxItems = maxItems;
            NumberOfPages = (int)Math.Ceiling(((double)titles.Count) / maxItems);
        }

        public Page this[int index]
        {
            get
            {
                return GetPages(index);
            }
        }

        private Page GetPages(int index)
        {
            Page page = new Page();
            int start = MaxItems * index;

            for (int i = start; i < (start + MaxItems); i++)
            {
                try
                {
                    page.Title.Add(Titles[i]);
                    page.Description.Add(Descriptions[i]);
                }
                catch { }
            }

            return page;
        }
    }

    public class Page
    {
        public List<string> Title { get; set; }
        public List<string> Description { get; set; }

        public Page()
        {
            Title = new List<string>();
            Description = new List<string>();
        }
    }
}
