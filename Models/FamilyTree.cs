namespace Testament_App.Models
{
	public class FamilyTree
	{
		public List<Person> Members { get; set; }
		public List<Marriage> Marriages { get; set; }

		public void AddMember(Person person)
		{
			Members.Add(person);
		}

		public void AddMarriage(Marriage marriage)
		{
			Marriages.Add(marriage);
		}

		//public Person FindPersonById(Guid id)
		//{
		//	return Members.FirstOrDefault(person => person.Id == id);
		//}

		//public TreeGraphStructure GetFamilyTreeAsGraph()
		//{

		//}
	}
}
