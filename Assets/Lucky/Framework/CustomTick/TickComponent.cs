namespace Lucky.Framework.CustomTick
{
    public class TickComponent
    {
        public bool Active;
        public TickBehaviour parent { get; private set; }

        public TickComponent(bool active = true)
        {
            Active = active;
        }

        public virtual void Added(TickBehaviour entity)
        {
            parent = entity;
        }


        public virtual void Removed(TickBehaviour entity)
        {
            parent = null;
        }

        public virtual void Tick()
        {
        }

        public void RemoveSelf()
        {
            if (parent != null)
            {
                parent.Remove(this);
            }
        }
    }
}