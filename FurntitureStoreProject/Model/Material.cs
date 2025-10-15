namespace FurntitureStoreProject.Model
{
    public class Material
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public double MaterialPrice { get;set; }
        public List<Colour> ColourList { get; set; }
    }
}
