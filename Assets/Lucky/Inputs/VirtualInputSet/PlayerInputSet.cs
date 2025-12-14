using Lucky.Inputs.VirtualInputs;

namespace Lucky.Inputs.VirtualInputSet
{
    /// <summary>
    /// VirtualInput 集合
    /// </summary>
    public class PlayerInputSet : InputSet
    {
        public PlayerInputSet(BindingSet bindingSet) : base(bindingSet)
        {
            Left = new VirtualButton(bindingSet.LeftBinding, 0);
            Right = new VirtualButton(bindingSet.RightBinding, 0);
            Up = new VirtualButton(bindingSet.UpBinding, 0);
            Down = new VirtualButton(bindingSet.DownBinding, 0);
            Jump = new VirtualButton(bindingSet.SpaceBinding, 0.1f);
            Dash = new VirtualButton(bindingSet.Mouse0Binding, 0.1f);
            Grab = new VirtualButton(bindingSet.Mouse1Binding, 0);
            
            MoveX = new VirtualIntegerAxis(bindingSet.LeftBinding, bindingSet.RightBinding);
            MoveZ = new VirtualIntegerAxis(bindingSet.DownBinding, bindingSet.UpBinding);
        }

        public VirtualButton Left;
        public VirtualButton Right;
        public VirtualButton Up;
        public VirtualButton Down;
        public VirtualButton Jump;
        public VirtualButton Dash;
        public VirtualButton Grab;


        public VirtualIntegerAxis MoveX;
        public VirtualIntegerAxis MoveZ;
    }
}