namespace JackHenryChallenge.Models.Twitter
{
    public class Field
    {
        public static Field Entities { get; } = new Field("entities");

        public string Name { get; private set; }

        private Field(string name)
        {
            Name = name;
        }
    }
}
