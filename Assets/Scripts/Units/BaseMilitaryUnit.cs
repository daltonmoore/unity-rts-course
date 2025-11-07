namespace Units
{
    public class BaseMilitaryUnit : AbstractUnit, ITransportable
    {
        public int TransportCapacityUsage => _unitSO.TransportConfig.GetTransportCapacityUsage();
        
        public void LoadInto(ITransporter transporter)
        {
            throw new System.NotImplementedException();
        }
    }
}