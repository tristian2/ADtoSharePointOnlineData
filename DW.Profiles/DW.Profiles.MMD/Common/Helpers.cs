using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using DW.Profiles.MMD.Taxonomy;

namespace DW.Profiles.MMD.Common
{
    public static class Helpers
    {
        public static List<TaxonomyTermSet> GetTermSetCollection(TaxonomyManager taxonomyManager, string termSetNames, bool splitByGroups, bool printToScreen)
        {
            List<TaxonomyTermSet> termSetCollection = new List<TaxonomyTermSet>();
            TaxonomyTermStore termStore = taxonomyManager.GetTermSets(termSetNames, 1033);

            foreach (var group in termStore.Groups)
            {
                if (splitByGroups && printToScreen)
                    System.Console.WriteLine("{0} =====", group.ToString().ToUpper());

                foreach (var termSet in group.TermSets)
                {
                    if (printToScreen)
                        PrintTermSet(termSet);

                    //because there can be multiple termsets in multiple groups
                    termSetCollection.Add(termSet);
                }
            }

            return termSetCollection;
        }

        public static TaxonomyTermSet GetTermSet(List<TaxonomyTermSet> termSetCollection, string termSetName)
        {
           TaxonomyTermSet ts = termSetCollection.Where(t => t.Name.ToLower() == termSetName.ToLower()).FirstOrDefault();           
          
            return ts;
        }

        private static void CreateTermInTermSetByTermSetName(TaxonomyManager taxonomyManager, string termSetName, string termName, bool printToScreen)
        {
            //Create a Term (if not exists already) in TermSet knowing just the termset name
            bool inserted = taxonomyManager.CreateTermInTermSet(termSetName, termName);

            if (printToScreen)
                System.Console.WriteLine("Term: {0} inserted in: {1} ? {2}", termName, termSetName, inserted);
        }

        private static void CreateTermInTermSetByTermSetId(TaxonomyManager taxonomyManager, Guid termSetId, string termName, bool printToScreen)
        {            
            bool inserted = taxonomyManager.CreateTermInTermSet(termSetId, termName);

            if (printToScreen)
                System.Console.WriteLine("Term: {0} inserted in: {1} ? {2}", termName, termSetId, inserted);
        }

        private static void GetTermInTermSetByTermSetId(TaxonomyManager taxonomyManager, Guid termSetId, string termName)
        {
            TaxonomyTerm term = taxonomyManager.GetTermInTermSet(termSetId, termName);

            if (term.IsNull)
                System.Console.WriteLine(String.Format("Term: {0} Not found in: {1}", termName, termSetId));
            else
                System.Console.WriteLine(String.Format("Term: {0} in: {1} Found!!", termName, termSetId));
        }

        private static void GetTermInTermSet(TaxonomyManager taxonomyManager, string termSetName, string termName)
        {
            bool excludeKeyword = true;

            TaxonomyTerm term = taxonomyManager.GetTermInTermSet(termSetName, termName, excludeKeyword);

            if (term.IsNull)
                System.Console.WriteLine(String.Format("Term: {0} Not found in: {1}", termName, termSetName));
            else
                System.Console.WriteLine(String.Format("Term: {0} in: {1} Found!!", termName, termSetName));
        }

        private static void GetFullDefaultTermStore(TaxonomyManager taxonomyManager)
        {
            System.Console.WriteLine("++++++++ Testing GetFullDefaultTermStore ++++++++");

            TaxonomyTermStore termStore = null;
            termStore = taxonomyManager.GetFullDefaultTermStore();

            foreach (var group in termStore.Groups)
            {
                System.Console.WriteLine("{0} =====", group.ToString().ToUpper());
                foreach (var termSet in group.TermSets)
                    PrintTermSet(termSet);
            }
        }

        private static void PrintTermSet(TaxonomyTermSet termSet)
        {
            System.Console.WriteLine("{0} -TS", termSet.ToString());
            foreach (var term in termSet.Terms)
            {
                Print(term.ToString(), 0);
                PrintTermChildren(term, 1);
            }
        }

        private static void PrintTermChildren(TaxonomyTerm term, int indent)
        {
            foreach (var childTerm in term.Terms)
            {
                Print(childTerm.ToString(), indent);
                indent = indent + 1;
                PrintTermChildren(childTerm, indent);
                indent = indent - 1;
            }
        }
        
        private static void Print(string name, int indent)
        {
            for (int i = 0; i < indent; i++)
                System.Console.Write(" ");

            System.Console.WriteLine(name);
        }
    }
}
