namespace ooadproject.Models
{
    public class Teacher: Person
    {
        public string Title { get; set; }  

        public Teacher() { }

        public string GetFullName()
        {
            return Title + " " + this.FirstName + " " + this.LastName;    
        }
        
    }
}
