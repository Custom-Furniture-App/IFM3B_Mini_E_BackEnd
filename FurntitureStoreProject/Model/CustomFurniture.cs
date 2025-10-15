namespace FurntitureStoreProject.Model
{
    public class CustomFurniture
    {
        public  int CustomFurnitureId { get;set; }
        public int CartId { get;set; }
        public int CustomFurnitureQuantity { get;set; }
        public double CustomFurniturePrice { get;set; }
        public int MaterialColourId { get;set; }
        public int FurnitureBaseId { get;set; } 
        public List<int> ComponentIdList { get;set; }
    }
}
