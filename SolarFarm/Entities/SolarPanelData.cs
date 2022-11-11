namespace SolarFarm.Entities
{
    public class SolarPanelData
    {
        public Guid Id { get; set; }
        public Guid PanelId { get; set; }
        public int EnergyGeneratedKwh { get; set; }
        public int PowerGenerated { get; set; }
        public int VoltageGenerated { get; set; }
        public DateTimeOffset Time { get; set; }

        public virtual SolarPanel SolarPanel { get; set; }
    }
}
