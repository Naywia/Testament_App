﻿namespace Testament_App.Models
{
	public static class FamilyTree
	{
		private static List<Person> Members = new();
		private static List<Marriage> Marriages = new();

		public static void AddMember(Person person)
		{
			Members.Add(person);
		}
		
		public static List<Person> GetMembers()
		{
			return Members;
		}

		public static void RemoveMember(Person person)
		{
			Members.Remove(person);
		}

		public static void AddMarriage(Marriage marriage)
		{
			Marriages.Add(marriage);
		}
        public static List<Marriage> GetMarriages()
        {
            return Marriages;
        }

        //public TreeGraphStructure GetFamilyTreeAsGraph()
        //{

        //}
    }
}
