using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace DW.Profiles.MMD.Taxonomy
{
	public class TaxonomyTerm: TaxonomyBaseItem
	{
		public Guid TermStoreId { get; set; }
		public IList<TaxonomyTerm> Terms { get; internal set; }
		public TaxonomyTermSet TermSet { get; internal set; }
		public TaxonomyTerm Parent { get; internal set; }
		public bool IsNull
		{
			get
			{
				return this.Id.Equals(Guid.Empty) && this.Name.Equals(String.Empty);
			}
		}

		public TaxonomyTerm(Guid id, string name)
		{
			this.Id = id;
			this.Name = name;
		}

		public TaxonomyTerm(Term term, TaxonomyTermSet termSet)
		{
			this.Id = term.Id;
			this.Name = term.Name;
			this.TermSet = termSet;
			this.Terms = new List<TaxonomyTerm>();

			termSet.Terms.Add(this);

			LoadAllChildTerms(term, this);
		}

		public TaxonomyTerm(Term term, TaxonomyTerm parentTerm)
		{
			this.Id = term.Id;
			this.Name = term.Name;
			this.TermSet = null;
			this.Terms = new List<TaxonomyTerm>();

			parentTerm.Terms.Add(this);
		}

		private void LoadAllChildTerms(Term term, TaxonomyTerm parentTerm)
		{
			term.Context.Load(term.Terms, tc => tc.Include(t => t.Id, t => t.Name));
			term.Context.ExecuteQuery();

			foreach (var childTerm in term.Terms)
			{
				var _childTerm = new TaxonomyTerm(childTerm, parentTerm);

				LoadAllChildTerms(childTerm, _childTerm);
			}
		}
	}
}
