namespace Testament_App.Models
{
	public class Person
	{
		public string Name { get; set; }
		public DateOnly Birthday { get; set; }
		public List<Person> Children { get; set; } = new();
		public List<Person> Parents { get; set; } = new();
		public decimal Percentage { get; set; }
		public bool IsAlive { get; set; }

		public Person()
		{
			
		}

		public void AddChild(Person child)
		{
			Children.Add(child);
		}

		public void AddParent(Person parent)
		{
			Parents.Add(parent);
		}

		public List<Person> GetAncestors()
		{
			return Parents;
		}

		public List<Person> GetChildren()
		{
			return Children;
		}
	}
}
