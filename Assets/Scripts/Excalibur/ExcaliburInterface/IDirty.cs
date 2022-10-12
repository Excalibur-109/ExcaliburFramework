namespace Excalibur
{
    public interface IDirty : IExcalibur
    {
        // 刷新一次
        void SetDirty();
    }
}