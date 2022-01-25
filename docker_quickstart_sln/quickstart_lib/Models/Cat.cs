namespace quickstart_lib.Models
{
    public class Cat
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> NickNames { get; set; }
    }
}
