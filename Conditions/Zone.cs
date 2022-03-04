namespace QoLBar.Conditions
{
    public class ZoneCondition : ICondition, IConditionCategory
    {
        public string ID => "z";
        public string ConditionName => "Zone";
        public string CategoryName => "Zone";
        public int DisplayPriority => 0;
        public bool Check(dynamic arg) => DalamudApi.ClientState.TerritoryType == (ushort)arg;
    }
}