namespace FurntitureStoreProject.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public DateTime CartCreationDate { get; set; }
        public string CartStatus { get; set; }  
        public int UserId { get; set; }

    }
}
