namespace Models.DataServiceModels
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string UniqueName { get; set; }
        public byte[] ImageByteArray { get; set; }
        public string TypeOfModel { get; set; }
    }
}
