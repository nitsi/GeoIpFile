namespace Kubernetes1.Model
{
    public class GeoResult
    {
        public GeoResult(string country, string city)
        {
            Country = country;
            City = city;
        }

        public string City { get; set; }
        public string Country { get; set; }
    }
}