namespace Excalibur
{
    public class DirtyBase : IDirty
    {
        public DirtyBase()
        {
            SetDirty();
        }

        public void SetDirty()
        {
            Dirty();
        }

        protected virtual void Dirty()
        {

        }
    }
}
