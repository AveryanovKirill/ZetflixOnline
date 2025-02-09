namespace TemplateEngine.UnitTests.Models
{
    public class Person
    {
        public string Login { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool Gender { get; set; }

        public Group Group { get; set; }
    }

    public class Group
    {
        public int Name { get; set; }
    }

}
