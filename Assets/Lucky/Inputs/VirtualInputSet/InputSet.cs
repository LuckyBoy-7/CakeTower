namespace Lucky.Inputs.VirtualInputSet
{
    /// <summary>
    /// VirtualInput 集合
    /// </summary>
    public class InputSet
    {
        protected BindingSet bindingSet;

        public InputSet(BindingSet bindingSet)
        {
            this.bindingSet = bindingSet;
        }
    }
}