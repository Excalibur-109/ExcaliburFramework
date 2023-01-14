using UnityEditor.IMGUI.Controls;

namespace Excalibur
{
    public class ExcelTreeItem : TreeViewItem
    {
        string excelName;

        public string ExcelName => excelName;

        public ExcelTreeItem(int id, string excelName) : base(id)
        {
            depth = 0;
            this.excelName = excelName;
            displayName = excelName;
        }
    }
}
