namespace OrderApi.Entities
{
    public class DevTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }

        public int Stage { get; set; }

        public bool isCompleted { get; set; }
    }
}
