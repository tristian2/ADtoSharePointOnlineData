using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.Profiles.MMD.Taxonomy
{
	public class TaxonomyBaseItem
	{
		public string Name { get; set; }
		public Guid Id { get; set; }

		public override string ToString()
		{
			return String.Format("{0} ({1})", this.Name, this.Id);
		}
	}
}
