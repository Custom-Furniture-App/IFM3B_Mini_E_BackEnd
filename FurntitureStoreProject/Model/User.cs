namespace FurntitureStoreProject.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }    
        public string UserEmail { get; set; }
        public string UserPhone { get; set; } 
        public bool Admin {  get; set; }
    }
}
