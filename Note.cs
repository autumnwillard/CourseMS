using SQLite;

namespace C971
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Text { get; set; }
        public int CourseId { get; set; }
    }
}
