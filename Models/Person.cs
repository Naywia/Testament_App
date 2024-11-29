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
			// Console.WriteLine($"Add child with name: {child.Name}");
			Children.Add(child);
			child.Parents.Add(this);
		}

		public void AddParent(Person parent)
		{
			// Console.WriteLine($"Add parent with name: {parent.Name}");
			Parents.Add(parent);
			parent.Children.Add(this);
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
