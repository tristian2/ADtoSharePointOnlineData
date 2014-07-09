using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using DW.Profiles.Common;
using DW.Profiles.Extensions;
using DW.ClientOmAuth;

namespace DW.Profiles.MMD.Taxonomy
{

    public class TaxonomyManager
	{
		private ClientContext clientContext;
		private TaxonomySession taxonomySession;
        private String TermSetGroupName { get; set; }
        private String TermStoreGuid { get; set; }

		public TaxonomyManager(ClientContext clientContext) 
		{
			this.clientContext = clientContext;
			this.taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
		}

        public TaxonomyManager(string url, string user, string password, string TermStoreGuid, string TermSetGroupName)
		{
            this.TermStoreGuid = TermStoreGuid;
            Uri oUri = new Uri(url); //added by tob
            Office365ClaimsHelper claimsHelper = new Office365ClaimsHelper(url, user, password);
			this.clientContext = new ClientContext(oUri);
            this.clientContext.ExecutingWebRequest += claimsHelper.clientContext_ExecutingWebRequest;
			this.taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
		}

		public TaxonomyTermStore GetTermSets(string commaSeparatedTermSets, int lcid = 1033)
		{
            TaxonomyTermStore _termStore = new TaxonomyTermStore(new Guid(this.TermStoreGuid));
            TaxonomyGroup _group = new TaxonomyGroup(Guid.Empty, this.TermSetGroupName);
            
			_termStore.Groups.Add(_group);

			IList<string> termSetsToLoad = commaSeparatedTermSets.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

			foreach (var termSetName in termSetsToLoad)
			{                
                TaxonomyTermSet _termSet = new TaxonomyTermSet(Guid.Empty, termSetName.Trim(), _group);
                TermSetCollection termSets = this.taxonomySession.GetTermSetsByName(_termSet.Name, lcid);

                try
                {
                    this.clientContext.Load(termSets, tsetcol => tsetcol.Include(
                                                                            ts => ts.Id,
                                                                            ts => ts.Name,
                                                                            ts => ts.Terms.Include(
                                                                                                    t => t.Id,
                                                                                                    t => t.Name)));
                    this.clientContext.ExecuteQuery();
                }
                catch (ServerUnauthorizedAccessException suex)
                {
                    string message = "Check if the account has access to the termstore";
                    throw new Exception(message, suex);
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }


				if (!termSets.ServerObjectIsNull.Value && termSets.Count > 0)
				{
					TermSet termSet = termSets[0];
					_termSet.Id = termSet.Id;

					foreach (var term in termSet.Terms)
					{
						TaxonomyTerm _term = new TaxonomyTerm(term, _termSet);
					}

					_group.TermSets.Add(_termSet);
				}
			}

			return _termStore;
		}

		public TaxonomyTermStore GetFullDefaultTermStore()
		{
			TermStore defaultTermStore = this.taxonomySession.GetDefaultSiteCollectionTermStore();

			clientContext.Load(defaultTermStore, 
								ts => ts.Name, 
								ts => ts.Id, 
								ts => ts.Groups.Include(
												g => g.Id, 
												g => g.Name, 
												g => g.TermSets.Include(
																tset => tset.Id, 
																tset => tset.Name, 
																tset => tset.Terms.Include(
																					t => t.Id, 
																					t => t.Name))));

			clientContext.ExecuteQuery();

			if (!defaultTermStore.ServerObjectIsNull.Value)
			{
				TaxonomyTermStore _termStore = new TaxonomyTermStore(defaultTermStore.Id);

				foreach (var group in defaultTermStore.Groups)
				{
					TaxonomyGroup _group = new TaxonomyGroup(group.Id, group.Name);

					foreach (var termSet in group.TermSets)
					{
						TaxonomyTermSet _termSet = new TaxonomyTermSet(termSet.Id, termSet.Name, _group);

						foreach (var term in termSet.Terms)
						{
							TaxonomyTerm _term = new TaxonomyTerm(term, _termSet);
						}

						_group.TermSets.Add(_termSet);
					}

					_termStore.Groups.Add(_group);
				}

				return _termStore;

			}


			return new TaxonomyTermStore(Guid.Empty);
		}

		public TaxonomyTerm GetTermInTermSet(string termSetName, string termName, bool excludeKeyword)
		{
			TermCollection terms = this.taxonomySession.GetTermsInDefaultLanguage(termName, true, StringMatchOption.ExactMatch, 10, true, true);

			this.clientContext.Load(terms, tc => tc.Include(t => t.Id, t => t.Name, t => t.TermSet, t => t.IsKeyword, t => t.IsDeprecated));
			this.clientContext.ExecuteQuery();

			foreach (var term in terms)
			{
				if (term.TermSet.Name.Equals(termSetName, StringComparison.InvariantCultureIgnoreCase))
				{
					if (excludeKeyword == false || term.IsKeyword == false)
					{
						return new TaxonomyTerm(term.Id, term.Name);
					}					
				}
			}

			return new TaxonomyTerm(Guid.Empty, String.Empty);
		}

		public TaxonomyTerm GetTermInTermSet(Guid termSetId, string termName)
		{
			TermStore defaultTermStore = this.taxonomySession.GetDefaultSiteCollectionTermStore();
			TermSet termSet = defaultTermStore.GetTermSet(termSetId);

			LabelMatchInformation labelSearchCriteria = new LabelMatchInformation(this.clientContext);
			labelSearchCriteria.TermLabel = termName;
			labelSearchCriteria.TrimUnavailable = true;
			TermCollection terms = termSet.GetTerms(labelSearchCriteria);

			this.clientContext.Load(terms, tc => tc.Include(t => t.Id, t => t.Name, t => t.TermSet, t => t.IsKeyword, t => t.IsDeprecated));
			this.clientContext.ExecuteQuery();

			if (!terms.ServerObjectIsNull.Value && terms.Count > 0)
			{
				Term term = terms[0];
				return new TaxonomyTerm(term.Id, term.Name);
			}

			return new TaxonomyTerm(Guid.Empty, String.Empty);
		}

		public bool CreateTermInTermSet(Guid termSetId, string termName, string newTermId = "", int lcid = 1033)
		{
			TaxonomyTerm term = GetTermInTermSet(termSetId, termName);

			if (term.IsNull)
			{
				TermStore defaultTermStore = this.taxonomySession.GetDefaultSiteCollectionTermStore();
				TermSet termSet = defaultTermStore.GetTermSet(termSetId);
				Guid termId = Guid.NewGuid();
				if (!String.IsNullOrEmpty(newTermId))
				{
					termId = new Guid(newTermId);
				}
				Term newTerm = termSet.CreateTerm(termName, lcid, termId);

				this.clientContext.Load(newTerm);

				defaultTermStore.CommitAll();
				clientContext.ExecuteQuery();

				return true;
			}

			return false;
		}

		public bool CreateTermInTermSet(string termSetName, string termName, string newTermId = "", int lcid = 1033)
		{
			TermSetCollection termSets = this.taxonomySession.GetTermSetsByName(termSetName, lcid);
			this.clientContext.Load(termSets);
			this.clientContext.ExecuteQuery();

			if (!termSets.ServerObjectIsNull.Value && termSets.Count > 0)
			{
				TermSet termSet = termSets[0];				

				return CreateTermInTermSet(termSet.Id, termName, newTermId, lcid);
			}

			return false;
		}
	}
}
