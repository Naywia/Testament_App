namespace Testament_App.Models
{
    public class PersonNode
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<int> Children { get; set; }

        public List<int> Partners { get; set; }

        public bool Root { get; set; }

        public int Level { get; set; }

        public List<int> Parents { get; set; }
    }
}
