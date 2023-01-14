
namespace Excalibur
{
    public enum DisplayeType
    {
        /// <summary>
        /// 再界面常驻，无法关掉
        /// </summary>
        DT_RESIDENT,
        /// <summary>
        /// 可以显示隐藏
        /// </summary>
        DT_DYNAMIC,
    }

    public enum DisplaySubType
    {
        /// <summary>
        /// 显示
        /// </summary>
        DST_OPEN,
        /// <summary>
        /// 隐藏
        /// </summary>
        DST_CLOSE,
    }

    public enum InOutMoveType
    {
        DTM_NONE,
        DTM_MOVE_HORIZONTAL_RIGHT,
        DTM_MOVE_HORIZONTAL_LEFT,
        DTM_MOVE_VERTICAL_UP,
        DTM_MOVE_VERTICAL_DOWN,
    }

    public enum InOutRotateType
    {
        DTR_NONE,
        DTR_HORIZONTAL,
        DTR_VERTICAL,
    }

    public enum InOutScaleType
    {
        DTS_NONE,
        DTS_SCALE_ZERO_ONE,
        DTS_SCALE_ZERO_JELLY,
    }

    public enum InOutAlphaType
    {
        DTA_NONE,
        DTA_ALPHA_ZERO_ONE,
    }

    public enum BlockRaycastType
    {
        BRT_NONE,
        BRT_BLOCK,
    }
}