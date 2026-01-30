namespace Tools {
    public interface IActivatable {
        /// <summary>Activates object after pulling out of pool</summary>
        public void Activate();
        /// <summary>Deactivates object before returning to pool</summary>
        public void Deactivate();
    }
}