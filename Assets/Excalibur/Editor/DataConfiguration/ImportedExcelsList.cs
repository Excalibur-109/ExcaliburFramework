using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Excalibur
{
	public class ImportedExcelsList : TreeView
    {
        TreeViewState state;
        DataConfigurationEditorWin parent;

        public ImportedExcelsList(TreeViewState state, DataConfigurationEditorWin parent) : base(state)
        {
            this.state = state;
            this.parent = parent;
            parent.data_SO.onReloadImportTree -= Reload;
            parent.data_SO.onReloadImportTree += Reload;
            Reload();
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem { id = -1, depth = -1 };
            if (parent.data_SO.ImportedExcels.Count == 0)
            {
                root.AddChild(new TreeViewItem { id = 0, depth = -1 });
            }
            else
            {
                for (int i = 0; i < parent.data_SO.ImportedExcels.Count; ++i)
                {
                    ExcelTreeItem newItem = new ExcelTreeItem(i, parent.data_SO.ImportedExcels[i].filename);
                    root.AddChild(newItem);
                }
            }
            return root;
        }

        protected override void ContextClickedItem(int id)
        {
            TreeViewItem item = FindItem(id, rootItem);
            if (item is ExcelTreeItem)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete"), false, DeleteImportExcel, item);
                menu.AddItem(new GUIContent("Open"), false, OpenImportedExcel, item);
                menu.ShowAsContext();
            }
        }

        private void DeleteImportExcel(object context)
        {
            IList<int> selection = new List<int>();
            if (selection.Count > 1)
            {
                for (int i = 0; i < selection.Count; ++i)
                {
                    ExcelTreeItem item = FindItem(selection[i], rootItem) as ExcelTreeItem;
                    parent.data_SO.RemoveImportExcel(item.ExcelName);
                }
                Reload();
            }
            else
            {
                ExcelTreeItem item = context as ExcelTreeItem;
                parent.data_SO.RemoveImportExcel(item.ExcelName);
                Reload();
            }
        }

        private void OpenImportedExcel(object context)
        {
            ExcelTreeItem item = context as ExcelTreeItem;
            string path = parent.data_SO.GetImportedExcel(item.ExcelName);
            SerializeUtility.OpenFile(path);
        }
    }
}
