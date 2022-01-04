namespace TemplateWriterTests
{
    public partial class TransformTests
    {
        class Model
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public int Id;

            public Model(string firstName, string lastName)
            {
                this.FirstName = firstName;
                this.LastName = lastName;
            }
        }


    }
}
