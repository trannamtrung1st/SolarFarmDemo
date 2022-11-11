namespace SolarFarm.Entities
{
    public class SolarPanel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SolarPanelData> Data { get; set; }
    }
}
