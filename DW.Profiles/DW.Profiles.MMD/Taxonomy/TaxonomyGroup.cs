using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.Profiles.MMD.Taxonomy
{
	public class TaxonomyGroup: TaxonomyBaseItem
	{
		internal TaxonomyGroup(Guid guid, string name)
		{
			this.Id = guid;
			this.Name = name;
			this.TermSets = new List<TaxonomyTermSet>();
		
        }

		public Guid TermStoreId { get; set; }

		public IList<TaxonomyTermSet> TermSets { get; internal set; } 
		
	}
}
