using System.Collections.Generic;

namespace Lucky.Inputs.VirtualInputs
{
    public abstract class VirtualInput
    {
        public static HashSet<VirtualInput> VirtualInputs { get; } = new();

        public VirtualInput()
        {
            VirtualInputs.Add(this);
        }

        public abstract void Update(float deltaTime);
    }
}