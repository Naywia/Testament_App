namespace Testament_App.Models
{
	public class Marriage
	{
		public Person Partner1 { get; set; }
		public Person Partner2 { get; set; }
        public DateOnly Date { get; set; }
    }
}
