namespace TablesLibrary
{
    public class Tablet
    {
        private int ID { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string OSType { get; set; }

        public static Tablet Create(int id, string model, string serialNumber, string osType)
        {
            return new Tablet
            {
                ID = id,
                Model = model,
                SerialNumber = serialNumber,
                OSType = osType
            };
        }

        public string PrintObject()
        {
            return $"ID: {ID}, Model: {Model}, SerialNumber: {SerialNumber}, OSType: {OSType}";
        }
    }
}
