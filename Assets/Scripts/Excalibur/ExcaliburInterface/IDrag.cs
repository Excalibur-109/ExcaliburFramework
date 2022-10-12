using UnityEngine.EventSystems;

namespace Excalibur
{
    public interface IDragHandle : IBeginDragHandler, IDragHandler, IEndDragHandler
    {

    }

    public interface IClickHandle : IPointerClickHandler
    {

    }
}
