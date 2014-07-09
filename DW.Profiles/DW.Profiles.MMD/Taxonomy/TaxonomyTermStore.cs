using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.Profiles.MMD.Taxonomy
{
	public class TaxonomyTermStore: TaxonomyBaseItem
	{
		public IList<TaxonomyGroup> Groups { get; internal set; }

		internal TaxonomyTermStore(Guid id)
		{
			this.Id = id;            
			this.Groups = new List<TaxonomyGroup>();			
		}
	}
}
