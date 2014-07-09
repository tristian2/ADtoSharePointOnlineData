using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.Profiles.MMD.Taxonomy
{
	public class TaxonomyTermSet: TaxonomyBaseItem
	{
		public IList<TaxonomyTerm> Terms { get; internal set; }

		public TaxonomyGroup Group { get; internal set; }

		public TaxonomyTermSet(Guid guid, string name, TaxonomyGroup group)
		{
			this.Group = group;
			this.Id = guid;
			this.Name = name;
			this.Terms = new List<TaxonomyTerm>();
		}
	}
}
