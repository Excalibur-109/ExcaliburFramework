namespace Excalibur
{
    public interface IForm : IExcalibur
    {
        void OnInitialzed(FormType formType);
        FormType FormType { get; }
        bool IsActive { get; }
        void ActivteInOut();
        void ForceActivateInOut();
        void Open();
        void Close();
        void SetAsFirstSibling();
    }
}