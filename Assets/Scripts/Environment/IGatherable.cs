namespace Environment
{
    public interface IGatherable
    {
        public SupplySO SupplySO { get; }
        public int Amount { get; }
        public bool IsBusy { get; }

        public bool BeginGather();
        public int EndGather();
    }
}