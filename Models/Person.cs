using Newtonsoft.Json;

namespace Testament_App.Models
{
    public class Person
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateOnly Birthday { get; set; }

        [JsonIgnore]
        public List<Person> Children { get; set; } = new();

        [JsonIgnore]
        public List<Person> Parents { get; set; } = new();

		#region Parents and children as ids for pdf
		public List<int> ChildrenIds { get; set; } = [];
        public List<int> ParentIds { get; set; } = [];
        #endregion

        public decimal Percentage { get; set; }
		public bool IsAlive { get; set; }

		public void AddChild(Person child)
		{
			Children.Add(child);
			child.Parents.Add(this);

			ChildrenIds.Add(child.Id);
			child.ParentIds.Add(this.Id);
		}

		public void AddParent(Person parent)
		{
			Parents.Add(parent);
			parent.Children.Add(this);


			ParentIds.Add(parent.Id);
            parent.ChildrenIds.Add(this.Id);
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
